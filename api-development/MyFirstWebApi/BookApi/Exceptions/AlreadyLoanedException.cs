namespace MyFirstWebApi.Exceptions;

public class AlreadyLoanedException : Exception
{
    public AlreadyLoanedException(string message) : base(message) { }
}