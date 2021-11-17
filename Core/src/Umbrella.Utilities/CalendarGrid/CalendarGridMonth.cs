// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Umbrella.Utilities;
using Umbrella.Utilities.ObjectModel;

namespace Umbrella.Utilities.CalendarGrid
{
	public class CalendarGridMonth<TStatus, TData> : UmbrellaObservableRangeCollection<CalendarGridDay<TStatus, TData>>
		where TStatus : struct, Enum
		where TData : class
	{
		public string Name { get; }
		public int Month { get; }
		public int Year { get; }

		public CalendarGridMonth(int month, int year)
			: this(month, year, Array.Empty<CalendarGridDay<TStatus, TData>>())
		{
		}

		public CalendarGridMonth(int month, int year, IEnumerable<CalendarGridDay<TStatus, TData>> collection)
			: base(collection)
		{
			Guard.ArgumentInRange(month, nameof(month), 1, 12);
			Guard.ArgumentInRange(year, nameof(year), 1, 9999);

			Month = month;
			Year = year;

			Name = month switch
			{
				1 => "January",
				2 => "February",
				3 => "March",
				4 => "April",
				5 => "May",
				6 => "June",
				7 => "July",
				8 => "August",
				9 => "September",
				10 => "October",
				11 => "November",
				12 => "December",
				_ => ""
			};
		}
	}
}