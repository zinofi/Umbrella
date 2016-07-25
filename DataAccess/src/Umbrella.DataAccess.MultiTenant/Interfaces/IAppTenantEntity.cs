using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.MultiTenant.Interfaces
{
    public interface IAppTenantEntity : IAppTenantEntity<int>
    {
    }

    public interface IAppTenantEntity<TKey>
    {
        TKey AppTenantId { get; set; }
    }
}