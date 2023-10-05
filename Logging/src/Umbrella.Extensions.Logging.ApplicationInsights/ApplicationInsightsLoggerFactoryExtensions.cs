using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.ApplicationInsights;

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
	[Obsolete("Use the overload that accepts ILoggingBuilder instead.")]
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

	/// <summary>
	/// Adds the application insights logging provider.
	/// </summary>
	/// <param name="loggingBuilder">The logging builder.</param>
	/// <param name="telemetryClient">The telemetry client.</param>
	/// <param name="minLevel">The minimum level.</param>
	/// <param name="trackExceptionsAsExceptionTelemetry">if set to <c>true</c> [track exceptions as exception telemetry].</param>
	/// <param name="includeEventId">if set to <c>true</c> [include event identifier].</param>
	/// <returns>The current <see cref="ILoggingBuilder"/> instance.</returns>
	public static ILoggingBuilder AddApplicationInsights(
		this ILoggingBuilder loggingBuilder,
		TelemetryClient telemetryClient,
		LogLevel minLevel = LogLevel.Information,
		bool trackExceptionsAsExceptionTelemetry = true,
		bool includeEventId = false)
	{
		_ = loggingBuilder.AddProvider(new ApplicationInsightsLoggerProvider(telemetryClient, minLevel, trackExceptionsAsExceptionTelemetry, includeEventId));

		return loggingBuilder;
	}
}