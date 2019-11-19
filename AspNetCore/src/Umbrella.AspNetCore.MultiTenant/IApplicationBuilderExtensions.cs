using System;
using Umbrella.AspNetCore.MultiTenant.Middleware;
using Umbrella.DataAccess.MultiTenant.Abstractions;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// Extension methods used to register Middleware for the <see cref="Umbrella.AspNetCore.MultiTenant"/> package with a specified <see cref="IApplicationBuilder"/>.
	/// </summary>
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.MultiTenant"/> Middleware to the specified <see cref="IApplicationBuilder"/>.
		/// </summary>
		/// <param name="builder">The application builder to which the Middleware will be added.</param>
		/// <returns>The <see cref="IApplicationBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="builder"/> is null.</exception>
		public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey>(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey>>();
		}

		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.MultiTenant"/> Middleware to the specified <see cref="IApplicationBuilder"/>.
		/// </summary>
		/// <param name="builder">The application builder to which the Middleware will be added.</param>
		/// <returns>The <see cref="IApplicationBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="builder"/> is null.</exception>
		/// <remarks>
		/// The ability to allow a <typeparamref name="TNullableAppTenantKey"/> to be specified allows for a <see cref="DbAppTenantSessionContext{TNullableAppTenantKey}"/>
		/// to be accessed in a context where the current user may or may not be associated with a specific tenant. For example, application users will be stored in a database
		/// and each user will normally be associated with a tenant. However, the same database table may contain users not associated to a single tenant, e.g. system administrators,
		/// and those users will need to perform actions outside the context of a single tenant. The provision of a nullable key allows for this under these specialized circumstances.
		/// </remarks>
		public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey, TNullableAppTenantKey>(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>>();
		}
	}
}