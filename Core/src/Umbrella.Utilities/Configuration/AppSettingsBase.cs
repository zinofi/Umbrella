using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Configuration
{
    public abstract class AppSettingsBase : ReadOnlyAppSettingsBase<IAppSettingsSource>
    {
        #region Constructors
        public AppSettingsBase(ILogger logger,
            IMemoryCache cache,
            IAppSettingsSource appSettingsSource)
            : base(logger, cache, appSettingsSource)
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
                    var type = typeof(T);
                    var typeInfo = type.GetTypeInfo();

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
        #endregion
    }
}