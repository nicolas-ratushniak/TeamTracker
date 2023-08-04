using System.Reflection;
using TeamTracker.Wpf.Data.Attributes;
using TeamTracker.Wpf.Data.Exceptions;

namespace TeamTracker.Wpf.Core;

public abstract class BaseModel
{
    private static readonly string Separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
    private readonly IEnumerable<PropertyInfo> _properties;

    [Key]
    public Guid Id { get; init; }
    
    [NotTracked]
    public ModelState ModelState { get; set; }

    protected BaseModel()
    {
        _properties = GetTrackedProperties(GetType());
    }

    public static T ParseFromDbRecord<T>(string? record)  where T : BaseModel
    {
        if (string.IsNullOrEmpty(record))
        {
            throw new InvalidOperationException("The record to be parsed should not be null or empty.");
        }

        var values = record.Split(Separator);
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
                        BindingFlags.InvokeMethod, null, result, null)!;
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
    
    public static bool TryParseFromDbRecord<T>(string? record, out T result) where T : BaseModel
    {
        result = default!;
        
        try
        {
            result = ParseFromDbRecord<T>(record);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public string ToDbRecord()
    {
        return string.Join(Separator, _properties.Select(p => p.GetValue(this)));
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