using System.Globalization;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for use with <see cref="decimal"/> instances.
/// </summary>
public static class DecimalExtensions
{
	/// <summary>
	/// Converts the specified <paramref name="value"/> to a string using the specified <paramref name="format"/>.
	/// If the value is zero, the string specified by <paramref name="valueIfZero"/> is returned.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="format">The format.</param>
	/// <param name="valueIfZero">The value if zero.</param>
	/// <param name="cultureInfo">The culture info.</param>
	/// <returns>The result.</returns>
	/// <remarks>If <paramref name="cultureInfo"/> is not specified, <see cref="CultureInfo.CurrentCulture"/> will be used.</remarks>
	public static string ToFriendlyString(this decimal value, string format, string valueIfZero = "", CultureInfo? cultureInfo = null) => value != 0 ? value.ToString(format, cultureInfo ?? CultureInfo.CurrentCulture) : valueIfZero;

	/// <summary>
	/// Converts the specified <paramref name="value"/> to a string using the specified <paramref name="format"/>.
	/// If the value is null, the string specified by <paramref name="valueIfNull"/> is returned.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="format">The format.</param>
	/// <param name="valueIfNull">The value if null.</param>
	/// <param name="cultureInfo"></param>
	/// <returns>The result.</returns>
	/// <remarks>If <paramref name="cultureInfo"/> is not specified, <see cref="CultureInfo.CurrentCulture"/> will be used.</remarks>
	public static string ToFriendlyString(this decimal? value, string format, string valueIfNull = "", CultureInfo? cultureInfo = null) => value.HasValue ? ToFriendlyString(value.Value, format, valueIfNull, cultureInfo) : valueIfNull;
}