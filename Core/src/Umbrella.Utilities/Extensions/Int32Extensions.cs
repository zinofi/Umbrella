using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class Int32Extensions
    {
        public static string ToOrdinal(this int value)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int last_digits = value % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (last_digits < 11 || last_digits > 13)
            {
                // Check the last digit.
                switch (last_digits % 10)
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
    }
}