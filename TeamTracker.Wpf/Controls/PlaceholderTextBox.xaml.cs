using System.Windows;
using System.Windows.Controls;

namespace TeamTracker.Wpf.Controls;

public partial class PlaceholderTextBox : UserControl
{
    public static readonly DependencyProperty TextProperty;
    public static readonly DependencyProperty PlaceholderProperty;
    public static readonly DependencyProperty IsTypingProperty;
    public static readonly DependencyProperty MaxLenghtProperty;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public int MaxLength
    {
        get => (int)GetValue(MaxLenghtProperty);
        set => SetValue(MaxLenghtProperty, value);
    }
    
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }
    
    private bool IsTyping
    {
        get => (bool)GetValue(IsTypingProperty);
        set => SetValue(IsTypingProperty, value);
    }


    static PlaceholderTextBox()
    {
        TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlaceholderTextBox),
            new PropertyMetadata(OnTextChanged));
        PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PlaceholderTextBox));
        IsTypingProperty = DependencyProperty.Register(nameof(IsTyping), typeof(bool), typeof(PlaceholderTextBox));
        MaxLenghtProperty = DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(PlaceholderTextBox),
            new PropertyMetadata(10));
    }

    public PlaceholderTextBox()
    {
        InitializeComponent();
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var control = (PlaceholderTextBox)sender;
        var newText = (string)e.NewValue;

        control.IsTyping = !string.IsNullOrEmpty(newText);
    }
}