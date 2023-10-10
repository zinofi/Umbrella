using BenchmarkDotNet.Running;
using Umbrella.Utilities.Benchmark.Extensions;

//BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
// _ = BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
//BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
//BenchmarkRunner.Run<SpanExtensionsBenchmark>();
BenchmarkRunner.Run<StringExtensionsBenchmark>();

Console.WriteLine("Press any key to exit...");
_ = Console.Read();