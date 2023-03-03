﻿using System.Windows;
using System.Windows.Media;

public class AttachedProperties : DependencyObject
{
    public static DependencyProperty OveringBackgroundProperty = DependencyProperty.Register("OveringBackground", typeof(Brush), typeof(AttachedProperties));

    public static Brush GetOveringBackground(DependencyObject obj)
    {
        return (Brush)obj.GetValue(OveringBackgroundProperty);
    }

    public static void SetOveringBackground(DependencyObject obj, Brush value)
    {
        obj.SetValue(OveringBackgroundProperty, value);
    }

    public static DependencyProperty PressingBackgroundProperty = DependencyProperty.Register("PressingBackground", typeof(Brush), typeof(AttachedProperties));

    public static Brush GetPressingBackground(DependencyObject obj)
    {
        return (Brush)obj.GetValue(PressingBackgroundProperty);
    }

    public static void SetPressingBackground(DependencyObject obj, Brush value)
    {
        obj.SetValue(PressingBackgroundProperty, value);
    }

    public static DependencyProperty WatermaskForcegroundProperty = DependencyProperty.Register("WatermaskForceground", typeof(Brush), typeof(AttachedProperties));

    public static Brush GetWatermaskForceground(DependencyObject obj)
    {
        return (Brush)obj.GetValue(WatermaskForcegroundProperty);
    }

    public static void SetWatermaskForceground(DependencyObject obj, Brush value)
    {
        obj.SetValue(WatermaskForcegroundProperty, value);
    }
}
