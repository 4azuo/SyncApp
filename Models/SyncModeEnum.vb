Public Class SyncModeEnum
    Public Shared ReadOnly DefaultSyncModes As SyncModeEnum() = {
        New SyncModeEnum(2, "Normal", "Sync the file and recheck directory."),
        New SyncModeEnum(3, "Best", "Recheck all files and directories (include sub-directory).")
        }

#Region "Constructor"
    Public ReadOnly Property Value As Integer
    Public ReadOnly Property Name As String
    Public ReadOnly Property Tooltip As String

    Public Sub New(iValue As Integer, iName As String, iTooltip As String)
        Value = iValue
        Name = iName
        Tooltip = iTooltip
    End Sub
#End Region
End Class
