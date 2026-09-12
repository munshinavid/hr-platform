namespace EmployeeManagement.Aggregator.Exceptions;

public class DomainException : System.Exception
{
    public DomainException(string message)
        : base(message)
    {
    }
}