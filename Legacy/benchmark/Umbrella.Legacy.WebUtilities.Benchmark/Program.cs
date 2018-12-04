using BenchmarkDotNet.Running;
using System;
using Umbrella.Legacy.WebUtilities.Benchmark.Hosting;

namespace Umbrella.Legacy.WebUtilities.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<UmbrellaWebHostingEnvironmentBenchmark>();

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
