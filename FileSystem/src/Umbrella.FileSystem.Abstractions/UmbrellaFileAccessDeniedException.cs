using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
    public class UmbrellaFileAccessDeniedException : Exception
    {
        public string Subpath { get; }

        public UmbrellaFileAccessDeniedException(string subpath)
            : base($"Access to the file located at {subpath} has been denied.")
        {
            Subpath = subpath;
        }

        public UmbrellaFileAccessDeniedException(string subpath, Exception innerException)
            : base($"Access to the file located at {subpath} has been denied.", innerException)
        {
            Subpath = subpath;
        }
    }
}