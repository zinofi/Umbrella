using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
    }

    public class ResponsiveImageTag : IHtmlString
    {
        #region Protected Members
        protected readonly string p_Path;
        protected readonly Func<string, string> p_MapVirtualPathFunc;
        protected readonly HashSet<int> p_PixelDensities = new HashSet<int> { 1 };
        protected readonly Dictionary<string, string> p_HtmlAttributes;
        #endregion

        #region Constructors
        public ResponsiveImageTag(string path, string altText, IDictionary<string, object> htmlAttributes, Func<string, string> mapVirtualPath)
        {
            p_Path = path;
            p_MapVirtualPathFunc = mapVirtualPath;

            if (htmlAttributes == null)
                p_HtmlAttributes = new Dictionary<string, string>();
            else
                p_HtmlAttributes = htmlAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

            p_HtmlAttributes.Add("src", mapVirtualPath(path));
            p_HtmlAttributes.Add("alt", altText);
        }
        #endregion

        #region IHtmlString Members
        public virtual string ToHtmlString()
        {
            var imgTag = new TagBuilder("img");

            if (p_PixelDensities.Count > 1)
                AddSrcsetAttribute(imgTag);

            imgTag.MergeAttributes(p_HtmlAttributes);

            //TODO: Need to add a caching layer
            return imgTag.ToString(TagRenderMode.SelfClosing);
        }
        #endregion

        #region Public Methods
        public ResponsiveImageTag WithDensities(params int[] densities)
        {
            foreach (int density in densities)
                p_PixelDensities.Add(density);

            return this;
        }

        public ResponsiveImageTag WithFixedSize(int width)
        {
            string strWidth = width.ToString();

            p_HtmlAttributes["width"] = strWidth;
            p_HtmlAttributes["height"] = strWidth;

            return this;
        }

        public ResponsiveImageTag WithFixedSize(int width, int height)
        {
            p_HtmlAttributes["width"] = width.ToString();
            p_HtmlAttributes["height"] = height.ToString();

            return this;
        }
        #endregion

        #region Protected Methods
        protected virtual void AddSrcsetAttribute(TagBuilder imgTag)
        {
            int densityIndex = p_Path.LastIndexOf('.');

            IEnumerable<string> srcsetImagePaths =
                from density in p_PixelDensities
                let densityX = $"{density}x"
                let highResImagePath = p_Path.Insert(densityIndex, $"@{densityX}") + $" {densityX}"
                select p_MapVirtualPathFunc(highResImagePath);

            imgTag.Attributes["srcset"] = string.Join(", ", srcsetImagePaths);
        }
        #endregion
    }
}