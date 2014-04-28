<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Put105Thru
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Put105Thru))
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
        Me.btn_ManualStart_U = New System.Windows.Forms.Button
        Me.btn_ManualStart_A = New System.Windows.Forms.Button
        Me.txt_Status = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.txt_BatchNumber = New System.Windows.Forms.TextBox
        Me.BGW_ReadGUMP = New System.ComponentModel.BackgroundWorker
        Me.BGW_GUMPProcess = New System.ComponentModel.BackgroundWorker
        Me.menu_105 = New System.Windows.Forms.MenuStrip
        Me.menu_File = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_Exit = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_Edit = New System.Windows.Forms.ToolStripMenuItem
        Me.GUMPKeywordToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_UAPSUP = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_UAPCCS = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.menu_DelBatch = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_DelGUMP = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.menu_Options = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_Mode = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_GUMP = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_Existing = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_Manual = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_ViewBatch = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_IMPSBatch = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_ViewGUMP = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.menu_Update = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.menu_About = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.BGW_SQLCheck = New System.ComponentModel.BackgroundWorker
        Me.PictureBox2 = New System.Windows.Forms.PictureBox
        Me.trayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.tray_Menu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tray_Exit = New System.Windows.Forms.ToolStripMenuItem
        Me.BGW_Existing = New System.ComponentModel.BackgroundWorker
        Me.BGW_HideTray = New System.ComponentModel.BackgroundWorker
        Me.BGW_OnlineStatus = New System.ComponentModel.BackgroundWorker
        Me.grp_Mode.SuspendLayout()
        Me.grp_GUMPControls.SuspendLayout()
        Me.grp_ExistingControls.SuspendLayout()
        Me.grp_ManualControls.SuspendLayout()
        Me.menu_105.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tray_Menu.SuspendLayout()
        Me.SuspendLayout()
        '
        'txt_Info
        '
        Me.txt_Info.Location = New System.Drawing.Point(12, 29)
        Me.txt_Info.Name = "txt_Info"
        Me.txt_Info.ReadOnly = True
        Me.txt_Info.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.txt_Info.Size = New System.Drawing.Size(302, 151)
        Me.txt_Info.TabIndex = 0
        Me.txt_Info.Text = ""
        '
        'grp_Mode
        '
        Me.grp_Mode.BackColor = System.Drawing.Color.Transparent
        Me.grp_Mode.Controls.Add(Me.rdo_Exist)
        Me.grp_Mode.Controls.Add(Me.rdo_Manual)
        Me.grp_Mode.Controls.Add(Me.rdo_GUMP)
        Me.grp_Mode.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grp_Mode.Location = New System.Drawing.Point(12, 186)
        Me.grp_Mode.Name = "grp_Mode"
        Me.grp_Mode.Size = New System.Drawing.Size(197, 66)
        Me.grp_Mode.TabIndex = 1
        Me.grp_Mode.TabStop = False
        Me.grp_Mode.Text = "Mode Selection"
        '
        'rdo_Exist
        '
        Me.rdo_Exist.AutoSize = True
        Me.rdo_Exist.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdo_Exist.ForeColor = System.Drawing.Color.White
        Me.rdo_Exist.Location = New System.Drawing.Point(94, 19)
        Me.rdo_Exist.Name = "rdo_Exist"
        Me.rdo_Exist.Size = New System.Drawing.Size(99, 18)
        Me.rdo_Exist.TabIndex = 2
        Me.rdo_Exist.TabStop = True
        Me.rdo_Exist.Text = "&Existing Case"
        Me.rdo_Exist.UseVisualStyleBackColor = True
        '
        'rdo_Manual
        '
        Me.rdo_Manual.AutoSize = True
        Me.rdo_Manual.Enabled = False
        Me.rdo_Manual.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdo_Manual.ForeColor = System.Drawing.Color.White
        Me.rdo_Manual.Location = New System.Drawing.Point(6, 42)
        Me.rdo_Manual.Name = "rdo_Manual"
        Me.rdo_Manual.Size = New System.Drawing.Size(95, 18)
        Me.rdo_Manual.TabIndex = 1
        Me.rdo_Manual.TabStop = True
        Me.rdo_Manual.Text = "&Manual Entry"
        Me.rdo_Manual.UseVisualStyleBackColor = True
        '
        'rdo_GUMP
        '
        Me.rdo_GUMP.AutoSize = True
        Me.rdo_GUMP.Checked = True
        Me.rdo_GUMP.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdo_GUMP.ForeColor = System.Drawing.Color.White
        Me.rdo_GUMP.Location = New System.Drawing.Point(6, 19)
        Me.rdo_GUMP.Name = "rdo_GUMP"
        Me.rdo_GUMP.Size = New System.Drawing.Size(79, 18)
        Me.rdo_GUMP.TabIndex = 0
        Me.rdo_GUMP.TabStop = True
        Me.rdo_GUMP.Text = "&GUMP File"
        Me.rdo_GUMP.UseVisualStyleBackColor = True
        '
        'btn_GUMPStart
        '
        Me.btn_GUMPStart.BackColor = System.Drawing.Color.Green
        Me.btn_GUMPStart.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_GUMPStart.Location = New System.Drawing.Point(3, 20)
        Me.btn_GUMPStart.Name = "btn_GUMPStart"
        Me.btn_GUMPStart.Size = New System.Drawing.Size(95, 25)
        Me.btn_GUMPStart.TabIndex = 2
        Me.btn_GUMPStart.Text = "&Start"
        Me.btn_GUMPStart.UseVisualStyleBackColor = False
        '
        'btn_GUMPStop
        '
        Me.btn_GUMPStop.BackColor = System.Drawing.Color.Red
        Me.btn_GUMPStop.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_GUMPStop.Location = New System.Drawing.Point(100, 20)
        Me.btn_GUMPStop.Name = "btn_GUMPStop"
        Me.btn_GUMPStop.Size = New System.Drawing.Size(95, 25)
        Me.btn_GUMPStop.TabIndex = 3
        Me.btn_GUMPStop.Text = "S&top"
        Me.btn_GUMPStop.UseVisualStyleBackColor = False
        '
        'grp_GUMPControls
        '
        Me.grp_GUMPControls.BackColor = System.Drawing.Color.Transparent
        Me.grp_GUMPControls.Controls.Add(Me.btn_GUMPStart)
        Me.grp_GUMPControls.Controls.Add(Me.btn_GUMPStop)
        Me.grp_GUMPControls.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grp_GUMPControls.Location = New System.Drawing.Point(12, 258)
        Me.grp_GUMPControls.Name = "grp_GUMPControls"
        Me.grp_GUMPControls.Size = New System.Drawing.Size(197, 79)
        Me.grp_GUMPControls.TabIndex = 4
        Me.grp_GUMPControls.TabStop = False
        Me.grp_GUMPControls.Text = "GUMP Controls"
        '
        'grp_ExistingControls
        '
        Me.grp_ExistingControls.BackColor = System.Drawing.Color.Transparent
        Me.grp_ExistingControls.Controls.Add(Me.Label1)
        Me.grp_ExistingControls.Controls.Add(Me.txt_CaseNumber)
        Me.grp_ExistingControls.Controls.Add(Me.btn_Search)
        Me.grp_ExistingControls.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grp_ExistingControls.Location = New System.Drawing.Point(12, 258)
        Me.grp_ExistingControls.Name = "grp_ExistingControls"
        Me.grp_ExistingControls.Size = New System.Drawing.Size(197, 79)
        Me.grp_ExistingControls.TabIndex = 5
        Me.grp_ExistingControls.TabStop = False
        Me.grp_ExistingControls.Text = "Existing Case Controls"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(6, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 14)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Case Number:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txt_CaseNumber
        '
        Me.txt_CaseNumber.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_CaseNumber.Location = New System.Drawing.Point(91, 19)
        Me.txt_CaseNumber.MaxLength = 10
        Me.txt_CaseNumber.Name = "txt_CaseNumber"
        Me.txt_CaseNumber.Size = New System.Drawing.Size(98, 20)
        Me.txt_CaseNumber.TabIndex = 4
        '
        'btn_Search
        '
        Me.btn_Search.BackColor = System.Drawing.Color.SteelBlue
        Me.btn_Search.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_Search.Location = New System.Drawing.Point(91, 45)
        Me.btn_Search.Name = "btn_Search"
        Me.btn_Search.Size = New System.Drawing.Size(98, 25)
        Me.btn_Search.TabIndex = 2
        Me.btn_Search.Text = "&Search"
        Me.btn_Search.UseVisualStyleBackColor = False
        '
        'grp_ManualControls
        '
        Me.grp_ManualControls.BackColor = System.Drawing.Color.Transparent
        Me.grp_ManualControls.Controls.Add(Me.btn_ManualStart_U)
        Me.grp_ManualControls.Controls.Add(Me.btn_ManualStart_A)
        Me.grp_ManualControls.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grp_ManualControls.Location = New System.Drawing.Point(12, 258)
        Me.grp_ManualControls.Name = "grp_ManualControls"
        Me.grp_ManualControls.Size = New System.Drawing.Size(197, 79)
        Me.grp_ManualControls.TabIndex = 6
        Me.grp_ManualControls.TabStop = False
        Me.grp_ManualControls.Text = "Manual Entry Controls"
        '
        'btn_ManualStart_U
        '
        Me.btn_ManualStart_U.BackColor = System.Drawing.Color.Gold
        Me.btn_ManualStart_U.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_ManualStart_U.Location = New System.Drawing.Point(99, 19)
        Me.btn_ManualStart_U.Name = "btn_ManualStart_U"
        Me.btn_ManualStart_U.Size = New System.Drawing.Size(95, 25)
        Me.btn_ManualStart_U.TabIndex = 3
        Me.btn_ManualStart_U.Text = "'&U' Batch Start"
        Me.btn_ManualStart_U.UseVisualStyleBackColor = False
        '
        'btn_ManualStart_A
        '
        Me.btn_ManualStart_A.BackColor = System.Drawing.Color.SteelBlue
        Me.btn_ManualStart_A.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_ManualStart_A.Location = New System.Drawing.Point(3, 19)
        Me.btn_ManualStart_A.Name = "btn_ManualStart_A"
        Me.btn_ManualStart_A.Size = New System.Drawing.Size(95, 25)
        Me.btn_ManualStart_A.TabIndex = 2
        Me.btn_ManualStart_A.Text = "'&A' Batch Start"
        Me.btn_ManualStart_A.UseVisualStyleBackColor = False
        '
        'txt_Status
        '
        Me.txt_Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_Status.Location = New System.Drawing.Point(213, 275)
        Me.txt_Status.Name = "txt_Status"
        Me.txt_Status.ReadOnly = True
        Me.txt_Status.Size = New System.Drawing.Size(100, 20)
        Me.txt_Status.TabIndex = 7
        Me.txt_Status.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(210, 258)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 14)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Status:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(210, 300)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(114, 14)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Last Batch Number:"
        '
        'txt_BatchNumber
        '
        Me.txt_BatchNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_BatchNumber.Location = New System.Drawing.Point(213, 317)
        Me.txt_BatchNumber.Name = "txt_BatchNumber"
        Me.txt_BatchNumber.ReadOnly = True
        Me.txt_BatchNumber.Size = New System.Drawing.Size(100, 20)
        Me.txt_BatchNumber.TabIndex = 9
        Me.txt_BatchNumber.Text = "-------"
        Me.txt_BatchNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
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
        'menu_105
        '
        Me.menu_105.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menu_File, Me.menu_Edit, Me.menu_Mode, Me.ViewToolStripMenuItem1, Me.AboutToolStripMenuItem1})
        Me.menu_105.Location = New System.Drawing.Point(0, 0)
        Me.menu_105.Name = "menu_105"
        Me.menu_105.Size = New System.Drawing.Size(326, 24)
        Me.menu_105.TabIndex = 12
        Me.menu_105.Text = "MenuStrip1"
        '
        'menu_File
        '
        Me.menu_File.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menu_Exit})
        Me.menu_File.Name = "menu_File"
        Me.menu_File.Size = New System.Drawing.Size(37, 20)
        Me.menu_File.Text = "File"
        '
        'menu_Exit
        '
        Me.menu_Exit.Name = "menu_Exit"
        Me.menu_Exit.Size = New System.Drawing.Size(92, 22)
        Me.menu_Exit.Text = "&Exit"
        '
        'menu_Edit
        '
        Me.menu_Edit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GUMPKeywordToolStripMenuItem1, Me.ToolStripSeparator3, Me.menu_DelBatch, Me.menu_DelGUMP, Me.ToolStripSeparator4, Me.menu_Options})
        Me.menu_Edit.Name = "menu_Edit"
        Me.menu_Edit.Size = New System.Drawing.Size(39, 20)
        Me.menu_Edit.Text = "Edit"
        '
        'GUMPKeywordToolStripMenuItem1
        '
        Me.GUMPKeywordToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menu_UAPSUP, Me.menu_UAPCCS})
        Me.GUMPKeywordToolStripMenuItem1.Name = "GUMPKeywordToolStripMenuItem1"
        Me.GUMPKeywordToolStripMenuItem1.Size = New System.Drawing.Size(165, 22)
        Me.GUMPKeywordToolStripMenuItem1.Text = "GUMP &Keyword"
        '
        'menu_UAPSUP
        '
        Me.menu_UAPSUP.Checked = True
        Me.menu_UAPSUP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menu_UAPSUP.Name = "menu_UAPSUP"
        Me.menu_UAPSUP.Size = New System.Drawing.Size(119, 22)
        Me.menu_UAPSUP.Text = "UAPSUP"
        '
        'menu_UAPCCS
        '
        Me.menu_UAPCCS.Name = "menu_UAPCCS"
        Me.menu_UAPCCS.Size = New System.Drawing.Size(119, 22)
        Me.menu_UAPCCS.Text = "UAPCCS"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(162, 6)
        '
        'menu_DelBatch
        '
        Me.menu_DelBatch.Name = "menu_DelBatch"
        Me.menu_DelBatch.Size = New System.Drawing.Size(165, 22)
        Me.menu_DelBatch.Text = "Delete &Batch"
        '
        'menu_DelGUMP
        '
        Me.menu_DelGUMP.Name = "menu_DelGUMP"
        Me.menu_DelGUMP.Size = New System.Drawing.Size(165, 22)
        Me.menu_DelGUMP.Text = "Delete GUMP &File"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(162, 6)
        '
        'menu_Options
        '
        Me.menu_Options.Name = "menu_Options"
        Me.menu_Options.Size = New System.Drawing.Size(165, 22)
        Me.menu_Options.Text = "&Options"
        '
        'menu_Mode
        '
        Me.menu_Mode.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menu_GUMP, Me.menu_Existing, Me.menu_Manual})
        Me.menu_Mode.Name = "menu_Mode"
        Me.menu_Mode.Size = New System.Drawing.Size(50, 20)
        Me.menu_Mode.Text = "Mode"
        '
        'menu_GUMP
        '
        Me.menu_GUMP.Checked = True
        Me.menu_GUMP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menu_GUMP.Name = "menu_GUMP"
        Me.menu_GUMP.Size = New System.Drawing.Size(144, 22)
        Me.menu_GUMP.Text = "&GUMP File"
        '
        'menu_Existing
        '
        Me.menu_Existing.Name = "menu_Existing"
        Me.menu_Existing.Size = New System.Drawing.Size(144, 22)
        Me.menu_Existing.Text = "&Existing Case"
        '
        'menu_Manual
        '
        Me.menu_Manual.Enabled = False
        Me.menu_Manual.Name = "menu_Manual"
        Me.menu_Manual.Size = New System.Drawing.Size(144, 22)
        Me.menu_Manual.Text = "&Manual Entry"
        '
        'ViewToolStripMenuItem1
        '
        Me.ViewToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menu_ViewBatch, Me.menu_IMPSBatch, Me.menu_ViewGUMP})
        Me.ViewToolStripMenuItem1.Name = "ViewToolStripMenuItem1"
        Me.ViewToolStripMenuItem1.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem1.Text = "View"
        '
        'menu_ViewBatch
        '
        Me.menu_ViewBatch.Name = "menu_ViewBatch"
        Me.menu_ViewBatch.Size = New System.Drawing.Size(172, 22)
        Me.menu_ViewBatch.Text = "&Batch List"
        '
        'menu_IMPSBatch
        '
        Me.menu_IMPSBatch.Name = "menu_IMPSBatch"
        Me.menu_IMPSBatch.Size = New System.Drawing.Size(172, 22)
        Me.menu_IMPSBatch.Text = "&IMPS Batch List"
        '
        'menu_ViewGUMP
        '
        Me.menu_ViewGUMP.Name = "menu_ViewGUMP"
        Me.menu_ViewGUMP.Size = New System.Drawing.Size(172, 22)
        Me.menu_ViewGUMP.Text = "&GUMP File Archive"
        '
        'AboutToolStripMenuItem1
        '
        Me.AboutToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menu_Update, Me.ToolStripSeparator1, Me.menu_About})
        Me.AboutToolStripMenuItem1.Name = "AboutToolStripMenuItem1"
        Me.AboutToolStripMenuItem1.Size = New System.Drawing.Size(52, 20)
        Me.AboutToolStripMenuItem1.Text = "About"
        '
        'menu_Update
        '
        Me.menu_Update.Name = "menu_Update"
        Me.menu_Update.Size = New System.Drawing.Size(168, 22)
        Me.menu_Update.Text = "&Check For Update"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(165, 6)
        '
        'menu_About
        '
        Me.menu_About.Name = "menu_About"
        Me.menu_About.Size = New System.Drawing.Size(168, 22)
        Me.menu_About.Text = "&About Phoenix..."
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(41, 20)
        Me.ViewToolStripMenuItem.Text = "&View"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.AboutToolStripMenuItem.Text = "&About"
        '
        'BGW_SQLCheck
        '
        Me.BGW_SQLCheck.WorkerReportsProgress = True
        Me.BGW_SQLCheck.WorkerSupportsCancellation = True
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox2.Image = Global.Phoenix.My.Resources.Resources.RedBirdLogo
        Me.PictureBox2.Location = New System.Drawing.Point(214, 191)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(110, 97)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 13
        Me.PictureBox2.TabStop = False
        '
        'trayIcon
        '
        Me.trayIcon.ContextMenuStrip = Me.tray_Menu
        Me.trayIcon.Icon = CType(resources.GetObject("trayIcon.Icon"), System.Drawing.Icon)
        Me.trayIcon.Text = "Phoenix"
        Me.trayIcon.Visible = True
        '
        'tray_Menu
        '
        Me.tray_Menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tray_Exit})
        Me.tray_Menu.Name = "tray_Menu"
        Me.tray_Menu.Size = New System.Drawing.Size(93, 26)
        '
        'tray_Exit
        '
        Me.tray_Exit.Name = "tray_Exit"
        Me.tray_Exit.Size = New System.Drawing.Size(92, 22)
        Me.tray_Exit.Text = "&Exit"
        '
        'BGW_Existing
        '
        Me.BGW_Existing.WorkerReportsProgress = True
        Me.BGW_Existing.WorkerSupportsCancellation = True
        '
        'BGW_HideTray
        '
        '
        'BGW_OnlineStatus
        '
        Me.BGW_OnlineStatus.WorkerSupportsCancellation = True
        '
        'Put105Thru
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Phoenix.My.Resources.Resources.RedBG
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(326, 347)
        Me.Controls.Add(Me.grp_Mode)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txt_BatchNumber)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txt_Status)
        Me.Controls.Add(Me.menu_105)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.txt_Info)
        Me.Controls.Add(Me.grp_GUMPControls)
        Me.Controls.Add(Me.grp_ExistingControls)
        Me.Controls.Add(Me.grp_ManualControls)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menu_105
        Me.MaximizeBox = False
        Me.Name = "Put105Thru"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Phoenix - 105 Processing"
        Me.grp_Mode.ResumeLayout(False)
        Me.grp_Mode.PerformLayout()
        Me.grp_GUMPControls.ResumeLayout(False)
        Me.grp_ExistingControls.ResumeLayout(False)
        Me.grp_ExistingControls.PerformLayout()
        Me.grp_ManualControls.ResumeLayout(False)
        Me.menu_105.ResumeLayout(False)
        Me.menu_105.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tray_Menu.ResumeLayout(False)
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
    Friend WithEvents btn_ManualStart_U As System.Windows.Forms.Button
    Friend WithEvents txt_Status As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txt_BatchNumber As System.Windows.Forms.TextBox
    Friend WithEvents BGW_ReadGUMP As System.ComponentModel.BackgroundWorker
    Friend WithEvents BGW_GUMPProcess As System.ComponentModel.BackgroundWorker
    Friend WithEvents menu_105 As System.Windows.Forms.MenuStrip
    Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BGW_SQLCheck As System.ComponentModel.BackgroundWorker
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents menu_File As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_Exit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_Edit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GUMPKeywordToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_UAPSUP As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_UAPCCS As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents menu_DelBatch As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_DelGUMP As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents menu_Options As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_Mode As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_Existing As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_Manual As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_ViewBatch As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_GUMP As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_ViewGUMP As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menu_About As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents trayIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents BGW_Existing As System.ComponentModel.BackgroundWorker
    Friend WithEvents tray_Menu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tray_Exit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BGW_HideTray As System.ComponentModel.BackgroundWorker
    Friend WithEvents menu_Update As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents BGW_OnlineStatus As System.ComponentModel.BackgroundWorker
    Friend WithEvents menu_IMPSBatch As System.Windows.Forms.ToolStripMenuItem

End Class
