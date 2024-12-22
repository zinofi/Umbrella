﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers.Options;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Imaging.Abstractions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers;

/// <summary>
/// A tag helper used to generate img tags have the correct URLs for use with the DynamicImage infrastructure.
/// </summary>
/// <seealso cref="DynamicImageTagHelperBase" />
[OutputElementHint("img")]
[HtmlTargetElement("dynamic-image", Attributes = RequiredAttributeNames, TagStructure = TagStructure.WithoutEndTag)]
public class DynamicImageTagHelper : DynamicImageTagHelperBase
{
	/// <summary>
	/// Gets or sets the size widths.
	/// </summary>
	public string? SizeWidths { get; set; }

	/// <summary>
	/// Gets the name of the output tag.
	/// </summary>
	protected override string OutputTagName => "img";

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageTagHelper"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="cache">The cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="responsiveImageHelper">The responsive image helper.</param>
	/// <param name="dynamicImageUtility">The dynamic image utility.</param>
	/// <param name="umbrellaHostingEnvironment">The umbrella hosting environment.</param>
	/// <param name="dynamicImageTagHelperOptions">The dynamic image tag helper options.</param>
	public DynamicImageTagHelper(
		ILogger<DynamicImageTagHelper> logger,
		IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment,
		IHybridCache cache,
		ICacheKeyUtility cacheKeyUtility,
		IResponsiveImageHelper responsiveImageHelper,
		IDynamicImageUtility dynamicImageUtility,
		DynamicImageTagHelperOptions dynamicImageTagHelperOptions)
		: base(logger, umbrellaHostingEnvironment, cache, cacheKeyUtility, responsiveImageHelper, dynamicImageUtility, dynamicImageTagHelperOptions)
	{
	}

	/// <summary>
	/// Asynchronously executes the <see cref="TagHelper"/> with the given <paramref name="context"/> and <paramref name="output"/>.
	/// </summary>
	/// <param name="context">Contains information associated with the current HTML tag.</param>
	/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		Guard.IsNotNull(context);
		Guard.IsNotNull(output);

		//If we don't have any size information just call into the base method
		IReadOnlyCollection<int>? lstSizeWidth = SizeWidths is not null ? ResponsiveImageHelper.GetParsedIntegerItems(SizeWidths) : null;

		if (lstSizeWidth is null || lstSizeWidth.Count is 0)
		{
			await base.ProcessAsync(context, output).ConfigureAwait(false);
		}
		else if (SizeWidths is not null)
		{
			string src = BuildCoreTag(output);

			string cacheKey = CacheKeyUtility.Create<DynamicImageTagHelper>($"{src}:{MaxPixelDensity}:{SizeWidths}");
			string srcsetValue = Cache.GetOrCreate(
				cacheKey,
				() => ResponsiveImageHelper.GetSizeSrcSetValue(src, SizeWidths, MaxPixelDensity, WidthRequest, HeightRequest, x =>
				{
					var options = new DynamicImageOptions(src, x.imageWidth, x.imageHeight, ResizeMode, ImageFormat);

					string virtualPath = DynamicImageUtility.GenerateVirtualPath(DynamicImageTagHelperOptions.DynamicImagePathPrefix, options);

					return ResolveImageUrl(virtualPath);
				}),
				() => TimeSpan.FromHours(1),
				priority: CacheItemPriority.Low);

			if (!string.IsNullOrWhiteSpace(srcsetValue))
				output.Attributes.Add("srcset", srcsetValue);
		}
	}
}