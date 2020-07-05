using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	public interface IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource> : IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, RepoOptions>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, new()
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
	{
	}

	public interface IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions> : IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, new()
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
		where TRepoOptions : RepoOptions, new()
	{
	}

	public interface IGenericMultiRemoteRepository<TItem, TIdentifier, TRemoteSource, TRepoOptions, TService>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>, new()
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
		where TRepoOptions : RepoOptions, new()
		where TService : IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>
	{
		Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures)> DeleteAllAsync(CancellationToken cancellationToken = default, TRepoOptions options = null);
		
		
		Task<(bool success, string message)> DeleteAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken = default, TRepoOptions options = null);
		
		
		Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures, IReadOnlyCollection<TItem> results)> FindAllAsync(int pageNumber = 0, int pageSizeRequest = 20, CancellationToken cancellationToken = default, TRepoOptions options = null, params SortExpression<TItem>[] sortExpressions);
		
		
		Task<(bool success, string message, TItem result)> FindByIdAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken = default, TRepoOptions options = null);
		
		
		Task<SaveResult<TItem>> SaveAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null);
		
		
		Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures, IReadOnlyCollection<SaveResult<TItem>> saveResults)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default, TRepoOptions options = null);
		
		
		Task<(bool success, string message, bool? exists)> ExistsByIdAsync(TIdentifier id, TRemoteSource remoteSourceType, CancellationToken cancellationToken = default);
		
		
		Task<(bool success, IReadOnlyCollection<RemoteSourceFailure<TRemoteSource>> sourceFailures, int totalCount)> FindTotalCountAsync(CancellationToken cancellationToken = default);
	}
}