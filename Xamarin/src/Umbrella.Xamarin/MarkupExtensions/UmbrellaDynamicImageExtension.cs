using System;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Xamarin.Converters;
using Xamarin.Forms;

namespace Umbrella.Xamarin.MarkupExtensions
{
	public class UmbrellaDynamicImageExtension : UmbrellaResponsiveImageExtension
	{
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;
		public int WidthRequest { get; set; } = 1;
		public int HeightRequest { get; set; } = 1;
		public DynamicResizeMode ResizeMode { get; set; } = DynamicResizeMode.UniformFill;
		public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;
		public string? StripPrefix { get; set; }

		/// <inheritdoc />
		public override BindingBase ProvideValue(IServiceProvider serviceProvider)
		{
			var converter = new UmbrellaDynamicImageConverter(MaxPixelDensity, Converter, DynamicImagePathPrefix, WidthRequest, HeightRequest, ResizeMode, ImageFormat, StripPrefix);

			return new Binding(Path, Mode, converter, ConverterParameter, StringFormat, Source);
		}
	}
}