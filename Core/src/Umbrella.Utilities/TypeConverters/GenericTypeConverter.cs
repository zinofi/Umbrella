using System;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.Utilities.TypeConverters
{
	public class GenericTypeConverter : IGenericTypeConverter
	{
		private readonly ILogger _log;

		public GenericTypeConverter(ILogger logger)
		{
			_log = logger;
		}

		public T Convert<T>(string value, Func<T> fallbackCreator, Func<string, T> customValueConverter = null)
		{
			Guard.ArgumentNotNull(fallbackCreator, nameof(fallbackCreator));

			try
			{
				return Convert<T>(value, fallbackCreator(), customValueConverter);
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value, fallbackCreator, customValueConverter }))
			{
				throw new UmbrellaException("There has been a problem converting the value.", exc);
			}
		}

		public T Convert<T>(string value, T fallback = default, Func<string, T> customValueConverter = null)
		{
			try
			{
				var type = typeof(T);

				if (!string.IsNullOrEmpty(value))
					return customValueConverter != null ? customValueConverter(value) : (T)System.Convert.ChangeType(value, type);

				return type == typeof(string) && fallback == null
					? (T)System.Convert.ChangeType(string.Empty, type)
					: fallback;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value, fallback, customValueConverter }))
			{
				throw new UmbrellaException("There has been a problem converting the value.", exc);
			}
		}

		public T ConvertToEnum<T>(string value, T fallback = default)
			where T : struct, Enum
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(value) && typeof(Enum).IsAssignableFrom(typeof(T)) && Enum.TryParse(value, true, out T output))
					return output;

				return fallback;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem converting the value.", exc);
			}
		}
	}
}