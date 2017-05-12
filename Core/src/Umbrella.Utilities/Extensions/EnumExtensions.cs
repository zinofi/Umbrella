using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.Utilities.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string value) where T : struct => value.ToEnum(default(T));

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (Enum.TryParse(value, true, out T result))
                return result;

            return defaultValue;
        }

        public static T ToEnum<T>(this int value) => value.ToEnum<T>(default(T));

        public static T ToEnum<T>(this int value, T defaultValue)
        {
            Type eType = typeof(T);

            if (Enum.IsDefined(eType, value))
            {
                object obj = Enum.ToObject(eType, value);
                return (T)obj;
            }

            return defaultValue;
        }
    }
}