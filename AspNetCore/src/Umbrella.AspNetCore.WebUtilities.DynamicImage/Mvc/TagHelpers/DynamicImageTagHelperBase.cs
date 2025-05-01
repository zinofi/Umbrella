// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers.Options;
using Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Imaging.Abstractions;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers;

/// <summary>
/// The base class used for all Dynamic Image tag helpers.
/// </summary>
/// <seealso cref="ResponsiveImageTagHelper" />
public abstract class DynamicImageTagHelperBase : ResponsiveImageTagHelper
{
	/// <summary>
	/// The required attribute names
	/// </summary>
	protected const string RequiredAttributeNames = "src," + WidthRequestAttributeName + "," + HeightRequestAttributeName;

	/// <summary>
	/// The width request attribute name
	/// </summary>
	protected const string WidthRequestAttributeName = "width-request";

	/// <summary>
	/// The height request attribute name
	/// </summary>
	protected const string HeightRequestAttributeName = "height-request";

	/// <summary>
	/// The resize mode attribute name
	/// </summary>
	protected const string ResizeModeAttributeName = "resize-mode";	
	/// <summary>
	/// Gets the <see cref="IDynamicImageUtility"/>.
	/// </summary>
	protected IDynamicImageUtility DynamicImageUtility { get; }

	/// <summary>
	/// Gets the dynamic image tag helper options.
	/// </summary>
	protected DynamicImageTagHelperOptions DynamicImageTagHelperOptions { get; }

	/// <summary>
	/// Gets the name of the output tag. This is abstract and always overridden.
	/// </summary>
	protected abstract string OutputTagName { get; }

	/// <summary>
	/// Gets or sets the width request in pixels.
	/// </summary>
	[HtmlAttributeName(WidthRequestAttributeName)]
	public int WidthRequest { get; set; }

	/// <summary>
	/// Gets or sets the height request in pixels.
	/// </summary>
	[HtmlAttributeName(HeightRequestAttributeName)]
	public int HeightRequest { get; set; }

	/// <summary>
	/// Gets or sets the resize mode. Defaults to <see cref="DynamicResizeMode.Crop"/>.
	/// </summary>
	/// <remarks>
	/// For more information on how these resize modes work, please refer to the <see cref="DynamicResizeMode"/> code documentation.
	/// </remarks>
	[HtmlAttributeName(ResizeModeAttributeName)]
	public DynamicResizeMode ResizeMode { get; set; } = DynamicResizeMode.Crop;

	/// <summary>
	/// Gets or sets the string that is prepended onto the value of the "src" attribute.
	/// </summary>
	[HtmlAttributeName("src-prefix")]
	public string? SrcPrefix { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="DynamicImageFormat"/>.
	/// </summary>
	public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageTagHelperBase"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dynamicImageUtility">The dynamic image utility.</param>
	/// <param name="umbrellaHostingEnvironment">The umbrella hosting environment.</param>
	/// <param name="cache">The cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="responsiveImageHelper">The responsive image helper.</param>
	/// <param name="dynamicImageTagHelperOptions">The dynamic image tag helper options.</param>
	protected DynamicImageTagHelperBase(
		ILogger<DynamicImageTagHelperBase> logger,
		IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment,
		IMemoryCache cache,
		ICacheKeyUtility cacheKeyUtility,
		IResponsiveImageHelper responsiveImageHelper,
		IDynamicImageUtility dynamicImageUtility,
		DynamicImageTagHelperOptions dynamicImageTagHelperOptions)
		: base(logger, umbrellaHostingEnvironment, cache, cacheKeyUtility, responsiveImageHelper)
	{
		DynamicImageUtility = dynamicImageUtility;
		DynamicImageTagHelperOptions = dynamicImageTagHelperOptions;
	}

	/// <summary>
	/// Asynchronously executes the <see cref="TagHelper"/> with the given <paramref name="context"/> and <paramref name="output"/>.
	/// </summary>
	/// <param name="context">Contains information associated with the current HTML tag.</param>
	/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		_ = BuildCoreTag(output);

		await base.ProcessAsync(context, output).ConfigureAwait(false);
	}

	/// <summary>
	/// Builds the core tag and returns the <c>src</c> attribute.
	/// </summary>
	/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
	/// <returns>The <c>src</c> attribute of the tag.</returns>
	protected string BuildCoreTag(TagHelperOutput output)
	{
		Guard.IsNotNull(output);
		Guard.IsGreaterThan(WidthRequest, 0);
		Guard.IsGreaterThan(HeightRequest, 0);

		TagHelperAttribute attrSrc = output.Attributes["src"];
		string? src = attrSrc?.Value?.ToString()?.Trim();

		if (string.IsNullOrEmpty(src))
			throw new UmbrellaWebException("src cannot be null or empty.");

		var options = new DynamicImageOptions(src, WidthRequest, HeightRequest, ResizeMode, ImageFormat);

		string x1Url = GenerateVirtualPath(options);

		_ = output.Attributes.Remove(attrSrc);
		output.Attributes.Add("src", ResolveImageUrl(x1Url));

		output.TagName = OutputTagName;

		return src;
	}
	
	/// <inheritdoc/>
	protected override string ResolveImageUrl(string url)
	{
		Guard.IsNotNullOrWhiteSpace(url);

		if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			return url;

		string output = base.ResolveImageUrl(url);

		if(!string.IsNullOrWhiteSpace(SrcPrefix))
			output = SrcPrefix.TrimToLowerInvariant() + output;

		return output;
	}

	/// <summary>
	/// Generates the virtual path for the image.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <returns>The virtual path.</returns>
	protected virtual string GenerateVirtualPath(DynamicImageOptions options)
	{
		Guard.IsNotNull(options);

		return DynamicImageUtility.GenerateVirtualPath(DynamicImageTagHelperOptions.DynamicImagePathPrefix, options);
	}
}