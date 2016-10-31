using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Tags
{
    public class ResponsiveImageTag : IHtmlString
    {
        #region Protected Members
        protected string Path { get; }
        protected Func<string, string> MapVirtualPathFunc { get; }
        protected HashSet<int> PixelDensities { get; } = new HashSet<int> { 1 };
        protected Dictionary<string, string> HtmlAttributes { get; }
        #endregion

        #region Constructors
        public ResponsiveImageTag(string path, string altText, IDictionary<string, object> htmlAttributes, Func<string, string> mapVirtualPath)
        {
            Path = path;
            MapVirtualPathFunc = mapVirtualPath;

            if (htmlAttributes == null)
                HtmlAttributes = new Dictionary<string, string>();
            else
                HtmlAttributes = htmlAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

            HtmlAttributes.Add("src", mapVirtualPath(path));
            HtmlAttributes.Add("alt", altText);
        }
        #endregion

        #region IHtmlString Members
        public virtual string ToHtmlString()
        {
            var imgTag = new TagBuilder("img");

            if (PixelDensities.Count > 1)
                AddSrcsetAttribute(imgTag);

            imgTag.MergeAttributes(HtmlAttributes);

            //TODO: Need to add a caching layer
            return imgTag.ToString(TagRenderMode.SelfClosing);
        }
        #endregion

        #region Public Methods
        public ResponsiveImageTag WithDensities(params int[] densities)
        {
            foreach (int density in densities)
                PixelDensities.Add(density);

            return this;
        }

        public ResponsiveImageTag WithFixedSize(int width)
        {
            string strWidth = width.ToString();

            HtmlAttributes["width"] = strWidth;
            HtmlAttributes["height"] = strWidth;

            return this;
        }

        public ResponsiveImageTag WithFixedSize(int width, int height)
        {
            HtmlAttributes["width"] = width.ToString();
            HtmlAttributes["height"] = height.ToString();

            return this;
        }
        #endregion

        #region Protected Methods
        protected virtual void AddSrcsetAttribute(TagBuilder imgTag)
        {
            string path = HtmlAttributes["src"];

            int densityIndex = path.LastIndexOf('.');

            IEnumerable<string> srcsetImagePaths =
                from density in PixelDensities
                let densityX = $"{density}x"
                let highResImagePath = path.Insert(densityIndex, $"@{densityX}") + $" {densityX}"
                select MapVirtualPathFunc(highResImagePath);

            imgTag.Attributes["srcset"] = string.Join(", ", srcsetImagePaths);
        }
        #endregion
    }
}
