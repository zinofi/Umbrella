using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Extensions
{
	/// <summary>
	/// Blazor specific extension methods for use with strings.
	/// </summary>
	public static class StringExtensions
	{
		private static readonly string _encodedNewLineToken;

		static StringExtensions()
		{
			_encodedNewLineToken = HtmlEncoder.Default.Encode("\n");
		}

		/// <summary>
		/// Encodes the specified <paramref name="value"/> as HTML and then replaces all encoded '\n' new line characters with the specified <paramref name="replacement"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="replacement">The replacement.</param>
		/// <returns>The HTML encoded output.</returns>
		public static MarkupString ReplaceNewLine(this string? value, string replacement = "<br />")
		{
			if (string.IsNullOrWhiteSpace(value))
				return default;

			return (MarkupString)HtmlEncoder.Default.Encode(value).Replace(_encodedNewLineToken, replacement);
		}
	}
}