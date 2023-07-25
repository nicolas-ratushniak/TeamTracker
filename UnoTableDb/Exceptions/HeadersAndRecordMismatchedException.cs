namespace UnoTableDb.Exceptions;

public class HeadersAndRecordMismatchedException : Exception
{
    public HeadersAndRecordMismatchedException() { }

    public HeadersAndRecordMismatchedException(string message) : base(message) { }
    
    public HeadersAndRecordMismatchedException(string message, Exception inner) : base(message, inner) { }
}