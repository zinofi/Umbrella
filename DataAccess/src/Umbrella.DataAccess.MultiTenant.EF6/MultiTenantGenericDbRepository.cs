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
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiTenantGenericDbRepository{TEntity, TDbContext}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="userAuditDataFactory">The user audit data factory.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="entityValidator">The entity validator.</param>
		/// <param name="dbAppTenantSessionContext">The database application tenant session context.</param>
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
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiTenantGenericDbRepository{TEntity, TDbContext, TRepoOptions}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="userAuditDataFactory">The user audit data factory.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="entityValidator">The entity validator.</param>
		/// <param name="dbAppTenantSessionContext">The database application tenant session context.</param>
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
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiTenantGenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="userAuditDataFactory">The user audit data factory.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="entityValidator">The entity validator.</param>
		/// <param name="dbAppTenantSessionContext">The database application tenant session context.</param>
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
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiTenantGenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="userAuditDataFactory">The user audit data factory.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="entityValidator">The entity validator.</param>
		/// <param name="dbAppTenantSessionContext">The database application tenant session context.</param>
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
		/// <summary>
		/// Gets the application tenant session context.
		/// </summary>
		protected DbAppTenantSessionContext<TAppTenantKey> AppTenantSessionContext { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiTenantGenericDbRepository{TEntity, TDbContext, TRepoOptions, TEntityKey, TUserAuditKey, TAppTenantKey}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		/// <param name="entityValidator">The entity validator.</param>
		/// <param name="dbAppTenantSessionContext">The database application tenant session context.</param>
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

		/// <inheritdoc />
		protected override void PreSaveWork(TEntity entity, bool addToContext, out bool isNew)
		{
			if (entity is IAppTenantEntity<TAppTenantKey> tenantEntity && AppTenantSessionContext.IsAuthenticated)
				tenantEntity.AppTenantId = AppTenantSessionContext.AppTenantId;

			base.PreSaveWork(entity, addToContext, out isNew);
		}
	}
}