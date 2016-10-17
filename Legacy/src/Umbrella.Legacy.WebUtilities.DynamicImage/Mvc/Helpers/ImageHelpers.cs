using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
    public static class ImageHelpers
    {
        public static ResponsiveImageTag Image(this HtmlHelper helper, string path, string altText, object htmlAttributes = null)
        {
            var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return helper.Image(path, altText, attributesDictionary);
        }

        public static ResponsiveImageTag Image(this HtmlHelper helper, string path, string altText, IDictionary<string, object> htmlAttributes)
        {
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            return new ResponsiveImageTag(path, altText, htmlAttributes, urlHelper.Content);
        }

        public static ResponsiveImageTag DynamicImage(this HtmlHelper helper, IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, object htmlAttributes = null, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return helper.DynamicImage(dynamicImageUtility, path, altText, width, height, resizeMode, attributesDictionary, format, toAbsolutePath);
        }

        public static ResponsiveImageTag DynamicImage(this HtmlHelper helper, IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, IDictionary<string, object> htmlAttributes, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
        {
            string imageUrl = dynamicImageUtility.GetResizedUrl(path, width, height, resizeMode, format, toAbsolutePath);

            return helper.Image(imageUrl, altText, htmlAttributes);
        }
    }

    public class ResponsiveImageTag : IHtmlString
    {
        private readonly string m_Path;
        private readonly Func<string, string> m_MapVirtualPathFunc;
        private readonly HashSet<int> m_PixelDensities;
        private readonly IDictionary<string, string> m_HtmlAttributes;

        public ResponsiveImageTag(string path, string altText, IDictionary<string, object> htmlAttributes, Func<string, string> mapVirtualPath)
        {
            m_Path = path;
            m_MapVirtualPathFunc = mapVirtualPath;

            m_PixelDensities = new HashSet<int> { 1 };

            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            htmlAttributes.Add("src", mapVirtualPath(path));
            htmlAttributes.Add("alt", altText);
        }

        public string ToHtmlString()
        {
            var imgTag = new TagBuilder("img");

            if (m_PixelDensities.Count > 1)
                AddSrcsetAttribute(imgTag);
            
            imgTag.MergeAttributes(m_HtmlAttributes);

            return imgTag.ToString(TagRenderMode.SelfClosing);
        }

        private void AddSrcsetAttribute(TagBuilder imgTag)
        {
            int densityIndex = m_Path.LastIndexOf('.');

            IEnumerable<string> srcsetImagePaths =
                from density in m_PixelDensities
                let densityX = $"{density}x"
                let highResImagePath = m_Path.Insert(densityIndex, $"@{densityX}") + $" {densityX}"
                select m_MapVirtualPathFunc(highResImagePath);

            imgTag.Attributes["srcset"] = string.Join(", ", srcsetImagePaths);
        }

        public ResponsiveImageTag WithDensities(params int[] densities)
        {
            foreach (int density in densities)
                m_PixelDensities.Add(density);

            return this;
        }

        public ResponsiveImageTag WithSize(int width, int? height = null)
        {
            m_HtmlAttributes["width"] = width.ToString();
            m_HtmlAttributes["height"] = (height ?? width).ToString();

            return this;
        }
    }
}