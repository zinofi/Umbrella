using System;
using Umbrella.Xamarin.Converters;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Umbrella.Xamarin.MarkupExtensions;

/// <summary>
/// A custom Markup Extension used to generate a version of the URL that contains pixel density data by looking at the pixel density
/// of the device the app is running on and then determining the optimal image resolution. This results in, e.g. /images/garden.png being
/// rewritten to include the optimal density value, e.g. /images/garden@3x.png, etc.
/// </summary>
/// <seealso cref="BindableObject" />
/// <seealso cref="IMarkupExtension{BindingBase}" />
[ContentProperty("Path")]
[AcceptEmptyServiceProvider]
public class UmbrellaResponsiveImageExtension : BindableObject, IMarkupExtension<BindingBase>
{
	/// <summary>
	/// Gets the <see cref="BindingMode"/> being used.
	/// </summary>
	protected BindingMode Mode { get; } = BindingMode.Default;

	/// <summary>
	/// Gets or sets the converter that is applied to the <see cref="Path"/> first before the responsive URL is created. This is useful for cases where
	/// for cases where preprocessing needs to occur, e.g. to convert the URL to absolute.
	/// </summary>
	public IValueConverter? Converter { get; set; }

	/// <summary>
	/// Gets or sets the converter parameter used by the <see cref="Converter"/>.
	/// </summary>
	public object? ConverterParameter { get; set; }

	/// <summary>
	/// Gets or sets the path of the image.
	/// </summary>
	public string? Path { get; set; }

	/// <summary>
	/// Gets or sets the string format.
	/// </summary>
	public string? StringFormat { get; set; }

	/// <summary>
	/// Gets or sets the source.
	/// </summary>
	public object? Source { get; set; }

	/// <summary>
	/// Gets or sets the maximum pixel density. Defaults to 1.
	/// </summary>
	public int MaxPixelDensity { get; set; } = 1;

	/// <inheritdoc />
	public virtual BindingBase ProvideValue(IServiceProvider serviceProvider)
	{
		var converter = new UmbrellaResponsiveImageConverter(MaxPixelDensity, Converter);

		return new Binding(Path, Mode, converter, ConverterParameter, StringFormat, Source);
	}

	/// <inheritdoc />
	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ((IMarkupExtension<string>)this).ProvideValue(serviceProvider);
}