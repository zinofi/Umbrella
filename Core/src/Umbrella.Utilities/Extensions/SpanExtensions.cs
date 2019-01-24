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
#if NET461 || DEBUG
            return AppendInternalNetClr(span, startIndex, value);
#else
            return AppendInternalCoreClr(span, startIndex, value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AppendInternalNetClr(in Span<char> source, int startIndex, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                source[startIndex++] = value[i];
            }

            return startIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AppendInternalCoreClr(in Span<char> source, int startIndex, string value)
        {
            value.AsSpan().CopyTo(source.Slice(startIndex, value.Length));

            return startIndex + value.Length;
        }
        #endregion

        #region Append ReadOnlySpan<char>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Append(this in Span<char> source, int startIndex, in ReadOnlySpan<char> value)
        {
#if NET461 || DEBUG
            return AppendInternalNetClr(source, startIndex, value);
#else
            return AppendInternalCoreClr(source, startIndex, value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AppendInternalNetClr(in Span<char> source, int startIndex, in ReadOnlySpan<char> value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                source[startIndex++] = value[i];
            }

            return startIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AppendInternalCoreClr(in Span<char> source, int startIndex, in ReadOnlySpan<char> value)
        {
            value.CopyTo(source.Slice(startIndex, value.Length));

            return startIndex + value.Length;
        }
        #endregion

        #region ToLower
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLower(this in Span<char> source, CultureInfo culture = null)
        {
#if NET461 || DEBUG
            ToLowerNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToLowerCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToLowerNetClr(in Span<char> source, CultureInfo culture)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = char.ToLower(source[i], culture);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToLowerCoreClr(in Span<char> source, CultureInfo culture)
        {
            ReadOnlySpan<char> readOnlySpan = source;

            readOnlySpan.ToLower(source, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLowerInvariant(this in Span<char> source) => ToLower(source, CultureInfo.InvariantCulture);
        #endregion

        #region ToUpper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpper(this in Span<char> source, CultureInfo culture = null)
        {
#if NET461 || DEBUG
            ToUpperNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToUpperCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToUpperNetClr(in Span<char> source, CultureInfo culture)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = char.ToUpper(source[i], culture);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToUpperCoreClr(in Span<char> source, CultureInfo culture)
        {
            ReadOnlySpan<char> readOnlySpan = source;

            readOnlySpan.ToUpper(source, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpperInvariant(this in Span<char> source) => ToUpper(source, CultureInfo.InvariantCulture);
        #endregion
    }
}