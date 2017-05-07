using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Tags;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Helpers
{
    public static class ImageHelpers
    {
        private const string c_DefaultPrefix = "dynamicimage";

        public static ResponsiveDynamicImageTag DynamicImage(this HtmlHelper helper, IDynamicImageUtility utility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, object htmlAttributes = null, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false, string dynamicImagePathPrefix = c_DefaultPrefix)
        {
            var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return helper.DynamicImage(utility, path, altText, width, height, resizeMode, attributesDictionary, format, toAbsolutePath, dynamicImagePathPrefix);
        }

        public static ResponsiveDynamicImageTag DynamicImage(this HtmlHelper helper, IDynamicImageUtility utility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, IDictionary<string, object> htmlAttributes, DynamicImageFormat format = DynamicImageFormat.Jpeg, string dynamicImagePathPrefix = c_DefaultPrefix)
        {
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            return new ResponsiveDynamicImageTag(utility, dynamicImagePathPrefix, path, altText, width, height, resizeMode, htmlAttributes, format, urlHelper.Content);
        }

        public static ResponsiveDynamicImagePictureSourceTag DynamicImagePictureSource(this HtmlHelper helper, IDynamicImageUtility utility, string path, int width, int height, DynamicResizeMode resizeMode, string mediaAttributeValue, object htmlAttributes = null, DynamicImageFormat format = DynamicImageFormat.Jpeg, string dynamicImagePathPrefix = c_DefaultPrefix)
        {
            var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return helper.DynamicImagePictureSource(utility, path, width, height, resizeMode, mediaAttributeValue, attributesDictionary, format, dynamicImagePathPrefix);
        }

        public static ResponsiveDynamicImagePictureSourceTag DynamicImagePictureSource(this HtmlHelper helper, IDynamicImageUtility utility, string path, int width, int height, DynamicResizeMode resizeMode, string mediaAttributeValue, IDictionary<string, object> htmlAttributes, DynamicImageFormat format = DynamicImageFormat.Jpeg, string dynamicImagePathPrefix = c_DefaultPrefix)
        {
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var options = new DynamicImageOptions
            {
                Format = format,
                Height = height,
                ResizeMode = resizeMode,
                SourcePath = path,
                Width = width
            };

            string url = utility.GenerateVirtualPath(dynamicImagePathPrefix, options);

            return new ResponsiveDynamicImagePictureSourceTag(url, mediaAttributeValue, htmlAttributes, urlHelper.Content);
        }
    }
}