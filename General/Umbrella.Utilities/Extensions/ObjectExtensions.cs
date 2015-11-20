using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        #region Private Static Members
        private static readonly JsonSerializerSettings s_CamelCaseJsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        #endregion

        #region Public Static Methods
        public static string ToJsonString(this object value, bool useCamelCasingRules = false)
        {
            return useCamelCasingRules
                ? JsonConvert.SerializeObject(value)
                : JsonConvert.SerializeObject(value, s_CamelCaseJsonSerializerSettings);
        }
        #endregion
    }
}
