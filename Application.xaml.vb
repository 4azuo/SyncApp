Class Application

    ' Startup、Exit、DispatcherUnhandledException などのアプリケーション レベルのイベントは、
    ' このファイルで処理できます。

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        Dim thisProcessName As String = Process.GetCurrentProcess().ProcessName
        If Process.GetProcesses().Count(Function(p) p.ProcessName = thisProcessName) > 1 Then
            MsgBox(E007)
            Environment.Exit(0)
            Return
        End If

        MyBase.OnStartup(e)
    End Sub

End Class
