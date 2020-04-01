using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="IAuthorizationService" /> type.
	/// </summary>
	public static class IAuthorizationServiceExtensions
	{
		/// <summary>
		/// Authorizes the specified 
		/// </summary>
		/// <typeparam name="TResource">The type of the resource being authorized for access by the specifed user.</typeparam>
		/// <param name="authorizationService">The authorization service.</param>
		/// <param name="user">The user.</param>
		/// <param name="resources">The resources.</param>
		/// <param name="policyName">Name of the policy.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> containing the result.</returns>
		public static async Task<bool> AuthorizeAllAsync<TResource>(this IAuthorizationService authorizationService, ClaimsPrincipal user, IEnumerable<TResource> resources, string policyName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var tasks = new List<Task<AuthorizationResult>>();

			foreach (var item in resources)
			{
				tasks.Add(authorizationService.AuthorizeAsync(user, item, policyName));
			}

			AuthorizationResult[] authResults = await Task.WhenAll(tasks);

			return authResults.All(x => x.Succeeded);
		}
	}
}