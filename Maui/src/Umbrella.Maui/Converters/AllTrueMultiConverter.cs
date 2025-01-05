﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Globalization;
using CommunityToolkit.Diagnostics;
using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Converters;

/// <summary>
/// A multi-converter that returns <see langword="true"/> when all source parameters are <see langword="true"/>.
/// </summary>
/// <seealso cref="IMultiValueConverter" />
public class AllTrueMultiConverter : IMultiValueConverter
{
	/// <inheritdoc />
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		Guard.IsNotNull(targetType);

		return values is null || !targetType.IsAssignableFrom(typeof(bool)) ? false : (object)values.OfType<bool>().All(x => x);
	}

	/// <inheritdoc />
	public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}