using UnoTableDb.Core;

namespace UnoTableDb.Interfaces;

public interface IModelParser
{
    public static abstract T Parse<T>(string? s, IFormatProvider? provider) where T : BaseModel;
    public static abstract bool TryParse<T>(string? s, IFormatProvider? provider, out T result) where T : BaseModel;
}