using BenchmarkDotNet.Running;
using Umbrella.DynamicImage.Benchmark;

_ = BenchmarkRunner.Run<DynamicImageUtilityBenchmark>();
Console.WriteLine("Press any key to exit...");
_ = Console.Read();