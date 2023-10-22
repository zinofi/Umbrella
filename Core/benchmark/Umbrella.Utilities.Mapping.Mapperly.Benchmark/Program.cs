using BenchmarkDotNet.Running;

BenchmarkRunner.Run<UmbrellaMapperBenchmark>();

Console.WriteLine("Press any key to exit...");
_ = Console.Read();