using System.Security.Claims;
using Umbrella.AspNetCore.Shared.Services.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services;

/// <summary>
/// This class is used to provide a no-op implementation of the <see cref="IHttpContextService"/> interface.
/// When components run using Blazor WebAssembly, there is no context so this class is used to provide a no-op implementation.
/// </summary>
internal sealed class NoopHttpContextService : IHttpContextService
{
	public CancellationToken RequestAborted { get; }
	public ClaimsPrincipal? User { get; }
}