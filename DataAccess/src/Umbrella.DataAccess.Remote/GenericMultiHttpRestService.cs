using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Umbrella.DataAccess.Remote.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.Remote
{
	// TODO: Abstract away the multi aspect of this to create a GenericHttpService.
	// We already have the base class. Move stuff into there.
	public abstract class GenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource> : GenericHttpService, IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSource>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>
		where TRemoteSource : Enum
	{
		#region Protected Properties
		protected abstract string ApiUrl { get; }
		public abstract TRemoteSource RemoteSourceType { get; }
		#endregion

		#region Constructors
		public GenericMultiHttpRestService(
			ILogger logger,
			HttpClient client)
			: base(logger, client)
		{
		}
		#endregion

		#region Public Methods
		public virtual async Task<(HttpStatusCode statusCode, string message, TItem result)> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			string url = $"{ApiUrl}?id={WebUtility.UrlEncode(id.ToString())}";

			try
			{
				HttpResponseMessage response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
						TItem item = await response.Content.ReadAsAsync<TItem>().ConfigureAwait(false);
						ApplyRemoteSourceType(item);
						return (HttpStatusCode.OK, null, item);
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
						return (HttpStatusCode.BadRequest, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, url }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		public virtual async Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, params SortExpression<TItem>[] sortExpressions)
		{
			cancellationToken.ThrowIfCancellationRequested();

			string url = ApiUrl;

			if ((pageNumber > 0 && pageSize > 0) || sortExpressions?.Length > 0)
			{
				var urlBuilder = new StringBuilder(url);

				if (pageNumber > 0 && pageSize > 0)
					urlBuilder.Append($"?pageNumber={pageNumber}&pageSize={pageSize}");

				if (sortExpressions?.Length > 0)
				{
					string delimiter = urlBuilder.Contains("?") ? "&" : "?";

					string sortExpressionsJson = JsonConvert.SerializeObject(sortExpressions.ToSortExpressionSerializables());
					string sortExpressionsJsonEncoded = WebUtility.UrlEncode(sortExpressionsJson);
					urlBuilder.Append(delimiter + "sortExpressions=" + sortExpressionsJsonEncoded);
				}

				url = urlBuilder.ToString();
			}

			try
			{
				HttpResponseMessage response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
						List<TItem> items = await response.Content.ReadAsAsync<List<TItem>>().ConfigureAwait(false);
						ApplyRemoteSourceType(items);
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
						return (HttpStatusCode.BadRequest, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { pageNumber, pageSize, url, sortExpressions = sortExpressions.ToSortExpressionSerializables() }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		//public virtual async Task<(HttpStatusCode statusCode, string message, bool exists)> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();

		//	try
		//	{
		//		var (statusCode, message, result) = await FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

		//		// TODO: Decide on a better way to handle this.
		//		return (statusCode, message, result != null);
		//	}
		//	catch (Exception exc) when (Log.WriteError(exc, new { id }, returnValue: true))
		//	{
		//		throw CreateServiceAccessException(exc);
		//	}
		//}

		//public Task<(HttpStatusCode statusCode, string message, int totalCount)> FindTotalCountAsync(CancellationToken cancellationToken = default)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();

		//	try
		//	{
		//		// TODO: Need to come up with a way of implementing this.
		//		throw new NotImplementedException("This functionality has not yet been implemented.");
		//	}
		//	catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
		//	{
		//		throw CreateServiceAccessException(exc);
		//	}
		//}

		public virtual async Task<(HttpStatusCode statusCode, string message, TItem result)> SaveAsync(TItem item, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(item, nameof(item));

			string url = ApiUrl;
			bool isNew = item.Id.Equals(default(TIdentifier));

			try
			{
				HttpResponseMessage response = isNew
					? await Client.PostAsJsonAsync(url, item, cancellationToken).ConfigureAwait(false)
					: await Client.PutAsJsonAsync(url, item, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK when isNew:
					case HttpStatusCode.Created when isNew:
						TItem savedItem = await response.Content.ReadAsAsync<TItem>().ConfigureAwait(false);
						ApplyRemoteSourceType(savedItem);
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
						return (HttpStatusCode.BadRequest, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { item.Id, item.Source, url, isNew }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		public virtual async Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(items, nameof(items));

			string url = ApiUrl;

			try
			{
				int itemsCount = items.Count();
				bool atLeastOneNewItem = items.Any(x => x.Id.Equals(default(TIdentifier)));

				HttpResponseMessage response = await Client.PostAsJsonAsync(url, items, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					case HttpStatusCode.OK when atLeastOneNewItem:
					case HttpStatusCode.Created when atLeastOneNewItem:
						List<TItem> results = null;
						if (itemsCount == 1)
						{
							TItem result = await response.Content.ReadAsAsync<TItem>().ConfigureAwait(false);
							results = new List<TItem> { result };
						}
						else
						{
							results = await response.Content.ReadAsAsync<List<TItem>>().ConfigureAwait(false);
						}
						ApplyRemoteSourceType(results);
						return (HttpStatusCode.Created, null, results);
					case HttpStatusCode.OK when !atLeastOneNewItem:
					case HttpStatusCode.NoContent when !atLeastOneNewItem:
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
						return (HttpStatusCode.BadRequest, UnknownErrorMessage, null);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { items = items.Where(x => x != null).Select(x => (x.Id, x.Source)), url }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		public virtual async Task<(HttpStatusCode statusCode, string message)> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(id, nameof(id));

			string url = $"{ApiUrl}?id={WebUtility.UrlEncode(id.ToString())}";

			try
			{
				HttpResponseMessage response = await Client.DeleteAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					// Ideally we prefer a 204 from the server to indicate success but 200 will suffice.
					case HttpStatusCode.OK:
					case HttpStatusCode.NoContent:
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
						return (HttpStatusCode.BadRequest, UnknownErrorMessage);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { id, url }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		public virtual async Task<(HttpStatusCode statusCode, string message)> DeleteAllAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			string url = ApiUrl;

			try
			{
				HttpResponseMessage response = await Client.DeleteAsync(url, cancellationToken).ConfigureAwait(false);

				switch (response.StatusCode)
				{
					// Ideally we prefer a 204 from the server to indicate success but 200 will suffice.
					case HttpStatusCode.OK:
					case HttpStatusCode.NoContent:
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
						return (HttpStatusCode.BadRequest, UnknownErrorMessage);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}
		#endregion

		#region Protected Methods
		protected virtual void ApplyRemoteSourceType(TItem item) => item.Source = RemoteSourceType;
		protected virtual void ApplyRemoteSourceType(List<TItem> items) => items.ForEach(x => x.Source = RemoteSourceType);
		protected virtual object CreatePostModel(TItem source) => source;
		protected virtual object CreatePutModel(TItem source) => source;
		#endregion
	}
}