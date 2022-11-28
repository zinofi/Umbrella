using System.Security.Claims;

namespace Umbrella.AspNetCore.Blazor.Security.Abstractions;

public interface IJwtAuthenticationStateProvider
{
	event EventHandler? AuthenticatedStateHasChanged;
	Task MarkUserAsAuthenticatedAsync(ClaimsPrincipal principal);
	Task MarkUserAsLoggedOutAsync();
}