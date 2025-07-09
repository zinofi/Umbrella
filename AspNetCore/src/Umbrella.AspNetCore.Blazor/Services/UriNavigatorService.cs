using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Services.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services;

/// <summary>
/// A service used to perform navigation to a specific URI.
/// </summary>
public class UriNavigatorService : IUriNavigatorService
{
	private readonly ILogger _logger;
	private readonly NavigationManager _navigationManager;

	/// <summary>
	/// Initializes a new instance of the <see cref="UriNavigatorService"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="navigationManager">The navigation manager.</param>
	public UriNavigatorService(
		ILogger<UriNavigatorService> logger,
		NavigationManager navigationManager)
	{
		_logger = logger;
		_navigationManager = navigationManager;
	}

	/// <inheritdoc />
	public ValueTask OpenAsync(string uri, bool openInNewWindow)
	{
		try
		{
			_navigationManager.NavigateTo(uri);

			return default;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { uri, openInNewWindow }))
		{
			throw new UmbrellaBlazorException("There has been a problem opening the specified URI.", exc);
		}
	}
}