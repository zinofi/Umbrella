using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.N2.CustomProperties.LinkEditor.Items
{
    public class ImageItem : LinkItemBase
    {
        public string Url { get; set; }
        public string AltText { get; set; }
        public override string LinkTypeString
        {
            get { return "Image"; }
        }

        public override HtmlString ToHtmlString(string cssClass = "")
        {
            TagBuilder builder = new TagBuilder("img");
            builder.MergeAttribute("src", Url);
            builder.MergeAttribute("alt", AltText);

            if (!string.IsNullOrEmpty(cssClass))
                builder.MergeAttribute("class", cssClass);

            return new HtmlString(builder.ToString());
        }

        public override void RebaseLinkItem(string fromAppPath, string toAppPath)
        {
            Url = global::N2.Web.Url.Rebase(Url, fromAppPath, toAppPath);
        }
    }
}