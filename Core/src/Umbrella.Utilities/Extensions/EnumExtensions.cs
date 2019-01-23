using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.Utilities.Extensions
{
    /// <summary>
    /// A set of extension methods for <see cref="Enum"/> types.
    /// </summary>
    public static class EnumExtensions
    {
        // TODO: Add some internal caching to avoid repeatedly doing the same work.
        public static string ToFlagsString(this Enum options, Func<string, string> valueTransformer = null, string separator = ",")
        {
            Guard.ArgumentNotNull(separator, nameof(separator));

            List<string> lstOption = new List<string>();

            foreach (Enum item in Enum.GetValues(options.GetType()))
            {
                if (options.HasFlag(item))
                    lstOption.Add(valueTransformer != null ? valueTransformer(item.ToString()) : item.ToString());
            }

            return string.Join(separator, lstOption);
        }
    }
}