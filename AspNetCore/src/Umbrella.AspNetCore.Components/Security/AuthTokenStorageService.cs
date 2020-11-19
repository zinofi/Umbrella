using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AspNetCore.Components.Exceptions;

namespace Umbrella.AspNetCore.Components.Security
{
	public class AuthTokenStorageService : IAppAuthTokenStorageService
	{
		private const string AuthTokenStorageKey = "App.AuthToken";
		private const string ClientIdStorageKey = "App.ClientId";

		private readonly ILogger<AuthTokenStorageService> _logger;
		private readonly ILocalStorageService _localStorageService;

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthTokenStorageService"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="localStorageService">The local storage service.</param>
		public AuthTokenStorageService(
			ILogger<AuthTokenStorageService> logger,
			ILocalStorageService localStorageService)
		{
			_logger = logger;
			_localStorageService = localStorageService;
		}

		/// <inheritdoc />
		public async Task<string> GetClientIdAsync()
		{
			try
			{
				string? clientId = await _localStorageService.GetItemAsync<string?>(ClientIdStorageKey);

				if (string.IsNullOrEmpty(clientId))
				{
					clientId = Guid.NewGuid().ToString("N");
					await _localStorageService.SetItemAsync(ClientIdStorageKey, clientId);
				}

				return clientId;
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There was a problem reading the Client Id from local storage.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<string?> GetTokenAsync()
		{
			try
			{
				return await _localStorageService.GetItemAsync<string?>(AuthTokenStorageKey);
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There was a problem reading the auth token from local storage.", exc);
			}
		}

		/// <inheritdoc />
		public async Task SetTokenAsync(string? token)
		{
			try
			{
				if (!string.IsNullOrEmpty(token))
					await _localStorageService.SetItemAsync(AuthTokenStorageKey, token);
				else
					await _localStorageService.RemoveItemAsync(AuthTokenStorageKey);
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There was a problem saving the auth token to local storage.", exc);
			}
		}
	}
}