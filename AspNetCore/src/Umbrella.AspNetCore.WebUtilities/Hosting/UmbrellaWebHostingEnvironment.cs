using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.Hosting
{
    public class UmbrellaWebHostingEnvironment : IUmbrellaWebHostingEnvironment
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(UmbrellaWebHostingEnvironment).FullName;
        private static readonly Regex s_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected IHostingEnvironment HostingEnvironment { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected IMemoryCache Cache { get; }
        #endregion

        #region Constructors
        public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache)
        {
            Log = logger;
            HostingEnvironment = hostingEnvironment;
            HttpContextAccessor = httpContextAccessor;
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

                    string cleanedPath = TransformPath(virtualPath, false, true, false);

                    var applicationPath = HttpContextAccessor.HttpContext.Request.PathBase;

                    //Prefix the path with the virtual application segment
                    string basePath = applicationPath != "/"
                        ? applicationPath.Add(cleanedPath).Value
                        : cleanedPath;

                    string url = basePath;

                    if (toAbsoluteUrl)
                    {
                        string host = ResolveHttpHost();

                        url = $"{scheme}://{host}{cleanedPath}";
                    }

                    //TODO: Consider using the IUmbrellaFileProvider to do this work. That way we can use the append version
                    //stuff with files stored elsewhere in future, e.g. in blob storage, etc. - meh probably not workable!
                    if(appendVersion)
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