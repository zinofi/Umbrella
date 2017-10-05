using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Unity.Logging;

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
    }
}