using System.Reflection;
using UnoTableDb.Core;
using UnoTableDb.Exceptions;
using UnoTableDb.Interfaces;

namespace UnoTableDb;

public class PropertyParser<T> : IPropertyParser<T> where T: BaseModel
{
    private readonly char _separator;
    private string[]? PropertyNames { get; set; }

    public PropertyParser(char valueSeparator)
    {
        _separator = valueSeparator;
    }
    
    public bool TryParsePropertyNames(string? line)
    {
        if (line is null) return false;
        PropertyNames = line.Split(_separator);
        return true;
    }

    public bool TryParseModelFromLine(string? line, out T? model)
    {
        if (PropertyNames is null)
        {
            throw new InvalidOperationException("Cannot parse, without PropertyNames being set.");
        }
        
        model = null;
        
        if (string.IsNullOrEmpty(line)) return false;
        
        var values = line.Split(_separator);

        if (PropertyNames.Length != values.Length)
        {
            return false;
        }
        
        // Calling a model's constructor
        var result = Activator.CreateInstance<T>();
        
        for (int i = 0; i < values.Length; i++)
        {
            var propertyInfo = typeof(T).GetProperty(PropertyNames[i]);

            if (propertyInfo is null || !propertyInfo.CanWrite)
            {
                return false;
            }

            dynamic value = values[i];
        
            if (propertyInfo.PropertyType != typeof(string))
            {
                try
                {
                    value = propertyInfo.PropertyType.InvokeMember(
                        "Parse",
                        BindingFlags.Public |
                        BindingFlags.Static |
                        BindingFlags.InvokeMethod, null, result, new object[] { values[i] })!;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            propertyInfo.SetValue(result, value);
        }
        
        model = result;
        return true;
    }
    
    internal T ParseModelFromLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            throw new InvalidOperationException("The line to be parsed should not be null or empty.");
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
                "The number of headers and values in a record should be the same.");
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
                try
                {
                    value = propertyInfo.PropertyType.InvokeMember(
                        "Parse", 
                        BindingFlags.Public | 
                        BindingFlags.Static |
                        BindingFlags.InvokeMethod, null, result, new object[] { values[i] })!;
                }
                catch (MissingMethodException)
                {
                    throw new InvalidModelTypeException("The types of properties tracked should implement IParsable<> interface.");
                }
                catch (TargetInvocationException)
                {
                    throw new FormatException($"Cannot parse \"{values[i]}\" to {propertyInfo.PropertyType.Name}");
                }
            }
            propertyInfo.SetValue(result, value);
        }
        return result;
    }
}