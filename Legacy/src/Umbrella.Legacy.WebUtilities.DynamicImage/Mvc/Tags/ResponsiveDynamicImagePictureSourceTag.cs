using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Tags;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Tags
{
    public class ResponsiveDynamicImagePictureSourceTag : ResponsiveImageTag
    {
        #region Constructors
        public ResponsiveDynamicImagePictureSourceTag(string path, string mediaAttributeValue, IDictionary<string, object> htmlAttributes, Func<string, string> mapVirtualPathFunc)
            : base(path, string.Empty, htmlAttributes, mapVirtualPathFunc)
        {
            HtmlAttributes["media"] = mediaAttributeValue;
        } 
        #endregion

        #region Overridden Methods
        public override string ToHtmlString()
        {
            var tag = new TagBuilder("source");

            AddSrcsetAttribute(tag);

            //Remove obsolete values
            HtmlAttributes.Remove("alt");
            HtmlAttributes.Remove("src");

            tag.MergeAttributes(HtmlAttributes);

            return tag.ToString(TagRenderMode.SelfClosing);
        } 
        #endregion
    }
}