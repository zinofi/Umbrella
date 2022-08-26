﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for use with <see cref="ReadOnlySpan{T}"/> instances.
/// </summary>
public static class ReadOnlySpanExtensions
{
	#region ToLower
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToLowerSlim(this in ReadOnlySpan<char> source, in Span<char> destination, CultureInfo? culture = null) =>
#if NET461 || DEBUG
		ToLowerSlimNetClr(source, destination, culture ?? CultureInfo.CurrentCulture);
#else
            ToLowerSlimCoreClr(source, destination, culture ?? CultureInfo.CurrentCulture);
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ToLowerSlimNetClr(in ReadOnlySpan<char> source, in Span<char> destination, CultureInfo culture)
	{
		for (int i = 0; i < source.Length; i++)
		{
			destination[i] = char.ToLower(source[i], culture);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ToLowerSlimCoreClr(in ReadOnlySpan<char> source, in Span<char> destination, CultureInfo culture) => source.ToLower(destination, culture);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToLowerInvariantSlim(this in ReadOnlySpan<char> source, in Span<char> destination) => ToLowerSlim(source, destination, CultureInfo.InvariantCulture);
	#endregion

	#region ToUpper
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToUpperSlim(this in ReadOnlySpan<char> source, in Span<char> destination, CultureInfo? culture = null) =>
#if NET461 || DEBUG
		ToUpperSlimNetClr(source, destination, culture ?? CultureInfo.CurrentCulture);
#else
            ToUpperSlimCoreClr(source, destination, culture ?? CultureInfo.CurrentCulture);
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ToUpperSlimNetClr(in ReadOnlySpan<char> source, in Span<char> destination, CultureInfo culture)
	{
		for (int i = 0; i < source.Length; i++)
		{
			destination[i] = char.ToUpper(source[i], culture);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ToUpperSlimCoreClr(in ReadOnlySpan<char> source, in Span<char> destination, CultureInfo culture) => source.ToUpper(destination, culture);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToUpperInvariantSlim(this in ReadOnlySpan<char> source, in Span<char> destination) => ToUpperSlim(source, destination, CultureInfo.InvariantCulture);
	#endregion
}