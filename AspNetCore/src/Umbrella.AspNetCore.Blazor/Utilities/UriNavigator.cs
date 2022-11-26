using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AspNetCore.Blazor.Exceptions;

namespace Umbrella.AspNetCore.Blazor.Utilities;

/// <summary>
/// A utility used to perform navigation to a specific URI.
/// </summary>
public class UriNavigator : IUriNavigator
{
	private readonly ILogger _logger;
	private readonly NavigationManager _navigationManager;

	/// <summary>
	/// Initializes a new instance of the <see cref="UriNavigator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="navigationManager">The navigation manager.</param>
	public UriNavigator(
		ILogger<UriNavigator> logger,
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