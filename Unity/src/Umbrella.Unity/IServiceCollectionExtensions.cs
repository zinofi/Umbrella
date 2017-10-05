using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Unity.Networking.Abstractions;
using Umbrella.Unity.Networking.WebRequest;
using Umbrella.Unity.Utilities;
using Umbrella.Unity.Utilities.Async;
using Umbrella.Unity.Utilities.Configuration;
using Umbrella.Utilities.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUnity(this IServiceCollection services)
        {
            services.AddSingleton<IGameObjectUtility, GameObjectUtility>();
            services.AddSingleton<IAppSettingsSource, PlayerPrefsSettingsSource>();
            services.AddSingleton<IUnityNetworkManager, UnityNetworkManager>();
            services.AddSingleton<ITaskCompletionSourceProcessor>(x => TaskCompletionSourceProcessor.Instance);

            return services;
        }
    }
}