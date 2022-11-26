// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Umbrella.Extensions.Logging.Azure.Management;
using Umbrella.Extensions.Logging.Azure.Management.Options;

[assembly: InternalsVisibleTo("Umbrella.Extensions.Logging.Azure.Test")]

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.Extensions.Logging.Azure"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.Extensions.Logging.Azure"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <param name="optionsBuilder">The <see cref="AzureTableStorageLogManagementOptions"/> builder.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
	public static IServiceCollection AddUmbrellaLoggingAzureStorage(this IServiceCollection services, Action<IServiceProvider, AzureTableStorageLogManagementOptions> optionsBuilder)
	{
		Guard.IsNotNull(services);
		Guard.IsNotNull(optionsBuilder);

		_ = services.AddSingleton<IAzureTableStorageLogManager, AzureTableStorageLogManager>();
		_ = services.ConfigureUmbrellaOptions(optionsBuilder);

		return services;
	}
}