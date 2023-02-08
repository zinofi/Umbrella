using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.Utilities.Options.Abstractions;

/// <summary>
/// A service used to intialize options types.
/// </summary>
public interface IOptionsInitializer
{
	/// <summary>
	/// Initializes the specified options.
	/// </summary>
	/// <typeparam name="T">The type of the options.</typeparam>
	/// <param name="options">The options.</param>
	/// <param name="services">The services.</param>
	/// <param name="isDevelopmentMode">Specifies if the application is running in development mode.</param>
	void Initialize<T>(T options, IServiceCollection services, bool isDevelopmentMode = false) where T : class;
}