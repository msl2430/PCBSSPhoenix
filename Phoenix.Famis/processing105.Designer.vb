<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class processing105
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(processing105))
        Me.txt_Info = New System.Windows.Forms.RichTextBox
        Me.grp_Mode = New System.Windows.Forms.GroupBox
        Me.rdo_Exist = New System.Windows.Forms.RadioButton
        Me.rdo_Manual = New System.Windows.Forms.RadioButton
        Me.rdo_GUMP = New System.Windows.Forms.RadioButton
        Me.btn_GUMPStart = New System.Windows.Forms.Button
        Me.btn_GUMPStop = New System.Windows.Forms.Button
        Me.grp_GUMPControls = New System.Windows.Forms.GroupBox
        Me.grp_ExistingControls = New System.Windows.Forms.GroupBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txt_CaseNumber = New System.Windows.Forms.TextBox
        Me.btn_Search = New System.Windows.Forms.Button
        Me.grp_ManualControls = New System.Windows.Forms.GroupBox
        Me.btn_ManulStart_U = New System.Windows.Forms.Button
        Me.btn_ManualStart_A = New System.Windows.Forms.Button
        Me.txt_Status = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.BGW_ReadGUMP = New System.ComponentModel.BackgroundWorker
        Me.BGW_GUMPProcess = New System.ComponentModel.BackgroundWorker
        Me.grp_Mode.SuspendLayout()
        Me.grp_GUMPControls.SuspendLayout()
        Me.grp_ExistingControls.SuspendLayout()
        Me.grp_ManualControls.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txt_Info
        '
        Me.txt_Info.Location = New System.Drawing.Point(12, 3)
        Me.txt_Info.Name = "txt_Info"
        Me.txt_Info.ReadOnly = True
        Me.txt_Info.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.txt_Info.Size = New System.Drawing.Size(293, 151)
        Me.txt_Info.TabIndex = 0
        Me.txt_Info.Text = ""
        '
        'grp_Mode
        '
        Me.grp_Mode.Controls.Add(Me.rdo_Exist)
        Me.grp_Mode.Controls.Add(Me.rdo_Manual)
        Me.grp_Mode.Controls.Add(Me.rdo_GUMP)
        Me.grp_Mode.Location = New System.Drawing.Point(12, 160)
        Me.grp_Mode.Name = "grp_Mode"
        Me.grp_Mode.Size = New System.Drawing.Size(184, 66)
        Me.grp_Mode.TabIndex = 1
        Me.grp_Mode.TabStop = False
        Me.grp_Mode.Text = "Mode Selection"
        '
        'rdo_Exist
        '
        Me.rdo_Exist.AutoSize = True
        Me.rdo_Exist.Location = New System.Drawing.Point(94, 19)
        Me.rdo_Exist.Name = "rdo_Exist"
        Me.rdo_Exist.Size = New System.Drawing.Size(88, 17)
        Me.rdo_Exist.TabIndex = 2
        Me.rdo_Exist.TabStop = True
        Me.rdo_Exist.Text = "Existing Case"
        Me.rdo_Exist.UseVisualStyleBackColor = True
        '
        'rdo_Manual
        '
        Me.rdo_Manual.AutoSize = True
        Me.rdo_Manual.Location = New System.Drawing.Point(6, 42)
        Me.rdo_Manual.Name = "rdo_Manual"
        Me.rdo_Manual.Size = New System.Drawing.Size(87, 17)
        Me.rdo_Manual.TabIndex = 1
        Me.rdo_Manual.TabStop = True
        Me.rdo_Manual.Text = "Manual Entry"
        Me.rdo_Manual.UseVisualStyleBackColor = True
        '
        'rdo_GUMP
        '
        Me.rdo_GUMP.AutoSize = True
        Me.rdo_GUMP.Checked = True
        Me.rdo_GUMP.Location = New System.Drawing.Point(6, 19)
        Me.rdo_GUMP.Name = "rdo_GUMP"
        Me.rdo_GUMP.Size = New System.Drawing.Size(76, 17)
        Me.rdo_GUMP.TabIndex = 0
        Me.rdo_GUMP.TabStop = True
        Me.rdo_GUMP.Text = "GUMP File"
        Me.rdo_GUMP.UseVisualStyleBackColor = True
        '
        'btn_GUMPStart
        '
        Me.btn_GUMPStart.Location = New System.Drawing.Point(6, 20)
        Me.btn_GUMPStart.Name = "btn_GUMPStart"
        Me.btn_GUMPStart.Size = New System.Drawing.Size(84, 25)
        Me.btn_GUMPStart.TabIndex = 2
        Me.btn_GUMPStart.Text = "Start"
        Me.btn_GUMPStart.UseVisualStyleBackColor = True
        '
        'btn_GUMPStop
        '
        Me.btn_GUMPStop.Location = New System.Drawing.Point(96, 20)
        Me.btn_GUMPStop.Name = "btn_GUMPStop"
        Me.btn_GUMPStop.Size = New System.Drawing.Size(84, 25)
        Me.btn_GUMPStop.TabIndex = 3
        Me.btn_GUMPStop.Text = "Stop"
        Me.btn_GUMPStop.UseVisualStyleBackColor = True
        '
        'grp_GUMPControls
        '
        Me.grp_GUMPControls.Controls.Add(Me.btn_GUMPStart)
        Me.grp_GUMPControls.Controls.Add(Me.btn_GUMPStop)
        Me.grp_GUMPControls.Location = New System.Drawing.Point(12, 232)
        Me.grp_GUMPControls.Name = "grp_GUMPControls"
        Me.grp_GUMPControls.Size = New System.Drawing.Size(184, 79)
        Me.grp_GUMPControls.TabIndex = 4
        Me.grp_GUMPControls.TabStop = False
        Me.grp_GUMPControls.Text = "GUMP Controls"
        '
        'grp_ExistingControls
        '
        Me.grp_ExistingControls.Controls.Add(Me.Label1)
        Me.grp_ExistingControls.Controls.Add(Me.txt_CaseNumber)
        Me.grp_ExistingControls.Controls.Add(Me.btn_Search)
        Me.grp_ExistingControls.Location = New System.Drawing.Point(12, 232)
        Me.grp_ExistingControls.Name = "grp_ExistingControls"
        Me.grp_ExistingControls.Size = New System.Drawing.Size(184, 79)
        Me.grp_ExistingControls.TabIndex = 5
        Me.grp_ExistingControls.TabStop = False
        Me.grp_ExistingControls.Text = "Existing Case Controls"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Case Number:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txt_CaseNumber
        '
        Me.txt_CaseNumber.Location = New System.Drawing.Point(80, 19)
        Me.txt_CaseNumber.MaxLength = 10
        Me.txt_CaseNumber.Name = "txt_CaseNumber"
        Me.txt_CaseNumber.Size = New System.Drawing.Size(98, 20)
        Me.txt_CaseNumber.TabIndex = 4
        '
        'btn_Search
        '
        Me.btn_Search.Location = New System.Drawing.Point(80, 45)
        Me.btn_Search.Name = "btn_Search"
        Me.btn_Search.Size = New System.Drawing.Size(98, 25)
        Me.btn_Search.TabIndex = 2
        Me.btn_Search.Text = "Search"
        Me.btn_Search.UseVisualStyleBackColor = True
        '
        'grp_ManualControls
        '
        Me.grp_ManualControls.Controls.Add(Me.btn_ManulStart_U)
        Me.grp_ManualControls.Controls.Add(Me.btn_ManualStart_A)
        Me.grp_ManualControls.Location = New System.Drawing.Point(12, 232)
        Me.grp_ManualControls.Name = "grp_ManualControls"
        Me.grp_ManualControls.Size = New System.Drawing.Size(184, 79)
        Me.grp_ManualControls.TabIndex = 6
        Me.grp_ManualControls.TabStop = False
        Me.grp_ManualControls.Text = "Manual Entry Controls"
        '
        'btn_ManulStart_U
        '
        Me.btn_ManulStart_U.Location = New System.Drawing.Point(96, 20)
        Me.btn_ManulStart_U.Name = "btn_ManulStart_U"
        Me.btn_ManulStart_U.Size = New System.Drawing.Size(84, 25)
        Me.btn_ManulStart_U.TabIndex = 3
        Me.btn_ManulStart_U.Text = "'U' Batch Start"
        Me.btn_ManulStart_U.UseVisualStyleBackColor = True
        '
        'btn_ManualStart_A
        '
        Me.btn_ManualStart_A.Location = New System.Drawing.Point(6, 20)
        Me.btn_ManualStart_A.Name = "btn_ManualStart_A"
        Me.btn_ManualStart_A.Size = New System.Drawing.Size(84, 25)
        Me.btn_ManualStart_A.TabIndex = 2
        Me.btn_ManualStart_A.Text = "'A' Batch Start"
        Me.btn_ManualStart_A.UseVisualStyleBackColor = True
        '
        'txt_Status
        '
        Me.txt_Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_Status.Location = New System.Drawing.Point(205, 249)
        Me.txt_Status.Name = "txt_Status"
        Me.txt_Status.ReadOnly = True
        Me.txt_Status.Size = New System.Drawing.Size(100, 20)
        Me.txt_Status.TabIndex = 7
        Me.txt_Status.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(202, 232)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Status:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(202, 274)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Last Batch Number:"
        '
        'TextBox2
        '
        Me.TextBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(205, 291)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(100, 20)
        Me.TextBox2.TabIndex = 9
        Me.TextBox2.Text = "U999301"
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.Phoenix.My.Resources.Resources.bird2
        Me.PictureBox1.Location = New System.Drawing.Point(205, 160)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(100, 66)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 11
        Me.PictureBox1.TabStop = False
        '
        'BGW_ReadGUMP
        '
        Me.BGW_ReadGUMP.WorkerSupportsCancellation = True
        '
        'BGW_GUMPProcess
        '
        Me.BGW_GUMPProcess.WorkerReportsProgress = True
        Me.BGW_GUMPProcess.WorkerSupportsCancellation = True
        '
        'processing105
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(315, 315)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txt_Status)
        Me.Controls.Add(Me.grp_Mode)
        Me.Controls.Add(Me.txt_Info)
        Me.Controls.Add(Me.grp_ManualControls)
        Me.Controls.Add(Me.grp_GUMPControls)
        Me.Controls.Add(Me.grp_ExistingControls)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "processing105"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Phoenix - 105 Processing"
        Me.grp_Mode.ResumeLayout(False)
        Me.grp_Mode.PerformLayout()
        Me.grp_GUMPControls.ResumeLayout(False)
        Me.grp_ExistingControls.ResumeLayout(False)
        Me.grp_ExistingControls.PerformLayout()
        Me.grp_ManualControls.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txt_Info As System.Windows.Forms.RichTextBox
    Friend WithEvents grp_Mode As System.Windows.Forms.GroupBox
    Friend WithEvents rdo_Exist As System.Windows.Forms.RadioButton
    Friend WithEvents rdo_Manual As System.Windows.Forms.RadioButton
    Friend WithEvents rdo_GUMP As System.Windows.Forms.RadioButton
    Friend WithEvents btn_GUMPStart As System.Windows.Forms.Button
    Friend WithEvents btn_GUMPStop As System.Windows.Forms.Button
    Friend WithEvents grp_GUMPControls As System.Windows.Forms.GroupBox
    Friend WithEvents grp_ExistingControls As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txt_CaseNumber As System.Windows.Forms.TextBox
    Friend WithEvents btn_Search As System.Windows.Forms.Button
    Friend WithEvents grp_ManualControls As System.Windows.Forms.GroupBox
    Friend WithEvents btn_ManualStart_A As System.Windows.Forms.Button
    Friend WithEvents btn_ManulStart_U As System.Windows.Forms.Button
    Friend WithEvents txt_Status As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents BGW_ReadGUMP As System.ComponentModel.BackgroundWorker
    Friend WithEvents BGW_GUMPProcess As System.ComponentModel.BackgroundWorker

End Class
