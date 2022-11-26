namespace Umbrella.Utilities.Configuration.Abstractions;

/// <summary>
/// Defines the contract for a read-only source of app settings.
/// </summary>
public interface IReadOnlyAppSettingsSource
{
	/// <summary>
	/// Gets the value with the specified <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The value.</returns>
	string? GetValue(string key);
}