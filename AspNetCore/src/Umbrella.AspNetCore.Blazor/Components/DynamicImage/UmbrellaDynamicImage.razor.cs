using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Components.ResponsiveImage;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.Blazor.Components.DynamicImage
{
	public partial class UmbrellaDynamicImage : UmbrellaResponsiveImage
	{
		/// <summary>
		/// Gets or sets the dynamic image utility.
		/// </summary>
		[Inject]
		protected IDynamicImageUtility DynamicImageUtility { get; set; } = null!;

		/// <summary>
		/// Gets or sets the dynamic image path prefix. Defaults to <see cref="DynamicImageConstants.DefaultPathPrefix"/>.
		/// </summary>
		[Parameter]
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

		/// <summary>
		/// Gets or sets the width request in pixels. Defaults to 1.
		/// </summary>
		[Parameter]
		public int WidthRequest { get; set; } = 1;

		/// <summary>
		/// Gets or sets the height request in pixels. Defaults to 1.
		/// </summary>
		[Parameter]
		public int HeightRequest { get; set; } = 1;

		/// <summary>
		/// Gets or sets the resize mode. Defaults to <see cref="DynamicResizeMode.UniformFill"/>.
		/// </summary>
		/// <remarks>
		/// For more information on how these resize modes work, please refer to the <see cref="DynamicResizeMode"/> code documentation.
		/// </remarks>
		[Parameter]
		public DynamicResizeMode ResizeMode { get; set; } = DynamicResizeMode.UniformFill;

		/// <summary>
		/// Gets or sets the image format. Defaults to <see cref="DynamicImageFormat.Jpeg"/>.
		/// </summary>
		[Parameter]
		public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;

		/// <summary>
		/// Gets or sets the size widths.
		/// </summary>
		[Parameter]
		public string? SizeWidths { get; set; }

		/// <summary>
		/// Gets or sets the prefix to be stripped from the <see cref="UmbrellaResponsiveImageBase.Url"/>.
		/// </summary>
		[Parameter]
		public string? StripPrefix { get; set; }

		/// <summary>
		/// Gets or sets the source value.
		/// </summary>
		protected string SrcValue { get; set; } = null!;

		/// <inheritdoc />
		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			Guard.ArgumentInRange(WidthRequest, nameof(WidthRequest), 1);
			Guard.ArgumentInRange(HeightRequest, nameof(HeightRequest), 1);
		}

		/// <inheritdoc />
		protected override void InitializeImage()
		{
			if (Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				SrcValue = Url;
				return;
			}

			string strippedUrl = StripUrlPrefix(Url);

			var options = new DynamicImageOptions(strippedUrl, WidthRequest, HeightRequest, ResizeMode, ImageFormat);

			SrcValue = DynamicImageUtility.GenerateVirtualPath(DynamicImagePathPrefix, options).TrimStart('~').Replace("//", "/");

			IReadOnlyCollection<int> lstSizeWidth = ResponsiveImageHelper.GetParsedIntegerItems(SizeWidths ?? "");

			if (lstSizeWidth.Count is 0)
			{
				SrcSetValue = ResponsiveImageHelper.GetPixelDensitySrcSetValue(SrcValue, MaxPixelDensity);
			}
			else
			{
				SrcSetValue = ResponsiveImageHelper.GetSizeSrcSetValue(strippedUrl, SizeWidths ?? "", MaxPixelDensity, WidthRequest, HeightRequest, x =>
				{
					var options = new DynamicImageOptions(strippedUrl, x.imageWidth, x.imageHeight, ResizeMode, ImageFormat);

					return DynamicImageUtility.GenerateVirtualPath(DynamicImagePathPrefix, options).TrimStart('~').Replace("//", "/");
				});
			}
		}

		private string StripUrlPrefix(string url) => !string.IsNullOrEmpty(StripPrefix) ? url.Remove(0, StripPrefix.Length) : url;
	}
}