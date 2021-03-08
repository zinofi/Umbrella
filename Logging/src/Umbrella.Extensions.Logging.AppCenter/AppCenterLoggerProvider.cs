using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.AppCenter
{
	/// <summary>
	/// <see cref="ILoggerProvider"/> implementation that creates instances of <see cref="AppCenterLogger"/>.
	/// </summary>
	/// <seealso cref="Microsoft.Extensions.Logging.ILoggerProvider" />
	[ProviderAlias("AppCenter")]
	public class AppCenterLoggerProvider : ILoggerProvider
	{
		private readonly LogLevel _minLevel;
		private readonly bool _includeEventId;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppCenterLoggerProvider"/> class.
		/// </summary>
		/// <param name="minLevel">The minimum level.</param>
		/// <param name="includeEventId">if set to <c>true</c> [include event identifier].</param>
		public AppCenterLoggerProvider(LogLevel minLevel, bool includeEventId)
		{
			_minLevel = minLevel;
			_includeEventId = includeEventId;
		}

		/// <inheritdoc />
		public ILogger CreateLogger(string categoryName) => new AppCenterLogger(categoryName, _minLevel, _includeEventId);

		/// <inheritdoc />
		public void Dispose()
		{
		}
	}
}