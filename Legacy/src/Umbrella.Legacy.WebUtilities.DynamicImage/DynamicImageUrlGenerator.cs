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
        private const string c_UrlFormat = "~/DynamicImage/{0}/{1}/{2}/{3}/{4}";

        private static readonly Regex m_Regex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

            virtualPath = m_Regex.Replace(virtualPath, "/");

            if (toAbsolutePath)
                virtualPath = VirtualPathUtility.ToAbsolute(virtualPath);

            return virtualPath;
        }
    }
}
