using Umbrella.AspNetCore.Shared.Services.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services;

/// <summary>
/// This class is used to provide a no-op implementation of the <see cref="IHttpRequestAbortedService"/> interface.
/// When components run using Blazor WebAssembly, there is no request to abort so this class is used to provide a no-op implementation.
/// </summary>
internal class NoopHttpRequestAbortedService : IHttpRequestAbortedService
{
	public CancellationToken RequestAborted { get; }
}