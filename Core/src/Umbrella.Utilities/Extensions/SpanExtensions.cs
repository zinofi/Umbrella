using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Append(this in Span<char> span, int startIndex, string value)
        {
            value.AsSpan().CopyTo(span.Slice(startIndex, value.Length));

            return startIndex + value.Length;
        }

#if !AzureDevOps
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AppendOld(this in Span<char> span, int startIndex, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                span[startIndex++] = value[i];
            }

            return startIndex;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Append(this in Span<char> span, int startIndex, in ReadOnlySpan<char> value)
        {
            value.CopyTo(span.Slice(startIndex, value.Length));

            return startIndex + value.Length;
        }

#if !AzureDevOps
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AppendOld(this in Span<char> span, int startIndex, in ReadOnlySpan<char> value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                span[startIndex++] = value[i];
            }

            return startIndex;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLower(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToLower(span, CultureInfo.CurrentCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLowerInvariant(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToLowerInvariant(span);
        }

#if !AzureDevOps
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToLowerInvariantOld(this in Span<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = char.ToLowerInvariant(span[i]);
            }
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpper(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToUpper(span, CultureInfo.CurrentCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpperInvariant(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToUpperInvariant(span);
        }
    }
}