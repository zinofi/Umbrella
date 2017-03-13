using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Hosting;
using Microsoft.AspNetCore.Http;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.AspNetCore.DynamicImage
{
    public class DynamicImageUrlGenerator : IDynamicImageUrlGenerator
    {
        #region Private Constants
        private const string c_UrlFormat = "~/DynamicImage/{0}/{1}/{2}/{3}/{4}";
        #endregion

        #region Private Static Members
        private static readonly Regex s_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Private Members
        private readonly ILogger m_Logger;
        private readonly IUmbrellaHostingEnvironment m_HostingEnvironment;
        private readonly IHttpContextAccessor m_HttpContextAccessor;
        #endregion

        #region Constructors
        public DynamicImageUrlGenerator(ILogger<DynamicImageUrlGenerator> logger,
            IUmbrellaHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            m_Logger = logger;
            m_HostingEnvironment = hostingEnvironment;
            m_HttpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region IDynamicImageUrlGenerator Members
        public string GenerateUrl(DynamicImageOptions options, bool toAbsolutePath = false)
        {
            string originalExtension = Path.GetExtension(options.OriginalVirtualPath).ToLower().Remove(0, 1);

            string path = options.OriginalVirtualPath.Replace("~/", "");

            string virtualPath = string.Format(c_UrlFormat,
                options.Width,
                options.Height,
                options.Mode,
                originalExtension,
                path.Replace(originalExtension, options.Format.ToFileExtensionString()));

            virtualPath = s_Regex.Replace(virtualPath, "/");

            if (toAbsolutePath)
                virtualPath = m_HostingEnvironment.MapWebPath(virtualPath, m_HttpContextAccessor.HttpContext.Request.Scheme);

            return virtualPath;
        }
        #endregion
    }
}