using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class ILoggerExtensions
    {
        #region Public Static Methods
        public static void WriteDebug(this ILogger log, string message, object state = null, [CallerMemberName]string methodName = null, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Debug, null, state, message, methodName, filePath, lineNumber);
        }

        public static void WriteVerbose(this ILogger log, string message, object state = null, [CallerMemberName]string methodName = null, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Verbose, null, state, message, methodName, filePath, lineNumber);
        }

        public static void WriteInformation(this ILogger log, string message, object state = null, [CallerMemberName]string methodName = null, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Information, null, state, message, methodName, filePath, lineNumber);
        }

        public static void WriteWarning(this ILogger log, string message = null, object state = null, Exception exc = null, [CallerMemberName]string methodName = null, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Warning, exc, state, message, methodName, filePath, lineNumber);
        }

        public static bool WriteError(this ILogger log, Exception exc, object state = null, string message = null, bool returnValue = false, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogErrorOrCritical(log, LogLevel.Error, exc, state, message, methodName, filePath, lineNumber);

            return returnValue;
        }

        public static bool WriteCritical(this ILogger log, Exception exc, object state = null, string message = null, bool returnValue = false, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogErrorOrCritical(log, LogLevel.Critical, exc, state, message, methodName, filePath, lineNumber);

            return returnValue;
        }
        #endregion

        #region Private Static Methods
        private static void LogDetails(ILogger log, LogLevel level, Exception exc, object state, string message, string methodName, string filePath, int lineNumber)
        {
            StringBuilder messageBuilder = new StringBuilder();

            if (state != null)
            {
                string jsonState = JsonConvert.SerializeObject(state);
                messageBuilder.Append($"{methodName}({jsonState})");
            }
            else
            {
                messageBuilder.Append($"{methodName}()");
            }

            if (level >= LogLevel.Error)
                messageBuilder.Append(" failed");

            if (!string.IsNullOrEmpty(message))
                messageBuilder.Append(" - " + message);

            messageBuilder.Append($" on Line: {lineNumber}, Path: {filePath}");

            string output = messageBuilder.ToString();

            switch (level)
            {
                case LogLevel.Debug:
                    log.LogDebug(output);
                    break;
                case LogLevel.Verbose:
                    log.LogVerbose(output);
                    break;
                case LogLevel.Information:
                    log.LogInformation(output);
                    break;
                case LogLevel.Warning:
                    log.LogWarning(output, exc);
                    break;
                case LogLevel.Error:
                    log.LogError(output, exc);
                    break;
                case LogLevel.Critical:
                    log.LogCritical(output, exc);
                    break;
            }
        }

        private static void LogErrorOrCritical(ILogger log, LogLevel level, Exception exc, object state, string message, string methodName, string filePath, int lineNumber)
        {
            LogDetails(log, level, exc, state, message, methodName, filePath, lineNumber);

            AggregateException aggregateException = exc as AggregateException;

            if (aggregateException?.InnerExceptions != null && aggregateException.InnerExceptions.Count > 0)
            {
                foreach (Exception excInner in aggregateException.InnerExceptions)
                {
                    LogDetails(log, level, excInner, state, message, methodName, filePath, lineNumber);
                }
            }
        }
        #endregion
    }
}