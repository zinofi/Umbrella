﻿using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Umbrella.Utilities.Imaging.Abstractions;

namespace Umbrella.Maui.Converters;

// TODO: Could make this public and encode the parameters in the ConverterParameter property using a pipe?
internal class UmbrellaResponsiveImageConverter : IValueConverter
{
	protected int MaxPixelDensity { get; }
	protected IValueConverter? InnerConverter { get; }
	private IResponsiveImageHelper ResponsiveImageHelper { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaResponsiveImageConverter"/> class.
	/// </summary>
	/// <param name="maxPixelDensity">The maximum pixel density.</param>
	/// <param name="innerConverter">The inner converter.</param>
	public UmbrellaResponsiveImageConverter(int maxPixelDensity, IValueConverter? innerConverter)
	{
		MaxPixelDensity = maxPixelDensity;
		InnerConverter = innerConverter;

		ResponsiveImageHelper = UmbrellaMauiServices.GetService<IResponsiveImageHelper>();
	}

	/// <inheritdoc />
	public virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		object? convertedValue = value;

		if (InnerConverter is not null)
			convertedValue = InnerConverter.Convert(value, targetType, parameter, culture);

		if (convertedValue is string imageUrl)
		{
			int devicePixelDensity = (int)Math.Ceiling(DeviceDisplay.MainDisplayInfo.Density);
			int targetPixelDensity = Math.Min(devicePixelDensity, MaxPixelDensity);

			string densityUrl = ResponsiveImageHelper.GetPixelDensityImageUrl(imageUrl, targetPixelDensity);

			return densityUrl;
		}

		return null;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}