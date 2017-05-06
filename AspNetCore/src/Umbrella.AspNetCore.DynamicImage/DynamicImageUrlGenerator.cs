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

        #region Protected Properties
        protected ILogger Log { get; }
        protected IUmbrellaHostingEnvironment UmbrellaHostingEnvironment { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        #endregion

        #region Constructors
        public DynamicImageUrlGenerator(ILogger<DynamicImageUrlGenerator> logger,
            IUmbrellaHostingEnvironment umbrellaHostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            Log = logger;
            UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
            HttpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region IDynamicImageUrlGenerator Members
        public virtual string GenerateUrl(string dynamicImagePathPrefix, DynamicImageOptions options, bool toAbsolutePath = false)
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

                virtualPath = UmbrellaHostingEnvironment.MapWebPath(virtualPath, toAbsolutePath, HttpContextAccessor.HttpContext.Request.Scheme);

                return virtualPath;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { dynamicImagePathPrefix, options, toAbsolutePath }, returnValue: true))
            {
                throw new DynamicImageException("An error occurred whilst generating the url.", exc, options);
            }
        }
        #endregion
    }
}