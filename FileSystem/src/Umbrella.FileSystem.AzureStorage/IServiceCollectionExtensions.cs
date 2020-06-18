using System;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
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
			Guard.ArgumentNotNull(services, nameof(services));
			Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

			services.AddUmbrellaFileSystemCore();

			// TODO: Are we sticking with this approach??
			services.AddSingleton<IUmbrellaAzureBlobStorageFileProvider>(x =>
			{
				var factory = x.GetService<IUmbrellaFileProviderFactory>();
				var options = x.GetService<TOptions>();

				return factory.CreateProvider<TFileProvider, TOptions>(options);
			});
			services.ReplaceSingleton<IUmbrellaFileProvider>(x => x.GetService<IUmbrellaAzureBlobStorageFileProvider>());

			// Options
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}
	}
}