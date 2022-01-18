using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Threading.Abstractions
{
	/// <summary>
	/// A synchoronization manager which can be used to ensure exclusive execution of a section of code using granular locking in both
	/// sync and async methods.
	/// </summary>
	public interface ISynchronizationManager
	{
		/// <summary>
		/// Gets the synchronization root and waits for it to be released.
		/// </summary>
		/// <param name="type">A target type used to create a the granular synchronization key.</param>
		/// <param name="key">A key used in conjunction with the specified <paramref name="type"/> to create the synchronization key.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The created <see cref="ISynchronizationRoot"/>.</returns>
		/// <remarks>
		/// Returned <see cref="ISynchronizationRoot"/> instances need to be disposed in order for the acquired lock to be released by either calling
		/// <see cref="IDisposable.Dispose"/> or <see cref="IAsyncDisposable.DisposeAsync"/> explicity or wrapping code in a using statement.
		/// </remarks>
		ValueTask<ISynchronizationRoot> GetSynchronizationRootAndWaitAsync(Type type, string key, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the synchronization root and waits for it to be released.
		/// </summary>
		/// <typeparam name="T">A target type used to create a the granular synchronization key.</typeparam>
		/// <param name="key">A key used in conjunction with the specified <typeparamref name="T"/> to create the synchronization key.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The created <see cref="ISynchronizationRoot"/>.</returns>
		/// <remarks>
		/// Returned <see cref="ISynchronizationRoot"/> instances need to be disposed in order for the acquired lock to be released by either calling
		/// <see cref="IDisposable.Dispose"/> or <see cref="IAsyncDisposable.DisposeAsync"/> explicity or wrapping code in a using statement.
		/// </remarks>
		ValueTask<ISynchronizationRoot> GetSynchronizationRootAndWaitAsync<T>(string key, CancellationToken cancellationToken = default);
	}
}