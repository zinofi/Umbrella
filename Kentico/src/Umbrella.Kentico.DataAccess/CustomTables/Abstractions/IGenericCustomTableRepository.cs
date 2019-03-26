using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMS.CustomTables;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Sorting;

namespace Umbrella.Kentico.DataAccess.CustomTables.Abstractions
{
	// TODO: V3 - There is the future potential to unify these interfaces with the IGenericRepository interfaces. Slight issue with extra params
	// that are EF specific though so maybe keep them separate?
	public interface IGenericCustomTableRepository<TItem> : IGenericCustomTableRepository<TItem, RepoOptions>
		where TItem : CustomTableItem, new()
	{
	}

	public interface IGenericCustomTableRepository<TItem, TRepoOptions>
		where TItem : CustomTableItem, new()
		where TRepoOptions : RepoOptions, new()
	{
		Task DeleteAllAsync(CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions);
		Task DeleteAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions);
		Task<IReadOnlyCollection<TItem>> FindAllAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default, TRepoOptions options = null, RepoOptions[] childOptions = null, params SortExpression<TItem>[] sortExpressions);
		Task<TItem> FindByIdAsync(int id, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions);
		Task<int> FindTotalCountAsync(CancellationToken cancellationToken = default);
		Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
		Task<SaveResult<TItem>> SaveAsync(TItem item, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions);
		Task<(bool success, IReadOnlyCollection<SaveResult<TItem>> saveResults)> SaveAllAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default, TRepoOptions options = null, params RepoOptions[] childOptions);
	}
}