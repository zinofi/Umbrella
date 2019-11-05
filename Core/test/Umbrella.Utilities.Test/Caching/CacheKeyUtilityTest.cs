using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.Internal.Mocks;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Data.Abstractions;
using Xunit;

namespace Umbrella.Utilities.Test.Caching
{
	public class CacheKeyUtilityTest
	{
		[Fact]
		public void CreateCacheKey_Valid()
		{
			CacheKeyUtility utility = CreateCacheKeyUtility();

			string key = utility.Create<CacheKeyUtilityTest>("test:key");

			Assert.Equal($"{typeof(CacheKeyUtilityTest).FullName}:test:key".ToUpperInvariant(), key);
		}

		[Fact]
		public void CreateCacheKey_Parts_Valid()
		{
			CacheKeyUtility utility = CreateCacheKeyUtility();
			string[] keyParts = new[] { "part1", "part2", "part3", "part4", "part5" };

			string key = utility.Create<CacheKeyUtilityTest>(keyParts);

			Assert.Equal($"{typeof(CacheKeyUtilityTest).FullName}:{string.Join(":", keyParts)}".ToUpperInvariant(), key);
		}

		private static CacheKeyUtility CreateCacheKeyUtility()
		{
			ILookupNormalizer lookupNormalizer = CoreUtilitiesMocks.CreateILookupNormalizer();
			return new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, lookupNormalizer);
		}
	}
}