using BenchmarkDotNet.Running;
using System;

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