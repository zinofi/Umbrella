using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class Int32Extensions
    {
        public static string ToOrdinalString(this int value)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int lastDigits = value % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (lastDigits < 11 || lastDigits > 13)
            {
                // Check the last digit.
                switch (lastDigits % 10)
                {
                    case 1:
                        extension = "st";
                        break;
                    case 2:
                        extension = "nd";
                        break;
                    case 3:
                        extension = "rd";
                        break;
                }
            }

            return value + extension;
        }

        public static T ToEnum<T>(this int value) => value.ToEnum<T>(default);

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