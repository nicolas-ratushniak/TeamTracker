using System.Reflection;
using UnoTableDb.Core;
using UnoTableDb.Exceptions;

namespace UnoTableDb;

public class SimpleParser<T> where T: BaseModel
{
    private readonly char _separator;
    public string[]? PropertyNames { get; set; }

    public SimpleParser(char valueSeparator)
    {
        _separator = valueSeparator;
    }
    
    public static string[] ParseHeader(char separator, string line)
    {
        return line.Split(separator);
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
        
        // Calling a model's constructor with no Id generated
        T result = (T)Activator.CreateInstance(typeof(T), Guid.Empty)!;
        

        for (int i = 0; i < values.Length; i++)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(PropertyNames[i])
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