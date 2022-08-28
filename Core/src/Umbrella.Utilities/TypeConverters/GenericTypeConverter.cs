using System;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.Utilities.TypeConverters
{
	/// <summary>
	/// A custom type converter which converts strings to objects.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.TypeConverters.Abstractions.IGenericTypeConverter" />
	public class GenericTypeConverter : IGenericTypeConverter
	{
		private readonly ILogger _log;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericTypeConverter"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public GenericTypeConverter(ILogger<GenericTypeConverter> logger)
		{
			_log = logger;
		}

		/// <inheritdoc />
		public T Convert<T>(string? value, Func<T> fallbackCreator, Func<string?, T>? customValueConverter = null)
		{
			try
			{
				return GenericTypeConverterHelper.Convert(value, fallbackCreator, customValueConverter);
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value, fallbackCreator, customValueConverter }))
			{
				throw new UmbrellaException("There has been a problem converting the value.", exc);
			}
		}

		/// <inheritdoc />
		public T Convert<T>(string? value, T fallback = default!, Func<string?, T>? customValueConverter = null)
		{
			try
			{
				return GenericTypeConverterHelper.Convert(value, fallback, customValueConverter)!;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value, fallback, customValueConverter }))
			{
				throw new UmbrellaException("There has been a problem converting the value.", exc);
			}
		}

		/// <inheritdoc />
		public T ConvertToEnum<T>(string? value, T fallback = default)
			where T : struct, Enum
		{
			try
			{
				return GenericTypeConverterHelper.ConvertToEnum(value, fallback);
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value }))
			{
				throw new UmbrellaException("There has been a problem converting the value.", exc);
			}
		}
	}
}