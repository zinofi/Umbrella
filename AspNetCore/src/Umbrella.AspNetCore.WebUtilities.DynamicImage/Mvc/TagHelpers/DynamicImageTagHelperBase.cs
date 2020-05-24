using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers
{
	/// <summary>
	/// The base class used for all Dynamic Image tag helpers.
	/// </summary>
	/// <seealso cref="ResponsiveImageTagHelper" />
	public abstract class DynamicImageTagHelperBase : ResponsiveImageTagHelper
	{
		#region Protected Constants
		/// <summary>
		/// The required attribute names
		/// </summary>
		protected const string RequiredAttributeNames = "src," + WidthRequestAttributeName + "," + HeightRequestAttributeName + "," + ResizeModeAttributeName;

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
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the <see cref="IDynamicImageUtility"/>.
		/// </summary>
		protected IDynamicImageUtility DynamicImageUtility { get; }

		/// <summary>
		/// Gets the name of the output tag. This is abstract and always overridden.
		/// </summary>
		protected abstract string OutputTagName { get; }
		#endregion

		// TODO: Can we not register an options class and use that??
		/// <summary>
		/// This is the path prefix used for all generated image urls unless overridden using the "path-prefix" attribute on the tag.
		/// </summary>
		public static string GlobalDynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

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
		/// Gets or sets the <see cref="DynamicResizeMode"/>.
		/// </summary>
		[HtmlAttributeName(ResizeModeAttributeName)]
		public DynamicResizeMode ResizeMode { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="DynamicImageFormat"/>.
		/// </summary>
		public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;

		/// <summary>
		/// Gets or sets the dynamic image path prefix. This defaults to "dynamicimage".
		/// </summary>
		[HtmlAttributeName("path-prefix")]
		public string DynamicImagePathPrefix { get; set; } = GlobalDynamicImagePathPrefix;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicImageTagHelperBase"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="memoryCache">The memory cache.</param>
		/// <param name="dynamicImageUtility">The dynamic image utility.</param>
		/// <param name="umbrellaHostingEnvironment">The umbrella hosting environment.</param>
		public DynamicImageTagHelperBase(
			ILogger<DynamicImageTagHelperBase> logger,
			IMemoryCache memoryCache,
			IDynamicImageUtility dynamicImageUtility,
			IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
			: base(logger, umbrellaHostingEnvironment, memoryCache)
		{
			DynamicImageUtility = dynamicImageUtility;
		}

		/// <summary>
		/// Asynchronously executes the <see cref="TagHelper"/> with the given <paramref name="context"/> and <paramref name="output"/>.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			BuildCoreTag(output);

			await base.ProcessAsync(context, output);
		}

		/// <summary>
		/// Builds the core tag and returns the 'src' attribute.
		/// </summary>
		/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
		/// <returns>The 'src' attribute of the tag.</returns>
		protected string BuildCoreTag(TagHelperOutput output)
		{
			Guard.ArgumentInRange(WidthRequest, nameof(WidthRequestAttributeName), 1);
			Guard.ArgumentInRange(HeightRequest, nameof(HeightRequestAttributeName), 1);
			Guard.ArgumentNotNullOrWhiteSpace(DynamicImagePathPrefix, nameof(DynamicImagePathPrefix));

			TagHelperAttribute attrSrc = output.Attributes["src"];
			string? src = attrSrc?.Value?.ToString()?.Trim();

			if (string.IsNullOrEmpty(src))
				throw new Exception("src cannot be null or empty.");

			var options = new DynamicImageOptions(src, WidthRequest, HeightRequest, ResizeMode, ImageFormat);

			string x1Url = DynamicImageUtility.GenerateVirtualPath(DynamicImagePathPrefix, options);

			output.Attributes.Remove(attrSrc);
			output.Attributes.Add("src", ResolveImageUrl(x1Url));

			output.TagName = OutputTagName;

			return src;
		}
	}
}