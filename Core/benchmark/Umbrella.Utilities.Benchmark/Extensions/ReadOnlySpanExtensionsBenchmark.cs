using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Benchmark.Extensions;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.NetCoreApp31)]
public class ReadOnlySpanExtensionsBenchmark
{
	[Benchmark]
	public int Char_ToLowerBenchmark()
	{
		Span<char> span = stackalloc char[10];
		span.Fill('A');

		ReadOnlySpan<char> readOnlySpan = span;
		Span<char> destination = stackalloc char[readOnlySpan.Length];

		readOnlySpan.ToLowerSlim(destination);

		return destination.Length;
	}

	[Benchmark]
	public int Char_ToLowerInvariantBenchmark()
	{
		Span<char> span = stackalloc char[10];
		span.Fill('A');

		ReadOnlySpan<char> readOnlySpan = span;
		Span<char> destination = stackalloc char[readOnlySpan.Length];

		readOnlySpan.ToLowerInvariantSlim(destination);

		return destination.Length;
	}

	[Benchmark]
	public int Char_ToUpperBenchmark()
	{
		Span<char> span = stackalloc char[10];
		span.Fill('A');

		ReadOnlySpan<char> readOnlySpan = span;
		Span<char> destination = stackalloc char[readOnlySpan.Length];

		readOnlySpan.ToUpperSlim(destination);

		return destination.Length;
	}

	[Benchmark]
	public int Char_ToUpperInvariantBenchmark()
	{
		Span<char> span = stackalloc char[10];
		span.Fill('A');

		ReadOnlySpan<char> readOnlySpan = span;
		Span<char> destination = stackalloc char[readOnlySpan.Length];

		readOnlySpan.ToUpperInvariantSlim(destination);

		return destination.Length;
	}
}