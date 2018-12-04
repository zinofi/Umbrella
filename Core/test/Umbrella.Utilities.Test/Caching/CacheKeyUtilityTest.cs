using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;
using Xunit;

namespace Umbrella.Utilities.Test.Caching
{
    public class CacheKeyUtilityTest
    {
        [Fact]
        public void CreateCacheKey_Valid()
        {
            var utility = CreateCacheKeyUtility();
            var keyParts = new[] { "part1", "part2", "part3", "part4", "part5" };

            string key = utility.Create<CacheKeyUtilityTest>(keyParts);

            Assert.Equal($"{typeof(CacheKeyUtilityTest).FullName}:{nameof(CreateCacheKey_Valid)}:{string.Join(":", keyParts)}".ToUpperInvariant(), key);
        }

        private static CacheKeyUtility CreateCacheKeyUtility()
        {
            return new CacheKeyUtility();
        }
    }
}