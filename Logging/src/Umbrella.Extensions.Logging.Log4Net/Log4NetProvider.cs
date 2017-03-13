using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Umbrella.Extensions.Logging.Log4Net
{
    public class Log4NetProvider : ILoggerProvider
    {
        private ConcurrentDictionary<string, ILogger> m_LoggersDictionary = new ConcurrentDictionary<string, ILogger>();

        public ILogger CreateLogger(string name)
        {
            return m_LoggersDictionary.GetOrAdd(name, (x) => new Log4NetAdapter(x));
        }

        public void Dispose()
        {
            m_LoggersDictionary.Clear();
            m_LoggersDictionary = null;
        }
    }
}