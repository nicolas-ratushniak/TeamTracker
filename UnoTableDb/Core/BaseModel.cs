using UnoTableDb.Attributes;

namespace UnoTableDb.Core;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }
    
    [NotTracked]
    public ModelState ModelState { get; protected set; }
    
    protected BaseModel()
    {
        Id = Guid.NewGuid();
    }
    
    protected internal BaseModel(Guid id)
    {
        Id = id;
    }
    
    /// <summary>
    /// Checks whether a model class is valid for proper parsing
    /// </summary>
    /// <param name="modelType"> The type of the model class</param>
    /// <remarks>Model is considered valid if:<br />
    /// 1. It derives from a BaseModel class <br />
    /// 2. It has a one-parameter constructor with Guid parameter <br />
    /// 3. The types of properties tracked should implement IParsable <br />
    /// 4. All tracked properties should have public get, set <br />
    /// 5. Only one property with [PrimaryKey] which hasn't got a [NotTracking]</remarks>
    public static bool IsModelTypeValid(Type modelType)
    {
        if (!modelType.IsSubclassOf(typeof(BaseModel)))
        {
            return false;
        }
        
        if (!modelType.GetConstructors().
            Select(c => c.GetParameters())
            .Any(ps => ps.Length == 1 && ps[0].ParameterType == typeof(Guid)))
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
        
        if (!properties.All(p => p.CanRead && p.CanWrite))
        {
            return false;
        }

        return properties.Select(p => p.PropertyType)
            .Where(pt => pt != typeof(string))
            .All(pt => typeof(IParsable<>).IsAssignableFrom(pt));
    }
}