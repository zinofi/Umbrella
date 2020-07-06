using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	/// <summary>
	/// A generic HTTP Service.
	/// </summary>
	/// <typeparam name="TItem">The type of the item the service operates on.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier for the item.</typeparam>
	public interface IGenericHttpService<TItem, TIdentifier>
		where TItem : class, IRemoteItem<TIdentifier>
		where TIdentifier : IEquatable<TIdentifier>
	{
		/// <summary>
		/// Deletes all items from the service.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code and any message.</returns>
		Task<(HttpStatusCode statusCode, string message)> DeleteAllAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes the item with the specified <paramref name="id"/> from the service.
		/// </summary>
		/// <param name="id">The identifier of the item to delete.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code and any message.</returns>
		Task<(HttpStatusCode statusCode, string message)> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default);

		/// <summary>
		/// Finds all items using the specified parameters.
		/// </summary>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="sortExpressions">The sort expressions.</param>
		/// <param name="filterExpressions">The filter expressions.</param>
		/// <param name="filterExpressionCombinator">The filter expression combinator.</param>
		/// <returns>A tuple containing the HTTP status code, any message, and the results.</returns>
		Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, IEnumerable<SortExpression<TItem>> sortExpressions = null, IEnumerable<FilterExpression<TItem>> filterExpressions = null, FilterExpressionCombinator filterExpressionCombinator = FilterExpressionCombinator.Or);

		/// <summary>
		/// Finds the item with the specified <paramref name="id"/> from the service.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code, any message and the result.</returns>
		Task<(HttpStatusCode statusCode, string message, TItem result)> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);

		/// <summary>
		/// Saves the specified <paramref name="item"/> to the service.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code, any message and the saved item.</returns>
		Task<(HttpStatusCode statusCode, string message, TItem result)> SaveAsync(TItem item, CancellationToken cancellationToken = default);

		/// <summary>
		/// Saves all <paramref name="items"/> to the service.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code, any message and the saved items.</returns>
		Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default);

		/// <summary>
		/// Determines if the item with the specified <paramref name="id"/> exists on the service.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code, any message and a boolean value indicating if the item exists.</returns>
		Task<(HttpStatusCode statusCode, string message, bool? exists)> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);

		/// <summary>
		/// Finds the total count of all items in the service.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A tuple containing the HTTP status code, any message and the total count of all items.</returns>
		Task<(HttpStatusCode statusCode, string message, int totalCount)> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}