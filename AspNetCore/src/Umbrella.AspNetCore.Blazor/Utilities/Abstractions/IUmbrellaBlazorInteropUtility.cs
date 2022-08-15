// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AspNetCore.Blazor.Utilities.Abstractions;

/// <summary>
/// A utility containing core interop functionality between Blazor and JavaScript for features not yet supported
/// natively by Blazor.
/// </summary>
public interface IUmbrellaBlazorInteropUtility
{
	/// <summary>
	/// An event that is raised when the browser window is scrolled to the top.
	/// </summary>
	event AwaitableBlazorEventHandler OnWindowScrolledTop;

	/// <summary>
	/// Animates the scroll position of the browser window to the specified position.
	/// </summary>
	/// <param name="scrollY">The vertical position.</param>
	/// <param name="offset">The vertical offset.</param>
	/// <returns>A <see cref="ValueTask"/> that completes when the operation has completed.</returns>
	ValueTask AnimateScrollToAsync(int scrollY, int offset = 0);

	/// <summary>
	/// Animates the scroll position of the browser window to the specified element.
	/// </summary>
	/// <param name="elementSelector">The element selector specified using CSS selector syntax.</param>
	/// <param name="offset">The vertical offset.</param>
	/// <returns>A <see cref="ValueTask"/> that completes when the operation has completed.</returns>
	ValueTask AnimateScrollToAsync(string elementSelector, int offset = 0);

	/// <summary>
	/// Animates the scroll position of the browser window to the bottom of the document.
	/// </summary>
	/// <returns>A <see cref="ValueTask"/> that completes when the operation has completed.</returns>
	ValueTask AnimateScrollToBottomAsync();

	/// <summary>
	/// Sets the value for the <![CDATA[<title>]]> tag in the HTML <![CDATA[<head>]]> element.
	/// </summary>
	/// <param name="pageTitle">The page title.</param>
	/// <returns>A <see cref="ValueTask"/> that completes when the operation has completed.</returns>
	ValueTask SetPageTitleAsync(string pageTitle);
}