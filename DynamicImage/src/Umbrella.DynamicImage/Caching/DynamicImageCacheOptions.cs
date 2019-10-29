using Microsoft.Extensions.Caching.Memory;

namespace Umbrella.DynamicImage.Caching
{
	/// <summary>
	/// Used to specify the core Dynamic Image options.
	/// </summary>
	public class DynamicImageCacheCoreOptions
	{
		/// <summary>
		/// Gets or sets the <see cref="MemoryCacheEntryOptions"/> used for caching the generated cache keys which correspond to cached filenames.
		/// </summary>
		public MemoryCacheEntryOptions CacheKeyCacheOptions { get; set; }
	}
}