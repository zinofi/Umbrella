using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Contains useful DateTime extension methods
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns a string representation of the time between the date and the current DateTime according to the following:
		/// 1) Under 1 minute - "Just now"
		/// 2) Under 1 hour - "xx minute(s) ago"
		/// 3) Under 1 day - "xx hour(s) ago"
		/// 4) Under 7 days - "xx day(s) ago"
		/// 5) >= 7 days - the date according to the specified format
		/// </summary>
		/// <param name="date">The date being dealt with.</param>
		/// <param name="format">The format for a DateTime value to be returned in. Defaults to a short date string.</param>
		/// <param name="dtComparison">Allows a reference value to be passed. By default this method use DateTime.Now for comparison.</param>
		/// <returns></returns>
		public static string ToTimeSinceString(this DateTime date, string format = "d", DateTime? dtComparison = null)
		{
			//Get the current date
			TimeSpan ts = DateTime.Now - date;

			//See if a value has been passed for dtComparison - if so, use that
			if (dtComparison.HasValue)
				ts = dtComparison.Value - date;

			//Depending on how far the date is from the current date alter what is return
			if(ts.Days >= 7)
				return date.ToString(format);
			else if (ts.Days > 0)
			{
				if (ts.Days > 1)
					return ts.Days + " days ago";
				else
					return "1 day ago";
			}
			else if(ts.Hours > 0)
			{
				if (ts.Hours > 1)
					return ts.Hours + " hours ago";
				else
					return "1 hour ago";
			}
			else if (ts.Minutes > 0)
			{
				if (ts.Minutes > 1)
					return ts.Minutes + " minutes ago";
				else
					return "1 minute ago";
			}
			else
				return "Just now";
		}
	}
}