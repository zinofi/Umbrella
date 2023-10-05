using BenchmarkDotNet.Running;
using Umbrella.Utilities.Benchmark.Caching;

//BenchmarkRunner.Run<SecureStringGeneratorBenchmark>();
_ = BenchmarkRunner.Run<CacheKeyUtilityBenchmark>();
//BenchmarkRunner.Run<ReadOnlySpanExtensionsBenchmark>();
//BenchmarkRunner.Run<SpanExtensionsBenchmark>();

Console.WriteLine("Press any key to exit...");
_ = Console.Read();