using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.ApplicationInsights
{
    /// <summary>
    /// Extension methods for <see cref="ILoggerFactory"/> that allow adding Application Insights logger.
    /// </summary>
    public static class ApplicationInsightsLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds the application insights logging provider.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="minLevel">The minimum level.</param>
        /// <param name="trackExceptionsAsExceptionTelemetry">if set to <c>true</c> [track exceptions as exception telemetry].</param>
        /// <param name="includeEventId">if set to <c>true</c> [include event identifier].</param>
        /// <returns>The current <see cref="ILoggerFactory"/> instance.</returns>
        public static ILoggerFactory AddApplicationInsights(
            this ILoggerFactory factory,
            TelemetryClient telemetryClient,
            LogLevel minLevel = LogLevel.Information,
            bool trackExceptionsAsExceptionTelemetry = true,
            bool includeEventId = false)
        {
            factory.AddProvider(new ApplicationInsightsLoggerProvider(telemetryClient, minLevel, trackExceptionsAsExceptionTelemetry, includeEventId));

            return factory;
        }
    }
}