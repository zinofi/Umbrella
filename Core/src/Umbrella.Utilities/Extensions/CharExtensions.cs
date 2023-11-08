// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for the <see cref="char"/> type.
/// </summary>
public static class CharExtensions
{
	/// <summary>
	/// Remaps the international character to ASCII.
	/// </summary>
	/// <param name="character">The character.</param>
	/// <returns>The ASCII equivalent.</returns>
	/// <exception cref="ArgumentOutOfRangeException">character - The specified character must be greater than the unicode character 128.</exception>
	public static string RemapInternationalCharacterToAscii(this char character)
	{
		if (character < 128)
			throw new ArgumentOutOfRangeException(nameof(character), "The specified character must be greater than the unicode character 128.");

		char s = char.ToLowerInvariant(character);

		if ("àåáâäãåą".Contains(s))
		{
			return "a";
		}
		else if ("èéêëę".Contains(s))
		{
			return "e";
		}
		else if ("ìíîïı".Contains(s))
		{
			return "i";
		}
		else if ("òóôõöøőð".Contains(s))
		{
			return "o";
		}
		else if ("ùúûüŭů".Contains(s))
		{
			return "u";
		}
		else if ("çćčĉ".Contains(s))
		{
			return "c";
		}
		else if ("żźž".Contains(s))
		{
			return "z";
		}
		else if ("śşšŝ".Contains(s))
		{
			return "s";
		}
		else if ("ñń".Contains(s))
		{
			return "n";
		}
		else if ("ýÿ".Contains(s))
		{
			return "y";
		}
		else if ("ğĝ".Contains(s))
		{
			return "g";
		}
		else if (character is 'ř')
		{
			return "r";
		}
		else if (character is 'ł')
		{
			return "l";
		}
		else if (character is 'đ')
		{
			return "d";
		}
		else if (character is 'ß')
		{
			return "ss";
		}
		else if (character is 'þ')
		{
			return "th";
		}
		else if (character is 'ĥ')
		{
			return "h";
		}
		else if (character is 'ĵ')
		{
			return "j";
		}
		else
		{
			return "";
		}
	}
}