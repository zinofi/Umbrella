using Microsoft.Extensions.Logging;

namespace Umbrella.Utilities.log4net
{
    public static class Log4NetExtensions
    {
        public static void AddLog4Net(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new Log4NetProvider());
        }
    }
}
