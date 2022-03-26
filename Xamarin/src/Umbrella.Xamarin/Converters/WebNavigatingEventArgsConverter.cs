// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters
{
	/// <summary>
	/// A converter used to cast an object whose underying type is <see cref="WebNavigatedEventArgs"/> to its concrete type. Primarily used in conjunction
	/// with the <see cref="global::Xamarin.CommunityToolkit.Behaviors.EventToCommandBehavior{TType}"/> type.
	/// </summary>
	/// <seealso cref="IValueConverter" />
	public class WebNavigatingEventArgsConverter : IValueConverter
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is WebNavigatingEventArgs eventArgs))
				throw new ArgumentException("Expected WebNavigatingEventArgs as value", "value");

			return eventArgs;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}