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
		Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
		Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
		Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
		Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.DoCopyHistory = New System.Windows.Forms.Button()
		Me.StopIt = New System.Windows.Forms.Button()
		Me.CopyHistory = New System.Windows.Forms.CheckBox()
		Me.CopyJournal = New System.Windows.Forms.CheckBox()
		Me.DatabaseFolder = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.DestinationFolder = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.FromDate = New System.Windows.Forms.DateTimePicker()
		Me.ToDate = New System.Windows.Forms.DateTimePicker()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.DoDeleteHistory = New System.Windows.Forms.Button()
		Me.DoSizeHistory = New System.Windows.Forms.Button()
		Me.HistoryFolder = New System.Windows.Forms.TextBox()
		Me.JournalFolder = New System.Windows.Forms.TextBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.StatusStrip1.SuspendLayout()
		Me.MenuStrip1.SuspendLayout()
		Me.SuspendLayout()
		'
		'StatusStrip1
		'
		Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
		Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel})
		Me.StatusStrip1.Location = New System.Drawing.Point(0, 316)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New System.Drawing.Size(670, 26)
		Me.StatusStrip1.TabIndex = 1
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'StatusLabel
		'
		Me.StatusLabel.Name = "StatusLabel"
		Me.StatusLabel.Size = New System.Drawing.Size(34, 20)
		Me.StatusLabel.Text = "Idle"
		'
		'MenuStrip1
		'
		Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
		Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.HelpToolStripMenuItem})
		Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
		Me.MenuStrip1.Name = "MenuStrip1"
		Me.MenuStrip1.Size = New System.Drawing.Size(670, 28)
		Me.MenuStrip1.TabIndex = 30
		Me.MenuStrip1.Text = "MenuStrip1"
		'
		'FileToolStripMenuItem
		'
		Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
		Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
		Me.FileToolStripMenuItem.Size = New System.Drawing.Size(46, 24)
		Me.FileToolStripMenuItem.Text = "&File"
		'
		'ExitToolStripMenuItem
		'
		Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
		Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(116, 26)
		Me.ExitToolStripMenuItem.Text = "E&xit"
		'
		'HelpToolStripMenuItem
		'
		Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
		Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
		Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(55, 24)
		Me.HelpToolStripMenuItem.Text = "&Help"
		'
		'AboutToolStripMenuItem
		'
		Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
		Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(142, 26)
		Me.AboutToolStripMenuItem.Text = "&About..."
		'
		'DoCopyHistory
		'
		Me.DoCopyHistory.Location = New System.Drawing.Point(233, 263)
		Me.DoCopyHistory.Name = "DoCopyHistory"
		Me.DoCopyHistory.Size = New System.Drawing.Size(97, 29)
		Me.DoCopyHistory.TabIndex = 31
		Me.DoCopyHistory.Text = "Copy"
		Me.DoCopyHistory.UseVisualStyleBackColor = True
		'
		'StopIt
		'
		Me.StopIt.Location = New System.Drawing.Point(439, 264)
		Me.StopIt.Name = "StopIt"
		Me.StopIt.Size = New System.Drawing.Size(97, 27)
		Me.StopIt.TabIndex = 32
		Me.StopIt.Text = "Stop"
		Me.StopIt.UseVisualStyleBackColor = True
		'
		'CopyHistory
		'
		Me.CopyHistory.AutoSize = True
		Me.CopyHistory.Location = New System.Drawing.Point(147, 192)
		Me.CopyHistory.Name = "CopyHistory"
		Me.CopyHistory.Size = New System.Drawing.Size(331, 21)
		Me.CopyHistory.TabIndex = 34
		Me.CopyHistory.Text = "History (specify Monday-Sunday midnight GMT)"
		Me.CopyHistory.UseVisualStyleBackColor = True
		'
		'CopyJournal
		'
		Me.CopyJournal.AutoSize = True
		Me.CopyJournal.Location = New System.Drawing.Point(147, 219)
		Me.CopyJournal.Name = "CopyJournal"
		Me.CopyJournal.Size = New System.Drawing.Size(367, 21)
		Me.CopyJournal.TabIndex = 35
		Me.CopyJournal.Text = "Journal (specify inclusive days from/to midnight GMT)"
		Me.CopyJournal.UseVisualStyleBackColor = True
		'
		'DatabaseFolder
		'
		Me.DatabaseFolder.Location = New System.Drawing.Point(147, 81)
		Me.DatabaseFolder.Name = "DatabaseFolder"
		Me.DatabaseFolder.Size = New System.Drawing.Size(511, 22)
		Me.DatabaseFolder.TabIndex = 36
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(15, 84)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(113, 17)
		Me.Label6.TabIndex = 37
		Me.Label6.Text = "Database Folder"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(15, 112)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(123, 17)
		Me.Label1.TabIndex = 39
		Me.Label1.Text = "Destination Folder"
		'
		'DestinationFolder
		'
		Me.DestinationFolder.Location = New System.Drawing.Point(147, 109)
		Me.DestinationFolder.Name = "DestinationFolder"
		Me.DestinationFolder.Size = New System.Drawing.Size(511, 22)
		Me.DestinationFolder.TabIndex = 38
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(328, 58)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(25, 17)
		Me.Label2.TabIndex = 41
		Me.Label2.Text = "To"
		'
		'FromDate
		'
		Me.FromDate.Location = New System.Drawing.Point(147, 53)
		Me.FromDate.Name = "FromDate"
		Me.FromDate.Size = New System.Drawing.Size(166, 22)
		Me.FromDate.TabIndex = 42
		'
		'ToDate
		'
		Me.ToDate.Location = New System.Drawing.Point(370, 53)
		Me.ToDate.Name = "ToDate"
		Me.ToDate.Size = New System.Drawing.Size(166, 22)
		Me.ToDate.TabIndex = 43
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(15, 58)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(40, 17)
		Me.Label3.TabIndex = 44
		Me.Label3.Text = "From"
		'
		'DoDeleteHistory
		'
		Me.DoDeleteHistory.Location = New System.Drawing.Point(336, 263)
		Me.DoDeleteHistory.Name = "DoDeleteHistory"
		Me.DoDeleteHistory.Size = New System.Drawing.Size(97, 29)
		Me.DoDeleteHistory.TabIndex = 45
		Me.DoDeleteHistory.Text = "Delete ..."
		Me.DoDeleteHistory.UseVisualStyleBackColor = True
		'
		'DoSizeHistory
		'
		Me.DoSizeHistory.Location = New System.Drawing.Point(130, 263)
		Me.DoSizeHistory.Name = "DoSizeHistory"
		Me.DoSizeHistory.Size = New System.Drawing.Size(97, 29)
		Me.DoSizeHistory.TabIndex = 46
		Me.DoSizeHistory.Text = "Size"
		Me.DoSizeHistory.UseVisualStyleBackColor = True
		'
		'HistoryFolder
		'
		Me.HistoryFolder.Location = New System.Drawing.Point(147, 137)
		Me.HistoryFolder.Name = "HistoryFolder"
		Me.HistoryFolder.Size = New System.Drawing.Size(120, 22)
		Me.HistoryFolder.TabIndex = 47
		Me.HistoryFolder.Text = "History"
		'
		'JournalFolder
		'
		Me.JournalFolder.Location = New System.Drawing.Point(147, 164)
		Me.JournalFolder.Name = "JournalFolder"
		Me.JournalFolder.Size = New System.Drawing.Size(120, 22)
		Me.JournalFolder.TabIndex = 48
		Me.JournalFolder.Text = "Journal"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(15, 140)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(96, 17)
		Me.Label4.TabIndex = 49
		Me.Label4.Text = "History Folder"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(15, 167)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(99, 17)
		Me.Label5.TabIndex = 50
		Me.Label5.Text = "Journal Folder"
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(670, 342)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.JournalFolder)
		Me.Controls.Add(Me.HistoryFolder)
		Me.Controls.Add(Me.DoSizeHistory)
		Me.Controls.Add(Me.DoDeleteHistory)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.ToDate)
		Me.Controls.Add(Me.FromDate)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.DestinationFolder)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.DatabaseFolder)
		Me.Controls.Add(Me.CopyJournal)
		Me.Controls.Add(Me.CopyHistory)
		Me.Controls.Add(Me.StopIt)
		Me.Controls.Add(Me.DoCopyHistory)
		Me.Controls.Add(Me.MenuStrip1)
		Me.Controls.Add(Me.StatusStrip1)
		Me.Name = "Form1"
		Me.Text = "Copy, Size or Delete SCX History and Journal by Time Range"
		Me.StatusStrip1.ResumeLayout(False)
		Me.StatusStrip1.PerformLayout()
		Me.MenuStrip1.ResumeLayout(False)
		Me.MenuStrip1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DoCopyHistory As System.Windows.Forms.Button
    Friend WithEvents StopIt As System.Windows.Forms.Button
    Friend WithEvents CopyHistory As System.Windows.Forms.CheckBox
    Friend WithEvents CopyJournal As System.Windows.Forms.CheckBox
    Friend WithEvents DatabaseFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DestinationFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents FromDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents ToDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents StatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents DoDeleteHistory As System.Windows.Forms.Button
    Friend WithEvents DoSizeHistory As System.Windows.Forms.Button
    Friend WithEvents HistoryFolder As System.Windows.Forms.TextBox
    Friend WithEvents JournalFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label

End Class
