using System;
using System.Globalization;
using Umbrella.Utilities.Extensions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	public class LongDateTimeOrdinalConverter : IValueConverter
	{
		/// <inheritdoc />
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value is DateTime dt ? $"{dt:dddd} {dt.Day.ToOrdinalString()} {dt:MMMM}, {dt:yyyy}" : null;

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}