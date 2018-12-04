using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Caching
{
    public class CacheKeyUtility : ICacheKeyUtility
    {
        public string Create<T>(IList<string> keyParts, [CallerMemberName]string callerMemberName = "")
        {
            Guard.ArgumentNotNullOrEmpty(keyParts, nameof(keyParts));
            Guard.ArgumentNotNullOrEmpty(callerMemberName, nameof(callerMemberName));

            int partsCount = keyParts.Count();
            string typeName = typeof(T).FullName;
            int partsLengthTotal = -1;

            for (int i = 0; i < partsCount; i++)
            {
                string part = keyParts[i];

                if (part != null)
                    partsLengthTotal += part.Length + 1;
            }

            int length = typeName.Length + callerMemberName.Length + partsLengthTotal + 2;

            Span<char> span = stackalloc char[length];

            int currentIndex = span.Append(0, typeName);
            span[currentIndex++] = ':';

            currentIndex = span.Append(currentIndex, callerMemberName);
            span[currentIndex++] = ':';

            for (int i = 0; i < partsCount; i++)
            {
                string part = keyParts[i];

                if (part != null)
                {
                    currentIndex = span.Append(currentIndex, part);

                    if (i < partsCount - 1)
                        span[currentIndex++] = ':';
                }
            }

            ReadOnlySpan<char> readOnlySpan = span;
            readOnlySpan.ToUpperInvariant(span);

            return span.ToString();
        }

        public string CreateOld<T>(IEnumerable<string> keyParts, [CallerMemberName]string callerMemberName = "")
        {
            Guard.ArgumentNotNullOrEmpty(keyParts, nameof(keyParts));
            Guard.ArgumentNotNullOrEmpty(callerMemberName, nameof(callerMemberName));

            return $"{typeof(T).FullName}:{callerMemberName}:{string.Join(":", keyParts)}".ToUpperInvariant();
        }
    }
}