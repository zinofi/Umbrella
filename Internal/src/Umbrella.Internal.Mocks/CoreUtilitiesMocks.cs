using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.Internal.Mocks
{
	public static class CoreUtilitiesMocks
	{
		public static ILookupNormalizer CreateILookupNormalizer()
		{
			var lookupNormalizer = new Mock<ILookupNormalizer>();
			lookupNormalizer.Setup(x => x.Normalize(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((value, trim) =>
			{
				if (value is null)
					return null;

				return trim ? value.Trim().ToUpperInvariant() : value.ToUpperInvariant();
			});

			return lookupNormalizer.Object;
		}

		public static ICacheKeyUtility CreateCacheKeyUtility() => new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, CreateILookupNormalizer());

		public static IHybridCache CreateHybridCache(bool enableCaching = true)
		{
			return new HybridCache(
				new Mock<ILogger<HybridCache>>().Object,
				new HybridCacheOptions { CacheEnabled = enableCaching },
				CreateILookupNormalizer(),
				new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
				new MemoryCache(Options.Create(new MemoryCacheOptions())));
		}
	}
}