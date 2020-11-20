using System.Threading.Tasks;

namespace Umbrella.AppFramework.Security.Abstractions
{
	public interface IAppAuthTokenStorageService
	{
		ValueTask<string?> GetTokenAsync();
		ValueTask SetTokenAsync(string? token);
		ValueTask<string> GetClientIdAsync();
	}
}