using System.Threading.Tasks;

namespace Umbrella.AppFramework.Utilities.Abstractions;

/// <summary>
/// A persistent storage service used to store string values used by the app.
/// </summary>
public interface IAppLocalStorageService
{
	/// <summary>
	/// Gets the value with the specified <paramref name="key"/> or <see langword="null" /> if it does not exist.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>An awaitable Task that returns when the operations completes.</returns>
	ValueTask<string?> GetAsync(string key);

	/// <summary>
	/// Sets the value using the specified <paramref name="key"/>.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns>An awaitable Task that returns when the operations completes.</returns>
	ValueTask SetAsync(string key, string value);

	/// <summary>
	/// Removes the item with the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>An awaitable Task that returns when the operations completes.</returns>
	ValueTask RemoveAsync(string key);
}