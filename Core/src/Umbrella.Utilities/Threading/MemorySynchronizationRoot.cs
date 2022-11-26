using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.Utilities.Threading;

/// <summary>
/// An implementation of <see cref="ISynchronizationRoot"/> that used a <see cref="SemaphoreSlim"/> internally to perform synchronization.
/// </summary>
/// <seealso cref="ISynchronizationRoot" />
public sealed class MemorySynchronizationRoot : ISynchronizationRoot
{
	private readonly SemaphoreSlim _semaphoreSlim;
	private Task? _currentTask;

	internal MemorySynchronizationRoot(SemaphoreSlim semaphoreSlim)
	{
		_semaphoreSlim = semaphoreSlim;
	}

	internal async ValueTask<ISynchronizationRoot> WaitAsync(CancellationToken cancellationToken = default)
	{
		_currentTask = _semaphoreSlim.WaitAsync(cancellationToken);
		await _currentTask.ConfigureAwait(false);

		return this;
	}

	/// <inheritdoc />
	public void Dispose() => DisposeCore();

	/// <inheritdoc />
	public ValueTask DisposeAsync()
	{
		DisposeCore();

		return default;
	}

	private void DisposeCore()
	{
		// Checking the Semaphore wasn't cancelled here to ensure we don't release when we shouldn't
		// because the Semaphore will already have been released internally.
		if (_currentTask?.Status == TaskStatus.RanToCompletion)
			_semaphoreSlim.Release();
	}
}