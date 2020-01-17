using System;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.EF6;
using Umbrella.DataAccess.MultiTenant.Abstractions;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.DataAccess.MultiTenant.EF6
{
	public abstract class MultiTenantGenericDbRepository<TEntity, TDbContext> : MultiTenantGenericDbRepository<TEntity, TDbContext, RepoOptions>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
	{
		public MultiTenantGenericDbRepository(
			TDbContext dbContext,
			ICurrentUserIdAccessor<int> userAuditDataFactory,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			IEntityValidator entityValidator,
			DbAppTenantSessionContext<int> dbAppTenantSessionContext)
			: base(dbContext, userAuditDataFactory, logger, lookupNormalizer, entityValidator, dbAppTenantSessionContext)
		{
		}
	}

	public abstract class MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions> : MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions, int>
		where TEntity : class, IEntity<int>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
	{
		public MultiTenantGenericDbRepository(
			TDbContext dbContext,
			ICurrentUserIdAccessor<int> userAuditDataFactory,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			IEntityValidator entityValidator,
			DbAppTenantSessionContext<int> dbAppTenantSessionContext)
			: base(dbContext, userAuditDataFactory, logger, lookupNormalizer, entityValidator, dbAppTenantSessionContext)
		{
		}
	}

	public abstract class MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey> : MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		public MultiTenantGenericDbRepository(
			TDbContext dbContext,
			ICurrentUserIdAccessor<int> userAuditDataFactory,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			IEntityValidator entityValidator,
			DbAppTenantSessionContext<int> dbAppTenantSessionContext)
			: base(dbContext, userAuditDataFactory, logger, lookupNormalizer, entityValidator, dbAppTenantSessionContext)
		{
		}
	}

	public abstract class MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey> : MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, int, int>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
	{
		public MultiTenantGenericDbRepository(
			TDbContext dbContext,
			ICurrentUserIdAccessor<int> userAuditDataFactory,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			IEntityValidator entityValidator,
			DbAppTenantSessionContext<int> dbAppTenantSessionContext)
			: base(dbContext, userAuditDataFactory, logger, lookupNormalizer, entityValidator, dbAppTenantSessionContext)
		{
		}
	}

	public abstract class MultiTenantGenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey, TAppTenantKey> : GenericDbRepository<TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey>
		where TEntity : class, IEntity<TEntityKey>
		where TDbContext : UmbrellaDbContext
		where TRepoOptions : RepoOptions, new()
		where TEntityKey : IEquatable<TEntityKey>
		where TUserAuditKey : IEquatable<TUserAuditKey>
		where TAppTenantKey : IEquatable<TAppTenantKey>
	{
		protected DbAppTenantSessionContext<TAppTenantKey> AppTenantSessionContext { get; }

		public MultiTenantGenericDbRepository(
			TDbContext dbContext,
			ICurrentUserIdAccessor<TUserAuditKey> currentUserIdAccessor,
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			IEntityValidator entityValidator,
			DbAppTenantSessionContext<TAppTenantKey> dbAppTenantSessionContext)
			: base(dbContext, logger, lookupNormalizer, currentUserIdAccessor, entityValidator)
		{
			AppTenantSessionContext = dbAppTenantSessionContext;
		}

		protected override void PreSaveWork(TEntity entity, bool addToContext, out bool isNew)
		{
			if (entity is IAppTenantEntity<TAppTenantKey> tenantEntity && AppTenantSessionContext.IsAuthenticated)
				tenantEntity.AppTenantId = AppTenantSessionContext.AppTenantId;

			base.PreSaveWork(entity, addToContext, out isNew);
		}
	}
}