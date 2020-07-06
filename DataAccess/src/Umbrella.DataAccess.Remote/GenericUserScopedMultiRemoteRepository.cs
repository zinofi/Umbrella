using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.DataAccess.Remote
{
	// TODO: Create a GenericUserScopedRemoteRepository.

	public abstract class GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TUserId> : GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, RepoOptions, TUserId>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, IUserScopedRemoteItem<TUserId>, new()
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
		where TUserId : IEquatable<TUserId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericUserScopedMultiRemoteRepository{TItem, TIdentifier, TRemoteSource, TUserId}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataAccessLookupNormalizer">The data access lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="services">The services.</param>
		public GenericUserScopedMultiRemoteRepository(
			ILogger logger,
			ILookupNormalizer dataAccessLookupNormalizer,
			ICurrentUserIdAccessor<TUserId> currentUserIdAccessor,
			params IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>[] services)
			: base(logger, dataAccessLookupNormalizer, currentUserIdAccessor, services)
		{
		}
	}

	public abstract class GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TUserId> : GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>, TUserId>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, IUserScopedRemoteItem<TUserId>, new()
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
		where TRepoOptions : RepoOptions, new()
		where TUserId : IEquatable<TUserId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericUserScopedMultiRemoteRepository{TItem, TIdentifier, TRemoteSource, TRepoOptions, TUserId}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataAccessLookupNormalizer">The data access lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="services">The services.</param>
		public GenericUserScopedMultiRemoteRepository(
			ILogger logger,
			ILookupNormalizer dataAccessLookupNormalizer,
			ICurrentUserIdAccessor<TUserId> currentUserIdAccessor,
			params IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>[] services)
			: base(logger, dataAccessLookupNormalizer, currentUserIdAccessor, services)
		{
		}
	}

	public abstract class GenericUserScopedMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService, TUserId> : GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, IUserScopedRemoteItem<TUserId>, new()
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
		where TRepoOptions : RepoOptions, new()
		where TService : IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>
		where TUserId : IEquatable<TUserId>
	{
		/// <summary>
		/// Gets the current user identifier accessor.
		/// </summary>
		protected ICurrentUserIdAccessor<TUserId> CurrentUserIdAccessor { get; }

		/// <summary>
		/// Gets the current user identifier.
		/// </summary>
		protected TUserId CurrentUserId => CurrentUserIdAccessor.CurrentUserId;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericUserScopedMultiRemoteRepository{TItem, TIdentifier, TRemoteSource, TRepoOptions, TService, TUserId}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataAccessLookupNormalizer">The data access lookup normalizer.</param>
		/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
		/// <param name="services">The services.</param>
		public GenericUserScopedMultiRemoteRepository(
			ILogger logger,
			ILookupNormalizer dataAccessLookupNormalizer,
			ICurrentUserIdAccessor<TUserId> currentUserIdAccessor,
			params TService[] services)
			: base(logger, dataAccessLookupNormalizer, services)
		{
			CurrentUserIdAccessor = currentUserIdAccessor;
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		protected override async Task BeforeSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await base.BeforeSavingItemAsync(item, cancellationToken, options).ConfigureAwait(false);

			item.UserId = CurrentUserId;
		}
	}
}