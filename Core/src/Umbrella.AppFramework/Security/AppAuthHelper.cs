// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Security.Claims;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
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
public class AppAuthHelper : IAppAuthHelper
{
	private static readonly ClaimsPrincipal _emptyPrincipal = new(new ClaimsIdentity());

	private readonly ILogger _logger;
	private readonly IJwtUtility _jwtUtility;
	private readonly IAppAuthTokenStorageService _tokenStorageService;
	private readonly AppAuthHelperOptions _options;

	// Declaring this as static but can't really be avoided because we can't declare this service as a singleton.
	// Could wrap this in a singleton service but not much point.
	private static volatile ClaimsPrincipal? _claimsPrincipal;

	/// <inheritdoc />
	public event Func<ClaimsPrincipal, Task> OnAuthenticationStateChanged
	{
		add => WeakReferenceMessenger.Default.TryRegister<AuthenticationStateChangedMessage>(value.Target, (sender, args) => value(args.Value));
		remove => WeakReferenceMessenger.Default.Unregister<AuthenticationStateChangedMessage>(value.Target);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AppAuthHelper"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="jwtUtility">The JWT utility.</param>
	/// <param name="tokenStorageService">The token storage service.</param>
	/// <param name="options">The app auth helper options.</param>
	public AppAuthHelper(
		ILogger<AppAuthHelper> logger,
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
	public async ValueTask<ClaimsPrincipal> GetCurrentClaimsPrincipalAsync(string? token = null)
	{
		try
		{
			if (!string.IsNullOrEmpty(token))
			{
				await _tokenStorageService.SetTokenAsync(token);
				_claimsPrincipal = null;
			}
			else if (_claimsPrincipal is not null)
			{
				return _claimsPrincipal;
			}
			else
			{
				token = await _tokenStorageService.GetTokenAsync();
			}

			if (string.IsNullOrWhiteSpace(token))
				return _emptyPrincipal;

			IReadOnlyCollection<Claim>? claims = null;

			try
			{
				claims = _jwtUtility.ParseClaimsFromJwt(token!);
			}
			catch (Exception)
			{
				// There was a problem parsing the token. To ensure the user can get a new token,
				// remove it from storage.
				await _tokenStorageService.SetTokenAsync(null);
				throw;
			}

			_claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
			_ = WeakReferenceMessenger.Default.Send(new AuthenticationStateChangedMessage(_claimsPrincipal));

			return _claimsPrincipal;
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaAppFrameworkException("There has been a problem getting the ClaimsPrincipal.", exc);
		}
	}

	/// <inheritdoc />
	public async ValueTask LocalLogoutAsync(bool executeDefaultPostLogoutAction = true)
	{
		try
		{
			await _tokenStorageService.SetTokenAsync(null);
			_claimsPrincipal = null;

			_ = WeakReferenceMessenger.Default.Send(new AuthenticationStateChangedMessage(new ClaimsPrincipal(new ClaimsIdentity())));

			if (executeDefaultPostLogoutAction && _options.PostLogoutAction is not null)
				await _options.PostLogoutAction();
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { executeDefaultPostLogoutAction }))
		{
			throw new UmbrellaAppFrameworkException("There has been a problem logging out the user locally.", exc);
		}
	}
}