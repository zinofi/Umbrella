// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.FileSystem.AzureStorage"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.FileSystem.AzureStorage"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <param name="optionsBuilder">The <see cref="UmbrellaAzureBlobStorageFileProviderOptions"/> builder.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
	public static IServiceCollection AddUmbrellaAzureBlobStorageFileProvider(this IServiceCollection services, Action<IServiceProvider, UmbrellaAzureBlobStorageFileProviderOptions> optionsBuilder)
		=> AddUmbrellaAzureBlobStorageFileProvider<UmbrellaAzureBlobStorageFileProvider>(services, optionsBuilder);

	/// <summary>
	/// Adds the <see cref="Umbrella.FileSystem.AzureStorage"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <typeparam name="TFileProvider">
	/// The concrete implementation of <see cref="IUmbrellaAzureBlobStorageFileProvider"/> to register. This allows consuming applications to override the default implementation and allow it to be
	/// resolved from the container correctly for both the <see cref="IUmbrellaFileProvider"/> and <see cref="IUmbrellaAzureBlobStorageFileProvider"/> interfaces.
	/// </typeparam>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <param name="optionsBuilder">The <see cref="UmbrellaAzureBlobStorageFileProviderOptions"/> builder.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
	public static IServiceCollection AddUmbrellaAzureBlobStorageFileProvider<TFileProvider>(this IServiceCollection services, Action<IServiceProvider, UmbrellaAzureBlobStorageFileProviderOptions> optionsBuilder)
		where TFileProvider : class, IUmbrellaAzureBlobStorageFileProvider
		=> AddUmbrellaAzureBlobStorageFileProvider<TFileProvider, UmbrellaAzureBlobStorageFileProviderOptions>(services, optionsBuilder);

	/// <summary>
	/// Adds the <see cref="Umbrella.FileSystem.AzureStorage"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <typeparam name="TFileProvider">
	/// The concrete implementation of <see cref="IUmbrellaAzureBlobStorageFileProvider"/> to register. This allows consuming applications to override the default implementation and allow it to be
	/// resolved from the container correctly for both the <see cref="IUmbrellaFileProvider"/> and <see cref="IUmbrellaAzureBlobStorageFileProvider"/> interfaces.
	/// </typeparam>
	/// <typeparam name="TOptions">The type of the options.</typeparam>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <param name="optionsBuilder">The <typeparamref name="TOptions"/> builder.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
	public static IServiceCollection AddUmbrellaAzureBlobStorageFileProvider<TFileProvider, TOptions>(this IServiceCollection services, Action<IServiceProvider, TOptions> optionsBuilder)
		where TFileProvider : class, IUmbrellaAzureBlobStorageFileProvider
		where TOptions : UmbrellaAzureBlobStorageFileProviderOptions, new()
	{
		Guard.IsNotNull(services);
		Guard.IsNotNull(optionsBuilder);

		services.AddUmbrellaFileSystemCore();

		_ = services.AddSingleton<IUmbrellaAzureBlobStorageFileProvider>(x =>
		{
			var factory = x.GetRequiredService<IUmbrellaFileProviderFactory>();
			var options = x.GetRequiredService<TOptions>();

			return factory.CreateProvider<TFileProvider, TOptions>(options);
		});
		_ = services.ReplaceSingleton<IUmbrellaFileProvider>(x => x.GetRequiredService<IUmbrellaAzureBlobStorageFileProvider>());

		// Options
		_ = services.ConfigureUmbrellaOptions(optionsBuilder);

		return services;
	}
}