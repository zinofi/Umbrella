using System;
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
		[Obsolete("Use the overload that accepts ILoggingBuilder instead.")]
		public static ILoggerFactory AddUmbrellaLog4Net(this ILoggerFactory loggerFactory, string contentRootPath, string configFileRelativePath)
		{
			loggerFactory.AddProvider(new Log4NetProvider(contentRootPath, configFileRelativePath));

			return loggerFactory;
		}

		/// <summary>
		/// This method will configure log4net using the file path provided.
		/// </summary>
		/// <param name="loggingBuilder">The <see cref="ILoggingBuilder"/> to add this provider to.</param>
		/// <param name="contentRootPath">The root path of the application.</param>
		/// <param name="configFileRelativePath">The file path to the config file relative to <paramref name="contentRootPath"/>
		/// </param>
		/// <returns>The instance of <see cref="ILoggingBuilder"/> with the log4net provider added to it.</returns>
		public static ILoggingBuilder AddUmbrellaLog4Net(this ILoggingBuilder loggingBuilder, string contentRootPath, string configFileRelativePath)
		{
			loggingBuilder.AddProvider(new Log4NetProvider(contentRootPath, configFileRelativePath));

			return loggingBuilder;
		}
	}
}