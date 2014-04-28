<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeleteFile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DeleteFile))
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmb_FileList = New System.Windows.Forms.ComboBox
        Me.btn_Submit = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(7, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(112, 14)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Select A GUMP File:"
        '
        'cmb_FileList
        '
        Me.cmb_FileList.FormattingEnabled = True
        Me.cmb_FileList.Location = New System.Drawing.Point(125, 6)
        Me.cmb_FileList.MaxDropDownItems = 15
        Me.cmb_FileList.Name = "cmb_FileList"
        Me.cmb_FileList.Size = New System.Drawing.Size(179, 21)
        Me.cmb_FileList.TabIndex = 3
        Me.cmb_FileList.Text = "Select A File"
        '
        'btn_Submit
        '
        Me.btn_Submit.Location = New System.Drawing.Point(229, 33)
        Me.btn_Submit.Name = "btn_Submit"
        Me.btn_Submit.Size = New System.Drawing.Size(75, 23)
        Me.btn_Submit.TabIndex = 4
        Me.btn_Submit.Text = "Submit"
        Me.btn_Submit.UseVisualStyleBackColor = True
        '
        'DeleteFile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Phoenix.My.Resources.Resources.RedBG
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(309, 63)
        Me.Controls.Add(Me.btn_Submit)
        Me.Controls.Add(Me.cmb_FileList)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeleteFile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Phoenix - Delete GUMP File"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmb_FileList As System.Windows.Forms.ComboBox
    Friend WithEvents btn_Submit As System.Windows.Forms.Button
End Class
