using System;
using Umbrella.Xamarin.Converters;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Umbrella.Xamarin.MarkupExtensions
{
	[ContentProperty("Path")]
	[AcceptEmptyServiceProvider]
	public class UmbrellaResponsiveImageExtension : IMarkupExtension<BindingBase>
	{
		protected BindingMode Mode { get; } = BindingMode.Default;

		public IValueConverter? Converter { get; set; }
		public object? ConverterParameter { get; set; }
		public string? Path { get; set; }
		public string? StringFormat { get; set; }
		public object? Source { get; set; }
		public int MaxPixelDensity { get; set; } = 1;

		public virtual BindingBase ProvideValue(IServiceProvider serviceProvider)
		{
			var converter = new UmbrellaResponsiveImageConverter(MaxPixelDensity, Converter);

			return new Binding(Path, Mode, converter, ConverterParameter, StringFormat, Source);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ((IMarkupExtension<string>)this).ProvideValue(serviceProvider);
	}
}