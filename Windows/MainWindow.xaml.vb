Imports System.ComponentModel
Imports System.Windows.Media.Animation

Class MainWindow
    Private ni As New Forms.NotifyIcon With {.Icon = New System.Drawing.Icon("./data-transfer.ico"), .Visible = True}

    Private Sub window_Loaded(sender As Object, e As EventArgs) Handles Me.Loaded
        AddHandler ni.DoubleClick, Sub(a, b)
                                       Show()
                                       WindowState = WindowState.Normal
                                   End Sub
    End Sub

    Private Sub window_OnStateChanged(sender As Object, e As EventArgs) Handles Me.StateChanged
        If WindowState = WindowState.Minimized Then Hide()
    End Sub

    Private Sub window_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Save(WindowData.ListPlans)
        ni.Visible = False
        ni.Dispose()
    End Sub

    Private Sub window_Closeing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        e.Cancel = WindowData.CancelPlan = 0
        If e.Cancel Then Return
        e.Cancel = WindowData.ListPlans.Any(Function(x) x.IsSync)
        If e.Cancel Then MsgBox(I001)
    End Sub

    Private Sub btnNew_Click(sender As Object, e As RoutedEventArgs) Handles btnNew.Click
        WindowData.EditPlan()
    End Sub

    Private Sub btnNewCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnNewCancel.Click
        If WindowData.CancelPlan > 0 Then CType(FindResource("A02:New2Main"), Storyboard).Begin()
    End Sub

    Private Sub btnNewSave_Click(sender As Object, e As RoutedEventArgs) Handles btnNewSave.Click
        If WindowData.SavePlan = 0 Then CType(FindResource("A02:New2Main"), Storyboard).Begin()
    End Sub

    Private Sub lvAllPlans_MouseDoubleClick(sender As ListView, e As MouseButtonEventArgs)
        If sender.SelectedItem Is Nothing Then Return
        WindowData.EditPlan(sender.SelectedItem)
        CType(FindResource("A01:Main2New"), Storyboard).Begin()
    End Sub

    Private Sub lvManualSync_Click(sender As Object, e As RoutedEventArgs)
        Dim plan As PlanObj = sender.tag
        plan.ManualSync()
    End Sub

    Private Sub lvDelete_Click(sender As Object, e As RoutedEventArgs)
        Dim plan As PlanObj = sender.tag
        WindowData.ListPlans.Remove(plan)
        Save(WindowData.ListPlans)
        GC.SuppressFinalize(plan)
    End Sub

    Private Sub btnManualSyncAll_Click(sender As Object, e As RoutedEventArgs)
        For Each p As PlanObj In WindowData.ListPlans
            p.ManualSync()
        Next
    End Sub
End Class
