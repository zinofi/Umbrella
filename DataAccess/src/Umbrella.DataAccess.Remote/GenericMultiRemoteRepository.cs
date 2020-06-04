using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.DataAccess.Remote.Exceptions;
using Umbrella.Utilities;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.DataAnnotations.Abstractions;

namespace Umbrella.DataAccess.Remote
{
	// TODO: Same here - create a GenericRemoteRepository.
	public abstract class GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource> : GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, RepoOptions>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, new()
		where TRemoteSource : Enum
	{
		public GenericMultiRemoteRepository(
			ILogger logger,
			ILookupNormalizer dataAccessLookupNormalizer,
			params IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>[] services)
			: base(logger, dataAccessLookupNormalizer, services)
		{
		}
	}

	public abstract class GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions> : GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, new()
		where TRemoteSource : Enum
		where TRepoOptions : RepoOptions, new()
	{
		public GenericMultiRemoteRepository(
			ILogger logger,
			ILookupNormalizer dataAccessLookupNormalizer,
			params IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>[] services)
			: base(logger, dataAccessLookupNormalizer, services)
		{
		}
	}

	//TODO: Pass through more detailed error messages / codes to callers
	public abstract class GenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService> : IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, new()
		where TRemoteSource : Enum
		where TRepoOptions : RepoOptions, new()
		where TService : IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>
	{
		#region Private Static Members
		private static readonly IReadOnlyCollection<TItem> _emptyItemList = Array.Empty<TItem>();
		private static readonly IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> _emptyRemoteSourceFailuresList = Array.Empty<RemoteSourceFailure<TRemoteSource>>();
		private static readonly IReadOnlyCollection<ValidationResult> _emptyValidationResultList = Array.Empty<ValidationResult>();
		#endregion

		#region Private Members
		private readonly IReadOnlyDictionary<Type, TService> _typedServiceDictionary;
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the lookup normalizer.
		/// </summary>
		protected ILookupNormalizer LookupNormalizer { get; }
		protected IReadOnlyList<TService> ServiceList { get; }
		protected IReadOnlyDictionary<TRemoteSource, TService> ServiceDictionary { get; }
		#endregion

		#region Constructors
		public GenericMultiRemoteRepository(
			ILogger logger,
			ILookupNormalizer lookupNormalizer,
			params TService[] services)
		{
			Guard.ArgumentNotNullOrEmpty(services, nameof(services));

			Log = logger;
			LookupNormalizer = lookupNormalizer;
			ServiceList = services;
			ServiceDictionary = services.ToDictionary(x => x.RemoteSourceType, x => x);
			_typedServiceDictionary = services.ToDictionary(x => x.GetType().GetInterfaces().First(y => y.Name.EndsWith(x.GetType().Name, StringComparison.OrdinalIgnoreCase)), x => x);
		}
		#endregion

		#region IGenericRemoteRepository Members
		/// <inheritdoc />
		public virtual async Task<(bool success, string message, TItem result)> FindByIdAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken = default, TRepoOptions options = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));
			EnsureOptions(ref options);

			try
			{
				var service = ServiceDictionary[remoteSourceType];

				var (statusCode, message, result) = await service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

				bool success = IsSuccessStatusCode(statusCode);

				if (!success || result == null)
					return (false, message, null);

				if (!await CheckAccessAsync(result, cancellationToken).ConfigureAwait(false))
					throw new UmbrellaDataAccessForbiddenException();

				await AfterItemLoadedAsync(result, cancellationToken, options).ConfigureAwait(false);

				return (true, message, result);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, remoteSourceType, options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the specified item.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures, IReadOnlyCollection<TItem> results)> FindAllAsync(int pageNumber = 0, int pageSizeRequest = 20, CancellationToken cancellationToken = default, TRepoOptions options = null, params SortExpression<TItem>[] sortExpressions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureOptions(ref options);

			try
			{
				var lstTask = new Task<(HttpStatusCode status, string message, IReadOnlyCollection<TItem> results)>[ServiceList.Count];
				var dicTask = new Dictionary<TRemoteSource, Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)>>(ServiceList.Count);

				for (int i = 0; i < ServiceList.Count; i++)
				{
					var service = ServiceList[i];
					var task = service.FindAllAsync(pageNumber, pageSizeRequest, cancellationToken, sortExpressions);

					lstTask[i] = task;
					dicTask.Add(service.RemoteSourceType, task);
				}

				await Task.WhenAll(lstTask).ConfigureAwait(false);

				bool allSuccess = true;
				List<RemoteSourceFailure<TRemoteSource>> lstSourceFailure = null;
				var lstResult = new List<TItem>();

				foreach (var item in dicTask)
				{
					var (statusCode, message, results) = item.Value.Result;

					if (IsSuccessStatusCode(statusCode))
					{
						lstResult.AddRange(results);
					}
					else
					{
						allSuccess = false;
						lstSourceFailure = lstSourceFailure ?? new List<RemoteSourceFailure<TRemoteSource>>();
						lstSourceFailure.Add(new RemoteSourceFailure<TRemoteSource>(item.Key, message));
					}
				}

				var sortedResults = lstResult.ApplySortExpressions(sortExpressions, new SortExpression<TItem>(x => x.Id, SortDirection.Ascending));

				var lstItem = sortedResults.ToList();

				if (lstItem.Count == 0)
					return (allSuccess, lstSourceFailure ?? _emptyRemoteSourceFailuresList, _emptyItemList);

				for (int i = 0; i < lstItem.Count; i++)
				{
					var item = lstItem[i];

					// Filter out any items that fail the access check
					if (!await CheckAccessAsync(item, cancellationToken).ConfigureAwait(false))
					{
						Log.WriteWarning(state: new { item.Id, item.Source }, message: "The specified item failed the access check. This should not happen.");

						lstItem.RemoveAt(i);
						i--;
					}
				}

				if (lstItem.Count == 0)
					return (allSuccess, lstSourceFailure ?? _emptyRemoteSourceFailuresList, _emptyItemList);

				await AfterAllItemsLoadedAsync(lstItem, cancellationToken, options).ConfigureAwait(false);

				return (allSuccess, lstSourceFailure ?? _emptyRemoteSourceFailuresList, lstItem.Take(pageSizeRequest).ToArray());
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSizeRequest, options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem retrieving the items.", exc);
			}
		}

		//public virtual async Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures, int totalCount)> FindTotalCountAsync(CancellationToken cancellationToken = default)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();

		//	try
		//	{
		//		var lstTask = new Task[ServiceList.Count];
		//		var dicTask = new Dictionary<TRemoteSource, Task<(HttpStatusCode status, string message, int totalCount)>>(ServiceList.Count);

		//		for (int i = 0; i < ServiceList.Count; i++)
		//		{
		//			var task = ServiceList[i].FindTotalCountAsync(cancellationToken);

		//			lstTask[i] = task;
		//			dicTask.Add(ServiceList[i].RemoteSourceType, task);
		//		}

		//		await Task.WhenAll(lstTask).ConfigureAwait(false);

		//		bool success = true;
		//		List<RemoteSourceFailure<TRemoteSource>> lstSourceFailure = null;
		//		int count = 0;

		//		foreach (var item in dicTask)
		//		{
		//			var (status, message, totalCount) = item.Value.Result;

		//			switch (status)
		//			{
		//				case HttpStatusCode.OK:
		//					count += totalCount;
		//					break;
		//				default:
		//					success = false;
		//					lstSourceFailure = lstSourceFailure ?? new List<RemoteSourceFailure<TRemoteSource>>();
		//					lstSourceFailure.Add(new RemoteSourceFailure<TRemoteSource>(item.Key, message));
		//					break;
		//			}
		//		}

		//		return (success, lstSourceFailure ?? _emptyRemoteSourceFailuresList, count);
		//	}
		//	catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
		//	{
		//		throw new UmbrellaDataAccessException("There has been a problem finding the total count of all items.", exc);
		//	}
		//}

		//public virtual async Task<(bool success, string message, bool exists)> ExistsByIdAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken = default)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();
		//	Guard.ArgumentNotNull(id, nameof(id));

		//	try
		//	{
		//		var service = ServiceDictionary[remoteSourceType];

		//		var (statusCode, message, exists) = await service.ExistsByIdAsync(id, cancellationToken);

		//		return (IsSuccessStatusCode(statusCode), message, exists);
		//	}
		//	catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
		//	{
		//		throw new UmbrellaDataAccessException("There has been a problem determining if the item with the specified id exists.", exc);
		//	}
		//}

		/// <inheritdoc />
		public virtual async Task<SaveResult<TItem>> SaveAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));
			EnsureOptions(ref options);

			try
			{
				if (options.SanitizeEntity)
					await SanitizeItemAsync(item, cancellationToken, options).ConfigureAwait(false);

				bool isNew = item.Id.Equals(default(TIdentifier));

				if (options.ValidateEntity)
				{
					var (isValid, results) = await ValidateItemAsync(item, cancellationToken, options).ConfigureAwait(false);

					if (!isValid)
						return new SaveResult<TItem>(false, item, results, !isNew);
				}

				await BeforeSavingItemAsync(item, cancellationToken, options).ConfigureAwait(false);

				if (!await CheckAccessAsync(item, cancellationToken).ConfigureAwait(false))
					throw new UmbrellaDataAccessForbiddenException();

				var service = ServiceDictionary[item.Source];

				var (statusCode, message, result) = await service.SaveAsync(item, cancellationToken).ConfigureAwait(false);

				// If the item is new and we get a conflict then it is because it already exists
				if (statusCode == HttpStatusCode.Conflict && isNew)
					return new SaveResult<TItem>(false, result, null, true);

				if (statusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(message))
					return new SaveResult<TItem>(false, result, new List<ValidationResult> { new ValidationResult(message) }, false);

				if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Created && statusCode != HttpStatusCode.NoContent)
					return new SaveResult<TItem>(false, result, new List<ValidationResult> { new ValidationResult("There has been a problem saving the item.") }, false);

				await AfterSavingItemAsync(result, cancellationToken, options).ConfigureAwait(false);

				return new SaveResult<TItem>(true, result, null, !isNew);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { item.Id, options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem saving the item.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures, IReadOnlyCollection<SaveResult<TItem>> saveResults)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default, TRepoOptions options = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrEmpty(items, nameof(items));

			items = items.Where(x => x != null);
			Guard.ArgumentNotNullOrEmpty(items, nameof(items));

			EnsureOptions(ref options);

			try
			{
				bool allValid = true;
				var lstResult = new List<SaveResult<TItem>>();

				foreach (TItem item in items)
				{
					if (options.SanitizeEntity)
						await SanitizeItemAsync(item, cancellationToken, options).ConfigureAwait(false);

					if (options.ValidateEntity)
					{
						var (isValid, validationResults) = await ValidateItemAsync(item, cancellationToken, options).ConfigureAwait(false);

						if (!isValid)
						{
							allValid = false;
							lstResult.Add(new SaveResult<TItem>(false, item, validationResults));
						}
					}
				}

				if (options.ValidateEntity && !allValid)
					return (false, _emptyRemoteSourceFailuresList, lstResult);

				foreach (TItem item in items)
				{
					await BeforeSavingItemAsync(item, cancellationToken, options).ConfigureAwait(false);

					if (!await CheckAccessAsync(item, cancellationToken).ConfigureAwait(false))
						throw new UmbrellaDataAccessForbiddenException();
				}

				var groups = items.GroupBy(x => x.Source).ToList();

				var lstTask = new Task[groups.Count];
				var dicTask = new Dictionary<TRemoteSource, Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)>>(groups.Count);

				for (int i = 0; i < groups.Count; i++)
				{
					IGrouping<TRemoteSource, TItem> group = groups[i];

					var service = ServiceDictionary[group.Key];

					var task = service.SaveAllAsync(items, cancellationToken);
					lstTask[i] = task;
					dicTask.Add(group.Key, task);
				}

				await Task.WhenAll(lstTask).ConfigureAwait(false);

				bool success = true;
				List<RemoteSourceFailure<TRemoteSource>> lstSourceFailure = null;
				var lstSaveResult = new List<SaveResult<TItem>>();

				foreach (var item in dicTask)
				{
					List<ValidationResult> lstValidationResult = null;

					var (statusCode, message, results) = item.Value.Result;

					// TODO: Could probably handle 409 more explicitly. Would also be good to get the remote services to return more specifics about why any particular item failed.
					// Too many breaking changes to consider at this stage though.
					switch (statusCode)
					{
						case HttpStatusCode.Conflict:
							// Just pretend like everything succeeded.
							break;
						case HttpStatusCode.BadRequest when !string.IsNullOrWhiteSpace(message):
							success = false;
							break;
						case var status when statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Created && statusCode != HttpStatusCode.NoContent:
							success = false;
							lstSourceFailure = lstSourceFailure ?? new List<RemoteSourceFailure<TRemoteSource>>();
							lstSourceFailure.Add(new RemoteSourceFailure<TRemoteSource>(item.Key, message));
							lstValidationResult = lstValidationResult ?? new List<ValidationResult>();
							lstValidationResult.Add(new ValidationResult(message));
							break;
					}

					if (results != null)
					{
						var mappedItems = results.Select(x => new SaveResult<TItem>(success, x, lstValidationResult ?? _emptyValidationResultList));
						lstSaveResult.AddRange(mappedItems);
					}
				}

				return (success, lstSourceFailure ?? _emptyRemoteSourceFailuresList, lstSaveResult);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ItemIds = items.Select(x => x.Id), options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem saving the items.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(bool success, string message)> DeleteAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken = default, TRepoOptions options = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));
			EnsureOptions(ref options);

			try
			{
				// TODO: Perform access check when deleting?

				// When deleting something we always have to ensure children are deleted as well to avoid orphans
				options.ProcessChildren = true;

				await BeforeDeletingItemAsync(id, remoteSourceType, cancellationToken, options).ConfigureAwait(false);

				var service = ServiceDictionary[remoteSourceType];

				var (statusCode, message) = await service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

				await AfterDeletingItemAsync(id, remoteSourceType, cancellationToken, options).ConfigureAwait(false);

				return (IsSuccessStatusCode(statusCode), message);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, remoteSourceType, options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem deleting the specified item.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures)> DeleteAllAsync(CancellationToken cancellationToken = default, TRepoOptions options = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureOptions(ref options);

			try
			{
				// TODO: Perform access check when deleting?

				// When deleting something we always have to ensure children are deleted as well to avoid orphans
				options.ProcessChildren = true;

				await BeforeDeletingAllAsync(cancellationToken, options).ConfigureAwait(false);

				var lstTask = new Task[ServiceList.Count];
				var dicTask = new Dictionary<TRemoteSource, Task<(HttpStatusCode statusCode, string message)>>(ServiceList.Count);

				for (int i = 0; i < ServiceList.Count; i++)
				{
					var task = ServiceList[i].DeleteAllAsync(cancellationToken);

					lstTask[i] = task;
					dicTask.Add(ServiceList[i].RemoteSourceType, task);
				}

				await Task.WhenAll(lstTask).ConfigureAwait(false);

				bool success = true;
				List<RemoteSourceFailure<TRemoteSource>> lstFailure = null;

				foreach (var item in dicTask)
				{
					var (statusCode, message) = item.Value.Result;

					if (!IsSuccessStatusCode(statusCode))
					{
						success = false;

						if (lstFailure == null)
							lstFailure = new List<RemoteSourceFailure<TRemoteSource>>();

						lstFailure.Add(new RemoteSourceFailure<TRemoteSource>(item.Key, message));
					}
				}

				if (!success)
					return (false, lstFailure ?? _emptyRemoteSourceFailuresList);

				await AfterDeletingAllAsync(cancellationToken, options).ConfigureAwait(false);

				return (true, lstFailure ?? _emptyRemoteSourceFailuresList);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { options }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem deleting the specified item.", exc);
			}
		}
		#endregion

		#region Protected Methods
		protected void ValidateServiceCount(int requiredCount)
		{
			if (ServiceList.Count != requiredCount)
				throw new UmbrellaHttpServiceAccessException($"The number of registered services is {ServiceList.Count} which does not match the required count of {requiredCount}.");
		}

		protected T GetHttpService<T>()
			where T : TService
			=> (T)_typedServiceDictionary[typeof(T)];

		protected void EnsureOptions(ref TRepoOptions options)
		{
			if (options == null)
				options = new TRepoOptions();
		}
		#endregion

		#region Events
		protected virtual Task SanitizeItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task<bool> CheckAccessAsync(TItem item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}

		/// <summary>
		/// Overriding this method allows you to perform custom validation on the item.
		/// By default, this calls into the <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}, bool)"/> method.
		/// By design, this doesn't recursively perform validation on the entity. If this is required, override this method and use the <see cref="IObjectGraphValidator"/>
		/// by injecting it as a service or perform more extensive validation elsewhere.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		protected virtual Task<(bool isValid, List<ValidationResult> results)> ValidateItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var lstResult = new List<ValidationResult>();

			var ctx = new ValidationContext(item);

			bool isValid = Validator.TryValidateObject(item, ctx, lstResult, true);

			return Task.FromResult((isValid, lstResult));
		}

		protected virtual Task AfterItemLoadedAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual async Task AfterAllItemsLoadedAsync(List<TItem> items, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var tasks = new Task[items.Count];

			for (int i = 0; i < items.Count; i++)
			{
				tasks[i] = AfterItemLoadedAsync(items[i], cancellationToken, options);
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		protected virtual Task BeforeSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task AfterSavingItemAsync(TItem item, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task BeforeDeletingItemAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task AfterDeletingItemAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task BeforeDeletingAllAsync(CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual Task AfterDeletingAllAsync(CancellationToken cancellationToken, TRepoOptions options)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		protected virtual bool IsSuccessStatusCode(HttpStatusCode statusCode)
			=> statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent;
		#endregion
	}
}