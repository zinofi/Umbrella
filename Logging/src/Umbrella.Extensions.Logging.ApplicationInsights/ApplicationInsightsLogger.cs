using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.ApplicationInsights
{
    internal class ApplicationInsightsLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly TelemetryClient _telemetryClient;
        private readonly LogLevel _minLevel;
        private readonly bool _trackExceptionsAsExceptionTelemetry;
        private readonly bool _includeEventId;

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationInsightsLogger"/>
        /// </summary>
        public ApplicationInsightsLogger(string name, TelemetryClient telemetryClient, LogLevel minLevel, bool trackExceptionsAsExceptionTelemetry, bool includeEventId)
        {
            _categoryName = name;
            _telemetryClient = telemetryClient;
            _minLevel = minLevel;
            _trackExceptionsAsExceptionTelemetry = trackExceptionsAsExceptionTelemetry;
            _includeEventId = includeEventId;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return _telemetryClient != null && logLevel >= _minLevel && _telemetryClient.IsEnabled();
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (this.IsEnabled(logLevel))
            {
                var stateDictionary = state as IReadOnlyList<KeyValuePair<string, object>>;
                if (exception == null || _trackExceptionsAsExceptionTelemetry == false)
                {
                    var traceTelemetry = new TraceTelemetry(formatter(state, exception), GetSeverityLevel(logLevel));
                    PopulateTelemetry(traceTelemetry, stateDictionary, eventId);
                    _telemetryClient.TrackTrace(traceTelemetry);
                }
                else
                {
                    var exceptionTelemetry = new ExceptionTelemetry(exception)
                    {
                        Message = formatter(state, exception),
                        SeverityLevel = GetSeverityLevel(logLevel)
                    };
                    exceptionTelemetry.Properties["Exception"] = exception.ToString();
                    PopulateTelemetry(exceptionTelemetry, stateDictionary, eventId);
                    _telemetryClient.TrackException(exceptionTelemetry);
                }
            }
        }

        private void PopulateTelemetry(ITelemetry telemetry, IReadOnlyList<KeyValuePair<string, object>> stateDictionary, in EventId eventId)
        {
            if (telemetry is ISupportProperties telemetryWithProperties)
            {
                IDictionary<string, string> dict = telemetryWithProperties.Properties;
                dict["CategoryName"] = _categoryName;

                if (_includeEventId)
                {
                    if (eventId.Id != 0)
                    {
                        dict["EventId"] = eventId.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(eventId.Name))
                    {
                        dict["EventName"] = eventId.Name;
                    }
                }

                if (stateDictionary != null)
                {
                    foreach (KeyValuePair<string, object> item in stateDictionary)
                    {
                        dict[item.Key] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        private SeverityLevel GetSeverityLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return SeverityLevel.Critical;
                case LogLevel.Error:
                    return SeverityLevel.Error;
                case LogLevel.Warning:
                    return SeverityLevel.Warning;
                case LogLevel.Information:
                    return SeverityLevel.Information;
                case LogLevel.Debug:
                case LogLevel.Trace:
                default:
                    return SeverityLevel.Verbose;
            }
        }
    }
}