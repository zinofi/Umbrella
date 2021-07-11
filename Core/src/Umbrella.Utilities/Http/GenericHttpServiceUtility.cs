using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Http.Constants;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// A utility class used to support querying HTTP services.
	/// </summary>
	public class GenericHttpServiceUtility : IGenericHttpServiceUtility
	{
		private readonly ILogger<GenericHttpServiceUtility> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericHttpServiceUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public GenericHttpServiceUtility(ILogger<GenericHttpServiceUtility> logger)
		{
			_logger = logger;
		}

		/// <inheritdoc />
		public IDictionary<string, string> CreateSearchQueryParameters<TItem>(int pageNumber = 0, int pageSize = 20, IEnumerable<SortExpression<TItem>>? sorters = null, IEnumerable<FilterExpression<TItem>>? filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.Or)
		{
			try
			{
				return CreateSearchQueryParameters(pageNumber, pageSize, sorters?.ToSortExpressionDescriptors(), filters?.ToFilterExpressionDescriptors(), filterCombinator);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { pageNumber, pageSize, sorters = sorters?.ToSortExpressionDescriptors(), filters = filters?.ToFilterExpressionDescriptors(), filterCombinator }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem creating the query parameters from the search parameters.", exc);
			}
		}

		/// <inheritdoc />
		public IDictionary<string, string> CreateSearchQueryParameters(int pageNumber = 0, int pageSize = 20, IEnumerable<SortExpressionDescriptor>? sorters = null, IEnumerable<FilterExpressionDescriptor>? filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.Or)
		{
			try
			{
				var parameters = new Dictionary<string, string>();

				int sorterCount = sorters?.Count() ?? 0;
				int filterCount = filters?.Count() ?? 0;

				if ((pageNumber > 0 && pageSize > 0) || sorterCount > 0 || filterCount > 0)
				{
					if (pageNumber > 0 && pageSize > 0)
					{
						parameters.Add("pageNumber", pageNumber.ToString());
						parameters.Add("pageSize", pageSize.ToString());
					}

					if (sorterCount > 0)
						parameters.Add("sorters", UmbrellaStatics.SerializeJson(sorters!));

					if (filterCount > 0)
					{
						parameters.Add("filters", UmbrellaStatics.SerializeJson(filters!));
						parameters.Add("filterCombinator", filterCombinator.ToString());
					}
				}

				return parameters;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { pageNumber, pageSize, sorters, filters, filterCombinator }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem creating the query parameters from the search parameters.", exc);
			}
		}

		/// <inheritdoc />
		public string GetUrlWithParmeters(string url, IEnumerable<KeyValuePair<string, string>>? parameters)
		{
			try
			{
				return parameters?.Count() > 0 ? QueryHelpers.AddQueryString(url, parameters) : url;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { url, parameters }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem creating the url with the specified parameters appended.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<(bool processed, HttpCallResult<TResult> result)> ProcessResponseAsync<TResult>(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			try
			{
				if (response.IsSuccessStatusCode)
				{
					// NB: The ProcessResponseAsync below was added after this method. Need to keep this check here to avoid breaking existing apps.
					if (response.StatusCode == HttpStatusCode.NoContent)
						return (true, new HttpCallResult<TResult>(true, await GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false)));

					if (response.Content.Headers.ContentLength > 0)
					{
						TResult result = response.Content.Headers.ContentType.MediaType switch
						{
							"text/plain" when typeof(TResult) == typeof(string) => (TResult)(object)(await response.Content.ReadAsStringAsync().ConfigureAwait(false)),
							"application/json" => UmbrellaStatics.DeserializeJson<TResult>(await response.Content.ReadAsStringAsync()),
							"text/html" => throw new NotSupportedException("HTML responses are not supported and should not be returned by API endpoints. This might indicate an incorrect API url is being used which doesn't exist on the server."),
							_ => throw new NotImplementedException()
						};

						return (true, new HttpCallResult<TResult>(true, result: result));
					}
				}

				return default;
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem processing the response.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<(bool processed, HttpCallResult result)> ProcessResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			try
			{
				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
						return (true, new HttpCallResult(true, await GetProblemDetailsAsync(response, cancellationToken).ConfigureAwait(false)));

					throw new NotSupportedException("Only 204 NoContent responses are supported when there is no result from the endpoint.");
				}

				return default;
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem processing the response.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<HttpProblemDetails> GetProblemDetailsAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
		{
			try
			{
				string defaultMessage = response.StatusCode switch
				{
					HttpStatusCode.Unauthorized => HttpServiceMessages.DefaultUnauthorizedErrorMessage,
					HttpStatusCode.Forbidden => HttpServiceMessages.DefaultForbiddenErrorMessage,
					HttpStatusCode.InternalServerError => HttpServiceMessages.DefaultServerErrorMessage,
					_ => HttpServiceMessages.DefaultUnknownErrorMessage
				};

				HttpContentHeaders headers = response.Content.Headers;

				if (headers.Count() == 0 || headers.ContentType?.MediaType?.Equals("application/problem+json", StringComparison.OrdinalIgnoreCase) != true)
					return new HttpProblemDetails { Title = "Error", Detail = defaultMessage };

				string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				return UmbrellaStatics.DeserializeJson<HttpProblemDetails>(json);
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem getting the problemdetails response.", exc);
			}
		}
	}
}