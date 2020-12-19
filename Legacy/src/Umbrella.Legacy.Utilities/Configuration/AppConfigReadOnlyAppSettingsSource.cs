using System.Configuration;
using Umbrella.Utilities.Configuration.Abstractions;

namespace Umbrella.Legacy.Utilities.Configuration
{
	/// <summary>
	/// Provides readonly access to settings stored in either web.config or app.config (depending on context).
	/// </summary>
	/// <seealso cref="IReadOnlyAppSettingsSource" />
	public class AppConfigReadOnlyAppSettingsSource : IReadOnlyAppSettingsSource
    {
        #region IReadOnlyAppSettingsAccessor Members
		/// <inheritdoc />
        public string GetValue(string key) => ConfigurationManager.AppSettings[key];
        #endregion
    }
}