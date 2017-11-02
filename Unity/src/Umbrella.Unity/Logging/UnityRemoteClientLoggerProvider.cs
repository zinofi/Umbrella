using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities;

namespace Umbrella.Unity.Logging
{
    public class UnityRemoteClientLoggerProvider : ILoggerProvider
    {
        private ConcurrentDictionary<string, ILogger> m_LoggersDictionary = new ConcurrentDictionary<string, ILogger>();
        private readonly Func<IServiceProvider> m_ServiceProviderAccessor;

        public UnityRemoteClientLoggerProvider(Func<IServiceProvider> serviceProviderAccessor)
        {
            Guard.ArgumentNotNull(serviceProviderAccessor, nameof(serviceProviderAccessor));

            m_ServiceProviderAccessor = serviceProviderAccessor;
        }

        public ILogger CreateLogger(string name)
            => m_LoggersDictionary.GetOrAdd(name, x => new UnityRemoteClientLogger(m_ServiceProviderAccessor, x));

        public void Dispose()
        {
            m_LoggersDictionary.Clear();
            m_LoggersDictionary = null;
        }
    }
}