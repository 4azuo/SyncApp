Imports System.Collections.ObjectModel
Imports System.IO
Imports Newtonsoft.Json

Public Module PlanExt
    Public Const PLAN_DB_FILE_PATH As String = "./db"

    Public Sub Save(data As ObservableCollection(Of PlanObj))
        File.WriteAllText(PLAN_DB_FILE_PATH, JsonConvert.SerializeObject(data))
    End Sub

    Public Function Load() As ObservableCollection(Of PlanObj)
        If Not File.Exists(PLAN_DB_FILE_PATH) Then Return New ObservableCollection(Of PlanObj)
        Return JsonConvert.DeserializeObject(File.ReadAllText(PLAN_DB_FILE_PATH), GetType(ObservableCollection(Of PlanObj)))
    End Function
End Module
