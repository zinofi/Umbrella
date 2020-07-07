using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Security.Options;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Security.Extensions;

namespace Umbrella.AspNetCore.WebUtilities.Security
{
	/// <summary>
	/// A claims accessor implementation that covers the most common use cases. This class can be extended if required
	/// to add additional behaviours before registration with DI.
	/// </summary>
	/// <typeparam name="TUserId">The type of the user identifier.</typeparam>
	/// <typeparam name="TRole">The type of the role.</typeparam>
	/// <seealso cref="Umbrella.Utilities.Context.Abstractions.ICurrentUserIdAccessor{TUserId}" />
	/// <seealso cref="Umbrella.Utilities.Context.Abstractions.ICurrentUserRolesAccessor{TRole}" />
	public class UmbrellaClaimsUserAccessor<TUserId, TRole> : ICurrentUserIdAccessor<TUserId>, ICurrentUserRolesAccessor<TRole>
		where TRole : struct, Enum
	{
		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the HTTP context accessor.
		/// </summary>
		protected IHttpContextAccessor HttpContextAccessor { get; }

		/// <summary>
		/// Gets the options.
		/// </summary>
		protected UmbrellaClaimsUserAccessorOptions Options { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaClaimsUserAccessor{TUserId, TRole}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="httpContextAccessor">The HTTP context accessor.</param>
		/// <param name="options">The options.</param>
		public UmbrellaClaimsUserAccessor(
			ILogger<UmbrellaClaimsUserAccessor<TUserId, TRole>> logger,
			IHttpContextAccessor httpContextAccessor,
			UmbrellaClaimsUserAccessorOptions options)
		{
			Logger = logger;
			HttpContextAccessor = httpContextAccessor;
			Options = options;
		}

		/// <inheritdoc />
		public virtual TUserId CurrentUserId
		{
			get
			{
				try
				{
					return HttpContextAccessor.HttpContext.User.GetId<TUserId>(Options.NameIdentifierClaimType);
				}
				catch (Exception exc) when (Logger.WriteError(exc, message: "There has been a problem accessing the id of the current user."))
				{
					throw;
				}
			}
		}

		/// <inheritdoc />
		public virtual IReadOnlyCollection<string> RoleNames => HttpContextAccessor.HttpContext.User.GetRoleNames();

		/// <inheritdoc />
		public virtual IReadOnlyCollection<TRole> Roles => HttpContextAccessor.HttpContext.User.GetRoles<TRole>();
	}
}