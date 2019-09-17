using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Primitives;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Hosting
{
    // TODO: Add IHostingEnvironmentAccessor and IHttpContextAccessor abstractions as per ASP.NET Core
    // so that this class can be fully tested.
	// TODO: Find all usage of the stackalloc keyword and allocate to the heap where the length of data we are dealing
	// with is below a certain value, e.g. 128. For all other instances, allocate to the heap to avoid stack overflow.
    public class UmbrellaWebHostingEnvironment : UmbrellaHostingEnvironment, IUmbrellaWebHostingEnvironment
    {
		#region Constructors
		public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
            IMemoryCache cache,
            ICacheKeyUtility cacheKeyUtility)
            : base(logger, cache, cacheKeyUtility)
        {
			var fileProvider = new PhysicalFileProvider(MapPath("~/"));

			ContentRootFileProvider = fileProvider;
			WebRootFileProvider = fileProvider;
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public override string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

			if (!fromContentRoot)
				throw new ArgumentException("This value must always be true in a classic .NET application. It can only be set to false inside a .NET Core application.", nameof(fromContentRoot));

			string[] cacheKeyParts = null;

			try
            {
				cacheKeyParts = ArrayPool<string>.Shared.Rent(2);
				cacheKeyParts[0] = virtualPath;
				cacheKeyParts[1] = fromContentRoot.ToString();

                string key = CacheKeyUtility.Create<UmbrellaWebHostingEnvironment>(cacheKeyParts, 2);

                return Cache.GetOrCreate(key, entry =>
                {
					// TODO: Move to an Options class!
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                    string cleanedPath = TransformPath(virtualPath, true, false, false);

                    return System.Web.Hosting.HostingEnvironment.MapPath(cleanedPath);
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot }, returnValue: true))
            {
				throw new UmbrellaWebException("There has been a problem mapping the specified virtual path.", exc);
            }
			finally
			{
				if (cacheKeyParts != null)
					ArrayPool<string>.Shared.Return(cacheKeyParts);
			}
        }
		#endregion

		#region IUmbrellaWebHostingEnvironment Members
		public virtual string MapWebPath(string virtualPath, bool toAbsoluteUrl = false, string scheme = "http", bool appendVersion = false, string versionParameterName = "v", bool mapFromContentRoot = true, bool watchWhenAppendVersion = true)
        {
            Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));
            Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));
            Guard.ArgumentNotNullOrWhiteSpace(versionParameterName, nameof(versionParameterName));

			string[] cacheKeyParts = null;

			try
            {
				cacheKeyParts = ArrayPool<string>.Shared.Rent(7);
				cacheKeyParts[0] = virtualPath;
				cacheKeyParts[1] = toAbsoluteUrl.ToString();
				cacheKeyParts[2] = scheme;
				cacheKeyParts[3] = appendVersion.ToString();
				cacheKeyParts[4] = versionParameterName;
				cacheKeyParts[5] = mapFromContentRoot.ToString();
				cacheKeyParts[6] = watchWhenAppendVersion.ToString();

                string key = CacheKeyUtility.Create<UmbrellaWebHostingEnvironment>(cacheKeyParts, 7);

                return Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);
                    
                    string cleanedPath = TransformPath(virtualPath, false, true, true);

                    string virtualApplicationPath = HttpRuntime.AppDomainAppVirtualPath != "/"
                        ? HttpRuntime.AppDomainAppVirtualPath
                        : "";

                    //Prefix the path with the virtual application segment but only if the cleanedPath doesn't already start with the segment
                    string url = cleanedPath.StartsWith(virtualApplicationPath, StringComparison.OrdinalIgnoreCase)
                        ? cleanedPath
                        : virtualApplicationPath + cleanedPath;

                    if (toAbsoluteUrl)
                        url = $"{scheme}://{ResolveHttpHost()}{url}";

                    if (appendVersion)
                    {
                        string physicalPath = MapPath(cleanedPath, mapFromContentRoot);

                        var fileInfo = new FileInfo(physicalPath);

                        if (!fileInfo.Exists)
                            throw new FileNotFoundException($"The specified virtual path {virtualPath} does not exist on disk at {physicalPath}.");

                        if (watchWhenAppendVersion)
                            entry.AddExpirationToken(new PhysicalFileChangeToken(fileInfo));

                        long versionHash = fileInfo.LastWriteTimeUtc.ToFileTimeUtc() ^ fileInfo.Length;
                        string version = Convert.ToString(versionHash, 16);

                        string qsStart = url.Contains("?") ? "&" : "?";

                        url = $"{url}{qsStart}{versionParameterName}={version}";
                    }

                    return url;
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, toAbsoluteUrl, scheme, appendVersion, versionParameterName, mapFromContentRoot, watchWhenAppendVersion }, returnValue: true))
            {
				throw new UmbrellaWebException("There has been a problem mapping the specified virtual path.", exc);
			}
			finally
			{
				if (cacheKeyParts != null)
					ArrayPool<string>.Shared.Return(cacheKeyParts);
			}
		}

        public virtual string GenerateActionUrl(string actionName, string controllerName, IDictionary<string, object> routeValues = null, string routeName = null)
        {
            Guard.ArgumentNotNullOrWhiteSpace(actionName, nameof(actionName));
            Guard.ArgumentNotNullOrWhiteSpace(controllerName, nameof(controllerName));

            try
            {
                if (routeValues == null)
                    routeValues = new Dictionary<string, object>();

                return UrlHelper.GenerateUrl(routeName, actionName, controllerName, new RouteValueDictionary(routeValues), RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { actionName, controllerName, routeValues, routeName }))
            {
                throw;
            }
        }

        public virtual string GenerateWebApiUrl(string controllerName, IDictionary<string, object> routeValues = null, string routeName = "DefaultApi")
        {
            Guard.ArgumentNotNullOrWhiteSpace(controllerName, nameof(controllerName));
            Guard.ArgumentNotNullOrWhiteSpace(routeName, nameof(routeName));

            try
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext, RouteTable.Routes);

                if (routeValues == null)
                    routeValues = new Dictionary<string, object>();

                routeValues.Add("httproute", "");
                routeValues.Add("controller", controllerName);

                return urlHelper.RouteUrl(routeName, new RouteValueDictionary(routeValues));
            }
            catch (Exception exc) when (Log.WriteError(exc, new { controllerName, routeValues, routeName }))
            {
                throw;
            }
        }
        #endregion

        #region Protected Methods
        protected virtual string ResolveHttpHost() => HttpContext.Current.Request.Url.Host;
        #endregion

        #region Internal Methods
        internal string TransformPath(string virtualPath, bool ensureStartsWithTildeSlash, bool ensureNoTilde, bool ensureLeadingSlash)
        {
            ReadOnlySpan<char> span = virtualPath.AsSpan().Trim();

            int length = span.Length;

            if (ensureNoTilde && span[0] == '~')
                span = span.TrimStart('~');

            bool leadingSlashToInsert = false;

            if (ensureLeadingSlash && span[0] != '/')
                leadingSlashToInsert = true;

            bool leadingTildaToInsert = false;
            bool leadingTildaSlashToInsert = false;

            if (ensureStartsWithTildeSlash && !span.StartsWith("~/".AsSpan(), StringComparison.Ordinal))
            {
                if (span[0] == '/')
                    leadingTildaToInsert = true;
                else
                    leadingTildaSlashToInsert = true;
            }

            int duplicateSlashCount = 0;

            if (leadingTildaSlashToInsert)
            {
                Span<char> outputSpan = stackalloc char[span.Length + 2];
                outputSpan[0] = '~';
                outputSpan[1] = '/';
                span.CopyTo(outputSpan.Slice(2));

                for (int i = 0; i < outputSpan.Length; i++)
                {
                    if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
                    {
                        duplicateSlashCount++;
                    }
                }

                if (duplicateSlashCount > 0)
                {
                    Span<char> cleanedSpan = stackalloc char[outputSpan.Length - duplicateSlashCount];

                    int j = 0;
                    for (int i = 0; i < outputSpan.Length; i++)
                    {
                        if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
                        {
                            continue;
                        }

                        cleanedSpan[j++] = outputSpan[i];
                    }

                    return cleanedSpan.ToString();
                }

                return outputSpan.ToString();
            }
            else if (leadingTildaToInsert)
            {
                Span<char> outputSpan = stackalloc char[span.Length + 1];
                outputSpan[0] = '~';
                span.CopyTo(outputSpan.Slice(1));

                for (int i = 0; i < outputSpan.Length; i++)
                {
                    if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
                    {
                        duplicateSlashCount++;
                    }
                }

                if (duplicateSlashCount > 0)
                {
                    Span<char> cleanedSpan = stackalloc char[outputSpan.Length - duplicateSlashCount];

                    int j = 0;
                    for (int i = 0; i < outputSpan.Length; i++)
                    {
                        if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
                        {
                            continue;
                        }

                        cleanedSpan[j++] = outputSpan[i];
                    }

                    return cleanedSpan.ToString();
                }

                return outputSpan.ToString();
            }
            else if (leadingSlashToInsert)
            {
                Span<char> outputSpan = stackalloc char[span.Length + 1];
                outputSpan[0] = '/';
                span.CopyTo(outputSpan.Slice(1));

                for (int i = 0; i < outputSpan.Length; i++)
                {
                    if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
                    {
                        duplicateSlashCount++;
                    }
                }

                if (duplicateSlashCount > 0)
                {
                    Span<char> cleanedSpan = stackalloc char[outputSpan.Length - duplicateSlashCount];

                    int j = 0;
                    for (int i = 0; i < outputSpan.Length; i++)
                    {
                        if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
                        {
                            continue;
                        }

                        cleanedSpan[j++] = outputSpan[i];
                    }

                    return cleanedSpan.ToString();
                }

                return outputSpan.ToString();
            }

            for (int i = 0; i < span.Length; i++)
            {
                if (i > 0 && span[i] == '/' && span[i - 1] == '/')
                {
                    duplicateSlashCount++;
                }
            }

            if (duplicateSlashCount > 0)
            {
                Span<char> cleanedSpan = stackalloc char[span.Length - duplicateSlashCount];

                int j = 0;
                for (int i = 0; i < span.Length; i++)
                {
                    if (i > 0 && span[i] == '/' && span[i - 1] == '/')
                    {
                        continue;
                    }

                    cleanedSpan[j++] = span[i];
                }

                return cleanedSpan.ToString();
            }

            return span.ToString();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal string TransformPathOld(string virtualPath, bool ensureStartsWithTildeSlash, bool ensureNoTilde, bool ensureLeadingSlash)
        {
            StringBuilder sb = new StringBuilder(virtualPath)
                .Trim();

            if (ensureNoTilde && sb.StartsWith("~"))
                sb.Remove(0, 1);

            if (ensureLeadingSlash && !sb.StartsWith('/'))
                sb.Insert(0, '/');

            if (ensureStartsWithTildeSlash && !sb.StartsWith("~/"))
            {
                if (sb.StartsWith('/'))
                    sb.Insert(0, '~');
                else
                    sb.Insert(0, "~/");
            }

            for (int i = 0; i < sb.Length; i++)
            {
                if (i > 0 && sb[i] == '/' && sb[i - 1] == '/')
                    sb.Remove(i--, 1);
            }

            return sb.ToString();
        }
        #endregion
    }
}