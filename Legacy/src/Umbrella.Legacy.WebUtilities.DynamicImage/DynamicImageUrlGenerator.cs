using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.Legacy.WebUtilities.DynamicImage
{
    public class DynamicImageUrlGenerator : IDynamicImageUrlGenerator
    {
        private const string c_UrlFormat = "~/{0}/{1}/{2}/{3}/{4}/{5}";

        private static readonly Regex m_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string GenerateUrl(string dynamicImagePathPrefix, DynamicImageOptions options, bool toAbsolutePath = false)
        {
            string originalExtension = Path.GetExtension(options.SourcePath).ToLower().Remove(0, 1);

            string path = options.SourcePath.Replace("~/", "");

            string virtualPath = string.Format(c_UrlFormat,
                dynamicImagePathPrefix,
                options.Width,
                options.Height,
                options.ResizeMode,
                originalExtension,
                path.Replace(originalExtension, options.Format.ToFileExtensionString()));

            virtualPath = m_Regex.Replace(virtualPath, "/");

            if (toAbsolutePath)
                virtualPath = VirtualPathUtility.ToAbsolute(virtualPath);

            return virtualPath;
        }
    }
}