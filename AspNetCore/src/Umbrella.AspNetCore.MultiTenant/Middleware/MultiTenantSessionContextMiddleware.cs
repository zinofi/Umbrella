using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbrella.DataAccess.MultiTenant;

namespace Umbrella.AspNetCore.MultiTenant.Middleware
{
    public class MultiTenantSessionContextMiddleware<TAppTenantKey>
    {
        private readonly RequestDelegate m_Next;
        private readonly ILogger m_Logger;
        private readonly string m_TenantClaimType;

        public MultiTenantSessionContextMiddleware(RequestDelegate next, ILogger<MultiTenantSessionContextMiddleware<TAppTenantKey>> logger, string tenantClaimType)
        {
            m_Next = next;
            m_Logger = logger;
            m_TenantClaimType = tenantClaimType;
        }

        public async Task Invoke(HttpContext context, DbAppTenantSessionContext<TAppTenantKey> dbAppAuthSessionContext)
        {
            try
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    string strAppTenantId = context.User.Claims.SingleOrDefault(x => x.Type == m_TenantClaimType)?.Value;

                    if (!string.IsNullOrWhiteSpace(strAppTenantId))
                    {
                        TAppTenantKey id = (TAppTenantKey)Convert.ChangeType(strAppTenantId, typeof(TAppTenantKey));

                        dbAppAuthSessionContext.AppTenantId = id;
                    }

                    dbAppAuthSessionContext.IsAuthenticated = true;
                }

                await m_Next.Invoke(context);
            }
            catch (Exception exc) when (m_Logger.WriteError(exc))
            {
                throw;
            }
        }
    }

    public class MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>
    {
        private readonly RequestDelegate m_Next;
        private readonly ILogger m_Logger;
        private readonly string m_TenantClaimType;

        public MultiTenantSessionContextMiddleware(RequestDelegate next, ILogger<MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>> logger, string tenantClaimType)
        {
            m_Next = next;
            m_Logger = logger;
            m_TenantClaimType = tenantClaimType;
        }

        public async Task Invoke(HttpContext context, DbAppTenantSessionContext<TAppTenantKey> dbAppAuthSessionContext, DbAppTenantSessionContext<TNullableAppTenantKey> dbNullableAppAuthSessionContext)
        {
            try
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    string strAppTenantId = context.User.Claims.SingleOrDefault(x => x.Type == m_TenantClaimType)?.Value;

                    if (!string.IsNullOrWhiteSpace(strAppTenantId))
                    {
                        TAppTenantKey id = (TAppTenantKey)Convert.ChangeType(strAppTenantId, typeof(TAppTenantKey));

                        dbAppAuthSessionContext.AppTenantId = id;

                        if (!id.Equals(default(TAppTenantKey)))
                            dbNullableAppAuthSessionContext.AppTenantId = (dynamic)id;
                    }

                    dbAppAuthSessionContext.IsAuthenticated = true;
                    dbNullableAppAuthSessionContext.IsAuthenticated = true;
                }

                await m_Next.Invoke(context);
            }
            catch (Exception exc) when (m_Logger.WriteError(exc))
            {
                throw;
            }
        }
    }
}