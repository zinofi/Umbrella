using System.Globalization;
using Umbrella.Utilities.Extensions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters;

/// <summary>
/// A Value Converter that converts instance of <see cref="DateTime"/> to long dates with ordinal day numbers,
/// e.g. 25/12/2020 -> Friday 25th December, 2020
/// </summary>
/// <seealso cref="IValueConverter" />
public class LongDateTimeOrdinalConverter : IValueConverter
{
	/// <inheritdoc />
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		=> value is DateTime dt ? $"{dt:dddd} {dt.Day.ToOrdinalString()} {dt:MMMM}, {dt:yyyy}" : null;

	/// <inheritdoc />
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}