using System.Globalization;

namespace Umbrella.Utilities.TypeConverters;

public static class GenericTypeConverterHelper
{
	public static T Convert<T>(string? value, Func<T> fallbackCreator, Func<string?, T>? customValueConverter = null, CultureInfo? cultureInfo = null)
	{
		Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

		if (!string.IsNullOrEmpty(value))
			return customValueConverter is not null ? customValueConverter(value) : (T)System.Convert.ChangeType(value, type, cultureInfo ?? CultureInfo.CurrentCulture);

		T? fallback = fallbackCreator is not null ? fallbackCreator() : default;

		return type == typeof(string) && fallback is null
			? (T)System.Convert.ChangeType(string.Empty, type, cultureInfo ?? CultureInfo.CurrentCulture)
			: fallback!;
	}

	public static T Convert<T>(string? value, T fallback = default!, Func<string?, T>? customValueConverter = null) => Convert(value, () => fallback, customValueConverter)!;

	public static T ConvertToEnum<T>(string? value, T fallback = default)
		where T : struct, Enum
	{
		if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out T output))
			return output;

		return fallback;
	}

	public static T? ConvertToNullableEnum<T>(string? value, T? fallback = null)
		where T : struct, Enum
	{
		if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out T output))
			return output;

		return fallback;
	}
}