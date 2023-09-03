using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TeamTracker.Wpf.Controls;

public partial class PlaceholderIntTextBox : UserControl
{
    public static readonly DependencyProperty TextProperty;
    public static readonly DependencyProperty PlaceholderProperty;
    public static readonly DependencyProperty IsTypingProperty;
    public static readonly DependencyProperty ValueProperty;
    public static readonly DependencyProperty MaxDigitsProperty;
    public static readonly DependencyProperty AcceptZeroProperty;

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }
    
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public int MaxDigits
    {
        get => (int)GetValue(MaxDigitsProperty);
        set => SetValue(MaxDigitsProperty, value);
    }
    
    public bool AcceptZero
    {
        get => (bool)GetValue(AcceptZeroProperty);
        set => SetValue(AcceptZeroProperty, value);
    }
    
    private bool IsTyping
    {
        get => (bool)GetValue(IsTypingProperty);
        set => SetValue(IsTypingProperty, value);
    }
    
    private string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    static PlaceholderIntTextBox()
    {
        var textMetadata = new PropertyMetadata(OnTextChanged)
        {
            CoerceValueCallback = CoerceText
        };

        TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlaceholderIntTextBox), textMetadata);
        ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(PlaceholderIntTextBox),
            new PropertyMetadata(default(int), OnValueChanged));
        PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PlaceholderIntTextBox));
        IsTypingProperty = DependencyProperty.Register(nameof(IsTyping), typeof(bool), typeof(PlaceholderIntTextBox));
        MaxDigitsProperty = DependencyProperty.Register(nameof(MaxDigits), typeof(int), typeof(PlaceholderIntTextBox),
            new PropertyMetadata(10));
        AcceptZeroProperty = DependencyProperty.Register(nameof(AcceptZero), typeof(bool), typeof(PlaceholderIntTextBox),
            new PropertyMetadata(true));
    }

    public PlaceholderIntTextBox()
    {
        InitializeComponent();
    }

    private static object CoerceText(DependencyObject d, object value)
    {
        var text = (string)value;

        if (text.Length > 1 && text.StartsWith('0'))
        {
            return text.TrimStart('0');
        }

        return text;
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var control = (PlaceholderIntTextBox)sender;
        var newText = (string)e.NewValue;

        control.Value = int.TryParse(newText, out int num) ? num : default;
        control.IsTyping = !string.IsNullOrEmpty(newText);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PlaceholderIntTextBox)d;
        var newValue = (int)e.NewValue;

        if (!string.IsNullOrEmpty(control.Text))
        {
            control.Text = newValue.ToString();
        }
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var oldText = ((TextBox)sender).Text;

        if (!int.TryParse(e.Text, out int digit))
        {
            e.Handled = true;
        }
        // handle first zero if not acceptable
        else if (!AcceptZero && string.IsNullOrEmpty(oldText) && digit == 0)
        {
            e.Handled = true;
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
    }

    private void OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        var text = (string?)e.DataObject.GetData(typeof(string));

        if (text is null || !Regex.IsMatch(text, @"^\d*$") || text.Length > MaxDigits)
        {
            e.CancelCommand();
        }
    }
}