using System;
using BenchmarkDotNet.Running;
using Umbrella.Utilities.Benchmark.Caching;

/* Unmerged change from project 'Umbrella.Utilities.Benchmark(net461)'
Before:
namespace Umbrella.Utilities.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
            BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
            //BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
            //BenchmarkRunner.Run<SpanExtensionsBenchmark>();

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
After:
namespace Umbrella.Utilities.Benchmark;

internal class Program
    {
	private static void Main(string[] args)
        {
            //BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
            BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
            //BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
            //BenchmarkRunner.Run<SpanExtensionsBenchmark>();

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
*/
using Umbrella.Utilities.Benchmark.Encryption;
using Umbrella.Utilities.Benchmark.Extensions;

namespace Umbrella.Utilities.Benchmark;

internal class Program
{
	private static void Main(string[] args)
	{
		//BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
		BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
		//BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
		//BenchmarkRunner.Run<SpanExtensionsBenchmark>();

		Console.WriteLine("Press any key to exit...");
		Console.Read();
	}
}