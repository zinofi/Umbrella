using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Umbrella.AppFramework.Security.Abstractions
{
	/// <summary>
	/// A helper used to perform common auth actions.
	/// </summary>
	/// <remarks>
	/// This is registered with DI as a scoped service due to internal dependencies which are also scoped.
	/// However, care must be taken when using this service in conjunction with HttpClientFactory and DelegatingHandlers
	/// because services resolved there use their own scope which means working with a different instance.
	/// </remarks>
	public interface IAppAuthHelper
	{
		/// <summary>
		/// Invoked when the authentication state changes.
		/// </summary>
		event Func<ClaimsPrincipal, Task> OnAuthenticationStateChanged;

		/// <summary>
		/// Gets the current claims principal.
		/// </summary>
		/// <param name="token">An optional token used to construct the claims principal.
		/// If this is not provided, the principal is constructed based on existing ambient state.</param>
		/// <returns>The <see cref="ClaimsPrincipal"/>.</returns>
		ValueTask<ClaimsPrincipal> GetCurrentClaimsPrincipalAsync(string? token = null);

		/// <summary>
		/// Logs out the current user from the app locally, i.e. without telling the server.
		/// </summary>
		/// <param name="executeDefaultPostLogoutAction">if set to <c>true</c> executes any additional registered post logout actions.</param>
		/// <returns>An task used to await completion of the operation.</returns>
		ValueTask LocalLogoutAsync(bool executeDefaultPostLogoutAction = true);
	}
}