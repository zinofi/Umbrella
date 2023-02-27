using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Security.Options;
using Umbrella.AppFramework.Services.Abstractions;

namespace Umbrella.AppFramework.Security;

/// <summary>
/// A storage service for application auth tokens.
/// </summary>
/// <seealso cref="IAppAuthTokenStorageService" />
public class AppAuthTokenStorageService : IAppAuthTokenStorageService
{
	private readonly ILogger _logger;
	private readonly IAppLocalStorageService _localStorageService;
	private readonly IAppSessionStorageService _sessionStorageService;
	private readonly AppAuthTokenStorageServiceOptions _options;
	private string? _authToken;

	/// <summary>
	/// Initializes a new instance of the <see cref="AppAuthTokenStorageService"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="storageService">The storage service.</param>
	/// <param name="sessionStorageService">The session storage service.</param>
	/// <param name="options">The options.</param>
	public AppAuthTokenStorageService(
		ILogger<AppAuthTokenStorageService> logger,
		IAppLocalStorageService storageService,
		IAppSessionStorageService sessionStorageService,
		AppAuthTokenStorageServiceOptions options)
	{
		_logger = logger;
		_localStorageService = storageService;
		_sessionStorageService = sessionStorageService;
		_options = options;
	}

	/// <inheritdoc />
	public async ValueTask<string> GetClientIdAsync()
	{
		try
		{
			string? clientId = await _localStorageService.GetAsync(_options.ClientIdStorageKey).ConfigureAwait(false);

			if (string.IsNullOrEmpty(clientId))
			{
				clientId = Guid.NewGuid().ToString("N");
				await _localStorageService.SetAsync(_options.ClientIdStorageKey, clientId).ConfigureAwait(false);
			}

			return clientId!;
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			// If something goes wrong just return a new id without storing it.
			return Guid.NewGuid().ToString("N");
		}
	}

	/// <inheritdoc />
	public async ValueTask<string?> GetTokenAsync()
	{
		try
		{
			_authToken = _options.UseAuthTokenLocalStorage
				? await _localStorageService.GetAsync(_options.AuthTokenStorageKey).ConfigureAwait(false)
				: await _sessionStorageService.GetAsync(_options.AuthTokenStorageKey).ConfigureAwait(false);

			return _authToken;
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			// Just return the value assigned to the local variable if we can't read it from storage.
			// This should ensure auth at least works for the period the app is running.
			return _authToken;
		}
	}

	/// <inheritdoc />
	public async ValueTask SetTokenAsync(string? token)
	{
		try
		{
			// Mirror the token in a local variable to ensure that if secure storage isn't available that the app
			// will still work even though it means the user will have to login again when they next run it.
			if (!string.IsNullOrEmpty(token))
			{
				_authToken = token;

				if (_options.UseAuthTokenLocalStorage)
					await _localStorageService.SetAsync(_options.AuthTokenStorageKey, token!).ConfigureAwait(false);
				else
					await _sessionStorageService.SetAsync(_options.AuthTokenStorageKey, token!).ConfigureAwait(false);
			}
			else
			{
				_authToken = null;

				if (_options.UseAuthTokenLocalStorage)
					await _localStorageService.RemoveAsync(_options.AuthTokenStorageKey).ConfigureAwait(false);
				else
					await _sessionStorageService.RemoveAsync(_options.AuthTokenStorageKey).ConfigureAwait(false);
			}
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			// Do nothing here. The token will still be persisted in memory
			// which at worst means a user will have to reauthenticate the next time
			// they use the application.
		}
	}
}