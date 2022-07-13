using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.Log4Net
{
	/// <summary>
	/// A log4net specific implementation of the Microsoft <see cref="ILogger"/>.
	/// </summary>
	public class Log4NetLogger : ILogger
	{
		#region Private Members
		private readonly ILog m_Logger;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of <see cref="Log4NetLogger"/>.
		/// </summary>
		/// <param name="repositoryName">The repo name.</param>
		/// <param name="loggerName">The logger name.</param>
		public Log4NetLogger(string repositoryName, string loggerName)
		{
			m_Logger = LogManager.GetLogger(repositoryName, loggerName);
		}
		#endregion

		#region ILogger Members
		/// <inheritdoc />
		public IDisposable? BeginScope<TState>(TState state) => null;

		/// <inheritdoc />
		public bool IsEnabled(LogLevel logLevel) => logLevel switch
		{
			LogLevel.Trace => m_Logger.IsDebugEnabled,
			LogLevel.Debug => m_Logger.IsDebugEnabled,
			LogLevel.Information => m_Logger.IsInfoEnabled,
			LogLevel.Warning => m_Logger.IsWarnEnabled,
			LogLevel.Error => m_Logger.IsErrorEnabled,
			LogLevel.Critical => m_Logger.IsFatalEnabled,
			LogLevel.None => false,
			_ => throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel)),
		};

		/// <inheritdoc />
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			=> LogInner(logLevel, eventId, state, exception, formatter);

		private void LogInner<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, int recursiveLevel = 0)
		{
			// Checking how many levels deep we are here to prevent a possible StackOverflowException.
			if (recursiveLevel > 5)
			{
				if (IsEnabled(LogLevel.Warning))
					m_Logger.Warn("Whilst logging an exception the recursion level exceeded 5. This indicates a problem with the way the AggregateException has been constructed.");

				return;
			}

			if (!IsEnabled(logLevel))
				return;

			if (formatter == null)
				throw new InvalidOperationException();

			// If the eventId is 0 check if the Name has a value as we have hijacked this to allow for recursive calls
			// to this method to use the same id for correlating messages.
			string messageId = eventId.Id is 0
				? string.IsNullOrWhiteSpace(eventId.Name) ? "Correlation Id: " + DateTime.UtcNow.Ticks.ToString() : eventId.Name
				: eventId.Id.ToString();

			var messageBuider = new StringBuilder()
				.AppendLine(messageId)
				.AppendLine($"{DateTime.UtcNow} UTC")
				.Append(formatter(state, exception));

			if (state is IEnumerable<KeyValuePair<string, string>> stateDictionary)
			{
				var entries = stateDictionary.ToArray();

				messageBuider.AppendLine();
				messageBuider.AppendLine();
				messageBuider.AppendLine("--------------- Begin State ---------------");

				if (entries.Length > 0)
				{
					foreach (var entry in stateDictionary)
					{
						messageBuider.AppendLine($"{entry.Key}: {entry.Value}");
					}
				}

				messageBuider.AppendLine("--------------- End State ---------------");
			}

			string message = messageBuider.ToString();

			switch (logLevel)
			{
				case LogLevel.Trace:
				case LogLevel.Debug:
					m_Logger.Debug(message, exception);
					break;
				case LogLevel.Information:
					m_Logger.Info(message, exception);
					break;
				case LogLevel.Warning:
					m_Logger.Warn(message, exception);
					break;
				case LogLevel.Error:
					m_Logger.Error(message, exception);
					break;
				case LogLevel.Critical:
					m_Logger.Fatal(message, exception);
					break;
				default:
					m_Logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
					m_Logger.Info(message, exception);
					break;
			}

			// Log4Net doesn't seem to log AggregateExceptions properly so handling them manually here
			if (exception != null)
			{
				AggregateException? aggregate = null;

				if (!(exception is AggregateException))
					aggregate = exception.InnerException as AggregateException;

				if (aggregate?.InnerExceptions?.Count > 0)
				{
					foreach (Exception innerException in aggregate.InnerExceptions)
					{
						LogInner(logLevel, new EventId(0, messageId), state, exception, formatter, ++recursiveLevel);

						// The call returned here so we can reduce by 1 to indicate we have moved back up.
						recursiveLevel--;
					}
				}
			}
		}
		#endregion
	}
}