using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Unity.Logging;
using Umbrella.Utilities;

namespace Microsoft.Extensions.Logging
{
    public static class ILoggerFactoryExtensions
    {
        public static ILoggerFactory AddUmbrellaUnityConsoleLogger(this ILoggerFactory loggerFactory, LogLevel logLevel)
        {
            UnityConsoleLogger.LogLevel = logLevel;
            loggerFactory.AddProvider(new UnityConsoleLoggerProvider());

            return loggerFactory;
        }

        public static ILoggerFactory AddUmbrellaUnityWebBrowserConsoleLogger(this ILoggerFactory loggerFactory, LogLevel logLevel)
        {
            UnityWebBrowserConsoleLogger.LogLevel = logLevel;
            loggerFactory.AddProvider(new UnityWebBrowserConsoleLoggerProvider());

            return loggerFactory;
        }

        public static ILoggerFactory AddUmbrellaUnityRemoteClientLogger(this ILoggerFactory loggerFactory, Func<IServiceProvider> serviceProviderAccessor, LogLevel logLevel)
        {
            Guard.ArgumentNotNull(loggerFactory, nameof(loggerFactory));
            Guard.ArgumentNotNull(serviceProviderAccessor, nameof(serviceProviderAccessor));

            UnityRemoteClientLogger.LogLevel = logLevel;
            loggerFactory.AddProvider(new UnityRemoteClientLoggerProvider(serviceProviderAccessor));

            return loggerFactory;
        }
    }
}