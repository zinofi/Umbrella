using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Hosting
{
    public class UmbrellaHostingEnvironment : IUmbrellaHostingEnvironment
    {
        #region Private Members
        private readonly ILogger m_Logger;
        private readonly IMemoryCache m_Cache;
        #endregion

        #region Constructors
        public UmbrellaHostingEnvironment(ILogger<UmbrellaHostingEnvironment> logger,
            IMemoryCache cache)
        {
            m_Logger = logger;
            m_Cache = cache;
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            try
            {
                string cleanedPath = TransformPath(virtualPath, true, false, false);

                return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
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
                string baseUrl = $"{scheme}://{HttpContext.Current.Request.Url.Host}";

                string appPath = HttpRuntime.AppDomainAppVirtualPath;
                if (appPath != "/")
                    baseUrl += appPath;

                string cleanedPath = TransformPath(virtualPath, false, true, true);

                return baseUrl + cleanedPath;
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, new { virtualPath, scheme }))
            {
                throw;
            }
        }
        #endregion

        #region Private Methods
        private string TransformPath(string virtualPath, bool ensureStartsWithTildeSlash, bool ensureNoTilde, bool ensureLeadingSlash)
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

            return sb.ToString();
        } 
        #endregion
    }
}