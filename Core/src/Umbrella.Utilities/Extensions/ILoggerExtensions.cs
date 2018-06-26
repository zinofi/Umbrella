using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Umbrella.Utilities;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// A set of extension methods for <see cref="ILogger"/> instances which provide more comprehensive logging functionality than the built-in <see cref="ILogger.Log"/>
    /// method and the Log* extension methods provided by Microsoft.
    /// </summary>
    public static class ILoggerExtensions
    {
        #region Private Static Members
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> s_TypePropertyInfoDictionary = new ConcurrentDictionary<Type, PropertyInfo[]>();
        #endregion

        #region Public Static Methods
        public static void WriteDebug(this ILogger log, object state = null, string message = null, in EventId eventId = default, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
            => LogDetails(log, LogLevel.Debug, null, state, message, in eventId, methodName, filePath, lineNumber);

        public static void WriteTrace(this ILogger log, object state = null, string message = null, in EventId eventId = default, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
            => LogDetails(log, LogLevel.Trace, null, state, message, in eventId, methodName, filePath, lineNumber);

        public static void WriteInformation(this ILogger log, object state = null, string message = null, in EventId eventId = default, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
            => LogDetails(log, LogLevel.Information, null, state, message, in eventId, methodName, filePath, lineNumber);

        public static bool WriteWarning(this ILogger log, Exception exc = null, object state = null, string message = null, in EventId eventId = default, bool returnValue = false, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Error, exc, state, message, in eventId, methodName, filePath, lineNumber);

            return returnValue;
        }

        public static bool WriteError(this ILogger log, Exception exc, object state = null, string message = null, in EventId eventId = default, bool returnValue = false, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Error, exc, state, message, in eventId, methodName, filePath, lineNumber);

            return returnValue;
        }

        public static bool WriteCritical(this ILogger log, Exception exc, object state = null, string message = null, in EventId eventId = default, bool returnValue = false, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Critical, exc, state, message, in eventId, methodName, filePath, lineNumber);

            return returnValue;
        }
        #endregion

        #region Private Static Methods
        private static void LogDetails(ILogger log, LogLevel level, Exception exc, object state, string message, in EventId eventId, string methodName, string filePath, int lineNumber)
        {
            StringBuilder messageBuilder = new StringBuilder();

            var stateDictionary = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(nameof(methodName), methodName),
                new KeyValuePair<string, string>(nameof(filePath), filePath),
                new KeyValuePair<string, string>(nameof(lineNumber), lineNumber.ToString(CultureInfo.InvariantCulture))
            };

            if (state != null)
            {
                string jsonState = UmbrellaStatics.SerializeJson(state);
                messageBuilder.Append($"{methodName}({jsonState})");

                //Here we will convert the stateDictionary to an IReadOnlyList<KeyValuePair<string, object>> (actual call to .AsReadOnly is further down) so that it is compatible with ApplicationInsights.
                //It should also be compatible with other logging implementations that can perform serialization of collections.
                PropertyInfo[] propertyInfos = s_TypePropertyInfoDictionary.GetOrAdd(state.GetType(), x => x.GetProperties());

                foreach (var pi in propertyInfos)
                {
                    stateDictionary.Add(new KeyValuePair<string, string>(pi.Name, Convert.ToString(pi.GetValue(state) ?? string.Empty, CultureInfo.InvariantCulture)));
                }
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

            //We are passing the state to logger. It is up to the logging implementation to then process it. We have already serialized it to JSON above and included it as part of the message
            //just in case the logging implementation doesn't do something with it.
            log.Log(level, eventId, stateDictionary.AsReadOnly(), exc, (stateObject, exceptionObject) => messageBuilder.ToString());
        }
        #endregion
    }
}