using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Tags;
using Umbrella.Legacy.WebUtilities.Mvc.Helpers;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Helpers
{
    public static class ImageHelpers
    {
        public static ResponsiveDynamicImageTag DynamicImage(this HtmlHelper helper, IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, object htmlAttributes = null, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return helper.DynamicImage(dynamicImageUtility, path, altText, width, height, resizeMode, attributesDictionary, format, toAbsolutePath);
        }

        public static ResponsiveDynamicImageTag DynamicImage(this HtmlHelper helper, IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, IDictionary<string, object> htmlAttributes, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            return new ResponsiveDynamicImageTag(dynamicImageUtility, path, altText, width, height, resizeMode, htmlAttributes, format, toAbsolutePath, urlHelper.Content);
        }

        public static ResponsiveDynamicImagePictureSourceTag DynamicImagePictureSource(this HtmlHelper helper, IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, string mediaAttributeValue, object htmlAttributes = null, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return helper.DynamicImagePictureSource(dynamicImageUtility, path, altText, width, height, resizeMode, mediaAttributeValue, attributesDictionary, format, toAbsolutePath);
        }

        public static ResponsiveDynamicImagePictureSourceTag DynamicImagePictureSource(this HtmlHelper helper, IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, string mediaAttributeValue, IDictionary<string, object> htmlAttributes, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            string url = dynamicImageUtility.GetResizedUrl(path, width, height, resizeMode, format, toAbsolutePath);

            return new ResponsiveDynamicImagePictureSourceTag(url, altText, mediaAttributeValue, htmlAttributes, urlHelper.Content);
        }
    }
}