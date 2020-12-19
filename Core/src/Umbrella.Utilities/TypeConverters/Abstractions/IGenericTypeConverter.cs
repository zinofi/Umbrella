using System;

namespace Umbrella.Utilities.TypeConverters.Abstractions
{
	/// <summary>
	/// A custom type converter which converts strings to objects.
	/// </summary>
	public interface IGenericTypeConverter
	{
		/// <summary>
		/// Converts the specified value to an instance of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="fallbackCreator">
		/// The fallback creator.
		/// This is used to provide a default value in cases where the <paramref name="value"/> is null or empty.
		/// In cases where no fallback has been specified, the <see langword="default"/> value for <typeparamref name="T"/> will be returned EXCEPT if <typeparamref name="T"/>
		/// is a <see langword="string"/> in which case <see cref="string.Empty"/> will be returned.
		/// </param>
		/// <param name="customValueConverter">The custom value converter.</param>
		/// <returns>The conversion result.</returns>
		T Convert<T>(string? value, Func<T> fallbackCreator, Func<string?, T>? customValueConverter = null);

		/// <summary>
		/// Converts the specified value to an instance of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="fallback">
		/// The fallback. This is used to provide a default value in cases where the <paramref name="value"/> is null or empty.
		/// In cases where no fallback has been specified, the <see langword="default"/> value for <typeparamref name="T"/> will be returned EXCEPT if <typeparamref name="T"/>
		/// is a <see langword="string"/> in which case <see cref="string.Empty"/> will be returned.
		/// </param>
		/// <param name="customValueConverter">The custom value converter.</param>
		/// <returns>The conversion result.</returns>
		T Convert<T>(string? value, T fallback = default, Func<string?, T>? customValueConverter = null);

		/// <summary>
		/// Converts the specified <paramref name="value"/> to an <see langword="enum"/> instance of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The <see langword="enum"/></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="fallback">The fallback which is returned if the value is null, empty or whitespace, or if the value cannot be converted to the specified enum <typeparamref name="T"/>.</param>
		/// <returns>The conversion result.</returns>
		T ConvertToEnum<T>(string? value, T fallback = default) where T : struct, Enum;
	}
}