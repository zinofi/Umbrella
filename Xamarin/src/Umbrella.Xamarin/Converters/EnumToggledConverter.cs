using System;
using System.Globalization;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	public class EnumToggledConverter<T> : IValueConverter
		where T : struct, Enum
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null)
				return false;

			return value.Equals(GetParameterValue(parameter));
		}

		/// <inheritdoc />
		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool selected && selected)
				return GetParameterValue(parameter);

			return null;
		}

		private T GetParameterValue(object parameter) => parameter switch
		{
			T typedParameter => typedParameter,
			BindableObject bindable when bindable.BindingContext is T t => t,
			_ => throw new Exception("The parameter cannot be converted to the specified enum type.")
		};
	}
}