using System;
using System.Globalization;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	// TODO: Remove this. There is a EnumToBoolConverter inside the Xamarin Community Toolkit that does the same job.

	/// <summary>
	/// A converter that converts to / from a boolean if the binding value is an enum and if so,
	/// if it matches the enum value provided as the converter parameter. Primarily used in conjunction
	/// with the <see cref="Controls.ToggleButton" /> and <see cref="Controls.ToggleImageButton" /> where
	/// we need two-way binding.
	/// </summary>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <seealso cref="IValueConverter" />
	/// <remarks>
	/// This converter differs from the <see cref="global::Xamarin.CommunityToolkit.Converters.EnumToBoolConverter"/> because this
	/// allows the binding to be updated both ways because <see cref="ConvertBack(object, Type, object, CultureInfo)"/> has been implemented here whereas the XCT version
	/// does not implement this.
	/// </remarks>
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