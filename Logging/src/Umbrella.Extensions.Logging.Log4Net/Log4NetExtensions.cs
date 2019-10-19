using log4net;
using log4net.Config;
using System.IO;
using Umbrella.Extensions.Logging.Log4Net;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// A set of extension methods to configure log4net to be used by your application
    /// </summary>
    public static class Log4NetExtensions
    {
        /// <summary>
        /// This method will configure log4net using the file path provided.
        /// </summary>
		/// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to add this provider to.</param>
        /// <param name="contentRootPath">The root path of the application.</param>
        /// <param name="configFileRelativePath">The file path to the config file relative to <paramref name="contentRootPath"/>
        /// </param>
		/// <returns>The instance of <see cref="ILoggerFactory"/> with the log4net provider added to it.</returns>
        public static ILoggerFactory AddUmbrellaLog4Net(this ILoggerFactory loggerFactory, string contentRootPath, string configFileRelativePath)
        {
			// TODO: This method should be removed and replaced (or marked as obsolete at least).
			// The way to add loggers in .NET Core 3 is via the IHostBuilder.ConfigureLogging method which allows you
			// to add loggers via ILoggingBuilder.
			// Therefore, this extension method needs to be changed to accept ILoggerBuilder instead of ILoggerFactory.
			// That type is actually inside the Micrsoft.Extensions.Logging assembly though. Hmmm... Hopefully they've moved it
			// because we don't really want to take a hard dependency on that. Maybe this approach will still be fine in .NET Core 3??
			GlobalContext.Properties["appRoot"] = contentRootPath;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(contentRootPath, configFileRelativePath)));

            loggerFactory.AddProvider(new Log4NetProvider());

            return loggerFactory;
        }
    }
}