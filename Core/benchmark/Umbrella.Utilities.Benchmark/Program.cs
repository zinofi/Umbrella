using BenchmarkDotNet.Running;
using Umbrella.Utilities.Benchmark.Caching;

namespace Umbrella.Utilities.Benchmark;

internal class Program
{
	private static void Main(string[] args)
	{
		//BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
		_ = BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
		//BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
		//BenchmarkRunner.Run<SpanExtensionsBenchmark>();

		Console.WriteLine("Press any key to exit...");
		_ = Console.Read();
	}
}