using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Csv;
using Umbrella.Utilities.Csv.Abstractions;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.Utilities.Csv"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.Utilities.Csv"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	public static IServiceCollection AddUmbrellaCsv(this IServiceCollection services)
	{
		Guard.IsNotNull(services);

		_ = services.AddSingleton<ICsvService, CsvService>();

		return services;
	}
}