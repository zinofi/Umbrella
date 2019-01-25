using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    public class MultiCacheException : Exception
    {
        public MultiCacheException()
        {
        }

        public MultiCacheException(string message)
            : base(message)
        {
        }

        public MultiCacheException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MultiCacheException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}