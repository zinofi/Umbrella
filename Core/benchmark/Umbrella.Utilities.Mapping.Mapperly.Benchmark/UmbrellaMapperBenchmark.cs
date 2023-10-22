using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Umbrella.Utilities.Mapping.Mapperly.Benchmark;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.Net60)]
public class UmbrellaMapperBenchmark
{
	
}