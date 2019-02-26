using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    // TODO: Switch internally to use Span<T> methods.
    public static class HashSetExtensions
    {
        public static bool AddNotNull<T>(this HashSet<T> hashSet, T value)
        {
            if (value != null)
                return hashSet.Add(value);

            return false;
        }

        public static bool AddNotNullTrim(this HashSet<string> hashSet, string value)
            => AddNotNull(hashSet, value?.Trim());

        public static bool AddNotNullTrimToLowerInvariant(this HashSet<string> hashSet, string value)
            => AddNotNull(hashSet, value?.TrimToLowerInvariant());
    }
}