using System.Threading.Tasks;

namespace Umbrella.AspNetCore.Blazor.Utilities.Abstractions
{
	/// <summary>
	/// A utility containing core interop functionality between Blazor and JavaScript for features not yet supported
	/// natively by Blazor.
	/// </summary>
	public interface IUmbrellaBlazorInteropUtility
	{
		event AwaitableBlazorEventHandler OnWindowScrolledTop;

		ValueTask AnimateScrollToAsync(int scrollY, int offset = 0);
		ValueTask AnimateScrollToAsync(string elementSelector, int offset = 0);
		ValueTask AnimateScrollToBottomAsync();

		/// <summary>
		/// Sets the value for the <![CDATA[<title>]]> tag in the HTML <![CDATA[<head>]]> element.
		/// </summary>
		/// <param name="pageTitle">The page title.</param>
		/// <returns>A <see cref="ValueTask"/> that completes when the operation has completed.</returns>
		ValueTask SetPageTitleAsync(string pageTitle);
	}
}