using System;
using Microsoft.AspNetCore.Components;
using Umbrella.Utilities.Http;

namespace Umbrella.AspNetCore.Blazor.Extensions
{
	/// <summary>
	/// Extension methods for use with the <see cref="NavigationManager" /> class.
	/// </summary>
	public static class NavigationManagerExtensions
	{
		/// <summary>
		/// Tries the get query string value from the <see cref="NavigationManager.Uri"/> property.
		/// </summary>
		/// <typeparam name="T">The type of the value being read from the querystring.</typeparam>
		/// <param name="navManager">The nav manager.</param>
		/// <param name="key">The key of the value being read from the querystring..</param>
		/// <returns>A tuple containing fields indicating success together with any value.</returns>
		/// <exception cref="NotSupportedException">Query Paramaters of type {typeof(T).Name} cannot be converted.</exception>
		public static (bool success, T value) TryGetQueryStringValue<T>(this NavigationManager navManager, string key)
		{
			var uri = navManager.ToAbsoluteUri(navManager.Uri);

			if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
			{
				if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out int valueAsInt))
				{
					var value = (T)(object)valueAsInt;
					return (true, value);
				}

				if (typeof(T) == typeof(string))
				{
					var value = (T)(object)valueFromQueryString.ToString();
					return (true, value);
				}

				if (typeof(T) == typeof(double) && double.TryParse(valueFromQueryString, out double valueAsDouble))
				{
					var value = (T)(object)valueAsDouble;
					return (true, value);
				}

				if (typeof(T) == typeof(bool) && bool.TryParse(valueFromQueryString, out bool valueAsBool))
				{
					var value = (T)(object)valueAsBool;
					return (true, value);
				}

				throw new NotSupportedException($"Query Paramaters of type {typeof(T).Name} cannot be converted.");
			}

			return default;
		}
	}
}