namespace UnoTableDb.Interfaces;

public interface IPropertyParser<T>
{
    bool TryParsePropertyNames(string? line);
    bool TryParseModelFromLine(string? line, out T? model);
}