﻿using System.Globalization;
using System.Reflection;
using TeamTracker.Data.Exceptions;

namespace TeamTracker.Data;

public class ModelConverter<TModel> : IModelConverter<TModel>
{
    private readonly CultureInfo _cultureInfo;
    private readonly string _separator;
    private readonly List<PropertyInfo> _propertyInfos;

    public ModelConverter(CultureInfo cultureInfo)
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
            throw new InvalidOperationException("The record to be parsed should not be null or empty.");
        }

        var values = record.Split(_separator);
        var result = Activator.CreateInstance<TModel>();

        for (int i = 0; i < values.Length; i++)
        {
            var propertyInfo = _propertyInfos[i];

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
                        BindingFlags.InvokeMethod, null, result, new object[] { values[i], _cultureInfo })!;
                }
                catch (MissingMethodException)
                {
                    throw new InvalidModelTypeException(
                        "The types of properties tracked should implement IParsable<> interface.");
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

    public bool TryParseFromDbRecord(string record, out TModel result)
    {
        result = default!;

        try
        {
            result = ParseFromDbRecord(record);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public string ToDbRecord(TModel model)
    {
        var values = _propertyInfos
            .Select(p => string.Format(_cultureInfo, "{0}",  p.GetValue(model)))
            .ToList();

        if (values.Any(v => v.Contains(_separator)))
        {
            throw new FormatException("Property string representation shouldn't contain separator");
        }

        return string.Join(_separator, values);
    }

    /// <summary>
    /// Checks whether a model class is valid for correct parsing and formatting
    /// </summary>
    /// <remarks>Model is considered valid if:<br />
    /// 1. All public properties should be readable and writable 
    /// 2. The types of properties tracked should implement IParsable</remarks>
    private static bool ValidateModelType()
    {
        return typeof(TModel).GetProperties()
            .All(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string) ||
                                              typeof(IParsable<>).IsAssignableFrom(p.PropertyType));
    }
}