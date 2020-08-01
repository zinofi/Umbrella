using System.Collections.Generic;
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
	}
}