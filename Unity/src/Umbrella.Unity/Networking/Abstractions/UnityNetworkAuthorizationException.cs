using System;

namespace Umbrella.Unity.Networking.Abstractions
{
    public class UnityNetworkAuthorizationException : UnityNetworkException
    {
        public UnityNetworkAuthorizationException(string message = null)
            : base(message)
        {
        }

        public UnityNetworkAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}