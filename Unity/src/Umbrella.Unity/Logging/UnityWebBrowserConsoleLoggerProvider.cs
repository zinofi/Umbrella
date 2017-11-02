using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Umbrella.Unity.Logging
{
    public class UnityWebBrowserConsoleLoggerProvider : ILoggerProvider
    {
        private ConcurrentDictionary<string, Microsoft.Extensions.Logging.ILogger> m_LoggersDictionary = new ConcurrentDictionary<string, Microsoft.Extensions.Logging.ILogger>();

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
                => m_LoggersDictionary.GetOrAdd(name, (x) => new UnityWebBrowserConsoleLogger(x));

        public void Dispose()
        {
            m_LoggersDictionary.Clear();
            m_LoggersDictionary = null;
        }
    }
}