using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	public class AllTrueMultiConverter : IMultiValueConverter
	{
		/// <inheritdoc />
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			=> values is null || !targetType.IsAssignableFrom(typeof(bool)) ? false : (object)values.OfType<bool>().All(x => x);

		/// <inheritdoc />
		public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}