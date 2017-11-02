using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Umbrella.Unity.Logging
{
    public class UnityWebBrowserConsoleLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string m_CategoryName;

        public static LogLevel LogLevel { get; set; } = LogLevel.None;

        public UnityWebBrowserConsoleLogger(string categoryName)
        {
            m_CategoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            => null;

        public bool IsEnabled(LogLevel logLevel)
            => logLevel >= LogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter != null)
            {
                string message = formatter(state, exception);

                StringBuilder sbMessage = new StringBuilder(message);

                if (!string.IsNullOrWhiteSpace(exception?.Message))
                    sbMessage.AppendLine($"{exception.GetType().FullName}: {exception.Message}");

                var innerException = exception?.InnerException;

                while (innerException != null)
                {
                    if (!string.IsNullOrWhiteSpace(innerException.Message))
                        sbMessage.AppendLine($"{innerException.GetType().FullName}: {innerException.Message}");

                    innerException = innerException.InnerException;
                }

                if (!string.IsNullOrWhiteSpace(exception?.StackTrace))
                {
                    sbMessage
                        .AppendLine()
                        .AppendLine("********** Stack Trace **********")
                        .AppendLine()
                        .AppendLine(exception?.StackTrace)
                        .AppendLine()
                        .AppendLine("*********************************");
                }

                string messageToLog = sbMessage.ToString();

                string methodName = null;

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        methodName = "trace";
                        break;
                    case LogLevel.Debug:
                        methodName = "debug";
                        break;
                    default:
                    case LogLevel.Information:
                        methodName = "info";
                        break;
                    case LogLevel.Warning:
                        methodName = "warn";
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        methodName = "error";
                        break;
                }

                Application.ExternalCall($"console.{methodName}", messageToLog);
            }
        }
    }
}