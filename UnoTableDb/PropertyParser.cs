using System.Reflection;
using UnoTableDb.Core;
using UnoTableDb.Exceptions;

namespace UnoTableDb;

public class PropertyParser<T> where T: BaseModel
{
    private readonly char _separator;
    private string[]? PropertyNames { get; set; }

    public PropertyParser(char valueSeparator)
    {
        _separator = valueSeparator;
    }
    
    public void ParsePropertyNames(string line)
    {
        PropertyNames = line.Split(_separator);
    }

    public T ParseModelFromLine(string line)
    {
        if (line is null)
        {
            throw new ArgumentNullException(nameof(line));
        }
        if (PropertyNames is null)
        {
            throw new InvalidOperationException(
                "Cannot parse, without PropertyNames being set.");
        }

        var values = line.Split(_separator);

        if (PropertyNames.Length != values.Length)
        {
            throw new HeadersAndRecordMismatchedException(
                "The number of _headers and values in a record should be the same.");
        }
        
        // Calling a model's constructor
        var result = Activator.CreateInstance<T>();
        
        for (int i = 0; i < values.Length; i++)
        {
            var propertyInfo = typeof(T).GetProperty(PropertyNames[i])
                               ?? throw new PropertyNotFoundException();

            if (!propertyInfo.CanWrite)
            {
                throw new InvalidModelTypeException("All tracked properties must be writable.");
            }
        
            dynamic value = values[i];
        
            if (propertyInfo.PropertyType != typeof(string))
            {
                // todo: An exception can be thrown if a property is not Parsable
                value = propertyInfo.PropertyType.InvokeMember(
                            "Parse", 
                            BindingFlags.Public | 
                            BindingFlags.Static |
                            BindingFlags.InvokeMethod, null, result, new object[] { values[i] })!;
            }
            propertyInfo.SetValue(result, value);
        }
        return result;
    }
}