// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Umbrella.AppFramework.Exceptions;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Security.Messages;
using Umbrella.AppFramework.Security.Options;
using Umbrella.Utilities.Security.Abstractions;

namespace Umbrella.AppFramework.Security;

/// <summary>
/// A base class which application specific authentication helpers can extend.
/// </summary>
/// <seealso cref="IAppAuthHelper" />
public class JwtAppAuthHelper : IAppAuthHelper
{
	private readonly ILogger _logger;
	private readonly IJwtUtility _jwtUtility;
	private readonly IAppAuthTokenStorageService _tokenStorageService;
	private readonly AppAuthHelperOptions _options;

	/// <inheritdoc />
	public event Func<ClaimsPrincipal, Task> OnAuthenticationStateChanged
	{
		add
		{
			Guard.IsNotNull(value?.Target);
			WeakReferenceMessenger.Default.TryRegister<AuthenticationStateChangedMessage>(value.Target, (sender, args) => _ = value(args.Value));
		}
		remove
		{
			Guard.IsNotNull(value?.Target);
			WeakReferenceMessenger.Default.Unregister<AuthenticationStateChangedMessage>(value.Target);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="JwtAppAuthHelper"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="jwtUtility">The JWT utility.</param>
	/// <param name="tokenStorageService">The token storage service.</param>
	/// <param name="options">The app auth helper options.</param>
	public JwtAppAuthHelper(
		ILogger<JwtAppAuthHelper> logger,
		IJwtUtility jwtUtility,
		IAppAuthTokenStorageService tokenStorageService,
		AppAuthHelperOptions options)
	{
		_logger = logger;
		_jwtUtility = jwtUtility;
		_tokenStorageService = tokenStorageService;
		_options = options;
	}

	/// <inheritdoc />
	public async ValueTask<ClaimsPrincipal> SetCurrentClaimsPrincipalAsync(string token, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(token);

		try
		{
			await _tokenStorageService.SetTokenAsync(token).ConfigureAwait(false);

			ClaimsPrincipal claimsPrincipal = await InitializeClaimsPrincipalAsync(token, cancellationToken).ConfigureAwait(false);
			Thread.CurrentPrincipal = claimsPrincipal;

			_ = WeakReferenceMessenger.Default.Send(new AuthenticationStateChangedMessage(claimsPrincipal));

			return claimsPrincipal;
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaAppFrameworkException("There has been a problem setting the ClaimsPrincipal.", exc);
		}
	}

	/// <inheritdoc />
	public async ValueTask<ClaimsPrincipal> GetCurrentClaimsPrincipalAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (ClaimsPrincipal.Current is not null)
				return ClaimsPrincipal.Current;

			string? token = await _tokenStorageService.GetTokenAsync().ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(token))
				return new ClaimsPrincipal(new ClaimsIdentity());

			return await InitializeClaimsPrincipalAsync(token!, cancellationToken);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaAppFrameworkException("There has been a problem getting the ClaimsPrincipal.", exc);
		}
	}

	/// <inheritdoc />
	public async ValueTask LocalLogoutAsync(bool executeDefaultPostLogoutAction = true, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			await _tokenStorageService.SetTokenAsync(null).ConfigureAwait(false);

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

			Thread.CurrentPrincipal = claimsPrincipal;

			_ = WeakReferenceMessenger.Default.Send(new AuthenticationStateChangedMessage(claimsPrincipal));

			if (executeDefaultPostLogoutAction && _options.PostLogoutAction is not null)
				await _options.PostLogoutAction().ConfigureAwait(false);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { executeDefaultPostLogoutAction }))
		{
			throw new UmbrellaAppFrameworkException("There has been a problem logging out the user locally.", exc);
		}
	}

	private async ValueTask<ClaimsPrincipal> InitializeClaimsPrincipalAsync(string token, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			IReadOnlyCollection<Claim> claims = _jwtUtility.ParseClaimsFromJwt(token!);

			return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
		}
		catch (Exception)
		{
			// There was a problem parsing the token. To ensure the user can get a new token,
			// remove it from storage.
			await _tokenStorageService.SetTokenAsync(null).ConfigureAwait(false);
			throw;
		}
	}
}