using System;

namespace HRPlatform.Shared.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

