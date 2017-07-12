using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        #region Public Static Methods
        public static string ToJsonString(this object value, bool useCamelCasingRules = false)
            => UmbrellaStatics.JsonSerializer(value, useCamelCasingRules);
        #endregion
    }
}
