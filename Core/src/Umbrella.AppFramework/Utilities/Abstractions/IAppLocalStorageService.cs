using System.Threading.Tasks;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	public interface IAppLocalStorageService
	{
		ValueTask<string> GetAsync(string key);
		ValueTask SetAsync(string key, string value);
		ValueTask RemoveAsync(string key);
	}
}