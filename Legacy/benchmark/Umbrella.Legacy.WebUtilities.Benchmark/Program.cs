using BenchmarkDotNet.Running;
using Umbrella.Legacy.WebUtilities.Benchmark.Middleware;

//BenchmarkRunner.Run<UmbrellaWebHostingEnvironmentBenchmark>();
_ = BenchmarkRunner.Run<FrontEndCompressionMiddlewareBenchmark>();

Console.WriteLine("Press any key to exit...");
_ = Console.Read();