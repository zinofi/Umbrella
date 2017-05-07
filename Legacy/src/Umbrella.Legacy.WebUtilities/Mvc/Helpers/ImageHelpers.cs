using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Tags;

namespace System.Web.Mvc
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
}