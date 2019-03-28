using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.DataAccess.Remote.Abstractions;

namespace Umbrella.DataAccess.Remote
{
	public abstract class GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TUserId> : GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, RepoOptions, TUserId>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, IUserScopedRemoteItem<TUserId>, new()
		where TRemoteSource : Enum
	{
		public GenericUserScopedMultiRemoteRepository(
			ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer,
			IUserAuditDataFactory<TUserId> currentUserIdAccessor,
			params IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>[] services)
			: base(logger, dataAccessLookupNormalizer, currentUserIdAccessor, services)
		{
		}
	}

	public abstract class GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TUserId> : GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>, TUserId>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, IUserScopedRemoteItem<TUserId>, new()
		where TRemoteSource : Enum
		where TRepoOptions : RepoOptions, new()
	{
		public GenericUserScopedMultiRemoteRepository(
			ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer,
			IUserAuditDataFactory<TUserId> currentUserIdAccessor,
			params IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>[] services)
			: base(logger, dataAccessLookupNormalizer, currentUserIdAccessor, services)
		{
		}
	}

	public abstract class GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService, TUserId> : GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, IUserScopedRemoteItem<TUserId>, new()
		where TRemoteSource : Enum
		where TRepoOptions : RepoOptions, new()
		where TService : IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>
	{
		protected IUserAuditDataFactory<TUserId> CurrentUserIdAccessor { get; }
		protected TUserId CurrentUserId => CurrentUserIdAccessor.CurrentUserId;

		public GenericUserScopedMultiRemoteRepository(
			ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer,
			IUserAuditDataFactory<TUserId> currentUserIdAccessor,
			params TService[] services)
			: base(logger, dataAccessLookupNormalizer, services)
		{
			CurrentUserIdAccessor = currentUserIdAccessor;
		}

		protected override async Task<bool> CheckAccessAsync(TItem item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			bool isValid = await base.CheckAccessAsync(item, cancellationToken).ConfigureAwait(false);

			if (isValid)
			{
				return item.UserId.Equals(CurrentUserId);
			}

			return isValid;
		}

		protected override async Task BeforeSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await base.BeforeSavingItemAsync(item, cancellationToken, options).ConfigureAwait(false);

			item.UserId = CurrentUserId;
		}
	}
}