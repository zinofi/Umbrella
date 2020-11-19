using System;
using System.Threading.Tasks;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	public interface IAppUpdateMessageUtility
	{
		event Func<bool, string, Task> OnShowAsync;
		Task ShowAsync(bool updateRequired, string message);
	}
}