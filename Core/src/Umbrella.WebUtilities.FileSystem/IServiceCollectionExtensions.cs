// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.WebUtilities.FileSystem.Middleware.Options;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.WebUtilities.FileSystem"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.WebUtilities.FileSystem"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <param name="optionsBuilder">The optional <see cref="FileSystemMiddlewareOptions"/> builder.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	public static IServiceCollection AddUmbrellaWebUtilitiesFileSystem(
		this IServiceCollection services,
		Action<IServiceProvider, FileSystemMiddlewareOptions>? optionsBuilder = null)
	{
		Guard.IsNotNull(services);

		// Options
		_ = services.ConfigureUmbrellaOptions(optionsBuilder);

		return services;
	}
}