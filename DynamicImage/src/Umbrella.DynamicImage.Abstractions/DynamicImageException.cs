using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Abstractions
{
    public class DynamicImageException : Exception
    {
        public DynamicImageOptions Options { get; }

        public DynamicImageException()
        {
        }

        public DynamicImageException(string message, DynamicImageOptions options = default(DynamicImageOptions))
            : base(message)
        {
            Options = options;
        }

        public DynamicImageException(string message, Exception innerException, DynamicImageOptions options = default(DynamicImageOptions))
            : base(message, innerException)
        {
            Options = options;
        }
    }
}