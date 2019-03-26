using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.CustomTables;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.Kentico.DataAccess.CustomTables.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Sorting;

namespace Umbrella.Kentico.DataAccess.CustomTables
{
	public abstract class GenericCustomTableRepository<TItem> : GenericCustomTableRepository<TItem, RepoOptions>, IGenericCustomTableRepository<TItem>
		where TItem : CustomTableItem, new()
	{
		public GenericCustomTableRepository(
			ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer)
			: base(logger, dataAccessLookupNormalizer)
		{
		}
	}

	public abstract class GenericCustomTableRepository<TItem, TRepoOptions> : IGenericCustomTableRepository<TItem, TRepoOptions>
		where TItem : CustomTableItem, new()
		where TRepoOptions : RepoOptions, new()
	{
		#region Protected Properties
		protected ILogger Log { get; }
		protected IDataAccessLookupNormalizer DataAccessLookupNormalizer { get; }
		protected virtual IQueryable<TItem> Items => CustomTableItemProvider.GetItems<TItem>();
		#endregion

		#region Constructors
		public GenericCustomTableRepository(
			ILogger logger,
			IDataAccessLookupNormalizer dataAccessLookupNormalizer)
		{
			Log = logger;
			DataAccessLookupNormalizer = dataAccessLookupNormalizer;
		}
		#endregion

		#region IGenericCustomTableRepository<T> Members
		public virtual async Task<TItem> FindByIdAsync(int id, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureOptions(ref options);

			try
			{
				TItem item = Items.SingleOrDefault(x => x.ItemID == id);

				if (item == null)
					return null;

				await AfterItemLoadedAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

				return item;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the specified item.", exc);
			}
		}

		public virtual async Task<IReadOnlyCollection<TItem>> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, TRepoOptions options = null, RepoOptions[] childOptions = null, params SortExpression<TItem>[] sortExpressions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureOptions(ref options);

			try
			{
				IQueryable<TItem> query = Items
					.ApplySortExpressions(sortExpressions, new SortExpression<TItem>(x => x.ItemCreatedWhen, SortDirection.Ascending))
					.ApplyPagination(pageNumber, pageSize);

				List<TItem> lstItem = query.ToList();

				if (lstItem.Count == 0)
					return lstItem;

				await AfterAllItemsLoadedAsync(lstItem, cancellationToken, options, childOptions).ConfigureAwait(false);

				return lstItem;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSize, options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the items.", exc);
			}
		}

		public virtual async Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await Task.FromResult(Items.Count()).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the total item count.", exc);
			}
		}

		public virtual async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				bool exists = Items.Any(x => x.ItemID == id);

				return await Task.FromResult(exists);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem determining the existence of the specified item.", exc);
			}
		}

		public virtual async Task<SaveResult<TItem>> SaveAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));
			EnsureOptions(ref options);

			try
			{
				if (options.SanitizeEntity)
					await SanitizeItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

				bool isNew = item.ItemID == 0;

				if (options.ValidateEntity)
				{
					var (isValid, results) = await ValidateItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

					if (!isValid)
					{
						return new SaveResult<TItem>(isValid, item, results, !isNew);
					}
				}

				await BeforeSavingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

				CustomTableItemProvider.SetItem(item);

				await AfterSavingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

				return new SaveResult<TItem>(true, item, null, !isNew);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { item.ItemID, options, childOptions }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem saving the item.", exc);
			}
		}

		public virtual async Task<(bool success, IReadOnlyCollection<SaveResult<TItem>> saveResults)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(items, nameof(items));
			EnsureOptions(ref options);

			try
			{
				bool allValid = true;
				var lstResult = new List<SaveResult<TItem>>();

				foreach (TItem item in items)
				{
					if (options.SanitizeEntity)
						await SanitizeItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

					if (options.ValidateEntity)
					{
						var (isValid, results) = await ValidateItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

						if (!isValid)
						{
							allValid = false;
							lstResult.Add(new SaveResult<TItem>(false, item, results));
						}
					}
				}

				if (options.ValidateEntity && !allValid)
				{
					return (false, lstResult);
				}

				bool allSucceeded = true;

				foreach (TItem item in items)
				{
					var saveResult = new SaveResult<TItem>
					{
						Item = item
					};

					try
					{
						await BeforeSavingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

						// Kentico doesn't seem to offer a way of updating items in bulk which isn't great!
						CustomTableItemProvider.SetItem(item);

						await AfterSavingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

						saveResult.Success = true;
					}
					catch (Exception exc) when (Log.WriteError(exc, new { item.ItemID, options, childOptions }, $"There was a problem saving an item as part of the {nameof(SaveAllAsync)} method.", returnValue: true))
					{
						// Mask an error with any one item so that other can be saved. The results returned from this method will allow the caller
						// to determine which item(s) failed to save.
						allSucceeded = false;
					}

					lstResult.Add(saveResult);
				}

				return (allSucceeded, lstResult);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ItemIds = items.Select(x => x?.ItemID), options, childOptions }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem saving the items.", exc);
			}
		}

		public virtual async Task DeleteAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));
			EnsureOptions(ref options);

			try
			{
				// When deleting something we always have to ensure children are deleted as well to avoid orphans
				options.ProcessChildren = true;

				await BeforeDeletingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);

				CustomTableItemProvider.DeleteItem(item);

				await AfterDeletingItemAsync(item, cancellationToken, options, childOptions).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { item.ItemID }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem deleting the specified item.", exc);
			}
		}

		public virtual async Task DeleteAllAsync(CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureOptions(ref options);

			try
			{
				// When deleting something we always have to ensure children are deleted as well to avoid orphans
				options.ProcessChildren = true;

				IReadOnlyCollection<TItem> items = await FindAllAsync(cancellationToken: cancellationToken, options: options).ConfigureAwait(false);

				var deletionTasks = new Task[items.Count];

				for (int i = 0; i < items.Count; i++)
				{
					deletionTasks[i] = DeleteAsync(items.ElementAt(i), cancellationToken, options);
				}

				await Task.WhenAll(deletionTasks).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem deleting the items.", exc);
			}
		}
		#endregion

		#region Protected Methods
		protected void EnsureOptions(ref TRepoOptions options)
		{
			if (options == null)
				options = new TRepoOptions();
		}
		#endregion

		#region Events
		protected virtual Task SanitizeItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task<(bool isValid, List<ValidationResult> results)> ValidateItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var lstResult = new List<ValidationResult>();

			var ctx = new ValidationContext(item);
			bool isValid = Validator.TryValidateObject(item, ctx, lstResult);

			return Task.FromResult((isValid, lstResult));
		}

		protected virtual Task AfterItemLoadedAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual async Task AfterAllItemsLoadedAsync(List<TItem> items, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			Task[] tasks = new Task[items.Count];

			for (int i = 0; i < items.Count; i++)
			{
				tasks[i] = AfterItemLoadedAsync(items[i], cancellationToken, options, childOptions);
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		protected virtual Task BeforeSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task AfterSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task BeforeDeletingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task AfterDeletingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options, RepoOptions[] childOptions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}
		#endregion
	}
}