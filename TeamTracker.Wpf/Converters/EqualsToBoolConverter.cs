using System;
using System.Globalization;
using System.Windows.Data;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.Converters;

public class EqualsToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (ViewType)value == (ViewType)parameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}