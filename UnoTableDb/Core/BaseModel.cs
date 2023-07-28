using System.Globalization;
using System.Reflection;
using UnoTableDb.Attributes;
using UnoTableDb.Exceptions;
using UnoTableDb.Interfaces;

namespace UnoTableDb.Core;

public abstract class BaseModel : IFormattable, IModelParser
{
    private static readonly CultureInfo DefaultCulture = new("en-US");
    private readonly IEnumerable<PropertyInfo> _properties;

    [Key]
    public Guid Id { get; init; }
    
    [NotTracked]
    public ModelState ModelState { get; set; }

    protected BaseModel()
    {
        _properties = GetTrackedProperties(GetType());
    }

    public static T Parse<T>(string? s)  where T : BaseModel
    {
        return Parse<T>(s, DefaultCulture);
    }
    
    public static T Parse<T>(string? s, IFormatProvider? provider)  where T : BaseModel
    {
        if (string.IsNullOrEmpty(s))
        {
            throw new InvalidOperationException("The line to be parsed should not be null or empty.");
        }
        
        provider ??= DefaultCulture;

        string separator = provider is CultureInfo culture
            ? culture.TextInfo.ListSeparator
            : DefaultCulture.TextInfo.ListSeparator;
        
        var values = s.Split(separator);
        var propertyNames = GetTrackedProperties(typeof(T))
            .Select(p => p.Name).ToList();
        
        var result = Activator.CreateInstance<T>();
        
        for (int i = 0; i < values.Length; i++)
        {
            var propertyInfo = typeof(T).GetProperty(propertyNames[i])
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
                    // Calling a Parse method
                    value = propertyInfo.PropertyType.InvokeMember(
                        "Parse",
                        BindingFlags.Public |
                        BindingFlags.Static |
                        BindingFlags.InvokeMethod, null, result, new object[] { values[i], provider })!;
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

    public static bool TryParse<T>(string? s, out T result)  where T : BaseModel
    {
        return TryParse(s, DefaultCulture, out result);
    }

    public static bool TryParse<T>(string? s, IFormatProvider? provider, out T result) where T : BaseModel
    {
        result = default;
        
        try
        {
            result = Parse<T>(s, provider);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return ToString(null, DefaultCulture);
    }

    // Honestly, the format here is for now unusable...
    // And the formatProvider is considered as CultureInfo
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        formatProvider ??= DefaultCulture;

        string separator = formatProvider is CultureInfo culture
            ? culture.TextInfo.ListSeparator
            : DefaultCulture.TextInfo.ListSeparator;

        return string.Join(separator, _properties.Select(p => p.GetValue(this)));
    }
    
    /// <summary>
    /// Checks whether a model class is valid
    /// </summary>
    /// <param name="modelType"> The type of the model class</param>
    /// <remarks>Model is considered valid if:<br />
    /// 1. It derives from a BaseModel class <br />
    /// 2. Only one tracked property with [PrimaryKey] <br />
    /// 3. All tracked properties should have public get, set <br />
    /// 4. The types of properties tracked should implement IParsable</remarks>
    internal static bool IsModelTypeValid(Type modelType)
    {
        if (!modelType.IsSubclassOf(typeof(BaseModel)))
        {
            return false;
        }

        var propertyList = modelType.GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute))).ToList();

        if (propertyList.Count(p => Attribute.IsDefined(p, typeof(KeyAttribute))) != 1)
        {
            return false;
        }

        if (!propertyList
            .All(p => p.CanRead && p.CanWrite && (p.PropertyType == typeof(string) || 
                                                  typeof(IParsable<>).IsAssignableFrom(p.PropertyType))))
        {
            return false;
        }

        return true;
    }

    private static IEnumerable<PropertyInfo> GetTrackedProperties(Type modelType) 
    {
        return modelType.GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)));
    }
}