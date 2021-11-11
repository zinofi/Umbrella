using System;
using System.Globalization;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	public class IntGreaterThanConverter : IValueConverter
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value is int source && ((parameter is int target && source > target) || (int.TryParse(parameter as string, out target) && source > target));

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}