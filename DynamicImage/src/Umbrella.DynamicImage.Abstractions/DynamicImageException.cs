using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DynamicImage.Abstractions
{
    public class DynamicImageException : UmbrellaException
    {
        public DynamicImageOptions Options { get; }

        public DynamicImageException()
        {
        }

        public DynamicImageException(string message, DynamicImageOptions options = default)
            : base(message)
        {
            Options = options;
        }

        public DynamicImageException(string message, Exception innerException, DynamicImageOptions options = default)
            : base(message, innerException)
        {
            Options = options;
        }

        public DynamicImageException(string message, Exception innerException, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
            : base(message, innerException)
        {
            Options = new DynamicImageOptions("", width, height, resizeMode, format);
        }
    }
}