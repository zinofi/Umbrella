using System.Globalization;
using Umbrella.Xamarin.Exceptions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters;

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

		return value.Equals(EnumToggledConverter<T>.GetParameterValue(parameter));
	}

	/// <inheritdoc />
	public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool selected && selected)
			return EnumToggledConverter<T>.GetParameterValue(parameter);

		return null;
	}

	private static T GetParameterValue(object parameter) => parameter switch
	{
		T typedParameter => typedParameter,
		BindableObject bindable when bindable.BindingContext is T t => t,
		_ => throw new UmbrellaXamarinException("The parameter cannot be converted to the specified enum type.")
	};
}