using System;
using Umbrella.DynamicImage;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Caching;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddUmbrellaDynamicImage(this IServiceCollection services, Action<IServiceProvider, DynamicImageCacheOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IDynamicImageUtility, DynamicImageUtility>();
			services.AddSingleton<IDynamicImageCache, DynamicImageNoCache>();
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}

		public static IServiceCollection AddUmbrellaDynamicImageMemoryCache(this IServiceCollection services, Action<IServiceProvider, DynamicImageMemoryCacheOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IDynamicImageCache, DynamicImageMemoryCache>();
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}

		public static IServiceCollection AddUmbrellaDynamicImageDiskCache(this IServiceCollection services, Action<IServiceProvider, DynamicImageDiskCacheOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IDynamicImageCache, DynamicImageDiskCache>();
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}
	}
}