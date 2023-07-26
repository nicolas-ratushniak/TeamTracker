namespace UnoTableDb.Interfaces;

public interface IPropertyFormatter<T>
{
    string FormatPropertyNames(Type type);
    string FormatProperties(T instance);
}