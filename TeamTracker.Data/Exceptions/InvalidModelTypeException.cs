namespace TeamTracker.Data.Exceptions;

public class InvalidModelTypeException : Exception
{
    public InvalidModelTypeException() { }

    public InvalidModelTypeException(string message) : base(message) { }
    
    public InvalidModelTypeException(string message, Exception inner) : base(message, inner) { }
}