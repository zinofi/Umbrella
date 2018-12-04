using BenchmarkDotNet.Running;
using System;
using Umbrella.Utilities.Benchmark.Encryption;

namespace Umbrella.Utilities.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Span<char> s = stackalloc char[10];

            BenchmarkRunner.Run<PasswordGeneratorBenchmark>();
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
