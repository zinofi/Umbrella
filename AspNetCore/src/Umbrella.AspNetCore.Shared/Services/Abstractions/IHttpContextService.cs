using System.Security.Claims;

namespace Umbrella.AspNetCore.Shared.Services.Abstractions;

/// <summary>
/// An interface that can be implemented by Blazor components to provide access to certain HTTP context related data when running server-side.
/// </summary>
public interface IHttpContextService
{
	/// <summary>
	/// Gets the <see cref="CancellationToken"/> that will be signalled when the request is aborted.
	/// </summary>
	CancellationToken RequestAborted { get; }

	/// <summary>
	/// Gets the <see cref="ClaimsPrincipal"/> for the current user from the HTTP context.
	/// </summary>
	ClaimsPrincipal? User { get; }
}