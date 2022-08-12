Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports ViewX
Imports ClearScada.Client
Imports ClearScada.Client.Simple
Imports ClearScada.Client.Advanced

Public Class Form1

    Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)
    Private xvw As New ViewX.Application

    Private Sub StartViewX(ByVal system As String, ByVal username As String, ByVal password As String)
        'try
        xvw.Logon(system, username, password)
        ''xvw.WindowState = ViewX.WndState.WndFullScreen ''No longer implemented
        'Hide this app's window
    End Sub

    Private Sub CaptureScreen(ByVal system As String, ByVal mimic As String, ByVal filename As String)
        Dim mim As Mimic

        Me.Hide()

        mim = xvw.Mimics.OpenFromServer(True, system, mimic)

        Dim bmpScreenshot As Bitmap
        Dim gfxScreenshot As Graphics

        bmpScreenshot = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb)
        gfxScreenshot = Graphics.FromImage(bmpScreenshot)

        Threading.Thread.Sleep(Int(txtTime.Text))

        gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy)

        bmpScreenshot.Save(filename & ".png", ImageFormat.Png)
        Threading.Thread.Sleep(500)

        On Error Resume Next 'Mim may not be closable!
        mim.Close()

        Me.Show()
#Disable Warning BC42025 ' Access of shared member, constant member, enum member or nested type through an instance
        If Me.ModifierKeys = Keys.Control Then
#Enable Warning BC42025 ' Access of shared member, constant member, enum member or nested type through an instance
            End
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Button1.Enabled = False

        StartViewX(txtSystem.Text, txtUsername.Text, txtPassword.Text)

        Dim Filename As String = ""

        If GetAll.CheckState = False Then
            Filename = txtMimic.Text
            Filename = Filename.Replace("*", "$")
            Filename = txtFolder.Text & "\" & Filename

            CaptureScreen(txtSystem.Text, txtMimic.Text, Filename)
        Else
            Dim Node As ClearScada.Client.ServerNode = New ClearScada.Client.ServerNode(txtNode.Text, txtPort.Text)
            Dim CSconnection As Connection = New Connection(txtSystem.Text)
            CSconnection.Connect(Node)
            Dim SPassword As New System.Security.SecureString
            For i As Integer = 0 To Len(txtPassword.Text) - 1
                SPassword.AppendChar(CChar(txtPassword.Text.Substring(i, 1)))
            Next

            CSconnection.LogOn(txtUsername.Text, SPassword)
            Dim Exclusions() As String = Split(txtExclude.Text, ",")

            Dim gobj As DBObject = CSconnection.GetObject(txtMimic.Text)
            Do
                RecurseGroups(gobj, Exclusions)
            Loop While chkRepeat.CheckState = True
        End If
        Button1.Enabled = True

    End Sub
    Private Sub RecurseGroups(ByRef gobj As DBObject, Exclusions() As String)
        Dim objs As DBObjectCollection = gobj.GetChildren("CMimic", "")
        CaptureMimics(objs, Exclusions)
        Dim gobjs As DBObjectCollection = gobj.GetChildren("CGroup", "")
        Dim obj As DBObject
        For Each obj In gobjs
            RecurseGroups(obj, Exclusions)
        Next
    End Sub
    Private Sub CaptureMimics(ByRef objs As DBObjectCollection, Exclusions() As String)
        Dim Filename As String = ""
        Dim obj As DBObject
        For Each obj In objs
            If obj.ClassDefinition.DisplayName = "Mimic" Then
                If Not Exclusions.Contains(obj.Name) Then
                    Filename = obj.FullName
                    Filename = Filename.Replace("*", "$")
                    Filename = txtFolder.Text & "\" & Filename
                End If
                CaptureScreen(txtSystem.Text, obj.FullName, Filename)
            End If
        Next

    End Sub
    Private Sub Quit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Quit.Click
        End
    End Sub

End Class

