using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
	/// <summary>
	/// Extension methods for use with the <see cref="HtmlHelper"/> type, specifically for manipulating the HTML head content.
	/// </summary>
	public static class HtmlHeadHelpers
    {
		/// <summary>
		/// Outputs a meta tag using the specified parameters.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="name">The name.</param>
		/// <param name="content">The content.</param>
		/// <returns>The HTML string of the tag.</returns>
		public static MvcHtmlString? MetaTag(this HtmlHelper helper, string name, string content)
        {
            if(!string.IsNullOrWhiteSpace(content))
            {
                var tb = new TagBuilder("meta");
                tb.MergeAttribute("name", name);
                tb.MergeAttribute("content", content);

                return new MvcHtmlString(tb.ToString(TagRenderMode.SelfClosing));
            }

            return null;
        }
    }
}
