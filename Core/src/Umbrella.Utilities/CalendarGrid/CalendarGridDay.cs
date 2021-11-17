// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using Umbrella.Utilities.Helpers;

namespace Umbrella.Utilities.CalendarGrid
{
	public class CalendarGridDay<TStatus, TData> : INotifyPropertyChanged
		where TStatus : struct, Enum
		where TData : class
	{
		private TStatus _status;

		public CalendarGridDay(DateTime? dateTime, TStatus status = default)
		{
			if (dateTime.HasValue)
			{
				Day = dateTime.Value.Day;
				Date = dateTime;
				IsWeekend = dateTime.Value.DayOfWeek == DayOfWeek.Saturday || dateTime.Value.DayOfWeek == DayOfWeek.Sunday;
				IsInPast = dateTime.Value < DateTime.Now.Date;
				IsToday = dateTime.Value.Date == DateTime.Now.Date;
			}

			Status = status;
		}

		public int? Day { get; }
		public DateTime? Date { get; }
		public bool IsWeekend { get; }
		public bool IsInPast { get; }
		public bool IsToday { get; }
		public TStatus Status
		{
			get => _status;
			set
			{
				if (Day.HasValue)
					_ = INotifyPropertyChangedHelper.SetProperty(ref _status, value, this, PropertyChanged);
			}
		}

		public TData? Data { get; set; }

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}