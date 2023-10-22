using BenchmarkDotNet.Running;
using Umbrella.Utilities.Mapping.Mapperly.Benchmark;

BenchmarkRunner.Run<UmbrellaMapperBenchmark>();

Console.WriteLine("Press any key to exit...");
_ = Console.Read();