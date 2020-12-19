using System;
using Umbrella.DynamicImage.Abstractions.Caching;
using Umbrella.DynamicImage.Caching.AzureStorage;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.DynamicImage.Caching.AzureStorage"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.DynamicImage.Caching.AzureStorage"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder
		/// with Azure Blob Storage caching.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="dynamicImageCacheCoreOptionsBuilder">The optional <see cref="DynamicImageCacheCoreOptions"/> builder.</param>
		/// <param name="dynamicImageAzureBlobStorageCacheOptionsBuilder">The optional <see cref="DynamicImageAzureBlobStorageCacheOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDynamicImageAzureBlobStorageCache(
			this IServiceCollection services,
			Action<IServiceProvider, DynamicImageCacheCoreOptions>? dynamicImageCacheCoreOptionsBuilder = null,
			Action<IServiceProvider, DynamicImageAzureBlobStorageCacheOptions>? dynamicImageAzureBlobStorageCacheOptionsBuilder = null)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddUmbrellaDynamicImage();
			services.ReplaceSingleton<IDynamicImageCache, DynamicImageAzureBlobStorageCache>();
			services.ConfigureUmbrellaOptions(dynamicImageCacheCoreOptionsBuilder);
			services.ConfigureUmbrellaOptions(dynamicImageAzureBlobStorageCacheOptionsBuilder);

			return services;
		}
	}
}