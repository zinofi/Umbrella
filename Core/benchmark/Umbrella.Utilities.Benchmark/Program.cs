using BenchmarkDotNet.Running;
using System;
using Umbrella.Utilities.Benchmark.Caching;
using Umbrella.Utilities.Benchmark.Encryption;
using Umbrella.Utilities.Benchmark.Extensions;

namespace Umbrella.Utilities.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
            BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
            BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
            BenchmarkRunner.Run<SpanExtensionsBenchmark>();

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}