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
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.Hosting
{
    public class UmbrellaHostingEnvironment : IUmbrellaHostingEnvironment
    {
        #region Private Members
        private readonly ILogger m_Logger;
        private readonly IHostingEnvironment m_HostingEnvironment;
        private readonly IHttpContextAccessor m_HttpContextAccessor;
        private readonly IMemoryCache m_Cache;
        #endregion

        #region Constructors
        public UmbrellaHostingEnvironment(ILogger<UmbrellaHostingEnvironment> logger,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache)
        {
            m_Logger = logger;
            m_HostingEnvironment = hostingEnvironment;
            m_HttpContextAccessor = httpContextAccessor;
            m_Cache = cache;
        }
        #endregion

        public string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

                //Trim and remove the ~/ from the front of the path
                //Also change forward slashes to back slashes
                string cleanedPath = TransformPath(virtualPath, true, true);

                string key = $"UmbrellaHostingEnvironment:MapPath:{cleanedPath}:{fromContentRoot}".ToUpperInvariant();

                return m_Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1));

                    string rootPath = fromContentRoot
                        ? m_HostingEnvironment.ContentRootPath
                        : m_HostingEnvironment.WebRootPath;

                    return Path.Combine(rootPath, cleanedPath);
                });
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, new { virtualPath }))
            {
                throw;
            }
        }

        public string MapWebPath(string virtualPath, string scheme = "http")
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));
                Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));

                string cleanedPath = TransformPath(virtualPath, false, false);

                string key = $"UmbrellaHostingEnvironment:MapWebPath:{cleanedPath}:{scheme}".ToUpperInvariant();

                return m_Cache.GetOrCreate(key, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1));

                    HttpRequest request = m_HttpContextAccessor.HttpContext.Request;

                    return $"{scheme}://{request.Host}{cleanedPath}";
                });
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, new { virtualPath, scheme }))
            {
                throw;
            }
        }

        private string TransformPath(string virtualPath, bool removeLeadingSlash, bool convertForwardSlashesToBackSlashes)
        {
            StringBuilder sb = new StringBuilder(virtualPath)
                .Trim()
                .Trim('~');

            if (removeLeadingSlash)
                sb.Trim('/');

            if(convertForwardSlashesToBackSlashes)
                sb.Replace("/", @"\");

            return sb.ToString();
        }
    }
}