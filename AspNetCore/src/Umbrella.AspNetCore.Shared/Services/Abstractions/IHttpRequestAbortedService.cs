namespace Umbrella.AspNetCore.Shared.Services.Abstractions;

/// <summary>
/// An interface that can be implemented by Blazor components to provide access to the <see cref="CancellationToken"/> that will be signalled when the request is aborted.
/// </summary>
public interface IHttpRequestAbortedService
{
	/// <summary>
	/// Gets the <see cref="CancellationToken"/> that will be signalled when the request is aborted.
	/// </summary>
	CancellationToken RequestAborted { get; }
}