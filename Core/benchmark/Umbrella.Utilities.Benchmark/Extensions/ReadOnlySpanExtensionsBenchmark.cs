using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Benchmark.Extensions
{
    [CoreJob, ClrJob]
    [MemoryDiagnoser]
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
}