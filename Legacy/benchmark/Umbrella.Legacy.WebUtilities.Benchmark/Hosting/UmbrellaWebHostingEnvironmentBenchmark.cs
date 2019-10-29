using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Hosting.Options;

namespace Umbrella.Legacy.WebUtilities.Benchmark.Hosting
{
    [ClrJob]
    [MemoryDiagnoser]
    public class UmbrellaWebHostingEnvironmentBenchmark
    {
        private readonly UmbrellaWebHostingEnvironment _umbrellaWebHostingEnvironment;

        public UmbrellaWebHostingEnvironmentBenchmark()
        {
            var logger = new Mock<ILogger<UmbrellaWebHostingEnvironment>>();
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var cacheKeyUtility = new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object);

            _umbrellaWebHostingEnvironment = new UmbrellaWebHostingEnvironment(logger.Object, new UmbrellaHostingEnvironmentOptions(), memoryCache, cacheKeyUtility);
        }

        [Benchmark]
        public string TransformPath()
        {
            return _umbrellaWebHostingEnvironment.TransformPath("~/path/to/a/resource.jpg", true, false, false);
        }

        [Benchmark]
        public string TransformPathOld()
        {
            return _umbrellaWebHostingEnvironment.TransformPathOld("~/path/to/a/resource.jpg", true, false, false);
        }
    }
}