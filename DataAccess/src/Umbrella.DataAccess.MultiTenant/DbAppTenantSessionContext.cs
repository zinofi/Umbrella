using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.MultiTenant
{
    public class DbAppTenantSessionContext<TAppTenantKey>
    {
        public TAppTenantKey AppTenantId { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
