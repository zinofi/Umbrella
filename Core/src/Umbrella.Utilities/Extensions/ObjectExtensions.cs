// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extensions methods that operate on any <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
	#region Public Static Methods		
	/// <summary>
	/// Converts to the specified <paramref name="value"/> to a JSON string.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="useCamelCasingRules">if set to <see langword="true"/>, uses camelcase naming rules for property names.</param>
	/// <returns>The JSON string.</returns>
	public static string ToJsonString(this object value, bool useCamelCasingRules = false)
	{
		Guard.IsNotNull(value, nameof(value));

		return UmbrellaStatics.SerializeJson(value, useCamelCasingRules);
	}
	#endregion
}