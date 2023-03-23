Imports System.IO
'BinaryFormatted should not be used - please replace with JSON
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Form1
    Private Defaults As New SetDefaults
    Private OptionFile As String
    Private StopProcessing As Boolean = False
    Private StartNum As Long
    Private EndNum As Long
    Private SizeBytes As Long
    Private HisCount As Long
    Private JnlCount As Long
    Private HisBytes As Long
    Private JnlBytes As Long

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Set default optionfile name
        OptionFile = AppPath() & "CopyHis.ini"

        'Read settings from file
        If File.Exists(OptionFile) Then
            Dim myFileStream As Stream = File.OpenRead(OptionFile)
            Dim deserializer As New BinaryFormatter()
            Defaults = CType(deserializer.Deserialize(myFileStream), SetDefaults)
            myFileStream.Close()
            'Get from serialised structure
            FromDate.Value = Defaults.FromDate
            ToDate.Value = Defaults.ToDate
            DatabaseFolder.Text = Defaults.DatabaseFolder
            DestinationFolder.Text = Defaults.DestinationFolder
            CopyHistory.Checked = Defaults.CopyHistory
            CopyJournal.Checked = Defaults.CopyJournal
            HistoryFolder.Text = Defaults.HistoryFolder
            JournalFolder.Text = Defaults.JournalFolder
        Else
            FromDate.Value = Now.AddDays(-31)
            ToDate.Value = Now.AddDays(-1)
            DatabaseFolder.Text = "C:\ProgramData\Schneider Electric\ClearSCADA\Database"
            DestinationFolder.Text = "C:\ProgramData\Schneider Electric\ClearSCADA\Database-Copy"
            CopyHistory.Checked = True
            CopyJournal.Checked = True
            HistoryFolder.Text = "History"
            JournalFolder.Text = "Journal"
        End If
    End Sub
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        EndProgram()
    End Sub

    Private Sub EndProgram()
        'Save settings
        Defaults.FromDate = FromDate.Value
        Defaults.ToDate = ToDate.Value
        Defaults.DatabaseFolder = DatabaseFolder.Text
        Defaults.DestinationFolder = DestinationFolder.Text
        Defaults.CopyHistory = CopyHistory.Checked
        Defaults.CopyJournal = CopyJournal.Checked
        Defaults.HistoryFolder = HistoryFolder.text
        Defaults.JournalFolder = JournalFolder.text
        'Serialise and write
        Dim optionfile As String
        optionfile = AppPath() & "CopyHis.ini"
        Dim myFileStream As Stream = File.Create(optionfile)
        Dim serializer As New BinaryFormatter()
        serializer.Serialize(myFileStream, Defaults)
        myFileStream.Close()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox("CopyHis, Steve Beadle, July 2009", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "CopyHis")
    End Sub

    '* Get directory of executable file (includes \ at end)
    Private Function AppPath() As String
        Dim assembly As String = System.Reflection.Assembly.GetEntryAssembly().Location
        AppPath = Path.GetDirectoryName(assembly) & "\"
    End Function

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        EndProgram()
        End
    End Sub

    Private Sub DoSizeHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DoSizeHistory.Click
        Me.UseWaitCursor = True
        StatusLabel.Text = "Sizing"
        StopProcessing = False
        HisBytes = 0
        JnlBytes = 0
        HisCount = 0
        JnlCount = 0
        SizeBytes = 0

        If CopyHistory.Checked = True Then
            'Carefully worked out to match SCX file numbering by trial and error
            StartNum = CLng((FromDate.Value.Subtract(CDate("1 Jan 1601")).Days - 7 + 4) / 7)
            EndNum = CLng((ToDate.Value.Subtract(CDate("1 Jan 1601")).Days - 7 + 4) / 7)
            DoSize(DatabaseFolder.Text & "\" & HistoryFolder.text & "\", "WK", ".HRD")
        End If
        If CopyJournal.Checked = True Then
            'Carefully worked out to match SCX file numbering by trial and error
            StartNum = CLng((FromDate.Value.Subtract(CDate("1 Jan 1601")).Days * 24) + 14) - 14
            EndNum = CLng((ToDate.Value.Subtract(CDate("1 Jan 1601")).Days * 24) + 14) - 14 + 23
            DoSize(DatabaseFolder.Text & "\" & JournalFolder.text & "\", "H", ".MWJ")
        End If

        Me.UseWaitCursor = False
        StatusLabel.Text = "Total: " & SizeBytes.ToString & " His: " & HisBytes.ToString & " Jnl: " & JnlBytes.ToString & " His Count: " & HisCount.ToString & " Jnl Count: " & JnlCount.ToString
    End Sub

    Private Sub DoCopyHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DoCopyHistory.Click

        Me.UseWaitCursor = True
        StatusLabel.Text = "Copying"
        StopProcessing = False

        If CopyHistory.Checked = True Then
            'Carefully worked out to match SCX file numbering by trial and error
            StartNum = CLng((FromDate.Value.Subtract(CDate("1 Jan 1601")).Days - 7 + 4) / 7)
            EndNum = CLng((ToDate.Value.Subtract(CDate("1 Jan 1601")).Days - 7 + 4) / 7)
            DoCopy(DatabaseFolder.Text & "\" & HistoryFolder.text & "\", "WK", ".HRD")
        End If
        If CopyJournal.Checked = True Then
            'Carefully worked out to match SCX file numbering by trial and error
            StartNum = CLng((FromDate.Value.Subtract(CDate("1 Jan 1601")).Days * 24) + 14) - 14
            EndNum = CLng((ToDate.Value.Subtract(CDate("1 Jan 1601")).Days * 24) + 14) - 14 + 23
            DoCopy(DatabaseFolder.Text & "\" & JournalFolder.Text & "\", "H", ".MWJ")
        End If

        Me.UseWaitCursor = False
        StatusLabel.Text = "Idle"
    End Sub

    Private Sub DoDeleteHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DoDeleteHistory.Click
        Dim yn As MsgBoxResult
        yn = MsgBox("Delete the selected files between date ranges?", MsgBoxStyle.YesNo + MsgBoxStyle.Exclamation, "CopyHis")
        If yn = MsgBoxResult.Yes Then
            yn = MsgBox("Are you sure you want to delete the selected files between date ranges?", MsgBoxStyle.YesNo + MsgBoxStyle.Exclamation, "CopyHis")
            If yn = MsgBoxResult.Yes Then
                yn = MsgBox("Are you REALLY sure you want to delete the selected files between date ranges? (This is your last chance!)", MsgBoxStyle.YesNo + MsgBoxStyle.Exclamation, "CopyHis")
                If yn = MsgBoxResult.Yes Then
                    Me.UseWaitCursor = True
                    StatusLabel.Text = "Deleting"
                    StopProcessing = False

                    If CopyHistory.Checked = True Then
                        'Carefully worked out to match SCX file numbering by trial and error
                        StartNum = CLng((FromDate.Value.Subtract(CDate("1 Jan 1601")).Days - 7 + 4) / 7)
                        EndNum = CLng((ToDate.Value.Subtract(CDate("1 Jan 1601")).Days - 7 + 4) / 7)
                        DoDelete(DatabaseFolder.Text & "\" & HistoryFolder.text & "\", "WK", ".HRD")
                    End If
                    If CopyJournal.Checked = True Then
                        'Carefully worked out to match SCX file numbering by trial and error
                        StartNum = CLng((FromDate.Value.Subtract(CDate("1 Jan 1601")).Days * 24) + 14) - 14
                        EndNum = CLng((ToDate.Value.Subtract(CDate("1 Jan 1601")).Days * 24) + 14) - 14 + 23
                        DoDelete(DatabaseFolder.Text & "\" & JournalFolder.text & "\", "H", ".MWJ")
                    End If

                    Me.UseWaitCursor = False
                    StatusLabel.Text = "Idle"
                End If
            End If
        End If
    End Sub

    Private Sub DoSize(ByVal Folder As String, ByVal Prefix As String, ByVal Extension As String)
        Dim myDirectory As System.IO.DirectoryInfo

        myDirectory = New DirectoryInfo(Folder)
        SizeWithDirectory(myDirectory, Prefix, Extension)
    End Sub

    Private Sub DoCopy(ByVal Folder As String, ByVal Prefix As String, ByVal Extension As String)
        Dim myDirectory As System.IO.DirectoryInfo

        myDirectory = New DirectoryInfo(Folder)
        WorkWithDirectory(myDirectory, Prefix, Extension)
    End Sub

    Private Sub DoDelete(ByVal Folder As String, ByVal Prefix As String, ByVal Extension As String)
        Dim myDirectory As System.IO.DirectoryInfo

        myDirectory = New DirectoryInfo(Folder)
        DeleteWithDirectory(myDirectory, Prefix, Extension)
    End Sub

    Private Sub SizeWithDirectory(ByVal aDir As DirectoryInfo, ByVal Prefix As String, ByVal Extension As String)
        Dim nextDir As DirectoryInfo
        SizeWithFilesInDir(aDir, Prefix, Extension)
        For Each nextDir In aDir.GetDirectories
            SizeWithDirectory(nextDir, Prefix, Extension)
            If StopProcessing Then
                Exit For
            End If
        Next
    End Sub

    Private Sub WorkWithDirectory(ByVal aDir As DirectoryInfo, ByVal Prefix As String, ByVal Extension As String)
        Dim nextDir As DirectoryInfo
        WorkWithFilesInDir(aDir, Prefix, Extension)
        For Each nextDir In aDir.GetDirectories
            WorkWithDirectory(nextDir, Prefix, Extension)
            If StopProcessing Then
                Exit For
            End If
        Next
    End Sub

    Private Sub DeleteWithDirectory(ByVal aDir As DirectoryInfo, ByVal Prefix As String, ByVal Extension As String)
        Dim nextDir As DirectoryInfo
        DeleteWithFilesInDir(aDir, Prefix, Extension)
        For Each nextDir In aDir.GetDirectories
            DeleteWithDirectory(nextDir, Prefix, Extension)
            If StopProcessing Then
                Exit For
            End If
        Next
    End Sub

    Private Sub WorkWithFilesInDir(ByVal aDir As DirectoryInfo, ByVal Prefix As String, ByVal Extension As String)
        Dim aFile As FileInfo
        Dim DestFile As String
        Dim DoneCreateDir As Boolean = False
        Dim FileNum As Long

        For Each aFile In aDir.GetFiles()
            If StopProcessing Then
                Exit For
            End If
            If aFile.Name.StartsWith(Prefix) And aFile.Name.EndsWith(Extension) Then
                'Check number range
                Try
                    FileNum = CLng(aFile.Name.Substring(Prefix.Length, aFile.Name.Length - Prefix.Length - Extension.Length))
                    If FileNum >= StartNum And FileNum <= EndNum Then
                        'Copy
                        DestFile = aFile.FullName.ToUpper.Replace(DatabaseFolder.Text.ToUpper, DestinationFolder.Text.ToUpper)
                        'Make Dir once
                        If Not DoneCreateDir Then
                            CreateDirectoryStructure(DestFile)
                            DoneCreateDir = True
                        End If
                        aFile.CopyTo(DestFile, True)
                        StatusLabel.Text = "Copied: " & DestFile
                    End If
                Catch
                    StatusLabel.Text = "Copy Fault with: " & aFile.FullName
                End Try
            End If
            Application.DoEvents()
        Next
    End Sub

    Private Sub SizeWithFilesInDir(ByVal aDir As DirectoryInfo, ByVal Prefix As String, ByVal Extension As String)
        Dim aFile As FileInfo
        Dim FileNum As Long
        Dim FileLen As Long

        For Each aFile In aDir.GetFiles()
            If StopProcessing Then
                Exit For
            End If
            If aFile.Name.StartsWith(Prefix) And aFile.Name.EndsWith(Extension) Then
                'Check number range
                Try
                    FileNum = CLng(aFile.Name.Substring(Prefix.Length, aFile.Name.Length - Prefix.Length - Extension.Length))
                    If FileNum >= StartNum And FileNum <= EndNum Then
                        'Size
                        FileLen = aFile.Length
                        If Extension = ".HRD" Then
                            HisCount += 1
                            HisBytes += FileLen
                        Else
                            JnlCount += 1
                            JnlBytes += FileLen
                        End If
                        SizeBytes = SizeBytes + FileLen
                        StatusLabel.Text = "Size so far: " & SizeBytes.ToString("000,000,000,000")
                    End If
                Catch
                    StatusLabel.Text = "Size Fault with: " & aFile.FullName
                End Try
            End If
            Application.DoEvents()
        Next
    End Sub

    Private Sub DeleteWithFilesInDir(ByVal aDir As DirectoryInfo, ByVal Prefix As String, ByVal Extension As String)
        Dim aFile As FileInfo
        Dim FileNum As Long

        For Each aFile In aDir.GetFiles()
            If StopProcessing Then
                Exit For
            End If
            If aFile.Name.StartsWith(Prefix) And aFile.Name.EndsWith(Extension) Then
                'Check number range
                Try
                    FileNum = CLng(aFile.Name.Substring(Prefix.Length, aFile.Name.Length - Prefix.Length - Extension.Length))
                    If FileNum >= StartNum And FileNum <= EndNum Then
                        'Delete
                        aFile.Delete()
                        StatusLabel.Text = "Deleted: " & aFile.FullName
                    End If
                Catch
                    StatusLabel.Text = "Delete Fault with: " & aFile.FullName
                End Try
            End If
            Application.DoEvents()
        Next
    End Sub

    Private Sub CreateDirectoryStructure(ByVal Folder As String)
        Dim LocalLoc
        Dim NewName As String
        LocalLoc = Split(Folder, "\")
        Dim CurrentFolder As String = LocalLoc(0)
        Dim i As Integer = 1

        While i < UBound(LocalLoc)
            NewName = LocalLoc(i)
            LocalLoc(i) = NewName.ToUpper.Substring(0, 1) & NewName.ToLower.Substring(1)
            CurrentFolder = CurrentFolder & "\" & LocalLoc(i)
            If Not System.IO.Directory.Exists(CurrentFolder) Then
                System.IO.Directory.CreateDirectory(CurrentFolder)
            End If
            i += 1
        End While
    End Sub

    Private Sub StopIt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StopIt.Click
        StopProcessing = True
    End Sub

End Class

'Class holding options - set to defaults, and serialisable for file I/O
<Serializable()> Public Class SetDefaults
    Public FromDate As Date
    Public ToDate As Date
    Public DatabaseFolder As String
    Public DestinationFolder As String
    Public CopyHistory As Boolean = True
    Public CopyJournal As Boolean = True
    Public HistoryFolder As String
    Public JournalFolder As String
End Class

