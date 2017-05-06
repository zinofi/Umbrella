using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities;

namespace Umbrella.Legacy.WebUtilities.DynamicImage
{
    public class DynamicImageUrlGenerator : IDynamicImageUrlGenerator
    {
        #region Private Constants
        private const string c_UrlFormat = "~/{0}/{1}/{2}/{3}/{4}/{5}";
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected IUmbrellaHostingEnvironment UmbrellaHostingEnvironment { get; }
        #endregion

        #region Constructors
        public DynamicImageUrlGenerator(ILogger<DynamicImageUrlGenerator> logger,
            IUmbrellaHostingEnvironment umbrellaHostingEnvironment)
        {
            Log = logger;
            UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
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

                virtualPath = UmbrellaHostingEnvironment.MapWebPath(virtualPath, toAbsolutePath, HttpContext.Current.Request.Url.Scheme);

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