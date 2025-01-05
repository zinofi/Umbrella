using System.Globalization;
using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Converters;

/// <summary>
/// A converter that converts to / from a boolean if the binding value is an integer and if so, if it matches the parsed string value provided as the converter parameter.
/// </summary>
/// <seealso cref="IValueConverter" />
public class IntegerToggledConverter : IValueConverter
{
	/// <inheritdoc />
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is int answer && parameter is string strOption && int.TryParse(strOption, out int option) && answer == option;

	/// <inheritdoc />
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool selected && selected && parameter is string strOption && int.TryParse(strOption, out int option) ? option : (int?)null;
}