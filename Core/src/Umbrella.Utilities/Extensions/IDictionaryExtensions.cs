using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Primitives;
#if NET6_0_OR_GREATER
using System.Globalization;
#endif
using System.Text;
using System.Text.Encodings.Web;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for the <see cref="IDictionary{TKey, TValue}"/> interface.
/// </summary>
public static class IDictionaryExtensions
{
	/// <summary>
	/// Converts the dictionary to a query string.
	/// </summary>
	/// <param name="dictionary">The dictionary to convert.</param>
	/// <param name="prependQueryStart">True to prepend the query start character (?), otherwise false.</param>
	/// <returns>The query string.</returns>
	public static string ToQueryString(this IDictionary<string, StringValues> dictionary, bool prependQueryStart = false)
	{
		Guard.IsNotNull(dictionary);

		StringBuilder queryBuilder = prependQueryStart ? new("?") : new();
		bool firstParameter = true;

		foreach (var parameter in dictionary)
		{
			foreach (string? value in parameter.Value)
			{
				if (string.IsNullOrEmpty(value))
					continue;

				if (!firstParameter)
				{
					_ = queryBuilder.Append('&');
				}
				else
				{
					firstParameter = false;
				}

				// Use HttpUtility.UrlEncode to ensure the parameter names and values are properly escaped
#if NET6_0_OR_GREATER
				_ = queryBuilder.Append(CultureInfo.InvariantCulture, $"{UrlEncoder.Default.Encode(parameter.Key)}={UrlEncoder.Default.Encode(value)}");
#else
				_ = queryBuilder.Append($"{UrlEncoder.Default.Encode(parameter.Key)}={UrlEncoder.Default.Encode(value)}");
#endif
			}
		}

		return queryBuilder.ToString();
	}
}