using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Http.Exceptions;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// An opinionated generic HTTP service used to query remote endpoints that follow the same conventions.
	/// </summary>
	public class GenericHttpService : IGenericHttpService
	{
		/// <summary>
		/// The unauthorized error message
		/// </summary>
		protected const string DefaultUnauthorizedErrorMessage = "You need to be logged in to perform the current action.";

		/// <summary>
		/// The forbidden error message
		/// </summary>
		protected const string DefaultForbiddenErrorMessage = "You are not permitted to access the requested resource.";

		/// <summary>
		/// The server error message
		/// </summary>
		protected const string DefaultServerErrorMessage = "An error has occurred on the remote server. Please try again.";

		/// <summary>
		/// The unknown error message
		/// </summary>
		protected const string DefaultUnknownErrorMessage = "An unknown error has occurred. Please try again.";

		/// <summary>
		/// Gets the patch HTTP method.
		/// </summary>
		protected static HttpMethod PatchHttpMethod { get; } = new HttpMethod("PATCH");

		/// <summary>
		/// Gets the logger
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the HTTP client.
		/// </summary>
		protected HttpClient Client { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericHttpService"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="client">The client.</param>
		public GenericHttpService(
			ILogger<GenericHttpService> logger,
			HttpClient client)
		{
			Logger = logger;
			Client = client;
		}

		/// <inheritdoc />
		public virtual async Task<HttpCallResult<TResult>> GetAsync<TResult>(string url, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));

			try
			{
				string targetUrl = GetUrlWithParmeters(url, parameters);

				HttpResponseMessage response = await Client.GetAsync(targetUrl, cancellationToken).ConfigureAwait(false);

				if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
				{
					TResult result = await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken).ConfigureAwait(false);

					return new HttpCallResult<TResult>(true, result: result);
				}

				return new HttpCallResult<TResult>(false, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<HttpCallResult<TResult>> PostAsync<TItem, TResult>(string url, TItem item, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				string targetUrl = GetUrlWithParmeters(url, parameters);

				HttpResponseMessage response = await Client.PostAsJsonAsync(targetUrl, item, cancellationToken).ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
						return new HttpCallResult<TResult>(true, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));

					if (response.Content.Headers.ContentLength > 0)
					{
						TResult result = await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken).ConfigureAwait(false);

						return new HttpCallResult<TResult>(true, result: result);
					}
				}

				return new HttpCallResult<TResult>(false, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<HttpCallResult<TResult>> PutAsync<TItem, TResult>(string url, TItem item, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				string targetUrl = GetUrlWithParmeters(url, parameters);

				HttpResponseMessage response = await Client.PutAsJsonAsync(targetUrl, item, cancellationToken).ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
						return new HttpCallResult<TResult>(true, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));

					if (response.Content.Headers.ContentLength > 0)
					{
						TResult result = await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken).ConfigureAwait(false);

						return new HttpCallResult<TResult>(true, result: result);
					}
				}

				return new HttpCallResult<TResult>(false, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<HttpCallResult<TResult>> PatchAsync<TItem, TResult>(string url, TItem item, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				string targetUrl = GetUrlWithParmeters(url, parameters);

				var request = new HttpRequestMessage(PatchHttpMethod, targetUrl)
				{
					Content = JsonContent.Create(item)
				};

				HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
						return new HttpCallResult<TResult>(true, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));

					if (response.Content.Headers.ContentLength > 0)
					{
						TResult result = await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken).ConfigureAwait(false);

						return new HttpCallResult<TResult>(true, result: result);
					}
				}

				return new HttpCallResult<TResult>(false, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<HttpCallResult> DeleteAsync(string url, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));

			try
			{
				string targetUrl = GetUrlWithParmeters(url, parameters);

				HttpResponseMessage response = await Client.DeleteAsync(targetUrl, cancellationToken).ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
					return new HttpCallResult(true);

				return new HttpCallResult(false, await GetProblemDetails(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <summary>
		/// Gets the URL with parmeters appended as querystring values.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The URL.</returns>
		protected virtual string GetUrlWithParmeters(string url, IDictionary<string, string> parameters)
			=> parameters?.Count > 0 ? QueryHelpers.AddQueryString(url, parameters) : url;

		/// <summary>
		/// Gets the problem details from the response if available.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="HttpProblemDetails"/>.</returns>
		protected virtual async Task<HttpProblemDetails> GetProblemDetails(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			string defaultMessage = response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => DefaultUnauthorizedErrorMessage,
				HttpStatusCode.Forbidden => DefaultForbiddenErrorMessage,
				HttpStatusCode.InternalServerError => DefaultServerErrorMessage,
				_ => DefaultUnknownErrorMessage
			};

			HttpContentHeaders headers = response.Content.Headers;

			if (headers.Count() == 0 || headers.ContentType?.MediaType?.Equals("application/problem+json", StringComparison.OrdinalIgnoreCase) != true)
				return new HttpProblemDetails { Title = "Error", Detail = defaultMessage };

			return await response.Content.ReadFromJsonAsync<HttpProblemDetails>(cancellationToken: cancellationToken).ConfigureAwait(false);
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

			return new UmbrellaHttpServiceAccessException(DefaultUnknownErrorMessage, exception);
		}

		/// <summary>
		/// Used to log an unknown error.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="errorMessage">The error message.</param>
		protected void LogUnknownError(string url, string errorMessage)
			=> Logger.LogError($"There was a problem accessing the {url} endpoint. The error from the server was: {errorMessage}");
	}
}