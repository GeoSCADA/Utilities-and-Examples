<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.txtSystem = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtUsername = New System.Windows.Forms.TextBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtPassword = New System.Windows.Forms.TextBox()
		Me.Quit = New System.Windows.Forms.Button()
		Me.txtMimic = New System.Windows.Forms.TextBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.txtFolder = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.GetAll = New System.Windows.Forms.CheckBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.txtPort = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.txtNode = New System.Windows.Forms.TextBox()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.txtExclude = New System.Windows.Forms.TextBox()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.txtTime = New System.Windows.Forms.TextBox()
		Me.chkRepeat = New System.Windows.Forms.CheckBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.SuspendLayout()
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(125, 356)
		Me.Button1.Margin = New System.Windows.Forms.Padding(4)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(152, 43)
		Me.Button1.TabIndex = 8
		Me.Button1.Text = "Go"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'txtSystem
		'
		Me.txtSystem.Location = New System.Drawing.Point(125, 15)
		Me.txtSystem.Margin = New System.Windows.Forms.Padding(4)
		Me.txtSystem.Name = "txtSystem"
		Me.txtSystem.Size = New System.Drawing.Size(236, 22)
		Me.txtSystem.TabIndex = 0
		Me.txtSystem.Text = "Local"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(16, 15)
		Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(54, 17)
		Me.Label1.TabIndex = 3
		Me.Label1.Text = "System"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(16, 105)
		Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(73, 17)
		Me.Label2.TabIndex = 5
		Me.Label2.Text = "Username"
		'
		'txtUsername
		'
		Me.txtUsername.Location = New System.Drawing.Point(125, 105)
		Me.txtUsername.Margin = New System.Windows.Forms.Padding(4)
		Me.txtUsername.Name = "txtUsername"
		Me.txtUsername.Size = New System.Drawing.Size(236, 22)
		Me.txtUsername.TabIndex = 3
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(16, 137)
		Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(69, 17)
		Me.Label3.TabIndex = 7
		Me.Label3.Text = "Password"
		'
		'txtPassword
		'
		Me.txtPassword.Location = New System.Drawing.Point(125, 137)
		Me.txtPassword.Margin = New System.Windows.Forms.Padding(4)
		Me.txtPassword.Name = "txtPassword"
		Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
		Me.txtPassword.Size = New System.Drawing.Size(236, 22)
		Me.txtPassword.TabIndex = 4
		'
		'Quit
		'
		Me.Quit.Location = New System.Drawing.Point(285, 356)
		Me.Quit.Margin = New System.Windows.Forms.Padding(4)
		Me.Quit.Name = "Quit"
		Me.Quit.Size = New System.Drawing.Size(152, 43)
		Me.Quit.TabIndex = 9
		Me.Quit.Text = "Quit"
		Me.Quit.UseVisualStyleBackColor = True
		'
		'txtMimic
		'
		Me.txtMimic.Location = New System.Drawing.Point(125, 167)
		Me.txtMimic.Margin = New System.Windows.Forms.Padding(4)
		Me.txtMimic.Name = "txtMimic"
		Me.txtMimic.Size = New System.Drawing.Size(352, 22)
		Me.txtMimic.TabIndex = 5
		Me.txtMimic.Text = "Example Projects.Electricity.Generation"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(16, 167)
		Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(87, 17)
		Me.Label4.TabIndex = 11
		Me.Label4.Text = "Mimic/Group"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(16, 311)
		Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(69, 17)
		Me.Label5.TabIndex = 12
		Me.Label5.Text = "To Folder"
		'
		'txtFolder
		'
		Me.txtFolder.Location = New System.Drawing.Point(125, 311)
		Me.txtFolder.Margin = New System.Windows.Forms.Padding(4)
		Me.txtFolder.Name = "txtFolder"
		Me.txtFolder.Size = New System.Drawing.Size(352, 22)
		Me.txtFolder.TabIndex = 7
		Me.txtFolder.Text = "C:\TEMP\MimicDumps"
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(16, 197)
		Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(0, 17)
		Me.Label6.TabIndex = 14
		'
		'GetAll
		'
		Me.GetAll.AutoSize = True
		Me.GetAll.Checked = True
		Me.GetAll.CheckState = System.Windows.Forms.CheckState.Checked
		Me.GetAll.Location = New System.Drawing.Point(125, 196)
		Me.GetAll.Name = "GetAll"
		Me.GetAll.Size = New System.Drawing.Size(219, 21)
		Me.GetAll.TabIndex = 6
		Me.GetAll.Text = "Click to get all mimics in group"
		Me.GetAll.UseVisualStyleBackColor = True
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(16, 75)
		Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(34, 17)
		Me.Label7.TabIndex = 16
		Me.Label7.Text = "Port"
		'
		'txtPort
		'
		Me.txtPort.Location = New System.Drawing.Point(125, 75)
		Me.txtPort.Margin = New System.Windows.Forms.Padding(4)
		Me.txtPort.Name = "txtPort"
		Me.txtPort.Size = New System.Drawing.Size(236, 22)
		Me.txtPort.TabIndex = 2
		Me.txtPort.Text = "5481"
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(16, 45)
		Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(42, 17)
		Me.Label8.TabIndex = 18
		Me.Label8.Text = "Node"
		'
		'txtNode
		'
		Me.txtNode.Location = New System.Drawing.Point(125, 45)
		Me.txtNode.Margin = New System.Windows.Forms.Padding(4)
		Me.txtNode.Name = "txtNode"
		Me.txtNode.Size = New System.Drawing.Size(236, 22)
		Me.txtNode.TabIndex = 1
		Me.txtNode.Text = "localhost"
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(16, 224)
		Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(105, 17)
		Me.Label9.TabIndex = 19
		Me.Label9.Text = "Exclude Names"
		'
		'txtExclude
		'
		Me.txtExclude.Location = New System.Drawing.Point(125, 224)
		Me.txtExclude.Margin = New System.Windows.Forms.Padding(4)
		Me.txtExclude.Name = "txtExclude"
		Me.txtExclude.Size = New System.Drawing.Size(236, 22)
		Me.txtExclude.TabIndex = 20
		Me.txtExclude.Text = "Default,Symbol"
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(16, 254)
		Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(71, 17)
		Me.Label10.TabIndex = 21
		Me.Label10.Text = "Time (ms)"
		'
		'txtTime
		'
		Me.txtTime.Location = New System.Drawing.Point(125, 254)
		Me.txtTime.Margin = New System.Windows.Forms.Padding(4)
		Me.txtTime.Name = "txtTime"
		Me.txtTime.Size = New System.Drawing.Size(64, 22)
		Me.txtTime.TabIndex = 22
		Me.txtTime.Text = "1500"
		'
		'chkRepeat
		'
		Me.chkRepeat.AutoSize = True
		Me.chkRepeat.Location = New System.Drawing.Point(125, 283)
		Me.chkRepeat.Name = "chkRepeat"
		Me.chkRepeat.Size = New System.Drawing.Size(147, 21)
		Me.chkRepeat.TabIndex = 23
		Me.chkRepeat.Text = "Repeat indefinitely"
		Me.chkRepeat.UseVisualStyleBackColor = True
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(16, 407)
		Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(279, 17)
		Me.Label11.TabIndex = 24
		Me.Label11.Text = "Hold Ctrl key down to stop while executing. "
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(16, 440)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(360, 17)
		Me.Label12.TabIndex = 25
		Me.Label12.Text = "If ViewX is already started ensure it is in the foreground."
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(485, 475)
		Me.Controls.Add(Me.Label12)
		Me.Controls.Add(Me.Label11)
		Me.Controls.Add(Me.chkRepeat)
		Me.Controls.Add(Me.txtTime)
		Me.Controls.Add(Me.Label10)
		Me.Controls.Add(Me.txtExclude)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.Label8)
		Me.Controls.Add(Me.txtNode)
		Me.Controls.Add(Me.Label7)
		Me.Controls.Add(Me.txtPort)
		Me.Controls.Add(Me.GetAll)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.txtFolder)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.txtMimic)
		Me.Controls.Add(Me.Quit)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.txtPassword)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtUsername)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtSystem)
		Me.Controls.Add(Me.Button1)
		Me.Margin = New System.Windows.Forms.Padding(4)
		Me.Name = "Form1"
		Me.Text = "Mimic Grabber"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents txtSystem As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtUsername As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents Quit As System.Windows.Forms.Button
    Friend WithEvents txtMimic As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents GetAll As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtPort As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtNode As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents txtExclude As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents txtTime As TextBox
    Friend WithEvents chkRepeat As CheckBox
	Friend WithEvents Label11 As Label
	Friend WithEvents Label12 As Label
End Class
