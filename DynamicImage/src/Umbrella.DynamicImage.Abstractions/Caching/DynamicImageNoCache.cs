using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions.Caching
{
	/// <summary>
	/// A default caching implementation that doesn't actually perform any caching. Useful for unit testing
	/// and scenarios where caching isn't required.
	/// </summary>
	public class DynamicImageNoCache : IDynamicImageCache
	{
		#region IDynamicImageCache Members
		/// <inheritdoc />
		public Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default)
			=> Task.CompletedTask;

		/// <inheritdoc />
		public Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default)
			=> Task.FromResult<DynamicImageItem>(null);

		/// <inheritdoc />
		public Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default)
			=> Task.CompletedTask;
		#endregion
	}
}