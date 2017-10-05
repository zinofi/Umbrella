using System;

namespace Umbrella.Unity.Utilities.Validation
{
    public class UnityValidationException : Exception
    {
        public UnityValidationException()
            : base()
        {
        }

        public UnityValidationException(string message)
            : base(message)
        {
        }

        public UnityValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}