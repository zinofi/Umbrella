// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Shared.Security.Extensions;
using Umbrella.AspNetCore.Blazor.Exceptions;
using Umbrella.AspNetCore.Blazor.Security.Abstractions;
using Umbrella.AspNetCore.Blazor.Security.Options;

namespace Umbrella.AspNetCore.Blazor.Security;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider, IJwtAuthenticationStateProvider
{
	private readonly ILogger _logger;
	private readonly IAppAuthHelper _authHelper;
	private readonly IApplicationInsights _applicationInsights;
	private readonly JwtAuthenticationStateProviderOptions _options;

	public event EventHandler? AuthenticatedStateHasChanged;

	/// <summary>
	/// Initializes a new instance of the <see cref="JwtAuthenticationStateProvider"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="authHelper">The authentication helper.</param>
	/// <param name="applicationInsights">The application insights.</param>
	/// <param name="options">The options.</param>
	public JwtAuthenticationStateProvider(
		ILogger<JwtAuthenticationStateProvider> logger,
		IAppAuthHelper authHelper,
		IApplicationInsights applicationInsights,
		JwtAuthenticationStateProviderOptions options)
	{
		_logger = logger;
		_authHelper = authHelper;
		_applicationInsights = applicationInsights;
		_options = options;
		_authHelper.OnAuthenticationStateChanged += MarkUserAsAuthenticatedAsync;
	}

	/// <inheritdoc />
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			ClaimsPrincipal principal = await _authHelper.GetCurrentClaimsPrincipalAsync();

			// Check the refresh token for expiration
			DateTime? refreshTokenExpiration = principal.GetRefreshTokenExpiration();

			if (refreshTokenExpiration > DateTime.UtcNow)
			{
				if (_options.IsApplicationInsightsEnabled && principal.Identity?.Name is not null)
					await _applicationInsights.SetAuthenticatedUserContext(principal.Identity.Name);

				return new AuthenticationState(principal);
			}
			else
			{
				if (_options.IsApplicationInsightsEnabled)
					await _applicationInsights.ClearAuthenticatedUserContext();

				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaBlazorException("There has been a problem getting the authentication state.", exc);
		}
	}

	/// <inheritdoc />
	public async Task MarkUserAsAuthenticatedAsync(ClaimsPrincipal principal)
	{
		try
		{
			var authState = Task.FromResult(new AuthenticationState(principal));
			NotifyAuthenticationStateChanged(authState);
			AuthenticatedStateHasChanged?.Invoke(this, EventArgs.Empty);

			if (_options.IsApplicationInsightsEnabled && principal.Identity?.Name is not null)
				await _applicationInsights.SetAuthenticatedUserContext(principal.Identity.Name);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { principal.Identity?.Name }))
		{
			throw new UmbrellaBlazorException("There has been a problem marking the user as authenticated.", exc);
		}
	}

	/// <inheritdoc />
	public async Task MarkUserAsLoggedOutAsync()
	{
		try
		{
			var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
			NotifyAuthenticationStateChanged(authState);
			AuthenticatedStateHasChanged?.Invoke(this, EventArgs.Empty);

			if (_options.IsApplicationInsightsEnabled)
				await _applicationInsights.ClearAuthenticatedUserContext();
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaBlazorException("There has been a problem marking the user as logged out.", exc);
		}
	}
}