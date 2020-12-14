using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// A set of extension methods for <see cref="Enum"/> types.
	/// </summary>
	public static class EnumExtensions
	{
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
			Guard.ArgumentNotNull(separator, nameof(separator));

			var lstOption = new List<string>();

			foreach (Enum item in Enum.GetValues(options.GetType()))
			{
				if (options.HasFlag(item))
					lstOption.Add(valueTransformer?.Invoke(item.ToString()) ?? item.ToString());
			}

			return string.Join(separator, lstOption);
		}
	}
}