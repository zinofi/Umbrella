using System;

namespace Umbrella.Utilities.TypeConverters
{
	internal static class GenericTypeConverterHelper
	{
		public static T Convert<T>(string? value, Func<T> fallbackCreator, Func<string?, T>? customValueConverter = null)
		{
			Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

			if (!string.IsNullOrEmpty(value))
				return customValueConverter != null ? customValueConverter(value) : (T)System.Convert.ChangeType(value, type);

			T fallback = fallbackCreator != null ? fallbackCreator() : default;

			return type == typeof(string) && fallback is null
				? (T)System.Convert.ChangeType(string.Empty, type)
				: fallback!;
		}

		public static T Convert<T>(string? value, T fallback = default!, Func<string?, T>? customValueConverter = null)
		{
			return Convert(value, () => fallback, customValueConverter)!;
		}

		public static T ConvertToEnum<T>(string? value, T fallback = default)
			where T : struct, Enum
		{
			if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out T output))
				return output;

			return fallback;
		}
	}
}