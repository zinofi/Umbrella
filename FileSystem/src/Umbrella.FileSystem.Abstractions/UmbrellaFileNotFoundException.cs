using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.FileSystem.Abstractions
{
    public class UmbrellaFileNotFoundException : Exception
    {
        public string Subpath { get; }

        public UmbrellaFileNotFoundException(string subpath)
            :base($"The file located at {subpath} could not be found.")
        {
            Subpath = subpath;
        }

        public UmbrellaFileNotFoundException(string subpath, Exception innerException)
            :base($"The file located at {subpath} could not be found.", innerException)
        {
            Subpath = subpath;
        }
    }
}