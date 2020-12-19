using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions
{
	/// <summary>
	/// Defines the contract for a utility that can generate HTML script and style/link tags
	/// for embedding names bundles inside a HTML document.
	/// </summary>
	public interface IMvcBundleUtility
	{
		/// <summary>
		/// Generates a script tag with a path to the specified bundle.
		/// </summary>
		/// <param name="bundleName">Name of the bundle.</param>
		/// <returns>The generated script tag.</returns>
		MvcHtmlString? GetScript(string bundleName);

		/// <summary>
		/// Generates a script tag whose tag content is the content contained within the specified bundle.
		/// </summary>
		/// <param name="bundleName">Name of the bundle.</param>
		/// <returns>The generated script tag.</returns>
		MvcHtmlString? GetScriptInline(string bundleName);

		/// <summary>
		/// Generates a link tag with a path to the specified bundle.
		/// </summary>
		/// <param name="bundleName">Name of the bundle.</param>
		/// <returns>The generated link tag.</returns>
		MvcHtmlString? GetStyleSheet(string bundleName);

		/// <summary>
		/// Generates a style tag whose tag content is the content contained within the specified bundle.
		/// </summary>
		/// <param name="bundleName">Name of the bundle.</param>
		/// <returns>The generated style tag.</returns>
		MvcHtmlString? GetStyleSheetInline(string bundleName);
	}
}