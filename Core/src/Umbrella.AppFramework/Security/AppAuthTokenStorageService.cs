using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AppFramework.Security
{
	public class AppAuthTokenStorageService : IAppAuthTokenStorageService
	{
		private const string AuthTokenStorageKey = "App.AuthToken";
		private const string ClientIdStorageKey = "App.ClientId";

		private readonly ILogger _logger;
		private readonly IAppLocalStorageService _storageService;
		private string? _authToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppAuthTokenStorageService"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="storageService">The storage service.</param>
		public AppAuthTokenStorageService(
			ILogger<AppAuthTokenStorageService> logger,
			IAppLocalStorageService storageService)
		{
			_logger = logger;
			_storageService = storageService;
		}

		/// <inheritdoc />
		public async ValueTask<string> GetClientIdAsync()
		{
			try
			{
				string clientId = await _storageService.GetAsync(ClientIdStorageKey);

				if (string.IsNullOrEmpty(clientId))
				{
					clientId = Guid.NewGuid().ToString("N");
					await _storageService.SetAsync(ClientIdStorageKey, clientId);
				}

				return clientId;
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				return Guid.NewGuid().ToString("N");
			}
		}

		/// <inheritdoc />
		public async ValueTask<string?> GetTokenAsync()
		{
			try
			{
				_authToken = await _storageService.GetAsync(AuthTokenStorageKey);

				return _authToken;
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
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
					await _storageService.SetAsync(AuthTokenStorageKey, token!);
				}
				else
				{
					_authToken = null;
					await _storageService.RemoveAsync(AuthTokenStorageKey);
				}
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				// Do nothing
			}
		}
	}
}