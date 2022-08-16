using Umbrella.Extensions.Logging.AppCenter;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.Logging
{
	/// <summary>
	/// A set of extension methods to configure Microsoft AppCenter to be used as a logging provider.
	/// </summary>
	public static class AppCenterLoggerFactoryExtensions
    {
		/// <summary>
		/// Adds Microsoft AppCenter as a logging provider.
		/// </summary>
		/// <param name="loggingBuilder">The logging builder.</param>
		/// <param name="minLevel">The minimum level.</param>
		/// <param name="includeEventId">if set to <c>true</c> includes the event id in log messages.</param>
		/// <returns>The <paramref name="loggingBuilder"/>.</returns>
		public static ILoggingBuilder AddAppCenter(
			this ILoggingBuilder loggingBuilder,
			LogLevel minLevel = LogLevel.Information,
			bool includeEventId = false)
		{
			loggingBuilder.AddProvider(new AppCenterLoggerProvider(minLevel, includeEventId));

			return loggingBuilder;
		}
	}
}