using UnoTableDb.Attributes;

namespace UnoTableDb.Core;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; internal set; }
    
    [NotTracked]
    public ModelState ModelState { get; protected set; }
    
    /// <summary>
    /// Checks whether a model class is valid for proper parsing
    /// </summary>
    /// <param name="modelType"> The type of the model class</param>
    /// <remarks>Model is considered valid if:<br />
    /// 1. It derives from a BaseModel class <br />
    /// 2. The types of properties tracked should implement IParsable <br />
    /// 3. All tracked properties should have public get, set <br />
    /// 4. Only one property with [PrimaryKey] which hasn't got a [NotTracking]</remarks>
    public static bool IsModelTypeValid(Type modelType)
    {
        if (!modelType.IsSubclassOf(typeof(BaseModel)))
        {
            return false;
        }
        
        var properties = modelType.GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
            .ToList();

        if (properties.Count(p => Attribute.IsDefined(p, typeof(KeyAttribute))) != 1)
        {
            return false;
        }

        return properties.All(p => 
            p.CanRead && p.CanWrite && (p.PropertyType == typeof(string) || typeof(IParsable<>).IsAssignableFrom(p.PropertyType)));
    }
}