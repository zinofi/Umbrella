using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Configuration.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.Utilities.Configuration
{
	public abstract class AppSettingsBase : ReadOnlyAppSettingsBase<IAppSettingsSource>
	{
		#region Constructors
		public AppSettingsBase(ILogger logger,
			IMemoryCache cache,
			IAppSettingsSource appSettingsSource,
			IGenericTypeConverter genericTypeConverter)
			: base(logger, cache, appSettingsSource, genericTypeConverter)
		{
		}
		#endregion

		#region Protected Methods
		protected virtual void SetSetting<T>(T value, [CallerMemberName]string key = "")
		{
			try
			{
				Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

				string valueToStore = null;

				if (value != null)
				{
					Type type = typeof(T);
					TypeInfo typeInfo = type.GetTypeInfo();

					if (typeInfo.IsPrimitive || typeInfo.IsEnum || type == typeof(string))
					{
						valueToStore = value.ToString().Trim();

						if (string.IsNullOrEmpty(valueToStore))
							valueToStore = null;
					}
					else
					{
						valueToStore = ToJson(value);
					}
				}

				AppSettingsSource.SetValue(key, valueToStore);
				Cache.Remove(GenerateCacheKey(key));
			}
			catch (Exception exc) when (Log.WriteError(exc))
			{
				throw;
			}
		}

		protected virtual string ToJson(object value) => UmbrellaStatics.SerializeJson(value);
		#endregion
	}
}