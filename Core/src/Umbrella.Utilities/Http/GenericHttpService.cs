// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Net.Http;
using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Http.Constants;
using Umbrella.Utilities.Http.Exceptions;

namespace Umbrella.Utilities.Http;

/// <summary>
/// An opinionated generic HTTP service used to query remote endpoints that follow the same conventions.
/// </summary>
public class GenericHttpService : IGenericHttpService
{
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
	public virtual async Task<IHttpCallResult<TResult?>> GetAsync<TResult>(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			HttpResponseMessage response = await Client.GetAsync(targetUrl, cancellationToken).ConfigureAwait(false);

			var (processed, result) = await HttpServiceUtility.ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

			var retVal = processed
				? result
				: new HttpCallResult<TResult?>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult> PostAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		=> await PostAsync<TItem, object?>(url, item, parameters, cancellationToken).ConfigureAwait(false);

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult<TResult?>> PostAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));
		Guard.IsNotNull(item, nameof(item));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			string json = UmbrellaStatics.SerializeJson(item!);
			using var request = new HttpRequestMessage(HttpMethod.Post, targetUrl)
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

			var (processed, result) = await HttpServiceUtility.ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

			var retVal = processed
				? result
				: new HttpCallResult<TResult?>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult> PutAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		=> await PutAsync<TItem, object?>(url, item, parameters, cancellationToken).ConfigureAwait(false);

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult<TResult?>> PutAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));
		Guard.IsNotNull(item, nameof(item));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			string json = UmbrellaStatics.SerializeJson(item!);
			using var request = new HttpRequestMessage(HttpMethod.Put, targetUrl)
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

			var (processed, result) = await HttpServiceUtility.ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

			var retVal = processed
				? result
				: new HttpCallResult<TResult?>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult> PatchAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		=> await PatchAsync<TItem, object?>(url, item, parameters, cancellationToken).ConfigureAwait(false);

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult<TResult?>> PatchAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));
		Guard.IsNotNull(item, nameof(item));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			string json = UmbrellaStatics.SerializeJson(item!);
			using var request = new HttpRequestMessage(HttpMethodExtras.Patch, targetUrl)
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

			var (processed, result) = await HttpServiceUtility.ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

			var retVal = processed
				? result
				: new HttpCallResult<TResult?>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult> PatchAsync(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			using var request = new HttpRequestMessage(HttpMethodExtras.Patch, targetUrl);

			HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

			var (processed, result) = await HttpServiceUtility.ProcessResponseAsync(response, cancellationToken).ConfigureAwait(false);

			var retVal = processed
				? result
				: new HttpCallResult(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult<TResult?>> PatchAsync<TResult>(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			using var request = new HttpRequestMessage(HttpMethodExtras.Patch, targetUrl);

			HttpResponseMessage response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

			var (processed, result) = await HttpServiceUtility.ProcessResponseAsync<TResult>(response, cancellationToken).ConfigureAwait(false);

			var retVal = processed
				? result
				: new HttpCallResult<TResult?>(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <inheritdoc />
	public virtual async Task<IHttpCallResult> DeleteAsync(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(url, nameof(url));

		try
		{
			string targetUrl = HttpServiceUtility.GetUrlWithParmeters(url, parameters);

			HttpResponseMessage response = await Client.DeleteAsync(targetUrl, cancellationToken).ConfigureAwait(false);

			var retVal = response.IsSuccessStatusCode
				? new HttpCallResult(true)
				: new HttpCallResult(false, await HttpServiceUtility.GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false));

			ThrowIfConcurrencyStampMismatchResponse(retVal);

			return retVal;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { url, parameters }) && exc is not UmbrellaHttpServiceConcurrencyException)
		{
			throw CreateServiceAccessException(exc);
		}
	}

	/// <summary>
	/// Throws a <see cref="UmbrellaHttpServiceConcurrencyException"/> if the server returns
	/// a <see cref="HttpProblemCodes.ConcurrencyStampMismatch"/> code in the problem details response.
	/// </summary>
	/// <param name="result">The result of the Http server call.</param>
	protected static void ThrowIfConcurrencyStampMismatchResponse(IHttpCallResult result)
	{
		Guard.IsNotNull(result);

		if (result.ProblemDetails?.Code?.Equals(HttpProblemCodes.ConcurrencyStampMismatch, StringComparison.OrdinalIgnoreCase) == true)
			throw new UmbrellaHttpServiceConcurrencyException("The server has reported a concurrency stamp mismatch.");
	}

	/// <summary>
	/// Creates a service access exception.
	/// </summary>
	/// <param name="exception">The exception.</param>
	/// <returns>The exception instance.</returns>
	protected static UmbrellaHttpServiceAccessException CreateServiceAccessException(Exception exception)
		=> exception is UmbrellaHttpServiceAccessException serviceAccessException
			? serviceAccessException
			: new UmbrellaHttpServiceAccessException(HttpServiceMessages.DefaultUnknownErrorMessage, exception);

	/// <summary>
	/// Used to log an unknown error.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <param name="errorMessage">The error message.</param>
	protected void LogUnknownError(string url, string errorMessage)
		=> Logger.WriteError(state: new { url, errorMessage });
}