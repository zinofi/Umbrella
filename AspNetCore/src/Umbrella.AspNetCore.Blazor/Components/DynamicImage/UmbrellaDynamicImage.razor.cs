using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Components.ResponsiveImage;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.Blazor.Components.DynamicImage
{
	public class UmbrellaDynamicImageBase : UmbrellaResponsiveImage
	{
		[Inject]
		protected IDynamicImageUtility DynamicImageUtility { get; set; } = null!;

		[Parameter]
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

		[Parameter]
		public int WidthRequest { get; set; } = 1;

		[Parameter]
		public int HeightRequest { get; set; } = 1;

		[Parameter]
		public DynamicResizeMode ResizeMode { get; set; } = DynamicResizeMode.UniformFill;

		[Parameter]
		public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;

		[Parameter]
		public string? SizeWidths { get; set; }

		[Parameter]
		public string? StripPrefix { get; set; }

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

			IReadOnlyCollection<int> lstSizeWidth = ResponsiveImageHelper.GetParsedIntegerItems(SizeWidths);

			if (lstSizeWidth.Count is 0)
			{
				SrcSetValue = ResponsiveImageHelper.GetPixelDensitySrcSetValue(SrcValue, MaxPixelDensity);
			}
			else
			{
				SrcSetValue = ResponsiveImageHelper.GetSizeSrcSetValue(strippedUrl, SizeWidths, MaxPixelDensity, WidthRequest, HeightRequest, x =>
				{
					var options = new DynamicImageOptions(strippedUrl, x.imageWidth, x.imageHeight, ResizeMode, ImageFormat);

					return DynamicImageUtility.GenerateVirtualPath(DynamicImagePathPrefix, options).TrimStart('~').Replace("//", "/");
				});
			}
		}

		private string StripUrlPrefix(string url) => !string.IsNullOrEmpty(StripPrefix) ? url.Remove(0, StripPrefix.Length) : url;
	}
}