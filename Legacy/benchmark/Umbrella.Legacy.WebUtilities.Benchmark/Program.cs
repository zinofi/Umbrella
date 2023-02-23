using BenchmarkDotNet.Running;
using Umbrella.Legacy.WebUtilities.Benchmark.Middleware;

namespace Umbrella.Legacy.WebUtilities.Benchmark;

internal class Program
{
	private static void Main(string[] args)
	{
		//BenchmarkRunner.Run<UmbrellaWebHostingEnvironmentBenchmark>();
		_ = BenchmarkRunner.Run<FrontEndCompressionMiddlewareBenchmark>();

		Console.WriteLine("Press any key to exit...");
		_ = Console.Read();
	}
}