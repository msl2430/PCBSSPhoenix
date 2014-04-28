Public Class Put105Thru

#Region "Declarations"
    Const COUNTY As String = "016"

    'Public FAMISCaseInformation As CaseInformation
    'Public FAMISApplicationInformation As ApplicationInformation
    'Public FAMISIndividualsInformation As IndividualsInformation
    'Public FAMISMedicaidInformation As MedicaidInformation
    'Public FAMISTANFInformation As TANFInformation
    'Public FAMISIncomeInformation As IncomeInformation
    'Public FAMISFoodStampInformation As FoodStampInformation
    'Public FAMISIandAInformation As IandAInformation
    'Public FAMISVRPInformation(35) As VRPInformation
    'Public FAMISCaseChild(35) As CaseChild
    Public TextFileAdjustments As FAMISDataAdjustments
    'Public numChildren, numVRP As Integer
    Public LoginErrorMsg As String                   '--Capture the login error message--
    Public LineNP As String                          '--String variable holding the user choice of A or U Batch 

    Private TEXT_CaseInformation, TEXT_ApplicationInformation, TEXT_IndividualsInformation, TEXT_MedicaidInformation, TEXT_AFDCInformation, TEXT_IncomeInformation, TEXT_FoodStampInformation, TEXT_IandAInformation, TEXT_VRPInformation(35), TEXT_CaseChild(35) As String
    Private CaseNumber As String                     '--Case number--
    Private FileName As String                       '--Text file name--
    Private CTRL As Control                          '--Global setting for invoke procedure--
    Private BATCHNUMBER As String                    '--Batch Number
    Private LastBatchNumber As String                '--Last successful batchnumber--
    Private isOptions As Boolean                     '--Pause SQL check while in options menu--
    Private isClose As Boolean                       '--Track if we should close the application--

    Public isFileSource As Boolean                   '--Tells what the data source choice's should be--
    Public isCaseCancel As Boolean                   '--Boolean to track if the user canceled the case--
    Public isView105 As Boolean                      '--Tracks if we should view the 105 form before submitting the case--
    Public isCaseError As Boolean                    '--Used to track SQL errors--
    Public isLoginError As Boolean                   '--Tracks if the case was cancelled due to a login/password error--
    Public isCaseSuccessful As Boolean               '--Tracks if case went through successfully--

    '--TODO: Remove--
    Public isRedoCase As Boolean
    'Public TEMPLocation As String   '--TEMP--
#End Region

    Public Sub WriteLog(ByVal Message As String, ByVal isError As Boolean)
        '--Temporary until main screens added--
        Dim LogDirectory As New DirectoryInfo(My.Application.Info.DirectoryPath & "\Log Files\")
        If Not LogDirectory.Exists Then LogDirectory.Create()
        Dim LogDate As String = Date.Now.Month & "_" & Date.Now.Day & "_" & Date.Now.Year
        Dim LogTime As String = Date.Now.TimeOfDay.Hours.ToString & ":" & Date.Now.TimeOfDay.Minutes.ToString & ":" & Date.Now.TimeOfDay.Seconds.ToString
        Dim File_Writer As New StreamWriter(My.Application.Info.DirectoryPath & "\Log Files\Phoenix Log (" & LogDate & ").txt", True)
        If isError Then
            File_Writer.WriteLine(LogDate & " " & LogTime & " >> ERROR: " & Message)
        Else
            File_Writer.WriteLine(LogDate & " " & LogTime & " >> " & Message)
        End If
        File_Writer.Close()
    End Sub
    Private Function isSecurityCheck() As Boolean
        Dim SQLConn As SqlConnection = New SqlConnection(My.Settings.phxSQLConn)
        Dim SQLComm As New SqlCommand
        Dim SQLReader As SqlDataReader
        If CheckSQL() Then
            SQLComm.Connection = SQLConn
            Try
                SQLConn.Open()
                SQLComm.CommandText = "SELECT OperatorNumber FROM OperatorID WHERE OperatorNumber = '" & My.Settings.FAMISOperatorID & "'"
                SQLReader = SQLComm.ExecuteReader
                If Not SQLReader.HasRows Then
                    setInfo("Operator not authorized to use Phoenix.", True)
                    btn_GUMPStop_Click(Nothing, Nothing)
                    setStatus("Error", Color.Red)
                    BGW_SQLCheck.CancelAsync()
                    rdo_GUMP.Enabled = False
                    rdo_Exist.Enabled = False
                    rdo_Manual.Enabled = False
                    btn_GUMPStart.Enabled = False
                    btn_GUMPStop.Enabled = False
                    btn_Search.Enabled = False
                    btn_ManualStart_A.Enabled = False
                    btn_ManualStart_U.Enabled = False
                    menu_Mode.Enabled = False
                    menu_DelBatch.Enabled = False
                    Return False
                Else
                    rdo_GUMP.Enabled = True
                    rdo_Exist.Enabled = True
                    'rdo_Manual.Enabled = True
                    btn_GUMPStart.Enabled = True
                    btn_Search.Enabled = True
                    'btn_ManualStart_A.Enabled = True
                    'btn_ManualStart_U.Enabled = True
                    menu_Mode.Enabled = True
                    menu_DelBatch.Enabled = True
                    Return True
                End If
            Catch ex As Exception
                MessageBox.Show("Location: SecurityCheck" & vbCrLf & ex.Message)
            Finally
                SQLConn.Close()
            End Try
            'Else
            '    'setInfo("SQL Server not found!", True)
            '    'btn_GUMPStop_Click(Nothing, Nothing)
            '    'setStatus("Error", Color.Red)
            '    'BGW_SQLCheck.CancelAsync()
            '    'rdo_GUMP.Enabled = False
            '    'rdo_Exist.Enabled = False
            '    'rdo_Manual.Enabled = False
            '    'btn_GUMPStart.Enabled = False
            '    'btn_GUMPStop.Enabled = False
            '    'btn_Search.Enabled = False
            '    'btn_ManualStart_A.Enabled = False
            '    'btn_ManualStart_U.Enabled = False
            '    'menu_Mode.Enabled = False
            '    'menu_DelBatch.Enabled = False
            '    'Return False
        End If
    End Function

    Private Sub Put105Thru_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        WriteLog("105 processing screen started.", False)
        If My.Settings.FAMISOperatorID = Nothing Or My.Settings.FAMISOperatorID = "" Then GetConfig()
        While My.Settings.FAMISOperatorID = Nothing Or My.Settings.FAMISOperatorID = "" Or My.Settings.FAMISOperatorID = " "
            rdo_GUMP.Enabled = False
            rdo_Exist.Enabled = False
            rdo_Manual.Enabled = False
            btn_GUMPStart.Enabled = False
            btn_GUMPStop.Enabled = False
            btn_Search.Enabled = False
            btn_ManualStart_A.Enabled = False
            btn_ManualStart_U.Enabled = False
            menu_Mode.Enabled = False
            menu_DelBatch.Enabled = False
            MessageBox.Show("No Operator ID Entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            menu_Options_Click(Nothing, Nothing)
        End While
        If Not isSecurityAvailable Then menu_IMPSBatch.Visible = False
        rdo_GUMP.Enabled = True
        rdo_Exist.Enabled = True
        rdo_Manual.Visible = False
        btn_GUMPStart.Enabled = True
        btn_Search.Enabled = False
        'btn_ManualStart_A.Enabled = True
        'btn_ManualStart_U.Enabled = True
        menu_Mode.Enabled = True
        menu_DelBatch.Enabled = True
        My.Settings.phxSQLConn = "User ID=PhoenixUser;Data Source=" & My.Settings.SQLAddress & "\Phoenix;;FailOver Partner=192.168.204.3\Phoenix;Password=password;Initial Catalog=PhoenixData;" & _
                "Connect Timeout=3;Integrated Security=False;Persist Security Info=False;"
        BGW_SQLCheck.RunWorkerAsync()
        isOptions = False
        setStatus("Stopped", Color.OrangeRed)
        If isSecurityAvailable Then
            If isSecurityCheck() Then btn_GUMPStart_Click(Nothing, Nothing)
        Else
            btn_GUMPStart_Click(Nothing, Nothing)
        End If
    End Sub
    Private Sub Put105Thru_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If isSecurityAvailable Then BGW_OnlineStatus_RunWorkerCompleted(Nothing, Nothing)
    End Sub

    Private Sub setInfo(ByVal StringToSend As String, ByVal isError As Boolean)
        Dim counter As Integer = 0
        If isError = False Then
            txt_Info.Text = Date.Now.TimeOfDay.Hours.ToString & ":" & Date.Now.TimeOfDay.Minutes.ToString & ":" & Date.Now.Second & ">> " & StringToSend & vbCrLf & txt_Info.Text
            If Me.WindowState = FormWindowState.Minimized Then
                trayIcon.BalloonTipIcon = ToolTipIcon.Info
                trayIcon.BalloonTipText = StringToSend
                trayIcon.BalloonTipTitle = "Phoenix"
                trayIcon.ShowBalloonTip(8000)
                If Not BGW_HideTray.IsBusy Then BGW_HideTray.RunWorkerAsync()
            End If
        Else
            txt_Info.Text = Date.Now.TimeOfDay.Hours.ToString & ":" & Date.Now.TimeOfDay.Minutes.ToString & ":" & Date.Now.Second & ">> ERROR: " & StringToSend & vbCrLf & txt_Info.Text
            If Me.WindowState = FormWindowState.Minimized Then
                trayIcon.BalloonTipIcon = ToolTipIcon.Error
                trayIcon.BalloonTipText = StringToSend
                trayIcon.BalloonTipTitle = "Phoenix"
                trayIcon.ShowBalloonTip(3000)
                If Not BGW_HideTray.IsBusy Then BGW_HideTray.RunWorkerAsync()
            End If
        End If
        txt_Info.Select(0, 0)
    End Sub
    Private Sub setStatus(ByVal Message As String, ByVal bgColor As Color)
        txt_Status.Text = Message
        txt_Status.BackColor = bgColor
        txt_Info.Focus()
        txt_Info.Select(0, 0)
    End Sub

    Private Sub GetConfig()
        If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.dat") Then
            Try
                DecryptFile(My.Application.Info.DirectoryPath & "\phxConfig.dat", My.Application.Info.DirectoryPath & "\phxConfig.phx", "PhOeNiX9")
                Dim ReadFile As New StreamReader(My.Application.Info.DirectoryPath & "\phxConfig.phx")
                Dim Reader As String
                While ReadFile.Peek <> -1
                    Reader = ReadFile.ReadLine
                    Select Case Reader.Substring(0, Reader.IndexOf(" "))
                        Case "FAMISOperatorID" : My.Settings.FAMISOperatorID = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "FAMISPassword" : My.Settings.FAMISPassword = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "Keyword" : My.Settings.FAMISKeyword = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "BatchNumber_Pre" : My.Settings.BatchNumber_Pre = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "BatchNumber_ANum" : My.Settings.BatchNumber_ANum = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "BatchNumber_UNum" : My.Settings.BatchNumber_UNum = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "FilePath_GUMP" : My.Settings.FilePath_GUMP = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "isArchive" : My.Settings.isArchive = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "ArchiveDirectory" : My.Settings.ArchiveDirectory = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "SQLAddress" : My.Settings.SQLAddress = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "UpdateAddress" : My.Settings.UpdateAddress = Reader.Substring(Reader.IndexOf("=") + 2)
                    End Select
                End While
                ReadFile.Close()
                If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.phx") Then File.Delete(My.Application.Info.DirectoryPath & "\phxConfig.phx")
            Catch ex As Exception
                MessageBox.Show("Location: Get Config" & vbCrLf & ex.Message)
            End Try
        Else
            Try
                Dim WriteFile As New StreamWriter(My.Application.Info.DirectoryPath & "\phxConfig.phx", False)
                WriteFile.WriteLine("FAMISOperatorID = 16A999")
                WriteFile.WriteLine("FAMISPassword = 123456")
                WriteFile.WriteLine("Keyword = UAPSUP")
                WriteFile.WriteLine("BatchNumber_Pre = 999")
                WriteFile.WriteLine("BatchNumber_ANum = 001")
                WriteFile.WriteLine("BatchNumber_UNum = 001")
                WriteFile.WriteLine("FilePath_GUMP = G:\")
                WriteFile.WriteLine("isArchive = False")
                WriteFile.WriteLine("ArchiveDirectory = C:\Program Files\Phoenix\Old Text Files")
                WriteFile.WriteLine("SQLAddress = 192.178.1.1")
                WriteFile.WriteLine("UpdateAddress = 192.178.1.1")
                WriteFile.Close()
                Dim ReadFile As New StreamReader(My.Application.Info.DirectoryPath & "\phxConfig.phx")
                Dim Reader As String
                While ReadFile.Peek <> -1
                    Reader = ReadFile.ReadLine
                    Select Case Reader.Substring(0, Reader.IndexOf(" "))
                        Case "FAMISOperatorID" : My.Settings.FAMISOperatorID = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "FAMISPassword" : My.Settings.FAMISPassword = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "Keyword" : My.Settings.FAMISKeyword = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "BatchNumber_Pre" : My.Settings.BatchNumber_Pre = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "BatchNumber_ANum" : My.Settings.BatchNumber_ANum = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "BatchNumber_UNum" : My.Settings.BatchNumber_UNum = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "FilePath_GUMP" : My.Settings.FilePath_GUMP = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "isArchive" : My.Settings.isArchive = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "ArchiveDirectory" : My.Settings.ArchiveDirectory = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "SQLAddress" : My.Settings.SQLAddress = Reader.Substring(Reader.IndexOf("=") + 2)
                        Case "UpdateAddress" : My.Settings.UpdateAddress = Reader.Substring(Reader.IndexOf("=") + 2)
                    End Select
                End While
                ReadFile.Close()
                If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.dat") Then File.Delete(My.Application.Info.DirectoryPath & "\phxConfig.dat")
                EncryptFile(My.Application.Info.DirectoryPath & "\phxConfig.phx", My.Application.Info.DirectoryPath & "\phxConfig.dat", "PhOeNiX9")
                If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.phx") Then File.Delete(My.Application.Info.DirectoryPath & "\phxConfig.phx")
            Catch ex As Exception
                MessageBox.Show("Location: Create Settings File" & vbCrLf & ex.Message)
            End Try
        End If
        If My.Settings.FAMISKeyword = "UAPSUP" Then
            menu_UAPSUP.Checked = True
            menu_UAPCCS.Checked = False
        Else
            menu_UAPSUP.Checked = False
            menu_UAPCCS.Checked = True
        End If
    End Sub
    Private Sub SetConfig()
        Try
            Dim WriteFile As New StreamWriter(My.Application.Info.DirectoryPath & "\phxConfig.phx", False)
            WriteFile.WriteLine("FAMISOperatorID = " & My.Settings.FAMISOperatorID)
            WriteFile.WriteLine("FAMISPassword = " & My.Settings.FAMISPassword)
            WriteFile.WriteLine("Keyword = " & My.Settings.FAMISKeyword)
            WriteFile.WriteLine("BatchNumber_Pre = " & My.Settings.BatchNumber_Pre)
            WriteFile.WriteLine("BatchNumber_ANum = " & My.Settings.BatchNumber_ANum)
            WriteFile.WriteLine("BatchNumber_UNum = " & My.Settings.BatchNumber_UNum)
            WriteFile.WriteLine("FilePath_GUMP = " & My.Settings.FilePath_GUMP)
            WriteFile.WriteLine("isArchive = " & My.Settings.isArchive)
            WriteFile.WriteLine("ArchiveDirectory = " & My.Settings.ArchiveDirectory)
            WriteFile.WriteLine("SQLAddress = " & My.Settings.SQLAddress)
            WriteFile.WriteLine("UpdateAddress = " & My.Settings.UpdateAddress)
            WriteFile.Close()
            If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.dat") Then File.Delete(My.Application.Info.DirectoryPath & "\phxConfig.dat")
            EncryptFile(My.Application.Info.DirectoryPath & "\phxConfig.phx", My.Application.Info.DirectoryPath & "\phxConfig.dat", "PhOeNiX9")
            If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.phx") Then File.Delete(My.Application.Info.DirectoryPath & "\phxConfig.phx")
        Catch ex As Exception
            MessageBox.Show("Location: Set Config" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub KillGLink()
        Dim GLProcess() As Process
        Dim ProcessToKill As Process
        Dim i As Integer
        GLProcess = Process.GetProcessesByName("gl")
        If GLProcess.Length > 0 Then
            ProcessToKill = GLProcess(0)
            For i = 1 To GLProcess.Length - 1
                If GLProcess(i).StartTime > ProcessToKill.StartTime Then ProcessToKill = GLProcess(i)
            Next
            ProcessToKill.Kill()
        End If
    End Sub

    Public Sub BlankOutMedicaidFields()
        BlankOutField(FAMISCaseInformation.AN)

        BlankOutField(FAMISApplicationInformation.EK)
        BlankOutField(FAMISApplicationInformation.EL)
        BlankOutField(FAMISApplicationInformation.EM)
        BlankOutField(FAMISApplicationInformation.EN)
        BlankOutField(FAMISApplicationInformation.XM)
        BlankOutField(FAMISApplicationInformation.XN)

        If (FAMISCaseInformation.AL.GetData().Substring(0, 1) <> "0") Then
            BlankOutField(FAMISApplicationInformation.BH)
            BlankOutField(FAMISApplicationInformation.BQ)
            BlankOutField(FAMISIndividualsInformation.GB)
            BlankOutField(FAMISIndividualsInformation.GC)
            BlankOutField(FAMISIndividualsInformation.GH)
            BlankOutField(FAMISIndividualsInformation.GI)
        End If

        BlankOutField(FAMISIncomeInformation.KJ)
        BlankOutField(FAMISIncomeInformation.JJ)
        BlankOutField(FAMISIncomeInformation.JQ)
        BlankOutField(FAMISIncomeInformation.JS)
        BlankOutField(FAMISIncomeInformation.JT)

        BlankOutField(FAMISMedicaidInformation.HB)
        BlankOutField(FAMISMedicaidInformation.HC)
        BlankOutField(FAMISMedicaidInformation.HD)
        BlankOutField(FAMISMedicaidInformation.HE)
        BlankOutField(FAMISMedicaidInformation.HI)
        BlankOutField(FAMISMedicaidInformation.HJ)
        BlankOutField(FAMISMedicaidInformation.HK)
        BlankOutField(FAMISMedicaidInformation.HL)
        BlankOutField(FAMISMedicaidInformation.HM)
        BlankOutField(FAMISMedicaidInformation.HQ)
        BlankOutField(FAMISMedicaidInformation.HR)
        BlankOutField(FAMISMedicaidInformation.HS)
        BlankOutField(FAMISMedicaidInformation.HT)
        BlankOutField(FAMISMedicaidInformation.WA)
        BlankOutField(FAMISMedicaidInformation.WB)
        BlankOutField(FAMISMedicaidInformation.WC)
        BlankOutField(FAMISMedicaidInformation.WD)
        BlankOutField(FAMISMedicaidInformation.WE)
        BlankOutField(FAMISMedicaidInformation.WF)
        BlankOutField(FAMISMedicaidInformation.WG)
        BlankOutField(FAMISMedicaidInformation.WH)
        BlankOutField(FAMISMedicaidInformation.WI)
        BlankOutField(FAMISMedicaidInformation.WL)
        BlankOutField(FAMISMedicaidInformation.WP)
        BlankOutField(FAMISMedicaidInformation.WR)
        BlankOutField(FAMISMedicaidInformation.WS)
        BlankOutField(FAMISMedicaidInformation.WW)
        BlankOutField(FAMISMedicaidInformation.XP)

        For i As Integer = 0 To numChildren - 1
            BlankOutField(FAMISCaseChild(i).QH)
            If FAMISCaseInformation.AL.GetData.Substring(0, 1) <> "0" Then
                BlankOutField(FAMISCaseChild(i).QK)
                BlankOutField(FAMISCaseChild(i).QL)
                BlankOutField(FAMISCaseChild(i).TD)
            End If
            If FAMISCaseInformation.AL.GetData.Substring(0, 1) = " " Then
                BlankOutField(FAMISCaseChild(i).QJ)
            End If
            BlankOutField(FAMISCaseChild(i).RD)
            BlankOutField(FAMISCaseChild(i).RF)
            BlankOutField(FAMISCaseChild(i).RN)
            BlankOutField(FAMISCaseChild(i).RO)
            BlankOutField(FAMISCaseChild(i).SR)            
            BlankOutField(FAMISCaseChild(i).UI)
        Next
    End Sub
    Private Sub BlankOutField(ByRef Block As FAMISBlock)
        Dim length As Integer = Block.Length
        Block.SetData(" ".PadRight(length))
    End Sub
    Private Sub BlankOutField(ByRef Block As FAMISBlock_Date)
        Dim length As Integer = Block.Length
        Block.SetData(" ".PadRight(length))
    End Sub

    Private Sub getBatchNumber(ByVal BatchType As String)
        If BatchType = "A" Then BATCHNUMBER = BatchType & My.Settings.BatchNumber_Pre & My.Settings.BatchNumber_ANum Else BATCHNUMBER = BatchType & My.Settings.BatchNumber_Pre & My.Settings.BatchNumber_UNum
    End Sub
    Private Sub setBatchNumber(ByVal BatchType As String)
        If BatchType = "A" Then
            If My.Settings.BatchNumber_ANum = "999" Then
                My.Settings.BatchNumber_ANum = "001"
            Else
                My.Settings.BatchNumber_ANum += 1
                My.Settings.BatchNumber_ANum = My.Settings.BatchNumber_ANum.PadLeft(3, "0")
            End If
            BATCHNUMBER = BatchType & My.Settings.BatchNumber_Pre & My.Settings.BatchNumber_ANum
        Else
            If My.Settings.BatchNumber_UNum = "999" Then
                My.Settings.BatchNumber_UNum = "001"
            Else
                My.Settings.BatchNumber_UNum += 1
                My.Settings.BatchNumber_UNum = My.Settings.BatchNumber_UNum.PadLeft(3, "0")
            End If
            BATCHNUMBER = BatchType & My.Settings.BatchNumber_Pre & My.Settings.BatchNumber_UNum
        End If
        SetConfig()
    End Sub

    Private Sub SQLConnectionError(ByVal isSQLError As Boolean)
        If isSQLError = True Then
            rdo_GUMP.Enabled = False
            rdo_Exist.Enabled = False
            rdo_Manual.Enabled = False
            btn_GUMPStart.Enabled = False
            btn_GUMPStop.Enabled = False
            btn_Search.Enabled = False
            btn_ManualStart_A.Enabled = False
            btn_ManualStart_U.Enabled = False
            menu_Mode.Enabled = False
            menu_DelBatch.Enabled = False
        Else
            rdo_GUMP.Enabled = True
            rdo_Exist.Enabled = True
            'rdo_Manual.Enabled = True
            btn_GUMPStart.Enabled = True
            btn_Search.Enabled = True
            'btn_ManualStart_A.Enabled = True
            'btn_ManualStart_U.Enabled = True
            menu_Mode.Enabled = True
            menu_DelBatch.Enabled = True
        End If
    End Sub
    Private Function CheckSQL() As Boolean
        Dim SQLConnCheck As SqlConnection = New SqlConnection(My.Settings.phxSQLConn)
        Try
            If My.Computer.Network.Ping(My.Settings.SQLAddress, 10000) Then
                SQLConnCheck.Open()
            Else
                If My.Computer.Network.Ping(My.Settings.SQLAddress, 10000) Then
                    SQLConnCheck.Open()
                Else
                    Return False
                End If
            End If
            SQLConnCheck.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Sub BGW_SQLCheck_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_SQLCheck.DoWork
        If isClose = True Then Me.Close()
        Dim isSQLFound As Boolean = True
        Dim isCheckSQL As Boolean
        Thread.Sleep(600000)
        While Not BGW_SQLCheck.CancellationPending
            If Not isOptions Then
                isCheckSQL = CheckSQL()
                If isCheckSQL Then
                    If Not isSQLFound Then
                        isSQLFound = True
                        BGW_SQLCheck.ReportProgress(11)
                    End If
                    isSQLFound = True
                Else
                    If isSQLFound Then BGW_SQLCheck.ReportProgress(5)
                    BGW_SQLCheck.ReportProgress(10)
                    isSQLFound = False
                End If
            End If
            Thread.Sleep(5000)
        End While
    End Sub
    Private Sub BGW_SQLCheck_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BGW_SQLCheck.ProgressChanged
        Select Case e.ProgressPercentage
            Case 5
                If BGW_GUMPProcess.IsBusy Then btn_GUMPStop.PerformClick()
                setInfo("Server not found!", True)
                WriteLog("Cannot find SQL Server @ " & My.Settings.SQLAddress, True)
            Case 10
                SQLConnectionError(True)
            Case 11
                SQLConnectionError(False)
                setInfo("Server found!", False)
                WriteLog("SQL Server found after disconnection @ " & My.Settings.SQLAddress, False)
        End Select
    End Sub
    Private Sub BGW_OnlineStatus_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_OnlineStatus.DoWork
        Dim SQLOnlineConn As New SqlConnection(My.Settings.phxSQLConn) '"Data Source=172.16.8.15\PHOENIX;Initial Catalog=PhoenixCaseData;User ID=famisuser;Password=password")
        Dim SQLOnlineComm As New SqlCommand
        Dim SQLReader As SqlDataReader
        Dim isFirstTime As Boolean = True
        Dim Version As String = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Revision.ToString

        While Not BGW_OnlineStatus.CancellationPending
            If Date.Now.Minute Mod 15 = 0 Or isFirstTime = True Then
                If CheckSQL() Then
                    SQLOnlineComm.Connection = SQLOnlineConn
                    isFirstTime = False
                    Try
                        SQLOnlineConn.Open()
                        SQLOnlineComm.CommandText = "SELECT * FROM OnlineUsers WHERE Operator = '" & My.Settings.FAMISOperatorID & "'"
                        SQLReader = SQLOnlineComm.ExecuteReader
                        SQLReader.Read()
                        If SQLReader.HasRows Then
                            SQLReader.Close()
                            SQLOnlineComm.CommandText = "UPDATE OnlineUsers SET Status = 'Online', CheckInTime = '" & Date.Now & "', Version = '" & Version & "' WHERE Operator = '" & My.Settings.FAMISOperatorID & "'"
                            SQLOnlineComm.ExecuteNonQuery()
                        Else
                            SQLReader.Close()
                            SQLOnlineComm.CommandText = "INSERT INTO OnlineUsers VALUES ('" & My.Settings.FAMISOperatorID & "', 'Online', '" & Date.Now & "', 0, '" & Version & "')"
                            SQLOnlineComm.ExecuteNonQuery()
                        End If
                    Catch ex As Exception
                        MessageBox.Show("Location: BGW_OnlineStatus" & vbCrLf & ex.Message.ToString)
                    Finally
                        SQLOnlineConn.Close()
                    End Try
                End If
            End If
            Thread.Sleep(5000)
        End While
    End Sub
    Private Sub BGW_OnlineStatus_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGW_OnlineStatus.RunWorkerCompleted
        Dim SQLOnlineConn As New SqlConnection(My.Settings.phxSQLConn) '"Data Source=172.16.8.15\PHOENIX;Initial Catalog=PhoenixCaseData;User ID=famisuser;Password=password")
        Dim SQLOnlineComm As New SqlCommand
        SQLOnlineComm.Connection = SQLOnlineConn
        Try
            If CheckSQL() Then
                SQLOnlineConn.Open()
                SQLOnlineComm.CommandText = "UPDATE OnlineUsers SET Status = 'Offline', CheckInTime = '" & Date.Now & "' WHERE Operator = '" & My.Settings.FAMISOperatorID & "'"
                SQLOnlineComm.ExecuteNonQuery()
            End If
        Catch ex As Exception
            MessageBox.Show("Location: BGW_OnlineStatus Complete" & vbCrLf & ex.Message.ToString)
        Finally
            SQLOnlineConn.Close()
        End Try
    End Sub
    Private Sub BGW_HideTray_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_HideTray.DoWork
        Thread.Sleep(3000)
    End Sub
    Private Sub BGW_HideTray_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGW_HideTray.RunWorkerCompleted
        trayIcon.Visible = False
        trayIcon.Visible = True
    End Sub

#Region "GUMP Functions"
    Private Sub BGW_GUMPProcess_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_GUMPProcess.DoWork
        Dim Directory As New DirectoryInfo(My.Settings.FilePath_GUMP)
        Dim FileList() As FileInfo
        Dim i, MAX, timeout As Integer
        Dim frmProcess As processingFAMIS
        Dim frmNPDialog As NPDialog
        Dim isOperator As Boolean
        LineNP = Nothing
        isRedoCase = False '--TEMP--
        While Not BGW_GUMPProcess.CancellationPending
            isOperator = False
            If Directory.Exists Then
                FileList = Directory.GetFiles("*.txt")
                If FileList.Length > 0 Then
                    MAX = FileList.Length
                    For i = 0 To MAX - 1
                        FileName = FileList(i).Name
                        If FileName.Substring(0, 6) = My.Settings.FAMISOperatorID Then
                            isOperator = True
                            '--Start reading in the file in the background to save time--
                            BGW_ReadGUMP.RunWorkerAsync()
                            '--Ask the user what kind of case this is--
                            If Not isRedoCase Then '--TEMP--
                                frmNPDialog = New NPDialog
                                frmNPDialog.CaseNumber = FileName.Replace(".TXT", "").Substring(6)
                                frmNPDialog.NPParentForm_105 = Me
                                frmNPDialog.LastChoice = LineNP
                                BGW_GUMPProcess.ReportProgress(10)
                                frmNPDialog.ShowDialog()
                                If LineNP = "ABatch" Then
                                    getBatchNumber("A")
                                ElseIf LineNP = "UBatch" Then
                                    getBatchNumber("U")
                                ElseIf LineNP = "Cancel" Then
                                    BGW_GUMPProcess.CancelAsync()
                                Else
                                    '--Something strange happened in the NP dialog--
                                    BGW_GUMPProcess.ReportProgress(20)
                                End If
                            Else
                                isRedoCase = False
                            End If '--TEMP--
                            '--Waiting for the case to finish being read in--
                            timeout = 0
                            While BGW_ReadGUMP.IsBusy
                                Application.DoEvents()
                                Thread.Sleep(350)
                                timeout += 1
                                If timeout = 60 Then
                                    '--Background Worker seems to be stuck setting the GUMP information--
                                    BGW_GUMPProcess.ReportProgress(30)
                                End If
                            End While
                            If LineNP <> "Cancel" Then
                                If LineNP = "UBatch" Then
                                    TextFileAdjustments.del_LineN(FAMISFoodStampInformation)
                                    TextFileAdjustments.del_LineP(FAMISIandAInformation, True, True)
                                End If
                                BlankOutMedicaidFields()
                                '--Sending the GUMP info to FAMIS for processing--
                                BGW_SQLCheck.CancelAsync()
                                BGW_GUMPProcess.ReportProgress(40)
                                frmProcess = New processingFAMIS
                                frmProcess.isView105 = isView105
                                frmProcess.isFileSource = isFileSource
                                frmProcess.BATCHNUMBER = BATCHNUMBER
                                frmProcess.ParentForm_Put105 = Me
                                isCaseError = False '--Set to false unless error occurs in processing--
                                BGW_GUMPProcess.ReportProgress(2)
                                If CheckSQL() Then
                                    frmProcess.ShowDialog()
                                    BGW_GUMPProcess.ReportProgress(50)
                                    If isCaseCancel Then
                                        '--Case was canceled by the user--
                                        BGW_GUMPProcess.ReportProgress(60)
                                    ElseIf isLoginError Then
                                        '--Login/Password error--
                                        BGW_GUMPProcess.ReportProgress(46)
                                    ElseIf isCaseSuccessful Then
                                        '--Case successful--
                                        LastBatchNumber = BATCHNUMBER
                                        BGW_GUMPProcess.ReportProgress(45)
                                        setBatchNumber(BATCHNUMBER.Substring(0, 1))
                                    Else
                                        '--FAMIS errors--
                                        '--Retry cases--
                                        isRedoCase = True
                                        BGW_GUMPProcess.ReportProgress(47)
                                    End If
                                    If BGW_GUMPProcess.CancellationPending Then
                                        '--The user or application pressed the stop button--
                                        '--Force the for loop to end--
                                        i = MAX
                                    End If
                                    If Not BGW_SQLCheck.IsBusy Then BGW_SQLCheck.RunWorkerAsync()
                                Else
                                    LineNP = "Cancel"
                                    BGW_GUMPProcess.ReportProgress(99)
                                    BGW_GUMPProcess.ReportProgress(3)
                                    BGW_GUMPProcess.CancelAsync()
                                End If
                            End If
                        End If
                        If LineNP <> "Cancel" And isOperator = True And isLoginError = False Then
                            '--Delete file if the user didn't cancel from NP dialog, the file is prefixed with the--
                            '--operator number, and there wasn't a login error. Else do nothing--
                            If File.Exists(My.Settings.FilePath_GUMP & FileName) And Not isRedoCase Then
                                If My.Settings.isArchive And Not isCaseCancel Then
                                    '--If selected, archive the text file instead of deleting it--
                                    Dim ArchDirectory As New DirectoryInfo(My.Settings.ArchiveDirectory & "\" & Date.Today.Month.ToString & "_" & Date.Today.Day.ToString & "_" & Date.Today.Year.ToString & "\")
                                    If Not ArchDirectory.Exists Then ArchDirectory.Create()
                                    If File.Exists(My.Settings.ArchiveDirectory & "\" & Date.Today.Month & "_" & Date.Today.Day & "_" & Date.Today.Year & "\" & FileName) Then File.Delete(My.Settings.ArchiveDirectory & "\" & Date.Today.Month & "_" & Date.Today.Day & "_" & Date.Today.Year & "\" & FileName)
                                    File.Move(My.Settings.FilePath_GUMP & FileName, My.Settings.ArchiveDirectory & "\" & Date.Today.Month & "_" & Date.Today.Day & "_" & Date.Today.Year & "\" & FileName)
                                Else
                                    '--TEMP-- Remove If statement
                                    If Not isRedoCase Then File.Delete(My.Settings.FilePath_GUMP & FileName)
                                End If
                            End If
                        ElseIf LineNP = "Cancel" And isOperator = True Then
                            If BGW_ReadGUMP.IsBusy Then BGW_ReadGUMP.CancelAsync()
                        End If
                        isCaseError = False
                        isCaseSuccessful = False
                        isLoginError = False
                        isCaseCancel = False
                    Next
                End If
            Else
                MessageBox.Show("Directory does not exist!", "Phoenix - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                BGW_GUMPProcess.CancelAsync()
                BGW_GUMPProcess.ReportProgress(70)
            End If
            If Not BGW_GUMPProcess.CancellationPending Then Thread.Sleep(3237)
        End While
        BGW_GUMPProcess.ReportProgress(100)
    End Sub
    Private Sub BGW_GUMPProcess_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BGW_GUMPProcess.ProgressChanged
        Select Case e.ProgressPercentage
            Case 2      '--Minimize form--
                If WindowState = FormWindowState.Normal Then WindowState = FormWindowState.Minimized
            Case 3      '--Restate form--
                If WindowState = FormWindowState.Minimized Then WindowState = FormWindowState.Normal
            Case 10     '--NP Dialog starting unenable main form--
                Me.Enabled = False
                Me.WindowState = FormWindowState.Minimized
                Me.Activate()
            Case 20     '--NP Dialog error--
                BGW_GUMPProcess.CancelAsync()
                setInfo("Unknown error in Line N/P dialog!", True)
                WriteLog("Unknown error in Line N/P dialog!", True)
            Case 30     '--BGW ReadGUMP error--
                BGW_GUMPProcess.CancelAsync()
                setInfo("Unable to read GUMP file!", True)
                WriteLog("Unable to read GUMP file!", True)
            Case 40     '--Begin processing case--
                setInfo("Processing GUMP Case: " & FileName.Replace(".TXT", "").Substring(6), False)
                WriteLog("Processing GUMP Case: " & FileName.Replace(".TXT", "").Substring(6), False)
                If Me.ShowInTaskbar = True Then Me.ShowInTaskbar = False
            Case 45     '--Case completed--
                txt_BatchNumber.Text = LastBatchNumber
                setInfo(FAMISCaseInformation.AA.GetData & " submitted successfully.", False)
                WriteLog(FAMISCaseInformation.AA.GetData & " submitted successfully. (" & LastBatchNumber & ")", False)
            Case 46     '--Login error--
                WriteLog("Login Error!" & vbCrLf & LoginErrorMsg & ".", True)
                setInfo(LoginErrorMsg & ".", True)
                btn_GUMPStop_Click(Nothing, Nothing)
            Case 47     '--Unknown Result--
                Dim SQLConn As New SqlConnection(My.Settings.phxSQLConn)
                Dim SQLComm As New SqlCommand
                SQLComm.Connection = SQLConn
                isRedoCase = True
                setInfo("FAMIS Communication Error! " & vbCrLf & "Retrying case...", False)
                KillGLink()
            Case 50     '--Case finished processing--
                Me.Enabled = True
                Me.Focus()
                txt_Info.Focus()
            Case 60     '--Report case was cancelled--
                setInfo(FAMISCaseInformation.AA.GetData & " was canceled.", False)
            Case 70     '--Halt on directory error--
                setInfo("Directory does not exist!", True)
                WriteLog("Directory does not exist!", True)
            Case 98
                'setInfo("Error backing up case to Server!", True)
                'WriteLog("Error backing up case to Server!", True)
                ''--TEMP--
                ''--Send file to server for SQL resubmission--
                'If File.Exists("\\" & My.Settings.SQLAddress & "\FAMISDrops\" & FileName) Then File.Delete("\\" & My.Settings.SQLAddress & "\FAMISDrops\" & FileName)
                'File.Move(My.Settings.FilePath_GUMP & FileName, "\\" & My.Settings.SQLAddress & "\FAMISDrops\" & FileName)
            Case 99     '--Halt on SQL connection error--
                setInfo("Server not found! Cannot process case.", True)
                WriteLog("Server not found! Cannot process case.", True)
            Case 100    '--Thread done reset main screens buttons--
                btn_GUMPStop.Enabled = False
                btn_GUMPStart.Enabled = True
                rdo_GUMP.Enabled = True
                rdo_Exist.Enabled = True
                'rdo_Manual.Enabled = True
                Me.Enabled = True
                Me.WindowState = FormWindowState.Normal
                If Me.ShowInTaskbar = False Then Me.ShowInTaskbar = True
                Me.Focus()
                If BGW_OnlineStatus.IsBusy Then BGW_OnlineStatus.CancelAsync()
                setInfo("GUMP processing stopped.", False)
                setStatus("Stopped", Color.OrangeRed)
        End Select
    End Sub
    Private Sub BGW_GUMPProcess_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGW_GUMPProcess.RunWorkerCompleted
        menu_File.Enabled = True
        menu_Edit.Enabled = True
        menu_Mode.Enabled = True
        menu_About.Enabled = True
        tray_Exit.Enabled = True
    End Sub
    Private Sub BGW_ReadGUMP_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_ReadGUMP.DoWork
        Dim i As Integer
        Try
            Read_TextFile()
            setBlockData_Regular()
            setBlockData_Child()
            setBlockData_VRP()
            '--Call the data adjustment class to correct all GUMP inaccuracies--
            TextFileAdjustments = New FAMISDataAdjustments(FAMISCaseInformation, FAMISApplicationInformation, FAMISIndividualsInformation, FAMISMedicaidInformation, FAMISTANFInformation, FAMISIncomeInformation, FAMISFoodStampInformation, FAMISIandAInformation)
            For i = 0 To numChildren - 1
                TextFileAdjustments.DataAdjustments_Child(FAMISCaseChild(i))
            Next
        Catch ex As Exception
            MessageBox.Show("Location: ReadGUMPFile" & vbCrLf & "Error reading GUMP file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Read_TextFile()
        Try
            Dim infile As New StreamReader(My.Settings.FilePath_GUMP & FileName, System.Text.Encoding.Default)
            Dim Record As String
            numChildren = 0
            numVRP = 0
            While infile.Peek <> -1
                Record = infile.ReadLine()
                If Record <> Nothing Then
                    Select Case Record.Substring(44, 40)
                        Case "FAMISAFDCInformation                    "
                            TEXT_AFDCInformation = Record
                        Case "FAMISApplicantInformation               "
                            TEXT_ApplicationInformation = Record
                        Case "FAMISCaseChild                          "
                            TEXT_CaseChild(numChildren) = Record
                            numChildren += 1
                        Case "FAMISCaseInformation                    "
                            TEXT_CaseInformation = Record
                        Case "FAMISFoodStampInformation               "
                            TEXT_FoodStampInformation = Record
                        Case "FAMISIndividualsInformation             "
                            TEXT_IndividualsInformation = Record
                        Case "FAMISIandAInformation                   "
                            TEXT_IandAInformation = Record
                        Case "FAMISIncomeInformation                  "
                            TEXT_IncomeInformation = Record
                        Case "FAMISIndividualsInformation             "
                            TEXT_IndividualsInformation = Record
                        Case "FAMISMedicaidInformation                "
                            TEXT_MedicaidInformation = Record
                        Case "FAMISCaseVRPInformation                 "
                            TEXT_VRPInformation(numVRP) = Record
                            numVRP += 1
                    End Select
                End If
            End While
            infile.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
        End Try
    End Sub
    Private Sub getBlockData(ByRef BLOCK As FAMISBlock)
        Select Case BLOCK.FileTable
            Case "FAMISAFDCInformation"
                BLOCK.SetData(TEXT_AFDCInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISApplicantInformation"
                BLOCK.SetData(TEXT_ApplicationInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISCaseInformation"
                BLOCK.SetData(TEXT_CaseInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISFoodStampInformation"
                BLOCK.SetData(TEXT_FoodStampInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISIandAInformation"
                BLOCK.SetData(TEXT_IandAInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISIncomeInformation"
                BLOCK.SetData(TEXT_IncomeInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISIndividualsInformation"
                BLOCK.SetData(TEXT_IndividualsInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISMedicaidInformation"
                BLOCK.SetData(TEXT_MedicaidInformation.Substring(BLOCK.StartIndex, BLOCK.Length))
        End Select
    End Sub
    Private Sub getBlockData(ByRef BLOCK As FAMISBlock, ByVal ArrayNum As Integer)
        Select Case BLOCK.FileTable
            Case "FAMISCaseChild"
                BLOCK.SetData(TEXT_CaseChild(ArrayNum).Substring(BLOCK.StartIndex, BLOCK.Length))
            Case "FAMISCaseVRPInformation"
                BLOCK.SetData(TEXT_VRPInformation(ArrayNum).Substring(BLOCK.StartIndex, BLOCK.Length))
        End Select
    End Sub
    Private Sub setBlockData_Regular()
        FAMISCaseInformation = New CaseInformation
        FAMISMedicaidInformation = New MedicaidInformation
        FAMISFoodStampInformation = New FoodStampInformation
        FAMISApplicationInformation = New ApplicationInformation
        FAMISTANFInformation = New TANFInformation
        FAMISIandAInformation = New IandAInformation
        FAMISIndividualsInformation = New IndividualsInformation
        FAMISIncomeInformation = New IncomeInformation

        getBlockData(FAMISCaseInformation.AA)
        getBlockData(FAMISCaseInformation.AB)
        getBlockData(FAMISCaseInformation.AC)
        getBlockData(FAMISCaseInformation.AD)
        getBlockData(FAMISCaseInformation.AE)
        getBlockData(FAMISCaseInformation.AF)
        getBlockData(FAMISCaseInformation.AG)
        getBlockData(FAMISCaseInformation.AH)
        getBlockData(FAMISCaseInformation.AI)
        getBlockData(FAMISCaseInformation.AJ)
        getBlockData(FAMISCaseInformation.AK)
        getBlockData(FAMISCaseInformation.AL)
        getBlockData(FAMISCaseInformation.AM)
        getBlockData(FAMISCaseInformation.AN)
        getBlockData(FAMISCaseInformation.AQ)

        getBlockData(FAMISApplicationInformation.BA)
        getBlockData(FAMISApplicationInformation.BB)
        getBlockData(FAMISApplicationInformation.BC)
        getBlockData(FAMISApplicationInformation.BD)
        getBlockData(FAMISApplicationInformation.BE)
        getBlockData(FAMISApplicationInformation.BF)
        getBlockData(FAMISApplicationInformation.BG)
        getBlockData(FAMISApplicationInformation.BH)
        getBlockData(FAMISApplicationInformation.BI)
        getBlockData(FAMISApplicationInformation.BJ)
        getBlockData(FAMISApplicationInformation.BK)
        getBlockData(FAMISApplicationInformation.BL)
        getBlockData(FAMISApplicationInformation.BM)
        getBlockData(FAMISApplicationInformation.BN)
        getBlockData(FAMISApplicationInformation.BO)
        getBlockData(FAMISApplicationInformation.BP)
        getBlockData(FAMISApplicationInformation.BQ)
        getBlockData(FAMISApplicationInformation.BR)
        getBlockData(FAMISApplicationInformation.BS)
        getBlockData(FAMISApplicationInformation.BT)
        getBlockData(FAMISApplicationInformation.BU)
        getBlockData(FAMISApplicationInformation.BV)
        getBlockData(FAMISApplicationInformation.BW)
        getBlockData(FAMISApplicationInformation.BX)
        getBlockData(FAMISApplicationInformation.BY1)
        getBlockData(FAMISApplicationInformation.BZ)

        getBlockData(FAMISApplicationInformation.CA)
        'getBlockData(FAMISApplicationInformation.CB)
        If FAMISApplicationInformation.CA.GetData.Substring(0, 1) <> " " Then FAMISApplicationInformation.CB.SetData("X")
        If FAMISApplicationInformation.CA.GetData.Substring(0, 1) = "-" Then FAMISApplicationInformation.CB.SetData("-")
        getBlockData(FAMISApplicationInformation.CC)
        getBlockData(FAMISApplicationInformation.CD1)
        getBlockData(FAMISApplicationInformation.CD2)
        getBlockData(FAMISApplicationInformation.CE)
        getBlockData(FAMISApplicationInformation.CF)
        getBlockData(FAMISApplicationInformation.CG)

        getBlockData(FAMISApplicationInformation.DA1)
        getBlockData(FAMISApplicationInformation.DA2)
        getBlockData(FAMISApplicationInformation.DA3)
        getBlockData(FAMISApplicationInformation.DB)
        getBlockData(FAMISApplicationInformation.DC)
        getBlockData(FAMISApplicationInformation.DD1)
        getBlockData(FAMISApplicationInformation.DD2)
        getBlockData(FAMISApplicationInformation.DE)
        getBlockData(FAMISApplicationInformation.DF)

        getBlockData(FAMISApplicationInformation.EA)
        getBlockData(FAMISApplicationInformation.EB)
        getBlockData(FAMISApplicationInformation.EC)
        getBlockData(FAMISApplicationInformation.ED1)
        getBlockData(FAMISApplicationInformation.ED2)
        getBlockData(FAMISApplicationInformation.EE)
        getBlockData(FAMISApplicationInformation.EF)
        getBlockData(FAMISApplicationInformation.EG)
        If FAMISApplicationInformation.EG.GetData.Substring(0, 1) = " " Then FAMISApplicationInformation.EG.SetData("-") '--Always delete sign if GUMP brings in blank-- 
        getBlockData(FAMISApplicationInformation.EH)
        getBlockData(FAMISApplicationInformation.EI)
        getBlockData(FAMISApplicationInformation.EJ)
        getBlockData(FAMISApplicationInformation.EK)
        getBlockData(FAMISApplicationInformation.EL)
        getBlockData(FAMISApplicationInformation.EM)
        getBlockData(FAMISApplicationInformation.EN)


        getBlockData(FAMISApplicationInformation.XA)
        getBlockData(FAMISApplicationInformation.XB)
        getBlockData(FAMISApplicationInformation.XC)
        getBlockData(FAMISApplicationInformation.XD)
        getBlockData(FAMISApplicationInformation.XE)
        getBlockData(FAMISApplicationInformation.XF)
        getBlockData(FAMISApplicationInformation.XG)
        getBlockData(FAMISApplicationInformation.XH)
        getBlockData(FAMISApplicationInformation.XI)
        getBlockData(FAMISApplicationInformation.XJ)
        getBlockData(FAMISApplicationInformation.XK)
        getBlockData(FAMISApplicationInformation.XL)
        getBlockData(FAMISApplicationInformation.XM)
        getBlockData(FAMISApplicationInformation.XN)

        getBlockData(FAMISIndividualsInformation.FA)
        getBlockData(FAMISIndividualsInformation.FB)
        getBlockData(FAMISIndividualsInformation.FC)
        getBlockData(FAMISIndividualsInformation.FD)
        getBlockData(FAMISIndividualsInformation.FD2)
        getBlockData(FAMISIndividualsInformation.FE1)
        getBlockData(FAMISIndividualsInformation.FE2)
        getBlockData(FAMISIndividualsInformation.FF)
        getBlockData(FAMISIndividualsInformation.FG)
        getBlockData(FAMISIndividualsInformation.FG2)
        getBlockData(FAMISIndividualsInformation.FG3)
        FAMISIndividualsInformation.FG.SetData(FAMISIndividualsInformation.FG.GetData & FAMISIndividualsInformation.FG2.GetData & FAMISIndividualsInformation.FG3.GetData)
        FAMISIndividualsInformation.FG.Length = FAMISIndividualsInformation.FG.Length + FAMISIndividualsInformation.FG2.Length + FAMISIndividualsInformation.FG3.Length
        getBlockData(FAMISIndividualsInformation.FH)
        getBlockData(FAMISIndividualsInformation.FI)
        getBlockData(FAMISIndividualsInformation.FK)
        getBlockData(FAMISIndividualsInformation.FJ)
        getBlockData(FAMISIndividualsInformation.FL)
        getBlockData(FAMISIndividualsInformation.FL2)
        getBlockData(FAMISIndividualsInformation.FM1)
        getBlockData(FAMISIndividualsInformation.FM2)
        getBlockData(FAMISIndividualsInformation.FN)
        getBlockData(FAMISIndividualsInformation.FO)
        getBlockData(FAMISIndividualsInformation.FP)
        getBlockData(FAMISIndividualsInformation.FP2)
        getBlockData(FAMISIndividualsInformation.FP3)
        FAMISIndividualsInformation.FP.SetData(FAMISIndividualsInformation.FP.GetData & FAMISIndividualsInformation.FP2.GetData & FAMISIndividualsInformation.FP3.GetData)
        FAMISIndividualsInformation.FP.Length = FAMISIndividualsInformation.FP.Length + FAMISIndividualsInformation.FP2.Length + FAMISIndividualsInformation.FP3.Length

        getBlockData(FAMISIndividualsInformation.B1)
        getBlockData(FAMISIndividualsInformation.B2)
        getBlockData(FAMISIndividualsInformation.B3)
        getBlockData(FAMISIndividualsInformation.B4)
        If TEXT_IndividualsInformation.Length > 442 Then
            getBlockData(FAMISIndividualsInformation.B5)
            getBlockData(FAMISIndividualsInformation.B6)
        Else
            FAMISIndividualsInformation.B5.SetData("        ")
            FAMISIndividualsInformation.B6.SetData("        ")
        End If
        getBlockData(FAMISIndividualsInformation.FQ)
        getBlockData(FAMISIndividualsInformation.FR)

        getBlockData(FAMISIndividualsInformation.GA)
        getBlockData(FAMISIndividualsInformation.GB)
        getBlockData(FAMISIndividualsInformation.GC)
        getBlockData(FAMISIndividualsInformation.GD)
        getBlockData(FAMISIndividualsInformation.GE)
        getBlockData(FAMISIndividualsInformation.GF)
        getBlockData(FAMISIndividualsInformation.GG)
        getBlockData(FAMISIndividualsInformation.GH)
        getBlockData(FAMISIndividualsInformation.GI)
        getBlockData(FAMISIndividualsInformation.GJ)
        getBlockData(FAMISIndividualsInformation.GK)
        getBlockData(FAMISIndividualsInformation.GL)

        getBlockData(FAMISMedicaidInformation.HA)
        getBlockData(FAMISMedicaidInformation.HB)
        getBlockData(FAMISMedicaidInformation.HC)
        getBlockData(FAMISMedicaidInformation.HD)
        getBlockData(FAMISMedicaidInformation.HE)
        getBlockData(FAMISMedicaidInformation.HF)
        getBlockData(FAMISMedicaidInformation.HG)
        getBlockData(FAMISMedicaidInformation.HH)
        getBlockData(FAMISMedicaidInformation.HI)
        getBlockData(FAMISMedicaidInformation.HJ)
        getBlockData(FAMISMedicaidInformation.HK)
        getBlockData(FAMISMedicaidInformation.HL)
        getBlockData(FAMISMedicaidInformation.HM)
        getBlockData(FAMISMedicaidInformation.HN)
        getBlockData(FAMISMedicaidInformation.HO)
        getBlockData(FAMISMedicaidInformation.HP)
        getBlockData(FAMISMedicaidInformation.HQ)
        getBlockData(FAMISMedicaidInformation.HR)
        getBlockData(FAMISMedicaidInformation.HS)
        getBlockData(FAMISMedicaidInformation.HT)

        getBlockData(FAMISMedicaidInformation.WA)
        getBlockData(FAMISMedicaidInformation.WB)
        getBlockData(FAMISMedicaidInformation.WC)
        'If Not isValidDate(FAMISMedicaidInformation.WC.GetData) Then FAMISMedicaidInformation.WC.SetData("        ")
        getBlockData(FAMISMedicaidInformation.WD)
        'If Not isValidDate(FAMISMedicaidInformation.WD.GetData) Then FAMISMedicaidInformation.WD.SetData("        ")
        getBlockData(FAMISMedicaidInformation.WE)
        getBlockData(FAMISMedicaidInformation.WF)
        'If Not isValidDate(FAMISMedicaidInformation.WF.GetData) Then FAMISMedicaidInformation.WF.SetData("        ")
        getBlockData(FAMISMedicaidInformation.WG)
        'If Not isValidDate(FAMISMedicaidInformation.WG.GetData) Then FAMISMedicaidInformation.WG.SetData("        ")
        getBlockData(FAMISMedicaidInformation.WH)
        getBlockData(FAMISMedicaidInformation.WI)
        getBlockData(FAMISMedicaidInformation.WK)
        getBlockData(FAMISMedicaidInformation.WL)
        getBlockData(FAMISMedicaidInformation.WM)
        getBlockData(FAMISMedicaidInformation.WN)
        getBlockData(FAMISMedicaidInformation.WO)
        getBlockData(FAMISMedicaidInformation.WP)
        getBlockData(FAMISMedicaidInformation.WQ)
        getBlockData(FAMISMedicaidInformation.WR)
        getBlockData(FAMISMedicaidInformation.WS)
        getBlockData(FAMISMedicaidInformation.WT)
        getBlockData(FAMISMedicaidInformation.WU)
        getBlockData(FAMISMedicaidInformation.WV)
        getBlockData(FAMISMedicaidInformation.WW)

        'getBlockData(FAMISMedicaidInformation.HU)
        FAMISMedicaidInformation.HU.SetData("")
        'getBlockData(FAMISMedicaidInformation.HV)
        FAMISMedicaidInformation.HV.SetData("")
        getBlockData(FAMISMedicaidInformation.XO)
        getBlockData(FAMISMedicaidInformation.XP)
        getBlockData(FAMISMedicaidInformation.XQ)
        getBlockData(FAMISMedicaidInformation.XR)
        getBlockData(FAMISMedicaidInformation.XS)
        getBlockData(FAMISMedicaidInformation.XT)
        getBlockData(FAMISMedicaidInformation.XU)
        getBlockData(FAMISMedicaidInformation.XV)

        getBlockData(FAMISIncomeInformation.JA)
        getBlockData(FAMISIncomeInformation.JB)
        getBlockData(FAMISIncomeInformation.JC)
        getBlockData(FAMISIncomeInformation.JD)
        getBlockData(FAMISIncomeInformation.JE)
        getBlockData(FAMISIncomeInformation.JF)
        getBlockData(FAMISIncomeInformation.JG)
        getBlockData(FAMISIncomeInformation.JH)
        getBlockData(FAMISIncomeInformation.JI)
        getBlockData(FAMISIncomeInformation.JJ)
        getBlockData(FAMISIncomeInformation.JK)
        getBlockData(FAMISIncomeInformation.JL)
        getBlockData(FAMISIncomeInformation.JM)
        getBlockData(FAMISIncomeInformation.JN)
        getBlockData(FAMISIncomeInformation.JO)
        getBlockData(FAMISIncomeInformation.JP)
        getBlockData(FAMISIncomeInformation.JQ)
        getBlockData(FAMISIncomeInformation.JR)
        getBlockData(FAMISIncomeInformation.JS)
        getBlockData(FAMISIncomeInformation.JT)
        getBlockData(FAMISIncomeInformation.JU)
        '  getBlockData(FAMISIncomeInformation.JV)
        getBlockData(FAMISIncomeInformation.JW)
        getBlockData(FAMISIncomeInformation.JX)

        getBlockData(FAMISIncomeInformation.KA)
        getBlockData(FAMISIncomeInformation.KB)
        getBlockData(FAMISIncomeInformation.KC)
        getBlockData(FAMISIncomeInformation.KD)
        getBlockData(FAMISIncomeInformation.KE)
        getBlockData(FAMISIncomeInformation.KF)
        getBlockData(FAMISIncomeInformation.KG)
        getBlockData(FAMISIncomeInformation.KH)
        getBlockData(FAMISIncomeInformation.KI)
        getBlockData(FAMISIncomeInformation.KJ)
        getBlockData(FAMISIncomeInformation.KK)
        getBlockData(FAMISIncomeInformation.KL)
        getBlockData(FAMISIncomeInformation.KM)
        getBlockData(FAMISIncomeInformation.KN)
        getBlockData(FAMISIncomeInformation.KO)
        getBlockData(FAMISIncomeInformation.KP)
        getBlockData(FAMISIncomeInformation.KQ)
        getBlockData(FAMISIncomeInformation.KR)
        getBlockData(FAMISIncomeInformation.KS)
        'getBlockData(FAMISIncomeInformation.KT)
        getBlockData(FAMISIncomeInformation.KU)
        getBlockData(FAMISIncomeInformation.KV)

        getBlockData(FAMISFoodStampInformation.LA)
        getBlockData(FAMISFoodStampInformation.LB)
        getBlockData(FAMISFoodStampInformation.LC)
        getBlockData(FAMISFoodStampInformation.LD)
        getBlockData(FAMISFoodStampInformation.LE)
        getBlockData(FAMISFoodStampInformation.LF)
        getBlockData(FAMISFoodStampInformation.LG)
        getBlockData(FAMISFoodStampInformation.LH)
        getBlockData(FAMISFoodStampInformation.LI)
        getBlockData(FAMISFoodStampInformation.LJ)
        getBlockData(FAMISFoodStampInformation.LK)
        getBlockData(FAMISFoodStampInformation.LL)
        getBlockData(FAMISFoodStampInformation.LM)
        getBlockData(FAMISFoodStampInformation.LN)
        getBlockData(FAMISFoodStampInformation.LO)
        getBlockData(FAMISFoodStampInformation.LP)
        getBlockData(FAMISFoodStampInformation.LQ)
        getBlockData(FAMISFoodStampInformation.LR)
        getBlockData(FAMISFoodStampInformation.LS)
        getBlockData(FAMISFoodStampInformation.LT)

        getBlockData(FAMISFoodStampInformation.MA)
        getBlockData(FAMISFoodStampInformation.MB)
        getBlockData(FAMISFoodStampInformation.MC)
        getBlockData(FAMISFoodStampInformation.MD)
        getBlockData(FAMISFoodStampInformation.ME1)
        getBlockData(FAMISFoodStampInformation.MF)
        getBlockData(FAMISFoodStampInformation.MG)
        getBlockData(FAMISFoodStampInformation.MH)
        getBlockData(FAMISFoodStampInformation.MI)
        getBlockData(FAMISFoodStampInformation.MJ)
        getBlockData(FAMISFoodStampInformation.MK)
        getBlockData(FAMISFoodStampInformation.ML)
        getBlockData(FAMISFoodStampInformation.MM)
        getBlockData(FAMISFoodStampInformation.MN)
        getBlockData(FAMISFoodStampInformation.MO)
        getBlockData(FAMISFoodStampInformation.MP)
        getBlockData(FAMISFoodStampInformation.MQ)
        getBlockData(FAMISFoodStampInformation.MR)
        getBlockData(FAMISFoodStampInformation.MS)
        getBlockData(FAMISFoodStampInformation.MT)

        getBlockData(FAMISFoodStampInformation.NA)
        getBlockData(FAMISFoodStampInformation.NB)
        getBlockData(FAMISFoodStampInformation.NC)
        FAMISFoodStampInformation.NC.SetData("        ")
        getBlockData(FAMISFoodStampInformation.ND)
        getBlockData(FAMISFoodStampInformation.NE)
        getBlockData(FAMISFoodStampInformation.NF)
        getBlockData(FAMISFoodStampInformation.NG)
        getBlockData(FAMISFoodStampInformation.NH)
        getBlockData(FAMISFoodStampInformation.NI)
        getBlockData(FAMISFoodStampInformation.NJ)
        getBlockData(FAMISFoodStampInformation.NK)
        getBlockData(FAMISFoodStampInformation.NL)
        getBlockData(FAMISFoodStampInformation.NM)
        getBlockData(FAMISFoodStampInformation.NN)
        getBlockData(FAMISFoodStampInformation.NO)
        getBlockData(FAMISFoodStampInformation.NP)

        getBlockData(FAMISFoodStampInformation.OA)
        getBlockData(FAMISFoodStampInformation.OB)
        getBlockData(FAMISFoodStampInformation.OC)
        getBlockData(FAMISFoodStampInformation.OD)
        getBlockData(FAMISFoodStampInformation.OE)
        getBlockData(FAMISFoodStampInformation.OF1)
        getBlockData(FAMISFoodStampInformation.OG)
        getBlockData(FAMISFoodStampInformation.OH)
        getBlockData(FAMISFoodStampInformation.OI)
        getBlockData(FAMISFoodStampInformation.OJ)
        getBlockData(FAMISFoodStampInformation.OK)
        getBlockData(FAMISFoodStampInformation.OL)
        'getBlockData(FAMISFoodStampInformation.OM)
        FAMISFoodStampInformation.OM.SetData("")
        getBlockData(FAMISFoodStampInformation.ON1)
        getBlockData(FAMISFoodStampInformation.OO)
        getBlockData(FAMISFoodStampInformation.OP)

        getBlockData(FAMISFoodStampInformation.WX)
        getBlockData(FAMISFoodStampInformation.WY)

        getBlockData(FAMISFoodStampInformation.MU)
        getBlockData(FAMISFoodStampInformation.NQ)
        getBlockData(FAMISFoodStampInformation.NR)
        getBlockData(FAMISFoodStampInformation.NS)
        getBlockData(FAMISFoodStampInformation.NT)
        getBlockData(FAMISFoodStampInformation.NU)
        getBlockData(FAMISFoodStampInformation.NV)
        getBlockData(FAMISFoodStampInformation.NW)
        getBlockData(FAMISFoodStampInformation.OR)
        getBlockData(FAMISFoodStampInformation.VB)
        getBlockData(FAMISFoodStampInformation.VF)
        getBlockData(FAMISFoodStampInformation.VH)
        getBlockData(FAMISFoodStampInformation.OQ1)
        getBlockData(FAMISFoodStampInformation.OQ2)
        If FAMISFoodStampInformation.OQ1.GetData.Substring(0, 1) = " " And FAMISFoodStampInformation.OQ2.GetData.Substring(0, 1) <> " " Then FAMISFoodStampInformation.OQ1.SetData("973")

        getBlockData(FAMISIandAInformation.PA)
        getBlockData(FAMISIandAInformation.PB)
        getBlockData(FAMISIandAInformation.PC)
        getBlockData(FAMISIandAInformation.PD)
        getBlockData(FAMISIandAInformation.PE)
        getBlockData(FAMISIandAInformation.PF)
        getBlockData(FAMISIandAInformation.PG)
        getBlockData(FAMISIandAInformation.PH)
        getBlockData(FAMISIandAInformation.PI)
        getBlockData(FAMISIandAInformation.PJ)
        getBlockData(FAMISIandAInformation.PK)
        getBlockData(FAMISIandAInformation.PL)
        getBlockData(FAMISIandAInformation.PM)
        getBlockData(FAMISIandAInformation.PN)
        getBlockData(FAMISIandAInformation.PO)
        getBlockData(FAMISIandAInformation.PP)

        getBlockData(FAMISTANFInformation.IA)
        getBlockData(FAMISTANFInformation.IB)
        getBlockData(FAMISTANFInformation.IC)
        getBlockData(FAMISTANFInformation.ID)
        getBlockData(FAMISTANFInformation.IE)
        getBlockData(FAMISTANFInformation.IF1)
        getBlockData(FAMISTANFInformation.IG)
        getBlockData(FAMISTANFInformation.IH)
        getBlockData(FAMISTANFInformation.II)
        getBlockData(FAMISTANFInformation.IJ)
        getBlockData(FAMISTANFInformation.IK)
        getBlockData(FAMISTANFInformation.IL)
        getBlockData(FAMISTANFInformation.IM)
        getBlockData(FAMISTANFInformation.IN1)
        getBlockData(FAMISTANFInformation.IO)
        getBlockData(FAMISTANFInformation.IP)

        '--SQL Data needs to have all information--
        SQLCaseInformation = New CaseInformation
        SQLMedicaidInformation = New MedicaidInformation
        SQLFoodStampInformation = New FoodStampInformation
        SQLApplicationInformation = New ApplicationInformation
        SQLTANFInformation = New TANFInformation
        SQLIandAInformation = New IandAInformation
        SQLIndividualsInformation = New IndividualsInformation
        SQLIncomeInformation = New IncomeInformation

        getBlockData(SQLCaseInformation.AA)
        getBlockData(SQLCaseInformation.AB)
        getBlockData(SQLCaseInformation.AC)
        getBlockData(SQLCaseInformation.AD)
        getBlockData(SQLCaseInformation.AE)
        getBlockData(SQLCaseInformation.AF)
        getBlockData(SQLCaseInformation.AG)
        getBlockData(SQLCaseInformation.AH)
        getBlockData(SQLCaseInformation.AI)
        getBlockData(SQLCaseInformation.AJ)
        getBlockData(SQLCaseInformation.AK)
        getBlockData(SQLCaseInformation.AL)
        getBlockData(SQLCaseInformation.AM)
        getBlockData(SQLCaseInformation.AN)
        getBlockData(SQLCaseInformation.AQ)

        getBlockData(SQLApplicationInformation.BA)
        getBlockData(SQLApplicationInformation.BB)
        getBlockData(SQLApplicationInformation.BC)
        getBlockData(SQLApplicationInformation.BD)
        getBlockData(SQLApplicationInformation.BE)
        getBlockData(SQLApplicationInformation.BF)
        getBlockData(SQLApplicationInformation.BG)
        getBlockData(SQLApplicationInformation.BH)
        getBlockData(SQLApplicationInformation.BI)
        getBlockData(SQLApplicationInformation.BJ)
        getBlockData(SQLApplicationInformation.BK)
        getBlockData(SQLApplicationInformation.BL)
        getBlockData(SQLApplicationInformation.BM)
        getBlockData(SQLApplicationInformation.BN)
        getBlockData(SQLApplicationInformation.BO)
        getBlockData(SQLApplicationInformation.BP)
        getBlockData(SQLApplicationInformation.BQ)
        getBlockData(SQLApplicationInformation.BR)
        getBlockData(SQLApplicationInformation.BS)
        getBlockData(SQLApplicationInformation.BT)
        getBlockData(SQLApplicationInformation.BU)
        getBlockData(SQLApplicationInformation.BV)
        getBlockData(SQLApplicationInformation.BW)
        getBlockData(SQLApplicationInformation.BX)
        getBlockData(SQLApplicationInformation.BY1)
        getBlockData(SQLApplicationInformation.BZ)

        getBlockData(SQLApplicationInformation.CA)
        'getBlockData(SQLApplicationInformation.CB)
        If SQLApplicationInformation.CA.GetData.Substring(0, 1) = "-" Then SQLApplicationInformation.CB.SetData("-")
        getBlockData(SQLApplicationInformation.CC)
        getBlockData(SQLApplicationInformation.CD1)
        getBlockData(SQLApplicationInformation.CD2)
        getBlockData(SQLApplicationInformation.CE)
        getBlockData(SQLApplicationInformation.CF)
        getBlockData(SQLApplicationInformation.CG)

        getBlockData(SQLApplicationInformation.DA1)
        getBlockData(SQLApplicationInformation.DA2)
        getBlockData(SQLApplicationInformation.DA3)
        getBlockData(SQLApplicationInformation.DB)
        getBlockData(SQLApplicationInformation.DC)
        getBlockData(SQLApplicationInformation.DD1)
        getBlockData(SQLApplicationInformation.DD2)
        getBlockData(SQLApplicationInformation.DE)
        getBlockData(SQLApplicationInformation.DF)

        getBlockData(SQLApplicationInformation.EA)
        getBlockData(SQLApplicationInformation.EB)
        getBlockData(SQLApplicationInformation.EC)
        getBlockData(SQLApplicationInformation.ED1)
        getBlockData(SQLApplicationInformation.ED2)
        getBlockData(SQLApplicationInformation.EE)
        getBlockData(SQLApplicationInformation.EF)
        getBlockData(SQLApplicationInformation.EG)
        getBlockData(SQLApplicationInformation.EH)
        getBlockData(SQLApplicationInformation.EI)
        getBlockData(SQLApplicationInformation.EJ)
        getBlockData(SQLApplicationInformation.EK)
        getBlockData(SQLApplicationInformation.EL)
        getBlockData(SQLApplicationInformation.EM)
        getBlockData(SQLApplicationInformation.EN)

        getBlockData(SQLApplicationInformation.XA)
        getBlockData(SQLApplicationInformation.XB)
        getBlockData(SQLApplicationInformation.XC)
        getBlockData(SQLApplicationInformation.XD)
        getBlockData(SQLApplicationInformation.XE)
        getBlockData(SQLApplicationInformation.XF)
        getBlockData(SQLApplicationInformation.XG)
        getBlockData(SQLApplicationInformation.XH)
        getBlockData(SQLApplicationInformation.XI)
        getBlockData(SQLApplicationInformation.XJ)
        getBlockData(SQLApplicationInformation.XK)
        getBlockData(SQLApplicationInformation.XL)
        getBlockData(SQLApplicationInformation.XM)
        getBlockData(SQLApplicationInformation.XN)

        getBlockData(SQLIndividualsInformation.FA)
        getBlockData(SQLIndividualsInformation.FB)
        getBlockData(SQLIndividualsInformation.FC)
        getBlockData(SQLIndividualsInformation.FD)
        getBlockData(SQLIndividualsInformation.FD2)
        getBlockData(SQLIndividualsInformation.FE1)
        getBlockData(SQLIndividualsInformation.FE2)
        getBlockData(SQLIndividualsInformation.FF)
        getBlockData(SQLIndividualsInformation.FG)
        getBlockData(SQLIndividualsInformation.FG2)
        getBlockData(SQLIndividualsInformation.FG3)
        SQLIndividualsInformation.FG.SetData(SQLIndividualsInformation.FG.GetData & SQLIndividualsInformation.FG2.GetData & SQLIndividualsInformation.FG3.GetData)
        SQLIndividualsInformation.FG.Length = SQLIndividualsInformation.FG.Length + SQLIndividualsInformation.FG2.Length + SQLIndividualsInformation.FG3.Length
        getBlockData(SQLIndividualsInformation.FH)
        getBlockData(SQLIndividualsInformation.FI)
        getBlockData(SQLIndividualsInformation.FK)
        getBlockData(SQLIndividualsInformation.FJ)
        getBlockData(SQLIndividualsInformation.FL)
        getBlockData(SQLIndividualsInformation.FL2)
        getBlockData(SQLIndividualsInformation.FM1)
        getBlockData(SQLIndividualsInformation.FM2)
        getBlockData(SQLIndividualsInformation.FN)
        getBlockData(SQLIndividualsInformation.FO)
        getBlockData(SQLIndividualsInformation.FP)
        getBlockData(SQLIndividualsInformation.FP2)
        getBlockData(SQLIndividualsInformation.FP3)
        SQLIndividualsInformation.FP.SetData(SQLIndividualsInformation.FP.GetData & SQLIndividualsInformation.FP2.GetData & SQLIndividualsInformation.FP3.GetData)
        SQLIndividualsInformation.FP.Length = SQLIndividualsInformation.FP.Length + SQLIndividualsInformation.FP2.Length + SQLIndividualsInformation.FP3.Length

        getBlockData(SQLIndividualsInformation.B1)
        getBlockData(SQLIndividualsInformation.B2)
        getBlockData(SQLIndividualsInformation.B3)
        getBlockData(SQLIndividualsInformation.B4)
        If TEXT_IndividualsInformation.Length > 442 Then
            getBlockData(FAMISIndividualsInformation.B5)
            getBlockData(FAMISIndividualsInformation.B6)
        Else
            FAMISIndividualsInformation.B5.SetData("        ")
            FAMISIndividualsInformation.B6.SetData("        ")
        End If
        getBlockData(SQLIndividualsInformation.FQ)
        getBlockData(SQLIndividualsInformation.FR)

        getBlockData(SQLIndividualsInformation.GA)
        getBlockData(SQLIndividualsInformation.GB)
        getBlockData(SQLIndividualsInformation.GC)
        getBlockData(SQLIndividualsInformation.GD)
        getBlockData(SQLIndividualsInformation.GE)
        getBlockData(SQLIndividualsInformation.GF)
        getBlockData(SQLIndividualsInformation.GG)
        getBlockData(SQLIndividualsInformation.GH)
        getBlockData(SQLIndividualsInformation.GI)
        getBlockData(SQLIndividualsInformation.GJ)
        getBlockData(SQLIndividualsInformation.GK)
        getBlockData(SQLIndividualsInformation.GL)

        getBlockData(SQLMedicaidInformation.HA)
        getBlockData(SQLMedicaidInformation.HB)
        getBlockData(SQLMedicaidInformation.HC)
        getBlockData(SQLMedicaidInformation.HD)
        getBlockData(SQLMedicaidInformation.HE)
        getBlockData(SQLMedicaidInformation.HF)
        getBlockData(SQLMedicaidInformation.HG)
        getBlockData(SQLMedicaidInformation.HH)
        getBlockData(SQLMedicaidInformation.HI)
        getBlockData(SQLMedicaidInformation.HJ)
        getBlockData(SQLMedicaidInformation.HK)
        getBlockData(SQLMedicaidInformation.HL)
        getBlockData(SQLMedicaidInformation.HM)
        getBlockData(SQLMedicaidInformation.HN)
        getBlockData(SQLMedicaidInformation.HO)
        getBlockData(SQLMedicaidInformation.HP)
        getBlockData(SQLMedicaidInformation.HQ)
        getBlockData(SQLMedicaidInformation.HR)
        getBlockData(SQLMedicaidInformation.HS)
        getBlockData(SQLMedicaidInformation.HT)

        getBlockData(SQLMedicaidInformation.WA)
        getBlockData(SQLMedicaidInformation.WB)
        getBlockData(SQLMedicaidInformation.WC)
        'If Not isValidDate(SQLMedicaidInformation.WC.GetData) Then SQLMedicaidInformation.WC.SetData("        ")
        getBlockData(SQLMedicaidInformation.WD)
        'If Not isValidDate(SQLMedicaidInformation.WD.GetData) Then SQLMedicaidInformation.WC.SetData("        ")
        getBlockData(SQLMedicaidInformation.WE)
        getBlockData(SQLMedicaidInformation.WF)
        'If Not isValidDate(SQLMedicaidInformation.WF.GetData) Then SQLMedicaidInformation.WF.SetData("        ")
        getBlockData(SQLMedicaidInformation.WG)
        'If Not isValidDate(SQLMedicaidInformation.WG.GetData) Then SQLMedicaidInformation.WG.SetData("        ")
        getBlockData(SQLMedicaidInformation.WH)
        getBlockData(SQLMedicaidInformation.WI)
        getBlockData(SQLMedicaidInformation.WK)
        getBlockData(SQLMedicaidInformation.WL)
        getBlockData(SQLMedicaidInformation.WM)
        getBlockData(SQLMedicaidInformation.WN)
        getBlockData(SQLMedicaidInformation.WO)
        getBlockData(SQLMedicaidInformation.WP)
        getBlockData(SQLMedicaidInformation.WQ)
        getBlockData(SQLMedicaidInformation.WR)
        getBlockData(SQLMedicaidInformation.WS)
        getBlockData(SQLMedicaidInformation.WT)
        getBlockData(SQLMedicaidInformation.WU)
        getBlockData(SQLMedicaidInformation.WV)
        getBlockData(SQLMedicaidInformation.WW)

        'getBlockData(SQLMedicaidInformation.HU)
        SQLMedicaidInformation.HU.SetData("")
        'getBlockData(SQLMedicaidInformation.HV)
        SQLMedicaidInformation.HV.SetData("")
        getBlockData(SQLMedicaidInformation.XO)
        getBlockData(SQLMedicaidInformation.XP)
        getBlockData(SQLMedicaidInformation.XQ)
        getBlockData(SQLMedicaidInformation.XR)
        getBlockData(SQLMedicaidInformation.XS)
        getBlockData(SQLMedicaidInformation.XT)
        getBlockData(SQLMedicaidInformation.XU)
        getBlockData(SQLMedicaidInformation.XV)

        getBlockData(SQLIncomeInformation.JA)
        getBlockData(SQLIncomeInformation.JB)
        getBlockData(SQLIncomeInformation.JC)
        getBlockData(SQLIncomeInformation.JD)
        getBlockData(SQLIncomeInformation.JE)
        getBlockData(SQLIncomeInformation.JF)
        getBlockData(SQLIncomeInformation.JG)
        getBlockData(SQLIncomeInformation.JH)
        getBlockData(SQLIncomeInformation.JI)
        getBlockData(SQLIncomeInformation.JJ)
        getBlockData(SQLIncomeInformation.JK)
        getBlockData(SQLIncomeInformation.JL)
        getBlockData(SQLIncomeInformation.JM)
        getBlockData(SQLIncomeInformation.JN)
        getBlockData(SQLIncomeInformation.JO)
        getBlockData(SQLIncomeInformation.JP)
        getBlockData(SQLIncomeInformation.JQ)
        getBlockData(SQLIncomeInformation.JR)
        getBlockData(SQLIncomeInformation.JS)
        getBlockData(SQLIncomeInformation.JT)
        getBlockData(SQLIncomeInformation.JU)
        '  getBlockData(SQLIncomeInformation.JV)
        getBlockData(SQLIncomeInformation.JW)
        getBlockData(SQLIncomeInformation.JX)

        getBlockData(SQLIncomeInformation.KA)
        getBlockData(SQLIncomeInformation.KB)
        getBlockData(SQLIncomeInformation.KC)
        getBlockData(SQLIncomeInformation.KD)
        getBlockData(SQLIncomeInformation.KE)
        getBlockData(SQLIncomeInformation.KF)
        getBlockData(SQLIncomeInformation.KG)
        getBlockData(SQLIncomeInformation.KH)
        getBlockData(SQLIncomeInformation.KI)
        getBlockData(SQLIncomeInformation.KJ)
        getBlockData(SQLIncomeInformation.KK)
        getBlockData(SQLIncomeInformation.KL)
        getBlockData(SQLIncomeInformation.KM)
        getBlockData(SQLIncomeInformation.KN)
        getBlockData(SQLIncomeInformation.KO)
        getBlockData(SQLIncomeInformation.KP)
        getBlockData(SQLIncomeInformation.KQ)
        getBlockData(SQLIncomeInformation.KR)
        getBlockData(SQLIncomeInformation.KS)
        'getBlockData(SQLIncomeInformation.KT)
        getBlockData(SQLIncomeInformation.KU)
        getBlockData(SQLIncomeInformation.KV)

        getBlockData(SQLFoodStampInformation.LA)
        getBlockData(SQLFoodStampInformation.LB)
        getBlockData(SQLFoodStampInformation.LC)
        getBlockData(SQLFoodStampInformation.LD)
        getBlockData(SQLFoodStampInformation.LE)
        getBlockData(SQLFoodStampInformation.LF)
        getBlockData(SQLFoodStampInformation.LG)
        getBlockData(SQLFoodStampInformation.LH)
        getBlockData(SQLFoodStampInformation.LI)
        getBlockData(SQLFoodStampInformation.LJ)
        getBlockData(SQLFoodStampInformation.LK)
        getBlockData(SQLFoodStampInformation.LL)
        getBlockData(SQLFoodStampInformation.LM)
        getBlockData(SQLFoodStampInformation.LN)
        getBlockData(SQLFoodStampInformation.LO)
        getBlockData(SQLFoodStampInformation.LP)
        getBlockData(SQLFoodStampInformation.LQ)
        getBlockData(SQLFoodStampInformation.LR)
        getBlockData(SQLFoodStampInformation.LS)
        getBlockData(SQLFoodStampInformation.LT)

        getBlockData(SQLFoodStampInformation.MA)
        getBlockData(SQLFoodStampInformation.MB)
        getBlockData(SQLFoodStampInformation.MC)
        getBlockData(SQLFoodStampInformation.MD)
        getBlockData(SQLFoodStampInformation.ME1)
        getBlockData(SQLFoodStampInformation.MF)
        getBlockData(SQLFoodStampInformation.MG)
        getBlockData(SQLFoodStampInformation.MH)
        getBlockData(SQLFoodStampInformation.MI)
        getBlockData(SQLFoodStampInformation.MJ)
        getBlockData(SQLFoodStampInformation.MK)
        getBlockData(SQLFoodStampInformation.ML)
        getBlockData(SQLFoodStampInformation.MM)
        getBlockData(SQLFoodStampInformation.MN)
        getBlockData(SQLFoodStampInformation.MO)
        getBlockData(SQLFoodStampInformation.MP)
        getBlockData(SQLFoodStampInformation.MQ)
        getBlockData(SQLFoodStampInformation.MR)
        getBlockData(SQLFoodStampInformation.MS)
        getBlockData(SQLFoodStampInformation.MT)

        getBlockData(SQLFoodStampInformation.NA)
        getBlockData(SQLFoodStampInformation.NB)
        getBlockData(SQLFoodStampInformation.NC)
        SQLFoodStampInformation.NC.SetData("        ")
        getBlockData(SQLFoodStampInformation.ND)
        getBlockData(SQLFoodStampInformation.NE)
        getBlockData(SQLFoodStampInformation.NF)
        getBlockData(SQLFoodStampInformation.NG)
        getBlockData(SQLFoodStampInformation.NH)
        getBlockData(SQLFoodStampInformation.NI)
        getBlockData(SQLFoodStampInformation.NJ)
        getBlockData(SQLFoodStampInformation.NK)
        getBlockData(SQLFoodStampInformation.NL)
        getBlockData(SQLFoodStampInformation.NM)
        getBlockData(SQLFoodStampInformation.NN)
        getBlockData(SQLFoodStampInformation.NO)
        getBlockData(SQLFoodStampInformation.NP)

        getBlockData(SQLFoodStampInformation.OA)
        getBlockData(SQLFoodStampInformation.OB)
        getBlockData(SQLFoodStampInformation.OC)
        getBlockData(SQLFoodStampInformation.OD)
        getBlockData(SQLFoodStampInformation.OE)
        getBlockData(SQLFoodStampInformation.OF1)
        getBlockData(SQLFoodStampInformation.OG)
        getBlockData(SQLFoodStampInformation.OH)
        getBlockData(SQLFoodStampInformation.OI)
        getBlockData(SQLFoodStampInformation.OJ)
        getBlockData(SQLFoodStampInformation.OK)
        getBlockData(SQLFoodStampInformation.OL)
        'getBlockData(SQLFoodStampInformation.OM)
        SQLFoodStampInformation.OM.SetData("")
        getBlockData(SQLFoodStampInformation.ON1)
        getBlockData(SQLFoodStampInformation.OO)
        getBlockData(SQLFoodStampInformation.OP)

        getBlockData(SQLFoodStampInformation.WX)
        getBlockData(SQLFoodStampInformation.WY)

        getBlockData(SQLFoodStampInformation.MU)
        getBlockData(SQLFoodStampInformation.NQ)
        getBlockData(SQLFoodStampInformation.NR)
        getBlockData(SQLFoodStampInformation.NS)
        getBlockData(SQLFoodStampInformation.NT)
        getBlockData(SQLFoodStampInformation.NU)
        getBlockData(SQLFoodStampInformation.NV)
        getBlockData(SQLFoodStampInformation.NW)
        getBlockData(SQLFoodStampInformation.OR)
        getBlockData(SQLFoodStampInformation.VB)
        getBlockData(SQLFoodStampInformation.VF)
        getBlockData(SQLFoodStampInformation.VH)
        getBlockData(SQLFoodStampInformation.OQ1)
        getBlockData(SQLFoodStampInformation.OQ2)

        getBlockData(SQLIandAInformation.PA)
        getBlockData(SQLIandAInformation.PB)
        getBlockData(SQLIandAInformation.PC)
        getBlockData(SQLIandAInformation.PD)
        getBlockData(SQLIandAInformation.PE)
        getBlockData(SQLIandAInformation.PF)
        getBlockData(SQLIandAInformation.PG)
        getBlockData(SQLIandAInformation.PH)
        getBlockData(SQLIandAInformation.PI)
        getBlockData(SQLIandAInformation.PJ)
        getBlockData(SQLIandAInformation.PK)
        getBlockData(SQLIandAInformation.PL)
        getBlockData(SQLIandAInformation.PM)
        getBlockData(SQLIandAInformation.PN)
        getBlockData(SQLIandAInformation.PO)
        getBlockData(SQLIandAInformation.PP)

        getBlockData(SQLTANFInformation.IA)
        getBlockData(SQLTANFInformation.IB)
        getBlockData(SQLTANFInformation.IC)
        getBlockData(SQLTANFInformation.ID)
        getBlockData(SQLTANFInformation.IE)
        getBlockData(SQLTANFInformation.IF1)
        getBlockData(SQLTANFInformation.IG)
        getBlockData(SQLTANFInformation.IH)
        getBlockData(SQLTANFInformation.II)
        getBlockData(SQLTANFInformation.IJ)
        getBlockData(SQLTANFInformation.IK)
        getBlockData(SQLTANFInformation.IL)
        getBlockData(SQLTANFInformation.IM)
        getBlockData(SQLTANFInformation.IN1)
        getBlockData(SQLTANFInformation.IO)
        getBlockData(SQLTANFInformation.IP)
    End Sub
    Private Sub setBlockData_Child()
        Dim i As Integer
        For i = 0 To numChildren - 1
            FAMISCaseChild(i) = New CaseChild

            getBlockData(FAMISCaseChild(i).QA, i)
            getBlockData(FAMISCaseChild(i).QB, i)
            getBlockData(FAMISCaseChild(i).QC, i)
            getBlockData(FAMISCaseChild(i).QD, i)
            getBlockData(FAMISCaseChild(i).QE, i)
            'getBlockData(FAMISCaseChild(i).QE2, i)
            'FAMISCaseChild(i).QE.SetData(FAMISCaseChild(i).QE.GetData & FAMISCaseChild(i).QE2.GetData)
            'FAMISCaseChild(i).QE.Length = FAMISCaseChild(i).QE.Length + FAMISCaseChild(i).QE2.Length

            getBlockData(FAMISCaseChild(i).QF, i)
            getBlockData(FAMISCaseChild(i).QG, i)
            getBlockData(FAMISCaseChild(i).QH, i)
            getBlockData(FAMISCaseChild(i).QI, i)
            getBlockData(FAMISCaseChild(i).QI2, i)
            FAMISCaseChild(i).QI.SetData(FAMISCaseChild(i).QI.GetData & FAMISCaseChild(i).QI2.GetData)
            FAMISCaseChild(i).QI.Length = FAMISCaseChild(i).QI.Length + FAMISCaseChild(i).QI2.Length
            getBlockData(FAMISCaseChild(i).QK, i)
            getBlockData(FAMISCaseChild(i).QJ, i)
            getBlockData(FAMISCaseChild(i).QL, i)
            getBlockData(FAMISCaseChild(i).QM, i)
            getBlockData(FAMISCaseChild(i).QN, i)
            getBlockData(FAMISCaseChild(i).QO, i)
            FAMISCaseChild(i).QO.SetData("  ")

            getBlockData(FAMISCaseChild(i).RA, i)
            getBlockData(FAMISCaseChild(i).RB, i)
            getBlockData(FAMISCaseChild(i).RC, i)
            getBlockData(FAMISCaseChild(i).RD, i)
            getBlockData(FAMISCaseChild(i).RE, i)
            getBlockData(FAMISCaseChild(i).RF, i)
            getBlockData(FAMISCaseChild(i).RG, i)
            getBlockData(FAMISCaseChild(i).RH, i)
            getBlockData(FAMISCaseChild(i).RI, i)
            getBlockData(FAMISCaseChild(i).RJ1, i)
            getBlockData(FAMISCaseChild(i).RJ2, i)
            getBlockData(FAMISCaseChild(i).RK, i)
            getBlockData(FAMISCaseChild(i).RL, i)
            getBlockData(FAMISCaseChild(i).RM, i)
            getBlockData(FAMISCaseChild(i).RN, i)
            getBlockData(FAMISCaseChild(i).RO, i)
            getBlockData(FAMISCaseChild(i).RP, i)
            getBlockData(FAMISCaseChild(i).RQ, i)
            getBlockData(FAMISCaseChild(i).RR, i)

            getBlockData(FAMISCaseChild(i).SA, i)
            getBlockData(FAMISCaseChild(i).SB, i)
            getBlockData(FAMISCaseChild(i).SC, i)
            getBlockData(FAMISCaseChild(i).SD, i)
            getBlockData(FAMISCaseChild(i).SE, i)
            getBlockData(FAMISCaseChild(i).SF, i)
            getBlockData(FAMISCaseChild(i).SG, i)
            getBlockData(FAMISCaseChild(i).SH, i)
            getBlockData(FAMISCaseChild(i).SI, i)
            getBlockData(FAMISCaseChild(i).SJ, i)
            getBlockData(FAMISCaseChild(i).SK, i)
            getBlockData(FAMISCaseChild(i).SL, i)
            getBlockData(FAMISCaseChild(i).SM, i)
            getBlockData(FAMISCaseChild(i).SN, i)
            getBlockData(FAMISCaseChild(i).SO, i)
            getBlockData(FAMISCaseChild(i).SP, i)
            getBlockData(FAMISCaseChild(i).SQ, i)
            getBlockData(FAMISCaseChild(i).SR, i)
            getBlockData(FAMISCaseChild(i).SS, i)
            getBlockData(FAMISCaseChild(i).ST, i)

            getBlockData(FAMISCaseChild(i).TA, i)
            getBlockData(FAMISCaseChild(i).TB, i)
            getBlockData(FAMISCaseChild(i).TC, i)
            getBlockData(FAMISCaseChild(i).TD, i)
            getBlockData(FAMISCaseChild(i).TE, i)
            getBlockData(FAMISCaseChild(i).TF, i)
            getBlockData(FAMISCaseChild(i).TG, i)
            getBlockData(FAMISCaseChild(i).TH, i)
            getBlockData(FAMISCaseChild(i).TI, i)
            getBlockData(FAMISCaseChild(i).TI2, i)
            getBlockData(FAMISCaseChild(i).TI3, i)
            FAMISCaseChild(i).TI.SetData(FAMISCaseChild(i).TI.GetData & FAMISCaseChild(i).TI2.GetData & FAMISCaseChild(i).TI3.GetData)
            FAMISCaseChild(i).TI.Length = FAMISCaseChild(i).TI.Length + FAMISCaseChild(i).TI2.Length + FAMISCaseChild(i).TI3.Length

            getBlockData(FAMISCaseChild(i).TJ, i)
            getBlockData(FAMISCaseChild(i).TK, i)
            getBlockData(FAMISCaseChild(i).TL, i)
            getBlockData(FAMISCaseChild(i).TM, i)
            getBlockData(FAMISCaseChild(i).TN, i)
            getBlockData(FAMISCaseChild(i).TO1, i)
            getBlockData(FAMISCaseChild(i).TP, i)
            getBlockData(FAMISCaseChild(i).TQ, i)
            getBlockData(FAMISCaseChild(i).TR, i)
            getBlockData(FAMISCaseChild(i).TS, i)

            getBlockData(FAMISCaseChild(i).UA, i)
            getBlockData(FAMISCaseChild(i).UB, i)
            getBlockData(FAMISCaseChild(i).UC, i)
            getBlockData(FAMISCaseChild(i).UD, i)
            getBlockData(FAMISCaseChild(i).UE, i)
            getBlockData(FAMISCaseChild(i).UF, i)
            If FAMISCaseChild(i).UF.GetData = "AF" Or FAMISCaseChild(i).UF.GetData = "ap" Then FAMISCaseChild(i).UF.SetData("  ")
            getBlockData(FAMISCaseChild(i).UG, i)
            getBlockData(FAMISCaseChild(i).UH, i)
            getBlockData(FAMISCaseChild(i).UI, i)
            getBlockData(FAMISCaseChild(i).UJ, i)
            getBlockData(FAMISCaseChild(i).UK, i)
            getBlockData(FAMISCaseChild(i).UL, i)
            getBlockData(FAMISCaseChild(i).YA, i)

            getBlockData(FAMISCaseChild(i).QU, i)
            getBlockData(FAMISCaseChild(i).RS, i)
            getBlockData(FAMISCaseChild(i).RT, i)        
            getBlockData(FAMISCaseChild(i).TT, i)
            If TEXT_CaseChild.Length > 799 Then
                getBlockData(FAMISCaseChild(i).TU, i)
            Else
                FAMISCaseChild(i).TU.SetData("        ")
            End If

            SQLCaseChild(i) = New CaseChild

            getBlockData(SQLCaseChild(i).QA, i)
            getBlockData(SQLCaseChild(i).QB, i)
            getBlockData(SQLCaseChild(i).QC, i)
            getBlockData(SQLCaseChild(i).QD, i)
            getBlockData(SQLCaseChild(i).QE, i)
            'getBlockData(SQLCaseChild(i).QE2, i)
            'SQLCaseChild(i).QE.SetData(SQLCaseChild(i).QE.GetData & SQLCaseChild(i).QE2.GetData)
            'SQLCaseChild(i).QE.Length = SQLCaseChild(i).QE.Length + SQLCaseChild(i).QE2.Length

            getBlockData(SQLCaseChild(i).QF, i)
            getBlockData(SQLCaseChild(i).QG, i)
            getBlockData(SQLCaseChild(i).QH, i)
            getBlockData(SQLCaseChild(i).QI, i)
            getBlockData(SQLCaseChild(i).QI2, i)
            SQLCaseChild(i).QI.SetData(SQLCaseChild(i).QI.GetData & SQLCaseChild(i).QI2.GetData)
            SQLCaseChild(i).QI.Length = SQLCaseChild(i).QI.Length + SQLCaseChild(i).QI2.Length
            getBlockData(SQLCaseChild(i).QK, i)
            getBlockData(SQLCaseChild(i).QJ, i)
            getBlockData(SQLCaseChild(i).QL, i)
            getBlockData(SQLCaseChild(i).QM, i)
            getBlockData(SQLCaseChild(i).QN, i)
            getBlockData(SQLCaseChild(i).QO, i)
            SQLCaseChild(i).QO.SetData("  ")

            getBlockData(SQLCaseChild(i).RA, i)
            getBlockData(SQLCaseChild(i).RB, i)
            getBlockData(SQLCaseChild(i).RC, i)
            getBlockData(SQLCaseChild(i).RD, i)
            getBlockData(SQLCaseChild(i).RE, i)
            getBlockData(SQLCaseChild(i).RF, i)
            getBlockData(SQLCaseChild(i).RG, i)
            getBlockData(SQLCaseChild(i).RH, i)
            getBlockData(SQLCaseChild(i).RI, i)
            getBlockData(SQLCaseChild(i).RJ1, i)
            getBlockData(SQLCaseChild(i).RJ2, i)
            getBlockData(SQLCaseChild(i).RK, i)
            getBlockData(SQLCaseChild(i).RL, i)
            getBlockData(SQLCaseChild(i).RM, i)
            getBlockData(SQLCaseChild(i).RN, i)
            getBlockData(SQLCaseChild(i).RO, i)
            getBlockData(SQLCaseChild(i).RP, i)
            getBlockData(SQLCaseChild(i).RQ, i)
            getBlockData(SQLCaseChild(i).RR, i)

            getBlockData(SQLCaseChild(i).SA, i)
            getBlockData(SQLCaseChild(i).SB, i)
            getBlockData(SQLCaseChild(i).SC, i)
            getBlockData(SQLCaseChild(i).SD, i)
            getBlockData(SQLCaseChild(i).SE, i)
            getBlockData(SQLCaseChild(i).SF, i)
            getBlockData(SQLCaseChild(i).SG, i)
            getBlockData(SQLCaseChild(i).SH, i)
            getBlockData(SQLCaseChild(i).SI, i)
            getBlockData(SQLCaseChild(i).SJ, i)
            getBlockData(SQLCaseChild(i).SK, i)
            getBlockData(SQLCaseChild(i).SL, i)
            getBlockData(SQLCaseChild(i).SM, i)
            getBlockData(SQLCaseChild(i).SN, i)
            getBlockData(SQLCaseChild(i).SO, i)
            getBlockData(SQLCaseChild(i).SP, i)
            getBlockData(SQLCaseChild(i).SQ, i)
            getBlockData(SQLCaseChild(i).SR, i)
            getBlockData(SQLCaseChild(i).SS, i)
            getBlockData(SQLCaseChild(i).ST, i)

            getBlockData(SQLCaseChild(i).TA, i)
            getBlockData(SQLCaseChild(i).TB, i)
            getBlockData(SQLCaseChild(i).TC, i)
            getBlockData(SQLCaseChild(i).TD, i)
            getBlockData(SQLCaseChild(i).TE, i)
            getBlockData(SQLCaseChild(i).TF, i)
            getBlockData(SQLCaseChild(i).TG, i)
            getBlockData(SQLCaseChild(i).TH, i)
            getBlockData(SQLCaseChild(i).TI, i)
            getBlockData(SQLCaseChild(i).TI2, i)
            getBlockData(SQLCaseChild(i).TI3, i)
            SQLCaseChild(i).TI.SetData(SQLCaseChild(i).TI.GetData & SQLCaseChild(i).TI2.GetData & SQLCaseChild(i).TI3.GetData)
            SQLCaseChild(i).TI.Length = SQLCaseChild(i).TI.Length + SQLCaseChild(i).TI2.Length + SQLCaseChild(i).TI3.Length

            getBlockData(SQLCaseChild(i).TJ, i)
            getBlockData(SQLCaseChild(i).TK, i)
            getBlockData(SQLCaseChild(i).TL, i)
            getBlockData(SQLCaseChild(i).TM, i)
            getBlockData(SQLCaseChild(i).TN, i)
            getBlockData(SQLCaseChild(i).TO1, i)
            getBlockData(SQLCaseChild(i).TP, i)
            getBlockData(SQLCaseChild(i).TQ, i)
            getBlockData(SQLCaseChild(i).TR, i)
            getBlockData(SQLCaseChild(i).TS, i)

            getBlockData(SQLCaseChild(i).UA, i)
            getBlockData(SQLCaseChild(i).UB, i)
            getBlockData(SQLCaseChild(i).UC, i)
            getBlockData(SQLCaseChild(i).UD, i)
            getBlockData(SQLCaseChild(i).UE, i)
            getBlockData(SQLCaseChild(i).UF, i)
            If SQLCaseChild(i).UF.GetData = "AF" Or SQLCaseChild(i).UF.GetData = "ap" Then SQLCaseChild(i).UF.SetData("  ")
            getBlockData(SQLCaseChild(i).UG, i)
            getBlockData(SQLCaseChild(i).UH, i)
            getBlockData(SQLCaseChild(i).UI, i)
            getBlockData(SQLCaseChild(i).UJ, i)
            getBlockData(SQLCaseChild(i).UK, i)
            getBlockData(SQLCaseChild(i).UL, i)
            getBlockData(SQLCaseChild(i).YA, i)

            getBlockData(SQLCaseChild(i).QU, i)
            getBlockData(SQLCaseChild(i).RS, i)
            getBlockData(SQLCaseChild(i).RT, i)
            getBlockData(SQLCaseChild(i).TT, i)
            If TEXT_CaseChild.Length > 799 Then
                getBlockData(SQLCaseChild(i).TU, i)
            Else
                SQLCaseChild(i).TU.SetData("        ")
            End If
        Next
    End Sub
    Private Sub setBlockData_VRP()
        Dim i As Integer
        For i = 0 To numVRP - 1
            FAMISVRPInformation(i) = New VRPInformation(i)
            getBlockData(FAMISVRPInformation(i).VA, i)
            getBlockData(FAMISVRPInformation(i).VC, i)
            getBlockData(FAMISVRPInformation(i).VE, i)
            getBlockData(FAMISVRPInformation(i).VG, i)
            getBlockData(FAMISVRPInformation(i).VI, i)
            getBlockData(FAMISVRPInformation(i).VK, i)
            getBlockData(FAMISVRPInformation(i).VM, i)
            getBlockData(FAMISVRPInformation(i).VO, i)
            getBlockData(FAMISVRPInformation(i).VQ, i)

            SQLVRPInformation(i) = New VRPInformation(i)
            getBlockData(SQLVRPInformation(i).VA, i)
            getBlockData(SQLVRPInformation(i).VC, i)
            getBlockData(SQLVRPInformation(i).VE, i)
            getBlockData(SQLVRPInformation(i).VG, i)
            getBlockData(SQLVRPInformation(i).VI, i)
            getBlockData(SQLVRPInformation(i).VK, i)
            getBlockData(SQLVRPInformation(i).VM, i)
            getBlockData(SQLVRPInformation(i).VO, i)
            getBlockData(SQLVRPInformation(i).VQ, i)
        Next
    End Sub
    Private Function isValidDate(ByVal tempDate As String) As Boolean
        Dim dateCompare As Date
        Try
            dateCompare = tempDate.Insert(2, "/").Insert(5, "/")
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
#End Region

#Region "Existing Functions"
    Private Sub BGW_Existing_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_Existing.DoWork
        Dim SQLConn As New SqlConnection(My.Settings.phxSQLConn) '"Data Source=" & My.Settings.SQLAddress & "\Phoenix;Initial Catalog=PhoenixCaseData;Integrated Security=True;Persist Security Info=True;User ID=FAMISUser;Password=password")
        Dim SQLComm As New SqlCommand
        Dim SQLReader As SqlDataReader
        Dim Form As New Existing_105Form
        Dim frmProcess As processingFAMIS
        Dim i As Integer

        Form.numChildren = 0
        Form.numVRP = 0
        SQLComm.Connection = SQLConn
        If txt_CaseNumber.Text.Length > 6 Then
            Select Case txt_CaseNumber.Text.Length
                Case 7 : BGW_Existing.ReportProgress(7)
                Case 8 : BGW_Existing.ReportProgress(8)
                Case 9 : BGW_Existing.ReportProgress(9)
            End Select
            SQLConn.Open()
            SQLComm.CommandText = "SELECT * FROM FAMISCaseInformation WHERE CaseNumber = '" & txt_CaseNumber.Text & "'"
            SQLReader = SQLComm.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                BGW_Existing.ReportProgress(1)
                FAMISCaseInformation = New CaseInformation
                FAMISMedicaidInformation = New MedicaidInformation
                FAMISFoodStampInformation = New FoodStampInformation
                FAMISApplicationInformation = New ApplicationInformation
                FAMISTANFInformation = New TANFInformation
                FAMISIandAInformation = New IandAInformation
                FAMISIndividualsInformation = New IndividualsInformation
                FAMISIncomeInformation = New IncomeInformation
                For i = 0 To numChildren - 1
                    FAMISCaseChild(i) = New CaseChild
                Next
                For i = 0 To numVRP - 1
                    FAMISVRPInformation(i) = New VRPInformation(i)
                Next
                '--Go to 105 screen--
                SQLReader.Close()
                SQLComm.CommandText = "SELECT * FROM FAMISCaseChild WHERE CaseNumber = '" & txt_CaseNumber.Text & "'"
                SQLReader = SQLComm.ExecuteReader
                While SQLReader.Read()
                    Form.numChildren += 1
                End While
                SQLReader.Close()
                SQLComm.CommandText = "SELECT * FROM FAMISVRPInformation WHERE CaseNumber = '" & txt_CaseNumber.Text & "'"
                SQLReader = SQLComm.ExecuteReader
                While SQLReader.Read
                    Form.numVRP += 1
                End While
                Form.CASENUMBER = txt_CaseNumber.Text
                Form.ParentForm_Put105 = Me
                BGW_Existing.ReportProgress(10)
                Form.ShowDialog()
                If LineNP = "ABatch" Then
                    getBatchNumber("A")
                ElseIf LineNP = "UBatch" Then
                    getBatchNumber("U")
                ElseIf LineNP = "Cancel" Then
                    '--Cancel--
                Else
                    '--Something strange happened in the 105 Form--
                End If
                If LineNP <> "Cancel" Then
                    BGW_SQLCheck.CancelAsync()
                    frmProcess = New processingFAMIS
                    frmProcess.isView105 = False
                    'frmProcess.FAMISApplicationInformation = FAMISApplicationInformation
                    'frmProcess.FAMISCaseInformation = FAMISCaseInformation
                    'frmProcess.FAMISFoodStampInformation = FAMISFoodStampInformation
                    'frmProcess.FAMISIandAInformation = FAMISIandAInformation
                    'frmProcess.FAMISIncomeInformation = FAMISIncomeInformation
                    'frmProcess.FAMISIndividualsInformation = FAMISIndividualsInformation
                    'frmProcess.FAMISMedicaidInformation = FAMISMedicaidInformation
                    'frmProcess.FAMISTANFInformation = FAMISTANFInformation
                    'For i = 0 To numChildren - 1
                    '    frmProcess.FAMISCaseChild(i) = FAMISCaseChild(i)
                    'Next
                    'For i = 0 To numVRP - 1
                    '    frmProcess.FAMISVRPInformation(i) = FAMISVRPInformation(i)
                    'Next
                    ''frmProcess.FAMISAdjustments = TextFileAdjustments
                    'frmProcess.numChildren = numChildren
                    'frmProcess.numVRP = numVRP
                    frmProcess.BATCHNUMBER = BATCHNUMBER
                    frmProcess.ParentForm_Put105 = Me
                    isCaseError = False '--Set to false unless error occurs in processing--
                    BGW_Existing.ReportProgress(20)
                    If CheckSQL() Then
                        frmProcess.ShowDialog()
                        BGW_Existing.ReportProgress(21)
                        'If isCaseError Then BGW_GUMPProcess.ReportProgress(98)
                        If isCaseCancel Then
                            '--Case was canceled by the user--
                            BGW_Existing.ReportProgress(60)
                        ElseIf isLoginError Then
                            '--Login/Password error--
                            BGW_Existing.ReportProgress(46)
                        Else
                            '--Case successful--
                            LastBatchNumber = BATCHNUMBER
                            BGW_Existing.ReportProgress(45)
                            setBatchNumber(BATCHNUMBER.Substring(0, 1))
                        End If
                        If BGW_Existing.CancellationPending Then
                            '--The user or application pressed the stop button--
                            '--Force the for loop to end--
                        End If
                        BGW_SQLCheck.RunWorkerAsync()
                    Else
                        LineNP = "Cancel"
                        BGW_Existing.ReportProgress(99)
                        BGW_Existing.ReportProgress(21)
                    End If
                Else
                    BGW_Existing.ReportProgress(60)
                End If
            Else
                BGW_Existing.ReportProgress(11)
            End If
        End If
        BGW_Existing.ReportProgress(21)
        BGW_Existing.ReportProgress(100)
    End Sub
    Private Sub BGW_Existing_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BGW_Existing.ProgressChanged
        Select Case e.ProgressPercentage
            Case 1
                Me.Cursor = Cursors.WaitCursor
                setStatus("Running", Color.Green)
            Case 7
                txt_CaseNumber.Text &= COUNTY
                btn_Search.Enabled = False
            Case 8
                txt_CaseNumber.Text &= COUNTY.Substring(1, 2)
                btn_Search.Enabled = False
            Case 9
                txt_CaseNumber.Text &= COUNTY.Substring(2, 1)
                btn_Search.Enabled = False
            Case 10     '--Start processing--
                setInfo("Processing Case: " & txt_CaseNumber.Text.ToUpper, False)
                WriteLog("Processing Case: " & txt_CaseNumber.Text.ToUpper, False)
                Me.Enabled = False
                Me.WindowState = FormWindowState.Minimized
            Case 11     '--Case not found--
                setInfo("Case: " & txt_CaseNumber.Text & " not found on server.", False)
                WriteLog("Case: " & txt_CaseNumber.Text & " not found on server.", False)
            Case 20     '--Minimize and unenabled main form--
                Me.Enabled = False
                Me.WindowState = FormWindowState.Minimized
            Case 21     '--Restate and enable form--
                Me.Enabled = True
                Me.WindowState = FormWindowState.Normal
                Me.Focus()
            Case 45     '--Case completed--
                txt_BatchNumber.Text = LastBatchNumber
                setInfo(FAMISCaseInformation.AA.GetData & " submitted successfully.", False)
                WriteLog(FAMISCaseInformation.AA.GetData & " submitted successfully. (" & LastBatchNumber & ")", False)
            Case 46     '--Login error--
                setInfo("Login error > " & LoginErrorMsg & ".", True)
                WriteLog("Login error > " & LoginErrorMsg & ".", True)
                btn_GUMPStop_Click(Nothing, Nothing)
            Case 60     '--Report case was cancelled--
                setInfo(FAMISCaseInformation.AA.GetData & " was canceled.", False)
            Case 98
                'setInfo("Error backing up case to Server!", True)
                'WriteLog("Error backing up case to Server!", True)
            Case 99     '--Halt on SQL connection error--
                setInfo("Server not found! Cannot process case.", True)
                WriteLog("Server not found! Cannot process case.", True)
            Case 100
                btn_Search.Enabled = True
                Me.Cursor = Cursors.Default
                txt_CaseNumber.Text = Nothing
                setStatus("Stopped", Color.OrangeRed)
        End Select
    End Sub
#End Region

#Region "Menu Functions"
    Private Sub rdo_GUMP_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdo_GUMP.CheckedChanged
        grp_ManualControls.Visible = False
        grp_ExistingControls.Visible = False
        grp_GUMPControls.Visible = True
        Me.AcceptButton = btn_GUMPStart
        txt_Status.Text = "Stopped"
        txt_Status.BackColor = Color.OrangeRed
        btn_GUMPStop.Enabled = False
        menu_GUMP.Checked = True
        menu_Existing.Checked = False
        menu_Manual.Checked = False
    End Sub
    Private Sub rdo_Exist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdo_Exist.CheckedChanged
        grp_ManualControls.Visible = False
        grp_ExistingControls.Visible = True
        grp_GUMPControls.Visible = False
        Me.AcceptButton = btn_Search
        menu_GUMP.Checked = False
        menu_Existing.Checked = True
        menu_Manual.Checked = False
    End Sub
    Private Sub rdo_Manual_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdo_Manual.CheckedChanged
        grp_ManualControls.Visible = True
        grp_ExistingControls.Visible = False
        grp_GUMPControls.Visible = False
        Me.AcceptButton = btn_ManualStart_A
        menu_GUMP.Checked = False
        menu_Existing.Checked = False
        menu_Manual.Checked = True
    End Sub

    Private Sub btn_GUMPStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_GUMPStart.Click
        If isSecurityAvailable Then
            If isSecurityCheck() Then
                btn_GUMPStart.Enabled = False
                btn_GUMPStop.Enabled = True
                rdo_GUMP.Enabled = False
                rdo_Exist.Enabled = False
                rdo_Manual.Enabled = False
                menu_File.Enabled = False
                menu_Edit.Enabled = False
                menu_Mode.Enabled = False
                tray_Exit.Enabled = False
                isFileSource = True
                If Me.WindowState = FormWindowState.Normal Then Me.WindowState = FormWindowState.Minimized
                If Me.ShowInTaskbar = True Then Me.ShowInTaskbar = False
                setInfo("GUMP processing started.", False)
                setStatus("Running", Color.Green)
                If Not BGW_OnlineStatus.IsBusy Then BGW_OnlineStatus.RunWorkerAsync()
                BGW_GUMPProcess.RunWorkerAsync()
            End If
        Else
            btn_GUMPStart.Enabled = False
            btn_GUMPStop.Enabled = True
            rdo_GUMP.Enabled = False
            rdo_Exist.Enabled = False
            rdo_Manual.Enabled = False
            menu_File.Enabled = False
            menu_Edit.Enabled = False
            menu_Mode.Enabled = False
            tray_Exit.Enabled = False
            isFileSource = True
            If Me.WindowState = FormWindowState.Normal Then Me.WindowState = FormWindowState.Minimized
            If Me.ShowInTaskbar = True Then Me.ShowInTaskbar = False
            setInfo("GUMP processing started.", False)
            setStatus("Running", Color.Green)
            BGW_GUMPProcess.RunWorkerAsync()
        End If
    End Sub
    Private Sub btn_GUMPStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_GUMPStop.Click
        btn_GUMPStop.Enabled = False
        setStatus("Stopping", Color.Orange)
        If BGW_OnlineStatus.IsBusy Then BGW_OnlineStatus.CancelAsync()
        BGW_GUMPProcess.CancelAsync()
    End Sub
    Private Sub btn_Search_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Search.Click
        txt_CaseNumber.Text = txt_CaseNumber.Text.ToUpper
        isFileSource = False
        btn_Search.Enabled = False
        BGW_Existing.RunWorkerAsync()
    End Sub
    Private Sub txt_CaseNumber_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_CaseNumber.TextChanged
        If txt_CaseNumber.Text.Length < 7 Then btn_Search.Enabled = False Else btn_Search.Enabled = True
    End Sub
    Private Sub btn_ManualStart_A_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ManualStart_A.Click

    End Sub
    Private Sub btn_ManualStart_U_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ManualStart_U.Click

    End Sub

    Private Sub menu_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_Exit.Click
        Me.Close()
    End Sub
    Private Sub menu_UAPSUP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_UAPSUP.Click
        Dim RegReader As Microsoft.Win32.RegistryKey
        Dim KeyValue As String = "Software\\Phoenix\\"
        RegReader = My.Computer.Registry.LocalMachine.OpenSubKey(KeyValue, True)
        If Not RegReader Is Nothing Then
            RegReader.SetValue("Keyword", "UAPSUP")
        End If
        If Not menu_UAPSUP.Checked Then
            menu_UAPSUP.Checked = True
            menu_UAPCCS.Checked = False
            My.Settings.FAMISKeyword = "UAPSUP"
        End If
        SetConfig()
    End Sub
    Private Sub menu_UAPCCS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_UAPCCS.Click
        Dim RegReader As Microsoft.Win32.RegistryKey
        Dim KeyValue As String = "Software\\Phoenix\\"
        RegReader = My.Computer.Registry.LocalMachine.OpenSubKey(KeyValue, True)
        If Not RegReader Is Nothing Then
            RegReader.SetValue("Keyword", "UAPCCS")
        End If
        If Not menu_UAPCCS.Checked Then
            menu_UAPSUP.Checked = False
            menu_UAPCCS.Checked = True
            My.Settings.FAMISKeyword = "UAPCCS"
        End If
        SetConfig()
    End Sub
    Private Sub menu_GUMP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_GUMP.Click
        If menu_GUMP.Checked = False Then
            rdo_GUMP_CheckedChanged(Nothing, Nothing)
            rdo_GUMP.Checked = True
            menu_GUMP.Checked = True
            menu_Existing.Checked = False
            menu_Manual.Checked = False
        End If
    End Sub
    Private Sub menu_existing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_Existing.Click
        If menu_Existing.Checked = False Then
            rdo_Exist_CheckedChanged(Nothing, Nothing)
            rdo_Exist.Checked = True
            menu_GUMP.Checked = False
            menu_Existing.Checked = True
            menu_Manual.Checked = False
        End If
    End Sub
    Private Sub menu_Manual_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_Manual.Click
        If menu_Manual.Checked = False Then
            rdo_Manual_CheckedChanged(Nothing, Nothing)
            rdo_Manual.Checked = True
            menu_GUMP.Checked = False
            menu_Existing.Checked = False
            menu_Manual.Checked = True
        End If
    End Sub
    Private Sub menu_Options_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_Options.Click
        Dim Form As New Options
        isOptions = True
        Me.Enabled = False
        Me.TopMost = False
        Form.ShowDialog()
        Me.Enabled = True
        isOptions = False
        If My.Settings.isArchive Then
            Dim Directory As New DirectoryInfo(My.Settings.ArchiveDirectory)
            If Not Directory.Exists Then Directory.Create()
        End If
        isOptions = False
        If isSecurityAvailable Then isSecurityCheck()
    End Sub
    Private Sub menu_DelBatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_DelBatch.Click
        Dim Form As New DeleteBatch
        Me.Enabled = False
        Me.TopMost = False
        Form.ShowDialog()
        Me.TopMost = True
        Me.Enabled = True
    End Sub
    Private Sub menu_DelGUMP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_DelGUMP.Click
        Dim Form As New DeleteFile
        Me.Enabled = False
        Me.TopMost = False
        Form.ShowDialog()
        Me.TopMost = True
        Me.Enabled = True
    End Sub
    Private Sub menu_ViewGUMP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_ViewGUMP.Click
        Try
            System.Diagnostics.Process.Start(My.Settings.ArchiveDirectory)
        Catch ex As Exception
            setInfo("Folder does not exist", True)
        End Try
    End Sub
    Private Sub menu_ViewBatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_ViewBatch.Click
        Dim Form As New viewBatch
        Me.Enabled = False
        Me.TopMost = False
        Form.ShowDialog()
        Me.TopMost = True
        Me.Enabled = True
    End Sub
    Private Sub menu_About_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_About.Click
        Dim Form As New Splash
        Me.Enabled = False
        Me.TopMost = False
        Form.isSplash = False
        Form.ShowDialog()
        Me.Enabled = True
        Me.TopMost = True
    End Sub
    Private Sub menu_Update_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_Update.Click
        If My.Settings.UpdateAddress <> "" Or My.Settings.UpdateAddress <> Nothing Then
            If My.Computer.Network.Ping(My.Settings.UpdateAddress) Then
                If File.Exists("\\" & My.Settings.UpdateAddress & "\Update\UpdateInfo.txt") Then
                    Dim fileVersion As New StreamReader("\\" & My.Settings.UpdateAddress & "\Update\UpdateInfo.txt")
                    If My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Revision.ToString <> fileVersion.ReadLine.Substring(8, 5) Then
                        If MessageBox.Show("You are using an older version." & vbCrLf & "Would you like to update?", "Phoenix - Update", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                            Try
                                Thread.Sleep(500)
                                Process.Start(My.Application.Info.DirectoryPath & "\Phoenix - Update.exe")
                                Me.Close()
                            Catch ex As Exception
                                MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Try
                        End If
                    Else
                        MessageBox.Show("You're using the most recent version.", "Phoenix", MessageBoxButtons.OK)
                    End If
                Else
                    MessageBox.Show("Update directory not found!" & vbCrLf & "Cannot check for updates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("Server not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show("No update server provided." & vbCrLf & "Cannot check for updates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        End If
    End Sub
    Private Sub trayIcon_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles trayIcon.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If Me.WindowState = FormWindowState.Normal Then
                Me.WindowState = FormWindowState.Minimized
                Me.ShowInTaskbar = False
            Else
                Me.WindowState = FormWindowState.Normal
                Me.ShowInTaskbar = True
                txt_Info.Focus()
            End If
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then

        End If
    End Sub
    Private Sub tray_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tray_Exit.Click
        Me.Close()
    End Sub
#End Region

    Private Sub menu_IMPSBatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menu_IMPSBatch.Click
        Dim Form As New viewIMPs
        Me.Enabled = False
        Me.TopMost = False
        Form.ShowDialog()
        Me.TopMost = True
        Me.Enabled = True
    End Sub
End Class