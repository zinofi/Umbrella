using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    public class HybridCacheException : Exception
    {
        public HybridCacheException()
        {
        }

        public HybridCacheException(string message)
            : base(message)
        {
        }

        public HybridCacheException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected HybridCacheException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}