namespace Umbrella.AspNetCore.Blazor.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IBrowserEventAggregator : IAsyncDisposable
{
	/// <summary>
	/// Adds a subscription for the named event.
	/// </summary>
	/// <param name="eventName">Name of the event.</param>
	/// <param name="callback">The callback.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	ValueTask SubscribeAsync(string eventName, Func<ValueTask> callback, CancellationToken cancellationToken = default);
}