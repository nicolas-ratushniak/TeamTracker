using UnoTableDb.Attributes;

namespace UnoTableDb.Core;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }
    
    [NotTracked]
    public ModelState ModelState { get; protected set; }
    
    protected BaseModel(bool generateId)
    {
        if (generateId)
        {
            Id = Guid.NewGuid();
        }
    }

    public static bool IsModelTypeValid(Type modelType)
    {
        // Model is considered valid if:
        // * There should be a one-parameter constructor with bool parameter "generateId"
        // TODO: * All All tracked properties should hold parsable value (Implement IParsable<T>)
        // * All tracked properties should have public get and set
        // * Only one property with [PrimaryKey] and it hasn't got a [NotTracking]

        if (modelType.GetConstructors().
            Select(c => c.GetParameters())
            .Any(ps => ps.Length == 1 &&
                       ps[0].ParameterType == typeof(bool) &&
                       ps[0].Name == "generateId"))
        {
            return false;
        }

        var properties = modelType.GetProperties();

        if (properties
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
            .Any(p => !p.CanRead || !p.CanWrite))
        {
            return false;
        }

        if (properties
                .Count(p => Attribute.IsDefined(p, typeof(KeyAttribute)) && 
                            !Attribute.IsDefined(p, typeof(NotTrackedAttribute)) ) != 1)
        {
            return false;
        }

        return true;
    }
}