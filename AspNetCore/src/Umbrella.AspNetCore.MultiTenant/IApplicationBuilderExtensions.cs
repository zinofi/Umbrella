using Umbrella.AspNetCore.MultiTenant.Middleware;

namespace Microsoft.AspNetCore.Builder
{
	public static class IApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey>(this IApplicationBuilder builder, string tenantClaimType)
		{
			return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey>>(tenantClaimType);
		}

		public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey, TNullableAppTenantKey>(this IApplicationBuilder builder, string tenantClaimType)
		{
			return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>>(tenantClaimType);
		}
	}
}
