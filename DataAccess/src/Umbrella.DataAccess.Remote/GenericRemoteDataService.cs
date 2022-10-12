// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using Umbrella.DataAccess.Remote.Exceptions;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Pagination;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.DataAnnotations.Abstractions;
using Umbrella.Utilities.DataAnnotations.Enumerations;
using Umbrella.Utilities.Http;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Http.Exceptions;

namespace Umbrella.DataAccess.Remote;

/// <summary>
/// A generic data service used to query and update resources using HTTP.
/// </summary>
public abstract class GenericRemoteDataService
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GenericRemoteDataService"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="remoteService">The remote service.</param>
	/// <param name="httpServiceUtility">The HTTP service utility.</param>
	/// <param name="validator">The validator.</param>
	protected GenericRemoteDataService(
		ILogger logger,
		IGenericHttpService remoteService,
		IGenericHttpServiceUtility httpServiceUtility,
		IUmbrellaValidator validator)
	{
		Logger = logger;
		RemoteService = remoteService;
		HttpServiceUtility = httpServiceUtility;
		Validator = validator;
	}

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
	/// Gets the validator.
	/// </summary>
	protected IUmbrellaValidator Validator { get; }

	/// <summary>
	/// Finds all slim results of <typeparamref name="TSlimItem"/> on the server using the specified parameters.
	/// </summary>
	/// <typeparam name="TPaginatedResultModel">The type of the paginated result model.</typeparam>
	/// <typeparam name="TSlimItem">The type of the slim item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <param name="endpointPath">The relative API endpoint to call.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">Size of the page.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="sorters">The sorters.</param>
	/// <param name="filters">The filters.</param>
	/// <param name="filterCombinator">The filter combinator.</param>
	/// <param name="afterAllItemsLoadedCallback">The optional callback to invoke on the results.</param>
	/// <returns>The result of the remote operation.</returns>
	/// <exception cref="UmbrellaRemoteDataAccessException" />
	protected virtual async Task<IHttpCallResult<TPaginatedResultModel?>> GetAllSlimAsync<TPaginatedResultModel, TSlimItem, TIdentifier>(
		string endpointPath,
		int pageNumber = 0,
		int pageSize = 20,
		CancellationToken cancellationToken = default,
		IEnumerable<SortExpressionDescriptor>? sorters = null,
		IEnumerable<FilterExpressionDescriptor>? filters = null,
		FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.And,
		Func<IEnumerable<TSlimItem>, CancellationToken, Task>? afterAllItemsLoadedCallback = null)
		where TIdentifier : IEquatable<TIdentifier>
		where TSlimItem : class, IKeyedItem<TIdentifier>
		where TPaginatedResultModel : PaginatedResultModel<TSlimItem>
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			var parameters = HttpServiceUtility.CreateSearchQueryParameters(pageNumber, pageSize, sorters, filters, filterCombinator);

			IHttpCallResult<TPaginatedResultModel?> result = await RemoteService.GetAsync<TPaginatedResultModel>(ApiUrl + endpointPath, parameters, cancellationToken).ConfigureAwait(false);

			if (result.Success && result.Result is not null && afterAllItemsLoadedCallback is not null)
				await afterAllItemsLoadedCallback(result.Result.Items, cancellationToken).ConfigureAwait(false);

			return result;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { endpointPath, pageNumber, pageSize, sorters, filters, filterCombinator }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem finding the items using the specified parameters.", exc);
		}
	}

	/// <summary>
	/// Finds the specified resource on the remote server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="afterItemLoadedCallback">An optional callback to be invoked on the result.</param>
	/// <param name="endpointPath">The optional relative endpoint path to call. By default this is empty and is a RESTful API call.</param>
	/// <returns>The result of the remote operation.</returns>
	/// <exception cref="UmbrellaRemoteDataAccessException" />
	protected async Task<IHttpCallResult<TItem?>> GetByIdAsync<TItem, TIdentifier>(
		TIdentifier id,
		CancellationToken cancellationToken,
		Func<TItem, CancellationToken, Task>? afterItemLoadedCallback = null,
		string endpointPath = "")
		where TItem : class, IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(id, nameof(id));

		try
		{
			var parameters = new Dictionary<string, string>
			{
				["id"] = id.ToString()
			};

			IHttpCallResult<TItem?> result = await RemoteService.GetAsync<TItem>(ApiUrl + endpointPath, parameters, cancellationToken).ConfigureAwait(false);

			if (result.Success && result.Result is not null && afterItemLoadedCallback is not null)
				await afterItemLoadedCallback(result.Result, cancellationToken).ConfigureAwait(false);

			return result;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { id, endpointPath }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem finding the specified item.", exc);
		}
	}

	/// <summary>
	/// POSTs the specified resource on the remote server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TPostResult">The type of the post result.</typeparam>
	/// <param name="item">The item.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
	/// <param name="validationType">The type of validation to be performed on the <paramref name="item"/> before saving.</param>
	/// <param name="afterItemSavedCallback">An optional callback to be invoked after the item has been successfully POSTed.</param>
	/// <param name="endpointPath">The optional relative endpoint path to call. By default this is empty and is a RESTful API call.</param>
	/// <returns>The result of the remote operation.</returns>
	/// <exception cref="UmbrellaRemoteDataAccessConcurrencyException" />
	/// <exception cref="UmbrellaRemoteDataAccessException" />
	protected virtual async Task<(IHttpCallResult<TPostResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> PostAsync<TItem, TPostResult>(
		TItem item,
		CancellationToken cancellationToken = default,
		bool sanitize = true,
		ValidationType validationType = ValidationType.Shallow,
		Func<TItem, TPostResult?, CancellationToken, Task>? afterItemSavedCallback = null,
		string endpointPath = "")
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(item, nameof(item));

		try
		{
			return await SaveCoreAsync(HttpMethod.Post, endpointPath, item, cancellationToken, sanitize, validationType, afterItemSavedCallback).ConfigureAwait(false);
		}
		catch (UmbrellaHttpServiceConcurrencyException exc)
		{
			throw new UmbrellaRemoteDataAccessConcurrencyException("There has been a concurrency error whilst posting the specified item.", exc);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { endpointPath, sanitize, validationType }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem posting the specified item.", exc);
		}
	}

	/// <summary>
	/// PUTs the specified resource on the remote server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TPutResult">The type of the PUT result.</typeparam>
	/// <param name="item">The item.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
	/// <param name="validationType">The type of validation to be performed on the <paramref name="item"/> before saving.</param>
	/// <param name="afterItemSavedCallback">An optional callback to be invoked after the item has been successfully PUTed.</param>
	/// <param name="endpointPath">The optional relative endpoint path to call. By default this is empty and is a RESTful API call.</param>
	/// <returns>The result of the remote operation.</returns>
	/// <exception cref="UmbrellaRemoteDataAccessConcurrencyException" />
	/// <exception cref="UmbrellaRemoteDataAccessException" />
	protected virtual async Task<(IHttpCallResult<TPutResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> PutAsync<TItem, TIdentifier, TPutResult>(
		TItem item,
		CancellationToken cancellationToken = default,
		bool sanitize = true,
		ValidationType validationType = ValidationType.Shallow,
		Func<TItem, TPutResult?, CancellationToken, Task>? afterItemSavedCallback = null,
		string endpointPath = "")
		where TItem : class, IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(item, nameof(item));

		try
		{
			return item.Id.Equals(default!)
				? throw new Exception("The item being patched must have an Id.")
				: await SaveCoreAsync(HttpMethod.Put, endpointPath, item, cancellationToken, sanitize, validationType, afterItemSavedCallback).ConfigureAwait(false);
		}
		catch (UmbrellaHttpServiceConcurrencyException exc)
		{
			throw new UmbrellaRemoteDataAccessConcurrencyException("There has been a concurrency error whilst putting the specified item.", exc);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { item.Id, endpointPath, sanitize, validationType }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem putting the specified item.", exc);
		}
	}

	/// <summary>
	/// PATCHes the specified resource on the remote server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TPatchResult">The type of the PATCH result.</typeparam>
	/// <param name="item">The item.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="sanitize">if set to <c>true</c> sanitizes the <paramref name="item"/> before saving.</param>
	/// <param name="validationType">The type of validation to be performed on the <paramref name="item"/> before saving.</param>
	/// <param name="afterItemSavedCallback">An optional callback to be invoked after the item has been successfully PATCHed.</param>
	/// <param name="endpointPath">The optional relative endpoint path to call. By default this is empty and is a RESTful API call.</param>
	/// <returns>The result of the remote operation.</returns>
	/// <exception cref="UmbrellaRemoteDataAccessConcurrencyException" />
	/// <exception cref="UmbrellaRemoteDataAccessException" />
	protected virtual async Task<(IHttpCallResult<TPatchResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> PatchAsync<TItem, TIdentifier, TPatchResult>(
		TItem item,
		CancellationToken cancellationToken = default,
		bool sanitize = true,
		ValidationType validationType = ValidationType.Shallow,
		Func<TItem, TPatchResult?, CancellationToken, Task>? afterItemSavedCallback = null,
		string endpointPath = "")
		where TItem : class, IKeyedItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(item, nameof(item));

		try
		{
			return item.Id.Equals(default!)
				? throw new Exception("The item being patched must have an Id.")
				: await SaveCoreAsync(HttpMethodExtras.Patch, endpointPath, item, cancellationToken, sanitize, validationType, afterItemSavedCallback).ConfigureAwait(false);
		}
		catch (UmbrellaHttpServiceConcurrencyException exc)
		{
			throw new UmbrellaRemoteDataAccessConcurrencyException("There has been a concurrency error whilst patching the specified item.", exc);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { item.Id, endpointPath }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem patching the specified item.", exc);
		}
	}

	/// <summary>
	/// DELETEs the specified resource from the remote server.
	/// </summary>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <param name="id">The identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="afterItemDeletedCallback">The optional operation to be invoked after the item has been deleted successfully.</param>
	/// <param name="endpointPath">The optional relative endpoint path to call. By default this is empty and is a RESTful API call.</param>
	/// <returns>The result of the remote operation.</returns>
	/// <exception cref="UmbrellaRemoteDataAccessConcurrencyException" />
	/// <exception cref="UmbrellaRemoteDataAccessException" />
	protected virtual async Task<IHttpCallResult> DeleteAsync<TIdentifier>(
		TIdentifier id,
		CancellationToken cancellationToken = default,
		Func<TIdentifier, CancellationToken, Task>? afterItemDeletedCallback = null,
		string endpointPath = "")
		where TIdentifier : IEquatable<TIdentifier>
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(id, nameof(id));

		try
		{
			var parameters = new Dictionary<string, string>
			{
				["id"] = id.ToString()
			};

			IHttpCallResult result = await RemoteService.DeleteAsync(ApiUrl, parameters, cancellationToken).ConfigureAwait(false);

			if (result.Success && afterItemDeletedCallback is not null)
				await afterItemDeletedCallback(id, cancellationToken).ConfigureAwait(false);

			return result;
		}
		catch (UmbrellaHttpServiceConcurrencyException exc)
		{
			throw new UmbrellaRemoteDataAccessConcurrencyException("There has been a concurrency error whilst deleting the specified item.", exc);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { id, endpointPath }))
		{
			throw new UmbrellaRemoteDataAccessException("There was a problem deleting the specified item.", exc);
		}
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
	/// By defaults this calls into <see cref="IUmbrellaValidator.ValidateItemAsync(object, ValidationType)" />.
	/// Overriding this method allows you to perform custom validation on the item.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="validationType">The type of validation to perform.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="Task"/> used to await completion of this operation.</returns>
	protected virtual Task<(bool isValid, IReadOnlyCollection<ValidationResult> results)> ValidateItemAsync(object item, ValidationType validationType, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.FromResult(Validator.ValidateItemAsync(item, validationType));
	}

	private async Task<(IHttpCallResult<TResult?> result, IReadOnlyCollection<ValidationResult> validationResults)> SaveCoreAsync<TItem, TResult>(
		HttpMethod method,
		string endpointPath,
		TItem item,
		CancellationToken cancellationToken,
		bool sanitize,
		ValidationType validationType,
		Func<TItem, TResult?, CancellationToken, Task>? afterItemSavedCallback)
	{
		if (item is not null)
		{
			if (sanitize)
				await SanitizeItemAsync(item, cancellationToken).ConfigureAwait(false);

			if (validationType != ValidationType.None)
			{
				var (isValid, results) = await ValidateItemAsync(item, validationType, cancellationToken).ConfigureAwait(false);

				if (!isValid)
					return (new HttpCallResult<TResult>(false), results);
			}
		}

		string url = ApiUrl + endpointPath;

		IHttpCallResult<TResult?> result = method switch
		{
			var _ when method == HttpMethod.Post => await RemoteService.PostAsync<TItem, TResult>(url, item, cancellationToken: cancellationToken).ConfigureAwait(false),
			var _ when method == HttpMethod.Put => await RemoteService.PutAsync<TItem, TResult>(url, item, cancellationToken: cancellationToken).ConfigureAwait(false),
			var _ when method == HttpMethodExtras.Patch => await RemoteService.PatchAsync<TItem, TResult>(url, item, cancellationToken: cancellationToken).ConfigureAwait(false),
			_ => throw new NotSupportedException()
		};

		if (result.Success && item is not null && afterItemSavedCallback is not null)
			await afterItemSavedCallback(item, result.Result, cancellationToken).ConfigureAwait(false);

		return (result, result.ProblemDetails?.ToValidationResults() ?? Array.Empty<ValidationResult>());
	}
}