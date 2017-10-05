using System;

namespace Umbrella.Unity.Networking.Abstractions
{
    public class UnityNetworkException : Exception
    {
        public UnityNetworkException(string message = null)
            : base(message)
        {
        }

        public UnityNetworkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}