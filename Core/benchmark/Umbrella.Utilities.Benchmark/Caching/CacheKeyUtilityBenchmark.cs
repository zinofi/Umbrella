﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.Internal.Mocks;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Data.Abstractions;

/* Unmerged change from project 'Umbrella.Utilities.Benchmark(net461)'
Before:
namespace Umbrella.Utilities.Benchmark.Caching
{
	[MemoryDiagnoser]
	[SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.NetCoreApp31)]
	public class CacheKeyUtilityBenchmark
	{
		private static readonly string[] _keyItems = new[] { "part1", "part2", "part3", "part4", "part5" };
		private static readonly string[] _keyItemsLong = new[] { "part1", "part2", "part3", "part4", "part5", "part6", "part7", "part8", "part9", "part10", "part11", "part12", "part13", "part14" };
		private readonly CacheKeyUtility _cacheKeyUtility;

		public CacheKeyUtilityBenchmark()
		{
			IDataLookupNormalizer lookupNormalizer = CoreUtilitiesMocks.CreateILookupNormalizer();
			_cacheKeyUtility = new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, lookupNormalizer);
		}

		[Benchmark]
		public string CreateCacheKey() =>
			// Length of 91
			_cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItems);

		[Benchmark]
		public string CreateCacheKey_NonStack() =>
			// Length of 150
			_cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItemsLong);

#if !AzureDevOps
		[Benchmark(Baseline = true)]
		public string CreateCacheKeyOld() =>
#pragma warning disable CS0612 // Type or member is obsolete
			_cacheKeyUtility.CreateOld<CacheKeyUtilityBenchmark>(_keyItems);
#pragma warning restore CS0612 // Type or member is obsolete

#endif
	}
After:
namespace Umbrella.Utilities.Benchmark.Caching;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.NetCoreApp31)]
public class CacheKeyUtilityBenchmark
{
	private static readonly string[] _keyItems = new[] { "part1", "part2", "part3", "part4", "part5" };
	private static readonly string[] _keyItemsLong = new[] { "part1", "part2", "part3", "part4", "part5", "part6", "part7", "part8", "part9", "part10", "part11", "part12", "part13", "part14" };
	private readonly CacheKeyUtility _cacheKeyUtility;

	public CacheKeyUtilityBenchmark()
	{
		IDataLookupNormalizer lookupNormalizer = CoreUtilitiesMocks.CreateILookupNormalizer();
		_cacheKeyUtility = new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, lookupNormalizer);
	}

	[Benchmark]
	public string CreateCacheKey() =>
		// Length of 91
		_cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItems);

	[Benchmark]
	public string CreateCacheKey_NonStack() =>
		// Length of 150
		_cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItemsLong);

#if !AzureDevOps
	[Benchmark(Baseline = true)]
	public string CreateCacheKeyOld() =>
#pragma warning disable CS0612 // Type or member is obsolete
		_cacheKeyUtility.CreateOld<CacheKeyUtilityBenchmark>(_keyItems);
#pragma warning restore CS0612 // Type or member is obsolete

#endif
*/
namespace Umbrella.Utilities.Benchmark.Caching;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.NetCoreApp31)]
public class CacheKeyUtilityBenchmark
{
	private static readonly string[] _keyItems = new[] { "part1", "part2", "part3", "part4", "part5" };
	private static readonly string[] _keyItemsLong = new[] { "part1", "part2", "part3", "part4", "part5", "part6", "part7", "part8", "part9", "part10", "part11", "part12", "part13", "part14" };
	private readonly CacheKeyUtility _cacheKeyUtility;

	public CacheKeyUtilityBenchmark()
	{
		IDataLookupNormalizer lookupNormalizer = CoreUtilitiesMocks.CreateILookupNormalizer();
		_cacheKeyUtility = new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, lookupNormalizer);
	}

	[Benchmark]
	public string CreateCacheKey() =>
		// Length of 91
		_cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItems);

	[Benchmark]
	public string CreateCacheKey_NonStack() =>
		// Length of 150
		_cacheKeyUtility.Create<CacheKeyUtilityBenchmark>(_keyItemsLong);

#if !AzureDevOps
	[Benchmark(Baseline = true)]
	public string CreateCacheKeyOld() =>
#pragma warning disable CS0612 // Type or member is obsolete
		_cacheKeyUtility.CreateOld<CacheKeyUtilityBenchmark>(_keyItems);
#pragma warning restore CS0612 // Type or member is obsolete

#endif
}