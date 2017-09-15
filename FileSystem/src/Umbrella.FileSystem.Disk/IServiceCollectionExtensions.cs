using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDiskFileProvider(this IServiceCollection services, UmbrellaDiskFileProviderOptions options)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(options, nameof(options));

            services.AddSingleton(options);
            services.AddSingleton<IUmbrellaFileProvider, UmbrellaDiskFileProvider>();
            services.AddSingleton<IUmbrellaDiskFileProvider, UmbrellaDiskFileProvider>();
            
            return services;
        }
    }
}