using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.DataAccess.EF6;
using Umbrella.DataAccess.MultiTenant.Interfaces;

namespace Umbrella.DataAccess.MultiTenant.EF6
{
    public abstract class MultiTenantGenericRepository<TEntity, TDbContext> : MultiTenantGenericRepository<TEntity, TDbContext, RepoOptions>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
        public MultiTenantGenericRepository(TDbContext dbContext,
            IUserAuditDataFactory<int> userAuditDataFactory,
            ILogger logger,
            IDataAccessLookupNormalizer lookupNormalizer,
            DbAppTenantSessionContext<int> dbAppTenantSessionContext)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer, dbAppTenantSessionContext)
        {
        }
    }

    public abstract class MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions> : MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions, int>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
    {
        public MultiTenantGenericRepository(TDbContext dbContext,
            IUserAuditDataFactory<int> userAuditDataFactory,
            ILogger logger,
            IDataAccessLookupNormalizer lookupNormalizer,
            DbAppTenantSessionContext<int> dbAppTenantSessionContext)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer, dbAppTenantSessionContext)
        {
        }
    }

    public abstract class MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
        where TEntityKey : IEquatable<TEntityKey>
    {
        public MultiTenantGenericRepository(TDbContext dbContext,
            IUserAuditDataFactory<int> userAuditDataFactory,
            ILogger logger,
            IDataAccessLookupNormalizer lookupNormalizer,
            DbAppTenantSessionContext<int> dbAppTenantSessionContext)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer, dbAppTenantSessionContext)
        {
        }
    }

    public abstract class MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int, int>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
        where TEntityKey : IEquatable<TEntityKey>
    {
        public MultiTenantGenericRepository(TDbContext dbContext,
            IUserAuditDataFactory<int> userAuditDataFactory,
            ILogger logger,
            IDataAccessLookupNormalizer lookupNormalizer,
            DbAppTenantSessionContext<int> dbAppTenantSessionContext)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer, dbAppTenantSessionContext)
        {
        }
    }

    public abstract class MultiTenantGenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey, TAppTenantKey> : GenericRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey>
        where TEntity : class, IEntity<TEntityKey>
        where TDbContext : DbContext
        where TRepoOptions : RepoOptions, new()
        where TEntityKey : IEquatable<TEntityKey>
    {
        private readonly DbAppTenantSessionContext<TAppTenantKey> m_DbAppTenantSessionContext;

        protected DbAppTenantSessionContext<TAppTenantKey> AppTenantSessionContext => m_DbAppTenantSessionContext;

        public MultiTenantGenericRepository(TDbContext dbContext,
            IUserAuditDataFactory<TUserAuditKey> userAuditDataFactory,
            ILogger logger,
            IDataAccessLookupNormalizer lookupNormalizer,
            DbAppTenantSessionContext<TAppTenantKey> dbAppTenantSessionContext)
            : base(dbContext, userAuditDataFactory, logger, lookupNormalizer)
        {
            m_DbAppTenantSessionContext = dbAppTenantSessionContext;
        }

        protected override void PreSaveWork(TEntity entity, bool addToContext, out bool isNew)
        {
            IAppTenantEntity<TAppTenantKey> tenantEntity = entity as IAppTenantEntity<TAppTenantKey>;

            if (tenantEntity != null && m_DbAppTenantSessionContext.IsAuthenticated)
                tenantEntity.AppTenantId = m_DbAppTenantSessionContext.AppTenantId;

            base.PreSaveWork(entity, addToContext, out isNew);
        }
    }
}
