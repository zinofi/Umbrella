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
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.DynamicImage
{
    public class DynamicImageUrlGenerator : IDynamicImageUrlGenerator
    {
        #region Private Constants
        private const string c_UrlFormat = "~/{0}/{1}/{2}/{3}/{4}/{5}";
        #endregion

        #region Private Static Members
        private static readonly Regex s_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Private Members
        private readonly ILogger m_Logger;
        private readonly IUmbrellaHostingEnvironment m_UmbrellaHostingEnvironment;
        private readonly IHttpContextAccessor m_HttpContextAccessor;
        #endregion

        #region Constructors
        public DynamicImageUrlGenerator(ILogger<DynamicImageUrlGenerator> logger,
            IUmbrellaHostingEnvironment umbrellaHostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            m_Logger = logger;
            m_UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
            m_HttpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region IDynamicImageUrlGenerator Members
        public string GenerateUrl(string dynamicImagePathPrefix, DynamicImageOptions options, bool toAbsolutePath = false)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(dynamicImagePathPrefix, nameof(dynamicImagePathPrefix));

                string originalExtension = Path.GetExtension(options.SourcePath).ToLower().Remove(0, 1);

                string path = options.SourcePath.Replace("~/", "");

                string virtualPath = string.Format(c_UrlFormat,
                    dynamicImagePathPrefix,
                    options.Width,
                    options.Height,
                    options.ResizeMode,
                    originalExtension,
                    path.Replace(originalExtension, options.Format.ToFileExtensionString()));

                virtualPath = s_Regex.Replace(virtualPath, "/");

                if (toAbsolutePath)
                    virtualPath = m_UmbrellaHostingEnvironment.MapWebPath(virtualPath, m_HttpContextAccessor.HttpContext.Request.Scheme);

                return virtualPath;
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, new { options, toAbsolutePath }, returnValue: true))
            {
                throw new DynamicImageException("An error occurred whilst generating the url.", exc, options);
            }
        }
        #endregion
    }
}