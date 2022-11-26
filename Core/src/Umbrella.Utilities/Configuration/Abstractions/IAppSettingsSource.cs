namespace Umbrella.Utilities.Configuration.Abstractions;

/// <summary>
/// Defines the contract for a source of app settings.
/// </summary>
/// <seealso cref="IReadOnlyAppSettingsSource" />
public interface IAppSettingsSource : IReadOnlyAppSettingsSource
{
	/// <summary>
	/// Sets the value for the specified <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	void SetValue(string key, string? value);
}