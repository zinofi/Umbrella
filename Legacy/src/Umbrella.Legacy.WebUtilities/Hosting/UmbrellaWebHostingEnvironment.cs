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
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Hosting
{
    public class UmbrellaWebHostingEnvironment : IUmbrellaWebHostingEnvironment
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(UmbrellaWebHostingEnvironment).FullName;
        private static readonly Regex s_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected IMemoryCache Cache { get; }
        #endregion

        #region Constructors
        public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
            IMemoryCache cache)
        {
            Log = logger;
            Cache = cache;
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public virtual string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

                string key = $"{s_CacheKeyPrefix}:MapPath:{virtualPath}:{fromContentRoot}".ToUpperInvariant();

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

        public virtual string MapWebPath(string virtualPath, bool toAbsoluteUrl = false, string scheme = "http", bool appendVersion = false, string versionParameterName = "v", bool mapFromContentRoot = true)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));
                Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));
                Guard.ArgumentNotNullOrWhiteSpace(versionParameterName, nameof(versionParameterName));

                string key = $"{s_CacheKeyPrefix}:MapWebPath:{virtualPath}:{toAbsoluteUrl}:{scheme}:{appendVersion}:{versionParameterName}:{mapFromContentRoot}".ToUpperInvariant();

                return Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                    string cleanedPath = TransformPath(virtualPath, false, true, true);

                    //Prefix the path with the virtual application segment
                    string basePath = HttpRuntime.AppDomainAppVirtualPath != "/"
                        ? HttpRuntime.AppDomainAppVirtualPath + cleanedPath
                        : cleanedPath;

                    string url = basePath;

                    if (toAbsoluteUrl)
                    {
                        string host = ResolveHttpHost();

                        string baseUrl = $"{scheme}://{host}";

                        url = baseUrl + basePath;
                    }

                    if (appendVersion)
                    {
                        string physicalPath = MapPath(cleanedPath, mapFromContentRoot);

                        FileInfo fileInfo = new FileInfo(physicalPath);

                        if (!fileInfo.Exists)
                            throw new FileNotFoundException($"The specified virtual path {virtualPath} does not exist on disk at {physicalPath}.");

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