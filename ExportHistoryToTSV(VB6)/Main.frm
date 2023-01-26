VERSION 5.00
Begin VB.Form Main 
   Caption         =   "Export History To TSV"
   ClientHeight    =   8052
   ClientLeft      =   792
   ClientTop       =   804
   ClientWidth     =   8496
   Icon            =   "Main.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   8052
   ScaleWidth      =   8496
   Begin VB.TextBox DriverName 
      Height          =   288
      Left            =   1800
      TabIndex        =   24
      Text            =   "ClearSCADA Driver"
      Top             =   1200
      Width           =   2292
   End
   Begin VB.TextBox DelayTime 
      Height          =   288
      Left            =   1800
      TabIndex        =   22
      Text            =   "0"
      Top             =   2760
      Width           =   852
   End
   Begin VB.CommandButton ImportFiles 
      Caption         =   "Import from Files"
      Height          =   372
      Left            =   3720
      TabIndex        =   21
      Top             =   6600
      Width           =   1332
   End
   Begin VB.CommandButton SelectNone 
      Caption         =   "Select None"
      Height          =   372
      Left            =   1320
      TabIndex        =   20
      Top             =   3240
      Width           =   1212
   End
   Begin VB.CommandButton SelectAll 
      Caption         =   "Select All"
      Height          =   372
      Left            =   120
      TabIndex        =   19
      Top             =   3240
      Width           =   1212
   End
   Begin VB.TextBox EndTime 
      Height          =   288
      Left            =   1800
      TabIndex        =   18
      Text            =   "Text1"
      Top             =   2400
      Width           =   1812
   End
   Begin VB.TextBox StartTime 
      Height          =   288
      Left            =   1800
      TabIndex        =   17
      Text            =   "Text1"
      Top             =   2040
      Width           =   1812
   End
   Begin VB.TextBox NameFilter 
      Height          =   288
      Left            =   1800
      TabIndex        =   14
      Text            =   "%"
      Top             =   1680
      Width           =   1812
   End
   Begin VB.CommandButton ReadTables 
      Caption         =   "Read Points"
      Height          =   372
      Left            =   3720
      TabIndex        =   12
      Top             =   1680
      Width           =   1212
   End
   Begin VB.TextBox DatabaseName 
      Height          =   288
      Index           =   1
      Left            =   1440
      TabIndex        =   11
      Text            =   "c:\Historic Data Export"
      Top             =   6240
      Width           =   6972
   End
   Begin VB.TextBox Doing 
      Enabled         =   0   'False
      Height          =   288
      Left            =   120
      TabIndex        =   10
      Top             =   7200
      Width           =   8292
   End
   Begin VB.ListBox MatchList 
      Height          =   2424
      ItemData        =   "Main.frx":1982
      Left            =   120
      List            =   "Main.frx":1984
      Style           =   1  'Checkbox
      TabIndex        =   9
      Top             =   3600
      Width           =   8292
   End
   Begin VB.CommandButton ExitBtn 
      Caption         =   "Exit"
      Height          =   372
      Left            =   3720
      TabIndex        =   8
      Top             =   7560
      Width           =   1332
   End
   Begin VB.CommandButton CopyTables 
      Caption         =   "Export to Files"
      Enabled         =   0   'False
      Height          =   372
      Left            =   3720
      TabIndex        =   0
      Top             =   3000
      Width           =   1212
   End
   Begin VB.TextBox DatabasePassword 
      Height          =   288
      IMEMode         =   3  'DISABLE
      Index           =   0
      Left            =   1800
      PasswordChar    =   "*"
      TabIndex        =   3
      Top             =   840
      Width           =   972
   End
   Begin VB.TextBox DatabaseUser 
      Height          =   288
      Index           =   0
      Left            =   1800
      TabIndex        =   2
      Top             =   480
      Width           =   972
   End
   Begin VB.TextBox DatabaseName 
      Height          =   288
      Index           =   0
      Left            =   1800
      TabIndex        =   1
      Top             =   120
      Width           =   972
   End
   Begin VB.Label Label4 
      BackColor       =   &H00C0FFFF&
      Caption         =   "Date/Time, Value, (state), Quality, Reason"
      Height          =   252
      Left            =   5160
      TabIndex        =   27
      Top             =   6720
      Width           =   3132
   End
   Begin VB.Label VersionLabel 
      Caption         =   "05/01/12"
      Height          =   252
      Left            =   6960
      TabIndex        =   26
      Top             =   120
      Width           =   1452
   End
   Begin VB.Label Label3 
      Caption         =   "ODBC Driver"
      Height          =   252
      Left            =   600
      TabIndex        =   25
      Top             =   1200
      Width           =   1092
   End
   Begin VB.Label Label2 
      BackStyle       =   0  'Transparent
      Caption         =   "Delay (millisec)"
      Height          =   252
      Left            =   600
      TabIndex        =   23
      Top             =   2760
      Width           =   1212
   End
   Begin VB.Label Label1 
      BackStyle       =   0  'Transparent
      Caption         =   "End Date"
      Height          =   252
      Index           =   7
      Left            =   600
      TabIndex        =   16
      Top             =   2400
      Width           =   1092
   End
   Begin VB.Label Label1 
      BackStyle       =   0  'Transparent
      Caption         =   "Start Date"
      Height          =   252
      Index           =   6
      Left            =   600
      TabIndex        =   15
      Top             =   2040
      Width           =   1092
   End
   Begin VB.Label Label1 
      BackStyle       =   0  'Transparent
      Caption         =   "Full Name Filter (SQL)"
      Height          =   252
      Index           =   5
      Left            =   120
      TabIndex        =   13
      Top             =   1680
      Width           =   1572
   End
   Begin VB.Label Label1 
      BackStyle       =   0  'Transparent
      Caption         =   "Target Folder"
      Height          =   252
      Index           =   4
      Left            =   120
      TabIndex        =   7
      Top             =   6240
      Width           =   1212
   End
   Begin VB.Label Label1 
      Caption         =   "Password"
      Height          =   252
      Index           =   2
      Left            =   840
      TabIndex        =   6
      Top             =   840
      Width           =   852
   End
   Begin VB.Label Label1 
      Caption         =   "User"
      Height          =   252
      Index           =   1
      Left            =   1080
      TabIndex        =   5
      Top             =   480
      Width           =   612
   End
   Begin VB.Label Label1 
      Caption         =   "Database Name"
      Height          =   252
      Index           =   3
      Left            =   360
      TabIndex        =   4
      Top             =   120
      Width           =   1332
   End
   Begin VB.Shape Shape1 
      BorderColor     =   &H00800000&
      BorderStyle     =   0  'Transparent
      BorderWidth     =   2
      FillColor       =   &H00C0C0FF&
      FillStyle       =   0  'Solid
      Height          =   4812
      Left            =   0
      Top             =   1560
      Width           =   8532
   End
   Begin VB.Shape Shape2 
      BorderColor     =   &H00800000&
      BorderStyle     =   0  'Transparent
      BorderWidth     =   2
      FillColor       =   &H00C0FFFF&
      FillStyle       =   0  'Solid
      Height          =   732
      Left            =   0
      Top             =   6360
      Width           =   8532
   End
End
Attribute VB_Name = "Main"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Dim NCX(1) As ADODB.Connection

Dim LogFile As String
Dim LogFileHandle As Integer
Dim DefaultSelections As String





Private Sub ReadTables_Click()
    Dim i As Long
    Dim SQL As String
    Dim RSA As ADODB.Recordset
    
    SaveOptions
    
    Open LogFile For Append As #LogFileHandle

    On Error GoTo NoConnection
    
    LogDoing "Starting up"
    For i = 0 To 0
        Set NCX(i) = New ADODB.Connection
        LogDoing "Connect to SCX SQL " & DatabaseName(i)
        NCX(i).Open "DRIVER={" & DriverName & "};SERVER=" & DatabaseName(i) & ";UID=" & DatabaseUser(i) & ";PWD=" & DatabasePassword(i)
    Next
    
    On Error GoTo 0
    
    LogDoing "Getting List"
    If NameFilter = "" Then NameFilter = "%"
    SQL = "SELECT O.FullName As OName FROM CHistory H INNER JOIN CDBObject O ON H.Id=O.Id WHERE OName LIKE '" & NameFilter & "' ORDER BY OName"
    Set RSA = New ADODB.Recordset
    RSA.Open SQL, NCX(0)
    
    MatchList.Clear
    i = 1
    Do While Not RSA.EOF
        i = i + 1
        MatchList.AddItem RSA![OName]
        MatchList.Selected(MatchList.ListCount - 1) = False
        RSA.MoveNext
        If (i Mod 100) = 0 Then DoEvents
    Loop
    RSA.Close
    
    'Load user table selections
    LogDoing "Set to last-run selection"
    For i = MatchList.ListCount - 1 To 0 Step -1
        If InStr(1, DefaultSelections, MatchList.List(i)) > 0 Then
            MatchList.Selected(i) = True
        End If
        DoEvents
    Next
    
    LogDoing "End Read"
    Close #LogFileHandle
    
    CopyTables.Enabled = True
    CopyTables.Default = True
    CopyTables.SetFocus
    Exit Sub
    
NoConnection::
    MsgBox "Cannot connect to database - check name and password", vbOKOnly
    Close #LogFileHandle
    
End Sub


Private Sub SelectAll_Click()
Dim i
    For i = MatchList.ListCount - 1 To 0 Step -1
        MatchList.Selected(i) = True
        DoEvents
    Next
End Sub


Private Sub SelectNone_Click()
Dim i
    For i = MatchList.ListCount - 1 To 0 Step -1
        MatchList.Selected(i) = False
        DoEvents
    Next
End Sub

Private Sub CopyTables_Click()
    Dim i As Integer
    Dim j As Long
    Dim k As Integer
    Dim SQL As String
    Dim SQLt As String
    Dim SQLf As String
    Dim RSA As ADODB.Recordset
    Dim RSAt As ADODB.Recordset
    Dim Field As ADODB.Field
    Dim CopyTable As Boolean
    Dim a As Integer
    Dim SourceRecordCount As Long
    Dim SQLConstraint As String
    Dim FieldOK As Boolean
    
    'Saves selected tables
    DefaultSelections = ""
    For i = 0 To MatchList.ListCount - 1
        If MatchList.Selected(i) = True Then
            DefaultSelections = DefaultSelections + "\" + MatchList.List(i)
        End If
        DoEvents
    Next
    SaveOptions
    
    Open LogFile For Append As #LogFileHandle

    On Error GoTo NoConnection
    
    LogDoing "Starting up"
    For i = 0 To 0
        Set NCX(i) = New ADODB.Connection
        LogDoing "Connect to SCX SQL " & DatabaseName(i)
        NCX(i).Open "DRIVER={" & DriverName & "};SERVER=" & DatabaseName(i) & ";UID=" & DatabaseUser(i) & ";PWD=" & DatabasePassword(i)
    Next
    
    On Error GoTo 0
    
    For i = 0 To MatchList.ListCount - 1
        If MatchList.Selected(i) = True Then
            LogDoing "Checking Table " & MatchList.List(i)
            CopyTable = True
            
            SQLConstraint = """RecordTime"" BETWEEN {TS '" & Format(StartTime, "YYYY-MM-DD HH:MM:SS") & "'} AND {TS '" & Format(EndTime, "YYYY-MM-DD HH:MM:SS") & "'}"
            
            'Count Source Records
            SQL = "SELECT COUNT(0) AS C FROM CDBHISTORIC H INNER JOIN CDBObject O ON H.Id=O.Id WHERE O.FullName = '" & MatchList.List(i) & "'  And " & SQLConstraint
            Set RSA = New ADODB.Recordset
            RSA.Open SQL, NCX(0)
            SourceRecordCount = RSA![C]
            RSA.Close
            
            If SourceRecordCount > 0 Then
                'first check file does not exist, otherwise ask
                SQL = Dir(DatabaseName(1) & "\" & Replace(MatchList.List(i), "*", "$") & ".txt")
                If SQL <> "" Then
                    'has records
                    a = MsgBox("Target File " & DatabaseName(1) & Replace(MatchList.List(i), "*", "$") & ".txt exists." & Chr$(10) & _
                               "Source Table " & MatchList.List(i) & " has " & SourceRecordCount & " records." & Chr$(10) & _
                               "Do you wish to delete Target File before copying ?" & Chr$(10) & _
                               "(Select OK to overwrite file or Cancel to ignore this table)", vbOKCancel)
                    If a = 1 Then
                        'delete target file
                        Kill DatabaseName(1) & "\" & Replace(MatchList.List(i), "*", "$") & ".txt"
                    Else
                        'Ignore this table
                        CopyTable = False
                    End If
                End If
                
                LogDoing "Copying rows " & MatchList.List(i)
                If CopyTable Then
                    SQL = "SELECT ""RecordTime"", ""ValueAsReal"", ""StateDesc"", ""QualityDesc"", ""ReasonDesc"" FROM CDBHISTORIC H INNER JOIN CDBObject O ON H.Id=O.Id WHERE O.FullName = '" & MatchList.List(i) & "' And " & SQLConstraint & " Order By ""RecordTime"" ASC"
                    Set RSA = New ADODB.Recordset
                    RSA.Open SQL, NCX(0)
                    
                    'Start by opening file
                    Open DatabaseName(1) & "\" & Replace(MatchList.List(i), "*", "$") & ".txt" For Output As #7
                    
                    'Then output the field names
                    SQLf = ""
                    For Each Field In RSA.Fields
                        SQLf = SQLf & """" & Field.Name & """" & Chr(9)
                    Next
                    SQLf = Left(SQLf, Len(SQLf) - 1)
                    Print #7, SQLf
                    
                    j = 0
                    Do While Not RSA.EOF
                        j = j + 1
                        ShowDoing "Exporting table " & MatchList.List(i) & " row " & j
                        'Then output each record
                        SQLt = ""
                        For Each Field In RSA.Fields
                            'Check data type
                            FieldOK = True
                            
                            On Error GoTo FieldError
                            FieldOK = (Field.Type <> -1) And Len(Field.Value) >= 0
                            
                            GoTo FieldErrorEnd
FieldError::
                            'Error with data - ignore it
                            LogDoing "Err" & Field.Name
                            FieldOK = False
                            Resume Next
FieldErrorEnd::
                            If FieldOK Then
                                                        
                            
                                If Field.Type = adVarChar Then
                                    SQLt = SQLt & """"
                                    For k = 1 To Len(Field.Value)
                                        Select Case Asc(Mid(Field.Value, k, 1))
                                            Case 0
                                                LogDoing "Row " & Str(j) & ". Ignoring NULL in field " & Field.Name & ". Is string field too short?"
                                            Case Asc("""")
                                                SQLt = SQLt & """"""
                                            Case Else
                                                SQLt = SQLt & Mid(Field.Value, k, 1)
                                        End Select
                                    Next
                                    SQLt = SQLt & """" & Chr(9)
                                ElseIf Field.Type = adDBTimeStamp Then
                                    SQLt = SQLt & Format(Field.Value, "YYYY-MM-DD HH:MM:SS") & Chr(9)
                                Else
                                    SQLt = SQLt & Field.Value & Chr(9)
                                End If
                            Else
                                SQLt = SQLt & """""" & Chr(9) 'Null

                            End If

                            DoEvents
                        Next
                        'Remove last set of ","
                        SQLt = Left(SQLt, Len(SQLt) - 1)
                        
                        DoEvents
                        'Insert
                        Print #7, SQLt
                        
                        RSA.MoveNext
                    Loop
                    RSA.Close
                    Close #7
                    MatchList.List(i) = MatchList.List(i) & ", " & Str(j) & " records exported"
                    LogDoing "Exported: " & MatchList.List(i) & ", " & Str(j) & " records exported"
                Else
                    MatchList.List(i) = "Not exported: " & MatchList.List(i)
                End If
            Else
                LogDoing "Not Exported as no records: " & MatchList.List(i)
                MatchList.List(i) = MatchList.List(i) & ", " & Str(0) & " records found, no file created"
            End If
        End If
        DoEvents
        If IsNumeric(DelayTime) Then
            Sleep (DelayTime)
        End If
    Next
    
    LogDoing "End Write"
    Close #LogFileHandle
    
    CopyTables.Enabled = False 'As we have overwritten the names
    
    ExitBtn.Default = True
    ExitBtn.SetFocus
    Exit Sub
NoConnection::
    MsgBox "Cannot connect to database - check name and password", vbOKOnly
    Close #LogFileHandle
    
End Sub

Private Sub ImportFiles_Click()
    Dim Fin As String
    Dim T As String
    Dim L() As String
    Dim Conn As New ScxV6Server
    Dim Obj As New ScxV6Object
    Dim ObjName As String
    Dim i As Integer
    Dim State As Long
    Dim a As Integer
    
    Dim RecordTime As Date
    Dim Value As Double
    Dim ValStr As String
    Dim Quality As Long
    Dim Reason As Long
    
    'Batch up for performance reasons
    ReDim RecordTimes(0) As Date
    ReDim Values(0) As Double
    ReDim Qualities(0) As Long
    ReDim Reasons(0) As Long
    
    SaveOptions
    
    a = MsgBox("Are you sure you wish to import data? This will merge new data with any existing data.", vbOKCancel)
    If a <> 1 Then
        Exit Sub
    End If
    
    Open LogFile For Append As #LogFileHandle

    On Error GoTo NoConnection
    
    LogDoing "Starting up"
    For i = 0 To 0
        Set NCX(i) = New ADODB.Connection
        LogDoing "Connect to SCX SQL " & DatabaseName(i)
        NCX(i).Open "DRIVER={" & DriverName & "};SERVER=" & DatabaseName(i) & ";UID=" & DatabaseUser(i) & ";PWD=" & DatabasePassword(i)
    Next
    
    LogDoing "Import started"
    Conn.Connect DatabaseName(0), DatabaseUser(0), DatabasePassword(0)
    LogDoing "Connected"
    
    On Error GoTo 0
    
    If Right(DatabaseName(1), 1) = "\" Then
        DatabaseName(1) = Left(DatabaseName(1), Len(DatabaseName(1)) - 1)
    End If
    
    Fin = Dir$(DatabaseName(1) & "\*.txt")
    Do While Fin <> ""
        LogDoing "Processing: " & Fin
        
        ObjName = Replace(Left(Fin, InStrRev(Fin, ".") - 1), "$", "*")
        LogDoing "Find Object: " & ObjName
        
        On Error GoTo cannotfindobject
        
        Set Obj = Conn.FindObject(ObjName)
        On Error GoTo 0
        
        Open DatabaseName(1) & "\" & Fin For Input As #8
        i = 0
        Do While Not EOF(8)
            
            Line Input #8, T
            If InStr(T, "RecordTime") = 0 Then 'not Header line
                'Divide line into tab-sep fields
                L = Split(T, Chr(9))
                
                '05/01/2012 SRB
                'Handle CSV as an alternative
                If (UBound(L) = 0) Then
                    L = Split(T, ",")
                End If
                'Handle date wrapped in ""
                If Left(L(0), 1) = """" And Right(L(0), 1) = """" Then
                    If Len(L(0)) - 2 > 0 Then
                        L(0) = Mid(L(0), 2, Len(L(0)) - 2)
                    End If
                End If
                
                ' ""RecordTime"", ""ValueAsReal"", ""StateDesc"", ""QualityDesc"", ""ReasonDesc""
                
                If IsDate(L(0)) And (UBound(L) >= 1) Then
                    RecordTime = CDate(L(0))
                    ValStr = Trim(L(1))
                    If IsNumeric(ValStr) Then
                    
                        Value = CDbl(ValStr)
                        
                        ''State As Long
                        
                        Quality = 192 'CLng(L(3))
                        If UBound(L) >= 3 Then
                            Select Case L(3)
                                Case "SubNormal"
                                    Quality = 88
                                Case "Good"
                                    Quality = 192
                                Case "Bad"
                                    Quality = 0
                                Case "Local Override"
                                    Quality = 216
                                Case "Engineering Units Exceeded"
                                    Quality = 84
                                Case "Error"
                                    Quality = 4
                            End Select
                        End If
                        
                        Reason = 3 'CLng(L(4))
                        ''CDR', 'Value Change', 'State Change', 'Report', 'End of Period', 'End of Period Reset', 'Override', 'Release'
                        If UBound(L) >= 4 Then
                            Select Case L(4)
                                Case "Current Data"
                                    Reason = 0
                                Case "CDR"
                                    Reason = 0
                                Case "Value Change"
                                    Reason = 1
                                Case "State Change"
                                    Reason = 2
                                Case "Report"
                                    Reason = 3
                                Case "Timed Report"
                                    Reason = 3
                                Case "End of Period"
                                    Reason = 4
                                Case "End of Period Reset"
                                    Reason = 5
                                Case "Override"
                                    Reason = 6
                                Case "Release"
                                    Reason = 7
                                Case "Release Override"
                                    Reason = 7
                                Case "Modified/Inserted"
                                    Reason = 8
                                Case "Modified"
                                    Reason = 8
                                Case "Inserted"
                                    Reason = 8
                            End Select
                        End If
                        
                        LogDoing "Setting: Reason, Quality, RecordTime, Value to: " & Reason & "," & Quality & "," & Format(RecordTime, "yyyy-MMM-dd HH:mm:ss") & "," & Value
                        If Reason <> 8 Then
                            'Store this for batching
                            ReDim Preserve RecordTimes(i) As Date
                            ReDim Preserve Values(i) As Double
                            ReDim Preserve Qualities(i) As Long
                            ReDim Preserve Reasons(i) As Long
                            Reasons(i) = Reason
                            Qualities(i) = Quality
                            RecordTimes(i) = RecordTime
                            Values(i) = Value
                            i = i + 1
                        
                            'If batch size is getting big then process and reduce its size
                            'Process batch
                            If i > 10000 Then
                                LogDoing "Batch of " & i
                                On Error GoTo cannotloadvaluebatchb
                                Obj.Interface.Historic.LoadDataValues Values, RecordTimes, Qualities, Reasons
                                ''Obj.Interface.Historic.LoadDataValue Reason, Quality, RecordTime, Value
                                On Error GoTo 0
                                GoTo cannotloadvaluebatchbOK
cannotloadvaluebatchb::
                                LogDoing "Cannot load batch of values " & Err.Description
                                Debug.Print Err.Description
                                Resume Next
cannotloadvaluebatchbOK::
                                i = 0
                            End If
                        
                        Else
                            On Error GoTo cannotloadvalue
                            Obj.Interface.Historic.ModifyValue RecordTime, Value, Quality
                            On Error GoTo 0
                        End If
                        GoTo cannotloadvalueOK
cannotloadvalue::
                        LogDoing "Cannot load modified value " & Err.Description
                        Resume Next
cannotloadvalueOK::
                    Else
                        LogDoing "Field is not a number: " & L(1)
                    End If
                Else
                    LogDoing "Field is not a date or insufficient fields: " & L(0)
                End If
            End If
            DoEvents
        Loop
        
        'Process remaining in batch
        If i > 0 Then
            LogDoing "Batch of " & i
            On Error GoTo cannotloadvaluebatch
            Obj.Interface.Historic.LoadDataValues Values, RecordTimes, Qualities, Reasons
            On Error GoTo 0
            GoTo cannotloadvaluebatchOK
cannotloadvaluebatch::
            LogDoing "Cannot load batch of values " & Err.Description
            Resume Next
cannotloadvaluebatchOK::
            i = 0
        End If
                    
        GoTo cannotfindobjectOK
cannotfindobject::
        LogDoing "Cannot Find Object: " & ObjName
        MsgBox "Cannot Find Object:" & ObjName
        'Resume Next
cannotfindobjectOK::
        Close #8
        DoEvents
        If IsNumeric(DelayTime) Then
            Sleep (DelayTime)
        End If
        Fin = Dir$
    Loop
    DoEvents
    LogDoing "Completed Import"
    Close #LogFileHandle
    Exit Sub
    
NoConnection::
    MsgBox "Cannot connect to database - check name and password", vbOKOnly
    Close #LogFileHandle
    
End Sub


Private Sub ExitBtn_Click()
    End
End Sub

Private Sub Form_Load()
    VersionLabel = App.Major & "." & App.Minor & "." & App.Revision
    LogFileHandle = FreeFile
    LogFile = App.Path & "\ExportSCXHistoryToTSV " & Format(Now, "dd-MMM-yyyy hh-mm-ss") & ".Log"
    StartTime = CDate(CLng(Now) - 7)
    EndTime = CDate(CLng(Now) + 1)
    LoadOptions
    'If command params specify scheduled export
    If InStr(Command$, "Export") > 0 Then
        'Export data
        
        'Alter start and end date - move on by the difference
        Dim OldStart As Date
        OldStart = CDate(StartTime)
        StartTime = CStr(CDate(EndTime))
        EndTime = CStr(CDate(EndTime) + CDate(EndTime) - CDate(OldStart))
        
        ReadTables_Click
        CopyTables_Click
        ExitBtn_Click
    End If
End Sub

Private Sub LogDoing(T As String)
    Doing = T
    Print #LogFileHandle, Now & ", " & T
    DoEvents
End Sub


Private Sub ShowDoing(T As String)
    Doing = T
    'Print #LogFileHandle, Now & ", " & T
    DoEvents
End Sub


'Save common options between runs
Private Sub SaveOptions()
    'Options to Save
    Dim F As Integer
    
    F = FreeFile
    
    Open App.Path & "\ExportSCXHistoryToTSV.ini" For Output As #F
    
    Write #F, DatabaseName(0)
    Write #F, DatabaseUser(0)
    Write #F, DatabasePassword(0)
    Write #F, DatabaseName(1)
    Write #F, DefaultSelections
    Write #F, NameFilter
    Write #F, DriverName
    Write #F, ""
    Write #F, ""
    
    Close #F
    
End Sub

Private Sub LoadOptions()
    'Options to Load
    Dim F As Integer
    Dim s As String
    Dim i As Integer
    
    If Dir$(App.Path & "\ExportSCXHistoryToTSV.ini") <> "" Then
        F = FreeFile
        Open App.Path & "\ExportSCXHistoryToTSV.ini" For Input As #F
        
        On Error GoTo FileEnd
        
        Input #F, s
        DatabaseName(0) = s
        Input #F, s
        DatabaseUser(0) = s
        Input #F, s
        DatabasePassword(0) = s
        Input #F, s
        DatabaseName(1) = s
        Input #F, s
        DefaultSelections = s
        Input #F, s
        NameFilter = s
        Input #F, s
        DriverName = s
        If DriverName = "" Then
            DriverName = "Serck Controls Scx V6 Driver"
        End If
        
FileEnd::
        On Error GoTo 0
        
        Close #F
    End If
End Sub



Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
    End
End Sub

