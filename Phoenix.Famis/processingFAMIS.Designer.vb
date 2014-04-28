<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class processingFAMIS
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(processingFAMIS))
        Me.progressbar_FAMIS = New System.Windows.Forms.ProgressBar
        Me.lbl_Status = New System.Windows.Forms.Label
        Me.btn_Cancel = New System.Windows.Forms.Button
        Me.BGW_Cancel = New System.ComponentModel.BackgroundWorker
        Me.txt_BatchNumber = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.BGW_ProcessFAMIS = New System.ComponentModel.BackgroundWorker
        Me.BGW_RestartCase = New System.ComponentModel.BackgroundWorker
        Me.SuspendLayout()
        '
        'progressbar_FAMIS
        '
        Me.progressbar_FAMIS.Location = New System.Drawing.Point(6, 12)
        Me.progressbar_FAMIS.Name = "progressbar_FAMIS"
        Me.progressbar_FAMIS.Size = New System.Drawing.Size(240, 23)
        Me.progressbar_FAMIS.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.progressbar_FAMIS.TabIndex = 0
        '
        'lbl_Status
        '
        Me.lbl_Status.BackColor = System.Drawing.Color.Transparent
        Me.lbl_Status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbl_Status.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_Status.ForeColor = System.Drawing.Color.White
        Me.lbl_Status.Location = New System.Drawing.Point(6, 38)
        Me.lbl_Status.Name = "lbl_Status"
        Me.lbl_Status.Size = New System.Drawing.Size(240, 33)
        Me.lbl_Status.TabIndex = 1
        Me.lbl_Status.Text = "Connecting to FAMIS..."
        Me.lbl_Status.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'btn_Cancel
        '
        Me.btn_Cancel.BackColor = System.Drawing.Color.Tomato
        Me.btn_Cancel.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_Cancel.Location = New System.Drawing.Point(171, 74)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 2
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = False
        '
        'BGW_Cancel
        '
        '
        'txt_BatchNumber
        '
        Me.txt_BatchNumber.BackColor = System.Drawing.Color.Tan
        Me.txt_BatchNumber.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_BatchNumber.Location = New System.Drawing.Point(100, 76)
        Me.txt_BatchNumber.Name = "txt_BatchNumber"
        Me.txt_BatchNumber.ReadOnly = True
        Me.txt_BatchNumber.Size = New System.Drawing.Size(59, 20)
        Me.txt_BatchNumber.TabIndex = 3
        Me.txt_BatchNumber.TabStop = False
        Me.txt_BatchNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 79)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(87, 14)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Batch Number:"
        '
        'BGW_ProcessFAMIS
        '
        Me.BGW_ProcessFAMIS.WorkerReportsProgress = True
        Me.BGW_ProcessFAMIS.WorkerSupportsCancellation = True
        '
        'BGW_RestartCase
        '
        Me.BGW_RestartCase.WorkerReportsProgress = True
        '
        'processingFAMIS
        '
        Me.AcceptButton = Me.btn_Cancel
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Phoenix.My.Resources.Resources.RedBG
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(251, 101)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txt_BatchNumber)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.lbl_Status)
        Me.Controls.Add(Me.progressbar_FAMIS)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "processingFAMIS"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "processingFAMIS"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents progressbar_FAMIS As System.Windows.Forms.ProgressBar
    Friend WithEvents lbl_Status As System.Windows.Forms.Label
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents BGW_Cancel As System.ComponentModel.BackgroundWorker
    Friend WithEvents txt_BatchNumber As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents BGW_ProcessFAMIS As System.ComponentModel.BackgroundWorker
    Friend WithEvents BGW_RestartCase As System.ComponentModel.BackgroundWorker
End Class
