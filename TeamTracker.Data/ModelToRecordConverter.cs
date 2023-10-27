using System.Globalization;
using System.Reflection;
using TeamTracker.Data.Abstract;
using TeamTracker.Data.Exceptions;

namespace TeamTracker.Data;

public class ModelToRecordConverter<TModel> : IModelToRecordConverter<TModel> 
    where TModel : new()
{
    private readonly CultureInfo _cultureInfo;
    private readonly string _separator;

    private readonly List<PropertyInfo> _propertyInfos;

    public ModelToRecordConverter() : this(CultureInfo.CurrentCulture)
    {
    }

    public ModelToRecordConverter(CultureInfo cultureInfo)
    {
        if (!ValidateModelType())
        {
            throw new InvalidModelTypeException();
        }

        _propertyInfos = typeof(TModel).GetProperties().ToList();
        _cultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        _separator = _cultureInfo.TextInfo.ListSeparator;
    }

    public TModel ParseFromDbRecord(string record)
    {
        if (string.IsNullOrEmpty(record))
        {
            throw new ArgumentException("The record to be parsed should not be null or empty.");
        }

        var values = record.Split(_separator);
        var target = Activator.CreateInstance<TModel>();

        for (int i = 0; i < values.Length; i++)
        {
            var propertyInfo = _propertyInfos[i];
            var propertyType = propertyInfo.PropertyType;
            dynamic value;

            if (propertyType == typeof(string))
            {
                value = values[i];
            }
            else
            {
                try
                {
                    value = propertyInfo.PropertyType.InvokeMember(
                        "Parse",
                        BindingFlags.Public |
                        BindingFlags.Static |
                        BindingFlags.InvokeMethod, null, target, new object[] { values[i], _cultureInfo })!;
                }
                catch (TargetInvocationException)
                {
                    throw new FormatException($"Cannot parse \"{values[i]}\" to {propertyType.Name}");
                }
            }

            propertyInfo.SetValue(target, value);
        }

        return target;
    }

    public string ToDbRecord(TModel model)
    {
        var values = _propertyInfos
            .Select(p => string.Format(_cultureInfo, "{0}", p.GetValue(model)))
            .ToList();

        if (values.Any(v => v.Contains(_separator)))
        {
            throw new FormatException("Property string representation shouldn't contain a separator");
        }

        return string.Join(_separator, values);
    }

    /// <summary>
    /// Checks whether a model class is valid for correct parsing and formatting
    /// </summary>
    /// <remarks>Model is considered valid if:<br />
    /// 1. All public properties should be readable and writable 
    /// 2. The types of properties should be string or implement generic IParsable interface</remarks>
    private static bool ValidateModelType()
    {
        return typeof(TModel).GetProperties().All(p =>
            p is { CanRead: true, CanWrite: true } &&
            p.PropertyType == typeof(string) ||
            InheritsGenericInterface(p.PropertyType, typeof(IParsable<>)));

        bool InheritsGenericInterface(Type valueType, Type genericInterface)
        {
            return valueType.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == genericInterface);
        }
    }
}