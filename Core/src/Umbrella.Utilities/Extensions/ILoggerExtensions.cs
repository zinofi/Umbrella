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
		/// <summary>
		/// Writes a <see cref="LogLevel.Debug"/> message to the specified <paramref name="log"/>.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <param name="state">The state.</param>
		/// <param name="message">The message.</param>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="lineNumber">The line number.</param>
		public static void WriteDebug(this ILogger log, object? state = null, string? message = null, in EventId eventId = default, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
            => LogDetails(log, LogLevel.Debug, null, state, message, in eventId, methodName, filePath, lineNumber);

		/// <summary>
		/// Writes a <see cref="LogLevel.Debug"/> message to the specified <paramref name="log"/>.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <param name="state">The state.</param>
		/// <param name="message">The message.</param>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="lineNumber">The line number.</param>
		public static void WriteTrace(this ILogger log, object? state = null, string? message = null, in EventId eventId = default, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
            => LogDetails(log, LogLevel.Trace, null, state, message, in eventId, methodName, filePath, lineNumber);

		/// <summary>
		/// Writes a <see cref="LogLevel.Information"/> message to the specified <paramref name="log"/>.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <param name="state">The state.</param>
		/// <param name="message">The message.</param>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="lineNumber">The line number.</param>
		public static void WriteInformation(this ILogger log, object? state = null, string? message = null, in EventId eventId = default, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
            => LogDetails(log, LogLevel.Information, null, state, message, in eventId, methodName, filePath, lineNumber);

		/// <summary>
		/// Writes a <see cref="LogLevel.Warning"/> message to the specified <paramref name="log"/>.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <param name="exc">The exc.</param>
		/// <param name="state">The state.</param>
		/// <param name="message">The message.</param>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="returnValue">
		/// The return value of this method will be this value.
		/// This is primarily to allowed these log methods to be called as a side-effect of exception filters.
		/// This return value can then control whether or not the associated catch block is entered.
		/// </param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="lineNumber">The line number.</param>
		/// <returns>The specified <paramref name="returnValue"/>.</returns>
		public static bool WriteWarning(this ILogger log, Exception? exc = null, object? state = null, string? message = null, in EventId eventId = default, bool returnValue = true, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Warning, exc, state, message, in eventId, methodName, filePath, lineNumber);

            return returnValue;
        }

		/// <summary>
		/// Writes a <see cref="LogLevel.Error"/> message to the specified <paramref name="log"/>.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <param name="exc">The exc.</param>
		/// <param name="state">The state.</param>
		/// <param name="message">The message.</param>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="returnValue">
		/// The return value of this method will be this value.
		/// This is primarily to allowed these log methods to be called as a side-effect of exception filters.
		/// This return value can then control whether or not the associated catch block is entered.
		/// </param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="lineNumber">The line number.</param>
		/// <returns>The specified <paramref name="returnValue"/>.</returns>
		public static bool WriteError(this ILogger log, Exception? exc, object? state = null, string? message = null, in EventId eventId = default, bool returnValue = true, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Error, exc, state, message, in eventId, methodName, filePath, lineNumber);

            return returnValue;
        }

		/// <summary>
		/// Writes a <see cref="LogLevel.Critical"/> message to the specified <paramref name="log"/>.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <param name="exc">The exc.</param>
		/// <param name="state">The state.</param>
		/// <param name="message">The message.</param>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="returnValue">
		/// The return value of this method will be this value.
		/// This is primarily to allowed these log methods to be called as a side-effect of exception filters.
		/// This return value can then control whether or not the associated catch block is entered.
		/// </param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="lineNumber">The line number.</param>
		/// <returns>The specified <paramref name="returnValue"/>.</returns>
		public static bool WriteCritical(this ILogger log, Exception exc, object? state = null, string? message = null, in EventId eventId = default, bool returnValue = true, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            LogDetails(log, LogLevel.Critical, exc, state, message, in eventId, methodName, filePath, lineNumber);

            return returnValue;
        }
        #endregion

        #region Private Static Methods
        private static void LogDetails(ILogger log, LogLevel level, Exception? exc, object? state, string? message, in EventId eventId, string methodName, string filePath, int lineNumber)
        {
            if (!log.IsEnabled(level))
                return;

            var messageBuilder = new StringBuilder();

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