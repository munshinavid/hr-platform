using System;

namespace LeaveManagement.Aggregator.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
