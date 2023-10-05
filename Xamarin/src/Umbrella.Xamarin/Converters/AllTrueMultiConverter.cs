﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Globalization;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters;

/// <summary>
/// A multi-converter that returns <see langword="true"/> when all source parameters are <see langword="true"/>.
/// </summary>
/// <seealso cref="IMultiValueConverter" />
public class AllTrueMultiConverter : IMultiValueConverter
{
	/// <inheritdoc />
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		=> values is null || !targetType.IsAssignableFrom(typeof(bool)) ? false : (object)values.OfType<bool>().All(x => x);

	/// <inheritdoc />
	public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}