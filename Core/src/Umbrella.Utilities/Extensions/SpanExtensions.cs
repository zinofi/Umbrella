using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class SpanExtensions
    {
        #region Append String
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Append(this in Span<char> span, int startIndex, string value)
        {
#if NET47 || DEBUG
            return AppendInternalNetClr(span, startIndex, value);
#else
            return AppendInternalCoreClr(span, startIndex, value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AppendInternalNetClr(this in Span<char> span, int startIndex, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                span[startIndex++] = value[i];
            }

            return startIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AppendInternalCoreClr(this in Span<char> span, int startIndex, string value)
        {
            value.AsSpan().CopyTo(span.Slice(startIndex, value.Length));

            return startIndex + value.Length;
        }
        #endregion

        #region Append ReadOnlySpan<char>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Append(this in Span<char> span, int startIndex, in ReadOnlySpan<char> value)
        {
#if NET47 || DEBUG
            return AppendInternalNetClr(span, startIndex, value);
#else
            return AppendInternalCoreClr(span, startIndex, value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AppendInternalNetClr(this in Span<char> span, int startIndex, in ReadOnlySpan<char> value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                span[startIndex++] = value[i];
            }

            return startIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AppendInternalCoreClr(this in Span<char> span, int startIndex, in ReadOnlySpan<char> value)
        {
            value.CopyTo(span.Slice(startIndex, value.Length));

            return startIndex + value.Length;
        }
        #endregion

        #region ToLower
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLower(this in Span<char> span)
        {
#if NET47 || DEBUG
            ToLowerNetClr(span);
#else
            ToLowerCoreClr(span);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToLowerNetClr(this in Span<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = char.ToLower(span[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToLowerCoreClr(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToLower(span, CultureInfo.CurrentCulture);
        }
        #endregion

        #region ToLowerInvariant
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLowerInvariant(this in Span<char> span)
        {
#if NET47 || DEBUG
            ToLowerInvariantNetClr(span);
#else
            ToLowerInvariantCoreClr(span);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToLowerInvariantNetClr(this in Span<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = char.ToLowerInvariant(span[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToLowerInvariantCoreClr(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToLowerInvariant(span);
        }
        #endregion

        #region ToUpper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpper(this in Span<char> span)
        {
#if NET47 || DEBUG
            ToUpperNetClr(span);
#else
            ToUpperCoreClr(span);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToUpperNetClr(this in Span<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = char.ToUpper(span[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToUpperCoreClr(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToUpper(span, CultureInfo.CurrentCulture);
        }
        #endregion

        #region ToUpperInvariant
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpperInvariant(this in Span<char> span)
        {
#if NET47 || DEBUG
            ToUpperInvariantNetClr(span);
#else
            ToUpperInvariantCoreClr(span);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToUpperInvariantNetClr(this in Span<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = char.ToUpperInvariant(span[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToUpperInvariantCoreClr(this in Span<char> span)
        {
            ReadOnlySpan<char> readOnlySpan = span;

            readOnlySpan.ToUpperInvariant(span);
        }
        #endregion
    }
}