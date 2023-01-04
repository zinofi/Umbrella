using System;
using BenchmarkDotNet.Running;

/* Unmerged change from project 'Umbrella.DynamicImage.Benchmark(net461)'
Before:
namespace Umbrella.DynamicImage.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DynamicImageUtilityBenchmark>();
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
After:
namespace Umbrella.DynamicImage.Benchmark;

internal class Program
    {
	private static void Main(string[] args)
        {
            BenchmarkRunner.Run<DynamicImageUtilityBenchmark>();
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
*/
namespace Umbrella.DynamicImage.Benchmark;

internal class Program
{
	private static void Main(string[] args)
	{
		BenchmarkRunner.Run<DynamicImageUtilityBenchmark>();
		Console.WriteLine("Press any key to exit...");
		Console.Read();
	}
}