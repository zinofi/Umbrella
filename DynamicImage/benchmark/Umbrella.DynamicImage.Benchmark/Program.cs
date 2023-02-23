using BenchmarkDotNet.Running;

namespace Umbrella.DynamicImage.Benchmark;

internal class Program
{
	private static void Main(string[] args)
	{
		_ = BenchmarkRunner.Run<DynamicImageUtilityBenchmark>();
		Console.WriteLine("Press any key to exit...");
		_ = Console.Read();
	}
}