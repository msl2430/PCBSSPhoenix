<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NPDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(NPDialog))
        Me.btn_Yes = New System.Windows.Forms.Button
        Me.btn_No = New System.Windows.Forms.Button
        Me.btn_Cancel = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.chk_View105 = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'btn_Yes
        '
        Me.btn_Yes.BackColor = System.Drawing.Color.SteelBlue
        Me.btn_Yes.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_Yes.Location = New System.Drawing.Point(3, 19)
        Me.btn_Yes.Name = "btn_Yes"
        Me.btn_Yes.Size = New System.Drawing.Size(75, 38)
        Me.btn_Yes.TabIndex = 0
        Me.btn_Yes.Text = "Yes"
        Me.btn_Yes.UseVisualStyleBackColor = False
        '
        'btn_No
        '
        Me.btn_No.BackColor = System.Drawing.Color.Yellow
        Me.btn_No.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_No.Location = New System.Drawing.Point(84, 19)
        Me.btn_No.Name = "btn_No"
        Me.btn_No.Size = New System.Drawing.Size(75, 38)
        Me.btn_No.TabIndex = 1
        Me.btn_No.Text = "No"
        Me.btn_No.UseVisualStyleBackColor = False
        '
        'btn_Cancel
        '
        Me.btn_Cancel.BackColor = System.Drawing.Color.Tomato
        Me.btn_Cancel.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_Cancel.Location = New System.Drawing.Point(165, 19)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 38)
        Me.btn_Cancel.TabIndex = 2
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(3, -1)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(239, 23)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Include Line N and P?"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'chk_View105
        '
        Me.chk_View105.BackColor = System.Drawing.Color.Transparent
        Me.chk_View105.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chk_View105.ForeColor = System.Drawing.Color.FromArgb(CType(CType(178, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(162, Byte), Integer))
        Me.chk_View105.Location = New System.Drawing.Point(10, 57)
        Me.chk_View105.Name = "chk_View105"
        Me.chk_View105.Size = New System.Drawing.Size(233, 24)
        Me.chk_View105.TabIndex = 4
        Me.chk_View105.Text = "Preview 105 Form Before Submitting"
        Me.chk_View105.UseVisualStyleBackColor = False
        '
        'NPDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Phoenix.My.Resources.Resources.RedBG
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(242, 80)
        Me.ControlBox = False
        Me.Controls.Add(Me.chk_View105)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_No)
        Me.Controls.Add(Me.btn_Yes)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "NPDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "NPDialog"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btn_Yes As System.Windows.Forms.Button
    Friend WithEvents btn_No As System.Windows.Forms.Button
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents chk_View105 As System.Windows.Forms.CheckBox
End Class
