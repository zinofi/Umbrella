using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
// TODO v4: using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Http.Constants;
using Umbrella.Utilities.Http.Exceptions;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// An opinionated generic HTTP service used to query remote endpoints that follow the same conventions.
	/// </summary>
	public class GenericHttpService : IGenericHttpService
	{
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
		/// Gets the HTTP service utility.
		/// </summary>
		protected IGenericHttpServiceUtility HttpServiceUtility { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericHttpService"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="client">The client.</param>
		/// <param name="httpServiceUtility">The HTTP service utility.</param>
		public GenericHttpService(
			ILogger<GenericHttpService> logger,
			HttpClient client,
			IGenericHttpServiceUtility httpServiceUtility)
		{
			Logger = logger;
			Client = client;
			HttpServiceUtility = httpServiceUtility;
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<TResult>> GetAsync<TResult>(string url, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));

			try
			{
				string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

				HttpResponseMessage response = await Client.GetAsync(targetUrl, cancellationToken).ConfigureAwait(false);

				var (processed, result) = await ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

				return processed
					? result
					: new HttpCallResult<TResult>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<TResult>> PostAsync<TItem, TResult>(string url, TItem item, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

				// TODO v4: HttpResponseMessage response = await Client.PostAsJsonAsync(targetUrl, item, cancellationToken).ConfigureAwait(false);

				string json = UmbrellaStatics.SerializeJson(item);
				var request = new HttpRequestMessage(HttpMethod.Post, targetUrl)
				{
					Content = new StringContent(json, Encoding.UTF8, "application/json")
				};

				HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

				var (processed, result) = await ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

				return processed
					? result
					: new HttpCallResult<TResult>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<TResult>> PutAsync<TItem, TResult>(string url, TItem item, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

				// TODO v4: HttpResponseMessage response = await Client.PutAsJsonAsync(targetUrl, item, cancellationToken).ConfigureAwait(false);

				string json = UmbrellaStatics.SerializeJson(item);
				var request = new HttpRequestMessage(HttpMethod.Put, targetUrl)
				{
					Content = new StringContent(json, Encoding.UTF8, "application/json")
				};

				HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

				var (processed, result) = await ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

				return processed
					? result
					: new HttpCallResult<TResult>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult<TResult>> PatchAsync<TItem, TResult>(string url, TItem item, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));
			Guard.ArgumentNotNull(item, nameof(item));

			try
			{
				string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

				string json = UmbrellaStatics.SerializeJson(item);
				var request = new HttpRequestMessage(PatchHttpMethod, targetUrl)
				{
					Content = new StringContent(json, Encoding.UTF8, "application/json")
				};

				// TODO v4
				//var request = new HttpRequestMessage(PatchHttpMethod, targetUrl)
				//{
				//	Content = JsonContent.Create(item)
				//};

				HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

				var (processed, result) = await ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

				return processed
					? result
					: new HttpCallResult<TResult>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IHttpCallResult> DeleteAsync(string url, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));

			try
			{
				string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

				HttpResponseMessage response = await Client.DeleteAsync(targetUrl, cancellationToken).ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
					return new HttpCallResult(true);

				return new HttpCallResult(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw CreateServiceAccessException(exc);
			}
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

			return new UmbrellaHttpServiceAccessException(HttpServiceMessages.DefaultUnknownErrorMessage, exception);
		}

		/// <summary>
		/// Used to log an unknown error.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="errorMessage">The error message.</param>
		protected void LogUnknownError(string url, string errorMessage)
			=> Logger.LogError($"There was a problem accessing the {url} endpoint. The error from the server was: {errorMessage}");

		/// <summary>
		/// Processes the response.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="response">The response.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containg a the result.</returns>
		protected async Task<(bool processed, HttpCallResult<TResult> result)> ProcessResponseAsync<TResult>(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			if (response.IsSuccessStatusCode)
			{
				if (response.StatusCode == HttpStatusCode.NoContent)
					return (true, new HttpCallResult<TResult>(true, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false)));

				if (response.Content.Headers.ContentLength > 0)
				{
					TResult result = response.Content.Headers.ContentType.MediaType switch
					{
						"text/plain" when typeof(TResult) == typeof(string) => (TResult)(object)(await response.Content.ReadAsStringAsync().ConfigureAwait(false)),
						// TODO v4: "application/json" => await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken).ConfigureAwait(false),
						"application/json" => UmbrellaStatics.DeserializeJson<TResult>(await response.Content.ReadAsStringAsync()),
						"text/html" => throw new NotSupportedException("HTML responses are not supported and should not be returned by API endpoints. This might indicate an incorrect API url is being used which doesn't exist on the server."),
						_ => throw new NotImplementedException()
					};

					return (true, new HttpCallResult<TResult>(true, result: result));
				}
			}

			return default;
		}
	}
}