// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.DataAccess.Abstractions;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.DataAccess.Abstractions"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.DataAccess.Abstractions"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	public static IServiceCollection AddUmbrellaDataAccess(this IServiceCollection services)
	{
		Guard.IsNotNull(services);

		_ = services.AddSingleton<IEntityValidator, EntityValidator>();
		_ = services.AddSingleton<IEntityMappingUtility, EntityMappingUtility>();
		_ = services.AddScoped<IUmbrellaDbContextHelper, UmbrellaDbContextHelper>();
		_ = services.AddScoped(typeof(DbAppTenantSessionContext<>));

		_ = services.AddScoped<IUmbrellaRepositoryCoreDataService, UmbrellaRepositoryCoreDataService>();

		return services;
	}
}