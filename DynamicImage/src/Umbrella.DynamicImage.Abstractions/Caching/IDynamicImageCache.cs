using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions.Caching
{
	/// <summary>
	/// Defines a contract for a caching mechanism for generated <see cref="DynamicImageItem"/> instances.
	/// </summary>
	public interface IDynamicImageCache
	{
		/// <summary>
		/// Adds the item to the cache.
		/// </summary>
		/// <param name="dynamicImage">The dynamic image.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the item has been added to the cache.</returns>
		Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the item in the cache if it exists.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="sourceLastModified">The source last modified.</param>
		/// <param name="fileExtension">The file extension.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The cached item.</returns>
		Task<DynamicImageItem?> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default);

		/// <summary>
		/// Removes the item from the cache.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="fileExtension">The file extension.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that completes when the item has been removed from the cache.</returns>
		Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default);
	}
}