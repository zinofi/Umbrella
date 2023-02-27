namespace Umbrella.AppFramework.Services.Abstractions;

/// <summary>
/// A utility used to perform navigation to a specific URI.
/// </summary>
public interface IUriNavigatorService
{
	/// <summary>
	/// Opens the specified URI on the current device.
	/// </summary>
	/// <param name="uri">The URI.</param>
	/// <param name="openInNewWindow">Specifies whether or not to open the <paramref name="uri"/> in the current or a new window.</param>
	/// <returns>An awaitable task that completes when the URI has been navigated to.</returns>
	ValueTask OpenAsync(string uri, bool openInNewWindow);
}