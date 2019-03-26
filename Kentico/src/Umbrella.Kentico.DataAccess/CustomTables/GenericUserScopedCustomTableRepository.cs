using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.CustomTables;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.Kentico.DataAccess.CustomTables.Abstractions;

namespace Umbrella.Kentico.DataAccess.CustomTables
{
	public abstract class GenericUserScopedCustomTableRepository<TItem, TUserId> : GenericUserScopedCustomTableRepository<TItem, RepoOptions, TUserId>, IGenericCustomTableRepository<TItem>
		where TItem : CustomTableItem, IUserScopedCustomTableItem<TUserId>, new()
	{
		public GenericUserScopedCustomTableRepository(ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer,
			IUserAuditDataFactory<TUserId> currentUserIdAccessor)
			: base(logger, dataAccessLookupNormalizer, currentUserIdAccessor)
		{
		}
	}

	public abstract class GenericUserScopedCustomTableRepository<TItem, TRepoOptions, TUserId> : GenericCustomTableRepository<TItem, TRepoOptions>, IGenericCustomTableRepository<TItem, TRepoOptions>
		where TItem : CustomTableItem, IUserScopedCustomTableItem<TUserId>, new()
		where TRepoOptions : RepoOptions, new()
	{
		protected IUserAuditDataFactory<TUserId> CurrentUserIdAccessor { get; }
		protected TUserId CurrentUserId => CurrentUserIdAccessor.CurrentUserId;
		protected override sealed IQueryable<TItem> Items
		{
			get
			{
				if (CurrentUserId == default)
					throw new UmbrellaDataAccessException("Calls to the underlying data source can only be made in the context of an authenticated user.");

				return base.Items.Where(x => x.UserId.Equals(CurrentUserId));
			}
		}

		public GenericUserScopedCustomTableRepository(
			ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer,
			IUserAuditDataFactory<TUserId> currentUserIdAccessor)
			: base(logger, dataAccessLookupNormalizer)
		{
			CurrentUserIdAccessor = currentUserIdAccessor;
		}

		protected override async Task BeforeSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await base.BeforeSavingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

			item.UserId = CurrentUserId;
		}
	}
}