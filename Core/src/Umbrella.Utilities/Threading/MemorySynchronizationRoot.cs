using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.Utilities.Threading;

/// <summary>
/// An implementation of <see cref="ISynchronizationRoot"/> that uses a <see cref="SemaphoreSlim"/> internally to perform synchronization.
/// Usage:
/// <code>
/// using (await syncRoot.WaitAsync(ct))
/// {
///     // Critical section
/// }
/// </code>
/// Dispose MUST only occur after a successful awaited call to <see cref="WaitAsync"/>; disposing before the wait completes
/// (i.e. not awaiting) is misuse and will not release the semaphore.
/// </summary>
public sealed class MemorySynchronizationRoot : ISynchronizationRoot
{
	private readonly SemaphoreSlim _semaphoreSlim;
	private bool _acquired;
	private bool _disposed;

	internal MemorySynchronizationRoot(SemaphoreSlim semaphoreSlim)
	{
		_semaphoreSlim = semaphoreSlim;
	}

	internal async ValueTask<ISynchronizationRoot> WaitAsync(CancellationToken cancellationToken = default)
	{
		// If this instance is reused incorrectly, enforce single-use per acquisition cycle.
		if (_acquired)
			throw new InvalidOperationException("This synchronization root has already been acquired. Dispose it before reusing.");

		await _semaphoreSlim
			.WaitAsync(cancellationToken)
			.ConfigureAwait(false);

		_acquired = true;

		return this;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (_disposed)
			return;

		DisposeCore();
		_disposed = true;
	}

	/// <inheritdoc />
	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

	private void DisposeCore()
	{
		// Only release if we actually acquired (i.e. WaitAsync completed successfully).
		if (_acquired)
		{
			_ = _semaphoreSlim.Release();
			_acquired = false;
		}
	}
}