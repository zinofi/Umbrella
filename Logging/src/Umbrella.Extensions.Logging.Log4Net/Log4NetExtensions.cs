using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.Log4Net
{
    public static class Log4NetExtensions
    {
        public static void AddLog4Net(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new Log4NetProvider());
        }
    }
}
