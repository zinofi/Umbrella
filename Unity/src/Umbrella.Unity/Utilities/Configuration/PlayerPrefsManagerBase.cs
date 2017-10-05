using Microsoft.Extensions.Caching.Memory;
using Umbrella.Utilities.Configuration;
using Umbrella.Utilities;

namespace Umbrella.Unity.Utilities.Configuration
{
    public abstract class PlayerPrefsManagerBase : AppSettingsBase
    {
        public PlayerPrefsManagerBase(Microsoft.Extensions.Logging.ILogger logger,
            IMemoryCache cache,
            IAppSettingsSource appSettingsSource)
            : base(logger, cache, appSettingsSource)
        {
        }

        protected override T FromJson<T>(string value)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            return UmbrellaStatics.DeserializeJson<T>(value);
        }

        protected override string ToJson(object value)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            return UmbrellaStatics.SerializeJson(value);
        }
    }
}