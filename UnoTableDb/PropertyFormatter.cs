using UnoTableDb.Attributes;
using UnoTableDb.Core;
using UnoTableDb.Interfaces;

namespace UnoTableDb;

public class PropertyFormatter<T> : IPropertyFormatter<T> where T : BaseModel
{
    private readonly char _separator;

    public PropertyFormatter(char separator)
    {
        _separator = separator;
    }

    public string FormatPropertyNames(Type type)
    {
        var propNames = type.GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
            .Select(p => p.Name);

        return string.Join(_separator, propNames);
    }

    public string FormatProperties(T instance)
    {
        var propValues = typeof(T).GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
            .Select(p => p.GetValue(instance));

        return string.Join(_separator, propValues);
    }
}