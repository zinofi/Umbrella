using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Umbrella.Unity.Logging
{
    public class UnityConsoleLoggerProvider : ILoggerProvider
    {
        private ConcurrentDictionary<string, ILogger> m_LoggersDictionary = new ConcurrentDictionary<string, ILogger>();

        public ILogger CreateLogger(string name)
                => m_LoggersDictionary.GetOrAdd(name, (x) => new UnityConsoleLogger(x));

        public void Dispose()
        {
            m_LoggersDictionary.Clear();
            m_LoggersDictionary = null;
        }
    }
}