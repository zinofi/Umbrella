using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.DataAccess.Remote.Exceptions;
using Umbrella.Utilities;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.Remote
{
	// TODO: Move the ValidateAsync and SanitizeAsync methods into here?
	// Could be advantageous to double check the validation before sending to the server.
	// Sanitization is usually needed for this to work. Although the front-end stuff should already be doing this.
	// Unless we construct models manually that aren't bound to forms. Then this could prove useful to do. Hmmmm...
	// Think having an additional repo layer is overkill though. Needed for the multi-stuff but not here.

	/// <summary>
	/// Serves as the base class for HTTP Services.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <seealso cref="IGenericHttpService{TItem, TIdentifier}" />
	public abstract class GenericHttpService<TItem, TIdentifier> : IGenericHttpService<TItem, TIdentifier>
		where TItem : class, IRemoteItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		#region Protected Constants
		/// <summary>
		/// The unauthorized error message
		/// </summary>
		protected const string UnauthorizedErrorMessage = "You need to be logged in to perform the current action.";

		/// <summary>
		/// The forbidden error message
		/// </summary>
		protected const string ForbiddenErrorMessage = "You are not permitted to access the requested resource.";

		/// <summary>
		/// The server error message
		/// </summary>
		protected const string ServerErrorMessage = "An error has occurred on the remote server. Please try again.";

		/// <summary>
		/// The unknown error message
		/// </summary>
		protected const string UnknownErrorMessage = "An unknown error has occurred. Please try again.";
		#endregion

		#region Protected Static Properties		
		/// <summary>
		/// Gets the patch HTTP method.
		/// </summary>
		protected static HttpMethod PatchHttpMethod { get; } = new HttpMethod("PATCH");
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the API URL.
		/// </summary>
		protected abstract string ApiUrl { get; }

		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the HTTP client.
		/// </summary>
		protected HttpClient Client { get; }
		#endregion

		#region Constructors				
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericHttpService{TItem, TIdentifier}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="client">The client.</param>
		public GenericHttpService(
			ILogger logger,
			HttpClient client)
		{
			Log = logger;
			Client = client;
		}
		#endregion

		#region IGenericHttpService Methods
		/// <inheritdoc />
		public virtual async Task<(HttpStatusCode statusCode, string message, TItem result)> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			try
			{
				string url = $"{ApiUrl}?id={UrlEncoder.Default.Encode(id.ToString())}";

				HttpResponseMessage response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
						TItem item = await response.Content.ReadFromJsonAsync<TItem>().ConfigureAwait(false);
						await AfterItemLoadedAsync(item, cancellationToken).ConfigureAwait(false);
						return (HttpStatusCode.OK, null, item);
					case HttpStatusCode.Unauthorized:
						return (HttpStatusCode.Unauthorized, UnauthorizedErrorMessage, default);
					case HttpStatusCode.Forbidden:
						return (HttpStatusCode.Forbidden, ForbiddenErrorMessage, null);
					case HttpStatusCode.NotFound:
						return (HttpStatusCode.NotFound, null, null);
					case HttpStatusCode.InternalServerError:
						return (HttpStatusCode.InternalServerError, ServerErrorMessage, null);
					default:
						LogUnknownError(url, response.ReasonPhrase);
						return (response.StatusCode, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ApiUrl, id }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, IEnumerable<SortExpression<TItem>> sortExpressions = null, IEnumerable<FilterExpression<TItem>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				string url = ApiUrl;

				int sortExpressionsCount = sortExpressions?.Count() ?? 0;
				int filterExpressionsCount = sortExpressions?.Count() ?? 0;

				if ((pageNumber > 0 && pageSize > 0) || sortExpressionsCount > 0 || filterExpressionsCount > 0)
				{
					var urlBuilder = new StringBuilder(url);

					if (pageNumber > 0 && pageSize > 0)
						urlBuilder.Append($"?pageNumber={pageNumber}&pageSize={pageSize}");

					if (sortExpressionsCount > 0)
					{
						string delimiter = urlBuilder.Contains("?") ? "&" : "?";

						string sortExpressionsJson = JsonSerializer.Serialize(sortExpressions.ToSortExpressionDescriptors());
						string sortExpressionsJsonEncoded = UrlEncoder.Default.Encode(sortExpressionsJson);
						urlBuilder.Append($"{delimiter}sorters={sortExpressionsJsonEncoded}");
					}

					if (filterExpressionsCount > 0)
					{
						string delimiter = urlBuilder.Contains("?") ? "&" : "?";

						string filterExpressionsJson = JsonSerializer.Serialize(filterExpressions.ToFilterExpressionDescriptors());
						string filterExpressionsJsonEncoded = UrlEncoder.Default.Encode(filterExpressionsJson);
						urlBuilder.Append($"{delimiter}filters={filterExpressionsJsonEncoded}");
						urlBuilder.Append($"{delimiter}filterCombinator={filterExpressionCombinator}");
					}

					url = urlBuilder.ToString();
				}

				HttpResponseMessage response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
						List<TItem> items = await response.Content.ReadFromJsonAsync<List<TItem>>().ConfigureAwait(false);
						await AfterAllItemsLoadedAsync(items, cancellationToken).ConfigureAwait(false);
						return (HttpStatusCode.OK, null, items);
					case HttpStatusCode.Unauthorized:
						return (HttpStatusCode.Unauthorized, UnauthorizedErrorMessage, null);
					case HttpStatusCode.Forbidden:
						return (HttpStatusCode.Forbidden, ForbiddenErrorMessage, null);
					case HttpStatusCode.NotFound:
						return (HttpStatusCode.NotFound, null, null);
					case HttpStatusCode.InternalServerError:
						return (HttpStatusCode.InternalServerError, ServerErrorMessage, null);
					default:
						LogUnknownError(url, response.ReasonPhrase);
						return (response.StatusCode, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ApiUrl, pageNumber, pageSize, sortExpressions = sortExpressions.ToSortExpressionDescriptors(), filterExpressions = filterExpressions.ToFilterExpressionDescriptors(), filterExpressionCombinator }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(HttpStatusCode statusCode, string message, bool? exists)> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			try
			{
				string url = $"{ApiUrl}/Exists?id={UrlEncoder.Default.Encode(id.ToString())}";

				HttpResponseMessage response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
					case HttpStatusCode.NoContent:
						return (HttpStatusCode.NoContent, null, true);
					case HttpStatusCode.Unauthorized:
						return (HttpStatusCode.Unauthorized, UnauthorizedErrorMessage, null);
					case HttpStatusCode.Forbidden:
						return (HttpStatusCode.Forbidden, ForbiddenErrorMessage, null);
					case HttpStatusCode.NotFound:
						return (HttpStatusCode.NotFound, null, false);
					case HttpStatusCode.InternalServerError:
						return (HttpStatusCode.InternalServerError, ServerErrorMessage, null);
					default:
						LogUnknownError(url, response.ReasonPhrase);
						return (response.StatusCode, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ApiUrl, id }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public async Task<(HttpStatusCode statusCode, string message, int? totalCount)> FindTotalCountAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				string url = $"{ApiUrl}/Count";

				HttpResponseMessage response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
						int count = await response.Content.ReadFromJsonAsync<int>().ConfigureAwait(false);
						return (HttpStatusCode.OK, null, count);
					case HttpStatusCode.Unauthorized:
						return (HttpStatusCode.Unauthorized, UnauthorizedErrorMessage, null);
					case HttpStatusCode.Forbidden:
						return (HttpStatusCode.Forbidden, ForbiddenErrorMessage, null);
					case HttpStatusCode.InternalServerError:
						return (HttpStatusCode.InternalServerError, ServerErrorMessage, null);
					default:
						LogUnknownError(url, response.ReasonPhrase);
						return (response.StatusCode, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<(HttpStatusCode statusCode, string message, TItem result)> SaveAsync(TItem item, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));

			string url = ApiUrl;
			bool isNew = item.Id.Equals(default);

			try
			{
				HttpResponseMessage response = isNew
					? await Client.PostAsJsonAsync(url, item, cancellationToken).ConfigureAwait(false)
					: await Client.PutAsJsonAsync(url, item, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK when isNew:
					case HttpStatusCode.Created when isNew:
						TItem savedItem = await response.Content.ReadFromJsonAsync<TItem>().ConfigureAwait(false);
						await AfterItemSavedAsync(item, cancellationToken).ConfigureAwait(false);
						return (HttpStatusCode.Created, null, savedItem);
					case HttpStatusCode.OK when !isNew:
					case HttpStatusCode.NoContent when !isNew:
						return (HttpStatusCode.NoContent, null, null);
					case HttpStatusCode.Unauthorized:
						return (HttpStatusCode.Unauthorized, UnauthorizedErrorMessage, null);
					case HttpStatusCode.Forbidden:
						return (HttpStatusCode.Forbidden, ForbiddenErrorMessage, null);
					case HttpStatusCode.NotFound:
						return (HttpStatusCode.NotFound, null, null);
					case HttpStatusCode.Conflict:
						return (HttpStatusCode.Conflict, null, null);
					case HttpStatusCode.InternalServerError:
						return (HttpStatusCode.InternalServerError, ServerErrorMessage, null);
					case HttpStatusCode.BadRequest:
						return (HttpStatusCode.BadRequest, await response.Content.ReadAsStringAsync().ConfigureAwait(false), null);
					default:
						LogUnknownError(url, response.ReasonPhrase);
						return (response.StatusCode, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { item.Id, url, isNew }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		// TODO: Maybe don't go with this approach for errors and use the problem details response instead?
		// We can assume therefore that this result model will only ever be returned for a 200 or a 201. Yes???
		public class GenericHttpServiceResultModel<TItem, TResultType>
		{
			public string Message { get; set; }
			public TResultType ResultType { get; set; } // Could be useful with this though.
			// Have a result type for Succeeded, Failed, ConcurrencyFailure.
			// Have a new SubResultType that we can optionally use to provide additional information instead of just using
			// HttpStatus codes. We then don't have to return the status codes to method callers.
			// Couldn't hurt to have the code though surely??
			public string ConcurrencyStamp { get; set; } // Deffo need this. As we have the object references when saving, we can update this value.
			// Will require an IConcurrencyStamp interface though.
			// IEntity as well here?? Might as well as it means we can do stuff with the Id.
			// I suppose it's not really a risk saving the same model shape we get back to the server. And if we need other shapes, we can
			// jump in and customize stuff where needed. Hmmmm...
			// Need to think about the contract between client and server using status codes. Need to get it spot on
			// otherwise we'll have problems.
			// Could then make a base controller - call it ServiceController or something like that. That way
			// we can minimize the code we write.
			// We can have virtual methods to allow for customization and also use the Auth Handlers to plugin to stuff.
			// And then do other stuff in the generic repos?
		}

		// TODO: Need a wrapper like the above so we can easily see the result type. Have a default result type which does nothing where we don't need it.
		// The Find methods should be fine as they are.

		/// <inheritdoc />
		public virtual async Task<(HttpStatusCode statusCode, string message)> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			try
			{
				string url = $"{ApiUrl}?id={WebUtility.UrlEncode(id.ToString())}";

				HttpResponseMessage response = await Client.DeleteAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					// Ideally we prefer a 204 from the server to indicate success but 200 will suffice.
					case HttpStatusCode.OK:
					case HttpStatusCode.NoContent:
						await AfterItemDeletedAsync(id, cancellationToken).ConfigureAwait(false);
						return (HttpStatusCode.NoContent, null);
					case HttpStatusCode.Unauthorized:
						return (HttpStatusCode.Unauthorized, UnauthorizedErrorMessage);
					case HttpStatusCode.Forbidden:
						return (HttpStatusCode.Forbidden, ForbiddenErrorMessage);
					case HttpStatusCode.NotFound:
						return (HttpStatusCode.NotFound, null);
					case HttpStatusCode.InternalServerError:
						return (HttpStatusCode.InternalServerError, ServerErrorMessage);
					default:
						LogUnknownError(url, response.ReasonPhrase);
						return (response.StatusCode, UnknownErrorMessage);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { ApiUrl, id }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}
		#endregion

		#region Protected Methods		
		/// <summary>
		/// Override this in a derived type to perform an operation on the item after it has been loaded.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		protected virtual Task AfterItemLoadedAsync(TItem item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		///  Override this in a derived type to perform an operation on the items after they have been loaded.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		protected virtual async Task AfterAllItemsLoadedAsync(IEnumerable<TItem> items, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			IEnumerable<Task> tasks = items.Select(x => AfterItemLoadedAsync(x, cancellationToken));

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		/// <summary>
		/// Overriding this method allows you to perform any work after the item has been saved.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		protected virtual Task AfterItemSavedAsync(TItem item, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Overriding this method allows you to perform any work after all items have been saved.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		protected virtual async Task AfterAllItemsSavedAsync(IEnumerable<TItem> items, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			IEnumerable<Task> tasks = items.Select(x => AfterItemSavedAsync(x, cancellationToken));

			await Task.WhenAll(tasks).ConfigureAwait(false);
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

		/// <summary>
		/// Overriding this method allows you to perform any work after all items have been deleted.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the operation has completed.</returns>
		protected virtual Task AfterAllItemsDeletedAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Creates a service access exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>The exception instance.</returns>
		protected UmbrellaHttpServiceAccessException CreateServiceAccessException(Exception exception)
		{
			// If we already have an exception of the requested type just return it
			if (exception is UmbrellaHttpServiceAccessException)
				return exception as UmbrellaHttpServiceAccessException;

			return new UmbrellaHttpServiceAccessException(UnknownErrorMessage, exception);
		}

		/// <summary>
		/// Used to log an unknown error.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="errorMessage">The error message.</param>
		protected void LogUnknownError(string url, string errorMessage)
			=> Log.LogError($"There was a problem accessing the {url} endpoint. The error from the server was: {errorMessage}");
		#endregion
	}
}