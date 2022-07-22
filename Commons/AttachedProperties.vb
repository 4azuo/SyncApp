Public Class AttachedProperties
    Inherits DependencyObject

    Public Shared ReadOnly OveringBackgroundProperty As DependencyProperty = DependencyProperty.Register("OveringBackground", GetType(Brush), GetType(AttachedProperties))

    Public Shared Function GetOveringBackground(d As DependencyObject) As Brush
        Return d.GetValue(OveringBackgroundProperty)
    End Function

    Public Shared Sub SetOveringBackground(d As DependencyObject, value As Brush)
        d.SetValue(OveringBackgroundProperty, value)
    End Sub

    Public Shared ReadOnly PressingBackgroundProperty As DependencyProperty = DependencyProperty.Register("PressingBackground", GetType(Brush), GetType(AttachedProperties))

    Public Shared Function GetPressingBackground(d As DependencyObject) As Brush
        Return d.GetValue(PressingBackgroundProperty)
    End Function

    Public Shared Sub SetPressingBackground(d As DependencyObject, value As Brush)
        d.SetValue(PressingBackgroundProperty, value)
    End Sub

    Public Shared ReadOnly WatermaskForcegroundProperty As DependencyProperty = DependencyProperty.Register("WatermaskForceground", GetType(Brush), GetType(AttachedProperties))

    Public Shared Function GetWatermaskForceground(d As DependencyObject) As Brush
        Return d.GetValue(WatermaskForcegroundProperty)
    End Function

    Public Shared Sub SetWatermaskForceground(d As DependencyObject, value As Brush)
        d.SetValue(WatermaskForcegroundProperty, value)
    End Sub
End Class
