using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Sorting;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	public interface IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSourceType> : IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSourceType, RepoOptions>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSourceType>, new()
		where TRemoteSourceType : Enum
	{
	}

	public interface IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSourceType, TRepoOptions> : IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSourceType, TRepoOptions, IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSourceType>>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSourceType>, new()
		where TRemoteSourceType : Enum
		where TRepoOptions : RepoOptions, new()
	{
	}

	public interface IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSourceType, TRepoOptions, TService>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSourceType>, new()
		where TRemoteSourceType : Enum
		where TRepoOptions : RepoOptions, new()
		where TService : IGenericMultiHttpRestService<TItem, TIdentifier, TRemoteSourceType>
	{
		Task<(bool success, IReadOnlyCollection<string> messages)> DeleteAllAsync(CancellationToken cancellationToken = default, TRepoOptions options = null);
		Task<(bool success, string message)> DeleteAsync(TIdentifier id, TRemoteSourceType remoteSourceType, CancellationToken cancellationToken = default, TRepoOptions options = null);
		Task<IReadOnlyCollection<TItem>> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, TRepoOptions options = null, params SortExpression<TItem>[] sortExpressions);
		Task<TItem> FindByIdAsync(TIdentifier id, TRemoteSourceType remoteSourceType, CancellationToken cancellationToken = default, TRepoOptions options = null);
		Task<SaveResult<TItem>> SaveAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null);
		Task<(bool success, IReadOnlyCollection<SaveResult<TItem>> saveResults)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default, TRepoOptions options = null);
		Task<bool> ExistsByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
		Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}