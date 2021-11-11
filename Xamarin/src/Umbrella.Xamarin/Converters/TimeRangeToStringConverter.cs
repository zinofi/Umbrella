using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	public class TimeRangeToStringConverter : IMultiValueConverter
	{
		/// <inheritdoc />
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			TimeSpan?[] timeSpans = values.OfType<TimeSpan?>().ToArray();

			return timeSpans.Length is 2 && timeSpans.All(x => x.HasValue)
				? FormatTime(timeSpans[0]!.Value) + " - " + FormatTime(timeSpans[1]!.Value)
				: parameter;
		}

		/// <inheritdoc />
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

		private string FormatTime(in TimeSpan time)
		{
			string period = time.Hours < 12 ? "AM" : "PM";
			int periodHours = time.Hours > 12 ? time.Hours - 12 : time.Hours;

			return $"{periodHours:00}:{time.Minutes:00} {period}";
		}
	}
}