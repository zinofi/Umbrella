﻿using System;

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
		/// <returns>A string representing the time that has passed.</returns>
		[Obsolete("Use Humanizer")]
		public static string ToTimeSinceString(this DateTime date, string format = "d", DateTime? dtComparison = null)
		{
			DateTime utcDate = date.ToUniversalTime();

			TimeSpan ts = dtComparison.HasValue
				? dtComparison.Value.ToUniversalTime() - utcDate
				: DateTime.UtcNow - utcDate;

			return ts switch
			{
				var _ when ts.Days >= 7 => date.ToString(format),
				var _ when ts.Days > 1 => ts.Days + " days ago",
				var _ when ts.Days == 1 => "1 day ago",
				var _ when ts.Hours > 1 => ts.Hours + " hours ago",
				var _ when ts.Hours == 1 => "1 hour ago",
				var _ when ts.Minutes > 1 => ts.Minutes + " minutes ago",
				var _ when ts.Minutes == 1 => "1 minute ago",
				_ => "Just now"
			};
		}

		/// <summary>
		/// Changes the specified <paramref name="dateTime"/> instance's <see cref="DateTime.Day"/> property to 1.
		/// </summary>
		/// <param name="dateTime">The datetime to change.</param>
		/// <returns>A new instance with the <see cref="DateTime.Day"/> property set to 1.</returns>
		public static DateTime ToMonthStart(this DateTime dateTime) => dateTime.AddDays(-dateTime.Day + 1);

		/// <summary>
		/// Changes the specified <paramref name="dateTime"/> instance's <see cref="DateTime.Day"/> property to the last day of the month
		/// according to the value of it's <see cref="DateTime.Month"/> and <see cref="DateTime.Year"/> properties.
		/// </summary>
		/// <param name="dateTime">The datetime to change.</param>
		/// <returns>A new instance with the <see cref="DateTime.Day"/> property set to the last day of the month.</returns>
		public static DateTime ToMonthEnd(this DateTime dateTime) => dateTime.AddDays(DateTime.DaysInMonth(dateTime.Year, dateTime.Month) - dateTime.Day);
	}
}