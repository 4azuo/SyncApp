Imports System.Collections.ObjectModel
Imports System.IO
Imports Newtonsoft.Json

Public Class MainWindowData
    Inherits CWindowData

    Private EditingItem As PlanObj = Nothing
    Private NewItemBk As String = ""

    ''' <summary>
    ''' Lists
    ''' </summary>
    Public Property ListPlans As ObservableCollection(Of PlanObj)
    Public ReadOnly Property ListSyncModes As SyncModeEnum() = SyncModeEnum.DefaultSyncModes

    ''' <summary>
    ''' Window Values
    ''' </summary>
    Public Property NewItem As PlanObj = Nothing
    Public ReadOnly Property NotifyMsg As String
        Get
            If ListPlans.Count = 0 Then
                Return $"Plans: 0"
            Else
                Dim lastSyncPlan As PlanObj = ListPlans.OrderByDescending(Function(x) x.LastSyncTime.ToString("yyyyMMddHHmmss.fff")).First()
                Return $"Plans: {ListPlans.Count} - Last sync: {lastSyncPlan.PlanName} at {lastSyncPlan.LastSyncTime:HH\:mm\:ss}"
            End If
        End Get
    End Property
    Public ReadOnly Property IsNotNewMode As Boolean
        Get
            Return NewItem Is Nothing
        End Get
    End Property

    Public Overrides Sub OnLoad()
        ListPlans = Load()
        For Each p As PlanObj In ListPlans
            p.ManualSync()
        Next
    End Sub

    Public Sub EditPlan(Optional plan As PlanObj = Nothing)
        If plan Is Nothing Then
            NewItem = New PlanObj
        Else
            NewItem = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(plan), GetType(PlanObj))
            NewItem.CreatedDate = Date.Now
            EditingItem = plan
        End If
        NewItemBk = JsonConvert.SerializeObject(NewItem)
    End Sub

    Public Function IsPlanChanged() As Boolean
        Return JsonConvert.SerializeObject(NewItem) <> NewItemBk
    End Function

    Public Function SavePlan() As Integer
        If NewItem Is Nothing Then Return -1
        If IsPlanChanged() Then
            If String.IsNullOrEmpty(NewItem.PlanName) Then
                MsgBox(E001)
                Return 1
            End If
            If ListPlans.Any(Function(x) x.PlanName = NewItem.PlanName) Then
                MsgBox(E009)
                Return 1
            End If
            If String.IsNullOrEmpty(NewItem.Dest) Then
                MsgBox(E002)
                Return 1
            End If
            If String.IsNullOrEmpty(NewItem.Source) Then
                MsgBox(E003)
                Return 1
            End If
            If NewItem.Dest = NewItem.Source Then
                MsgBox(E004)
                Return 1
            End If
            If Not Directory.Exists(NewItem.Dest) Then
                MsgBox(E005)
                Return 1
            End If
            If Not Directory.Exists(NewItem.Source) Then
                MsgBox(E006)
                Return 1
            End If
            Dim a As PlanObj = ListPlans.FirstOrDefault(Function(x) NewItem.Dest.Contains(x.Dest) OrElse x.Dest.Contains(NewItem.Dest))
            If a IsNot Nothing Then
                MsgBox(String.Format(E008, a.PlanName, a.Dest))
                Return 1
            End If

            'save
            If EditingItem IsNot Nothing Then
                ListPlans.Remove(EditingItem)
            End If
            ListPlans.Add(NewItem)
            Save(ListPlans)
            NewItem.ManualSync()
        End If
        EditingItem = Nothing
        NewItem = Nothing
        Return 0
    End Function

    Public Function CancelPlan() As Integer
        If NewItem Is Nothing Then Return -1
        If IsPlanChanged() Then
            If MsgBox(C001, MsgBoxStyle.YesNo) = MsgBoxResult.No Then Return 0
            EditingItem = Nothing
            NewItem = Nothing
            Return 1
        Else
            EditingItem = Nothing
            NewItem = Nothing
            Return 2
        End If
    End Function
End Class
