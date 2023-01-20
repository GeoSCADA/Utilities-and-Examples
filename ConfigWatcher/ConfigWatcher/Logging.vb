Module Logging
    Class Logger
        Private Writer As System.IO.StreamWriter
        Private logstatus As Boolean
        Private LogPrepend As String

        Public Sub New(ByVal PrePend As String)
            LogPrepend = PrePend
            logstatus = False
        End Sub

        Public Sub StartLogging(ByVal basename As String)
            Dim FileSearch() As System.IO.FileInfo
            Dim BaseFolder As New System.IO.DirectoryInfo(basename)

            FileSearch = BaseFolder.GetFiles("*.log")

            If FileSearch.Length = 0 Then
                'case 1, no logs yet created
                Writer = System.IO.File.CreateText(basename & "\" & LogPrepend & "0.Log")
                logstatus = True
            ElseIf FileSearch.Length > 0 Then
                'case 2, less than 10 logs are created
                Dim currentlog As Integer = 0
                Dim logfound As Boolean = False
                Do While logfound = False And currentlog < 9
                    If System.IO.File.Exists(basename & "\" & LogPrepend & currentlog.ToString() & ".log") Then
                        Dim Logattrib As New System.IO.FileInfo(basename & "\" & LogPrepend & currentlog.ToString() & ".log")
                        If Logattrib.Length < 5242880 Then
                            Try
                                Writer = System.IO.File.AppendText(basename & "\" & LogPrepend & currentlog.ToString() & ".log")
                                logfound = True
                            Catch
                                'not found a file, probably in use
                            End Try
                        Else
                            currentlog += 1
                        End If
                    Else
                        'Create new file
                        Writer = System.IO.File.CreateText(basename & "\" & LogPrepend & currentlog.ToString() & ".log")
                        logfound = True
                    End If
                Loop
                logstatus = logfound
                If currentlog = 9 And logfound = False Then
                    Dim Firstlog As Integer = 0
                    Dim ThisFile As System.IO.FileInfo
                    currentlog = 0
                    For Each ThisFile In FileSearch
                        If FileSearch(currentlog).LastWriteTime < FileSearch(Firstlog).LastWriteTime Then
                            Firstlog = currentlog
                        End If
                        currentlog += 1
                    Next
                    Try
                        System.IO.File.Delete(basename & "\" & LogPrepend & Firstlog.ToString() & ".log")
                    Catch
                        'Cannot delete, possibly in use
                    End Try
                    Try
                        Writer = System.IO.File.CreateText(basename & "\" & LogPrepend & Firstlog.ToString() & ".log")
                    Catch
                        'Cannot create - unknown error, will not log
                    End Try

                End If
                logstatus = logfound
            End If

            If logstatus = True Then
                Writer.WriteLine("")
                Writer.WriteLine(LogPrepend & " Execution at " & Now.ToString())
                Writer.WriteLine("---------------------------------------------------------------------")
                Writer.Flush()
            End If

        End Sub

        Public Sub WriteToLog(ByVal message As String)
            If logstatus = True Then
                Console.WriteLine(message)
                Writer.WriteLine(Now.ToString() & ": " & message)
                Writer.Flush()
            End If
        End Sub

        Public Sub StopLogging()
            If logstatus = True Then
                Writer.WriteLine("LOGGING TERMINATED")
                Writer.WriteLine("---------------------------------------------------------------------")
                Writer.Flush()
                Writer.Close()
            End If
        End Sub
    End Class
End Module
