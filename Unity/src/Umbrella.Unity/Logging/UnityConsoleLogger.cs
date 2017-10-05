using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Umbrella.Unity.Logging
{
    //TODO: Need to create a UnityWebServerLogger as well to send messages up to the server.
    //Create a UnityConsoleLogger by renaming this and also another logger to write to a web service.
    //Add a constructor to the providers to allow a config object to be passed in containing a property for the log level so that each provider can be
    //configured independently. Additionally, only allow the console logger to be changed at runtime by calling a JS method which calls into Unity and updates the log level.
    public class UnityConsoleLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string m_CategoryName;

        public static LogLevel LogLevel { get; set; } = LogLevel.None;

        public UnityConsoleLogger(string categoryName)
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
                    sbMessage.AppendLine($"\n{exception.Message}");

                var innerException = exception?.InnerException;

                while (innerException != null)
                {
                    if (!string.IsNullOrWhiteSpace(innerException.Message))
                        sbMessage.AppendLine($"\n{innerException.Message}");

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

                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        UnityEngine.Debug.Log(messageToLog);
                        break;
                    case LogLevel.Warning:
                        UnityEngine.Debug.LogWarning(messageToLog);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        UnityEngine.Debug.LogError(messageToLog);
                        break;
                }
            }
        }
    }
}