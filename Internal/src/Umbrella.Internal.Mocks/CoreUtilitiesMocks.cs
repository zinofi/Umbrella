using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
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
	}
}