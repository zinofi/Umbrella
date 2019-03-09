using System;
using Umbrella.Legacy.WebUtilities.DynamicImage.Middleware;
using Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddUmbrellaLegacyWebUtilitiesDynamicImage(this IServiceCollection services, Action<IServiceProvider, DynamicImageMiddlewareOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

			services.AddSingleton<DynamicImageMiddleware>();
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}
	}
}