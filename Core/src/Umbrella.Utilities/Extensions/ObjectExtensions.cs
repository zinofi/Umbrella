// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using System.ComponentModel;
using System.Reflection;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extensions methods that operate on any <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
	/// <summary>
	/// Converts to the specified <paramref name="value"/> to a JSON string.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="useCamelCasingRules">if set to <see langword="true"/>, uses camelcase naming rules for property names.</param>
	/// <returns>The JSON string.</returns>
	public static string ToJsonString(this object value, bool useCamelCasingRules = false)
	{
		Guard.IsNotNull(value);

		return UmbrellaStatics.SerializeJson(value, useCamelCasingRules);
	}

    // TODO: Write a method that can trim all string properties
    // Needs to be able to trim strings in all parts of the object graph, including collections, where possible.
    // Can only do this where we are allowed to assign a new string.

    // Initally create a version that uses compiled expressions / reflection to do this
    // but ultimately we need to use source generators to do this. Create an attribute that can be added to a class
    // that creates a TrimStringProperties method. Need to think about how this will work with inheritance.

    /// <summary>
    /// Trims all string properties recursively.
    /// </summary>
    /// <param name="value">The value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
	public static void TrimAllStringProperties(this object value)
	{
		if (value is null)
			return;

		Type type = value.GetType();
		PropertyInfo[] lstPublicProperty = type.GetProperties();

		foreach (PropertyInfo pi in lstPublicProperty)
		{
			if (pi.PropertyType == typeof(string) && pi.CanWrite && pi.GetValue(value) is string propertyValue && !string.IsNullOrEmpty(propertyValue))
			{
				pi.SetValue(value, propertyValue.Trim());
			}
			else
			{
				// Flat collections.
				// IList<T>, ICollection<T>, IList, ICollection

				// Dictionaries
				// IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>

				// Sets
				// ISet<T>
			}
		}
	}
}