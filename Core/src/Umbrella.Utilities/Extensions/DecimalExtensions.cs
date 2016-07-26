using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
	public static class DecimalExtensions
	{
        public static string ToFriendlyString(this decimal value, string format, string valueIfZero = "") => value > 0 ? value.ToString(format) : valueIfZero;

        public static string ToFriendlyString(this decimal? value, string format, string valueIfNull = "") => value.HasValue ? ToFriendlyString(value.Value, format, valueIfNull) : valueIfNull;
    }
}