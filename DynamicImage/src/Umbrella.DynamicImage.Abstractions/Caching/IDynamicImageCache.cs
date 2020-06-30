using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions.Caching
{
	public interface IDynamicImageCache
	{
		Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default);
		Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default);
		Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default);
	}
}