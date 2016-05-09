using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Umbrella.Extensions.Logging.Log4Net
{
    /// <summary>
    /// A set of extension methods to configure log4net to be used by your application
    /// </summary>
    public static class Log4NetExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new Log4NetProvider());

            return loggerFactory;
        }

        /// <summary>
        /// This method will configure log4net using the file path provided.
        /// </summary>
        /// <param name="appEnv">The environment in which your application is running.</param>
        /// <param name="configFileRelativePath">The file path to the relative to the value of <see cref="IApplicationEnvironment.ApplicationBasePath"/>.
        /// This path is the root of your CLI application and is normally the parent directory of your wwwroot folder.
        /// </param>
        public static void ConfigureLog4Net(this IApplicationEnvironment appEnv, string configFileRelativePath)
        {
            GlobalContext.Properties["appRoot"] = appEnv.ApplicationBasePath;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(appEnv.ApplicationBasePath, configFileRelativePath)));
        }
    }
}