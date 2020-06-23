using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Umbrella.Legacy.WebUtilities.Mvc.Tags;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
	// TODO - vUnlikely: Add 4 additional arguments like the ResponsiveDynamicImage helpers to allow for absolute path conversion with overrides.
	public static class ImageHelpers
	{
		public static ResponsiveImageTag ResponsiveImage(this HtmlHelper helper, string path, string altText, object htmlAttributes = null)
		{
			RouteValueDictionary attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return helper.ResponsiveImage(path, altText, attributesDictionary);
		}

		public static ResponsiveImageTag ResponsiveImage(this HtmlHelper helper, string path, string altText, IDictionary<string, object> htmlAttributes)
		{
			var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

			return new ResponsiveImageTag(path, altText, htmlAttributes, urlHelper.Content);
		}
	}
}