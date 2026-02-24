namespace backend.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; set; }

    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class BookNotFoundException : ApiException
{
    public int BookId { get; set; }
    
    public BookNotFoundException(int bookid) : base($"Boken med ID {bookid} kunde ej hittas", StatusCodes.Status404NotFound)
        {
        BookId = bookid;
        }
}