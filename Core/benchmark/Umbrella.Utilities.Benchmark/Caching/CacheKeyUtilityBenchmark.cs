using BenchmarkDotNet.Attributes;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching;

namespace Umbrella.Utilities.Benchmark.Caching
{
    [CoreJob, ClrJob]
    [MemoryDiagnoser]
    public class CacheKeyUtilityBenchmark
    {
        private static readonly string[] _keyItems = new[] { "part1", "part2", "part3", "part4", "part5" };
        private readonly CacheKeyUtility _cacheKeyUtility;

        public CacheKeyUtilityBenchmark()
        {
            _cacheKeyUtility = new CacheKeyUtility();
        }

        [Benchmark]
        public string CreateCacheKey()
        {
            return _cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItems);
        }

#if !AzureDevOps
        [Benchmark(Baseline = true)]
        public string CreateCacheKeyOld()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            return _cacheKeyUtility.CreateOld<CacheKeyUtilityBenchmark>(_keyItems);
#pragma warning restore CS0612 // Type or member is obsolete
        }
#endif
    }
}
