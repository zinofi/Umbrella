using System;
using System.Threading.Tasks;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	public interface IAppUpdateMessageUtility
	{
		event Func<bool, string, Task> OnShowAsync;
		ValueTask ShowAsync(bool updateRequired, string message);
	}
}