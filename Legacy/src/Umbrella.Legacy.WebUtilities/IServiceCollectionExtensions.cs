using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Hosting;
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

            return services;
        }
    }
}