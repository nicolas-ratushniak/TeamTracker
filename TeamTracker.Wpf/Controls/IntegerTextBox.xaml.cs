﻿using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TeamTracker.Wpf.Controls;

public partial class IntegerTextBox : UserControl
{
    public static readonly DependencyProperty TextProperty;
    public static readonly DependencyProperty ValueProperty;
    public static readonly DependencyProperty MaxDigitsProperty;
    public static readonly RoutedEvent ValueChangedEvent;
    public static readonly DependencyProperty AcceptZeroProperty;

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

    private string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool AcceptZero
    {
        get => (bool)GetValue(AcceptZeroProperty);
        set => SetValue(AcceptZeroProperty, value);
    }

    public event RoutedPropertyChangedEventHandler<int> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    static IntegerTextBox()
    {
        var textMetadata = new PropertyMetadata("0", OnTextChanged)
        {
            CoerceValueCallback = CoerceText
        };

        var valueMetadata = new PropertyMetadata(default(int), OnValueChanged);

        TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IntegerTextBox), textMetadata);
        ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(IntegerTextBox), valueMetadata);
        MaxDigitsProperty = DependencyProperty.Register(nameof(MaxDigits), typeof(int), typeof(IntegerTextBox),
            new PropertyMetadata(10));

        ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int>), typeof(IntegerTextBox));

        AcceptZeroProperty = DependencyProperty.Register(nameof(AcceptZero), typeof(bool), typeof(IntegerTextBox),
            new PropertyMetadata(true));
    }

    public IntegerTextBox()
    {
        InitializeComponent();
    }

    private static object CoerceText(DependencyObject sender, object value)
    {
        var text = (string)value;

        if (text.Length > 1 && text.StartsWith('0'))
        {
            return text.TrimStart('0');
        }

        return text;
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerTextBox)d;
        var newValue = (string)e.NewValue;

        control.Value = int.TryParse(newValue, out int num) ? num : default;
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerTextBox)d;
        var newValue = (int)e.NewValue;

        if (!string.IsNullOrEmpty(control.Text))
        {
            control.Text = newValue.ToString();
        }

        var args = new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, newValue, ValueChangedEvent);
        control.RaiseEvent(args);
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var oldText = ((TextBox)sender).Text;

        if (!int.TryParse(e.Text, out int digit) || oldText.Length >= MaxDigits)
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