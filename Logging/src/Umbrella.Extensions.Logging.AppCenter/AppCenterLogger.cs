using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.AppCenter
{
	internal class AppCenterLogger : ILogger
	{
		private readonly string _categoryName;
		private readonly LogLevel _minLevel;
		private readonly bool _includeEventId;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppCenterLogger"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="minLevel">The minimum level.</param>
		/// <param name="includeEventId">if set to <c>true</c> [include event identifier].</param>
		public AppCenterLogger(string name, LogLevel minLevel, bool includeEventId)
		{
			_categoryName = name;
			_minLevel = minLevel;
			_includeEventId = includeEventId;
		}

		/// <inheritdoc />
		public IDisposable BeginScope<TState>(TState state) => null;

		/// <inheritdoc />
		public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;

		/// <inheritdoc />
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
				return;

			string message = formatter(state, exception);

			if (exception is null)
			{
				Analytics.TrackEvent(logLevel.ToString(), PopulateProperties(eventId, message));
			}
			else
			{
				Crashes.TrackError(exception, PopulateProperties(eventId, message));
			}
		}

		private Dictionary<string, string> PopulateProperties(in EventId eventId, string message)
		{
			var dicProperties = new Dictionary<string, string>
			{
				["CategoryName"] = _categoryName,
				["Message"] = message
			};

			if (_includeEventId)
			{
				if (eventId.Id != 0)
				{
					dicProperties["EventId"] = eventId.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
				}

				if (!string.IsNullOrEmpty(eventId.Name))
				{
					dicProperties["EventName"] = eventId.Name;
				}
			}

			return dicProperties;
		}
	}
}