using Microsoft.Maui.Controls;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Maui.Converters;

namespace Umbrella.Maui.MarkupExtensions;

/// <summary>
/// A markup extensions used to generate image URLs for use with the Dynamic Image infrastructure.
/// </summary>
/// <seealso cref="UmbrellaResponsiveImageExtension" />
public class UmbrellaDynamicImageExtension : UmbrellaResponsiveImageExtension
{
	/// <summary>
	/// Gets or sets the dynamic image path prefix. Defaults to <see cref="DynamicImageConstants.DefaultPathPrefix"/>.
	/// </summary>
	public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

	/// <summary>
	/// Gets or sets the width request in pixels. Defaults to 1.
	/// </summary>
	public int WidthRequest { get; set; } = 1;

	/// <summary>
	/// Gets or sets the height request in pixels. Defaults to 1.
	/// </summary>
	public int HeightRequest { get; set; } = 1;

	/// <summary>
	/// Gets or sets the resize mode. Defaults to <see cref="DynamicResizeMode.Crop"/>.
	/// </summary>
	/// <remarks>
	/// For more information on how these resize modes work, please refer to the <see cref="DynamicResizeMode"/> code documentation.
	/// </remarks>
	public DynamicResizeMode ResizeMode { get; set; } = DynamicResizeMode.Crop;

	/// <summary>
	/// Gets or sets the image format. Defaults to <see cref="DynamicImageFormat.Jpeg"/>.
	/// </summary>
	public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;

	/// <summary>
	/// Gets or sets the prefix to be stripped from the <see cref="UmbrellaResponsiveImageExtension.Path"/>.
	/// </summary>
	public string? StripPrefix { get; set; }

	/// <inheritdoc />
	public override BindingBase ProvideValue(IServiceProvider serviceProvider)
	{
		var converter = new UmbrellaDynamicImageConverter(MaxPixelDensity, Converter, DynamicImagePathPrefix, WidthRequest, HeightRequest, ResizeMode, ImageFormat, StripPrefix);

		return new Binding(Path, Mode, converter, ConverterParameter, StringFormat, Source);
	}
}