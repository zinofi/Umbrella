using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Extensions
{
	/// <summary>
	/// Blazor specific extension method
	/// </summary>
	public static class StringExtensions
	{
		private static readonly string _encodedNewLineToken;

		static StringExtensions()
		{
			_encodedNewLineToken = HtmlEncoder.Default.Encode("\n");
		}

		public static MarkupString ReplaceNewLine(this string? value, string replacement = "<br />")
		{
			if (string.IsNullOrWhiteSpace(value))
				return default;

			return (MarkupString)HtmlEncoder.Default.Encode(value).Replace(_encodedNewLineToken, replacement);
		}
	}
}