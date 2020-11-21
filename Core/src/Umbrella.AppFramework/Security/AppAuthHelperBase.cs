using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Exceptions;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.Utilities.Security.Abstractions;

namespace Umbrella.AppFramework.Security
{
	/// <summary>
	/// A base class from which application specific authentication helpers can extend.
	/// </summary>
	/// <seealso cref="IAppAuthHelper" />
	public abstract class AppAuthHelperBase : IAppAuthHelper
	{
		private static readonly ClaimsPrincipal _emptyPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

		private readonly ILogger _logger;
		private readonly IJwtUtility _jwtUtility;
		private readonly IAppAuthTokenStorageService _tokenStorageService;

		// Declaring this as static but can't really be avoided because we can't declare this service as a singleton.
		// Could wrap this in a singleton service but not much point.
		private static ClaimsPrincipal? _claimsPrincipal;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppAuthHelperBase"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="jwtUtility">The JWT utility.</param>
		/// <param name="tokenStorageService">The token storage service.</param>
		public AppAuthHelperBase(
			ILogger logger,
			IJwtUtility jwtUtility,
			IAppAuthTokenStorageService tokenStorageService)
		{
			_logger = logger;
			_jwtUtility = jwtUtility;
			_tokenStorageService = tokenStorageService;
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
				else if (_claimsPrincipal != null)
				{
					return _claimsPrincipal;
				}
				else
				{
					token = await _tokenStorageService.GetTokenAsync();
				}

				if (string.IsNullOrWhiteSpace(token))
					return _emptyPrincipal;

				_claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(_jwtUtility.ParseClaimsFromJwt(token), "jwt"));

				return _claimsPrincipal;
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
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

				if (executeDefaultPostLogoutAction)
					await ExecutePostLogoutActionAsync();
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { executeDefaultPostLogoutAction }, returnValue: true))
			{
				throw new UmbrellaAppFrameworkException("There has been a problem logging out the user locally.", exc);
			}
		}

		/// <summary>
		/// An action to be executed after logging out the current user locally.
		/// </summary>
		/// <returns>A Task that can be awaited for this operation to complete.</returns>
		protected abstract ValueTask ExecutePostLogoutActionAsync();
	}
}