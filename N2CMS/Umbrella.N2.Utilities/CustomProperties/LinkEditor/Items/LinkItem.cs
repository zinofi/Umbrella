using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.N2.CustomProperties.LinkEditor.Items
{
    public abstract class LinkItem : LinkItemBase
    {
        public string AccessKey { get; set; }
        public string Title { get; set; }
        public string TargetFrame { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public bool NoFollow { get; set; }
        public string OnClickCode { get; set; }

        public abstract string Url { get; set; }
        public string AdditionalParameters { get; set; }

        public override HtmlString ToHtmlString(string cssClass = "")
        {
            TagBuilder builder = new TagBuilder("a");

            if (!string.IsNullOrEmpty(AccessKey))
                builder.MergeAttribute("accesskey", AccessKey);
            if (!string.IsNullOrEmpty(Title))
                builder.MergeAttribute("title", Title);
            if (!string.IsNullOrEmpty(TargetFrame) && TargetFrame != "_self")
                builder.MergeAttribute("target", TargetFrame);
            if (!string.IsNullOrEmpty(cssClass))
                builder.MergeAttribute("class", cssClass);

            builder.MergeAttribute("href", Url + AdditionalParameters);

            if (NoFollow)
                builder.MergeAttribute("rel", "nofollow");

            if (!string.IsNullOrEmpty(OnClickCode))
                builder.MergeAttribute("onclick", OnClickCode);

            if (!string.IsNullOrEmpty(ImageUrl))
            {
                TagBuilder imageBuilder = new TagBuilder("img");
                imageBuilder.MergeAttribute("src", ImageUrl);

                if (!string.IsNullOrEmpty(Title))
                    imageBuilder.MergeAttribute("alt", Title);

                builder.InnerHtml = imageBuilder.ToString();
            }
            else if(!string.IsNullOrEmpty(Text))
            {
                builder.SetInnerText(Text);
            }

            return new HtmlString(builder.ToString());
        }
    }
}