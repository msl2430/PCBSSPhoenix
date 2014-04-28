<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class viewBatch
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(viewBatch))
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.CaseNumberDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.OPERATORDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.BATCHNUMBERDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.FAMISCaseInformationBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PhoenixDataSet = New Phoenix.PhoenixDataSet
        Me.DateChoice = New System.Windows.Forms.DateTimePicker
        Me.Label1 = New System.Windows.Forms.Label
        Me.btn_Print = New System.Windows.Forms.Button
        Me.FAMISCaseInformationTableAdapter = New Phoenix.PhoenixDataSetTableAdapters.FAMISCaseInformationTableAdapter
        Me.PrintDoc = New System.Drawing.Printing.PrintDocument
        Me.txt_Font = New System.Windows.Forms.TextBox
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FAMISCaseInformationBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PhoenixDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AllowUserToOrderColumns = True
        Me.DataGridView1.AllowUserToResizeColumns = False
        Me.DataGridView1.AllowUserToResizeRows = False
        Me.DataGridView1.AutoGenerateColumns = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.CaseNumberDataGridViewTextBoxColumn, Me.OPERATORDataGridViewTextBoxColumn, Me.BATCHNUMBERDataGridViewTextBoxColumn})
        Me.DataGridView1.DataSource = Me.FAMISCaseInformationBindingSource
        Me.DataGridView1.Location = New System.Drawing.Point(6, 31)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridView1.Size = New System.Drawing.Size(322, 289)
        Me.DataGridView1.TabIndex = 1
        '
        'CaseNumberDataGridViewTextBoxColumn
        '
        Me.CaseNumberDataGridViewTextBoxColumn.DataPropertyName = "CaseNumber"
        Me.CaseNumberDataGridViewTextBoxColumn.HeaderText = "CaseNumber"
        Me.CaseNumberDataGridViewTextBoxColumn.Name = "CaseNumberDataGridViewTextBoxColumn"
        Me.CaseNumberDataGridViewTextBoxColumn.ReadOnly = True
        '
        'OPERATORDataGridViewTextBoxColumn
        '
        Me.OPERATORDataGridViewTextBoxColumn.DataPropertyName = "OPERATOR"
        Me.OPERATORDataGridViewTextBoxColumn.HeaderText = "OPERATOR"
        Me.OPERATORDataGridViewTextBoxColumn.Name = "OPERATORDataGridViewTextBoxColumn"
        Me.OPERATORDataGridViewTextBoxColumn.ReadOnly = True
        '
        'BATCHNUMBERDataGridViewTextBoxColumn
        '
        Me.BATCHNUMBERDataGridViewTextBoxColumn.DataPropertyName = "BATCHNUMBER"
        Me.BATCHNUMBERDataGridViewTextBoxColumn.HeaderText = "BATCHNUMBER"
        Me.BATCHNUMBERDataGridViewTextBoxColumn.Name = "BATCHNUMBERDataGridViewTextBoxColumn"
        Me.BATCHNUMBERDataGridViewTextBoxColumn.ReadOnly = True
        '
        'FAMISCaseInformationBindingSource
        '
        Me.FAMISCaseInformationBindingSource.DataMember = "FAMISCaseInformation"
        Me.FAMISCaseInformationBindingSource.DataSource = Me.PhoenixDataSet
        '
        'PhoenixDataSet
        '
        Me.PhoenixDataSet.DataSetName = "PhoenixDataSet"
        Me.PhoenixDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'DateChoice
        '
        Me.DateChoice.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateChoice.Location = New System.Drawing.Point(87, 5)
        Me.DateChoice.Name = "DateChoice"
        Me.DateChoice.Size = New System.Drawing.Size(142, 20)
        Me.DateChoice.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(3, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 14)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Select a date:"
        '
        'btn_Print
        '
        Me.btn_Print.BackColor = System.Drawing.Color.SteelBlue
        Me.btn_Print.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_Print.Location = New System.Drawing.Point(235, 4)
        Me.btn_Print.Name = "btn_Print"
        Me.btn_Print.Size = New System.Drawing.Size(93, 23)
        Me.btn_Print.TabIndex = 5
        Me.btn_Print.Text = "Print"
        Me.btn_Print.UseVisualStyleBackColor = False
        '
        'FAMISCaseInformationTableAdapter
        '
        Me.FAMISCaseInformationTableAdapter.ClearBeforeFill = True
        '
        'PrintDoc
        '
        '
        'txt_Font
        '
        Me.txt_Font.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_Font.Location = New System.Drawing.Point(117, 152)
        Me.txt_Font.Name = "txt_Font"
        Me.txt_Font.Size = New System.Drawing.Size(100, 20)
        Me.txt_Font.TabIndex = 10
        '
        'viewBatch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Phoenix.My.Resources.Resources.RedBG
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(334, 324)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.txt_Font)
        Me.Controls.Add(Me.btn_Print)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.DateChoice)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "viewBatch"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Phoenix - View Batches"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FAMISCaseInformationBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PhoenixDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents CaseNumberDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OPERATORDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BATCHNUMBERDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents FAMISCaseInformationBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents PhoenixDataSet As Phoenix.PhoenixDataSet
    Friend WithEvents FAMISCaseInformationTableAdapter As Phoenix.PhoenixDataSetTableAdapters.FAMISCaseInformationTableAdapter
    Friend WithEvents DateChoice As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btn_Print As System.Windows.Forms.Button
    Friend WithEvents PrintDoc As System.Drawing.Printing.PrintDocument
    Friend WithEvents txt_Font As System.Windows.Forms.TextBox
End Class
