Imports System.IO
Imports System.Text
Imports Newtonsoft.Json

Public Class PlanObj
    Inherits AutoNotifiableObject

    Private Shared SyncObj As New Object

#Region "Properties"
    Public Property PlanName As String
    Public Property Dest As String
    Public Property Source As String
    Public Property CreatedDate As Date = Date.Now '新規
    <JsonIgnore>
    Public ReadOnly Property LastSyncTime As Date = Date.Now '新規
    Public Property AutoSync As Boolean = True '新規
    Public Property SyncMode As Integer = 2 '新規
    <JsonIgnore>
    Public ReadOnly Property ViewDestFolder As String
        Get
            If Not Directory.Exists(Dest) Then Return ""
            Dim curDir As New DirectoryInfo(Dest)
            Dim prtDir As DirectoryInfo = Directory.GetParent(Dest)
            Return $"Dest: ...\{prtDir.Name}\{curDir.Name}"
        End Get
    End Property
    <JsonIgnore>
    Public ReadOnly Property ViewSourceFolder As String
        Get
            If Not Directory.Exists(Source) Then Return ""
            Dim curDir As New DirectoryInfo(Source)
            Dim prtDir As DirectoryInfo = Directory.GetParent(Source)
            Return $"Source: ...\{prtDir.Name}\{curDir.Name}"
        End Get
    End Property
    <JsonIgnore>
    Public ReadOnly Property ViewInfo As String
        Get
            Dim builder As New StringBuilder
            builder.AppendLine($"Created: {CreatedDate:HH\:mm\:ss}")
            builder.AppendLine($"Last: {LastSyncTime:HH\:mm\:ss}")
            Return builder.ToString.Trim
        End Get
    End Property
    <JsonIgnore>
    Public ReadOnly Property ViewOptions As String
        Get
            Dim builder As New StringBuilder
            builder.AppendLine($"Auto Sync: {If(AutoSync, "On", "Off")}")
            If AutoSync Then builder.AppendLine($"Sync Mode: {SyncModeEnum.DefaultSyncModes.First(Function(x) x.Value = SyncMode).Name}")
            Return builder.ToString.Trim
        End Get
    End Property
    <JsonIgnore>
    Public ReadOnly Property IsError As Boolean = False
    <JsonIgnore>
    Public ReadOnly Property IsSync As Boolean = False
    <JsonIgnore>
    Public ReadOnly Property BackgroundMode As Integer
        Get
            Return If(IsError, 1, 0)
        End Get
    End Property
#End Region

#Region "Watcher"
    Private ReadOnly Property Watcher As FileSystemWatcher
    Private OnChangedHandler As New FileSystemEventHandler(AddressOf OnChanged)
    Private OnCreatedHandler As New FileSystemEventHandler(AddressOf OnCreated)
    Private OnDeletedHandler As New FileSystemEventHandler(AddressOf OnDeleted)
    Private OnRenamedHandler As New RenamedEventHandler(AddressOf OnRenamed)
    Private OnErrorHandler As New ErrorEventHandler(AddressOf OnError)

    Public Sub CreateWatcher()
        If Not AutoSync OrElse Watcher IsNot Nothing Then Return
        If String.IsNullOrEmpty(Dest) OrElse String.IsNullOrEmpty(Source) OrElse Not Directory.Exists(Dest) OrElse Not Directory.Exists(Source) Then
            _IsError = True
            Return
        Else
            _IsError = False
        End If
        Dest = New DirectoryInfo(Dest).FullName.Replace("/"c, "\"c).TrimEnd("\"c)
        Source = New DirectoryInfo(Source).FullName.Replace("/"c, "\"c).TrimEnd("\"c)

        _Watcher = New FileSystemWatcher(Source)
        Watcher.NotifyFilter = NotifyFilters.Attributes Or NotifyFilters.CreationTime Or NotifyFilters.DirectoryName Or NotifyFilters.FileName Or NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.Security Or NotifyFilters.Size
        AddHandler Watcher.Changed, OnChangedHandler
        AddHandler Watcher.Created, OnCreatedHandler
        AddHandler Watcher.Deleted, OnDeletedHandler
        AddHandler Watcher.Renamed, OnRenamedHandler
        AddHandler Watcher.Error, OnErrorHandler
        Watcher.IncludeSubdirectories = True
        Watcher.EnableRaisingEvents = True
    End Sub

    Private Sub OnChanged(sender As Object, e As FileSystemEventArgs)
        _IsSync = True
        Dim srcPath As String = e.FullPath
        Dim dstPath As String = GetDestPath(e.FullPath)
        SyncWithMode(Directory.GetParent(srcPath).FullName, Directory.GetParent(dstPath).FullName)
        _LastSyncTime = Date.Now
        _IsSync = False
    End Sub

    Private Sub OnCreated(sender As Object, e As FileSystemEventArgs)
        _IsSync = True
        Dim srcPath As String = e.FullPath
        Dim dstPath As String = GetDestPath(e.FullPath)
        SyncWithMode(Directory.GetParent(srcPath).FullName, Directory.GetParent(dstPath).FullName)
        _LastSyncTime = Date.Now
        _IsSync = False
    End Sub

    Private Sub OnDeleted(sender As Object, e As FileSystemEventArgs)
        _IsSync = True
        Dim srcPath As String = e.FullPath
        Dim dstPath As String = GetDestPath(e.FullPath)
        SyncWithMode(Directory.GetParent(srcPath).FullName, Directory.GetParent(dstPath).FullName)
        _LastSyncTime = Date.Now
        _IsSync = False
    End Sub

    Private Sub OnRenamed(sender As Object, e As RenamedEventArgs)
        _IsSync = True
        Dim srcPath As String = e.FullPath
        Dim dstPath As String = GetDestPath(e.FullPath)
        SyncWithMode(Directory.GetParent(srcPath).FullName, Directory.GetParent(dstPath).FullName)
        _LastSyncTime = Date.Now
        _IsSync = False
    End Sub

    Private Sub OnError(sender As Object, e As ErrorEventArgs)
    End Sub
#End Region

#Region "Manual Sync"
    Public Sub ManualSync()
        _IsSync = True
        CreateWatcher()
        SyncFolder(Source, Dest, SearchOption.AllDirectories)
        _IsSync = False
    End Sub
#End Region

#Region "Commons"
    Private Function GetDestPath(srcPath As String) As String
        Dim localPath As String = srcPath.Replace(Source, "")
        Return Dest & localPath
    End Function

    Private Function GetSrcPath(dstPath As String) As String
        Dim localPath As String = dstPath.Replace(Dest, "")
        Return Source & localPath
    End Function

    Private Function GetFiles(path As String, Optional opt As SearchOption = SearchOption.TopDirectoryOnly) As String()
        Return Directory.GetFiles(path, "*.*", opt)
    End Function

    Private Function GetDirectories(path As String, Optional opt As SearchOption = SearchOption.TopDirectoryOnly) As String()
        Return Directory.GetDirectories(path, "*", opt)
    End Function

    Private Sub SyncWithMode(srcFolder As String, dstFolder As String)
        If Not Directory.Exists(srcFolder) OrElse Not Directory.Exists(dstFolder) Then
            _IsError = True
            Watcher.Dispose()
            _Watcher = Nothing
            Return
        Else
            _IsError = False
        End If

        Select Case SyncMode
            Case 2 'Normal
                SyncFolder(srcFolder, dstFolder)
            Case 3 'Best
                SyncFolder(Source, Dest, SearchOption.AllDirectories)
            Case Else
        End Select
    End Sub

    Private Sub SyncFolder(srcFolder As String, dstFolder As String, Optional opt As SearchOption = SearchOption.TopDirectoryOnly)
        SyncLock SyncObj
            If Not Directory.Exists(srcFolder) OrElse Not Directory.Exists(dstFolder) Then
                _IsError = True
                Watcher?.Dispose()
                _Watcher = Nothing
                Return
            Else
                _IsError = False
            End If

            Try
                'Files
                Dim srcFiles As String() = GetFiles(srcFolder, opt)
                Dim dstFiles As String() = GetFiles(dstFolder, opt)
                Dim notFoundFiles As String() = dstFiles.Where(Function(x) Not srcFiles.Contains(GetSrcPath(x))).ToArray
                Dim notSyncedFiles As String() = srcFiles.Where(Function(x) Not dstFiles.Contains(GetDestPath(x))).ToArray
                Dim changedFiles As String() = dstFiles.Where(Function(x)
                                                                  Dim srcFile As String = srcFiles.FirstOrDefault(Function(y) y = GetSrcPath(x))
                                                                  If srcFile Is Nothing Then Return False
                                                                  Return File.GetLastWriteTime(x) <> File.GetLastWriteTime(srcFile)
                                                              End Function).ToArray
                For Each f As String In notFoundFiles
                    Dim curF As New FileInfo(f)
                    Dim curD As DirectoryInfo = curF.Directory
                    curF.Delete()
                    If Not Directory.Exists(GetSrcPath(curD.FullName)) AndAlso curD.GetFiles().Count = 0 Then
                        curD.Delete()
                    End If
                Next
                For Each f As String In notSyncedFiles
                    Dim curF As New FileInfo(f)
                    Directory.CreateDirectory(GetDestPath(curF.DirectoryName))
                    File.Copy(f, GetDestPath(f), True)
                Next
                For Each f As String In changedFiles
                    Dim curF As New FileInfo(f)
                    Directory.CreateDirectory(curF.DirectoryName)
                    File.Copy(GetSrcPath(f), f, True)
                Next

                'Directories
                Dim srcDirs As String() = GetDirectories(srcFolder, opt)
                Dim dstDirs As String() = GetDirectories(dstFolder, opt)
                Dim notFoundDirs As String() = dstDirs.Where(Function(x) Not srcDirs.Contains(GetSrcPath(x))).ToArray
                Dim notSyncedDirs As String() = srcDirs.Where(Function(x) Not dstDirs.Contains(GetDestPath(x))).ToArray
                For Each d As String In notFoundDirs
                    Directory.Delete(d, True)
                Next
                For Each d As String In notSyncedDirs
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(d, GetDestPath(d))
                Next
            Catch
            End Try
        End SyncLock
    End Sub
#End Region

    ' Destructor
    Protected Overrides Sub Finalize()
        If Watcher IsNot Nothing Then
            RemoveHandler Watcher.Changed, OnChangedHandler
            RemoveHandler Watcher.Created, OnCreatedHandler
            RemoveHandler Watcher.Deleted, OnDeletedHandler
            RemoveHandler Watcher.Renamed, OnRenamedHandler
            RemoveHandler Watcher.Error, OnErrorHandler
            Watcher.Dispose()
            _Watcher = Nothing
        End If
    End Sub
End Class
