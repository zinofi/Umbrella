using System;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching.AzureStorage;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// 
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Used to register the Dynamic Image Azure Blob Storage Cache services with the DI container for the application.
		/// Any existing registrations for <see cref="IDynamicImageCache"/> will be replaced with <see cref="DynamicImageAzureBlobStorageCache"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> for the application.</param>
		/// <param name="optionsBuilder">The <see cref="DynamicImageAzureBlobStorageCacheOptions"/> cache options.</param>
		/// <returns>The services collection for the application.</returns>
		public static IServiceCollection AddUmbrellaDynamicImageAzureBlobStorageCache(this IServiceCollection services, Action<IServiceProvider, DynamicImageAzureBlobStorageCacheOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IDynamicImageCache, DynamicImageAzureBlobStorageCache>();
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}
	}
}