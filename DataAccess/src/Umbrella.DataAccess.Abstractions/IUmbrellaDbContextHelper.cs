using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions
{
	public interface IUmbrellaDbContextHelper
	{
		Task ExecutePostSaveChangesActionsAsync(CancellationToken cancellationToken = default);
		void RegisterPostSaveChangesAction(object entity, Func<CancellationToken, Task> wrappedAction);
		int SaveChanges(Func<int> baseSaveChanges);
		Task<int> SaveChangesAsync(Func<CancellationToken, Task<int>> baseSaveChangesAsync, CancellationToken cancellationToken = default);
	}
}