using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	public interface IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSourceType>
		where TItem : IMultiRemoteItem<TIdentifier, TRemoteSourceType>
		where TRemoteSourceType : Enum
	{
		TRemoteSourceType RemoteSourceType { get; }

		Task<(HttpStatusCode statusCode, string message)> DeleteAllAsync(CancellationToken cancellationToken = default);
		Task<(HttpStatusCode statusCode, string message)> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default);
		Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, params SortExpression<TItem>[] sortExpressions);
		Task<(HttpStatusCode statusCode, string message, TItem result)> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
		Task<(HttpStatusCode statusCode, string message, TItem result)> SaveAsync(TItem item, CancellationToken cancellationToken = default);
		Task<(HttpStatusCode statusCode, string message, IReadOnlyCollection<TItem> results)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default);
		//Task<(HttpStatusCode statusCode, string message, bool exists)> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
		//Task<(HttpStatusCode statusCode, string message, int totalCount)> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}