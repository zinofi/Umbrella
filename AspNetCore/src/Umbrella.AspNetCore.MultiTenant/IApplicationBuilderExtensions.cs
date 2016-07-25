using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.MultiTenant.Middleware;

namespace Umbrella.AspNetCore.MultiTenant
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenantSessionContext<TAppTenantKey>(this IApplicationBuilder builder, string tenantClaimType)
        {
            return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey>>(tenantClaimType);
        }

        public static IApplicationBuilder UseMultiTenantSessionContext<TAppTenantKey, TNullableAppTenantKey>(this IApplicationBuilder builder, string tenantClaimType)
        {
            return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>>(tenantClaimType);
        }
    }
}