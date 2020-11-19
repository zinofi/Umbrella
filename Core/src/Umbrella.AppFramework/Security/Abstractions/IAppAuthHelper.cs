using System.Security.Claims;
using System.Threading.Tasks;

namespace Umbrella.AppFramework.Security.Abstractions
{
	public interface IAppAuthHelper
	{
		Task<ClaimsPrincipal> GetCurrentClaimsPrincipalAsync(string token = null);
		Task LocalLogoutAsync(bool executeDefaultPostLogoutAction = true);
	}
}