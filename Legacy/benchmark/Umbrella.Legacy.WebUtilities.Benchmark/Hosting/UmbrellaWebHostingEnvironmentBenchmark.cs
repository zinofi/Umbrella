using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Umbrella.Internal.Mocks;
using Umbrella.Legacy.WebUtilities.Hosting;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Data.Abstractions;
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
			var memoryCache = CoreUtilitiesMocks.CreateHybridCache();
			ILookupNormalizer lookupNormalizer = CoreUtilitiesMocks.CreateILookupNormalizer();

			var cacheKeyUtility = new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, lookupNormalizer);

			_umbrellaWebHostingEnvironment = new UmbrellaWebHostingEnvironment(logger.Object, new UmbrellaHostingEnvironmentOptions(), memoryCache, cacheKeyUtility);
		}

		[Benchmark]
		public string TransformPath() => _umbrellaWebHostingEnvironment.TransformPath("~/path/to/a/resource.jpg", true, false, false);

		[Benchmark]
		public string TransformPathOld() => _umbrellaWebHostingEnvironment.TransformPathOld("~/path/to/a/resource.jpg", true, false, false);
	}
}