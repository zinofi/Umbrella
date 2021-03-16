﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Umbrella.AspNetCore.Blazor.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="IAuthorizationService" /> type.
	/// </summary>
	public static class IAuthorizationServiceExtensions
    {
		/// <summary>
		/// Checks if the specified<paramref name= "user" /> has the necessary specified <paramref name="roles"/> and meets the specified <paramref name="policyName"/>.
		/// </summary>
		/// <param name="authorizationService">The authorization service.</param>
		/// <param name="user">The user.</param>
		/// <param name="roles">The roles.</param>
		/// <param name="policyName">Name of the policy.</param>
		/// <returns>An awaitable <see cref="Task"/> containing the result.</returns>
		public static async Task<bool> AuthorizeRolesAndPolicyAsync(this IAuthorizationService authorizationService, ClaimsPrincipal user, string? roles, string? policyName)
		{
			if (string.IsNullOrEmpty(roles) && string.IsNullOrEmpty(policyName))
			{
				// No custom policy or roles have been specified. Just authorize based on whether or not the user
				// is authenticated.
				return user.Identity.IsAuthenticated;
			}
			else
			{
				// Start by assuming both types of checks are authorized.
				// If roles and/or a policy have been provided they can be invalidated.
				bool rolesAuthorized = true;
				bool policyAuthorized = true;

				if (!string.IsNullOrEmpty(roles))
				{
					string[] lstRole = roles.Split(',', StringSplitOptions.RemoveEmptyEntries);

					if (lstRole.Length > 0)
						rolesAuthorized = lstRole.Any(x => user.IsInRole(x));
				}

				if (!string.IsNullOrEmpty(policyName))
				{
					AuthorizationResult authResult = await authorizationService.AuthorizeAsync(user, policyName);
					policyAuthorized = authResult.Succeeded;
				}

				return rolesAuthorized && policyAuthorized;
			}
		}
    }
}