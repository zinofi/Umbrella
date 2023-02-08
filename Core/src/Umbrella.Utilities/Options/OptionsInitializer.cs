using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Utilities.Options;

/// <summary>
/// A service used to intialize options types.
/// </summary>
/// <seealso cref="IOptionsInitializer" />
public sealed class OptionsInitializer : IOptionsInitializer
{
	private readonly ILogger _logger;
	private readonly IServiceProvider _serviceProvider;

	/// <summary>
	/// Initializes a new instance of the <see cref="OptionsInitializer"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="serviceProvider">The service provider.</param>
	public OptionsInitializer(
		ILogger<OptionsInitializer> logger,
		IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	/// <inheritdoc/>
	public void Initialize<T>(T options, IServiceCollection services, bool isDevelopmentMode = false)
		where T : class
	{
		try
		{
			if (options is IDevelopmentModeUmbrellaOptions developmentModeOptions)
				developmentModeOptions.SetDevelopmentMode(isDevelopmentMode);

			if (options is IServicesResolverUmbrellaOptions servicesResolverOptions)
				servicesResolverOptions.Initialize(services, _serviceProvider);

			if (options is ISanitizableUmbrellaOptions sanitizableOptions)
				sanitizableOptions.Sanitize();

			if (options is IValidatableUmbrellaOptions validatableOptions)
				validatableOptions.Validate();
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { isDevelopmentMode }))
		{
			throw new UmbrellaException("There has been a problem initializing the options.", exc);
		}
	}
}