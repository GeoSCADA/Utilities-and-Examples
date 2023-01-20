Imports ClearScada.Client
Imports ClearScada.Client.Simple
Imports ClearScada.Client.Advanced
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Data.Odbc
Imports System.Data.SqlClient

#Const SimpleExportOnly = True

Enum RunModeType
    Normal = 0
    Auto = 1
End Enum

Public Class Form1

    Private Log As New Logger("ConfigWatcher")
    Private Defaults As New SetDefaults
    Private optionfile As String
    Private RunMode As RunModeType
    Private SettingsChanged As Boolean

    Private ActivityCount = 0

    Private ExportFolderText As String
    Private ExportFolder2Text As String
    Private ExportFolder3Text As String
    Private ExportFolder4Text As String


    Private Sub ExportChanges_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportChanges.Click
        StatusLabel.Text = "Exporting"
        NotifyIcon1.Visible = True
        Log.WriteToLog("Export for: " & optionfile)
        Try
            Dim SConnection As New Odbc.OdbcConnection("DRIVER={" & ODBCDriver.Text & "};SERVER=" & ConnectionName.Text & _
                                                                            ";uid=" & Username.Text & _
                                                                            ";pwd=" & Password.Text)
            Try
                Log.WriteToLog("Export Login to ODBC: " & ConnectionName.Text & "," & ODBCDriver.Text & "," & Username.Text)
                SConnection.Open()
            Catch ex As Exception
                Log.WriteToLog("Cannot connect to Geo SCADA ODBC")
                If RunMode = RunModeType.Normal Then
                    MsgBox("Cannot connect to Geo SCADA ODBC", MsgBoxStyle.OkOnly, "ConfigWatcher")
                End If
                StatusLabel.Text = "Idle"
                NotifyIcon1.Visible = False
                Exit Sub
            End Try
            ' Removed for GS 2022 ConnectionType.Standard, 
            Dim Node As New ClearScada.Client.ServerNode(ServerName.Text, ServerPort.Text)

            Log.WriteToLog("Export Login: " & ServerName.Text & "," & ServerPort.Text & "," & Username.Text)
            Dim GeoSCADA As New Connection("ConfigWatcher")
            Try
                GeoSCADA.Connect(Node)
                Dim spassword = New System.Security.SecureString()
                Dim c As Char
                For Each c In Password.Text
                    spassword.AppendChar(c)
                Next
                GeoSCADA.LogOn(Username.Text, spassword)
                Exit Try
            Catch Ex As Exception
                Log.WriteToLog("Cannot connect to Geo SCADA")
                If RunMode = RunModeType.Normal Then
                    MsgBox("Cannot connect to Geo SCADA", MsgBoxStyle.OkOnly, "ConfigWatcher")
                End If
                StatusLabel.Text = "Idle"
                NotifyIcon1.Visible = False
                Exit Sub
            End Try
            Dim Obj As Simple.DBObject
            'Dim Objs As Simple.DBObjectCollection

            'GUI control
            Me.UseWaitCursor = True
            ExportChanges.Enabled = False

            Log.WriteToLog("Starting Export, last Change is:" & LastChanged.Text)

            'Build SQL query to access data
            If Wildcard.Text = "*" Or Wildcard.Text = "" Then
                Wildcard.Text = "%"
                SettingsChanged = True
            End If
            Dim SQL As String = "SELECT Id,FullName,TypeName,ConfigTime FROM CDBObject WHERE FullName LIKE '" & Wildcard.Text & "' ORDER BY Id"
            Dim objDR As Odbc.OdbcDataReader
            Dim Cmd As New OdbcCommand(SQL, SConnection)
            Log.WriteToLog("Read: " & SQL)
            objDR = Cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            'Was using GetObjects, now replaced with SQL
            'Objs = GeoSCADA.GetObjects("CDBObject", "") 'Pattern constrained only on name

            Dim LastConfigTime As Date
            Dim FirstObj As Boolean = True

            'Move outside loop, only do this once
            ExportFolderText = ExportFolder.Text
            ExportFolder2Text = ExportFolder2.Text
            ExportFolder3Text = ExportFolder3.Text
            ExportFolder4Text = ExportFolder4.Text
            If ExportDailyFolders.Checked Then
                ExportFolderText = CreateDailyFolders(ExportFolderText)
            End If
            If Not ExportFolder2Text = "" Then
                If ExportDailyFolders.Checked Then
                    ExportFolder2Text = CreateDailyFolders(ExportFolder2Text)
                End If
            End If
            If Not ExportFolder3Text = "" Then
                If ExportDailyFolders.Checked Then
                    ExportFolder3Text = CreateDailyFolders(ExportFolder3Text)
                End If
            End If
            If Not ExportFolder4Text = "" Then
                If ExportDailyFolders.Checked Then
                    ExportFolder4Text = CreateDailyFolders(ExportFolder4Text)
                End If
            End If

            'Was: For Each Obj In Objs
            While objDR.Read()
                'Get the GeoSCADA Object associated with this row
                Obj = GeoSCADA.GetObject(objDR(0))

                'Update screen at a sensible interval
                ActivityCount += 1
                If ActivityCount Mod 50 = 0 Then
                    StatusLabel.Text = "Checking: " & Obj.FullName
                    Application.DoEvents()
                End If

                'If this object is a Template Group, we need to traverse all children recursively, performing the same checks
                'So the code below has been moved into a function, and the following calls the recursive search
                If Obj.Item("TypeName") <> "CTemplate" Then
                    CheckAndExport(Obj, GeoSCADA, LastConfigTime, FirstObj)
                Else
                    CheckAndExportChildren(Obj, GeoSCADA, LastConfigTime, FirstObj)
                End If

            End While

            'Add 1 sec to create next date to be used, could this be 1 mSec?
            If FirstObj = False Then
                LastChanged.Text = Format(LastConfigTime.AddSeconds(1), "dd MMM yyyy HH:mm:ss")
                'Mark as  changed
                SettingsChanged = True
            End If
            Log.WriteToLog("Finished Export, last Change is:" & LastChanged.Text)

        Catch ex As Exception
            Log.WriteToLog(ex.Message)
            If RunMode = RunModeType.Normal Then
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "ConfigWatcher")
            End If
        End Try
        'GUI control
        Me.UseWaitCursor = False
        ExportChanges.Enabled = True
        StatusLabel.Text = "Idle"
        NotifyIcon1.Visible = False
    End Sub

    'Recursively check for children if this is a template
    Private Sub CheckAndExportChildren(ByRef Obj As Simple.DBObject, ByRef GeoSCADA As Connection, ByRef LastConfigTime As Date, ByRef FirstObj As Boolean)
        'Get the children of this object, if groups or instances then call again
        Dim Children As DBObjectCollection
        Children = Obj.GetChildren("", "")
        For Each Child As DBObject In Children

            'Update screen at a sensible interval
            ActivityCount += 1
            If ActivityCount Mod 50 = 0 Then
                StatusLabel.Text = "Checking: " & Child.FullName
                Application.DoEvents()
            End If

            CheckAndExport(Child, GeoSCADA, LastConfigTime, FirstObj)
            If Child.Item("TypeName") = "CGroup" Or
                Child.Item("TypeName") = "CTemplateInstance" Then
                CheckAndExportChildren(Child, GeoSCADA, LastConfigTime, FirstObj)
            End If
        Next
    End Sub

    'Export this object, if conditions are right
    Private Sub CheckAndExport(ByRef Obj As Simple.DBObject, ByRef GeoSCADA As Connection, ByRef LastConfigTime As Date, ByRef FirstObj As Boolean)

        Dim DoExport As Boolean
        'Log to table variables
        Dim Action As String = "Export"
        Dim Status As String = "OK"
        Dim Comments As String = ""

        If OmitGroups.Checked = False Then
            DoExport = True
        Else
            If Obj.Item("TypeName") = "CRootGroup" Or
            Obj.Item("TypeName") = "CGroup" Or
            Obj.Item("TypeName") = "CTemplate" Or
            Obj.Item("TypeName") = "CTemplateInstance" Then
                DoExport = False
            Else
                DoExport = True
            End If
        End If
        'Debug.Print("Examining: " & Obj.FullName)

        'Check export constraints if field non-blank
        If DoExport = True Then 'Do not check if already decided not to export
            If ExportConstraint.Text <> "" Then
                If Obj.Item(ExportConstraint.Text) = ExportConstraintValue.Text Then
                    DoExport = True
                Else
                    DoExport = False
                End If
            End If
        End If

        'Need to read the ConfigTime field
        Dim ConfigTime As Date 'Use system type

        ConfigTime = CDate(Obj.Item("ConfigTime").ToString) 'Seems to be the only way to get a valid date/time
        If DoExport And (ConfigTime > CDate(LastChanged.Text)) Then
            'Get ready to export this object

            'Specific option 'ForceGroupsOnly' means to Export Instances of Template controlled objects,
            ' so that these objects can be imported without converting all instances to groups
            Dim ExportParent As Boolean
            Dim ParentObj As Simple.DBObject = Nothing

            ExportParent = False
            If ForceGroupsOnly.Checked = True Then
                'Check if this is contained within an instance, and if so, then import that instance
                'But it has to be template controlled, not just lying in an instance group
                'So get the parent group and if a template, find it and check it has a child of the same name
                'If not then an advanced check would also check the parent parent group (recursively), but this code will not support that at present

                ParentObj = GeoSCADA.GetObject(Obj.Item("ParentGroupName"))
                If ParentObj.Item("TypeName") = "CTemplateInstance" Then
                    'We have the parent instance, now get template
                    Dim TemplateId As New ClearScada.Client.ObjectId(ParentObj.Item("TemplateId"))
                    Dim TemplateObj As Simple.DBObject = GeoSCADA.GetObject(TemplateId)
                    Dim ChildObj As Simple.DBObject
                    ChildObj = GeoSCADA.GetObject(TemplateObj.FullName & "." & Obj.Item("Name"))
                    If Not (ChildObj Is Nothing) Then
                        'Found it, so now export the parent rather than the object
                        ExportParent = True
                    Else
                        'Parent is an instance, but this object does not belong in that template
                        'It may possibly be that this object is a freestanding object within a further template
                        'This case is ignored for now
                    End If
                Else
                    'This could possibly be the child of a normal group in an instance
                    'Again, we won't concern ourselves with this now
                End If
            End If

            'Show screen progress
            StatusLabel.Text = "Exporting: " & Obj.FullName
            Application.DoEvents()

            If ExportParent = False Then
                'If this is a group, and ExportGroupAsText is true, then write a text file instead
                Dim ExportAsText As Boolean

                ExportAsText = (ExportGroupAsText.Checked And (Obj.Item("TypeName") = "CRootGroup" Or
                                                    Obj.Item("TypeName") = "CGroup" Or
                                                    Obj.Item("TypeName") = "CTemplate" Or
                                                    Obj.Item("TypeName") = "CTemplateInstance"))
                'Write a text file with the details instead
                If ExportAsText Then
                    Comments = "Exported as Text file"
                Else
                    Comments = ""
                End If
                'export file to each of the 4 given locations, only if a path has been specified. MS 10/12/08
                'feature added to allow program to write to folders where name changes dynamically based on date. MS 23/12/08
                If Not ExportFolderText = "" Then
                    ExportFile(ExportFolderText, Obj, ExportAsText)
                End If
                If Not ExportFolder2Text = "" Then
                    ExportFile(ExportFolder2Text, Obj, ExportAsText)
                End If
                If Not ExportFolder3Text = "" Then
                    ExportFile(ExportFolder3Text, Obj, ExportAsText)
                End If
                If Not ExportFolder4Text = "" Then
                    ExportFile(ExportFolder4Text, Obj, ExportAsText)
                End If
                WriteResultsDataTable(GeoSCADA,
                               Obj,
                               Action,
                               Status,
                               Comments)
            Else
                Comments = "Exported parent Instance"
                If Not ExportFolderText = "" Then
                    ExportFile(ExportFolderText, ParentObj, False)
                End If
                If Not ExportFolder2Text = "" Then
                    ExportFile(ExportFolder2Text, ParentObj, False)
                End If
                If Not ExportFolder3Text = "" Then
                    ExportFile(ExportFolder3Text, ParentObj, False)
                End If
                If Not ExportFolder4Text = "" Then
                    ExportFile(ExportFolder4Text, ParentObj, False)
                End If
                WriteResultsDataTable(GeoSCADA,
                               ParentObj,
                               Action,
                               Status,
                               Comments)
            End If
            If FirstObj Then
                LastConfigTime = ConfigTime
                FirstObj = False
            End If
            If LastConfigTime < ConfigTime Then
                LastConfigTime = ConfigTime
            End If
        End If

    End Sub

    'Used to create backup files
    Private Sub ExportFileAdv(ByVal destination As String, ByRef GeoSCADA As ClearScada.Client.Advanced.IServer, ByRef Obj As ClearScada.Client.Advanced.ObjectDetails)
        'Make Filename
        Dim Exportfilename As String
        Dim FStream As System.IO.Stream
        Dim Warn() As ClearScada.Client.Advanced.Warning = Nothing
        Exportfilename = Obj.FullName & "+" &
                            GeoSCADA.GetProperty(Obj.Id, "TypeName") & "+" &
                            GeoSCADA.GetProperty(Obj.Id, "ConfigVersion") & "+" &
                            Format(GeoSCADA.GetProperty(Obj.Id, "ConfigTime").DateTime, "dd-MMM-yy hh-mm") & "+" &
                            GeoSCADA.GetProperty(Obj.Id, "ConfigUser")
        'Truncate to prevent filename from becoming too long - first apply 254 char limit to filename
        If Exportfilename.Length > 254 - 4 Then
            Exportfilename = Exportfilename.Substring(0, 254 - 4)
        End If
        'Then apply 260 char limit to pathname
        If (Exportfilename.Length + destination.Length) > 260 - 2 - 4 Then
            Exportfilename = Exportfilename.Substring(0, 260 - 2 - destination.Length - 4)
        End If
        Exportfilename = destination & "\" & Exportfilename
        Exportfilename = Exportfilename & ".sde"
        Exportfilename = Exportfilename.Replace("*", "$") 'Substitute with $, not X
        'Open file as stream
        CreateDirectoryStructure(Exportfilename)
        FStream = File.Create(Exportfilename)
        'export object
        Log.WriteToLog("Export to :" & Exportfilename)
        GeoSCADA.ExportConfiguration(Obj.Id, FStream, Warn)
        'Close stream
        FStream.Close()
    End Sub

    'Append record to DataTable
    'Simple Client
    Private Sub WriteResultsDataTable(ByRef GeoSCADA As ClearScada.Client.Simple.Connection,
                                       ByRef DBObject As ClearScada.Client.Simple.DBObject,
                                       ByRef Action As String,
                                       ByRef Status As String,
                                       ByRef Comments As String)
        Dim RecordId As Long
        Dim EmptyMethodArguments() As Object = Nothing
        Dim AddRecordArguments(2) As Object

        If DataTableName.Text.Length <> 0 Then
            Dim TimeStamp As Date = Now()
            Dim FullName As String = DBObject.FullName
            Dim TypeName As String = DBObject.Item("TypeName")
            Dim DataTable As ClearScada.Client.Simple.DBObject
            Try
                DataTable = GeoSCADA.GetObject(DataTableName.Text)
                RecordId = DataTable.InvokeMethod("AddRecord", EmptyMethodArguments)
                AddRecordArguments(1) = RecordId

                AddRecordArguments(0) = "FullName"
                AddRecordArguments(2) = FullName
                DataTable.InvokeMethod("SetValue", AddRecordArguments)

                AddRecordArguments(0) = "TimeStamp"
                AddRecordArguments(2) = TimeStamp.ToString("yyyy-MM-dd hh:mm:ss")
                DataTable.InvokeMethod("SetValue", AddRecordArguments)

                AddRecordArguments(0) = "TypeName"
                AddRecordArguments(2) = TypeName
                DataTable.InvokeMethod("SetValue", AddRecordArguments)

                AddRecordArguments(0) = "Action"
                AddRecordArguments(2) = Action
                DataTable.InvokeMethod("SetValue", AddRecordArguments)

                AddRecordArguments(0) = "Status"
                AddRecordArguments(2) = Status
                DataTable.InvokeMethod("SetValue", AddRecordArguments)

                AddRecordArguments(0) = "Comments"
                AddRecordArguments(2) = Comments
                DataTable.InvokeMethod("SetValue", AddRecordArguments)

                AddRecordArguments(0) = "ActivityName"
                AddRecordArguments(2) = GetFileShortName(optionfile)
                DataTable.InvokeMethod("SetValue", AddRecordArguments)
            Catch
                'No output if unsuccessful
                Log.WriteToLog("Failed to write to results data table")
            End Try
        End If
    End Sub

    'Append record to DataTable
    'Simple Client
    Private Sub WriteResultsDataTableAdv(ByRef GeoSCADA As ClearScada.Client.Advanced.IServer,
                                       ByRef DBObject As ClearScada.Client.Advanced.ObjectDetails,
                                       ByRef Action As String,
                                       ByRef Status As String,
                                       ByRef Comments As String)
        Dim RecordId As Long
        Dim EmptyMethodArguments() As Object = Nothing
        Dim AddRecordArguments(2) As Object

        If DataTableName.Text.Length <> 0 Then
            Dim TimeStamp As Date = Now()
            Dim FullName As String = DBObject.FullName
            Dim TypeName As String = GeoSCADA.GetProperty(DBObject.Id, "TypeName")
            Dim DataTable As ClearScada.Client.Advanced.ObjectDetails
            Try
                DataTable = GeoSCADA.FindObject(DataTableName.Text)
                RecordId = GeoSCADA.InvokeMethod(DataTable.Id, "AddRecord", EmptyMethodArguments)
                AddRecordArguments(1) = RecordId

                AddRecordArguments(0) = "FullName"
                AddRecordArguments(2) = FullName
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)

                AddRecordArguments(0) = "TimeStamp"
                AddRecordArguments(2) = TimeStamp.ToString("yyyy-MM-dd hh:mm:ss")
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)

                AddRecordArguments(0) = "TypeName"
                AddRecordArguments(2) = TypeName
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)

                AddRecordArguments(0) = "Action"
                AddRecordArguments(2) = Action
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)

                AddRecordArguments(0) = "Status"
                AddRecordArguments(2) = Status
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)

                AddRecordArguments(0) = "Comments"
                AddRecordArguments(2) = Comments
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)

                AddRecordArguments(0) = "ActivityName"
                AddRecordArguments(2) = GetFileShortName(optionfile)
                GeoSCADA.InvokeMethod(DataTable.Id, "SetValue", AddRecordArguments)
            Catch
                'No output if unsuccessful
                Log.WriteToLog("Failed to write to results data table")
            End Try
        End If
    End Sub

    Private Sub ExportFile(ByVal destination As String, ByVal obj As ClearScada.Client.Simple.DBObject, ByVal AsText As Boolean)
        'Make Filename
        Dim Exportfilename As String

        Exportfilename = obj.FullName & "+" &
                            obj.Item("TypeName") & "+" &
                            obj.Item("ConfigVersion") & "+" &
                            Format(obj.Item("ConfigTime").DateTime, "dd-MMM-yy hh-mm") & "+" &
                            obj.Item("ConfigUser")
        'Truncate to prevent filename from becoming too long - first apply 254 char limit to filename
        If Exportfilename.Length > 254 - 4 Then
            Exportfilename = Exportfilename.Substring(0, 254 - 4)
        End If
        'Then apply 260 char limit to pathname
        If (Exportfilename.Length + destination.Length) > 260 - 2 - 4 Then
            Exportfilename = Exportfilename.Substring(0, 260 - 2 - destination.Length - 4)
        End If
        Exportfilename = destination & "\" & Exportfilename
        Exportfilename = Exportfilename.Replace("*", "$")

        CreateDirectoryStructure(Exportfilename)
        'export object
        Log.WriteToLog("Export to :" & Exportfilename)
        If AsText Then
            Exportfilename = Exportfilename & ".txt"
            Dim Writer As System.IO.StreamWriter

            'Writing text file
            Writer = System.IO.File.CreateText(Exportfilename)
            Writer.WriteLine("Group Export")
            Writer.WriteLine(obj.FullName)
            Writer.WriteLine(obj.Item("TypeName").ToString)
            'If this is a template instance, export the template name
            If obj.Item("TypeName").ToString = "CTemplateInstance" Then
                Dim TId As Long = Long.Parse(obj.Item("TemplateId").ToString)
                'More details may be needed here in future, e.g. FullName of Template object
                Writer.WriteLine(obj.Item("TemplateId").ToString)
            End If
            Writer.Flush()
            Writer.Close()
        Else
            Exportfilename = Exportfilename & ".sde"
            Dim FStream As System.IO.Stream

            'Open file as stream
            FStream = File.Create(Exportfilename)
            obj.ExportConfiguration(FStream)
            'Close stream
            FStream.Close()
        End If
    End Sub

    Private Sub MoveDoneObject(ByVal FileName As String, ByRef currentfile As System.IO.FileInfo)
        MoveFile(FileName, currentfile, ImportOKFolder.Text)
    End Sub

    Private Sub MoveFailedObject(ByVal FileName As String, ByRef currentfile As System.IO.FileInfo)
        MoveFile(FileName, currentfile, ImportFailFolder.Text)
    End Sub

    Private Sub MoveFile(ByVal FileName As String, ByRef currentfile As System.IO.FileInfo, ByVal NewFolder As String)
        Dim FileParts() As String
        Dim Destination As String

        If Not NewFolder = "" Then
            If File.Exists(FileName) Then
                File.Delete(FileName)
            End If
            FileParts = Split(FileName, "\")
            If NewFolder.EndsWith("\") Then
                Destination = NewFolder & FileParts(UBound(FileParts))
            Else
                Destination = NewFolder & "\" & FileParts(UBound(FileParts))
            End If
            CreateDirectoryStructure(NewFolder)
            Try
                currentfile.MoveTo(Destination)
            Catch ex As Exception
                Log.WriteToLog("Cannot move file to " & Destination)
            End Try
        End If
    End Sub

    Private Sub CreateDirectoryStructure(ByVal Folder As String)
        Dim LocalLoc
        LocalLoc = Split(Folder, "\")
        Dim CurrentFolder As String = LocalLoc(0)
        Dim i As Integer = 1

        While i < UBound(LocalLoc)

            CurrentFolder = CurrentFolder & "\" & LocalLoc(i)
            If Not System.IO.Directory.Exists(CurrentFolder) Then
                System.IO.Directory.CreateDirectory(CurrentFolder)
            End If
            i += 1
        End While
    End Sub

    Private Function CreateDailyFolders(ByVal Folder As String) As String
        Dim currentday As String = Format(Now(), "yyyy-MM-dd")
        CreateDailyFolders = Folder & "\" & currentday
    End Function

    Private Sub ImportFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportFiles.Click
        StatusLabel.Text = "Importing"
        NotifyIcon1.Visible = True
        Log.WriteToLog("Import for: " & optionfile)
        Try
            Dim Node As New ClearScada.Client.ServerNode(ServerName.Text, ServerPort.Text)

            Log.WriteToLog("Import Login: " & ServerName.Text & "," & ServerPort.Text & "," & Username.Text)
            'Dim GeoSCADA As New Connection("ConfigWatcher")
            Dim GeoSCADA As ClearScada.Client.Advanced.IServer

            Try
                Dim ConnectionSettings As New ClientConnectionSettings()
                GeoSCADA = Node.Connect("ConfigWatcher", ConnectionSettings)
                Dim spassword = New System.Security.SecureString()
                Dim c As Char
                For Each c In Password.Text
                    spassword.AppendChar(c)
                Next
                GeoSCADA.LogOn(Username.Text, spassword)
                Exit Try
            Catch Ex As Exception
                Log.WriteToLog("Cannot connect to Geo SCADA")
                If RunMode = RunModeType.Normal Then
                    MsgBox("Cannot connect to Geo SCADA", MsgBoxStyle.OkOnly, "ConfigWatcher")
                End If
                StatusLabel.Text = "Idle"
                NotifyIcon1.Visible = False
                Exit Sub
            End Try

            'GUI control
            Me.UseWaitCursor = True
            ImportFiles.Enabled = False

            'Log to table variables
            Dim Action As String = "Import"
            Dim Status As String = "OK"
            Dim Comments As String = ""

            'List Files in folder
            Dim ImportDir As System.IO.DirectoryInfo = My.Computer.FileSystem.GetDirectoryInfo(ImportFolder.Text)
            Dim sdeFiles As FileInfo()

            If ImportDailyFolders.Checked Then
                sdeFiles = ImportDir.GetFiles(FileWildcard.Text, SearchOption.AllDirectories)
            Else
                sdeFiles = ImportDir.GetFiles(FileWildcard.Text, SearchOption.TopDirectoryOnly)
            End If
            Dim sdeFile As System.IO.FileInfo

            Dim FileList As New Collection 'Hold separate list indexed by Geo SCADA pathname

            Dim FileName As String
            Dim FileParts() As String
            Dim PathParts() As String
            Dim GeoSCADAPath As String
            Dim GeoSCADAFullPath As String
            'Dim GroupObjs As Simple.DBObjectCollection
            'Dim GroupObj As Simple.DBObject
            Dim CurrentObj As ClearScada.Client.Advanced.ObjectDetails
            Dim ParentObj As ClearScada.Client.Advanced.ObjectDetails
            Dim GroupObj As ClearScada.Client.Advanced.ObjectDetails
            Dim FStream As IO.FileStream
            Dim i As Integer
            Dim ImportedOK As Boolean = False

            'For a separate indexed list
            'Currently sorts by GeoSCADA FullName, but may need to change and sort by change date (if present in filename)
            For Each sdeFile In sdeFiles
                FileList.Add(sdeFile, sdeFile.FullName)
            Next

            'Now read from that list
            For Each sdeFile In FileList
                ImportedOK = False
                Comments = ""
                FileName = sdeFile.FullName
                Log.WriteToLog("Processing File: " & FileName)

                'If last part of name is .sde, then remove it
                If FileName.ToLower.EndsWith(".sde") Then
                    FileParts = Split(FileName.Substring(0, FileName.Length - 4), "\")
                Else
                    FileParts = Split(FileName, "\")
                End If
                'Now remove parts before \ (also not always present)
                GeoSCADAPath = FileParts(FileParts.GetUpperBound(0))
                'Remove the parts after + (may not always be present)
                '(Do this way round so there can be a + in the folder name)
                FileParts = Split(GeoSCADAPath, "+")
                GeoSCADAPath = FileParts(0)
                'Now split into GeoSCADA path items
                PathParts = Split(GeoSCADAPath, ".")
                'Get group name
                If PathParts.GetUpperBound(0) > 0 Then
                    GeoSCADAFullPath = ""
                    For i = 0 To PathParts.GetUpperBound(0)
                        GeoSCADAFullPath = GeoSCADAFullPath & PathParts(i)
                        If i < PathParts.GetUpperBound(0) Then
                            GeoSCADAPath = GeoSCADAFullPath
                            GeoSCADAFullPath = GeoSCADAFullPath + "."
                        End If
                    Next
                Else
                    'If there is only one, then the parent is the root group
                    GeoSCADAPath = "$Root"
                    GeoSCADAFullPath = PathParts(0)
                End If

                'Fix names, e.g. $ converted back to *
                GeoSCADAPath = FixGeoSCADAPath(GeoSCADAPath)
                GeoSCADAFullPath = FixGeoSCADAPath(GeoSCADAFullPath)

                'Find group object with matching names (workaround in absence of FindObject in Simple)
                CurrentObj = GeoSCADA.FindObject(GeoSCADAFullPath)
                ParentObj = GeoSCADA.FindObject(GeoSCADAPath)

                'If no parent, then move on
                If ParentObj Is Nothing Then
                    If RunMode = RunModeType.Normal Then
                        MsgBox("Cannot import, no parent object found: " & GeoSCADAPath, MsgBoxStyle.OkOnly, "ConfigWatcher")
                    End If
                    Log.WriteToLog("Cannot import, no parent object found: " & GeoSCADAPath)
                    MoveFailedObject(FileName, sdeFile)
                    Comments = Comments & "No parent object found: " & GeoSCADAPath & ". "
                Else
                    'If no file exists then move on
                    If Not File.Exists(FileName) Then
                        If RunMode = RunModeType.Normal Then
                            MsgBox("Error opening file, check specified paths.", MsgBoxStyle.OkOnly, "ConfigWatcher")
                        End If
                        Log.WriteToLog("Error opening file, check specified paths.")
                        Comments = Comments & "Error opening file. "
                    Else
                        Try
                            FStream = File.OpenRead(FileName)

                            Dim Warn() As ClearScada.Client.Advanced.Warning = Nothing

                            If (BackupFolder.Text <> "") Then
                                If (CurrentObj Is Nothing) Then
                                    'Nothing to back up
                                Else
                                    Log.WriteToLog("Backing up object: " & GeoSCADAPath)
                                    ExportFileAdv(BackupFolder.Text, GeoSCADA, CurrentObj)
                                End If
                            End If

                            Dim ImportTask As Integer ' Imports now return a task Id number
                            Try
                                'If object does not exist, then import normally
                                If CurrentObj Is Nothing Then
                                    'CurrentObj = ?
                                    ' Previously it returned the object id which it imported
                                    ImportTask = GeoSCADA.ImportConfiguration(ParentObj.Id, FStream, ImportOptions.Merge)

                                    Log.WriteToLog("Normal import")
                                    Comments = Comments & "New item(s). "
                                    ImportedOK = True
                                Else
                                    'Object exists, first try a merge
                                    Try
                                        'CurrentObj = ?
                                        ' Previously it returned the object id which it imported
                                        ImportTask = GeoSCADA.ImportConfiguration(ParentObj.Id, FStream, ImportOptions.Merge)
                                        Log.WriteToLog("Merged import")
                                        Comments = Comments & "Merged item(s). "
                                        ImportedOK = True
                                    Catch ex As Exception
                                        'On failure, delete it and then import normally
                                        Try
                                            GeoSCADA.DeleteObject(CurrentObj.Id, True, Warn)
                                            Log.WriteToLog("Deleted object in order to attempt import into template instance.")
                                            Comments = Comments & "Old item(s) deleted. "
                                        Catch ex2 As Exception
                                            'Could not delete because object is a template instance member
                                            'Only option (as GeoSCADA is too stupid to be able to merge into instances)
                                            ' is to convert all instances up the tree into groups
                                            If ConvertToForceMerge.Checked Then
                                                'Only do this if user has selected special option
                                                If PathParts.GetUpperBound(0) > 0 Then
                                                    GeoSCADAFullPath = ""
                                                    For i = 0 To PathParts.GetUpperBound(0)
                                                        GeoSCADAFullPath = GeoSCADAFullPath & PathParts(i)
                                                        If i < PathParts.GetUpperBound(0) Then
                                                            GeoSCADAPath = GeoSCADAFullPath
                                                            'Attempt to convert each element to group from instance
                                                            GroupObj = GeoSCADA.FindObject(GeoSCADAPath)
                                                            If GroupObj Is Nothing Then
                                                                'Failed
                                                            Else
                                                                If GroupObj.IsInstance Then
                                                                    'Convert
                                                                    Log.WriteToLog("Converting Instance to Group: " & GeoSCADAPath)
                                                                    GeoSCADA.ConvertObject(GroupObj.Id, "CGroup", Warn)
                                                                End If
                                                            End If

                                                            'Prepare path for next loop around
                                                            GeoSCADAFullPath = GeoSCADAFullPath + "."
                                                        End If
                                                        Comments = Comments & "Instances converted to Groups. "
                                                    Next
                                                    'If we survived that, then try to import/merge the object
                                                    'CurrentObj = ?
                                                    ' Previously it returned the object id which it imported
                                                    ImportTask = GeoSCADA.ImportConfiguration(ParentObj.Id, FStream, ImportOptions.Merge)
                                                    Log.WriteToLog("Merged import OK")
                                                    ImportedOK = True
                                                    Comments = Comments & "Merged item(s). "
                                                Else
                                                    'If there is only one, then the parent is the root group
                                                    'We're stuffed, must have failed for some other reason, so throw it
                                                    Log.WriteToLog("Inappropriate to convert root group")
                                                    Comments = Comments & "Cannot convert root group. "
                                                    Throw ex2
                                                End If
                                            Else
                                                Log.WriteToLog("Cannot import and merge into instance")
                                                Comments = Comments & "Cannot merge into instance. "
                                                Throw ex2
                                            End If
                                        End Try
                                        'If we have not already imported within the Catch above
                                        If ImportedOK = False Then
                                            'Have deleted object, now import it
                                            'CurrentObj = ?
                                            ' Previously it returned the object id which it imported
                                            ImportTask = GeoSCADA.ImportConfiguration(ParentObj.Id, FStream, ImportOptions.None)
                                            Log.WriteToLog("Imported")
                                            Comments = Comments & "Imported on second attempt. "
                                            ImportedOK = True
                                            'Failures here will be unhandled - may need to address this
                                        End If
                                    End Try
                                End If

                            Catch Ex As Exception
                                Dim rootid As New ObjectId(0)
                                'Dim rootobj As Advanced.ObjectDetails
                                'rootobj = GeoSCADA.LookupObject(New ClearScada.Client.ObjectId(0))
                                'GeoSCADA.InvokeMethod(rootobj.Id, "SetInterfaceAlarm", _
                                'New Object() {Now(), 1, CurrentObj.FullName, "Error importing config, please check log file.", ""})
                                Log.WriteToLog("Failed to import new item: " & Ex.Message)
                                Comments = Comments & "Failed to import new item. "
                            End Try
                            FStream.Close()

                            If ImportedOK = True Then
                                Log.WriteToLog("OK, moving file")
                                'Move OK files
                                MoveDoneObject(FileName, sdeFile)

                                'Optional, if user wants OPC tag properties to be written to
                                If MapOPCTags.Checked Then
                                    'If we didn't get the object, then get it now
                                    If CurrentObj Is Nothing Then
                                        'Find group object with matching names (workaround in absence of FindObject in Simple)
                                        Try
                                            CurrentObj = GeoSCADA.FindObject(GeoSCADAFullPath)
                                        Catch
                                            Log.WriteToLog("Cannot find object to re-map OPC tags")
                                            Comments = Comments & "Cannot find object to re-map OPC tags. "
                                        End Try
                                    End If
                                    Try
                                        'Dim ScannerId As String
                                        'Make up scanner Id from the object's path
                                        Dim ScannerId As New ClearScada.Client.ObjectId

                                        MapOPCTagsFor(CurrentObj, GeoSCADA, ScannerId)
                                        Comments = Comments & "Re-Mapped OPC tags. "
                                    Catch Ex3 As Exception
                                        Log.WriteToLog("Fault setting OPC tags")
                                        Comments = Comments & "Fault setting OPC tags. "
                                    End Try
                                End If
                            Else
                                If RunMode = RunModeType.Normal Then
                                    MsgBox("Error importing file, moved to failed folder.", MsgBoxStyle.OkOnly, "ConfigWatcher")
                                End If
                                Log.WriteToLog("Moving to failed folder")
                                'Move failed files - don't want to keep retrying
                                MoveFailedObject(FileName, sdeFile)
                            End If

                        Catch ex As Exception
                            If RunMode = RunModeType.Normal Then
                                MsgBox("Error reading file. Stopping Import. " & ex.Message, MsgBoxStyle.OkOnly, "ConfigWatcher")
                                StatusLabel.Text = "Idle"
                                NotifyIcon1.Visible = False
                                Exit Sub
                            End If
                            Log.WriteToLog("Error reading file. " & ex.Message)
                            Comments = Comments & "Error reading file. " & ex.Message
                        End Try
                    End If
                End If
                'Write to results table (will not attempt this if the name is blank)
                If ImportedOK = True Then
                    WriteResultsDataTableAdv(GeoSCADA,
                           CurrentObj,
                           Action,
                           "OK",
                           Comments)
                Else
                    WriteResultsDataTableAdv(GeoSCADA,
                           CurrentObj,
                           Action,
                           "Fail",
                           Comments)
                End If
            Next
            Log.WriteToLog("Finished Import")

        Catch ex As Exception 'All other unhandled errors
            If RunMode = RunModeType.Normal Then
                MsgBox("Error: " & ex.Message, MsgBoxStyle.OkCancel, "ConfigWatcher")
            End If
            Log.WriteToLog("Error: " & ex.Message)
        End Try
        'GUI control
        StatusLabel.Text = "Idle"
        NotifyIcon1.Visible = False
        Me.UseWaitCursor = False
        ImportFiles.Enabled = True
    End Sub

    Public Function FixGeoSCADAPath(ByVal GeoSCADAPath) As String
        If InStr(GeoSCADAPath, "$", CompareMethod.Text) Then
            GeoSCADAPath = Replace(GeoSCADAPath, "$", "*") 'Substitute for $, more generic
        End If

        Return GeoSCADAPath
    End Function

    '**************************************************************************************
    'This does not work because the id is cleared on object import, as the scanner itself changed id and type
    'Private Sub MapOPCTagsGetScannerId(ByRef ThisObject As ClearScada.Client.Advanced.ObjectDetails, _
    '                                        ByRef GeoSCADA As ClearScada.Client.Advanced.IServer, _
    '                                        ByRef ScannerId As ClearScada.Client.ObjectId)
    '    'ThisObject is a valid object
    '    Dim childObjects As ClearScada.Client.Advanced.ObjectDetails()
    '    Dim childObject As ClearScada.Client.Advanced.ObjectDetails
    '    Dim temp As Integer

    '    If ScannerId.ToInt32 < 1 Then
    '        If ThisObject.IsGroup Then
    '            'Get children and recurse
    '            childObjects = GeoSCADA.ListObjects("", "*", ThisObject.Id, True)
    '            For Each childObject In childObjects
    '                MapOPCTagsGetScannerId(childObject, GeoSCADA, ScannerId)
    '            Next
    '        Else
    '            'If this object has a scanner
    '            Try
    '                Dim id As New ClearScada.Client.ObjectId(GeoSCADA.GetProperty(ThisObject.Id, "ScannerId"))
    '                ScannerId = id
    '            Catch
    '                'Normal for this to occur, not handled
    '            End Try
    '        End If
    '        temp = 1
    '    End If
    'End Sub

    '**************************************************************************************
    'Write to OPC Tag properties of point objects
    Private Sub MapOPCTagsFor(ByRef ThisObject As ClearScada.Client.Advanced.ObjectDetails,
                              ByRef GeoSCADA As ClearScada.Client.Advanced.IServer,
                              ByRef ScannerId As ClearScada.Client.ObjectId)
        'Find all child objects of this and set OPC properties, recurse if group objects are found
        'Start by getting this object
        Dim childObjects As ClearScada.Client.Advanced.ObjectDetails()
        Dim childObject As ClearScada.Client.Advanced.ObjectDetails
        Dim TemplateObj As ClearScada.Client.Advanced.ObjectDetails
        Dim Warn() As ClearScada.Client.Advanced.Warning = Nothing
        Dim MethodArguments() As Object = Nothing
        Dim NewName As String
        Dim temp As Integer

        If ThisObject Is Nothing Then
            'No more work to do
            Exit Sub
        Else
            'Now find if this object needs converting and/or has children
            If ThisObject.IsGroup Then
                'If a template instance then get template name
                If ThisObject.IsInstance Then
                    Try
                        Dim id As New ClearScada.Client.ObjectId(GeoSCADA.GetProperty(ThisObject.Id, "TemplateId"))

                        TemplateObj = GeoSCADA.LookupObject(id)
                        If InStr(TemplateObj.FullName, ".Modbus ") <> 0 Then
                            'Convert by replacing all text for protocol name
                            NewName = Replace(TemplateObj.FullName, ".Modbus ", ".OPC ")
                            'Before conversion, try to get the scannerid so we can set it later
                            'Do not do this now, as it is too late, the id is not available
                            'MapOPCTagsGetScannerId(ThisObject, GeoSCADA, ScannerId)
                            'Convert template
                            'Should use the dedicated function rather than setting Id, will convert objects properly
                            'Was: GeoSCADA.SetProperty(ThisObject.Id, "TemplateId", NewName)
                            TemplateObj = GeoSCADA.FindObject(NewName)
                            Log.WriteToLog("Converting template for: " & ThisObject.FullName & " to: " & NewName)
                            GeoSCADA.ConvertInstance(ThisObject.Id, TemplateObj.Id, True, Warn)
                            'May need to repair template?
                            Log.WriteToLog("Repairing template")
                            GeoSCADA.InvokeMethod(ThisObject.Id, "Repair", MethodArguments)
                        End If
                    Catch ex As Exception
                        Log.WriteToLog("Error converting and repairing template for: " & ThisObject.FullName)
                    End Try
                End If
                'Get children and recurse
                childObjects = GeoSCADA.ListObjects("", "*", ThisObject.Id, True)
                For Each childObject In childObjects
                    MapOPCTagsFor(childObject, GeoSCADA, ScannerId)
                Next
            Else
                'not a group, is it an OPC point?
                If ThisObject.ClassName.StartsWith("COpc") And ThisObject.ClassName.EndsWith("Point") Then
                    'Read scanner - now read earlier, as was cleared by setting template
                    'Dim id As New ClearScada.Client.ObjectId(GeoSCADA.GetProperty(ThisObject.Id, "ScannerId"))

                    'Convert tag reference if not already
                    If GeoSCADA.GetProperty(ThisObject.Id, "TagName") <> ThisObject.FullName Then
                        Try
                            GeoSCADA.SetProperty(ThisObject.Id, "TagName", ThisObject.FullName)
                        Catch ex As Exception
                            Log.WriteToLog("Error setting OPC Tag for: " & ThisObject.FullName)
                        End Try
                    End If
                    'And set the OPC Scanner reference if not already
                    '**********************************************************************************
                    'Ignore the recursively found ScannerId, Geo SCADA bins that if the protocol is different
                    'So the only option we have now is to make up the Id from a coded up name
                    '**********************************************************************************
                    Dim ScannerName As String
                    Dim ScannerNameParts() As String

                    ScannerNameParts = Split(ThisObject.FullName, ".")
                    ScannerName = ScannerNameParts(0) & "." & ScannerNameParts(1) & ".PLC Modbus.DI Scanner"
                    If GeoSCADA.GetProperty(ThisObject.Id, "ScannerId") <> GeoSCADA.FindObject(ScannerName).Id Then
                        Try
                            GeoSCADA.SetProperty(ThisObject.Id, "ScannerId", GeoSCADA.FindObject(ScannerName).Id)
                        Catch
                            Log.WriteToLog("Error setting OPC Scanner for: " & ThisObject.FullName)
                        End Try
                    End If
                End If
            End If
        End If
        temp = 1 'Line of code for debugger to land on, has no purpose
    End Sub


    '**************************************************************************************
    '* Form open/close
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ImportOKToolTip.SetToolTip(Me.ImportOKFolder, "Files are moved here when successfully imported")

        Log.StartLogging(My.Application.Info.DirectoryPath)

        'Set default optionfile name
        optionfile = AppPath() & "ConfigWatcher.ini"
        'If file is specified on command line
        'Get command line argument if exists
        If Environment.GetCommandLineArgs.GetUpperBound(0) > 1 Then
            'Second argument is name of settings file
            optionfile = Environment.GetCommandLineArgs(2)
        End If
        SettingsFileName.Text = optionfile

        'Read settings from file
        LoadSettings()

        RunMode = RunModeType.Normal

        'Run command line argument if exists
        If Environment.GetCommandLineArgs.GetUpperBound(0) > 0 Then
            Select Case Environment.GetCommandLineArgs(1).ToLower
                Case "ExportChanges".ToLower
                    RunMode = RunModeType.Auto
                    ExportChanges_Click(sender, e)
                    SaveSettings(optionfile) 'Write the new last change date.
                    EndProgram(False)
                    End
                Case "ImportFiles".ToLower
                    RunMode = RunModeType.Auto
                    ImportFiles_Click(sender, e)
                    EndProgram(False)
                    End
                Case ""
                    'if empty string then do nothing - needed if 2nd argument is settings file name
                Case Else
                    MsgBox("Invalid command line argument.", MsgBoxStyle.Exclamation, "ConfigWatcher")
            End Select
        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        EndProgram(True)
    End Sub

    Private Sub EndProgram(ByVal SaveChange As Boolean)
        If SettingsChanged = True Then
            'prompt the user to save their settings on close MS 10/12/08
            If SaveChange Then
                Dim Result As Microsoft.VisualBasic.MsgBoxResult
                Result = MsgBox("Save settings?  This will replace the current configuration file.", MsgBoxStyle.YesNo, "Save Settings")

                If Result = MsgBoxResult.Yes Then
                    SaveSettings(optionfile)
                End If
            End If
        End If

        Log.StopLogging()
        Log = Nothing
    End Sub

    'Existing log routines replaced with dedicated Logging class

    '**************************************************************************************
    '* Log File Routines
    'Private Sub OpenLogFile()
    '    Dim LogFileName As String = "Not Assigned"
    '    Try
    '        'Get app path
    '        LogFileName = AppPath() & "ConfigWatcher.log"
    '        'Open for append
    '        LogFileWriter = My.Computer.FileSystem.OpenTextFileWriter(LogFileName, True)
    '        Log.WriteToLog("Opened Log File")
    '    Catch ex As Exception
    '        StatusLabel.Text = "Cannot open log file: " & LogFileName
    '    End Try
    'End Sub
    'Private Sub Log.WriteToLog(ByVal S As String)
    '    Try
    '        StatusLabel.Text = S
    '        LogFileWriter.WriteLine(Now.ToString & "," & S)
    '    Catch
    '    End Try
    'End Sub
    'Private Sub CloseLogFile()
    '    Try
    '        LogFileWriter.Close()
    '    Catch ex As Exception
    '        StatusLabel.Text = "Cannot close log file"
    '    End Try
    'End Sub

    '* Get directory of executable file (includes \ at end)
    Private Function AppPath() As String
        Dim assembly As String = System.Reflection.Assembly.GetEntryAssembly().Location
        AppPath = Path.GetDirectoryName(assembly) & "\"
    End Function

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox("ConfigWatcher" & Chr(10) & "Version " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & Chr(10) & "Created for demonstration by Schneider Electric. No warranty or liability included.", MsgBoxStyle.Information, "ConfigWatcher")
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        EndProgram(True)
        End
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        Dim Result As System.Windows.Forms.DialogResult
        Result = OpenDial.ShowDialog()

        If Result = Windows.Forms.DialogResult.OK Then
            'Set default optionfile name
            optionfile = OpenDial.FileName
            'Get command line argument if exists
            SettingsFileName.Text = optionfile
            'Read settings from file
            LoadSettings()
        End If
    End Sub

    'get name without directory name or .ini extension
    Private Function GetFileShortName(ByVal optionfile As String)
        Dim FileNameParts() As String = Split(optionfile, "\")
        Dim FileName As String = FileNameParts(FileNameParts.GetUpperBound(0))
        Dim BaseNameParts() As String = Split(FileName, ".")
        Return BaseNameParts(0)
    End Function

    Private Sub LoadSettings()
        If File.Exists(optionfile) Then
            Dim myFileStream As Stream = File.OpenRead(optionfile)
            Dim deserializer As New BinaryFormatter()
            Defaults = CType(deserializer.Deserialize(myFileStream), SetDefaults)
            myFileStream.Close()
        End If
        'Get from serialised structure
        ServerName.Text = Defaults.ServerName
        ServerPort.Text = Defaults.ServerPort
        Username.Text = Defaults.Username
        Password.Text = Defaults.Password
        ExportFolder.Text = Defaults.ExportFolder
        ExportFolder2.Text = Defaults.ExportFolder2
        ExportFolder3.Text = Defaults.ExportFolder3
        ExportFolder4.Text = Defaults.ExportFolder4
        Wildcard.Text = Defaults.Wildcard
        LastChanged.Text = Defaults.LastChanged
        OmitGroups.Checked = Defaults.OmitGroups
        ImportFolder.Text = Defaults.ImportFolder
        FileWildcard.Text = Defaults.FileWildcard
        ImportOKFolder.Text = Defaults.ImportOKFolder
        BackupFolder.Text = Defaults.BackupFolder
        ImportDailyFolders.Checked = Defaults.ImportDailyFolders
        ExportDailyFolders.Checked = Defaults.ExportDailyFolders
        ImportFailFolder.Text = Defaults.ImportFailFolder
        ExportConstraint.Text = Defaults.ExportConstraint
        ExportConstraintValue.Text = Defaults.ExportConstraintValue
        ConvertToForceMerge.Checked = Defaults.ConvertToForceMerge
        MapOPCTags.Checked = Defaults.MapOPCTags
        ForceGroupsOnly.Checked = Defaults.ForceGroupsOnly
        DataTableName.Text = Defaults.DataTableName
        ConnectionName.Text = Defaults.ConnectionName
        ODBCDriver.Text = Defaults.ODBCDriver
        ExportGroupAsText.Checked = Defaults.ExportGroupAsText
        AdvancedToolStripMenuItem.Checked = Defaults.GUIAdvanced

        'Set on-screen filename text
        SettingsFileName.Text = optionfile
        'Mark as not changed
        SettingsChanged = False
        'Update any greyed out
        If OmitGroups.Checked Then
            ExportGroupAsText.Enabled = False
        Else
            ExportGroupAsText.Enabled = True
        End If
        'Update UI field visibility
        UpdateGUI()
    End Sub

    Private Sub SaveSettings(ByVal FileName As String)
        'Save settings
        Defaults.ServerName = ServerName.Text
        Defaults.ServerPort = ServerPort.Text
        Defaults.Username = Username.Text
        Defaults.Password = Password.Text
        Defaults.ExportFolder = ExportFolder.Text
        Defaults.ExportFolder2 = ExportFolder2.Text
        Defaults.ExportFolder3 = ExportFolder3.Text
        Defaults.ExportFolder4 = ExportFolder4.Text
        Defaults.Wildcard = Wildcard.Text
        Defaults.LastChanged = LastChanged.Text
        Defaults.OmitGroups = OmitGroups.Checked
        Defaults.ImportFolder = ImportFolder.Text
        Defaults.FileWildcard = FileWildcard.Text
        Defaults.ImportOKFolder = ImportOKFolder.Text
        Defaults.BackupFolder = BackupFolder.Text
        Defaults.ImportDailyFolders = ImportDailyFolders.Checked
        Defaults.ExportDailyFolders = ExportDailyFolders.Checked
        Defaults.ImportFailFolder = ImportFailFolder.Text
        Defaults.ExportConstraint = ExportConstraint.Text
        Defaults.ExportConstraintValue = ExportConstraintValue.Text
        Defaults.ConvertToForceMerge = ConvertToForceMerge.Checked
        Defaults.MapOPCTags = MapOPCTags.Checked
        Defaults.ForceGroupsOnly = ForceGroupsOnly.Checked
        Defaults.DataTableName = DataTableName.Text
        Defaults.ConnectionName = ConnectionName.Text
        Defaults.ODBCDriver = ODBCDriver.Text
        Defaults.ExportGroupAsText = ExportGroupAsText.Checked
        Defaults.GUIAdvanced = AdvancedToolStripMenuItem.Checked

        'Serialise and write
        If Not optionfile = FileName Then
            optionfile = FileName
        End If
        Dim myFileStream As Stream = File.Create(optionfile)
        Dim serializer As New BinaryFormatter()
        serializer.Serialize(myFileStream, Defaults)
        myFileStream.Close()
        SettingsFileName.Text = optionfile
        'Mark as not changed
        SettingsChanged = False

    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        Dim Result As System.Windows.Forms.DialogResult
        Result = SaveDial.ShowDialog()

        If Result = Windows.Forms.DialogResult.OK Then
            SaveSettings(SaveDial.FileName)
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        If optionfile = "" Then
            Dim Result As System.Windows.Forms.DialogResult
            Result = SaveDial.ShowDialog()

            If Result = Windows.Forms.DialogResult.OK Then
                SaveSettings(SaveDial.FileName)
            End If
        Else
            SaveSettings(optionfile)
        End If
    End Sub

    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        ServerName.Text = ""
        ServerPort.Text = ""
        Username.Text = ""
        Password.Text = ""
        ExportFolder.Text = ""
        ExportFolder2.Text = ""
        ExportFolder3.Text = ""
        ExportFolder4.Text = ""
        Wildcard.Text = ""
        LastChanged.Text = ""
        OmitGroups.Checked = False
        ImportDailyFolders.Checked = False
        ExportDailyFolders.Checked = False
        ImportFolder.Text = ""
        FileWildcard.Text = ""
        ImportOKFolder.Text = ""
        ImportFailFolder.Text = ""
        optionfile = ""
        BackupFolder.Text = ""
        ExportConstraint.Text = ""
        ExportConstraintValue.Text = "True"
        ConvertToForceMerge.Checked = False
        MapOPCTags.Checked = False
        ForceGroupsOnly.Checked = False
        DataTableName.Text = ""
        ExportGroupAsText.Checked = False
        AdvancedToolStripMenuItem.Checked = True

        'Set filename on screen
        SettingsFileName.Text = optionfile

        SettingsChanged = True
    End Sub

    Private Sub CommandLineToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CommandLineToolStripMenuItem.Click
        MsgBox("ConfigWatcher.exe  [""ExportChanges"" | ""ImportFiles""]  [""Filename.ini""]", MsgBoxStyle.OkOnly, "ConfigWatcher")
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        LastChanged.Text = Now().ToString("dd-MMM-yyyy HH:mm:ss")
        'Mark as changed
        SettingsChanged = True
    End Sub

    Private Sub OmitGroups_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OmitGroups.CheckedChanged
        If OmitGroups.Checked Then
            ExportGroupAsText.Enabled = False
        Else
            ExportGroupAsText.Enabled = True
        End If
    End Sub

    Private Sub AdvancedToolStripMenuItem_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        UpdateGUI()
    End Sub

    Private Sub UpdateGUI()
        ImportBox.Visible = AdvancedToolStripMenuItem.Checked
        AdvancedBox1.Visible = AdvancedToolStripMenuItem.Checked
        AdvancedBox2.Visible = AdvancedToolStripMenuItem.Checked
        AdvancedBox3.Visible = AdvancedToolStripMenuItem.Checked

    End Sub

    Private Sub AdvancedToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AdvancedToolStripMenuItem.Click
        If AdvancedToolStripMenuItem.Checked = False Then
            AdvancedToolStripMenuItem.Checked = True
        Else
            'Clear it
            AdvancedToolStripMenuItem.Checked = False
        End If
        UpdateGUI()
    End Sub

End Class

'Class holding options - set to defaults, and serialisable for file I/O
'DO NOT CHANGE ORDER OF THESE ITEMS, ADD NEW ONES TO THE END
<Serializable()> Public Class SetDefaults
    Public ServerName As String = "localhost"
    Public ServerPort As String = "5481"
    Public Username As String = "AdminExample"
    Public Password As String = "AdminExample"
    Public ExportFolder As String = "c:\ProgramData\Schneider Electric\ClearSCADA\ExportedChanges"
    Public Wildcard As String = "%"
    Public LastChanged As String = "01-Jan-2023 00:00:00"
    Public OmitGroups As Boolean = False
    Public ImportFolder As String = "c:\ProgramData\Schneider Electric\ClearSCADA\ImportChanges"
    Public FileWildcard As String = "*.sde"
    Public ImportOKFolder As String = "c:\ProgramData\Schneider Electric\ClearSCADA\ImportChangesDone"
    Public ExportFolder2 As String = ""
    Public ExportFolder3 As String = ""
    Public ExportFolder4 As String = ""
    Public BackupFolder As String = "c:\ProgramData\Schneider Electric\ClearSCADA\ImportChangesBackup"
    Public ImportDailyFolders As Boolean = True
    Public ExportDailyFolders As Boolean = True
    Public ImportFailFolder As String = "c:\ProgramData\Schneider Electric\ClearSCADA\ImportChangesFail"
    Public ExportConstraint As String = ""
    Public ExportConstraintValue As String = "True"
    Public ConvertToForceMerge As Boolean = False
    Public MapOPCTags As Boolean = False
    Public ForceGroupsOnly As Boolean = False
    Public DataTableName As String = "ImportChangeStatus"
    Public ConnectionName As String = "Main"
    Public ODBCDriver As String = "Control Microsystems ClearSCADA Driver"
    Public ExportGroupAsText As Boolean = True
    Public GUIAdvanced As Boolean = False
End Class
