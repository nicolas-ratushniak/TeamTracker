namespace UnoTableDb.Interfaces;

public interface IParsableGeneric
{
    public static abstract T Parse<T>(string? s, IFormatProvider? provider);
    public static abstract bool TryParse<T>(string? s, IFormatProvider? provider, out T result);
}