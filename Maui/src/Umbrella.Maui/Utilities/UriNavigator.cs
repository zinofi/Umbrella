using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Umbrella.AppFramework.Services.Abstractions;
using Umbrella.Maui.Exceptions;

namespace Umbrella.Maui.Utilities;

/// <summary>
/// A utility used to perform navigation to a specific URI.
/// </summary>
public class UriNavigator : IUriNavigatorService
{
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="UriNavigator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public UriNavigator(ILogger<UriNavigator> logger)
	{
		_logger = logger;
	}

	/// <inheritdoc />
	public async ValueTask OpenAsync(string uri, bool openInNewWindow)
	{
		try
		{
			_ = await MainThread.InvokeOnMainThreadAsync(async () => await Browser.OpenAsync(uri, openInNewWindow ? BrowserLaunchMode.External : BrowserLaunchMode.SystemPreferred));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { uri, openInNewWindow }))
		{
			throw new UmbrellaMauiException("There has been a problem opening the specified URI.", exc);
		}
	}
}