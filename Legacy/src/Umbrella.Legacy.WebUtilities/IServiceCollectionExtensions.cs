using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Accessors;
using Umbrella.Legacy.WebUtilities.Accessors.Abstractions;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Legacy.WebUtilities.Middleware;
using Umbrella.Legacy.WebUtilities.Middleware.Options;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options;
using Umbrella.Utilities.Hosting;
using Umbrella.WebUtilities.Hosting;

[assembly: InternalsVisibleTo("Umbrella.Legacy.WebUtilities.Test")]
[assembly: InternalsVisibleTo("Umbrella.Legacy.WebUtilities.Benchmark")]

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaLegacyWebUtilities(this IServiceCollection services)
            => services.AddUmbrellaLegacyWebUtilities<UmbrellaWebHostingEnvironment>();

        public static IServiceCollection AddUmbrellaLegacyWebUtilities<TUmbrellaWebHostingEnvironment>(this IServiceCollection services)
            where TUmbrellaWebHostingEnvironment : class, IUmbrellaWebHostingEnvironment
        {
            // Add the hosting environment as a singleton and then ensure the same instance is bound to both interfaces
            services.AddSingleton<TUmbrellaWebHostingEnvironment>();
            services.AddSingleton<IUmbrellaHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());
            services.AddSingleton<IUmbrellaWebHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBundleUtility, BundleUtility>();

            services.AddSingleton<CleanupIDisposableMiddleware>();
            services.AddSingleton<FrontEndCompressionMiddleware>();
            services.AddSingleton<HttpContextAccessorMiddleware>();
            services.AddSingleton<RobotsMiddleware>();

            // Default Options - These can be replaced by calls to the Configure* methods below.
            services.AddSingleton(new FrontEndCompressionMiddlewareOptions());
            services.AddSingleton(new BundleUtilityOptions());

            return services;
        }

        public static IServiceCollection ConfigureFrontEndCompressionMiddlewareOptions(this IServiceCollection services, Action<IServiceProvider, FrontEndCompressionMiddlewareOptions> optionsBuilder)
            => services.ConfigureUmbrellaOptions(optionsBuilder);

        public static IServiceCollection ConfigureBundleUtilityOptions(this IServiceCollection services, Action<IServiceProvider, BundleUtilityOptions> optionsBuilder)
            => services.ConfigureUmbrellaOptions(optionsBuilder);
    }
}