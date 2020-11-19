using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	public class IntegerToggledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is int answer && parameter is string strOption && int.TryParse(strOption, out int option) && answer == option;
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool selected && selected && parameter is string strOption && int.TryParse(strOption, out int option) ? option : (int?)null;
		}
	}
}