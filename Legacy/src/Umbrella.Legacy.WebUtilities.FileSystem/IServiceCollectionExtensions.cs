using System;
using Microsoft.Extensions.DependencyInjection;
using Umbrella.Legacy.WebUtilities.FileSystem.Middleware;
using Umbrella.Legacy.WebUtilities.FileSystem.Middleware.Options;
using Umbrella.Utilities;

namespace Umbrella.Legacy.WebUtilities.FileSystem
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaLegacyWebUtilitiesFileSystem(this IServiceCollection services, Action<IServiceProvider, UmbrellaFileProviderMiddlewareOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(optionsBuilder, nameof(optionsBuilder));

			services.AddSingleton<UmbrellaFileProviderMiddleware>();
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}
    }
}