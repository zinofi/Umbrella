using System.Threading.Tasks;

namespace Umbrella.AppFramework.Security.Abstractions
{
	public interface IAppAuthTokenStorageService
	{
		Task<string?> GetTokenAsync();
		Task SetTokenAsync(string? token);
		Task<string> GetClientIdAsync();
	}
}