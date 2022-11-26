using System;
using System.Globalization;
using System.Runtime.CompilerServices;


/* Unmerged change from project 'Umbrella.Utilities(net461)'
Before:
namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for use with <see cref="Span{T}"/> instances.
	/// </summary>
	public static class SpanExtensions
    {
		#region Append String
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Append(this in Span<char> span, int startIndex, string value) =>
#if NET461 || DEBUG
			AppendInternalNetClr(span, startIndex, value);
#else
            AppendInternalCoreClr(span, startIndex, value);
#endif


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
		public static int Append(this in Span<char> source, int startIndex, in ReadOnlySpan<char> value) =>
#if NET461 || DEBUG
			AppendInternalNetClr(source, startIndex, value);
#else
            AppendInternalCoreClr(source, startIndex, value);
#endif


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
		public static void ToLower(this in Span<char> source, CultureInfo? culture = null) =>
#if NET461 || DEBUG
			ToLowerNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToLowerCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif


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
			for (int i = 0; i < source.Length; i++)
			{
				source[i] = char.ToLower(source[i], culture);
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLowerInvariant(this in Span<char> source) => ToLower(source, CultureInfo.InvariantCulture);
		#endregion

		#region ToUpper
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToUpper(this in Span<char> source, CultureInfo? culture = null) =>
#if NET461 || DEBUG
			ToUpperNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToUpperCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif


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
			for (int i = 0; i < source.Length; i++)
			{
				source[i] = char.ToUpper(source[i], culture);
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpperInvariant(this in Span<char> source) => ToUpper(source, CultureInfo.InvariantCulture);
        #endregion
    }
}
After:
namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for use with <see cref="Span{T}"/> instances.
/// </summary>
public static class SpanExtensions
    {
	#region Append String
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Append(this in Span<char> span, int startIndex, string value) =>
#if NET461 || DEBUG
		AppendInternalNetClr(span, startIndex, value);
#else
            AppendInternalCoreClr(span, startIndex, value);
#endif

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
	public static int Append(this in Span<char> source, int startIndex, in ReadOnlySpan<char> value) =>
#if NET461 || DEBUG
		AppendInternalNetClr(source, startIndex, value);
#else
            AppendInternalCoreClr(source, startIndex, value);
#endif

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
	public static void ToLower(this in Span<char> source, CultureInfo? culture = null) =>
#if NET461 || DEBUG
		ToLowerNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToLowerCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif

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
		for (int i = 0; i < source.Length; i++)
		{
			source[i] = char.ToLower(source[i], culture);
		}
	}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToLowerInvariant(this in Span<char> source) => ToLower(source, CultureInfo.InvariantCulture);
	#endregion

	#region ToUpper
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToUpper(this in Span<char> source, CultureInfo? culture = null) =>
#if NET461 || DEBUG
		ToUpperNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToUpperCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif

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
		for (int i = 0; i < source.Length; i++)
		{
			source[i] = char.ToUpper(source[i], culture);
		}
	}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToUpperInvariant(this in Span<char> source) => ToUpper(source, CultureInfo.InvariantCulture);
        #endregion
    }
*/
namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for use with <see cref="Span{T}"/> instances.
/// </summary>
public static class SpanExtensions
{
	#region Append String
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Append(this in Span<char> span, int startIndex, string value) =>
#if NET461 || DEBUG
		AppendInternalNetClr(span, startIndex, value);
#else
            AppendInternalCoreClr(span, startIndex, value);
#endif

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
	public static int Append(this in Span<char> source, int startIndex, in ReadOnlySpan<char> value) =>
#if NET461 || DEBUG
		AppendInternalNetClr(source, startIndex, value);
#else
            AppendInternalCoreClr(source, startIndex, value);
#endif

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
	public static void ToLower(this in Span<char> source, CultureInfo? culture = null) =>
#if NET461 || DEBUG
		ToLowerNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToLowerCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif

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
		for (int i = 0; i < source.Length; i++)
		{
			source[i] = char.ToLower(source[i], culture);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToLowerInvariant(this in Span<char> source) => ToLower(source, CultureInfo.InvariantCulture);
	#endregion

	#region ToUpper
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToUpper(this in Span<char> source, CultureInfo? culture = null) =>
#if NET461 || DEBUG
		ToUpperNetClr(source, culture ?? CultureInfo.CurrentCulture);
#else
            ToUpperCoreClr(source, culture ?? CultureInfo.CurrentCulture);
#endif

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
		for (int i = 0; i < source.Length; i++)
		{
			source[i] = char.ToUpper(source[i], culture);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToUpperInvariant(this in Span<char> source) => ToUpper(source, CultureInfo.InvariantCulture);
	#endregion
}