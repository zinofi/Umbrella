using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Unity.Logging.Models;
using Microsoft.Extensions.DependencyInjection;
using Umbrella.Unity.Logging.Services;

namespace Umbrella.Unity.Logging
{
    public class UnityRemoteClientLogger : ILogger
    {
        private readonly Func<IServiceProvider> m_ServiceProviderAccessor;
        private readonly string m_CategoryName;
        private IRemoteClientLogService m_LogService;

        private IRemoteClientLogService LogService
        {
            get
            {
                if (m_LogService == null)
                    m_LogService = m_ServiceProviderAccessor().GetService<IRemoteClientLogService>();

                return m_LogService;
            }
        }

        public static LogLevel LogLevel { get; set; } = LogLevel.None;

        public UnityRemoteClientLogger(Func<IServiceProvider> serviceProviderAccessor, string categoryName)
        {
            m_ServiceProviderAccessor = serviceProviderAccessor;
            m_CategoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            => null;

        public bool IsEnabled(LogLevel logLevel)
            => logLevel >= LogLevel.Warning && logLevel >= LogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter != null)
            {
                string message = formatter(state, exception);

                StringBuilder sbMessage = new StringBuilder(message);

                if (!string.IsNullOrWhiteSpace(exception?.Message))
                {
                    sbMessage
                        .AppendLine()
                        .AppendLine($"{exception.GetType().FullName}: {exception.Message}");
                }

                var innerException = exception?.InnerException;

                while (innerException != null)
                {
                    if (!string.IsNullOrWhiteSpace(innerException.Message))
                    {
                        sbMessage
                            .AppendLine()
                            .AppendLine($"{innerException.GetType().FullName}: {innerException.Message}");
                    }

                    innerException = innerException.InnerException;
                }

                RemoteClientLogModel model = new RemoteClientLogModel
                {
                    Id = Guid.NewGuid(),
                    Level = (RemoteClientLogLevel)logLevel, //1:1 mapping between log level values so this is ok
                    Message = sbMessage.ToString(),
                    StackTrace = exception?.StackTrace,
                    Source = "Garden Visualiser - WebGL"
                };

                var task = LogService.PostAsync(model);
                task.GetAwaiter().OnCompleted(() =>
                {
                    //In the event that the log message couldn't be uploaded to the server, write this out to the console instead
                    if (task.IsFaulted)
                        LogLoggerException(task.Exception);
                });
            }
        }

        private void LogLoggerException(Exception exception)
        {
            StringBuilder sbMessage = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(exception?.Message))
            {
                sbMessage
                    .AppendLine()
                    .AppendLine($"{exception.GetType().FullName}: {exception.Message}");
            }

            LogStackTrace(exception, sbMessage);

            var innerException = exception?.InnerException;

            while (innerException != null)
            {
                if (!string.IsNullOrWhiteSpace(innerException.Message))
                {
                    sbMessage
                        .AppendLine()
                        .AppendLine($"{innerException.GetType().FullName}: {innerException.Message}");
                }

                LogStackTrace(innerException, sbMessage);

                innerException = innerException.InnerException;
            }

            string messageToLog = sbMessage.ToString();

            UnityEngine.Debug.LogError(messageToLog);
        }

        private static void LogStackTrace(Exception exception, StringBuilder sbMessage)
        {
            if (!string.IsNullOrWhiteSpace(exception?.StackTrace))
            {
                sbMessage
                    .AppendLine()
                    .AppendLine($"********** Stack Trace: {exception.GetType().FullName} **********")
                    .AppendLine()
                    .AppendLine(exception?.StackTrace)
                    .AppendLine()
                    .AppendLine("******************************************************************");
            }
        }
    }
}