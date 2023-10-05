namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for the <see cref="int"/> type.
/// </summary>
public static class Int32Extensions
{
	/// <summary>
	/// Converts the specified enum to an ordinal string for the English language, e.g. 1 to 1st, 3 to 3rd, 5 to 5th, etc.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The ordinal string for the number.</returns>
	public static string ToOrdinalString(this int value)
	{
		// Start with the most common extension.
		string extension = "th";

		// Examine the last 2 digits.
		int lastDigits = value % 100;

		// If the last digits are 11, 12, or 13, use th. Otherwise:
		if (lastDigits < 11 || lastDigits > 13)
		{
			// Check the last digit.
			switch (lastDigits % 10)
			{
				case 1:
					extension = "st";
					break;
				case 2:
					extension = "nd";
					break;
				case 3:
					extension = "rd";
					break;
			}
		}

		return value + extension;
	}

	/// <summary>
	/// Converts the specified integer to its enum value.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The enum value.</returns>
	public static TEnum ToEnum<TEnum>(this int value)
		where TEnum : struct, Enum
		=> value.ToEnum<TEnum>(default);

	/// <summary>
	/// Converts the specified integer to its enum value.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	/// <param name="value">The value.</param>
	/// <param name="defaultValue">The default value when the value isn't defined.</param>
	/// <returns>The enum value.</returns>
	public static TEnum ToEnum<TEnum>(this int value, TEnum defaultValue)
		where TEnum : struct, Enum
	{
		Type eType = typeof(TEnum);

		if (Enum.IsDefined(eType, value))
		{
			object obj = Enum.ToObject(eType, value);
			return (TEnum)obj;
		}

		return defaultValue;
	}
}