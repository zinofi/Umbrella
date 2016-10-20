using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
    }

    public class ResponsiveDynamicImageTag : ResponsiveImageTag
    {
        #region Private Members
        private readonly HashSet<int> m_SizeWidths = new HashSet<int>();
        private string m_SizeAttributeValue;
        private readonly float m_Ratio;
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly DynamicResizeMode m_ResizeMode;
        private readonly DynamicImageFormat m_Format;
        private readonly bool m_ToAbsolutePath;
        #endregion

        #region Constructors
        public ResponsiveDynamicImageTag(IDynamicImageUtility dynamicImageUtility, string path, string altText, int width, int height, DynamicResizeMode resizeMode, IDictionary<string, object> htmlAttributes, DynamicImageFormat format, bool toAbsolutePath, Func<string, string> mapVirtualPathFunc)
            : base(path, altText, htmlAttributes, mapVirtualPathFunc)
        {
            m_DynamicImageUtility = dynamicImageUtility;
            m_ResizeMode = resizeMode;
            m_Format = format;
            m_ToAbsolutePath = toAbsolutePath;

            m_Ratio = width / height;

            string x1Url = dynamicImageUtility.GetResizedUrl(path, width, height, resizeMode, format, toAbsolutePath);

            p_HtmlAttributes["src"] = mapVirtualPathFunc(x1Url);
        }
        #endregion

        #region Overridden Methods
        public override string ToHtmlString()
        {
            var imgTag = new TagBuilder("img");

            AddSrcsetAttribute(imgTag);

            imgTag.MergeAttributes(p_HtmlAttributes);

            //TODO: Need to add a caching layer
            return imgTag.ToString(TagRenderMode.SelfClosing);
        }

        protected override void AddSrcsetAttribute(TagBuilder imgTag)
        {
            //If we don't have any size information just call into the base method
            //but only if we also have some density information
            if (m_SizeWidths.Count == 0 && p_PixelDensities.Count > 1)
            {
                base.AddSrcsetAttribute(imgTag);
            }
            else if (m_SizeWidths.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(m_SizeAttributeValue))
                    imgTag.Attributes["sizes"] = m_SizeAttributeValue;

                imgTag.Attributes["srcset"] = string.Join(",", GetSizeStrings());
            }
        }
        #endregion

        #region Public Methods
        public ResponsiveDynamicImageTag WithSizes(params int[] x1Widths)
        {
            foreach (int size in x1Widths)
                m_SizeWidths.Add(size);

            return this;
        }

        public ResponsiveDynamicImageTag WithSizesAttributeValue(string value)
        {
            m_SizeAttributeValue = value;

            return this;
        }
        #endregion

        #region Private Methods
        private IEnumerable<string> GetSizeStrings()
        {
            foreach (int sizeWidth in m_SizeWidths)
            {
                foreach (int density in p_PixelDensities)
                {
                    int imgWidth = sizeWidth * density;
                    int imgHeight = (int)Math.Ceiling(imgWidth / m_Ratio);

                    string imgUrl = m_DynamicImageUtility.GetResizedUrl(p_Path, imgWidth, imgHeight, m_ResizeMode, m_Format, m_ToAbsolutePath);

                    yield return $"{imgUrl} {imgWidth}w";
                }
            }
        }
        #endregion
    }
}