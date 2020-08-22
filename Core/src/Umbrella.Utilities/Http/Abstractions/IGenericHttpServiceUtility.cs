using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.Utilities.Http.Abstractions
{
	/// <summary>
	/// A utility class used to support querying HTTP services.A utility class used to support querying HTTP services.
	/// </summary>
	public interface IGenericHttpServiceUtility
	{
		/// <summary>
		/// Creates the search query parameters.
		/// </summary>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="sorters">The sorters.</param>
		/// <param name="filters">The filters.</param>
		/// <param name="filterCombinator">The filter combinator.</param>
		/// <returns>The query parameters.</returns>
		IDictionary<string, string> CreateSearchQueryParameters(int pageNumber = 0, int pageSize = 20, IEnumerable<SortExpressionDescriptor> sorters = null, IEnumerable<FilterExpressionDescriptor> filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.Or);

		/// <summary>
		/// Creates the search query parameters.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="sorters">The sorters.</param>
		/// <param name="filters">The filters.</param>
		/// <param name="filterCombinator">The filter combinator.</param>
		/// <returns>The query parameters.</returns>
		IDictionary<string, string> CreateSearchQueryParameters<TItem>(int pageNumber = 0, int pageSize = 20, IEnumerable<SortExpression<TItem>> sorters = null, IEnumerable<FilterExpression<TItem>> filters = null, FilterExpressionCombinator filterCombinator = FilterExpressionCombinator.Or);

		/// <summary>
		/// Gets the problem details from the response if available.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="HttpProblemDetails"/>.</returns>
		Task<HttpProblemDetails> GetProblemDetailsAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the URL with parmeters appended as querystring values.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The URL.</returns>
		string GetUrlWithParmeters(string url, IDictionary<string, string> parameters);
	}
}