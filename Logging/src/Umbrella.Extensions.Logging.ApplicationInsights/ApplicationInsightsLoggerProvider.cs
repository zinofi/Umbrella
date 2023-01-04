using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.ApplicationInsights;

/// <summary>
/// <see cref="ILoggerProvider"/> implementation that creates instances of <see cref="ApplicationInsightsLogger"/>
/// </summary>
[ProviderAlias("ApplicationInsights")]
public class ApplicationInsightsLoggerProvider : ILoggerProvider
{
	private readonly TelemetryClient _telemetryClient;
	private readonly LogLevel _minLevel;
	private readonly bool _trackExceptionsAsExceptionTelemetry;
	private readonly bool _includeEventId;

	/// <summary>
	/// Initializes a new instance of the <see cref="ApplicationInsightsLoggerProvider"/> class.
	/// </summary>
	public ApplicationInsightsLoggerProvider(TelemetryClient telemetryClient, LogLevel minLevel, bool trackExceptionsAsExceptionTelemetry = true, bool includeEventId = false)
	{
		_telemetryClient = telemetryClient;
		_minLevel = minLevel;
		_trackExceptionsAsExceptionTelemetry = trackExceptionsAsExceptionTelemetry;
		_includeEventId = includeEventId;
	}

	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName) => new ApplicationInsightsLogger(categoryName, _telemetryClient, _minLevel, _trackExceptionsAsExceptionTelemetry, _includeEventId);

	/// <inheritdoc />
	public void Dispose() => GC.SuppressFinalize(this);
}