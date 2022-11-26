using System.Threading.Tasks;

namespace Umbrella.AppFramework.Security.Abstractions;

/// <summary>
/// A storage service for application auth tokens.
/// </summary>
public interface IAppAuthTokenStorageService
{
	/// <summary>
	/// Gets the current token or null if it does not exist.
	/// </summary>
	/// <returns>The token.</returns>
	ValueTask<string?> GetTokenAsync();

	/// <summary>
	/// Stores the provided <paramref name="token"/> in the underlying storage system.
	/// </summary>
	/// <param name="token">The token.</param>
	/// <returns>An awaitable task which completes when the token has been stored.</returns>
	ValueTask SetTokenAsync(string? token);

	/// <summary>
	/// Gets the the current client id. This is used to the device on which this method is being
	/// executed and can be used to track tokens across different devices.
	/// </summary>
	/// <returns>The client id.</returns>
	ValueTask<string> GetClientIdAsync();
}