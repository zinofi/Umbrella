using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Unity.Logging.Services;
using Umbrella.Unity.Networking.Abstractions;
using Umbrella.Unity.Networking.WebRequest;
using Umbrella.Unity.Utilities;
using Umbrella.Unity.Utilities.Async;
using Umbrella.Unity.Utilities.Configuration;
using Umbrella.Unity.Utilities.Validation;
using Umbrella.Utilities;
using Umbrella.Utilities.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUnity(this IServiceCollection services, Action<RemoteClientLogServiceOptions> logServiceOptionsBuilder)
        {
            Guard.ArgumentNotNull(services, nameof(services));
            Guard.ArgumentNotNull(logServiceOptionsBuilder, nameof(logServiceOptionsBuilder));

            services.AddSingleton<IGameObjectUtility, GameObjectUtility>();
            services.AddSingleton<IAppSettingsSource, PlayerPrefsSettingsSource>();
            services.AddSingleton<IUnityNetworkManager, UnityNetworkManager>();
            services.AddSingleton<ITaskCompletionSourceProcessor>(x => TaskCompletionSourceProcessor.Instance);
            services.AddSingleton<IUnityValidationUtility, UnityValidationUtility>();
            services.AddSingleton<IRemoteClientLogService, RemoteClientLogService>();

            var logServiceOptions = new RemoteClientLogServiceOptions();
            logServiceOptionsBuilder(logServiceOptions);

            services.AddSingleton(logServiceOptions);

            return services;
        }
    }
}
