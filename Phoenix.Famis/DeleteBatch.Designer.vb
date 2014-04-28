<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeleteBatch
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DeleteBatch))
        Me.txt_Batch = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.btn_Submit = New System.Windows.Forms.Button
        Me.bgw_DeleteBatch = New System.ComponentModel.BackgroundWorker
        Me.lbl_Status = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'txt_Batch
        '
        Me.txt_Batch.Location = New System.Drawing.Point(147, 6)
        Me.txt_Batch.MaxLength = 7
        Me.txt_Batch.Name = "txt_Batch"
        Me.txt_Batch.Size = New System.Drawing.Size(75, 20)
        Me.txt_Batch.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(5, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(130, 14)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Enter A Batch Number:"
        '
        'btn_Submit
        '
        Me.btn_Submit.Location = New System.Drawing.Point(147, 32)
        Me.btn_Submit.Name = "btn_Submit"
        Me.btn_Submit.Size = New System.Drawing.Size(75, 23)
        Me.btn_Submit.TabIndex = 2
        Me.btn_Submit.Text = "Submit"
        Me.btn_Submit.UseVisualStyleBackColor = True
        '
        'bgw_DeleteBatch
        '
        Me.bgw_DeleteBatch.WorkerReportsProgress = True
        Me.bgw_DeleteBatch.WorkerSupportsCancellation = True
        '
        'lbl_Status
        '
        Me.lbl_Status.BackColor = System.Drawing.Color.Transparent
        Me.lbl_Status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbl_Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_Status.ForeColor = System.Drawing.Color.SandyBrown
        Me.lbl_Status.Location = New System.Drawing.Point(5, 32)
        Me.lbl_Status.Name = "lbl_Status"
        Me.lbl_Status.Size = New System.Drawing.Size(140, 23)
        Me.lbl_Status.TabIndex = 3
        Me.lbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DeleteBatch
        '
        Me.AcceptButton = Me.btn_Submit
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Phoenix.My.Resources.Resources.RedBG
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(227, 62)
        Me.Controls.Add(Me.lbl_Status)
        Me.Controls.Add(Me.btn_Submit)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txt_Batch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeleteBatch"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Phoenix - Delete Batch"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txt_Batch As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btn_Submit As System.Windows.Forms.Button
    Friend WithEvents bgw_DeleteBatch As System.ComponentModel.BackgroundWorker
    Friend WithEvents lbl_Status As System.Windows.Forms.Label
End Class
