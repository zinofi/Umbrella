using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Benchmark.Extensions
{
    [CoreJob]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class SpanExtensionsBenchmark
    {
        [Benchmark]
        [BenchmarkCategory(nameof(Char_AppendStringBenchmark))]
        public int Char_AppendStringBenchmark()
        {
            Span<char> span = stackalloc char[10];

            int currentIndex = span.Append(0, "12345");
            currentIndex = span.Append(currentIndex, "67890");

            return span.Length;
        }

#if !AzureDevOps
        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(Char_AppendStringBenchmark))]
        public int Char_AppendStringBenchmark_Old()
        {
            Span<char> span = stackalloc char[10];

            int currentIndex = span.AppendOld(0, "12345");
            currentIndex = span.AppendOld(currentIndex, "67890");

            return span.Length;
        }
#endif

        [Benchmark]
        [BenchmarkCategory(nameof(Char_AppendReadOnlySpanBenchmark))]
        public int Char_AppendReadOnlySpanBenchmark()
        {
            Span<char> span = stackalloc char[10];

            int currentIndex = span.Append(0, "12345".AsSpan());
            currentIndex = span.Append(currentIndex, "67890".AsSpan());

            return span.Length;
        }

#if !AzureDevOps
        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(Char_AppendReadOnlySpanBenchmark))]
        public int Char_AppendReadOnlySpanBenchmark_Old()
        {
            Span<char> span = stackalloc char[10];

            int currentIndex = span.AppendOld(0, "12345".AsSpan());
            currentIndex = span.AppendOld(currentIndex, "67890".AsSpan());

            return span.Length;
        }
#endif

        [Benchmark]
        [BenchmarkCategory(nameof(Char_ToLowerBenchmark))]
        public int Char_ToLowerBenchmark()
        {
            Span<char> span = stackalloc char[10];
            span.Fill('A');

            span.ToLower();

            return span.Length;
        }

        [Benchmark]
        [BenchmarkCategory(nameof(Char_ToLowerInvariantBenchmark))]
        public int Char_ToLowerInvariantBenchmark()
        {
            Span<char> span = stackalloc char[10];
            span.Fill('A');

            span.ToLowerInvariant();

            return span.Length;
        }

#if !AzureDevOps
        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(Char_ToLowerInvariantBenchmark))]
        public int Char_ToLowerInvariantBenchmark_Old()
        {
            Span<char> span = stackalloc char[10];
            span.Fill('A');

            span.ToLowerInvariantOld();

            return span.Length;
        }
#endif

        [Benchmark]
        [BenchmarkCategory(nameof(Char_ToUpperBenchmark))]
        public int Char_ToUpperBenchmark()
        {
            Span<char> span = stackalloc char[10];
            span.Fill('A');

            span.ToUpper();

            return span.Length;
        }

        [Benchmark]
        [BenchmarkCategory(nameof(Char_ToUpperInvariantBenchmark))]
        public int Char_ToUpperInvariantBenchmark()
        {
            Span<char> span = stackalloc char[10];
            span.Fill('A');

            span.ToUpperInvariant();

            return span.Length;
        }
    }
}