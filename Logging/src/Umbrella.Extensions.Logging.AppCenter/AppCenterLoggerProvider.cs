using Microsoft.Extensions.Logging;

namespace Umbrella.Extensions.Logging.AppCenter
{
	internal class AppCenterLoggerProvider : ILoggerProvider
	{
		private readonly LogLevel _minLevel;
		private readonly bool _includeEventId;

		public AppCenterLoggerProvider(LogLevel minLevel, bool includeEventId)
		{
			_minLevel = minLevel;
			_includeEventId = includeEventId;
		}

		public ILogger CreateLogger(string categoryName) => new AppCenterLogger(categoryName, _minLevel, _includeEventId);

		public void Dispose()
		{
		}
	}
}