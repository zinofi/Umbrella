using System.Security.Claims;
using Umbrella.DataAccess.MultiTenant.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.MultiTenant.Middleware.Options
{
	/// <summary>
	/// Options for the <see cref="MultiTenantSessionContextMiddleware{TAppTenantKey}"/> and <see cref="MultiTenantSessionContextMiddleware{TAppTenantKey, TNullableAppTenantKey}"/> middleware.
	/// </summary>
	/// <seealso cref="ISanitizableUmbrellaOptions" />
	/// <seealso cref="IValidatableUmbrellaOptions" />
	public class MultiTenantSessionContextMiddlewareOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// <para>
		/// Gets or sets the type of the tenant claim as stored on the <see cref="ClaimsPrincipal"/> of the currently authenticated user.
		/// Defaults to <see cref="ClaimTypes.GroupSid"/>.
		/// </para>
		/// <para>
		/// The value of this claim will be read from the current user's claims and assigned to the scoped instance of <see cref="DbAppTenantSessionContext{TAppTenantKey}"/> registered
		/// with the application's dependency injection container. The primary use of this would then be to perform a row filtering operation when accessing data to ensure
		/// that data cannot bleed across different tenants.
		/// </para>
		/// </summary>
		public string TenantClaimType { get; set; } = ClaimTypes.GroupSid;

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize() => TenantClaimType = TenantClaimType?.Trim();

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(TenantClaimType, nameof(TenantClaimType));
	}
}