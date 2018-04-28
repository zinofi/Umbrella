using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Primitives;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.Hosting
{
    public class UmbrellaWebHostingEnvironment : UmbrellaHostingEnvironment, IUmbrellaWebHostingEnvironment
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(UmbrellaWebHostingEnvironment).FullName;
        private static readonly Regex s_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        #endregion

        #region Protected Properties
        protected IHostingEnvironment HostingEnvironment { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        #endregion

        #region Constructors
        public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache)
            : base(logger, cache)
        {
            HostingEnvironment = hostingEnvironment;
            HttpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public override string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

                string key = $"{s_CacheKeyPrefix}:{nameof(MapPath)}:{virtualPath}:{fromContentRoot}".ToUpperInvariant();

                return Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                    //Trim and remove the ~/ from the front of the path
                    //Also change forward slashes to back slashes
                    string cleanedPath = TransformPath(virtualPath, true, false, true);

                    string rootPath = fromContentRoot
                        ? HostingEnvironment.ContentRootPath
                        : HostingEnvironment.WebRootPath;

                    return Path.Combine(rootPath, cleanedPath);
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
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));
                Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));
                Guard.ArgumentNotNullOrWhiteSpace(versionParameterName, nameof(versionParameterName));

                string key = $"{s_CacheKeyPrefix}:{nameof(MapWebPath)}:{virtualPath}:{toAbsoluteUrl}:{scheme}:{appendVersion}:{versionParameterName}:{mapFromContentRoot}:{watchWhenAppendVersion}".ToUpperInvariant();
                
                return Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                    string cleanedPath = TransformPath(virtualPath, false, true, false);

                    var applicationPath = HttpContextAccessor.HttpContext.Request.PathBase;

                    PathString virtualApplicationPath = applicationPath != "/"
                        ? applicationPath
                        : PathString.Empty;

                    //Prefix the path with the virtual application segment but only if the cleanedPath doesn't already start with the segment
                    string url = cleanedPath.StartsWith(virtualApplicationPath, StringComparison.OrdinalIgnoreCase)
                        ? cleanedPath
                        : virtualApplicationPath.Add(cleanedPath).Value;

                    if (toAbsoluteUrl)
                        url = $"{scheme}://{ResolveHttpHost()}{url}";

                    if(appendVersion)
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
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(actionName, nameof(actionName));
                Guard.ArgumentNotNullOrWhiteSpace(controllerName, nameof(controllerName));

                if (routeValues == null)
                    routeValues = new Dictionary<string, object>();

                throw new NotImplementedException();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { actionName, controllerName, routeValues, routeName }))
            {
                throw;
            }
        }

        public virtual string GenerateWebApiUrl(string controllerName, IDictionary<string, object> routeValues = null, string routeName = "DefaultApi")
        {
            //TODO: Should this call into the method above? Or should we just throw as below? Maybe with a better message?
            throw new InvalidOperationException();
        }
        #endregion

        #region Protected Methods
        protected virtual string ResolveHttpHost() => HttpContextAccessor.HttpContext.Request.Host.Value;
        #endregion

        #region Public Methods
        public string TransformPath(string virtualPath, bool removeLeadingSlash, bool ensureLeadingSlash, bool convertForwardSlashesToBackSlashes)
        {
            StringBuilder sb = new StringBuilder(virtualPath)
                .Trim()
                .Trim('~');

            if (removeLeadingSlash && ensureLeadingSlash)
                throw new ArgumentException($"{nameof(removeLeadingSlash)} and {nameof(ensureLeadingSlash)} are both set to true. This is not allowed.");

            if (removeLeadingSlash)
                sb.Trim('/');

            if (ensureLeadingSlash && !sb.StartsWith('/'))
                sb.Insert(0, '/');

            string path = s_Regex.Replace(sb.ToString(), "/");

            if (convertForwardSlashesToBackSlashes)
                path = path.Replace("/", @"\");

            return path;
        }
        #endregion
    }
}