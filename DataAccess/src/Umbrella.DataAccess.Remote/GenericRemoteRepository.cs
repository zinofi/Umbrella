using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.DataAnnotations.Abstractions;
using Umbrella.Utilities.Http;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Http.Constants;

namespace Umbrella.DataAccess.Remote
{
	/// <summary>
	/// A generic repository used to query and update a remote resource over HTTP.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TSlimItem">The type of the slim item.</typeparam>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <typeparam name="TCreateItem">The type of the create item.</typeparam>
	/// <typeparam name="TCreateResult">The type of the create result.</typeparam>
	/// <typeparam name="TUpdateItem">The type of the update item.</typeparam>
	/// <typeparam name="TUpdateResult">The type of the update result.</typeparam>
	public abstract class GenericRemoteRepository<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult> : IGenericRemoteRepository<TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult>
		where TItem : class, IRemoteItem<TIdentifier>
		where TSlimItem : class, IRemoteItem<TIdentifier>
		where TUpdateItem : class, IRemoteItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
		where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
	{
		/// <summary>
		/// Gets the API URL.
		/// </summary>
		/// <remarks>This is relative to the server and should not have a leading or trailing slash, e.g. api/v1/people</remarks>
		protected abstract string ApiUrl { get; }

		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the remote service.
		/// </summary>
		protected IGenericHttpService RemoteService { get; }

		/// <summary>
		/// Gets the HTTP service utility.
		/// </summary>
		protected IGenericHttpServiceUtility HttpServiceUtility { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericRemoteRepository{TItem, TIdentifier, TSlimItem, TPaginatedResultModel, TCreateItem, TCreateResult, TUpdateItem, TUpdateResult}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="httpService">The HTTP service.</param>
		/// <param name="httpServiceUtility">The HTTP service utility.</param>
		public GenericRemoteRepository(
			ILogger logger,
			IGenericHttpService httpService,
			IGenericHttpServiceUtility httpServiceUtility)
		{
			Logger = logger;
			RemoteService = httpService;
			HttpServiceUtility = httpServiceUtility;
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<TItem>> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			try
			{
				var parameters = new Dictionary<string, string>
				{
					["id"] = id.ToString()
				};

				IHttpCallResult<TItem> result = await RemoteService.GetAsync<TItem>(ApiUrl, parameters, cancellationToken).ConfigureAwait(false);

				if (result.Success && result.Result is not null)
					await AfterItemLoadedAsync(result.Result, cancellationToken).ConfigureAwait(false);

				return result;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { id }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem finding the specified item.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<TPaginatedResultModel>> FindAllSlimAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.Or)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var parameters = HttpServiceUtility.CreateSearchQueryParameters(pageNumber, pageSize, sorters, filters, filterCombinator);

				IHttpCallResult<TPaginatedResultModel> result = await RemoteService.GetAsync<TPaginatedResultModel>(ApiUrl + "/SearchSlim", parameters, cancellationToken).ConfigureAwait(false);

				if (result.Success && result.Result is not null)
					await AfterAllItemsLoadedAsync(result.Result.Items, cancellationToken).ConfigureAwait(false);

				return result;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { pageNumber, pageSize, sorters, filters, filterCombinator }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem finding the items using the specified parameters.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<int>> FindTotalCountAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				return await RemoteService.GetAsync<int>(ApiUrl + "/Count", cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem finding the total count.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<bool>> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			try
			{
				var parameters = new Dictionary<string, string>
				{
					["id"] = id.ToString()
				};

				return await RemoteService.GetAsync<bool>(ApiUrl + "/Exists", parameters, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { id }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem determining if the specified item exists.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(IHttpCallResult<TCreateResult> result, IReadOnlyCollection<ValidationResult> validationResults)> CreateAsync(TCreateItem item, CancellationToken cancellationToken = default, bool sanitize = true, bool validate = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				return await SaveCoreAsync<TCreateItem, TCreateResult>(HttpMethod.Post, item, cancellationToken, sanitize, validate).ConfigureAwait(false);
			}
			catch (UmbrellaDataAccessConcurrencyException)
			{
				// Just rethrow without logging.
				throw;
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem creating the specified item.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(IHttpCallResult<TUpdateResult> result, IReadOnlyCollection<ValidationResult> validationResults)> UpdateAsync(TUpdateItem item, CancellationToken cancellationToken = default, bool sanitize = true, bool validate = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				if (item.Id.Equals(default!))
					throw new Exception("The item being updated must have an Id");

				return await SaveCoreAsync<TUpdateItem, TUpdateResult>(HttpMethod.Put, item, cancellationToken, sanitize, validate).ConfigureAwait(false);
			}
			catch (UmbrellaDataAccessConcurrencyException)
			{
				// Just rethrow without logging.
				throw;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { item.Id }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem updating the specified item.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			try
			{
				var parameters = new Dictionary<string, string>
				{
					["id"] = id.ToString()
				};

				IHttpCallResult result = await RemoteService.DeleteAsync(ApiUrl, parameters, cancellationToken).ConfigureAwait(false);

				if (result.Success)
					await AfterItemDeletedAsync(id, cancellationToken).ConfigureAwait(false);

				return result;
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { id }, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There was a problem deleting the specified item.", exc);
			}
		}

		#region Protected Methods		
		/// <summary>
		/// Performs the core save operation logic.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="method">The method.</param>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
		/// <param name="validate">if set to <c>true</c> validated the <paramref name="item"/> before saving.</param>
		/// <returns>The result of the save operation.</returns>
		protected virtual async Task<(IHttpCallResult<TResult> result, IReadOnlyCollection<ValidationResult> validationResults)> SaveCoreAsync<T, TResult>(HttpMethod method, T item, CancellationToken cancellationToken, bool sanitize, bool validate)
		{
			if (item is not null)
			{
				if (sanitize)
					await SanitizeItemAsync(item, cancellationToken).ConfigureAwait(false);

				if (validate)
				{
					var (isValid, results) = await ValidateItemAsync(item, cancellationToken).ConfigureAwait(false);

					if (!isValid)
						return (new HttpCallResult<TResult>(false), results);
				}
			}

			IHttpCallResult<TResult> result = method switch
			{
				var _ when method == HttpMethod.Post => await RemoteService.PostAsync<T, TResult>(ApiUrl, item, cancellationToken: cancellationToken).ConfigureAwait(false),
				var _ when method == HttpMethod.Put => await RemoteService.PutAsync<T, TResult>(ApiUrl, item, cancellationToken: cancellationToken).ConfigureAwait(false),
				_ => throw new NotSupportedException()
			};

			if (result.Success && item is not null)
				await AfterItemSavedAsync(item, cancellationToken).ConfigureAwait(false);
			else if (result.ProblemDetails?.Code?.Equals(HttpProblemCodes.ConcurrencyStampMismatch, StringComparison.OrdinalIgnoreCase) == true)
				throw new UmbrellaDataAccessConcurrencyException("The server has reported a concurrency stamp mismatch.");

			return (result, result.ProblemDetails?.ToValidationResults() ?? Array.Empty<ValidationResult>());
		}

		/// <summary>
		/// Sanitizes the item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task to await sanitization of the item.</returns>
		protected virtual Task SanitizeItemAsync(object item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform custom validation on the item.
		/// By default, this calls into the <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}, bool)"/> method.
		/// By design, this doesn't recursively perform validation on the entity. If this is required, override this method and use the <see cref="IObjectGraphValidator"/>
		/// by injecting it as a service or perform more extensive validation elsewhere.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
		protected virtual Task<(bool isValid, List<ValidationResult> results)> ValidateItemAsync(object item, CancellationToken cancellationToken)
		{
			var lstResult = new List<ValidationResult>();

			var ctx = new ValidationContext(item);

			bool isValid = Validator.TryValidateObject(item, ctx, lstResult, true);

			return Task.FromResult((isValid, lstResult));
		}

		/// <summary>
		/// Override this in a derived type to perform an operation on the item after it has been loaded.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		protected virtual Task AfterItemLoadedAsync(object item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		///  Override this in a derived type to perform an operation on the items after they have been loaded.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		protected virtual Task AfterAllItemsLoadedAsync(IEnumerable<object> items, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform any work after the item has been saved.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		protected virtual Task AfterItemSavedAsync(object item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform any work after the item has been deleted.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		protected virtual Task AfterItemDeletedAsync(TIdentifier id, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}
		#endregion
	}
}