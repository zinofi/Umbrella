using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.FileSystem.Abstractions
{
    public class UmbrellaFileSystemException : Exception
    {
        public UmbrellaFileSystemException(string message)
            :base(message)
        {
        }

        public UmbrellaFileSystemException(string message, Exception innerException)
            :base(message, innerException)
        {
        }
    }
}