using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Primitives;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Hosting
{
    // TODO: Add IHostingEnvironment and IHttpContextAccessor abstractions as per ASP.NET Core
    // so that this class can be fully tested.
    public class UmbrellaWebHostingEnvironment : UmbrellaHostingEnvironment, IUmbrellaWebHostingEnvironment
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(UmbrellaWebHostingEnvironment).FullName;
        private static readonly Regex s_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        #endregion

        #region Constructors
        public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
            IMemoryCache cache)
            : base(logger, cache)
        {
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public override string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

            try
            {
                if (!fromContentRoot)
                    throw new ArgumentException("This value must always be true in a classic .NET application. It can only be set to false inside a .NET Core application.", nameof(fromContentRoot));

                string key = $"{s_CacheKeyPrefix}:{nameof(MapPath)}:{virtualPath}:{fromContentRoot}".ToUpperInvariant();

                return Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                    string cleanedPath = TransformPath(virtualPath, true, false, false);

                    return System.Web.Hosting.HostingEnvironment.MapPath(cleanedPath);
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot }))
            {
                throw;
            }
        }
        #endregion

        #region IUmbrellaWebHostingEnvironment Members
        public virtual string MapWebPath(string virtualPath, bool toAbsoluteUrl = false, string scheme = "http", bool appendVersion = false, string versionParameterName = "v", bool mapFromContentRoot = true, bool watchWhenAppendVersion = true)
        {
            Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));
            Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));
            Guard.ArgumentNotNullOrWhiteSpace(versionParameterName, nameof(versionParameterName));

            try
            {
                string key = $"{s_CacheKeyPrefix}:{nameof(MapWebPath)}:{virtualPath}:{toAbsoluteUrl}:{scheme}:{appendVersion}:{versionParameterName}:{mapFromContentRoot}:{watchWhenAppendVersion}".ToUpperInvariant();

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

                        FileInfo fileInfo = new FileInfo(physicalPath);

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
            catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, toAbsoluteUrl, scheme, appendVersion, versionParameterName, mapFromContentRoot }))
            {
                throw;
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

        #region Public Methods
        public string TransformPath(string virtualPath, bool ensureStartsWithTildeSlash, bool ensureNoTilde, bool ensureLeadingSlash)
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

            return s_Regex.Replace(sb.ToString(), "/");
        }
        #endregion
    }
}