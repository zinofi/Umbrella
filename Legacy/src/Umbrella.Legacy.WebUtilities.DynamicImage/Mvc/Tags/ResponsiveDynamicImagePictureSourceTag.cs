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
        public ResponsiveDynamicImagePictureSourceTag(string path, string altText, string mediaAttributeValue, IDictionary<string, object> htmlAttributes, Func<string, string> mapVirtualPathFunc)
            : base(path, altText, htmlAttributes, mapVirtualPathFunc)
        {
            p_HtmlAttributes["media"] = mediaAttributeValue;
        } 
        #endregion

        #region Overridden Methods
        public override string ToHtmlString()
        {
            var tag = new TagBuilder("source");

            AddSrcsetAttribute(tag);

            tag.MergeAttributes(p_HtmlAttributes);

            //TODO: Need to add a caching layer
            return tag.ToString(TagRenderMode.SelfClosing);
        } 
        #endregion
    }
}