using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaDiskFileProvider(this IServiceCollection services, UmbrellaDiskFileProviderOptions options)
            => AddUmbrellaDiskFileProvider<UmbrellaDiskFileProvider>(services, options);

        public static IServiceCollection AddUmbrellaDiskFileProvider<TFileProvider>(this IServiceCollection services, UmbrellaDiskFileProviderOptions options)
            where TFileProvider : class, IUmbrellaDiskFileProvider
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(options, nameof(options));

            services.AddSingleton(options);
            services.AddSingleton<IUmbrellaDiskFileProvider, TFileProvider>();
            services.AddSingleton<IUmbrellaFileProvider>(x => x.GetService<IUmbrellaDiskFileProvider>());

            return services;
        }
    }
}