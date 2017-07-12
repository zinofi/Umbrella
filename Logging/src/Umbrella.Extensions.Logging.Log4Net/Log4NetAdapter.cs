using log4net;
using Microsoft.Extensions.Logging;
using System;

namespace Umbrella.Extensions.Logging.Log4Net
{
    public class Log4NetAdapter : ILogger
    {
        #region Private Members
        private readonly ILog m_Logger;
        #endregion

        #region Constructors
        public Log4NetAdapter(string loggerName)
            => m_Logger = LogManager.GetLogger(loggerName);
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

            string message = null;

            if (formatter != null)
                message = formatter(state, exception);
            else
                throw new InvalidOperationException();

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
        } 
        #endregion
    }
}