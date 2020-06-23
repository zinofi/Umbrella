using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers.Options;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers
{
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
		/// <param name="memoryCache">The memory cache.</param>
		/// <param name="dynamicImageUtility">The dynamic image utility.</param>
		/// <param name="umbrellaHostingEnvironment">The umbrella hosting environment.</param>
		/// <param name="dynamicImageTagHelperOptions">The dynamic image tag helper options.</param>
		public DynamicImageTagHelper(
			ILogger<DynamicImageTagHelper> logger,
			IMemoryCache memoryCache,
			IDynamicImageUtility dynamicImageUtility,
			IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment,
			DynamicImageTagHelperOptions dynamicImageTagHelperOptions)
			: base(logger, memoryCache, dynamicImageUtility, umbrellaHostingEnvironment, dynamicImageTagHelperOptions)
		{
		}

		/// <summary>
		/// Asynchronously executes the <see cref="TagHelper"/> with the given <paramref name="context"/> and <paramref name="output"/>.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			//If we don't have any size information just call into the base method
			IReadOnlyCollection<int> lstSizeWidth = GetParsedItems(SizeWidths);

			if (lstSizeWidth.Count == 0)
			{
				await base.ProcessAsync(context, output);
			}
			else
			{
				string src = BuildCoreTag(output);

				string srcsetValue = Cache.GetOrCreate(GetCacheKey(src, PixelDensities, SizeWidths ?? ""), entry =>
				{
					entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

					return string.Join(", ", GetSizeStrings(src).Distinct().OrderBy(x => x));
				});

				if (!string.IsNullOrWhiteSpace(srcsetValue))
					output.Attributes.Add("srcset", srcsetValue);
			}
		}

		#region Private Methods
		private IReadOnlyCollection<int> GetSizeWidths() => GetParsedItems(SizeWidths);

		private IEnumerable<string> GetSizeStrings(string path)
		{
			float aspectRatio = WidthRequest / (float)HeightRequest;

			foreach (int sizeWidth in GetSizeWidths())
			{
				foreach (int density in GetPixelDensities())
				{
					int imgWidth = sizeWidth * density;
					int imgHeight = (int)Math.Ceiling(imgWidth / aspectRatio);

					var options = new DynamicImageOptions(path, imgWidth, imgHeight, ResizeMode, ImageFormat);

					string virtualPath = DynamicImageUtility.GenerateVirtualPath(DynamicImageTagHelperOptions.DynamicImagePathPrefix, options);

					string resolvedUrl = ResolveImageUrl(virtualPath);

					yield return $"{resolvedUrl} {imgWidth}w";
				}
			}
		}
		#endregion
	}
}