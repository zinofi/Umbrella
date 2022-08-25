// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// A set of extension methods for <see cref="Enum"/> types.
/// </summary>
public static class EnumExtensions
{
	private static readonly ConcurrentDictionary<Enum, string> _enumDisplayStringDictionary = new();

	/// <summary>
	/// Converts the specified enum value that uses the <see cref="FlagsAttribute"/> to encapsulate multiple values
	/// into a string containing the names of the enum values using the specified parameters.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <param name="valueTransformer">The value transformer.</param>
	/// <param name="separator">The separator.</param>
	/// <returns>A string containing the names of the values encapsulated by the enum.</returns>
	public static string ToFlagsString(this Enum options, Func<string, string>? valueTransformer = null, string separator = ",")
	{
		Guard.IsNotNull(separator, nameof(separator));

		var lstOption = new List<string>();

		foreach (Enum item in Enum.GetValues(options.GetType()))
		{
			if (options.HasFlag(item))
				lstOption.Add(valueTransformer?.Invoke(item.ToString()) ?? item.ToString());
		}

		return string.Join(separator, lstOption);
	}

	/// <summary>
	/// Converts the specified enum value to a friendly string that can be displayed in a UI.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The display string.</returns>
	/// <remarks>
	/// This tries to read the value from a <see cref="DisplayAttribute"/> that exists on the enum member. If it cannot be found,
	/// the enum value is converted directly to a string using <see cref="Enum.ToString()"/>
	/// </remarks>
	public static string ToDisplayString(this Enum value)
	{
		return _enumDisplayStringDictionary.GetOrAdd(value, option =>
		{
			return option.GetType().GetFields().Single(x => x.Name == option.ToString()).GetCustomAttribute<DisplayAttribute>()?.Name?.ToString() ?? option.ToString();
		});
	}
}