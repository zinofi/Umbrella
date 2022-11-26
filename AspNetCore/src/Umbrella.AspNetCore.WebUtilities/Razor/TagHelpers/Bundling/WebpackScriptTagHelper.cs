using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers.Bundling;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Security;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers.Bundling;

/// <summary>
/// A tag helper used to output script elements for named Webpack js bundles which either point to those bundles or render them inline.
/// </summary>
[HtmlTargetElement("webpack-script", Attributes = "name", TagStructure = TagStructure.WithoutEndTag)]
public class WebpackScriptTagHelper : BundleScriptTagHelper<IWebpackBundleUtility>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="WebpackScriptTagHelper"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="bundleUtility">The bundle utility.</param>
	/// <param name="nonceContext">The nonce context.</param>
	public WebpackScriptTagHelper(
		ILogger<WebpackScriptTagHelper> logger,
		IWebpackBundleUtility bundleUtility,
		Lazy<NonceContext> nonceContext)
		: base(logger, bundleUtility, nonceContext)
	{
	}
}