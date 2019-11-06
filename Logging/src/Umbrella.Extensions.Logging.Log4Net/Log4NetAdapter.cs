using System;
using System.Text;
using log4net;
using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.Log4Net
{
	internal sealed class Log4NetAdapter : ILogger
	{
		#region Private Members
		private readonly ILog m_Logger;
		#endregion

		#region Constructors
		public Log4NetAdapter(string repositoryName, string loggerName)
		{
			m_Logger = LogManager.GetLogger(repositoryName, loggerName);
		}
		#endregion

		#region ILogger Members
		public IDisposable BeginScope<TState>(TState state) => null;

		public bool IsEnabled(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Trace:
				case LogLevel.Debug:
					return m_Logger.IsDebugEnabled;
				case LogLevel.Information:
					return m_Logger.IsInfoEnabled;
				case LogLevel.Warning:
					return m_Logger.IsWarnEnabled;
				case LogLevel.Error:
					return m_Logger.IsErrorEnabled;
				case LogLevel.Critical:
					return m_Logger.IsFatalEnabled;
				default:
					throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
			}
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
				return;

			if (formatter == null)
				throw new InvalidOperationException();

			//If the eventId is 0 check if the Name has a value as we have hijacked this to allow for recursive calls
			//to this method to use the same id for correlating messages.
			string messageId = eventId.Id == 0
				? string.IsNullOrWhiteSpace(eventId.Name) ? "Correlation Id: " + DateTime.UtcNow.Ticks.ToString() : eventId.Name
				: eventId.Id.ToString();

			var messageBuider = new StringBuilder()
				.AppendLine(messageId)
				.Append(formatter(state, exception));

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
			if (!(exception is AggregateException aggregate))
				aggregate = exception?.InnerException as AggregateException;

			if (aggregate?.InnerExceptions?.Count > 0)
			{
				foreach (Exception innerException in aggregate.InnerExceptions)
				{
					Log(logLevel, new EventId(0, messageId), state, exception, formatter);
				}
			}
		}
		#endregion
	}
}