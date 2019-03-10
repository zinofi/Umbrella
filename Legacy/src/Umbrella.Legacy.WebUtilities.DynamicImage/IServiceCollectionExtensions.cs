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

			services.AddSingleton(ctx =>
			{
				var options = new DynamicImageMiddlewareOptions();
				optionsBuilder(ctx, options);

				// TODO: V3 Need to alter the constructor of the Middleware to accept just the options instance instead of an action.
				// This is a breaking change though so wait until the v3 release.
				return new Action<DynamicImageMiddlewareOptions>(opts =>
				{
					opts.CacheControlHeaderValue = options.CacheControlHeaderValue;
					opts.DynamicImagePathPrefix = options.DynamicImagePathPrefix;
					opts.SourceFileProvider = options.SourceFileProvider;
				});
			});

			return services;
		}
	}
}