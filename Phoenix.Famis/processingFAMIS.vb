Public Class processingFAMIS

#Region "Declarations"
    Public BATCHNUMBER As String
    Public ErrorMessage As String
    Public ParentForm_Put105 As Put105Thru

    Public isError As Boolean               '--Track if there was a GLink error--
    Public isFileSource As Boolean          '--Tells what the data source choice's should be--
    Public isView105 As Boolean             '--Tracks if we should view the 105 form before submitting the case--
    Public isContinue As Boolean            '--Tracks if the user selected to retry the case--
    Public isOverride As Boolean            '--Tracks if the user selected to override the errors--
    Public isCancelCase As Boolean          '--Tracks if the user selected to cancel the case--
    Public isGoToChild(35) As Boolean       '--Tracks whether the child should be entered if any information is needed to be entered--
    Public isRedo_Page1, isRedo_Page2, isRedo_Page3, isRedo_Page4, isRedo_Page5, isRedo_Page6, isRedo_Page9, isRedo_Page10 As Boolean
    Public Redo_ChildNum(35) As String
    Public Redo_totalPages As Integer
    Public Redo_totalChildren As Integer

    Private is105FormCancel As Boolean
    Private isRedo As Boolean               '--Tracks whether this is a redo of a page or not--
    Private isAutoOverride As Boolean       '--Tracks whether there is only W level errors that can be overrided--
    Private glapiTP8 As connGLinkTP8
    Private ProcessingThread As Thread      '--Captures the thread ID of the background worker so we can abort if cancel is selected--
    Private CASENUMBER As String
    Private ChildIndex, VRPIndex As Integer '--Holds the index of the child and VRP number we're putting through at the moment--
    Private SQLConn, SQLDHConn As SqlClient.SqlConnection '--SQL connection--
    Private SQLCommand, SQLDHCommand As SqlClient.SqlCommand       '--SQL command string--
    Private SQLReader As SqlClient.SqlDataReader     '--SQL data reader--
    Private SQLResubmitCounter As Integer

    Delegate Sub SetTextCallback(ByVal [text] As String)
#End Region
    Private Sub debugFields()
        Dim FIELDs As Glink.GlinkFields
        Dim FIELD As Glink.GlinkField
        Dim temp As String = Nothing

        FIELDs = glapiTP8.GLAPI.getFields
        'FIELDs = glapi.getFields
        If TypeOf FIELDs Is Glink.GlinkFields Then
            'MessageBox.Show(FIELDs.getCount)
        End If
        Dim i As Integer
        For i = 0 To FIELDs.getCount - 1
            FIELD = FIELDs.item(i + 1)
            If FIELD.getAttribute = "-536861665" Then
                'MessageBox.Show(temp & " is blinking.")
            End If

            If FIELD.isProtected = True Then temp = FIELD.getString
            If FIELD.isProtected = False Then
                Dim LogDate As String = Date.Now.Month & "_" & Date.Now.Day & "_" & Date.Now.Year
                Dim LogTime As String = Date.Now.TimeOfDay.Hours.ToString & ":" & Date.Now.TimeOfDay.Minutes.ToString & ":" & Date.Now.TimeOfDay.Seconds.ToString
                Dim File_Writer As New StreamWriter("c:\test.txt", True)
                File_Writer.WriteLine(temp & " = " & FIELDs.getFieldIndex(FIELD) & " (" & FIELD.getLength & ")" & "     -|" & FIELD.getAttribute & vbCrLf) 'FIELDs.getFieldIndex(FIELD) & " = " & FIELD.getString & "-| " & FIELD.getString.Length)
                File_Writer.Close()
                'ListBox1.Items.Add(temp & " = " & FIELDs.getFieldIndex(FIELD) & " (" & FIELD.getLength & ")" & "     -|" & FIELD.getAttribute & vbCrLf) 'FIELDs.getFieldIndex(FIELD) & " = " & FIELD.getString & "-| " & FIELD.getString.Length)
            End If
            '-536862689     NOT BLINKING
            '-536861665     BLINKING
        Next
    End Sub

    Private Sub processingFAMIS_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim StartLocation As Point
        Dim isBadMediDate As Boolean = False '--No FAMIS edits for Medicaid dates so we have to check before hand--
        Dim Form As New display105Form
        StartLocation = New Point(My.Computer.Screen.Bounds.Width - 263, My.Computer.Screen.WorkingArea.Height - 137)
        Me.Location = StartLocation
        CASENUMBER = FAMISCaseInformation.AA.GetData
        SQLConn = New SqlConnection(My.Settings.phxSQLConn)
        SQLDHConn = New SqlConnection(My.Settings.phxSQLConn)
        SQLCommand = New SqlCommand()
        SQLCommand.Connection = SQLConn
        SQLDHCommand = New SqlCommand
        SQLDHCommand.Connection = SQLDHConn
        SQLResubmitCounter = 0
        If FAMISCaseInformation.AK.GetData <> "00000000  " Then CompareToSQL()
        '--Medicaid Deletion Issue 6/25/2008--
        '--If JQ changed to - then HC and AN must be also--
        If FAMISIncomeInformation.JQ.GetData.Substring(0, 1) = "-" Then
            FAMISCaseInformation.AN.SetData("-".PadRight(FAMISCaseInformation.AN.Length, " "))
            FAMISMedicaidInformation.HC.SetData("-".PadRight(FAMISMedicaidInformation.HC.Length, " "))
        End If
        '--5/15/2009--
        '--HI and HQ still bringing in - even tho it's told not to--
        '--Double check and delete--
        If FAMISMedicaidInformation.HI.GetData = "- " Then FAMISMedicaidInformation.HI.SetData("  ")
        If FAMISMedicaidInformation.HQ.GetData = "- " Then FAMISMedicaidInformation.HQ.SetData("  ")
        is105FormCancel = False
        btn_Cancel.Enabled = False
        Me.Text = "Processing Case: " & CASENUMBER
        progressbar_FAMIS.Value = 0
        txt_BatchNumber.Text = BATCHNUMBER
        setProgressMax()
        '--Check Medicaid dates before submitting--
        If Not isValidDate(FAMISMedicaidInformation.WC.GetData) Then isBadMediDate = True
        If Not isValidDate(FAMISMedicaidInformation.WD.GetData) Then isBadMediDate = True
        If Not isValidDate(FAMISMedicaidInformation.WF.GetData) Then isBadMediDate = True
        If Not isValidDate(FAMISMedicaidInformation.WG.GetData) Then isBadMediDate = True
        If isBadMediDate Then
            isView105 = True
            Form.ErrorMessage = "MEDICAID DATE NOT VALID - WC, WD, WF, WG"
            Form.txt_WC.BackColor = Color.Red
            Form.txt_WD.BackColor = Color.Red
            Form.txt_WF.BackColor = Color.Red
            Form.txt_WG.BackColor = Color.Red
        End If
        If isView105 Then
            If isCaseExist() Then Form.isCaseOnSQL = True Else Form.isCaseOnSQL = False
            Form.BATCHNUMBER = BATCHNUMBER
            Form.CASENUMBER = CASENUMBER
            Form.glapiTP8 = glapiTP8
            'Form.numChildren = numChildren
            'Form.numVRP = numVRP
            If Not isBadMediDate Then Form.ErrorMessage = ""
            Form.isPreview = isView105
            Form.isFileSource = isFileSource
            'Form.FAMISApplicationInformation = FAMISApplicationInformation
            'Form.FAMISCaseInformation = FAMISCaseInformation
            'Form.FAMISFoodStampInformation = FAMISFoodStampInformation
            'Form.FAMISIandAInformation = FAMISIandAInformation
            'Form.FAMISIncomeInformation = FAMISIncomeInformation
            'Form.FAMISIndividualsInformation = FAMISIndividualsInformation
            'Form.FAMISMedicaidInformation = FAMISMedicaidInformation
            'Form.FAMISTANFInformation = FAMISTANFInformation
            'For i = 0 To numChildren - 1
            '    Form.FamisCaseChild(i) = FAMISCaseChild(i)
            'Next
            'For i = 0 To numVRP - 1
            '    Form.FAMISVRPInformation(i) = FAMISVRPInformation(i)
            'Next
            'Form.numChildren = numChildren
            'Form.numVRP = numVRP
            Form.BATCHNUMBER = BATCHNUMBER
            Form.isPageError = False
            Form.isOverride = False
            Form.ParentForm_Processing = Me
            Me.Enabled = False
            Form.ShowDialog()
            Me.Enabled = True
        End If
        isView105 = False
        isRedo = False
        BGW_ProcessFAMIS.RunWorkerAsync()
    End Sub
    Private Sub processingFAMIS_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        '--Handle some issues where GLink is not disconnecting--
        Try
            glapiTP8.Disconnect()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub GLink_Start()
        Dim Message As String = "          "
        Dim RetryCounter As Integer = 0
        Dim counter As Integer = 0
        Dim isLogonError As Boolean = False
        Dim isPasswordError As Boolean = False
        While RetryCounter < 3
            Message = "          "
            isError = False
            isLogonError = False
            glapiTP8 = New connGLinkTP8(My.Settings.GLinkDirectory & "BullProd.cfg")
            glapiTP8.bool_Visible = My.Settings.GLinkVisible
            '--Check to see if GLink actually started and connected--
            '--If not we will reintialize the GLink element and try again--
            While Not glapiTP8.isConnect()
                Thread.Sleep(500)
                KillAllGLink()
                glapiTP8 = New connGLinkTP8(My.Settings.GLinkDirectory & "BullProd.cfg")
                glapiTP8.bool_Visible = My.Settings.GLinkVisible
                counter += 1
                If counter > 5 Then
                    isError = True
                    isLogonError = True
                    ParentForm_Put105.LoginErrorMsg = "Cannot connect to FAMIS. Please retry."
                End If
            End While
            ' ParentForm_Put105.TEMPLocation = "connected"
            ''''''''''''''''' glapiTP8.SendKeysTransmit("HSA")
            'ParentForm_Put105.TEMPLocation = "HSA adding"

            'While glapiTP8.GetString(4, 7, 25, 7) <> "0100 YOU ARE CONNECTED" And counter < 30
            '    'ParentForm_Put105.TEMPLocation = "after HSA"
            '    counter += 1
            '    Thread.Sleep(500)
            '    If counter = 30 Then
            '        isError = True
            '        'isLogonError = False
            '    End If
            'End While

            counter = 0
            If glapiTP8.GetString(4, 9, 28, 9) = "0200 YOU ARE DISCONNECTED" Then
                'ParentForm_Put105.TEMPLocation = "you are disconnected"
                isError = True
                ' isLogonError = False
            End If
            If Not isError Then
                'ParentForm_Put105.TEMPLocation = "LOGON sending"
                glapiTP8.SendKeysTransmit("LOGON")
                While glapiTP8.GetString(4, 21, 11, 21) <> "OPERATOR" And counter < 30
                    'ParentForm_Put105.TEMPLocation = "checking logon"
                    counter += 1
                    Thread.Sleep(500)
                    If counter = 30 Then
                        isError = True
                        '   isLogonError = False
                    End If
                End While
                'ParentForm_Put105.TEMPLocation = "LOGON added"
                counter = 0
                'ParentForm_Put105.TEMPLocation = "before operator logon"    '--TODO: Remove--
                If Not isError Then
                    glapiTP8.SubmitField(4, My.Settings.FAMISOperatorID)
                    glapiTP8.SubmitField(6, My.Settings.FAMISPassword)
                    glapiTP8.SubmitField(8, My.Settings.FAMISKeyword)
                    glapiTP8.TransmitPage()
                    Message = glapiTP8.GetString(10, 22, 40, 22)
                End If
            End If
            If glapiTP8.GetString(30, 4, 37, 4) = "PASSWORD" Then
                setStatusLabel("Password is out of date. Please update it.")
                glapiTP8.SetVisible(True)
                isPasswordError = True
                AbortProcessing()
                ''--Wait until user hits retry or cancel--
                'If MessageBox.Show("Please update password" & vbCrLf & "then click retry?", "Phoenix", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                '    RetryCounter = 3
                '    isError = False
                '    isLogonError = False
                '    ParentForm_Put105.LoginErrorMsg = "User cancelled processing"
                '    AbortProcessing()
                'Else
                '    RetryCounter = 3
                '    isError = False
                '    isLogonError = False
                'End If
                'glapiTP8.SetVisible(False)
            ElseIf Message.Substring(0, 5) = "     " Then
                'ParentForm_Put105.TEMPLocation = "no msg provided"
                '--No message provided by GLink--
                RetryCounter += 1
                isError = True
                ' isLogonError = False
                Message = "Unknown GLink error. Please restart."
            ElseIf Message.Substring(0, 7) = "INVALID" Then
                'ParentForm_Put105.TEMPLocation = "invalid something"
                '--Invalid password--
                RetryCounter = 3
                isError = True
                isLogonError = True
                If Message.Substring(0, 15) = "INVALID KEYWORD" Then ParentForm_Put105.LoginErrorMsg = "Invalid keyword"
                If Message.Substring(0, 16) = "INVALID PASSWORD" Then ParentForm_Put105.LoginErrorMsg = "Invalid password"
            ElseIf Message.Substring(0, 8) = "OPERATOR" Then
                '--Operator already logged on--
                ' ParentForm_Put105.TEMPLocation = "operator already logged on"
                RetryCounter += 1
                KillAllGLink()
                'RetryCounter = 3
                isError = True
                isLogonError = False
                ParentForm_Put105.LoginErrorMsg = "Operator already logged on"
            ElseIf Message.Substring(0, 5) <> "LOGON" Then
                '--Double check that there is a message from GLink--
                RetryCounter += 1
                isError = True
                isLogonError = True
                ParentForm_Put105.LoginErrorMsg = Message
            Else
                RetryCounter = 3
                'isError = False
                'isLogonError = False
            End If
            If isPasswordError Then '--eliminate at some point--
                isPasswordError = False
                isError = False
                isLogonError = False
                setStatusLabel("Retrying...")
            ElseIf isError And Not isLogonError Then
                If RetryCounter <= 3 Then
                    Try
                        glapiTP8.Disconnect()
                    Catch
                    End Try
                    setStatusLabel("GLink Error. Retrying... (Attempt " & RetryCounter + 1 & " of 3)")
                Else
                    setStatusLabel(Message)
                    AbortProcessing()
                End If
            ElseIf isError And isLogonError Then
                RetryCounter = 3
                setStatusLabel(Message)
                AbortProcessing()
            End If
            If isError = False Then glapiTP8.TransmitPage()
            Thread.Sleep(500)
        End While
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
    Private Sub KillAllGLink()
        Dim GLProcess() As Process
        Dim i As Integer
        GLProcess = Process.GetProcessesByName("gl")
        If GLProcess.Length > 0 Then
            For i = 0 To GLProcess.Length - 1
                GLProcess(i).Kill()
            Next
        End If
    End Sub
    Private Sub GLink_PageErrorCheck(ByVal fromPageNumber As String, ByVal toPageNumber As String, ByVal isSetLabel As Boolean)
        Dim isLoop As Boolean = True
        Dim BlockArray(10), BlockName(10), BlockData(10) As String
        Dim i As Integer
        If glapiTP8.isPageError(toPageNumber) Then
            '--There is an error on our current page--
            '--Find the field's blinking that are in error and report them to the user--
            If glapiTP8.GetField_Blinking <> Nothing Then
                BlockArray = glapiTP8.GetField_Blinking.Split(vbCrLf)
                For i = 0 To BlockArray.Length - 1
                    If BlockArray(i) <> Nothing Then
                        If BlockArray(i).Length > 17 Then
                            If BlockArray(i).Substring(1, 16) = "CASEMODIFICATION" Then     '--11/29/2007 Fixed for block LG showing up as CA--
                                If BlockArray(i).Substring(28, 2) = "AB" Then               '--1/10/2008 Fixed for block AB showing up as the data--
                                    BlockName(i) = "AB"
                                ElseIf BlockArray(i).Substring(BlockArray(i).Length - 4, 2) = "OA" Then
                                    BlockName(i) = "OA"
                                Else
                                    BlockName(i) = BlockArray(i).Substring(BlockArray(i).Length - 3, 2)
                                End If
                            ElseIf BlockArray(i).Substring(1, 9) = "VOLUNTARY" Then     '--1/18/2008 Fixed for block VA showing up as VO--
                                BlockName(i) = "VA"
                                BlockData(i) = BlockArray(i).Substring(BlockArray(i).Length - 5, 4)
                            ElseIf BlockArray(i).Substring(1, 15) = "CASENAMEAC/LAST" Then  '--3/5/2008 Fixed fo block AC showing up as CA--
                                BlockName(i) = "AC"
                                BlockArray(i) = BlockArray(i).Substring(16)
                            Else
                                BlockName(i) = BlockArray(i).Substring(1, 2)
                                BlockData(i) = BlockArray(i).Substring(3)
                            End If
                        Else
                            If BlockArray(i).Length > 4 Then
                                If BlockArray(i).Substring(1, 4) = "BUBV" Then  '--12/11/2007 Fixed for block BV showing up as BU--
                                    BlockName(i) = BlockArray(i).Substring(BlockArray(i).Length - 3, 2)
                                ElseIf BlockArray(i).Substring(BlockArray(i).Length - 3, 2) = "FI" Then
                                    BlockName(i) = "FI"
                                ElseIf BlockArray(i).Substring(BlockArray(i).Length - 5, 2) = "FP" Then
                                    BlockName(i) = "FP"
                                Else
                                    BlockName(i) = BlockArray(i).Substring(1, 2)
                                    BlockData(i) = BlockArray(i).Substring(3)
                                End If
                            Else
                                BlockName(i) = BlockArray(i).Substring(1, 2)
                                BlockData(i) = BlockArray(i).Substring(3)
                            End If
                        End If
                    End If
                    If BlockName(i) = "DA" Then i += 2
                Next
                ErrorMessage = CASENUMBER & ": Error On Page " & fromPageNumber & "!"
                For i = 0 To BlockData.Length - 1
                    If BlockName(i) <> Nothing And BlockName(i) <> "  " Then ErrorMessage += vbCrLf & "Field: " & BlockName(i)
                Next
            Else
                ErrorMessage = CASENUMBER & ": Error On Page " & fromPageNumber & "!"
                glapiTP8.SetVisible(True)
            End If
            ErrorScreen(True)
            '--Determine if we are cancelling the case or are retrying the page with the corrected data--
            If glapiTP8.bool_Visible = True Then glapiTP8.SetVisible(False)
            If isCancelCase = True Then
                BGW_ProcessFAMIS.ReportProgress(99)
            Else
                Select Case fromPageNumber
                    Case "01"
                        Submit_Page1()
                        glapiTP8.TransmitPage()
                        If isRedo = False Then GLink_PageErrorCheck("01", "02", True) Else GLink_PageErrorCheck("01", "02", False)
                    Case "02"
                        Submit_Page2()
                        glapiTP8.TransmitPage()
                        If isRedo = False Then GLink_PageErrorCheck("02", "03", True) Else GLink_PageErrorCheck("02", "03", False)
                    Case "03"
                        Submit_Page3()
                        glapiTP8.TransmitPage()
                        If isRedo = False Then GLink_PageErrorCheck("03", "04", True) Else GLink_PageErrorCheck("03", "04", False)
                    Case "04"
                        Submit_Page4()
                        glapiTP8.TransmitPage()
                        If isRedo = False Then GLink_PageErrorCheck("04", "05", True) Else GLink_PageErrorCheck("04", "05", False)
                    Case "05"
                        Submit_Page5()
                        glapiTP8.TransmitPage()
                        If isRedo = False Then GLink_PageErrorCheck("05", "06", True) Else GLink_PageErrorCheck("05", "06", False)
                    Case "06"
                        Submit_Page6()
                        If numChildren > 0 Then
                            glapiTP8.TransmitPage()
                            If isRedo = False Then GLink_PageErrorCheck("06", "07", True) Else GLink_PageErrorCheck("06", "07", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        ElseIf numVRP > 0 Then
                            glapiTP8.SendCommand("09")
                            glapiTP8.TransmitPage()
                            If isRedo = False Then GLink_PageErrorCheck("06", "09", True) Else GLink_PageErrorCheck("06", "09", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        Else
                            glapiTP8.SendCommand("10")
                            glapiTP8.TransmitPage()
                            If isRedo = False Then GLink_PageErrorCheck("06", "10", True) Else GLink_PageErrorCheck("06", "10", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                    Case "07"
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QB.FieldNumber, FAMISCaseChild(ChildIndex).QB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QC.FieldNumber, FAMISCaseChild(ChildIndex).QC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QD.FieldNumber, FAMISCaseChild(ChildIndex).QD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QE.FieldNumber, FAMISCaseChild(ChildIndex).QE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QF.FieldNumber, FAMISCaseChild(ChildIndex).QF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QG.FieldNumber, FAMISCaseChild(ChildIndex).QG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QH.FieldNumber, FAMISCaseChild(ChildIndex).QH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QI.FieldNumber, FAMISCaseChild(ChildIndex).QI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QJ.FieldNumber, FAMISCaseChild(ChildIndex).QJ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QK.FieldNumber, FAMISCaseChild(ChildIndex).QK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QL.FieldNumber, FAMISCaseChild(ChildIndex).QL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QM.FieldNumber, FAMISCaseChild(ChildIndex).QM.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QN.FieldNumber, FAMISCaseChild(ChildIndex).QN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).QO.FieldNumber, FAMISCaseChild(ChildIndex).QO.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RA.FieldNumber, FAMISCaseChild(ChildIndex).RA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RB.FieldNumber, FAMISCaseChild(ChildIndex).RB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RC.FieldNumber, FAMISCaseChild(ChildIndex).RC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RD.FieldNumber, FAMISCaseChild(ChildIndex).RD.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).RE.FieldNumber, FAMISCaseChild(childindex).RE.GetData) --Protected--
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).RF.FieldNumber, FAMISCaseChild(childindex).RF.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RG.FieldNumber, FAMISCaseChild(ChildIndex).RG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RH.FieldNumber, FAMISCaseChild(ChildIndex).RH.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).RH2.FieldNumber, FAMISCaseChild(childindex).RH2.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RI.FieldNumber, FAMISCaseChild(ChildIndex).RI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RJ1.FieldNumber, FAMISCaseChild(ChildIndex).RJ1.GetData & FAMISCaseChild(ChildIndex).RJ2.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RK.FieldNumber, FAMISCaseChild(ChildIndex).RK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RL.FieldNumber, FAMISCaseChild(ChildIndex).RL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RM.FieldNumber, FAMISCaseChild(ChildIndex).RM.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RN.FieldNumber, FAMISCaseChild(ChildIndex).RN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RO.FieldNumber, FAMISCaseChild(ChildIndex).RO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RP.FieldNumber, FAMISCaseChild(ChildIndex).RP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RQ.FieldNumber, FAMISCaseChild(ChildIndex).RQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).RR.FieldNumber, FAMISCaseChild(ChildIndex).RR.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SA.FieldNumber, FAMISCaseChild(ChildIndex).SA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SB.FieldNumber, FAMISCaseChild(ChildIndex).SB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SC.FieldNumber, FAMISCaseChild(ChildIndex).SC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SD.FieldNumber, FAMISCaseChild(ChildIndex).SD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SE.FieldNumber, FAMISCaseChild(ChildIndex).SE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SF.FieldNumber, FAMISCaseChild(ChildIndex).SF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SG.FieldNumber, FAMISCaseChild(ChildIndex).SG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SH.FieldNumber, FAMISCaseChild(ChildIndex).SH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SI.FieldNumber, FAMISCaseChild(ChildIndex).SI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SJ.FieldNumber, FAMISCaseChild(ChildIndex).SJ.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).SK.FieldNumber, FAMISCaseChild(childindex).SK.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SL.FieldNumber, FAMISCaseChild(ChildIndex).SL.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).SM.FieldNumber, FAMISCaseChild(childindex).SM.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SN.FieldNumber, FAMISCaseChild(ChildIndex).SN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SO.FieldNumber, FAMISCaseChild(ChildIndex).SO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SP.FieldNumber, FAMISCaseChild(ChildIndex).SP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SQ.FieldNumber, FAMISCaseChild(ChildIndex).SQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SR.FieldNumber, FAMISCaseChild(ChildIndex).SR.GetData & " ")
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).SS.FieldNumber, FAMISCaseChild(ChildIndex).SS.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).ST.FieldNumber, FAMISCaseChild(childindex).ST.GetData) --Protected--
                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("07", "08", True)
                    Case "08"
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TA.FieldNumber, FAMISCaseChild(ChildIndex).TA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TB.FieldNumber, FAMISCaseChild(ChildIndex).TB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TC.FieldNumber, "       ")
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TD.FieldNumber, FAMISCaseChild(ChildIndex).TD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TE.FieldNumber, FAMISCaseChild(ChildIndex).TE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TF.FieldNumber, " ")
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TG.FieldNumber, FAMISCaseChild(ChildIndex).TG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TH.FieldNumber, FAMISCaseChild(ChildIndex).TH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TI.FieldNumber, FAMISCaseChild(ChildIndex).TI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TJ.FieldNumber, FAMISCaseChild(ChildIndex).TJ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TK.FieldNumber, FAMISCaseChild(ChildIndex).TK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TL.FieldNumber, FAMISCaseChild(ChildIndex).TL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TM.FieldNumber, FAMISCaseChild(ChildIndex).TM.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(childindex).TN.FieldNumber, FAMISCaseChild(childindex).TN.GetData) --Not Used--
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TO1.FieldNumber, FAMISCaseChild(ChildIndex).TO1.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TP.FieldNumber, FAMISCaseChild(ChildIndex).TP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TQ.FieldNumber, FAMISCaseChild(ChildIndex).TQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TR.FieldNumber, FAMISCaseChild(ChildIndex).TR.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).TS.FieldNumber, FAMISCaseChild(ChildIndex).TS.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UA.FieldNumber, FAMISCaseChild(ChildIndex).UA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UB.FieldNumber, FAMISCaseChild(ChildIndex).UB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UC.FieldNumber, FAMISCaseChild(ChildIndex).UC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UD.FieldNumber, FAMISCaseChild(ChildIndex).UD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UE.FieldNumber, FAMISCaseChild(ChildIndex).UE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UF.FieldNumber, FAMISCaseChild(ChildIndex).UF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UG.FieldNumber, FAMISCaseChild(ChildIndex).UG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UH.FieldNumber, FAMISCaseChild(ChildIndex).UH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(ChildIndex).UI.FieldNumber, FAMISCaseChild(ChildIndex).UI.GetData)

                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("08", "07", False)
                    Case "09"
                        Submit_Page9()
                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("09", "10", False)
                    Case "10"
                        Submit_Page10()
                        CloseCase()
                End Select
            End If
        Else
            If isSetLabel Then setStatusLabel("Submitting Page " & toPageNumber)
        End If
    End Sub

    Private Sub StoreSQL()
        Dim i As Integer
        Dim tempCases As Integer
        Try
            SQLConn.Open()
            SQLDHConn.Open()

            SQLCommand.CommandText() = "SELECT CASENUMBER FROM FAMISCaseInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                SQLReader.Close()
                SQLCommand.CommandText() = "SELECT CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW FROM FAMISCaseInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    If SQLReader.IsDBNull(22) Or SQLReader.IsDBNull(21) Or SQLReader.IsDBNull(19) Or SQLReader.IsDBNull(16) Or SQLReader.IsDBNull(17) Then
                        SQLDHCommand.CommandText() = "INSERT INTO DAILYHOLD_CaseInformation (CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetDateTime(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetDateTime(15) + "', ' ', ' ', '" + SQLReader.GetString(18) + "', ' ', '" + SQLReader.GetString(20) + "', ' ', '   ')"
                        SQLDHCommand.ExecuteNonQuery()
                    Else
                        SQLDHCommand.CommandText() = "INSERT INTO DAILYHOLD_CaseInformation (CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetDateTime(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetDateTime(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "')"
                        SQLDHCommand.ExecuteNonQuery()
                    End If
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, HA, IA, IB, IC, ID, IE, IF1, IG, IH, II, IJ, IL, IK, IP, OJ, IM, IN1, IO FROM FAMISAFDCInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_AFDCInformation (CASENUMBER, HA, IA, IB, IC, ID, IE, IF1, IG, IH, II, IJ, IL, IK, IP, OJ, IM, IN1, IO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetDateTime(4) + "', '" + SQLReader.GetDateTime(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetDateTime(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "',  '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, CA, CB, CC, CD1, CD2, CE, CF, CG, DA1, DA2, DA3, DB, DC, DD1, DD2, DE, DF FROM FAMISApplicantInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_ApplicantInformation (CASENUMBER, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, CA, CB, CC, CD1, CD2, CE, CF, CG, DA1, DA2, DA3, DB, DC, DD1, DD2, DE, DF) VALUES  ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "',  '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT EA, EB, EC, ED1, ED2, EE, EF, EG, EH, EJ, NN, BR, OM, EI, EK, EL, EM, EN, XA, XB, XC, XD, XE, XF, XG, XH, XI, XJ, XK, XL, XM, XN FROM FAMISApplicantInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "UPDATE DAILYHOLD_ApplicantInformation SET EA = '" + SQLReader.GetString(0) + "', EB = '" + SQLReader.GetString(1) + "', EC = '" + SQLReader.GetString(2) + "', ED1 = '" + SQLReader.GetString(3) + "', ED2 = '" + SQLReader.GetString(4) + "', EE = '" + SQLReader.GetString(5) + "', EF = '" + SQLReader.GetString(6) + "', EG = '" + SQLReader.GetString(7) + "', EH = '" + SQLReader.GetString(8) + "', EJ = '" + SQLReader.GetString(9) + "', NN = '" + SQLReader.GetString(10) + "', BR = '" + SQLReader.GetString(11) + "', OM = '" + SQLReader.GetString(12) + "', EI = '" + SQLReader.GetString(13) + "', EK = '" + SQLReader.GetString(14) + "', EL = '" + SQLReader.GetString(15) + "', EM = '" + SQLReader.GetString(16) + "', EN = '" + SQLReader.GetString(17) + "', XA = '" + SQLReader.GetDateTime(18) + "', XB = '" + SQLReader.GetString(19) + "', XC = '" + SQLReader.GetString(20) + "', XD = '" + SQLReader.GetString(21) + "', XE = '" + SQLReader.GetString(22) + "', XF = '" + SQLReader.GetString(23) + "',  XG = '" + SQLReader.GetString(24) + "', XH = '" + SQLReader.GetString(25) + "', XI = '" + SQLReader.GetString(26) + "', XJ = '" + SQLReader.GetString(27) + "', XK = '" + SQLReader.GetString(28) + "', XL = '" + SQLReader.GetString(29) + "', XM = '" + SQLReader.GetString(30) + "', XN = '" + SQLReader.GetString(31) + "' WHERE CASENUMBER = '" + CASENUMBER + "'"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, BS, BT, BX, FA, FB, BU, FC, BV, FD, FD2, BW, FE2, FE1, BY1, FF, BZ, FG, FI, FJ, FK, FL, FL2, FM1, FM2, FN, FP, GA, GB, GC, GD, GE, GF, GG, GH, GI, GJ, GK, GL, FH, FO FROM FAMISIndividualsInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_IndividualsInformation (CASENUMBER, BS, BT, BX, FA, FB, BU, FC, BV, FD, FD2, BW, FE2, FE1, BY1, FF, BZ, FG, FI, FJ, FK, FL, FL2, FM1, FM2, FN, FP, GA, GB, GC, GD, GE, GF, GG, GH, GI, GJ, GK, GL, FH, FO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetDateTime(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetDateTime(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "',  '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetDateTime(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetDateTime(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetDateTime(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetDateTime(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, JP, JQ, JS, JT, JW, JX, KU, KV, JA, JB, JC, JD, JE, JF, JG, JH, JI, JK, JL, JM, JN, JO, JR, JU, KA, KB, KC, KD, KE, KF, KG, KH, KI, KJ, KK, KL, KM, KN, KO, KP, KQ, KR, KS, JJ FROM FAMISIncomeInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_IncomeInformation (CASENUMBER, JP, JQ, JS, JT, JW, JX, KU, KV, JA, JB, JC, JD, JE, JF, JG, JH, JI, JK, JL, JM, JN, JO, JR, JU, KA, KB, KC, KD, KE, KF, KG, KH, KI, KJ, KK, KL, KM, KN, KO, KP, KQ, KR, KS, JJ) VALUES ('" + CASENUMBER + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "', '" + SQLReader.GetString(41) + "', '" + SQLReader.GetString(42) + "', '" + SQLReader.GetString(43) + "', '" + SQLReader.GetString(44) + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, HD, HE, HF, HG, HH, HI, HJ, HK, HL, HM, HN, HO, HP, HQ, HR, HS, HT, HB, HC, WL, WA, WB, WE, WH, WI, WK, WM, WN, WO, WP, WQ, WR, WS, WT, WU, WV, WC, WD, WF, WG FROM FAMISMedicaidInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_MedicaidInformation (CASENUMBER, HD, HE, HF, HG, HH, HI, HJ, HK, HL, HM, HN, HO, HP, HQ, HR, HS, HT, HB, HC, WL, WA, WB, WE, WH, WI, WK, WM, WN, WO, WP, WQ, WR, WS, WT, WU, WV, WC, WD, WF, WG) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetDateTime(37) + "', '" + SQLReader.GetDateTime(38) + "', '" + SQLReader.GetDateTime(39) + "', '" + SQLReader.GetDateTime(40) + "')" ', '" + FAMISApplicationInformation.XQ.getdata() + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, LA, LB, LC, LD, LE, LF, LG, LH, LI, LJ, LK, LL, LM, LO, LP, LQ, LR, LT, MD, OA, OH, OI, ON1, OO, OK, WX, WY, LS FROM FAMISFoodStampInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_FoodStampInformation (CASENUMBER, LA, LB, LC, LD, LE, LF, LG, LH, LI, LJ, LK, LL, LM, LO, LP, LQ, LR, LT, MD, OA, OH, OI, ON1, OO, OK, WX, WY, LS) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetDateTime(3) + "', '" + SQLReader.GetDateTime(4) + "', '" + SQLReader.GetDateTime(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetDateTime(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT MA, MB, MC, ME1, MF, MG, MH, MI, MJ, MK, ML, MM, MN, MO, MP, MQ, MR, NB, OB, OC, OD, OE, OF1, OG, OL, NA, LN FROM FAMISFoodStampInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "UPDATE DAILYHOLD_FoodStampInformation SET MA = '" + SQLReader.GetString(0) + "', MB = '" + SQLReader.GetString(1) + "', MC = '" + SQLReader.GetString(2) + "',  ME1 = '" + SQLReader.GetString(3) + "', MF = '" + SQLReader.GetString(4) + "', MG = '" + SQLReader.GetString(5) + "', MH = '" + SQLReader.GetString(6) + "', MI = '" + SQLReader.GetString(7) + "', MJ = '" + SQLReader.GetString(8) + "', MK = '" + SQLReader.GetString(9) + "', ML = '" + SQLReader.GetString(10) + "', MM = '" + SQLReader.GetString(11) + "', MN = '" + SQLReader.GetString(12) + "', MO = '" + SQLReader.GetString(13) + "', MP = '" + SQLReader.GetString(14) + "', MQ = '" + SQLReader.GetString(15) + "', MR = '" + SQLReader.GetString(16) + "', NB = '" + SQLReader.GetString(17) + "', OB = '" + SQLReader.GetString(18) + "', OC = '" + SQLReader.GetString(19) + "', OD = '" + SQLReader.GetString(20) + "', OE = '" + SQLReader.GetString(21) + "', OF1 = '" + SQLReader.GetString(22) + "', OG = '" + SQLReader.GetString(23) + "', OL = '" + SQLReader.GetString(24) + "', NA = '" + SQLReader.GetString(25) + "', LN = '" + SQLReader.GetString(26) + "' WHERE CASENUMBER = '" + CASENUMBER + "'"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT NB, NC, NE, NF, NG, NH, NI, NJ, NK, NL, NM, NO, NP, ND FROM FAMISFoodStampInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "UPDATE DAILYHOLD_FoodStampInformation SET NB = '" + SQLReader.GetString(0) + "', NC = '" + SQLReader.GetDateTime(1) + "', NE = '" + SQLReader.GetString(2) + "',  NF = '" + SQLReader.GetString(3) + "', NG = '" + SQLReader.GetDateTime(4) + "', NH = '" + SQLReader.GetString(5) + "', NI = '" + SQLReader.GetString(6) + "', NJ = '" + SQLReader.GetString(7) + "', NK = '" + SQLReader.GetString(8) + "', NL = '" + SQLReader.GetString(9) + "', NM = '" + SQLReader.GetString(10) + "', NO = '" + SQLReader.GetString(11) + "', NP = '" + SQLReader.GetString(12) + "', ND = '" + SQLReader.GetString(13) + "' WHERE CASENUMBER = '" + CASENUMBER + "'"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, PA, PB, PC, PD, PF, PG, PI, PJ, PK, PL, PN, PE, PH, PM, PO, PP FROM FAMISIandAInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_IandAInformation (CASENUMBER, PA, PB, PC, PD, PF, PG, PI, PJ, PK, PL, PN, PE, PH, PM, PO, PP) VALUES  ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetDateTime(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetDateTime(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "')"
                    SQLDHCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CaseNumber, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RH2, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR, TA, TB, TI, TJ, TF, TK, TD, UB, RN, RO, RQ, SA, SB, SC, SD, SE, SF, SG, SH, SJ, SK, SL, SM, SO, SP, SI, SS, TG, TH, TL, TM, TO1, TP, TQ, TR, TS, UA, UC, UD, UE, UF, UG, UH, UK, UL, TC, TE, UI FROM FAMISCaseChild WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                While SQLReader.Read()
                    If SQLReader.HasRows Then
                        SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_CaseChild (CaseNumber, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RH2, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR, TA, TB, TI, TJ, TF, TK, TD, UB, RN, RO, RQ, SA, SB, SC, SD, SE, SF,  SG, SH, SJ, SK, SL, SM, SO, SP, SI, SS, TG, TH, TL, TM, TO1, TP, TQ, TR, TS, UA, UC, UD, UE, UF, UG, UH, UK, UL, TC, TE, UI) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetDateTime(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetDateTime(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "', '" + SQLReader.GetString(41) + "', '" + SQLReader.GetString(42) + "', '" + SQLReader.GetString(43) + "', '" + SQLReader.GetString(44) + "', '" + SQLReader.GetString(45) + "', '" + SQLReader.GetString(46) + "', '" + SQLReader.GetString(47) + "', '" + SQLReader.GetString(48) + "', '" + SQLReader.GetString(49) + "', '" + SQLReader.GetString(50) + "', '" + SQLReader.GetString(51) + "', '" + SQLReader.GetString(52) + "', '" + SQLReader.GetString(53) + "', '" + SQLReader.GetString(54) + "', '" + SQLReader.GetString(55) + "', '" + SQLReader.GetString(56) + "', '" + SQLReader.GetString(57) + "', '" + SQLReader.GetString(58) + "', '" + SQLReader.GetString(59) + "', '" + SQLReader.GetString(60) + "', '" + SQLReader.GetString(61) + "', '" + SQLReader.GetString(62) + "', '" + SQLReader.GetString(63) + "', '" + SQLReader.GetString(64) + "', '" + SQLReader.GetDateTime(65) + "', '" + SQLReader.GetString(66) + "', '" + SQLReader.GetString(67) + "', '" + SQLReader.GetString(68) + "', '" + SQLReader.GetString(69) + "', '" + SQLReader.GetString(70) + "', '" + SQLReader.GetString(71) + "', '" + SQLReader.GetString(72) + "', '" + SQLReader.GetString(73) + "', '" + SQLReader.GetString(74) + "', '" + SQLReader.GetString(75) + "', '" + SQLReader.GetString(76) + "', '" + SQLReader.GetString(77) + "', '" + SQLReader.GetString(78) + "', '" + SQLReader.GetString(79) + "', '" + SQLReader.GetString(80) + "', '" + SQLReader.GetString(81) + "', '" + SQLReader.GetDateTime(82) + "', '" + SQLReader.GetString(83) + "')"
                        SQLDHCommand.ExecuteNonQuery()
                    End If
                End While
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT CASENUMBER, VRPNumber, VA, VC, VE, VG, VI, VQ, VK, VM, VO FROM FAMISVRPInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
                SQLReader = SQLCommand.ExecuteReader
                While SQLReader.Read()
                    If SQLReader.HasRows Then
                        SQLDHCommand.CommandText = "INSERT INTO DAILYHOLD_VRPInformation (CASENUMBER, VRPNumber, VA, VC, VE, VG, VI, VQ, VK, VM, VO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "')"
                        SQLDHCommand.ExecuteNonQuery()
                    End If
                End While
                SQLReader.Close()
                'SQLCommand.CommandText = "SELECT CASENUMBER, AQ, MS, MT, MU, NQ, NR, NS, NT, NU, NV, NW, OP, OR, VB, VF, VH, B1, B2, B3, B4, FQ, FR, HU, HV, XO, XP, XQ, XR, XS, XT, XU, XV, QU, RS, RT, ST, TT, YA FROM FAMISAppendedInformation WHERE CaseNumber = '" + CASENUMBER + "'"
                'SQLReader = SQLCommand.ExecuteReader
                'While SQLReader.Read()
                '    If SQLReader.HasRows Then
                '        SQLDHCommand.CommandText = "INSERT INTO FAMISAppendedInformation (CASENUMBER, AQ, MS, MT, MU, NQ, NR, NS, NT, NU, NV, NW, OP, OR, VB, VF, VH, B1, B2, B3, B4, FQ, FR, HU, HV, XO, XP, XQ, XR, XS, XT, XU, XV, QU, RS, RT, ST, TT, YA) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetDateTime(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetDateTime(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "')"
                '        SQLDHCommand.ExecuteNonQuery()
                '    End If
                'End While
            Else
                SQLReader.Close()
            End If

            SQLCommand.CommandText = "DELETE FROM FAMISCASECHILD WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISCASEINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISAFDCINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISAPPLICANTINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISFOODSTAMPINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISIANDAINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISINCOMEINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISINDIVIDUALSINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISMEDICAIDINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISVRPINFORMATION WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISAppendedInformation WHERE CaseNumber = '" & CASENUMBER & "'"

            CheckData()

            SQLCommand.CommandText() = "INSERT INTO FAMISCaseInformation (CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW) VALUES ('" + CASENUMBER + "', '" + SQLCaseInformation.AA.GetData() + "', '" + SQLCaseInformation.AB.GetData() + "', '" + SQLCaseInformation.AC.GetData() + "', '" + SQLCaseInformation.AD.GetData() + "', '" + SQLCaseInformation.AE.GetData() + "', '" + SQLCaseInformation.AF.GetData() + "', '" + SQLCaseInformation.AG.GetData() + "', '" + SQLCaseInformation.AH.GetData() + "', '" + SQLCaseInformation.AI.GetData() + "', '" + SQLCaseInformation.AJ.GetData() + "', '" + ConvertDate(SQLCaseInformation.AK.GetData()) + "', '" + SQLCaseInformation.AL.GetData() + "', '" + SQLCaseInformation.AM.GetData() + "', '" + SQLCaseInformation.AN.GetData() + "', '" + Date.Now.Month.ToString + "/" + Date.Now.Day.ToString + "/" + Date.Now.Year.ToString + "', '" + SQLCaseInformation.P03 + "', '" + SQLCaseInformation.P05 + "', '" + My.Settings.FAMISOperatorID + "', '" + SQLCaseInformation.P09 + "', '" + BATCHNUMBER + "', '" + SQLCaseInformation.P10 + "', '" + SQLMedicaidInformation.WW.GetData() + "')"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISAFDCInformation (CASENUMBER, HA, IA, IB, IC, ID, IE, IF1, IG, IH, II, IJ, IL, IK, IP, OJ, IM, IN1, IO) VALUES ('" + CASENUMBER + "', '" + SQLMedicaidInformation.HA.GetData() + "', '" + SQLTANFInformation.IA.GetData() + "', '" + SQLTANFInformation.IB.GetData() + "', '" + ConvertDate(SQLTANFInformation.IC.GetData()) + "', '" + ConvertDate(SQLTANFInformation.ID.GetData()) + "', '" + SQLTANFInformation.IE.GetData() + "', '" + ConvertDate(SQLTANFInformation.IF1.GetData()) + "', '" + ConvertDate(SQLTANFInformation.IG.GetData()) + "', '" + SQLTANFInformation.IH.GetData() + "', '" + SQLTANFInformation.II.GetData() + "', '" + SQLTANFInformation.IJ.GetData() + "', '" + SQLTANFInformation.IL.GetData() + "', '" + SQLTANFInformation.IK.GetData() + "', '" + SQLTANFInformation.IP.GetData() + "', '" + SQLFoodStampInformation.OJ.GetData() + "',  '" + SQLTANFInformation.IM.GetData() + "', '" + SQLTANFInformation.IN1.GetData() + "', '" + SQLTANFInformation.IO.GetData() + "')"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISApplicantInformation (CASENUMBER, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, CA, CB, CC, CD1, CD2, CE, CF, CG, DA1, DA2, DA3, DB, DC, DD1, DD2, DE, DF) VALUES  ('" + CASENUMBER + "', '" + SQLApplicationInformation.BA.GetData() + "', '" + SQLApplicationInformation.BB.GetData() + "', '" + SQLApplicationInformation.BC.GetData() + "', '" + SQLApplicationInformation.BD.GetData() + "', '" + SQLApplicationInformation.BE.GetData() + "', '" + SQLApplicationInformation.BF.GetData() + "', '" + SQLApplicationInformation.BG.GetData() + "', '" + SQLApplicationInformation.BH.GetData() + "', '" + SQLApplicationInformation.BI.GetData() + "', '" + SQLApplicationInformation.BJ.GetData() + "', '" + SQLApplicationInformation.BK.GetData() + "', '" + SQLApplicationInformation.BL.GetData() + "', '" + SQLApplicationInformation.BM.GetData() + "', '" + SQLApplicationInformation.BN.GetData() + "', '" + SQLApplicationInformation.BO.GetData() + "', '" + SQLApplicationInformation.BP.GetData() + "', '" + SQLApplicationInformation.BQ.GetData() + "', '" + SQLApplicationInformation.CA.GetData().Replace("'", "") + "', ' ', '" + SQLApplicationInformation.CC.GetData() + "',  '" + SQLApplicationInformation.CD1.GetData() + "', '" + SQLApplicationInformation.CD2.GetData() + "', '" + SQLApplicationInformation.CE.GetData() + "', '" + SQLApplicationInformation.CF.GetData() + "', '" + SQLApplicationInformation.CG.GetData() + "', '" + SQLApplicationInformation.DA1.GetData() + "', '" + SQLApplicationInformation.DA2.GetData + "', '" + SQLApplicationInformation.DA3.GetData + "', '" + SQLApplicationInformation.DB.GetData() + "', '" + SQLApplicationInformation.DC.GetData() + "', '" + SQLApplicationInformation.DD1.GetData() + "', '" + SQLApplicationInformation.DD2.GetData() + "', '" + SQLApplicationInformation.DE.GetData() + "', '" + SQLApplicationInformation.DF.GetData() + "')"
            SQLCommand.ExecuteNonQuery()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               '" + SQLApplicationInformation.CA2.getdata() + "

            SQLCommand.CommandText() = "UPDATE FAMISApplicantInformation SET EA = '" + SQLApplicationInformation.EA.GetData() + "', EB = '" + SQLApplicationInformation.EB.GetData() + "', EC = '" + SQLApplicationInformation.EC.GetData() + "', ED1 = '" + SQLApplicationInformation.ED1.GetData() + "', ED2 = '" + SQLApplicationInformation.ED2.GetData() + "', EE = '" + SQLApplicationInformation.EE.GetData() + "', EF = '" + SQLApplicationInformation.EF.GetData() + "', EG = '" + SQLApplicationInformation.EG.GetData() + "', EH = '" + SQLApplicationInformation.EH.GetData() + "', EJ = '" + SQLApplicationInformation.EJ.GetData() + "', NN = '" + SQLFoodStampInformation.NN.GetData() + "', BR = '" + SQLApplicationInformation.BR.GetData() + "', OM = '" + SQLFoodStampInformation.OM.GetData() + "', EI = '" + SQLApplicationInformation.EI.GetData() + "', EK = '" + SQLApplicationInformation.EK.GetData() + "', EL = '" + SQLApplicationInformation.EL.GetData() + "', EM = '" + SQLApplicationInformation.EM.GetData() + "', EN = '" + SQLApplicationInformation.EN.GetData() + "', XA = '" + ConvertDate(SQLApplicationInformation.XA.GetData()) + "', XB = '" + SQLApplicationInformation.XB.GetData() + "', XC = '" + SQLApplicationInformation.XC.GetData() + "', XD = '" + SQLApplicationInformation.XD.GetData() + "', XE = '" + SQLApplicationInformation.XE.GetData() + "', XF = '" + SQLApplicationInformation.XF.GetData() + "',  XG = '" + SQLApplicationInformation.XG.GetData() + "', XH = '" + SQLApplicationInformation.XH.GetData() + "', XI = '" + SQLApplicationInformation.XI.GetData() + "', XJ = '" + SQLApplicationInformation.XJ.GetData() + "', XK = '" + SQLApplicationInformation.XK.GetData() + "', XL = '" + SQLApplicationInformation.XL.GetData() + "', XM = '" + SQLApplicationInformation.XM.GetData() + "', XN = '" + SQLApplicationInformation.XN.GetData() + "' WHERE CASENUMBER = '" + CASENUMBER + "'"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISIndividualsInformation (CASENUMBER, BS, BT, BX, FA, FB, BU, FC, BV, FD, FD2, BW, FE2, FE1, BY1, FF, BZ, FG, FI, FJ, FK, FL, FL2, FM1, FM2, FN, FP, GA, GB, GC, GD, GE, GF, GG, GH, GI, GJ, GK, GL, FH, FO) VALUES ('" + CASENUMBER + "', '" + SQLApplicationInformation.BS.GetData() + "', '" + SQLApplicationInformation.BT.GetData() + "', '" + SQLApplicationInformation.BX.GetData() + "', '" + SQLIndividualsInformation.FA.GetData() + "', '" + ConvertDate(SQLIndividualsInformation.FB.GetData()) + "', '" + SQLApplicationInformation.BU.GetData() + "', '" + SQLIndividualsInformation.FC.GetData() + "', '" + SQLApplicationInformation.BV.GetData() + "', '" + SQLIndividualsInformation.FD.GetData() + "', '" + SQLIndividualsInformation.FD2.GetData() + "', '" + SQLApplicationInformation.BW.GetData() + "', '" + SQLIndividualsInformation.FE2.GetData() + "', '" + SQLIndividualsInformation.FE1.GetData() + "', '" + SQLApplicationInformation.BY1.GetData() + "', '" + SQLIndividualsInformation.FF.GetData() + "', '" + SQLApplicationInformation.BZ.GetData() + "', '" + SQLIndividualsInformation.FG.GetData() + "', '" + SQLIndividualsInformation.FI.GetData() + "', '" + ConvertDate(SQLIndividualsInformation.FJ.GetData()) + "', '" + SQLIndividualsInformation.FK.GetData() + "', '" + SQLIndividualsInformation.FL.GetData() + "', '" + SQLIndividualsInformation.FL2.GetData() + "', '" + SQLIndividualsInformation.FM1.GetData() + "', '" + SQLIndividualsInformation.FM2.GetData() + "', '" + SQLIndividualsInformation.FN.GetData() + "', '" + SQLIndividualsInformation.FP.GetData() + "', '" + SQLIndividualsInformation.GA.GetData() + "', '" + ConvertDate(SQLIndividualsInformation.GB.GetData()) + "', '" + SQLIndividualsInformation.GC.GetData() + "', '" + SQLIndividualsInformation.GD.GetData() + "', '" + SQLIndividualsInformation.GE.GetData() + "', '" + ConvertDate(SQLIndividualsInformation.GF.GetData()) + "', '" + SQLIndividualsInformation.GG.GetData() + "', '" + ConvertDate(SQLIndividualsInformation.GH.GetData()) + "', '" + SQLIndividualsInformation.GI.GetData() + "', '" + SQLIndividualsInformation.GJ.GetData() + "', '" + SQLIndividualsInformation.GK.GetData() + "', '" + ConvertDate(SQLIndividualsInformation.GL.GetData()) + "', '" + SQLIndividualsInformation.FH.GetData() + "', '" + SQLIndividualsInformation.FO.GetData() + "')"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISMedicaidInformation (CASENUMBER, HD, HE, HF, HG, HH, HI, HJ, HK, HL, HM, HN, HO, HP, HQ, HR, HS, HT, HB, HC, WL, WA, WB, WE, WH, WI, WK, WM, WN, WO, WP, WQ, WR, WS, WT, WU, WV, WC, WD, WF, WG) VALUES ('" + CASENUMBER + "', '" + SQLMedicaidInformation.HD.GetData() + "', '" + SQLMedicaidInformation.HE.GetData() + "', '" + SQLMedicaidInformation.HF.GetData() + "', '" + SQLMedicaidInformation.HG.GetData() + "', '" + SQLMedicaidInformation.HH.GetData() + "', '" + SQLMedicaidInformation.HI.GetData() + "', '" + SQLMedicaidInformation.HJ.GetData() + "', '" + SQLMedicaidInformation.HK.GetData() + "', '" + SQLMedicaidInformation.HL.GetData() + "', '" + SQLMedicaidInformation.HM.GetData() + "', '" + SQLMedicaidInformation.HN.GetData() + "', '" + SQLMedicaidInformation.HO.GetData() + "', '" + SQLMedicaidInformation.HP.GetData() + "', '" + SQLMedicaidInformation.HQ.GetData() + "', '" + SQLMedicaidInformation.HR.GetData() + "', '" + SQLMedicaidInformation.HS.GetData() + "', '" + SQLMedicaidInformation.HT.GetData() + "', '" + SQLMedicaidInformation.HB.GetData() + "', '" + SQLMedicaidInformation.HC.GetData() + "', '" + SQLMedicaidInformation.WL.GetData() + "', '" + SQLMedicaidInformation.WA.GetData() + "', '" + SQLMedicaidInformation.WB.GetData() + "', '" + SQLMedicaidInformation.WE.GetData() + "', '" + SQLMedicaidInformation.WH.GetData() + "', '" + SQLMedicaidInformation.WI.GetData() + "', '" + SQLMedicaidInformation.WK.GetData() + "', '" + SQLMedicaidInformation.WM.GetData() + "', '" + SQLMedicaidInformation.WN.GetData() + "', '" + SQLMedicaidInformation.WO.GetData() + "', '" + SQLMedicaidInformation.WP.GetData() + "', '" + SQLMedicaidInformation.WQ.GetData() + "', '" + SQLMedicaidInformation.WR.GetData() + "', '" + SQLMedicaidInformation.WS.GetData() + "', '" + SQLMedicaidInformation.WT.GetData() + "', '" + SQLMedicaidInformation.WU.GetData() + "', '" + SQLMedicaidInformation.WV.GetData() + "', '" + ConvertDate(SQLMedicaidInformation.WC.GetData()) + "', '" + ConvertDate(SQLMedicaidInformation.WD.GetData()) + "', '" + ConvertDate(SQLMedicaidInformation.WF.GetData()) + "', '" + ConvertDate(SQLMedicaidInformation.WG.GetData()) + "')" ', '" + SQLApplicationInformation.XQ.getdata() + "')"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISIncomeInformation (CASENUMBER, JP, JQ, JS, JT, JW, JX, KU, KV, JA, JB, JC, JD, JE, JF, JG, JH, JI, JK, JL, JM, JN, JO, JR, JU, KA, KB, KC, KD, KE, KF, KG, KH, KI, KJ, KK, KL, KM, KN, KO, KP, KQ, KR, KS, JJ) VALUES ('" + CASENUMBER + "', '" + SQLIncomeInformation.JP.GetData() + "', '" + SQLIncomeInformation.JQ.GetData() + "', '" + SQLIncomeInformation.JS.GetData() + "', '" + SQLIncomeInformation.JT.GetData() + "', '" + SQLIncomeInformation.JW.GetData() + "', '" + SQLIncomeInformation.JX.GetData() + "', '" + SQLIncomeInformation.KU.GetData() + "', '" + SQLIncomeInformation.KV.GetData() + "', '" + SQLIncomeInformation.JA.GetData() + "', '" + SQLIncomeInformation.JB.GetData() + "', '" + SQLIncomeInformation.JC.GetData() + "', '" + SQLIncomeInformation.JD.GetData() + "', '" + SQLIncomeInformation.JE.GetData() + "', '" + SQLIncomeInformation.JF.GetData() + "', '" + SQLIncomeInformation.JG.GetData() + "', '" + SQLIncomeInformation.JH.GetData() + "', '" + SQLIncomeInformation.JI.GetData() + "', '" + SQLIncomeInformation.JK.GetData() + "', '" + SQLIncomeInformation.JL.GetData() + "', '" + SQLIncomeInformation.JM.GetData() + "', '" + SQLIncomeInformation.JN.GetData() + "', '" + SQLIncomeInformation.JO.GetData() + "', '" + SQLIncomeInformation.JR.GetData() + "', '" + SQLIncomeInformation.JU.GetData() + "', '" + SQLIncomeInformation.KA.GetData() + "', '" + SQLIncomeInformation.KB.GetData() + "', '" + SQLIncomeInformation.KC.GetData() + "', '" + SQLIncomeInformation.KD.GetData() + "', '" + SQLIncomeInformation.KE.GetData() + "', '" + SQLIncomeInformation.KF.GetData() + "', '" + SQLIncomeInformation.KG.GetData() + "', '" + SQLIncomeInformation.KH.GetData() + "', '" + SQLIncomeInformation.KI.GetData() + "', '" + SQLIncomeInformation.KJ.GetData() + "', '" + SQLIncomeInformation.KK.GetData() + "', '" + SQLIncomeInformation.KL.GetData() + "', '" + SQLIncomeInformation.KM.GetData() + "', '" + SQLIncomeInformation.KN.GetData() + "', '" + SQLIncomeInformation.KO.GetData() + "', '" + SQLIncomeInformation.KP.GetData() + "', '" + SQLIncomeInformation.KQ.GetData() + "', '" + SQLIncomeInformation.KR.GetData() + "', '" + SQLIncomeInformation.KS.GetData() + "', '" + SQLIncomeInformation.JJ.GetData() + "')"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISFoodStampInformation (CASENUMBER, LA, LB, LC, LD, LE, LF, LG, LH, LI, LJ, LK, LL, LM, LO, LP, LQ, LR, LT, MD, OA, OH, OI, ON1, OO, OK, WX, WY, LS) VALUES ('" + CASENUMBER + "', '" + SQLFoodStampInformation.LA.GetData() + "', '" + SQLFoodStampInformation.LB.GetData() + "', '" + ConvertDate(SQLFoodStampInformation.LC.GetData()) + "', '" + ConvertDate(SQLFoodStampInformation.LD.GetData()) + "', '" + ConvertDate(SQLFoodStampInformation.LE.GetData()) + "', '" + SQLFoodStampInformation.LF.GetData() + "', '" + SQLFoodStampInformation.LG.GetData() + "', '" + SQLFoodStampInformation.LH.GetData() + "', '" + SQLFoodStampInformation.LI.GetData() + "', '" + SQLFoodStampInformation.LJ.GetData() + "', '" + SQLFoodStampInformation.LK.GetData() + "', '" + SQLFoodStampInformation.LL.GetData() + "', '" + SQLFoodStampInformation.LM.GetData() + "', '" + SQLFoodStampInformation.LO.GetData() + "', '" + SQLFoodStampInformation.LP.GetData() + "', '" + SQLFoodStampInformation.LQ.GetData() + "', '" + SQLFoodStampInformation.LR.GetData() + "', '" + ConvertDate(SQLFoodStampInformation.LT.GetData()) + "', '" + SQLFoodStampInformation.MD.GetData() + "', '" + SQLFoodStampInformation.OA.GetData() + "', '" + SQLFoodStampInformation.OH.GetData() + "', '" + SQLFoodStampInformation.OI.GetData() + "', '" + SQLFoodStampInformation.ON1.GetData() + "', '" + SQLFoodStampInformation.OO.GetData() + "', '" + SQLFoodStampInformation.OK.GetData() + "', '" + SQLFoodStampInformation.WX.GetData() + "', '" + SQLFoodStampInformation.WY.GetData() + "', '" + SQLFoodStampInformation.LS.GetData() + "')"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "UPDATE FAMISFoodStampInformation SET MA = '" + SQLFoodStampInformation.MA.GetData() + "', MB = '" + SQLFoodStampInformation.MB.GetData() + "', MC = '" + SQLFoodStampInformation.MC.GetData() + "',  ME1 = '" + SQLFoodStampInformation.ME1.GetData() + "', MF = '" + SQLFoodStampInformation.MF.GetData() + "', MG = '" + SQLFoodStampInformation.MG.GetData() + "', MH = '" + SQLFoodStampInformation.MH.GetData() + "', MI = '" + SQLFoodStampInformation.MI.GetData() + "', MJ = '" + SQLFoodStampInformation.MJ.GetData() + "', MK = '" + SQLFoodStampInformation.MK.GetData() + "', ML = '" + SQLFoodStampInformation.ML.GetData() + "', MM = '" + SQLFoodStampInformation.MM.GetData() + "', MN = '" + SQLFoodStampInformation.MN.GetData() + "', MO = '" + SQLFoodStampInformation.MO.GetData() + "', MP = '" + SQLFoodStampInformation.MP.GetData() + "', MQ = '" + SQLFoodStampInformation.MQ.GetData() + "', MR = '" + SQLFoodStampInformation.MR.GetData() + "', NB = '" + SQLFoodStampInformation.NB.GetData() + "', OB = '" + SQLFoodStampInformation.OB.GetData() + "', OC = '" + SQLFoodStampInformation.OC.GetData() + "', OD = '" + SQLFoodStampInformation.OD.GetData() + "', OE = '" + SQLFoodStampInformation.OE.GetData() + "', OF1 = '" + SQLFoodStampInformation.OF1.GetData() + "', OG = '" + SQLFoodStampInformation.OG.GetData() + "', OL = '" + SQLFoodStampInformation.OL.GetData() + "', NA = '" + SQLFoodStampInformation.NA.GetData() + "', LN = '" + SQLFoodStampInformation.LN.GetData() + "' WHERE CASENUMBER = '" + CASENUMBER + "'"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "UPDATE FAMISFoodStampInformation SET NA = '" + SQLFoodStampInformation.NA.GetData() + "', NB = '" + SQLFoodStampInformation.NB.GetData() + "', NC = '" + ConvertDate(SQLFoodStampInformation.NC.GetData()) + "', ND = '" + SQLFoodStampInformation.ND.GetData() + "', NE = '" + SQLFoodStampInformation.NE.GetData() + "', NF = '" + SQLFoodStampInformation.NF.GetData() + "', NG = '" + ConvertDate(SQLFoodStampInformation.NG.GetData()) + "', NH = '" + SQLFoodStampInformation.NH.GetData() + "', NI = '" + SQLFoodStampInformation.NI.GetData() + "',  NJ = '" + SQLFoodStampInformation.NJ.GetData() + "', NK = '" + SQLFoodStampInformation.NK.GetData() + "', NL = '" + SQLFoodStampInformation.NL.GetData() + "', NM = '" + SQLFoodStampInformation.NM.GetData() + "', NO = '" + SQLFoodStampInformation.NO.GetData() + "', NP = '" + SQLFoodStampInformation.NP.GetData() + "' WHERE CASENUMBER = '" + CASENUMBER + "'"
            SQLCommand.ExecuteNonQuery()

            SQLCommand.CommandText() = "INSERT INTO FAMISIandAInformation (CASENUMBER, PA, PB, PC, PD, PF, PG, PI, PJ, PK, PL, PN, PE, PH, PM, PO, PP) VALUES  ('" + CASENUMBER + "', '" + ConvertDate(SQLIandAInformation.PA.GetData()) + "', '" + SQLIandAInformation.PB.GetData() + "', '" + SQLIandAInformation.PC.GetData() + "', '" + SQLIandAInformation.PD.GetData() + "', '" + SQLIandAInformation.PF.GetData() + "', '" + SQLIandAInformation.PG.GetData() + "', '" + ConvertDate(SQLIandAInformation.PI.GetData()) + "', '" + SQLIandAInformation.PJ.GetData() + "', '" + SQLIandAInformation.PK.GetData() + "', '" + SQLIandAInformation.PL.GetData() + "', '" + SQLIandAInformation.PN.GetData() + "', '" + SQLIandAInformation.PE.GetData() + "', '" + ConvertDate(SQLIandAInformation.PH.GetData()) + "', '" + SQLIandAInformation.PM.GetData() + "', '" + SQLIandAInformation.PO.GetData() + "', '" + SQLIandAInformation.PP.GetData() + "')"
            SQLCommand.ExecuteNonQuery()

            If numChildren > 0 Then
                For i = 0 To numChildren - 1
                    SQLCommand.CommandText() = "INSERT INTO FAMISCaseChild (CASENUMBER, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RH2, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR) VALUES ('" + CASENUMBER + "', '" + SQLCaseChild(i).QA.GetData() + "', '" + SQLCaseChild(i).QB.GetData().Replace("'", " ") + "', '" + SQLCaseChild(i).QC.GetData().Replace("'", " ") + "', '" + SQLCaseChild(i).QD.GetData() + "', '" + SQLCaseChild(i).QE.GetData() + "', '" + SQLCaseChild(i).QF.GetData() + "', '" + ConvertDate(SQLCaseChild(i).QG.GetData()) + "', '" + SQLCaseChild(i).QH.GetData() + "', '" + SQLCaseChild(i).QI.GetData() + "', '" + SQLCaseChild(i).QJ.GetData() + "', '" + ConvertDate(SQLCaseChild(i).QK.GetData()) + "', '" + SQLCaseChild(i).QL.GetData() + "', '" + SQLCaseChild(i).QM.GetData() + "', '" + SQLCaseChild(i).QN.GetData() + "', '" + SQLCaseChild(i).QO.GetData() + "', '" + SQLCaseChild(i).RA.GetData() + "', '" + SQLCaseChild(i).RB.GetData() + "', '" + SQLCaseChild(i).RC.GetData() + "', '" + SQLCaseChild(i).RD.GetData() + "', '" + SQLCaseChild(i).RE.GetData() + "', '" + SQLCaseChild(i).RF.GetData() + "', '" + SQLCaseChild(i).RG.GetData() + "', '" + SQLCaseChild(i).RH.GetData() + "', '" + SQLCaseChild(i).RH2.GetData + "', '" + SQLCaseChild(i).RI.GetData() + "', '" + SQLCaseChild(i).RJ1.GetData() + "', '" + SQLCaseChild(i).RJ2.GetData() + "', '" + SQLCaseChild(i).RK.GetData() + "', '" + SQLCaseChild(i).RL.GetData() + "', '" + SQLCaseChild(i).RM.GetData() + "', '" + SQLCaseChild(i).RP.GetData() + "', '" + SQLCaseChild(i).RR.GetData() + "', '" + ConvertDate(SQLCaseChild(i).SN.GetData()) + "', '" + SQLCaseChild(i).SQ.GetData() + "', '" + SQLCaseChild(i).SR.GetData() + "')"
                    SQLCommand.ExecuteNonQuery()

                    If Not isSecurityAvailable Then
                        SQLCommand.CommandText() = "UPDATE FAMISCaseChild SET TA = '" + SQLCaseChild(i).TA.GetData() + "', TB = '" + SQLCaseChild(i).TB.GetData() + "', TI = '" + SQLCaseChild(i).TI.GetData() + "', TJ = '" + SQLCaseChild(i).TJ.GetData() + "', TF = '" + SQLCaseChild(i).TF.GetData() + "', TK = '" + SQLCaseChild(i).TK.GetData() + "', TD = '" + SQLCaseChild(i).TD.GetData() + "', UB = '" + SQLCaseChild(i).UB.GetData() + "', RN = '" + SQLCaseChild(i).RN.GetData() + "', RO = '" + SQLCaseChild(i).RO.GetData() + "', RQ = '" + SQLCaseChild(i).RQ.GetData() + "', SA = '" + SQLCaseChild(i).SA.GetData() + "', SB = '" + SQLCaseChild(i).SB.GetData() + "', SC = '" + SQLCaseChild(i).SC.GetData() + "', SD = '" + SQLCaseChild(i).SD.GetData() + "', SE = '" + SQLCaseChild(i).SE.GetData() + "', SF = '" + SQLCaseChild(i).SF.GetData() + "', SG = '" + SQLCaseChild(i).SG.GetData() + "', SH = '" + SQLCaseChild(i).SH.GetData() + "', SJ = '" + SQLCaseChild(i).SJ.GetData() + "', SK = '" + SQLCaseChild(i).SK.GetData() + "', SL = '" + SQLCaseChild(i).SL.GetData() + "', SM = '" + SQLCaseChild(i).SM.GetData() + "', SO = '" + SQLCaseChild(i).SO.GetData() + "', SP = '" + SQLCaseChild(i).SP.GetData() + "', SI = '" + SQLCaseChild(i).SI.GetData() + "', SS = '" + SQLCaseChild(i).SS.GetData() + "', TG = '" + SQLCaseChild(i).TG.GetData() + "', TH = '" + SQLCaseChild(i).TH.GetData() + "', TL = '" + ConvertDate(SQLCaseChild(i).TL.GetData()) + "', TM = '" + SQLCaseChild(i).TM.GetData() + "', TO1 = '" + SQLCaseChild(i).TO1.GetData() + "', TP = '" + SQLCaseChild(i).TP.GetData() + "', TQ = '" + SQLCaseChild(i).TQ.GetData() + "', TR = '" + SQLCaseChild(i).TR.GetData() + "', TS = '" + SQLCaseChild(i).TS.GetData() + "', UA = '" + SQLCaseChild(i).UA.GetData() + "', UC = '" + SQLCaseChild(i).UC.GetData() + "', UD = '" + SQLCaseChild(i).UD.GetData() + "', UE = '" + SQLCaseChild(i).UE.GetData() + "', UF = '" + SQLCaseChild(i).UF.GetData() + "', UG= '" + SQLCaseChild(i).UG.GetData() + "', UH = '" + SQLCaseChild(i).UH.GetData() + "', UK = '" + SQLCaseChild(i).UK.GetData() + "', UL = '" + SQLCaseChild(i).UL.GetData() + "', TC = '" + SQLCaseChild(i).TC.GetData() + "', TE = '" + ConvertDate(SQLCaseChild(i).TE.GetData()) + "', UI = '" + SQLCaseChild(i).UI.GetData() + "', QU = '" + SQLCaseChild(i).QU.GetData() + "', RS = '" + SQLCaseChild(i).RS.GetData + "', RT = '" + SQLCaseChild(i).RT.GetData + "', ST = '" + SQLCaseChild(i).ST.GetData + "', TT = '" + SQLCaseChild(i).TT.GetData + "', YA = '" + SQLCaseChild(i).YA.GetData() + "'  WHERE CASENUMBER = '" + CASENUMBER + "' AND QA = '" + SQLCaseChild(i).QA.GetData() + "'"
                    Else
                        SQLCommand.CommandText() = "UPDATE FAMISCaseChild SET TA = '" + SQLCaseChild(i).TA.GetData() + "', TB = '" + SQLCaseChild(i).TB.GetData() + "', TI = '" + SQLCaseChild(i).TI.GetData() + "', TJ = '" + SQLCaseChild(i).TJ.GetData() + "', TF = '" + SQLCaseChild(i).TF.GetData() + "', TK = '" + SQLCaseChild(i).TK.GetData() + "', TD = '" + SQLCaseChild(i).TD.GetData() + "', UB = '" + SQLCaseChild(i).UB.GetData() + "', RN = '" + SQLCaseChild(i).RN.GetData() + "', RO = '" + SQLCaseChild(i).RO.GetData() + "', RQ = '" + SQLCaseChild(i).RQ.GetData() + "', SA = '" + SQLCaseChild(i).SA.GetData() + "', SB = '" + SQLCaseChild(i).SB.GetData() + "', SC = '" + SQLCaseChild(i).SC.GetData() + "', SD = '" + SQLCaseChild(i).SD.GetData() + "', SE = '" + SQLCaseChild(i).SE.GetData() + "', SF = '" + SQLCaseChild(i).SF.GetData() + "', SG = '" + SQLCaseChild(i).SG.GetData() + "', SH = '" + SQLCaseChild(i).SH.GetData() + "', SJ = '" + SQLCaseChild(i).SJ.GetData() + "', SK = '" + SQLCaseChild(i).SK.GetData() + "', SL = '" + SQLCaseChild(i).SL.GetData() + "', SM = '" + SQLCaseChild(i).SM.GetData() + "', SO = '" + SQLCaseChild(i).SO.GetData() + "', SP = '" + SQLCaseChild(i).SP.GetData() + "', SI = '" + SQLCaseChild(i).SI.GetData() + "', SS = '" + SQLCaseChild(i).SS.GetData() + "', TG = '" + SQLCaseChild(i).TG.GetData() + "', TH = '" + SQLCaseChild(i).TH.GetData() + "', TL = '" + ConvertDate(SQLCaseChild(i).TL.GetData()) + "', TM = '" + SQLCaseChild(i).TM.GetData() + "', TO1 = '" + SQLCaseChild(i).TO1.GetData() + "', TP = '" + SQLCaseChild(i).TP.GetData() + "', TQ = '" + SQLCaseChild(i).TQ.GetData() + "', TR = '" + SQLCaseChild(i).TR.GetData() + "', TS = '" + SQLCaseChild(i).TS.GetData() + "', UA = '" + SQLCaseChild(i).UA.GetData() + "', UC = '" + SQLCaseChild(i).UC.GetData() + "', UD = '" + SQLCaseChild(i).UD.GetData() + "', UE = '" + SQLCaseChild(i).UE.GetData() + "', UF = '" + SQLCaseChild(i).UF.GetData() + "', UG= '" + SQLCaseChild(i).UG.GetData() + "', UH = '" + SQLCaseChild(i).UH.GetData() + "', UK = '" + SQLCaseChild(i).UK.GetData() + "', UL = '" + SQLCaseChild(i).UL.GetData() + "', TC = '" + SQLCaseChild(i).TC.GetData() + "', TE = '" + ConvertDate(SQLCaseChild(i).TE.GetData()) + "', UI = '" + SQLCaseChild(i).UI.GetData() + "', QU = '" + SQLCaseChild(i).QU.GetData() + "', RS = '" + SQLCaseChild(i).RS.GetData + "', RT = '" + SQLCaseChild(i).RT.GetData + "', ST = '" + SQLCaseChild(i).ST.GetData + "', TT = '" + SQLCaseChild(i).TT.GetData + "', TU = '" + SQLCaseChild(i).TU.GetData + "', YA = '" + SQLCaseChild(i).YA.GetData() + "'  WHERE CASENUMBER = '" + CASENUMBER + "' AND QA = '" + SQLCaseChild(i).QA.GetData() + "'"
                    End If
                    SQLCommand.ExecuteNonQuery()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ', YA = '" + YA(z).getdata() + "'
                Next
            End If

            If numVRP > 0 Then
                For i = 0 To numVRP - 1
                    SQLCommand.CommandText() = "INSERT INTO FAMISVRPInformation (CASENUMBER, VRPNumber, VA, VC, VE, VG, VI, VQ, VK, VM, VO) VALUES ('" + CASENUMBER + "', '" + i.ToString.PadLeft(2, "0") + "', '" + SQLVRPInformation(i).VA.GetData() + "', '" + SQLVRPInformation(i).VC.GetData() + "', '" + SQLVRPInformation(i).VE.GetData() + "', '" + SQLVRPInformation(i).VG.GetData() + "', '" + SQLVRPInformation(i).VI.GetData() + "', '" + SQLVRPInformation(i).VQ.GetData() + "', '" + SQLVRPInformation(i).VK.GetData() + "', '" + SQLVRPInformation(i).VM.GetData() + "', '" + SQLVRPInformation(i).VO.GetData() + "')"
                    SQLCommand.ExecuteNonQuery()
                Next
            End If

            If Not isSecurityAvailable Then
                SQLCommand.CommandText = "INSERT INTO FAMISAppendedInformation (CASENUMBER, AQ, MS, MT, MU, NQ, NR, NS, NT, NU, NV, NW, OP, [OR], VB, VF, VH, B1, B2, B3, B4, FQ, FR, HU, HV, XO, XP, XQ, XR, XS, XT, XU, XV) VALUES ('" + CASENUMBER + "', '" + SQLCaseInformation.AQ.GetData + "', '" + SQLFoodStampInformation.MQ.GetData + "', '" + SQLFoodStampInformation.MT.GetData + "', '" + SQLFoodStampInformation.MU.GetData + "', '" + SQLFoodStampInformation.NQ.GetData + "', '" + SQLFoodStampInformation.NR.GetData + "', '" + SQLFoodStampInformation.NS.GetData + "', '" + SQLFoodStampInformation.NT.GetData + "', '" + SQLFoodStampInformation.NU.GetData + "', '" + SQLFoodStampInformation.NV.GetData + "', '" + SQLFoodStampInformation.NW.GetData + "', '" + SQLFoodStampInformation.OP.GetData + "', '" + SQLFoodStampInformation.OR.GetData + "', '" + SQLFoodStampInformation.VB.GetData + "', '" + SQLFoodStampInformation.VF.GetData + "', '" + SQLFoodStampInformation.VH.GetData + "', '" + SQLIndividualsInformation.B1.GetData + "', '" + SQLIndividualsInformation.B2.GetData + "', '" + SQLIndividualsInformation.B3.GetData + "', '" + SQLIndividualsInformation.B4.GetData + "', '" + SQLIndividualsInformation.FQ.GetData + "', '" + SQLIndividualsInformation.FR.GetData + "', '" + ConvertDate(SQLMedicaidInformation.HU.GetData) + "', '" + ConvertDate(SQLMedicaidInformation.HV.GetData) + "', '" + SQLMedicaidInformation.XO.GetData + "', '" + SQLMedicaidInformation.XP.GetData + "', '" + SQLMedicaidInformation.XQ.GetData + "', '" + SQLMedicaidInformation.XR.GetData + "', '" + SQLMedicaidInformation.XS.GetData + "', '" + SQLMedicaidInformation.XT.GetData + "', '" + SQLMedicaidInformation.XU.GetData + "', '" + SQLMedicaidInformation.XV.GetData + "')"
            Else
                SQLCommand.CommandText = "INSERT INTO FAMISAppendedInformation (CASENUMBER, AQ, MS, MT, MU, NQ, NR, NS, NT, NU, NV, NW, OP, [OR], VB, VF, VH, B1, B2, B3, B4, FQ, FR, HU, HV, XO, XP, XQ, XR, XS, XT, XU, XV, B5, B6) VALUES ('" + CASENUMBER + "', '" + SQLCaseInformation.AQ.GetData + "', '" + SQLFoodStampInformation.MQ.GetData + "', '" + SQLFoodStampInformation.MT.GetData + "', '" + SQLFoodStampInformation.MU.GetData + "', '" + SQLFoodStampInformation.NQ.GetData + "', '" + SQLFoodStampInformation.NR.GetData + "', '" + SQLFoodStampInformation.NS.GetData + "', '" + SQLFoodStampInformation.NT.GetData + "', '" + SQLFoodStampInformation.NU.GetData + "', '" + SQLFoodStampInformation.NV.GetData + "', '" + SQLFoodStampInformation.NW.GetData + "', '" + SQLFoodStampInformation.OP.GetData + "', '" + SQLFoodStampInformation.OR.GetData + "', '" + SQLFoodStampInformation.VB.GetData + "', '" + SQLFoodStampInformation.VF.GetData + "', '" + SQLFoodStampInformation.VH.GetData + "', '" + SQLIndividualsInformation.B1.GetData + "', '" + SQLIndividualsInformation.B2.GetData + "', '" + SQLIndividualsInformation.B3.GetData + "', '" + SQLIndividualsInformation.B4.GetData + "', '" + SQLIndividualsInformation.FQ.GetData + "', '" + SQLIndividualsInformation.FR.GetData + "', '" + ConvertDate(SQLMedicaidInformation.HU.GetData) + "', '" + ConvertDate(SQLMedicaidInformation.HV.GetData) + "', '" + SQLMedicaidInformation.XO.GetData + "', '" + SQLMedicaidInformation.XP.GetData + "', '" + SQLMedicaidInformation.XQ.GetData + "', '" + SQLMedicaidInformation.XR.GetData + "', '" + SQLMedicaidInformation.XS.GetData + "', '" + SQLMedicaidInformation.XT.GetData + "', '" + SQLMedicaidInformation.XU.GetData + "', '" + SQLMedicaidInformation.XV.GetData + "', '" + SQLIndividualsInformation.B5.GetData + "', '" + SQLIndividualsInformation.B6.GetData + "')"
            End If
            SQLCommand.ExecuteNonQuery()

            If isSecurityAvailable Then
                SQLCommand.CommandText = "SELECT * FROM OnlineUsers WHERE Operator = '" & My.Settings.FAMISOperatorID & "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    tempCases = SQLReader.GetInt32(3)
                    SQLDHCommand.CommandText = "UPDATE OnlineUsers SET Status = 'Online', CheckInTime = '" & Date.Now & "', CasesDone = " & tempCases + 1 & " WHERE Operator = '" & My.Settings.FAMISOperatorID & "'"
                    SQLDHCommand.ExecuteNonQuery()
                Else
                    SQLDHCommand.CommandText = "INSERT INTO OnlineUsers VALUES ('" & My.Settings.FAMISOperatorID & "', 'Online', '" & Date.Now & "', 1)"
                    SQLDHCommand.ExecuteNonQuery()
                End If
            End If
        Catch SQLex As SqlTypes.SqlNullValueException
            SQLReader.Close()
            SQLCommand.CommandText = "DELETE FROM FAMISCaseChild WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISCaseInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISAFDCInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISApplicantInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISFoodStampInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISIandAInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISIncomeInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISIndividualsInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISMedicaidInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISVRPInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM FAMISAppendedInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_CaseChild WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_CaseInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_AFDCInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_ApplicantInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_FoodStampInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_IandAInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_IncomeInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_IndividualsInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_MedicaidInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_VRPInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            SQLCommand.CommandText = "DELETE FROM DAILYHOLD_AppendedInformation WHERE CaseNumber = '" & CASENUMBER & "'"
            SQLCommand.ExecuteNonQuery()
            ParentForm_Put105.isCaseError = True
        Catch ex As Exception
            MessageBox.Show("Error!" & vbCrLf & "'" & ex.Message.ToString & "'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ParentForm_Put105.WriteLog("SQL Error " & CASENUMBER & " - " & ex.Message.ToString, True)
            ParentForm_Put105.isCaseError = True
        Finally
            SQLConn.Close()
            SQLDHConn.Close()
        End Try
    End Sub
    Private Function ConvertDate(ByVal tempDate As String) As String
        If tempDate = "        " Or tempDate = "00000000  " Or tempDate = "          " Or tempDate = "00000000" Then
            Return "        "
        ElseIf tempDate.Substring(0, 1) = "-" Then
            Return "        "
        Else
            tempDate = tempDate.Insert(2, "/").Insert(5, "/")
            Return tempDate
        End If
        Return -1
    End Function
    Private Function isValidDate(ByVal tempDate As String) As Boolean
        Dim dateCompare As Date
        Try
            If tempDate <> "        " And tempDate <> "          " Then
                dateCompare = tempDate.Insert(2, "/").Insert(5, "/")
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Sub VerifySQL()
        Dim SQLvConn As New SqlConnection(My.Settings.phxSQLConn)
        Dim SQLvComm As New SqlCommand
        Dim TestDate As Date
        SQLvComm.Connection = SQLvConn
        If SQLResubmitCounter < 3 Then
            BGW_ProcessFAMIS.ReportProgress(19)
            Thread.Sleep(750)
            Try
                SQLvConn.Open()
                SQLvComm.CommandText = "SELECT CaseNumber, DateEntered FROM FAMISCaseInformation WHERE CaseNumber = '" & CASENUMBER & "'"
                SQLReader = SQLvComm.ExecuteReader
                If SQLReader.Read Then
                    TestDate = SQLReader.GetDateTime(1)
                    If TestDate.Month = Date.Now.Month And TestDate.Day = Date.Now.Day And TestDate.Year = Date.Now.Year Then
                        SQLvConn.Close()
                        Thread.Sleep(750)
                    Else
                        '--Report case not sent to SQL server--
                        BGW_ProcessFAMIS.ReportProgress(18)
                        SQLResubmitCounter += 1
                        SQLvConn.Close()
                        Thread.Sleep(750)
                        StoreSQL()
                        VerifySQL()
                    End If
                Else
                    '--Report case not sent to SQL server--
                    BGW_ProcessFAMIS.ReportProgress(18)
                    SQLResubmitCounter += 1
                    SQLvConn.Close()
                    Thread.Sleep(750)
                    StoreSQL()
                    VerifySQL()
                End If
            Catch ex As Exception
                MessageBox.Show("Location: Verify Case" & vbCrLf & ex.Message)
                ParentForm_Put105.WriteLog("Case not sent to SQL Server. Error: " & ex.Message.ToString, True)
                If SQLvConn.State = ConnectionState.Open Then SQLConn.Close()
            End Try
        Else
            If SQLvConn.State = ConnectionState.Open Then SQLvConn.Close()
            Thread.Sleep(250)
            If SQLResubmitCounter = 3 Then
                SQLResubmitCounter += 1
                MessageBox.Show("Case cannot be backed up to database" & vbCrLf & "and will be cancelled!", "Phoenix - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                BGW_ProcessFAMIS.ReportProgress(97)
                CancelCase()
            End If
        End If
    End Sub
    Private Sub CheckData()
        Dim i As Integer
        BlockCompare(FAMISCaseInformation.AA, SQLCaseInformation.AA)
        BlockCompare(FAMISCaseInformation.AB, SQLCaseInformation.AB)
        BlockCompare(FAMISCaseInformation.AC, SQLCaseInformation.AC)
        BlockCompare(FAMISCaseInformation.AD, SQLCaseInformation.AD)
        BlockCompare(FAMISCaseInformation.AE, SQLCaseInformation.AE)
        BlockCompare(FAMISCaseInformation.AF, SQLCaseInformation.AF)
        BlockCompare(FAMISCaseInformation.AG, SQLCaseInformation.AG)
        BlockCompare(FAMISCaseInformation.AH, SQLCaseInformation.AH)
        BlockCompare(FAMISCaseInformation.AI, SQLCaseInformation.AI)
        BlockCompare(FAMISCaseInformation.AJ, SQLCaseInformation.AJ)
        BlockCompare(FAMISCaseInformation.AK, SQLCaseInformation.AK)
        BlockCompare(FAMISCaseInformation.AL, SQLCaseInformation.AL)
        BlockCompare(FAMISCaseInformation.AM, SQLCaseInformation.AM)
        BlockCompare(FAMISCaseInformation.AN, SQLCaseInformation.AN)

        BlockCompare(FAMISApplicationInformation.BA, SQLApplicationInformation.BA)
        BlockCompare(FAMISApplicationInformation.BB, SQLApplicationInformation.BB)
        BlockCompare(FAMISApplicationInformation.BC, SQLApplicationInformation.BC)
        BlockCompare(FAMISApplicationInformation.BD, SQLApplicationInformation.BD)
        BlockCompare(FAMISApplicationInformation.BE, SQLApplicationInformation.BE)
        BlockCompare(FAMISApplicationInformation.BF, SQLApplicationInformation.BF)
        BlockCompare(FAMISApplicationInformation.BG, SQLApplicationInformation.BG)
        BlockCompare(FAMISApplicationInformation.BH, SQLApplicationInformation.BH)
        BlockCompare(FAMISApplicationInformation.BI, SQLApplicationInformation.BI)
        BlockCompare(FAMISApplicationInformation.BJ, SQLApplicationInformation.BJ)
        BlockCompare(FAMISApplicationInformation.BK, SQLApplicationInformation.BK)
        BlockCompare(FAMISApplicationInformation.BL, SQLApplicationInformation.BL)
        BlockCompare(FAMISApplicationInformation.BM, SQLApplicationInformation.BM)
        BlockCompare(FAMISApplicationInformation.BN, SQLApplicationInformation.BN)
        BlockCompare(FAMISApplicationInformation.BO, SQLApplicationInformation.BO)
        BlockCompare(FAMISApplicationInformation.BP, SQLApplicationInformation.BP)
        BlockCompare(FAMISApplicationInformation.BQ, SQLApplicationInformation.BQ)
        BlockCompare(FAMISApplicationInformation.BR, SQLApplicationInformation.BR)
        BlockCompare(FAMISApplicationInformation.BS, SQLApplicationInformation.BS)
        BlockCompare(FAMISApplicationInformation.BT, SQLApplicationInformation.BT)
        BlockCompare(FAMISApplicationInformation.BU, SQLApplicationInformation.BU)
        BlockCompare(FAMISApplicationInformation.BV, SQLApplicationInformation.BV)
        BlockCompare(FAMISApplicationInformation.BW, SQLApplicationInformation.BW)
        BlockCompare(FAMISApplicationInformation.BX, SQLApplicationInformation.BX)
        BlockCompare(FAMISApplicationInformation.BY1, SQLApplicationInformation.BY1)
        BlockCompare(FAMISApplicationInformation.BZ, SQLApplicationInformation.BZ)

        BlockCompare(FAMISApplicationInformation.CA, SQLApplicationInformation.CA)
        '"        'blockcompare	(FAMISApplicationInformation.CB)		,SQLApplicationInformation.CB)					"	
        BlockCompare(FAMISApplicationInformation.CB, SQLApplicationInformation.CB)
        BlockCompare(FAMISApplicationInformation.CC, SQLApplicationInformation.CC)
        BlockCompare(FAMISApplicationInformation.CD1, SQLApplicationInformation.CD1)
        BlockCompare(FAMISApplicationInformation.CD2, SQLApplicationInformation.CD2)
        BlockCompare(FAMISApplicationInformation.CE, SQLApplicationInformation.CE)
        BlockCompare(FAMISApplicationInformation.CF, SQLApplicationInformation.CF)
        BlockCompare(FAMISApplicationInformation.CG, SQLApplicationInformation.CG)

        BlockCompare(FAMISApplicationInformation.DA1, SQLApplicationInformation.DA1)
        BlockCompare(FAMISApplicationInformation.DA2, SQLApplicationInformation.DA2)
        BlockCompare(FAMISApplicationInformation.DA3, SQLApplicationInformation.DA3)
        BlockCompare(FAMISApplicationInformation.DB, SQLApplicationInformation.DB)
        BlockCompare(FAMISApplicationInformation.DC, SQLApplicationInformation.DC)
        BlockCompare(FAMISApplicationInformation.DD1, SQLApplicationInformation.DD1)
        BlockCompare(FAMISApplicationInformation.DD2, SQLApplicationInformation.DD2)
        BlockCompare(FAMISApplicationInformation.DE, SQLApplicationInformation.DE)
        BlockCompare(FAMISApplicationInformation.DF, SQLApplicationInformation.DF)

        BlockCompare(FAMISApplicationInformation.EA, SQLApplicationInformation.EA)
        BlockCompare(FAMISApplicationInformation.EB, SQLApplicationInformation.EB)
        BlockCompare(FAMISApplicationInformation.EC, SQLApplicationInformation.EC)
        BlockCompare(FAMISApplicationInformation.ED1, SQLApplicationInformation.ED1)
        BlockCompare(FAMISApplicationInformation.ED2, SQLApplicationInformation.ED2)
        BlockCompare(FAMISApplicationInformation.EE, SQLApplicationInformation.EE)
        BlockCompare(FAMISApplicationInformation.EF, SQLApplicationInformation.EF)
        BlockCompare(FAMISApplicationInformation.EG, SQLApplicationInformation.EG)
        BlockCompare(FAMISApplicationInformation.EH, SQLApplicationInformation.EH)
        BlockCompare(FAMISApplicationInformation.EI, SQLApplicationInformation.EI)
        BlockCompare(FAMISApplicationInformation.EJ, SQLApplicationInformation.EJ)
        BlockCompare(FAMISApplicationInformation.EK, SQLApplicationInformation.EK)
        BlockCompare(FAMISApplicationInformation.EL, SQLApplicationInformation.EL)
        BlockCompare(FAMISApplicationInformation.EM, SQLApplicationInformation.EM)
        BlockCompare(FAMISApplicationInformation.EN, SQLApplicationInformation.EN)

        BlockCompare(FAMISApplicationInformation.XA, SQLApplicationInformation.XA)
        BlockCompare(FAMISApplicationInformation.XB, SQLApplicationInformation.XB)
        BlockCompare(FAMISApplicationInformation.XC, SQLApplicationInformation.XC)
        BlockCompare(FAMISApplicationInformation.XD, SQLApplicationInformation.XD)
        BlockCompare(FAMISApplicationInformation.XE, SQLApplicationInformation.XE)
        BlockCompare(FAMISApplicationInformation.XF, SQLApplicationInformation.XF)
        BlockCompare(FAMISApplicationInformation.XG, SQLApplicationInformation.XG)
        BlockCompare(FAMISApplicationInformation.XH, SQLApplicationInformation.XH)
        BlockCompare(FAMISApplicationInformation.XI, SQLApplicationInformation.XI)
        BlockCompare(FAMISApplicationInformation.XJ, SQLApplicationInformation.XJ)
        BlockCompare(FAMISApplicationInformation.XK, SQLApplicationInformation.XK)
        BlockCompare(FAMISApplicationInformation.XL, SQLApplicationInformation.XL)
        BlockCompare(FAMISApplicationInformation.XM, SQLApplicationInformation.XM)
        BlockCompare(FAMISApplicationInformation.XN, SQLApplicationInformation.XN)

        BlockCompare(FAMISIndividualsInformation.FA, SQLIndividualsInformation.FA)
        BlockCompare(FAMISIndividualsInformation.FB, SQLIndividualsInformation.FB)
        BlockCompare(FAMISIndividualsInformation.FC, SQLIndividualsInformation.FC)
        BlockCompare(FAMISIndividualsInformation.FD, SQLIndividualsInformation.FD)
        BlockCompare(FAMISIndividualsInformation.FD2, SQLIndividualsInformation.FD2)
        BlockCompare(FAMISIndividualsInformation.FE1, SQLIndividualsInformation.FE1)
        BlockCompare(FAMISIndividualsInformation.FE2, SQLIndividualsInformation.FE2)
        BlockCompare(FAMISIndividualsInformation.FF, SQLIndividualsInformation.FF)
        BlockCompare(FAMISIndividualsInformation.FG, SQLIndividualsInformation.FG)
        BlockCompare(FAMISIndividualsInformation.FH, SQLIndividualsInformation.FH)
        BlockCompare(FAMISIndividualsInformation.FI, SQLIndividualsInformation.FI)
        BlockCompare(FAMISIndividualsInformation.FK, SQLIndividualsInformation.FK)
        BlockCompare(FAMISIndividualsInformation.FJ, SQLIndividualsInformation.FJ)
        BlockCompare(FAMISIndividualsInformation.FL, SQLIndividualsInformation.FL)
        BlockCompare(FAMISIndividualsInformation.FL2, SQLIndividualsInformation.FL2)
        BlockCompare(FAMISIndividualsInformation.FM1, SQLIndividualsInformation.FM1)
        BlockCompare(FAMISIndividualsInformation.FM2, SQLIndividualsInformation.FM2)
        BlockCompare(FAMISIndividualsInformation.FN, SQLIndividualsInformation.FN)
        BlockCompare(FAMISIndividualsInformation.FO, SQLIndividualsInformation.FO)
        BlockCompare(FAMISIndividualsInformation.FP, SQLIndividualsInformation.FP)

        BlockCompare(FAMISIndividualsInformation.GA, SQLIndividualsInformation.GA)
        BlockCompare(FAMISIndividualsInformation.GB, SQLIndividualsInformation.GB)
        BlockCompare(FAMISIndividualsInformation.GC, SQLIndividualsInformation.GC)
        BlockCompare(FAMISIndividualsInformation.GD, SQLIndividualsInformation.GD)
        BlockCompare(FAMISIndividualsInformation.GE, SQLIndividualsInformation.GE)
        BlockCompare(FAMISIndividualsInformation.GF, SQLIndividualsInformation.GF)
        BlockCompare(FAMISIndividualsInformation.GG, SQLIndividualsInformation.GG)
        BlockCompare(FAMISIndividualsInformation.GH, SQLIndividualsInformation.GH)
        BlockCompare(FAMISIndividualsInformation.GI, SQLIndividualsInformation.GI)
        BlockCompare(FAMISIndividualsInformation.GJ, SQLIndividualsInformation.GJ)
        BlockCompare(FAMISIndividualsInformation.GK, SQLIndividualsInformation.GK)
        BlockCompare(FAMISIndividualsInformation.GL, SQLIndividualsInformation.GL)

        BlockCompare(FAMISMedicaidInformation.HA, SQLMedicaidInformation.HA)
        BlockCompare(FAMISMedicaidInformation.HB, SQLMedicaidInformation.HB)
        BlockCompare(FAMISMedicaidInformation.HC, SQLMedicaidInformation.HC)
        BlockCompare(FAMISMedicaidInformation.HD, SQLMedicaidInformation.HD)
        BlockCompare(FAMISMedicaidInformation.HE, SQLMedicaidInformation.HE)
        BlockCompare(FAMISMedicaidInformation.HF, SQLMedicaidInformation.HF)
        BlockCompare(FAMISMedicaidInformation.HG, SQLMedicaidInformation.HG)
        BlockCompare(FAMISMedicaidInformation.HH, SQLMedicaidInformation.HH)
        BlockCompare(FAMISMedicaidInformation.HI, SQLMedicaidInformation.HI)
        BlockCompare(FAMISMedicaidInformation.HJ, SQLMedicaidInformation.HJ)
        BlockCompare(FAMISMedicaidInformation.HK, SQLMedicaidInformation.HK)
        BlockCompare(FAMISMedicaidInformation.HL, SQLMedicaidInformation.HL)
        BlockCompare(FAMISMedicaidInformation.HM, SQLMedicaidInformation.HM)
        BlockCompare(FAMISMedicaidInformation.HN, SQLMedicaidInformation.HN)
        BlockCompare(FAMISMedicaidInformation.HO, SQLMedicaidInformation.HO)
        BlockCompare(FAMISMedicaidInformation.HP, SQLMedicaidInformation.HP)
        BlockCompare(FAMISMedicaidInformation.HQ, SQLMedicaidInformation.HQ)
        BlockCompare(FAMISMedicaidInformation.HR, SQLMedicaidInformation.HR)
        BlockCompare(FAMISMedicaidInformation.HS, SQLMedicaidInformation.HS)
        BlockCompare(FAMISMedicaidInformation.HT, SQLMedicaidInformation.HT)

        BlockCompare(FAMISMedicaidInformation.WA, SQLMedicaidInformation.WA)
        BlockCompare(FAMISMedicaidInformation.WB, SQLMedicaidInformation.WB)
        BlockCompare(FAMISMedicaidInformation.WC, SQLMedicaidInformation.WC)
        BlockCompare(FAMISMedicaidInformation.WD, SQLMedicaidInformation.WD)
        BlockCompare(FAMISMedicaidInformation.WE, SQLMedicaidInformation.WE)
        BlockCompare(FAMISMedicaidInformation.WF, SQLMedicaidInformation.WF)
        BlockCompare(FAMISMedicaidInformation.WG, SQLMedicaidInformation.WG)
        BlockCompare(FAMISMedicaidInformation.WH, SQLMedicaidInformation.WH)
        BlockCompare(FAMISMedicaidInformation.WI, SQLMedicaidInformation.WI)
        BlockCompare(FAMISMedicaidInformation.WK, SQLMedicaidInformation.WK)
        BlockCompare(FAMISMedicaidInformation.WL, SQLMedicaidInformation.WL)
        BlockCompare(FAMISMedicaidInformation.WM, SQLMedicaidInformation.WM)
        BlockCompare(FAMISMedicaidInformation.WN, SQLMedicaidInformation.WN)
        BlockCompare(FAMISMedicaidInformation.WO, SQLMedicaidInformation.WO)
        BlockCompare(FAMISMedicaidInformation.WP, SQLMedicaidInformation.WP)
        BlockCompare(FAMISMedicaidInformation.WQ, SQLMedicaidInformation.WQ)
        BlockCompare(FAMISMedicaidInformation.WR, SQLMedicaidInformation.WR)
        BlockCompare(FAMISMedicaidInformation.WS, SQLMedicaidInformation.WS)
        BlockCompare(FAMISMedicaidInformation.WT, SQLMedicaidInformation.WT)
        BlockCompare(FAMISMedicaidInformation.WU, SQLMedicaidInformation.WU)
        BlockCompare(FAMISMedicaidInformation.WV, SQLMedicaidInformation.WV)
        BlockCompare(FAMISMedicaidInformation.WW, SQLMedicaidInformation.WW)

        ' BlockCompare(FAMISIncomeInformation.JA, SQLIncomeInformation.JA)
        BlockCompare(FAMISIncomeInformation.JB, SQLIncomeInformation.JB)
        BlockCompare(FAMISIncomeInformation.JC, SQLIncomeInformation.JC)
        BlockCompare(FAMISIncomeInformation.JD, SQLIncomeInformation.JD)
        BlockCompare(FAMISIncomeInformation.JE, SQLIncomeInformation.JE)
        BlockCompare(FAMISIncomeInformation.JF, SQLIncomeInformation.JF)
        BlockCompare(FAMISIncomeInformation.JG, SQLIncomeInformation.JG)
        BlockCompare(FAMISIncomeInformation.JH, SQLIncomeInformation.JH)
        BlockCompare(FAMISIncomeInformation.JI, SQLIncomeInformation.JI)
        BlockCompare(FAMISIncomeInformation.JJ, SQLIncomeInformation.JJ)
        BlockCompare(FAMISIncomeInformation.JK, SQLIncomeInformation.JK)
        BlockCompare(FAMISIncomeInformation.JL, SQLIncomeInformation.JL)
        BlockCompare(FAMISIncomeInformation.JM, SQLIncomeInformation.JM)
        BlockCompare(FAMISIncomeInformation.JN, SQLIncomeInformation.JN)
        BlockCompare(FAMISIncomeInformation.JO, SQLIncomeInformation.JO)
        BlockCompare(FAMISIncomeInformation.JP, SQLIncomeInformation.JP)
        BlockCompare(FAMISIncomeInformation.JQ, SQLIncomeInformation.JQ)
        BlockCompare(FAMISIncomeInformation.JR, SQLIncomeInformation.JR)
        BlockCompare(FAMISIncomeInformation.JS, SQLIncomeInformation.JS)
        BlockCompare(FAMISIncomeInformation.JT, SQLIncomeInformation.JT)
        BlockCompare(FAMISIncomeInformation.JU, SQLIncomeInformation.JU)
        '"        '  blockcompare	(FAMISIncomeInformation.JV)		,SQLIncomeInformation.JV)					"	
        BlockCompare(FAMISIncomeInformation.JW, SQLIncomeInformation.JW)
        BlockCompare(FAMISIncomeInformation.JX, SQLIncomeInformation.JX)

        'BlockCompare(FAMISIncomeInformation.KA, SQLIncomeInformation.KA)
        BlockCompare(FAMISIncomeInformation.KB, SQLIncomeInformation.KB)
        BlockCompare(FAMISIncomeInformation.KC, SQLIncomeInformation.KC)
        BlockCompare(FAMISIncomeInformation.KD, SQLIncomeInformation.KD)
        BlockCompare(FAMISIncomeInformation.KE, SQLIncomeInformation.KE)
        BlockCompare(FAMISIncomeInformation.KF, SQLIncomeInformation.KF)
        BlockCompare(FAMISIncomeInformation.KG, SQLIncomeInformation.KG)
        BlockCompare(FAMISIncomeInformation.KH, SQLIncomeInformation.KH)
        BlockCompare(FAMISIncomeInformation.KI, SQLIncomeInformation.KI)
        BlockCompare(FAMISIncomeInformation.KJ, SQLIncomeInformation.KJ)
        BlockCompare(FAMISIncomeInformation.KK, SQLIncomeInformation.KK)
        BlockCompare(FAMISIncomeInformation.KL, SQLIncomeInformation.KL)
        BlockCompare(FAMISIncomeInformation.KM, SQLIncomeInformation.KM)
        BlockCompare(FAMISIncomeInformation.KN, SQLIncomeInformation.KN)
        BlockCompare(FAMISIncomeInformation.KO, SQLIncomeInformation.KO)
        BlockCompare(FAMISIncomeInformation.KP, SQLIncomeInformation.KP)
        BlockCompare(FAMISIncomeInformation.KQ, SQLIncomeInformation.KQ)
        BlockCompare(FAMISIncomeInformation.KR, SQLIncomeInformation.KR)
        BlockCompare(FAMISIncomeInformation.KS, SQLIncomeInformation.KS)
        '"        'blockcompare	(FAMISIncomeInformation.KT)		,SQLIncomeInformation.KT)					"	
        BlockCompare(FAMISIncomeInformation.KU, SQLIncomeInformation.KU)
        BlockCompare(FAMISIncomeInformation.KV, SQLIncomeInformation.KV)

        BlockCompare(FAMISFoodStampInformation.LA, SQLFoodStampInformation.LA)
        BlockCompare(FAMISFoodStampInformation.LB, SQLFoodStampInformation.LB)
        BlockCompare(FAMISFoodStampInformation.LC, SQLFoodStampInformation.LC)
        BlockCompare(FAMISFoodStampInformation.LD, SQLFoodStampInformation.LD)
        BlockCompare(FAMISFoodStampInformation.LE, SQLFoodStampInformation.LE)
        BlockCompare(FAMISFoodStampInformation.LF, SQLFoodStampInformation.LF)
        BlockCompare(FAMISFoodStampInformation.LG, SQLFoodStampInformation.LG)
        BlockCompare(FAMISFoodStampInformation.LH, SQLFoodStampInformation.LH)
        BlockCompare(FAMISFoodStampInformation.LI, SQLFoodStampInformation.LI)
        BlockCompare(FAMISFoodStampInformation.LJ, SQLFoodStampInformation.LJ)
        BlockCompare(FAMISFoodStampInformation.LK, SQLFoodStampInformation.LK)
        BlockCompare(FAMISFoodStampInformation.LL, SQLFoodStampInformation.LL)
        BlockCompare(FAMISFoodStampInformation.LM, SQLFoodStampInformation.LM)
        BlockCompare(FAMISFoodStampInformation.LN, SQLFoodStampInformation.LN)
        BlockCompare(FAMISFoodStampInformation.LO, SQLFoodStampInformation.LO)
        BlockCompare(FAMISFoodStampInformation.LP, SQLFoodStampInformation.LP)
        BlockCompare(FAMISFoodStampInformation.LQ, SQLFoodStampInformation.LQ)
        BlockCompare(FAMISFoodStampInformation.LR, SQLFoodStampInformation.LR)
        BlockCompare(FAMISFoodStampInformation.LS, SQLFoodStampInformation.LS)
        BlockCompare(FAMISFoodStampInformation.LT, SQLFoodStampInformation.LT)

        BlockCompare(FAMISFoodStampInformation.MA, SQLFoodStampInformation.MA)
        BlockCompare(FAMISFoodStampInformation.MB, SQLFoodStampInformation.MB)
        BlockCompare(FAMISFoodStampInformation.MC, SQLFoodStampInformation.MC)
        BlockCompare(FAMISFoodStampInformation.MD, SQLFoodStampInformation.MD)
        BlockCompare(FAMISFoodStampInformation.ME1, SQLFoodStampInformation.ME1)
        BlockCompare(FAMISFoodStampInformation.MF, SQLFoodStampInformation.MF)
        BlockCompare(FAMISFoodStampInformation.MG, SQLFoodStampInformation.MG)
        BlockCompare(FAMISFoodStampInformation.MH, SQLFoodStampInformation.MH)
        BlockCompare(FAMISFoodStampInformation.MI, SQLFoodStampInformation.MI)
        BlockCompare(FAMISFoodStampInformation.MJ, SQLFoodStampInformation.MJ)
        BlockCompare(FAMISFoodStampInformation.MK, SQLFoodStampInformation.MK)
        BlockCompare(FAMISFoodStampInformation.ML, SQLFoodStampInformation.ML)
        BlockCompare(FAMISFoodStampInformation.MM, SQLFoodStampInformation.MM)
        BlockCompare(FAMISFoodStampInformation.MN, SQLFoodStampInformation.MN)
        BlockCompare(FAMISFoodStampInformation.MO, SQLFoodStampInformation.MO)
        BlockCompare(FAMISFoodStampInformation.MP, SQLFoodStampInformation.MP)
        BlockCompare(FAMISFoodStampInformation.MQ, SQLFoodStampInformation.MQ)
        BlockCompare(FAMISFoodStampInformation.MR, SQLFoodStampInformation.MR)
        '"        'blockcompare	(FAMISFoodStampInformation.MS		,SQLFoodStampInformation.MS		 ,"	")			"
        '"        'blockcompare	(FAMISFoodStampInformation.MT		,SQLFoodStampInformation.MT		 ,"	")			"

        BlockCompare(FAMISFoodStampInformation.NA, SQLFoodStampInformation.NA)
        BlockCompare(FAMISFoodStampInformation.NB, SQLFoodStampInformation.NB)
        BlockCompare(FAMISFoodStampInformation.NC, SQLFoodStampInformation.NC)
        BlockCompare(FAMISFoodStampInformation.ND, SQLFoodStampInformation.ND)
        BlockCompare(FAMISFoodStampInformation.NE, SQLFoodStampInformation.NE)
        BlockCompare(FAMISFoodStampInformation.NF, SQLFoodStampInformation.NF)
        BlockCompare(FAMISFoodStampInformation.NG, SQLFoodStampInformation.NG)
        BlockCompare(FAMISFoodStampInformation.NH, SQLFoodStampInformation.NH)
        BlockCompare(FAMISFoodStampInformation.NI, SQLFoodStampInformation.NI)
        BlockCompare(FAMISFoodStampInformation.NJ, SQLFoodStampInformation.NJ)
        BlockCompare(FAMISFoodStampInformation.NK, SQLFoodStampInformation.NK)
        BlockCompare(FAMISFoodStampInformation.NL, SQLFoodStampInformation.NL)
        BlockCompare(FAMISFoodStampInformation.NM, SQLFoodStampInformation.NM)
        BlockCompare(FAMISFoodStampInformation.NN, SQLFoodStampInformation.NN)
        BlockCompare(FAMISFoodStampInformation.NO, SQLFoodStampInformation.NO)
        BlockCompare(FAMISFoodStampInformation.NP, SQLFoodStampInformation.NP)

        BlockCompare(FAMISFoodStampInformation.OA, SQLFoodStampInformation.OA)
        BlockCompare(FAMISFoodStampInformation.OB, SQLFoodStampInformation.OB)
        BlockCompare(FAMISFoodStampInformation.OC, SQLFoodStampInformation.OC)
        BlockCompare(FAMISFoodStampInformation.OD, SQLFoodStampInformation.OD)
        BlockCompare(FAMISFoodStampInformation.OE, SQLFoodStampInformation.OE)
        BlockCompare(FAMISFoodStampInformation.OF1, SQLFoodStampInformation.OF1)
        BlockCompare(FAMISFoodStampInformation.OG, SQLFoodStampInformation.OG)
        BlockCompare(FAMISFoodStampInformation.OH, SQLFoodStampInformation.OH)
        BlockCompare(FAMISFoodStampInformation.OI, SQLFoodStampInformation.OI)
        BlockCompare(FAMISFoodStampInformation.OJ, SQLFoodStampInformation.OJ)
        BlockCompare(FAMISFoodStampInformation.OK, SQLFoodStampInformation.OK)
        BlockCompare(FAMISFoodStampInformation.OL, SQLFoodStampInformation.OL)
        '"        'blockcompare	(FAMISFoodStampInformation.OM		,SQLFoodStampInformation.OM		 ,"	")			"
        BlockCompare(FAMISFoodStampInformation.ON1, SQLFoodStampInformation.ON1)
        BlockCompare(FAMISFoodStampInformation.OO, SQLFoodStampInformation.OO)
        '"        'blockcompare	(FAMISFoodStampInformation.OP		,SQLFoodStampInformation.OP		 ,"	") --Not in XML or Text File--			"

        BlockCompare(FAMISFoodStampInformation.WX, SQLFoodStampInformation.WX)
        BlockCompare(FAMISFoodStampInformation.WY, SQLFoodStampInformation.WY)

        BlockCompare(FAMISIandAInformation.PA, SQLIandAInformation.PA)
        BlockCompare(FAMISIandAInformation.PB, SQLIandAInformation.PB)
        BlockCompare(FAMISIandAInformation.PC, SQLIandAInformation.PC)
        BlockCompare(FAMISIandAInformation.PD, SQLIandAInformation.PD)
        BlockCompare(FAMISIandAInformation.PE, SQLIandAInformation.PE)
        BlockCompare(FAMISIandAInformation.PF, SQLIandAInformation.PF)
        BlockCompare(FAMISIandAInformation.PG, SQLIandAInformation.PG)
        BlockCompare(FAMISIandAInformation.PH, SQLIandAInformation.PH)
        BlockCompare(FAMISIandAInformation.PI, SQLIandAInformation.PI)
        BlockCompare(FAMISIandAInformation.PJ, SQLIandAInformation.PJ)
        BlockCompare(FAMISIandAInformation.PK, SQLIandAInformation.PK)
        BlockCompare(FAMISIandAInformation.PL, SQLIandAInformation.PL)
        BlockCompare(FAMISIandAInformation.PM, SQLIandAInformation.PM)
        BlockCompare(FAMISIandAInformation.PN, SQLIandAInformation.PN)
        BlockCompare(FAMISIandAInformation.PO, SQLIandAInformation.PO)
        BlockCompare(FAMISIandAInformation.PP, SQLIandAInformation.PP)

        BlockCompare(FAMISTANFInformation.IA, SQLTANFInformation.IA)
        BlockCompare(FAMISTANFInformation.IB, SQLTANFInformation.IB)
        BlockCompare(FAMISTANFInformation.IC, SQLTANFInformation.IC)
        BlockCompare(FAMISTANFInformation.ID, SQLTANFInformation.ID)
        BlockCompare(FAMISTANFInformation.IE, SQLTANFInformation.IE)
        BlockCompare(FAMISTANFInformation.IF1, SQLTANFInformation.IF1)
        BlockCompare(FAMISTANFInformation.IG, SQLTANFInformation.IG)
        BlockCompare(FAMISTANFInformation.IH, SQLTANFInformation.IH)
        BlockCompare(FAMISTANFInformation.II, SQLTANFInformation.II)
        BlockCompare(FAMISTANFInformation.IJ, SQLTANFInformation.IJ)
        BlockCompare(FAMISTANFInformation.IK, SQLTANFInformation.IK)
        BlockCompare(FAMISTANFInformation.IL, SQLTANFInformation.IL)
        BlockCompare(FAMISTANFInformation.IM, SQLTANFInformation.IM)
        BlockCompare(FAMISTANFInformation.IN1, SQLTANFInformation.IN1)
        BlockCompare(FAMISTANFInformation.IO, SQLTANFInformation.IO)
        BlockCompare(FAMISTANFInformation.IP, SQLTANFInformation.IP)

        BlockCompare(FAMISCaseInformation.AQ, SQLCaseInformation.AQ)
        BlockCompare(FAMISFoodStampInformation.MU, SQLFoodStampInformation.MU)
        BlockCompare(FAMISFoodStampInformation.MT, SQLFoodStampInformation.MT)
        BlockCompare(FAMISFoodStampInformation.MU, SQLFoodStampInformation.MU)
        BlockCompare(FAMISFoodStampInformation.NQ, SQLFoodStampInformation.NQ)
        BlockCompare(FAMISFoodStampInformation.NR, SQLFoodStampInformation.NR)
        BlockCompare(FAMISFoodStampInformation.NS, SQLFoodStampInformation.NS)
        BlockCompare(FAMISFoodStampInformation.NT, SQLFoodStampInformation.NT)
        BlockCompare(FAMISFoodStampInformation.NU, SQLFoodStampInformation.NU)
        BlockCompare(FAMISFoodStampInformation.NV, SQLFoodStampInformation.NV)
        BlockCompare(FAMISFoodStampInformation.NW, SQLFoodStampInformation.NW)
        BlockCompare(FAMISFoodStampInformation.OP, SQLFoodStampInformation.OP)
        BlockCompare(FAMISFoodStampInformation.OR, SQLFoodStampInformation.OR)
        BlockCompare(FAMISFoodStampInformation.VB, SQLFoodStampInformation.VB)
        BlockCompare(FAMISFoodStampInformation.VF, SQLFoodStampInformation.VF)
        BlockCompare(FAMISFoodStampInformation.VH, SQLFoodStampInformation.VH)
        BlockCompare(FAMISIndividualsInformation.B1, SQLIndividualsInformation.B1)
        BlockCompare(FAMISIndividualsInformation.B2, SQLIndividualsInformation.B2)
        BlockCompare(FAMISIndividualsInformation.B3, SQLIndividualsInformation.B3)
        BlockCompare(FAMISIndividualsInformation.B4, SQLIndividualsInformation.B4)
        If Not isSecurityAvailable Then
            BlockCompare(FAMISIndividualsInformation.B5, SQLIndividualsInformation.B5)
            BlockCompare(FAMISIndividualsInformation.B6, SQLIndividualsInformation.B6)
        End If
        BlockCompare(FAMISIndividualsInformation.FQ, SQLIndividualsInformation.FQ)
        BlockCompare(FAMISIndividualsInformation.FR, SQLIndividualsInformation.FR)
        BlockCompare(FAMISMedicaidInformation.HU, SQLMedicaidInformation.HU)
        BlockCompare(FAMISMedicaidInformation.HV, SQLMedicaidInformation.HV)
        BlockCompare(FAMISMedicaidInformation.XO, SQLMedicaidInformation.XO)
        BlockCompare(FAMISMedicaidInformation.XP, SQLMedicaidInformation.XP)
        BlockCompare(FAMISMedicaidInformation.XQ, SQLMedicaidInformation.XQ)
        BlockCompare(FAMISMedicaidInformation.XR, SQLMedicaidInformation.XR)
        BlockCompare(FAMISMedicaidInformation.XS, SQLMedicaidInformation.XS)
        BlockCompare(FAMISMedicaidInformation.XT, SQLMedicaidInformation.XT)
        BlockCompare(FAMISMedicaidInformation.XU, SQLMedicaidInformation.XU)
        BlockCompare(FAMISMedicaidInformation.XV, SQLMedicaidInformation.XV)

        For i = 0 To numChildren - 1
            BlockCompare(FAMISCaseChild(i).QA, SQLCaseChild(i).QA)
            BlockCompare(FAMISCaseChild(i).QB, SQLCaseChild(i).QB)
            BlockCompare(FAMISCaseChild(i).QC, SQLCaseChild(i).QC)
            BlockCompare(FAMISCaseChild(i).QD, SQLCaseChild(i).QD)
            BlockCompare(FAMISCaseChild(i).QE, SQLCaseChild(i).QE)
            '"            'blockcompare	(FAMISCaseChild		,SQLCaseChild		i).QE2	 FAMISCaseChild	i).QA.getdata	))"	
            '"            'FAMISCaseChild	i).QE.SetData		i).QE.SetData		FAMISCaseChild	i).QE2.GetData)		"	
            '"            'FAMISCaseChild	i).QE.Length = (FAMISCaseChild		i).QE.Length = ,SQLCaseChild		i).QE.Length + FAMISCaseChild			"	

            BlockCompare(FAMISCaseChild(i).QF, SQLCaseChild(i).QF)
            BlockCompare(FAMISCaseChild(i).QG, SQLCaseChild(i).QG)
            BlockCompare(FAMISCaseChild(i).QH, SQLCaseChild(i).QH)
            BlockCompare(FAMISCaseChild(i).QI, SQLCaseChild(i).QI)
            BlockCompare(FAMISCaseChild(i).QK, SQLCaseChild(i).QK)
            BlockCompare(FAMISCaseChild(i).QL, SQLCaseChild(i).QL)
            BlockCompare(FAMISCaseChild(i).QM, SQLCaseChild(i).QM)
            BlockCompare(FAMISCaseChild(i).QN, SQLCaseChild(i).QN)
            BlockCompare(FAMISCaseChild(i).QO, SQLCaseChild(i).QO)

            BlockCompare(FAMISCaseChild(i).RA, SQLCaseChild(i).RA)
            BlockCompare(FAMISCaseChild(i).RB, SQLCaseChild(i).RB)
            BlockCompare(FAMISCaseChild(i).RC, SQLCaseChild(i).RC)
            BlockCompare(FAMISCaseChild(i).RD, SQLCaseChild(i).RD)
            BlockCompare(FAMISCaseChild(i).RE, SQLCaseChild(i).RE)
            BlockCompare(FAMISCaseChild(i).RF, SQLCaseChild(i).RF)
            BlockCompare(FAMISCaseChild(i).RG, SQLCaseChild(i).RG)
            BlockCompare(FAMISCaseChild(i).RH, SQLCaseChild(i).RH)
            BlockCompare(FAMISCaseChild(i).RI, SQLCaseChild(i).RI)
            BlockCompare(FAMISCaseChild(i).RJ1, SQLCaseChild(i).RJ1)
            BlockCompare(FAMISCaseChild(i).RJ2, SQLCaseChild(i).RJ2)
            BlockCompare(FAMISCaseChild(i).RK, SQLCaseChild(i).RK)
            BlockCompare(FAMISCaseChild(i).RL, SQLCaseChild(i).RL)
            BlockCompare(FAMISCaseChild(i).RM, SQLCaseChild(i).RM)
            BlockCompare(FAMISCaseChild(i).RN, SQLCaseChild(i).RN)
            BlockCompare(FAMISCaseChild(i).RO, SQLCaseChild(i).RO)
            BlockCompare(FAMISCaseChild(i).RP, SQLCaseChild(i).RP)
            BlockCompare(FAMISCaseChild(i).RQ, SQLCaseChild(i).RQ)
            BlockCompare(FAMISCaseChild(i).RR, SQLCaseChild(i).RR)

            'BlockCompare(FAMISCaseChild(i).SA, SQLCaseChild(i).SA)
            BlockCompare(FAMISCaseChild(i).SB, SQLCaseChild(i).SB)
            BlockCompare(FAMISCaseChild(i).SC, SQLCaseChild(i).SC)
            BlockCompare(FAMISCaseChild(i).SD, SQLCaseChild(i).SD)
            BlockCompare(FAMISCaseChild(i).SE, SQLCaseChild(i).SE)
            BlockCompare(FAMISCaseChild(i).SF, SQLCaseChild(i).SF)
            BlockCompare(FAMISCaseChild(i).SG, SQLCaseChild(i).SG)
            BlockCompare(FAMISCaseChild(i).SH, SQLCaseChild(i).SH)
            BlockCompare(FAMISCaseChild(i).SI, SQLCaseChild(i).SI)
            BlockCompare(FAMISCaseChild(i).SJ, SQLCaseChild(i).SJ)
            BlockCompare(FAMISCaseChild(i).SK, SQLCaseChild(i).SK)
            BlockCompare(FAMISCaseChild(i).SL, SQLCaseChild(i).SL)
            BlockCompare(FAMISCaseChild(i).SM, SQLCaseChild(i).SM)
            BlockCompare(FAMISCaseChild(i).SN, SQLCaseChild(i).SN)
            BlockCompare(FAMISCaseChild(i).SO, SQLCaseChild(i).SO)
            BlockCompare(FAMISCaseChild(i).SP, SQLCaseChild(i).SP)
            BlockCompare(FAMISCaseChild(i).SQ, SQLCaseChild(i).SQ)
            BlockCompare(FAMISCaseChild(i).SR, SQLCaseChild(i).SR)
            BlockCompare(FAMISCaseChild(i).SS, SQLCaseChild(i).SS)
            '"            'blockcompare	(FAMISCaseChild		,SQLCaseChild		(i).ST	 ,FAMISCaseChild	(i).QA.GetData	())"	

            BlockCompare(FAMISCaseChild(i).TA, SQLCaseChild(i).TA)
            BlockCompare(FAMISCaseChild(i).TB, SQLCaseChild(i).TB)
            BlockCompare(FAMISCaseChild(i).TC, SQLCaseChild(i).TC)
            BlockCompare(FAMISCaseChild(i).TD, SQLCaseChild(i).TD)
            BlockCompare(FAMISCaseChild(i).TE, SQLCaseChild(i).TE)
            BlockCompare(FAMISCaseChild(i).TF, SQLCaseChild(i).TF)
            BlockCompare(FAMISCaseChild(i).TG, SQLCaseChild(i).TG)
            BlockCompare(FAMISCaseChild(i).TH, SQLCaseChild(i).TH)
            BlockCompare(FAMISCaseChild(i).TI, SQLCaseChild(i).TI)

            BlockCompare(FAMISCaseChild(i).TJ, SQLCaseChild(i).TJ)
            BlockCompare(FAMISCaseChild(i).TK, SQLCaseChild(i).TK)
            BlockCompare(FAMISCaseChild(i).TL, SQLCaseChild(i).TL)
            BlockCompare(FAMISCaseChild(i).TM, SQLCaseChild(i).TM)
            '"            'blockcompare	(FAMISCaseChild		,SQLCaseChild		(i).TN	 ,FAMISCaseChild	(i).QA.GetData	())"	
            BlockCompare(FAMISCaseChild(i).TO1, SQLCaseChild(i).TO1)
            BlockCompare(FAMISCaseChild(i).TP, SQLCaseChild(i).TP)
            BlockCompare(FAMISCaseChild(i).TQ, SQLCaseChild(i).TQ)
            BlockCompare(FAMISCaseChild(i).TR, SQLCaseChild(i).TR)
            BlockCompare(FAMISCaseChild(i).TS, SQLCaseChild(i).TS)

            BlockCompare(FAMISCaseChild(i).UA, SQLCaseChild(i).UA)
            BlockCompare(FAMISCaseChild(i).UB, SQLCaseChild(i).UB)
            BlockCompare(FAMISCaseChild(i).UC, SQLCaseChild(i).UC)
            BlockCompare(FAMISCaseChild(i).UD, SQLCaseChild(i).UD)
            BlockCompare(FAMISCaseChild(i).UE, SQLCaseChild(i).UE)
            BlockCompare(FAMISCaseChild(i).UF, SQLCaseChild(i).UF)
            BlockCompare(FAMISCaseChild(i).UG, SQLCaseChild(i).UG)
            BlockCompare(FAMISCaseChild(i).UH, SQLCaseChild(i).UH)
            BlockCompare(FAMISCaseChild(i).UI, SQLCaseChild(i).UI)
            '"            'blockcompare	(FAMISCaseChild		,SQLCaseChild		(i).UJ	 ,FAMISCaseChild	(i).QA.GetData	())"	
            BlockCompare(FAMISCaseChild(i).UK, SQLCaseChild(i).UK)
            BlockCompare(FAMISCaseChild(i).UL, SQLCaseChild(i).UL)

            BlockCompare(FAMISCaseChild(i).QU, SQLCaseChild(i).QU)
            BlockCompare(FAMISCaseChild(i).RS, SQLCaseChild(i).RS)
            BlockCompare(FAMISCaseChild(i).RT, SQLCaseChild(i).RT)
            BlockCompare(FAMISCaseChild(i).ST, SQLCaseChild(i).ST)
            BlockCompare(FAMISCaseChild(i).TT, SQLCaseChild(i).TT)
            If Not isSecurityAvailable Then
                BlockCompare(FAMISCaseChild(i).TU, SQLCaseChild(i).TU)
            End If
            BlockCompare(FAMISCaseChild(i).YA, SQLCaseChild(i).YA)
        Next
        For i = 0 To numVRP - 1
            BlockCompare(FAMISVRPInformation(i).VA, SQLVRPInformation(i).VA)
            BlockCompare(FAMISVRPInformation(i).VC, SQLVRPInformation(i).VC)            
            BlockCompare(FAMISVRPInformation(i).VE, SQLVRPInformation(i).VE)
            BlockCompare(FAMISVRPInformation(i).VG, SQLVRPInformation(i).VG)
            BlockCompare(FAMISVRPInformation(i).VI, SQLVRPInformation(i).VI)
            BlockCompare(FAMISVRPInformation(i).VK, SQLVRPInformation(i).VK)
            BlockCompare(FAMISVRPInformation(i).VM, SQLVRPInformation(i).VM)
            BlockCompare(FAMISVRPInformation(i).VO, SQLVRPInformation(i).VO)
        Next
    End Sub
    Private Sub BlockCompare(ByVal FAMBlock As FAMISBlock, ByVal SQLBlock As FAMISBlock)
        Dim tempBlank As String = " ".PadLeft(FAMBlock.Length, " ")
        If FAMBlock.GetData <> tempBlank Then
            If FAMBlock.GetData.Length > 0 Then
                If FAMBlock.GetData.Substring(0, 1) = "-" Then
                    SQLBlock.SetData("-".PadRight(SQLBlock.Length, " "))
                Else
                    If FAMBlock.GetData <> SQLBlock.GetData Then SQLBlock.SetData(FAMBlock.GetData)
                End If
            End If
        End If
    End Sub
    Private Sub BlockCompare(ByVal FAMBlock As FAMISBlock_Date, ByVal SQLBlock As FAMISBlock_Date)
        'Dim tempBlank As String = " ".PadLeft(FAMBlock.Length, " ")
        Dim tempBlank10 As String = "          "
        Dim tempBlank8 As String = "        "
        If FAMBlock.GetData <> tempBlank10 And FAMBlock.GetData <> tempBlank8 Then
            If FAMBlock.GetData.Length > 0 Then
                If FAMBlock.GetData.Substring(0, 1) = "-" Then
                    SQLBlock.SetData("-".PadRight(SQLBlock.Length, " "))
                Else
                    If FAMBlock.GetData <> SQLBlock.GetData Then SQLBlock.SetData(FAMBlock.GetData)
                End If
            End If
        End If
    End Sub

    Private Sub CompareToSQL()
        Dim i As Integer
        Try
            If isCaseExist() Then
                If FAMISTANFInformation.IA.GetData = "C" And FAMISTANFInformation.IB.GetData = "RF " Then
                    '--Check to see if C RF is on the server--
                    '--If IB is RF then check for C in IA to blank out both otherwise leave alone--
                    If isBlankedSQL(FAMISTANFInformation.IB, "FAMISAFDCInformation") Then
                        isBlankedSQL(FAMISTANFInformation.IA, "FAMISAFDCInformation")
                        isBlankedSQL_DateTime(FAMISTANFInformation.IC, "FAMISAFDCInformation")
                    End If
                End If
                If FAMISTANFInformation.IA.GetData <> "1" And FAMISTANFInformation.IA.GetData <> "5" And FAMISTANFInformation.IA.GetData <> "A" Then isBlankedSQL_DateTime(FAMISTANFInformation.ID, "FAMISAFDCInformation")
                If FAMISTANFInformation.IA.GetData <> "1" And FAMISTANFInformation.IA.GetData <> "5" And FAMISTANFInformation.IA.GetData <> "A" Then isBlankedSQL(FAMISTANFInformation.IE, "FAMISAFDCInformation")
                If FAMISTANFInformation.IA.GetData <> "1" And FAMISTANFInformation.IA.GetData <> "5" And FAMISTANFInformation.IA.GetData <> "A" Then isBlankedSQL_DateTime(FAMISTANFInformation.IF1, "FAMISAFDCInformation")
                If FAMISTANFInformation.IA.GetData <> "1" And FAMISTANFInformation.IA.GetData <> "5" And FAMISTANFInformation.IA.GetData <> "A" Then isBlankedSQL_DateTime(FAMISTANFInformation.IG, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IH, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.II, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IJ, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IK, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IP, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IM, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IN1, "FAMISAFDCInformation")
                isBlankedSQL(FAMISTANFInformation.IO, "FAMISAFDCInformation")
                isBlankedSQL(FAMISFoodStampInformation.OJ, "FAMISAFDCInformation")

                isBlankedSQL(FAMISApplicationInformation.BA, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BB, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BC, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BD, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BE, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BF, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BF, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BG, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BH, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BI, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BJ, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BK, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BL, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BM, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BN, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BO, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BP, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BQ, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.CA, "FAMISApplicantInformation")
                'isBlankedSQL(FAMISApplicationInformation.CB, "FAMISApplicantInformation")
                If FAMISApplicationInformation.CA.GetData.Substring(0, 1) = "-" Then
                    FAMISApplicationInformation.CB.SetData("-")
                ElseIf FAMISApplicationInformation.CA.GetData.Substring(0, 1) <> " " Then
                    FAMISApplicationInformation.CB.SetData("X")
                End If
                isBlankedSQL(FAMISApplicationInformation.CC, "FAMISApplicantInformation")
                If isBlankedSQL(FAMISApplicationInformation.CD1, "FAMISApplicantInformation") Then FAMISApplicationInformation.CD2.SetData("  ")
                isBlankedSQL(FAMISApplicationInformation.CE, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.CF, "FAMISApplicantInformation")
                'isBlankedSQL(FAMISApplicationInformation.CG, "FAMISApplicantInformation")
                If isBlankedSQL(FAMISApplicationInformation.DA1, "FAMISApplicantInformation") Then FAMISApplicationInformation.DA2.SetData("        ") : FAMISApplicationInformation.DA3.SetData(" ")
                isBlankedSQL(FAMISApplicationInformation.DB, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.DC, "FAMISApplicantInformation")
                If isBlankedSQL(FAMISApplicationInformation.DD1, "FAMISApplicantInformation") Then FAMISApplicationInformation.DD2.SetData("  ")
                isBlankedSQL(FAMISApplicationInformation.DE, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.DF, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EA, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EB, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EC, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.ED1, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.ED2, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EE, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EF, "FAMISApplicantInformation")
                If FAMISApplicationInformation.EG.GetData <> "R" And (FAMISTANFInformation.IA.GetData() = " " And FAMISFoodStampInformation.LA.GetData() = " " And FAMISMedicaidInformation.WA.GetData() <> " ") Then isBlankedSQL(FAMISApplicationInformation.EG, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EH, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EJ, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EK, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EL, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EM, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.EN, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.BR, "FAMISApplicantInformation")
                isBlankedSQL(FAMISFoodStampInformation.NN, "FAMISApplicantInformation")
                isBlankedSQL(FAMISFoodStampInformation.OM, "FAMISApplicantInformation")
                isBlankedSQL_DateTime(FAMISApplicationInformation.XA, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XB, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XC, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XD, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XE, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XF, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XG, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XH, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XI, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XJ, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XK, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XL, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XM, "FAMISApplicantInformation")
                isBlankedSQL(FAMISApplicationInformation.XN, "FAMISApplicantInformation")
                'isBlankedSQL(FAMISApplicationInformation.jv, "FAMISApplicantInformation")
                'isBlankedSQL(FAMISApplicationInformation.kt, "FAMISApplicantInformation")

                If FAMISFoodStampInformation.LA.GetData <> "1" Then isBlankedSQL_DateTime(FAMISFoodStampInformation.LD, "FAMISFoodStampInformation")
                If FAMISFoodStampInformation.LA.GetData <> "1" Then isBlankedSQL_DateTime(FAMISFoodStampInformation.LE, "FAMISFoodStampInformation")
                'If FAMISFoodStampInformation.LA.GetData <> "1" Then isBlankedSQL(FAMISFoodStampInformation.LF, "FAMISFoodStampInformation")
                If FAMISFoodStampInformation.LA.GetData <> "1" Then isBlankedSQL(FAMISFoodStampInformation.LG, "FAMISFoodStampInformation")
                If FAMISFoodStampInformation.LA.GetData <> "1" Then isBlankedSQL(FAMISFoodStampInformation.LH, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LI, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LJ, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LK, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LL, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LM, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LO, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LP, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LQ, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LR, "FAMISFoodStampInformation")
                isBlankedSQL_DateTime(FAMISFoodStampInformation.LT, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MD, "FAMISFoodStampInformation")
                If FAMISFoodStampInformation.ND.GetData = "1" And FAMISFoodStampInformation.NE.GetData <> "1" Then isBlankedSQL(FAMISFoodStampInformation.NE, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NF, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NI, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NJ, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NP, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OA, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OK, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OO, "FAMISFoodStampInformation")
                'isBlankedSQL(FAMISFoodStampInformation.WX, "FAMISFoodStampInformation")
                'isBlankedSQL(FAMISFoodStampInformation.WY, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LS, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MA, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MB, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MC, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.ME1, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MF, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MG, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MH, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MI, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MJ, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MK, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.ML, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MM, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MN, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MO, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MP, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.MQ, "FAMISFoodStampInformation")
                If FAMISFoodStampInformation.MQ.GetData.Substring(0, 1) = "-" Then FAMISIndividualsInformation.FF.SetData("-")
                isBlankedSQL(FAMISFoodStampInformation.MR, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NB, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NK, "FAMISFoodStampInformation")
                If FAMISFoodStampInformation.NK.GetData.Substring(0, 1) = "-" Then FAMISIndividualsInformation.FN.SetData("-")
                isBlankedSQL(FAMISFoodStampInformation.NM, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NO, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OB, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OC, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OD, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OE, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.OG, "FAMISFoodStampInformation")
                'isBlankedSQL(FAMISFoodStampInformation.OL, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NA, "FAMISFoodStampInformation")
                isBlankedSQL_DateTime(FAMISFoodStampInformation.NC, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.NL, "FAMISFoodStampInformation")
                isBlankedSQL(FAMISFoodStampInformation.LN, "FAMISFoodStampInformation")

                If numVRP > 0 Then
                    If FAMISVRPInformation(0).VC.GetData = " " Then
                        isBlankedSQL(FAMISFoodStampInformation.OL, "FAMISFoodStampInformation")
                        isBlankedSQL(FAMISIandAInformation.PO, "FAMISIandAInformation")
                    End If
                Else
                    isBlankedSQL(FAMISFoodStampInformation.OL, "FAMISFoodStampInformation")
                    isBlankedSQL(FAMISIandAInformation.PO, "FAMISIandAInformation")
                End If

                isBlankedSQL_DateTime(FAMISIandAInformation.PA, "FAMISIandAInformation")
                'isBlankedSQL(FAMISIandAInformation.PB, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PC, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PD, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PE, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PF, "FAMISIandAInformation")    '--Removed 2/7/2008--
                isBlankedSQL(FAMISIandAInformation.PG, "FAMISIandAInformation")
                isBlankedSQL_DateTime(FAMISIandAInformation.PH, "FAMISIandAInformation")
                isBlankedSQL_DateTime(FAMISIandAInformation.PI, "FAMISIandAInformation")
                'isBlankedSQL(FAMISIandAInformation.PJ, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PK, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PL, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PM, "FAMISIandAInformation")    '--Removed 5/16/2008-- Complaints being removed even if date check requires it to stay--
                'isBlankedSQL(FAMISIandAInformation.PN, "FAMISIandAInformation")    '--Removed 2/7/2008--
                'isBlankedSQL(FAMISIandAInformation.PO, "FAMISIandAInformation")
                isBlankedSQL(FAMISIandAInformation.PP, "FAMISIandAInformation")

                isBlankedSQL(FAMISIncomeInformation.JP, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JQ, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JS, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JT, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JW, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JX, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KU, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KV, "FAMISIncomeInformation")
                'isBlankedSQL(FAMISIncomeInformation.JA, "FAMISIncomeInformation") '--Removed 1/18/2008 Reports that block is always needed--
                isBlankedSQL(FAMISIncomeInformation.JB, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JC, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JD, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JE, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JF, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JG, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JH, "FAMISIncomeInformation")
                If FAMISIncomeInformation.JA.GetData = "       " And FAMISTANFInformation.IA.GetData <> " " Then isBlankedSQL(FAMISIncomeInformation.JI, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JK, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JL, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JM, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JN, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JO, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JR, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JU, "FAMISIncomeInformation")
                'isBlankedSQL(FAMISIncomeInformation.KA, "FAMISIncomeInformation") '--Removed 1/18/2008 Reports that block is always needed--
                isBlankedSQL(FAMISIncomeInformation.KB, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KC, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KD, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KE, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KF, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KG, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KH, "FAMISIncomeInformation")
                If FAMISIncomeInformation.KA.GetData = "       " And FAMISTANFInformation.IA.GetData <> " " Then isBlankedSQL(FAMISIncomeInformation.KI, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KJ, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KK, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KL, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KM, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KN, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KO, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KP, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KQ, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KR, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.KS, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JJ, "FAMISIncomeInformation")
                isBlankedSQL(FAMISIncomeInformation.JA, "FAMISIncomeInformation")

                isBlankedSQL(FAMISApplicationInformation.BS, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISApplicationInformation.BT, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISApplicationInformation.BX, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISApplicationInformation.BV, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISApplicationInformation.BW, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISApplicationInformation.BY1, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISApplicationInformation.BZ, "FAMISIndividualsInformation")
                'isBlankedSQL(FAMISIndividualsInformation.FA, "FAMISIndividualsInformation")    '--Removed 11/29/2007 after complaints that the block was coming in blank when it wasn't supposed to be--
                isBlankedSQL_DateTime(FAMISIndividualsInformation.FB, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.FC, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.FD, "FAMISIndividualsInformation")
                If isBlankedSQL(FAMISIndividualsInformation.FE2, "FAMISIndividualsInformation") Then FAMISIndividualsInformation.FE1.SetData(" ")
                isBlankedSQL(FAMISIndividualsInformation.FF, "FAMISIndividualsInformation")
                If FAMISIndividualsInformation.FF.GetData.Substring(0, 1) = "-" Then FAMISFoodStampInformation.MQ.SetData("-")
                isBlankedSQL(FAMISIndividualsInformation.FG, "FAMISIndividualsInformation")
                'isBlankedSQL(FAMISIndividualsInformation.FI, "FAMISIndividualsInformation")    '--Removed 11/29/2007 after complaints that the block was coming in blank when it wasn't supposed to be--
                isBlankedSQL_DateTime(FAMISIndividualsInformation.FJ, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.FK, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.FL, "FAMISIndividualsInformation")
                If isBlankedSQL(FAMISIndividualsInformation.FM1, "FAMISIndividualsInformation") Then FAMISIndividualsInformation.FM2.SetData("  ")
                isBlankedSQL(FAMISIndividualsInformation.FN, "FAMISIndividualsInformation")
                If FAMISIndividualsInformation.FN.GetData.Substring(0, 1) = "-" Then FAMISFoodStampInformation.NK.SetData("-")
                isBlankedSQL(FAMISIndividualsInformation.FP, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.GA, "FAMISIndividualsInformation")
                If FAMISIndividualsInformation.GC.GetData <> "B" Then If isBlankedSQL_DateTime(FAMISIndividualsInformation.GB, "FAMISIndividualsInformation") Then isBlankedSQL(FAMISIndividualsInformation.GC, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.GD, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.GE, "FAMISIndividualsInformation")
                isBlankedSQL_DateTime(FAMISIndividualsInformation.GF, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.GG, "FAMISIndividualsInformation")
                If FAMISIndividualsInformation.GI.GetData <> "B" Then If isBlankedSQL_DateTime(FAMISIndividualsInformation.GH, "FAMISIndividualsInformation") Then isBlankedSQL(FAMISIndividualsInformation.GI, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.GK, "FAMISIndividualsInformation")
                isBlankedSQL_DateTime(FAMISIndividualsInformation.GL, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.FH, "FAMISIndividualsInformation")
                isBlankedSQL(FAMISIndividualsInformation.FO, "FAMISIndividualsInformation")

                isBlankedSQL(FAMISMedicaidInformation.HD, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HE, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HF, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HG, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HH, "FAMISMedicaidInformation")
                If FAMISIndividualsInformation.GC.GetData <> "B" Then isBlankedSQL(FAMISMedicaidInformation.HI, "FAMISMedicaidInformation")
                If FAMISMedicaidInformation.HI.GetData = "- " Then FAMISMedicaidInformation.HI.SetData("  ")
                isBlankedSQL(FAMISMedicaidInformation.HJ, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HK, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HL, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HM, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HN, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HO, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HP, "FAMISMedicaidInformation")
                If FAMISIndividualsInformation.GI.GetData <> "B" Then isBlankedSQL(FAMISMedicaidInformation.HQ, "FAMISMedicaidInformation")
                If FAMISMedicaidInformation.HQ.GetData = "- " Then FAMISMedicaidInformation.HQ.SetData("  ")
                isBlankedSQL(FAMISMedicaidInformation.HR, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HS, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HT, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HB, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.HC, "FAMISMedicaidInformation")
                If FAMISMedicaidInformation.WA.GetData = "1" Then isBlankedSQL(FAMISMedicaidInformation.WE, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WH, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WI, "FAMISMedicaidInformation")
                'isBlankedSQL(FAMISMedicaidInformation.WK, "FAMISMedicaidInformation")
                'isBlankedSQL(FAMISMedicaidInformation.WM, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WN, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WO, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WP, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WQ, "FAMISMedicaidInformation")
                'isBlankedSQL(FAMISMedicaidInformation.WR, "FAMISMedicaidInformation")
                isBlankedSQL(FAMISMedicaidInformation.WS, "FAMISMedicaidInformation")
                'isBlankedSQL(FAMISMedicaidInformation.WT, "FAMISMedicaidInformation")
                'isBlankedSQL(FAMISMedicaidInformation.WU, "FAMISMedicaidInformation")
                'isBlankedSQL(FAMISMedicaidInformation.WV, "FAMISMedicaidInformation")
                If FAMISMedicaidInformation.WA.GetData <> "1" And FAMISMedicaidInformation.WA.GetData <> "M" Then isBlankedSQL_DateTime(FAMISMedicaidInformation.WD, "FAMISMedicaidInformation") 'And (FAMISMedicaidInformation.WB.GetData <> "BB" And FAMISMedicaidInformation.WE.GetData <> "I") Then isBlankedSQL(FAMISMedicaidInformation.WD, "FAMISMedicaidInformation")
                If FAMISMedicaidInformation.WA.GetData <> "1" And FAMISMedicaidInformation.WA.GetData <> "M" Then isBlankedSQL_DateTime(FAMISMedicaidInformation.WF, "FAMISMedicaidInformation") 'And (FAMISMedicaidInformation.WB.GetData <> "BB" And FAMISMedicaidInformation.WE.GetData <> "I") Then isBlankedSQL(FAMISMedicaidInformation.WF, "FAMISMedicaidInformation")
                If FAMISMedicaidInformation.WA.GetData <> "1" And FAMISMedicaidInformation.WA.GetData <> "M" Then isBlankedSQL_DateTime(FAMISMedicaidInformation.WG, "FAMISMedicaidInformation")

                isBlankedSQL(FAMISCaseInformation.AQ, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.MS, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.MT, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.MU, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.NQ, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.NR, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.NS, "FAMISAppendedInformation")
                isBlankedSQL_DateTime(FAMISFoodStampInformation.NT, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.NU, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.NV, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.NW, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.OP, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.OR, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.VB, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.VF, "FAMISAppendedInformation")
                isBlankedSQL(FAMISFoodStampInformation.VH, "FAMISAppendedInformation")
                isBlankedSQL(FAMISIndividualsInformation.B1, "FAMISAppendedInformation")
                isBlankedSQL(FAMISIndividualsInformation.B2, "FAMISAppendedInformation")
                isBlankedSQL(FAMISIndividualsInformation.B3, "FAMISAppendedInformation")
                isBlankedSQL(FAMISIndividualsInformation.B4, "FAMISAppendedInformation")
                isBlankedSQL_DateTime(FAMISIndividualsInformation.B5, "FAMISAppendedInformation")
                isBlankedSQL_DateTime(FAMISIndividualsInformation.B6, "FAMISAppendedInformation")
                isBlankedSQL(FAMISIndividualsInformation.FQ, "FAMISAppendedInformation")
                isBlankedSQL(FAMISIndividualsInformation.FR, "FAMISAppendedInformation")
                isBlankedSQL_DateTime(FAMISMedicaidInformation.HU, "FAMISAppendedInformation")
                isBlankedSQL_DateTime(FAMISMedicaidInformation.HV, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XO, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XP, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XQ, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XR, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XS, "FAMISAppendedInformation")
                isBlankedSQL_DateTime(FAMISMedicaidInformation.XT, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XU, "FAMISAppendedInformation")
                isBlankedSQL(FAMISMedicaidInformation.XV, "FAMISAppendedInformation")
                
                For i = 0 To numChildren - 1
                    isGoToChild(i) = False
                    If Not isBlankedSQL(FAMISCaseChild(i).QE, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).QF, "FAMISCaseChild") Then isGoToChild(i) = True     '--Removed as test 4/16/2008--'
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).QG, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).QH, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).QI, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).QJ, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).QK, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then
                        isGoToChild(i) = True
                    Else
                        If Not isBlankedSQL(FAMISCaseChild(i).QL, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    End If
                    If Not isBlankedSQL(FAMISCaseChild(i).QM, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).QN, "FAMISCaseChild") Then isGoToChild(i) = True     '--Removed 11/29/2007 after complaints that the block was coming in blank when it wasn't supposed to be--
                    If Not isBlankedSQL(FAMISCaseChild(i).QO, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RA, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RB, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RC, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RD, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RE, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RF, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RG, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RH, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).RH2, "FAMISCaseChild") Then isGoToChild(i) = True     '--Removed as test 4/16/2008--'
                    'If Not isBlankedSQL(FAMISCaseChild(i).RI, "FAMISCaseChild") Then isGoToChild(i) = True      '--Removed as test 4/29/2008--'
                    'If Not isBlankedSQL(FAMISCaseChild(i).RJ2, "FAMISCaseChild") Then     '--Removed as test 4/16/2008--'
                    '    isGoToChild(i) = True
                    'Else
                    '    '--Added IF statement-- 11/28/2007
                    '    If Not isBlankedSQL(FAMISCaseChild(i).RJ1, "FAMISCaseChild") Then FAMISCaseChild(i).RJ1.SetData(" ")
                    'End If
                    If Not isBlankedSQL(FAMISCaseChild(i).RK, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).RL, "FAMISCaseChild") Then isGoToChild(i) = True     '--Removed as test 4/16/2008--'
                    If Not isBlankedSQL(FAMISCaseChild(i).RM, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RN, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RO, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RP, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RQ, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RR, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).SA, "FAMISCaseChild") Then isGoToChild(i) = True '--Removed 1/18/2008 Reports that block is always needed--
                    If Not isBlankedSQL(FAMISCaseChild(i).SB, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SC, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SD, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SE, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SF, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SG, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SH, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SI, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SJ, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SK, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SL, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SM, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).SN, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SO, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SP, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SQ, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SR, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).SS, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).ST, "FAMISCaseChild") Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TA, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TB, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TC, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TD, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).TE, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TF, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TG, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TH, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).TI, "FAMISCaseChild") Then isGoToChild(i) = True     '--Removed as test 4/16/2008--'
                    If Not isBlankedSQL(FAMISCaseChild(i).TJ, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TK, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).TL, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TM, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).TN, "FAMISCaseChild") Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TO1, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).TP, "FAMISCaseChild") Then isGoToChild(i) = True     '--Removed as test 4/16/2008--'
                    If Not isBlankedSQL(FAMISCaseChild(i).TQ, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TR, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TS, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UA, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UB, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UC, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UD, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UE, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UF, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UG, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UH, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UI, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).UJ, "FAMISCaseChild") Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UK, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).UL, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    'If Not isBlankedSQL(FAMISCaseChild(i).YA, "FAMISCaseChild") Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).QU, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).RS, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).RT, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).ST, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).TT, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL_DateTime(FAMISCaseChild(i).TU, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True
                    If Not isBlankedSQL(FAMISCaseChild(i).YA, "FAMISCaseChild", FAMISCaseChild(i).QA.GetData) Then isGoToChild(i) = True                    
                Next

                SQLConn.Open()
                SQLCommand.CommandText = "SELECT IA, IB, IC FROM FAMISAFDCInformation WHERE CaseNumber = '" & CASENUMBER & "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    If SQLReader.GetString(0) = FAMISTANFInformation.IA.GetData And SQLReader.GetString(1) = FAMISTANFInformation.IB.GetData And SQLReader.GetDateTime(2).Month.ToString.PadLeft(2, "0") & SQLReader.GetDateTime(2).Day.ToString.PadLeft(2, "0") & SQLReader.GetDateTime(2).Year.ToString = FAMISTANFInformation.IC.GetData And FAMISCaseInformation.P03 = "C" Then
                        FAMISAdjustments.del_LineI(FAMISTANFInformation)
                    End If
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT LA, LB, LC FROM FAMISFoodStampInformation WHERE CaseNumber = '" & CASENUMBER & "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    If SQLReader.GetString(0) = FAMISFoodStampInformation.LA.GetData And SQLReader.GetString(1) = FAMISFoodStampInformation.LB.GetData And SQLReader.GetDateTime(2).Month.ToString.PadLeft(2, "0") & SQLReader.GetDateTime(2).Day.ToString.PadLeft(2, "0") & SQLReader.GetDateTime(2).Year.ToString = FAMISFoodStampInformation.LC.GetData And FAMISCaseInformation.P05 = "C" Then
                        FAMISAdjustments.del_LineL(FAMISFoodStampInformation)
                    End If
                End If
                SQLReader.Close()
                SQLCommand.CommandText = "SELECT WA, WB, WC FROM FAMISMedicaidInformation WHERE CaseNumber = '" & CASENUMBER & "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    If SQLReader.GetString(0) = FAMISMedicaidInformation.WA.GetData And SQLReader.GetString(1) = FAMISMedicaidInformation.WB.GetData And SQLReader.GetDateTime(2).Month.ToString.PadLeft(2, "0") & SQLReader.GetDateTime(2).Day.ToString.PadLeft(2, "0") & SQLReader.GetDateTime(2).Year.ToString = FAMISMedicaidInformation.WC.GetData And FAMISCaseInformation.P10 = "C" Then
                        FAMISAdjustments.del_LineW(FAMISMedicaidInformation)
                    End If
                End If
                SQLConn.Close()
                FAMISFoodStampInformation.OM.SetData("")
                FAMISMedicaidInformation.HU.SetData("")
                FAMISMedicaidInformation.HV.SetData("")
            Else
                For i = 0 To numChildren - 1
                    isGoToChild(i) = True
                Next
            End If
            ParentForm_Put105.BlankOutMedicaidFields()
        Catch ex As Exception
            ParentForm_Put105.WriteLog("SQL Error " & CASENUMBER & " - " & ex.Message.ToString, True)
            SQLConn.Close()
        End Try
    End Sub
    Private Function isCaseExist()
        Try
            SQLConn.Open()
            SQLCommand.CommandText = "SELECT * FROM FAMISCaseInformation WHERE CaseNumber = '" & CASENUMBER & "' AND DATEENTERED > '" & Date.Now.AddYears(-3).Month & "/" & Date.Now.AddYears(-3).Day & "/" & Date.Now.AddYears(-3).Year & "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows = True Then
                SQLConn.Close()
                Return True
            Else
                SQLConn.Close()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show("CaseExist" & vbCrLf & ex.Message)
            Return False
        Finally
            SQLConn.Close()
        End Try
    End Function
    Private Function isBlankedSQL(ByRef Block As FAMISBlock, ByVal TableName As String) As Boolean
        Dim tempBlank As String = " "
        tempBlank = tempBlank.PadLeft(Block.Length, " ")
        Try
            SQLConn.Open()
            SQLCommand.CommandText() = "SELECT [" & Block.BlockName & "] FROM " & TableName & " WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                If Not SQLReader.IsDBNull(0) Then
                    If SQLReader.GetString(0) = Block.GetData Then
                        Block.SetData(" ".PadRight(Block.Length, " "))
                        SQLConn.Close()
                        Return True
                    Else
                        If Block.isDeleteAllowed Then
                            If Block.GetData = tempBlank And SQLReader.GetString(0) <> tempBlank Then
                                '--Auto Delete--
                                Block.SetData("-".PadRight(Block.Length, " "))
                            End If
                        End If
                        SQLConn.Close()
                        Return False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Regular Blanked" & vbCrLf & ex.Message.ToString)
            ParentForm_Put105.WriteLog("SQL Error " & CASENUMBER & " - " & ex.Message.ToString, True)
        Finally
            SQLConn.Close()
        End Try
    End Function
    Private Function isBlankedSQL(ByRef Block As FAMISBlock, ByVal TableName As String, ByVal Index As String) As Boolean
        Dim tempBlank As String = " "
        tempBlank = tempBlank.PadLeft(Block.Length, " ")
        Try
            SQLConn.Open()
            SQLCommand.CommandText() = "SELECT " & Block.BlockName & " FROM " & TableName & " WHERE CASENUMBER = '" & CASENUMBER & "' AND QA = '" & Index & "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                If Not SQLReader.IsDBNull(0) Then
                    If SQLReader.GetString(0) = Block.GetData Then
                        Block.SetData(" ".PadRight(Block.Length, " "))
                        SQLConn.Close()
                        Return True
                    Else
                        If Block.isDeleteAllowed Then
                            If Block.GetData = tempBlank And SQLReader.GetString(0) <> tempBlank Then
                                '--Auto Delete--
                                Block.SetData("-".PadRight(Block.Length, " "))
                            End If
                        End If
                        SQLConn.Close()
                        Return False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Regular Blanked Date" & vbCrLf & ex.Message.ToString)
            ParentForm_Put105.WriteLog("SQL Error " & CASENUMBER & " - " & ex.Message.ToString, True)
        Finally
            SQLConn.Close()
        End Try
    End Function
    Private Function isBlankedSQL_DateTime(ByRef Block As FAMISBlock, ByVal TableName As String) As Boolean
        Dim tempBlank As String = "        "
        Dim tempDate As Date = Nothing
        Try
            If Block.GetData().Length > 0 Then If Block.GetData <> tempBlank And Block.GetData <> "          " And Block.GetData.Substring(0, 1) <> "-" And isValidDate(Block.GetData) Then tempDate = Block.GetData.Insert(2, "/").Insert(5, "/")
            SQLConn.Open()
            SQLCommand.CommandText() = "SELECT " & Block.BlockName & " FROM " & TableName & " WHERE CASENUMBER = '" & CASENUMBER & "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                If Not SQLReader.IsDBNull(0) Then
                    If SQLReader.GetDateTime(0) = tempDate Then
                        Block.SetData(" ".PadRight(Block.Length, " "))
                        SQLConn.Close()
                        Return True
                    Else
                        If Block.isDeleteAllowed Then
                            If Block.GetData = tempBlank And SQLReader.GetDateTime(0) <> "1/1/1900" Then
                                '--Auto Delete--
                                Block.SetData("-".PadRight(Block.Length, " "))
                            End If
                        End If
                        SQLConn.Close()
                        Return False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("DateTime Blanked" & vbCrLf & ex.Message.ToString)
            ParentForm_Put105.WriteLog("SQL Error " & CASENUMBER & " - " & ex.Message.ToString, True)
        Finally
            SQLConn.Close()
        End Try
    End Function
    Private Function isBlankedSQL_DateTime(ByRef Block As FAMISBlock, ByVal TableName As String, ByVal Index As String) As Boolean
        Dim tempBlank As String = "        "
        Dim tempDate As Date
        Try
            If Block.GetData().Length > 0 Then If Block.GetData <> tempBlank And Block.GetData <> "          " And Block.GetData.Substring(0, 1) <> "-" Then tempDate = Block.GetData.Insert(2, "/").Insert(5, "/")
            SQLConn.Open()
            SQLCommand.CommandText() = "SELECT " & Block.BlockName & " FROM " & TableName & " WHERE CASENUMBER = '" & CASENUMBER & "' AND QA = '" & Index & "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                If Not SQLReader.IsDBNull(0) Then
                    If SQLReader.GetDateTime(0) = tempDate Then
                        Block.SetData(" ".PadRight(Block.Length, " "))
                        SQLConn.Close()
                        Return True
                    Else
                        If Block.isDeleteAllowed Then
                            If Block.GetData = tempBlank And SQLReader.GetDateTime(0) <> "1/1/1900" Then
                                '--Auto Delete--
                                Block.SetData("-".PadRight(Block.Length, " "))
                            End If
                        End If
                        SQLConn.Close()
                        Return False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("DateTime Blanked Date" & vbCrLf & ex.Message.ToString)
            ParentForm_Put105.WriteLog("SQL Error " & CASENUMBER & " - " & ex.Message.ToString, True)
        Finally
            SQLConn.Close()
        End Try
    End Function

    Private Sub CheckCaseStatus()
        Dim counter As Integer = 0
        glapiTP8.SendKeysTransmit("105A,," & CASENUMBER)
        If glapiTP8.GetString(3, 2, 19, 2) <> "CASE NOT ON FAMIS" Then
            'If glapiTP8.GetString(74, 3, 74, 3) = "C" And FAMISMedicaidInformation.WA.GetData = " " Then
            '    FAMISMedicaidInformation.WA.SetData("C")
            '    FAMISMedicaidInformation.WB.SetData("FR")
            '    FAMISMedicaidInformation.WC.SetData(Date.Now.AddMonths(1).Month.ToString.PadLeft(2, "0") & "/01/" & Date.Now.AddMonths(1).Year)
            'End If
            If glapiTP8.GetString(76, 3, 76, 3) = "C" And FAMISFoodStampInformation.LA.GetData = " " Then
                FAMISFoodStampInformation.LA.SetData("C")
                FAMISFoodStampInformation.LB.SetData("FR")
                FAMISFoodStampInformation.LC.SetData(Date.Now.AddMonths(1).Month.ToString.PadLeft(2, "0") & "/01/" & Date.Now.AddMonths(1).Year)
            End If
            If glapiTP8.GetString(78, 3, 78, 3) = "C" And FAMISTANFInformation.IA.GetData = " " Then
                FAMISTANFInformation.IA.SetData("C")
                FAMISTANFInformation.IB.SetData("FR")
                FAMISTANFInformation.IC.SetData(Date.Now.AddMonths(1).Month.ToString.PadLeft(2, "0") & "/01/" & Date.Now.AddMonths(1).Year)
            End If
        End If
        glapiTP8.SendCommandKey(Glink.GlinkKeyEnum.GlinkKey_F1)
        Thread.Sleep(500)
    End Sub

    Private Sub CreateBatch()
        Dim isDuplicate As Boolean
        Dim counter As Integer = 0
        'ParentForm_Put105.TEMPLocation = "create batch"
        setStatusLabel("Creating Batch")
        glapiTP8.SendKeysTransmit("BTCH")
        'ParentForm_Put105.TEMPLocation = "btch screen"
        BGW_ProcessFAMIS.ReportProgress(5)
        While glapiTP8.GetString(1, 1, 12, 1) <> "SCRN ( BTCH)"  '--Added 1/9/2008--
            Thread.Sleep(500)                                   '--Issue over BTCH screen not coming up after GLink sent a transfer method--
            counter += 1                                        '--Causing GLink to crash--
            If counter > 30 Then
                If counter > 60 Then
                    glapiTP8.SetVisible(True)
                    If MessageBox.Show("GLink Communication Error!" & vbCrLf & "Wait?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) = Windows.Forms.DialogResult.OK Then
                        counter = 0
                    Else
                        Exit While
                    End If
                Else
                    glapiTP8.SendCommandKey(Glink.GlinkKeyEnum.GlinkKey_F1)
                    Thread.Sleep(500)
                    glapiTP8.SendKeysTransmit("BTCH")
                End If
            End If
        End While
        Thread.Sleep(500)
        Do
            'ParentForm_Put105.TEMPLocation = "submit btch stuff"
            glapiTP8.SubmitField(6, BATCHNUMBER)
            glapiTP8.SubmitField(14, "99999999")
            glapiTP8.SubmitField(16, "01")
            'ParentForm_Put105.TEMPLocation = "transmitting stuff"
            glapiTP8.TransmitPage()
            'ParentForm_Put105.TEMPLocation = "stuff transmitted"
            If glapiTP8.GetString(1, 2, 3, 2) = "DUP" Then
                'ParentForm_Put105.TEMPLocation = "dup stuff"
                glapiTP8.SendCommand("DELE")
                glapiTP8.TransmitPage()
                isDuplicate = True
            Else
                isDuplicate = False
            End If
        Loop Until isDuplicate = False
        BGW_ProcessFAMIS.ReportProgress(5)
        'ParentForm_Put105.TEMPLocation = "batch created"
    End Sub
    Private Sub Submit_Page1()
        glapiTP8.SubmitField(FAMISCaseInformation.AA.FieldNumber, FAMISCaseInformation.AA.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AB.FieldNumber, FAMISCaseInformation.AB.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AC.FieldNumber, FAMISCaseInformation.AC.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AD.FieldNumber, FAMISCaseInformation.AD.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AE.FieldNumber, FAMISCaseInformation.AE.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AH.FieldNumber, FAMISCaseInformation.AH.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AI.FieldNumber, FAMISCaseInformation.AI.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AJ.FieldNumber, FAMISCaseInformation.AJ.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AK.FieldNumber, FAMISCaseInformation.AK.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AL.FieldNumber, FAMISCaseInformation.AL.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AM.FieldNumber, FAMISCaseInformation.AM.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AN.FieldNumber, FAMISCaseInformation.AN.GetData)
        glapiTP8.SubmitField(FAMISCaseInformation.AQ.FieldNumber, FAMISCaseInformation.AQ.GetData)

        glapiTP8.SubmitField(FAMISMedicaidInformation.HC.FieldNumber, FAMISMedicaidInformation.HC.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IA.FieldNumber, FAMISTANFInformation.IA.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IB.FieldNumber, FAMISTANFInformation.IB.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IC.FieldNumber, FAMISTANFInformation.IC.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.ID.FieldNumber, FAMISTANFInformation.ID.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IE.FieldNumber, FAMISTANFInformation.IE.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IF1.FieldNumber, FAMISTANFInformation.IF1.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IG.FieldNumber, FAMISTANFInformation.IG.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LA.FieldNumber, FAMISFoodStampInformation.LA.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LB.FieldNumber, FAMISFoodStampInformation.LB.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LC.FieldNumber, FAMISFoodStampInformation.LC.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LD.FieldNumber, FAMISFoodStampInformation.LD.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LE.FieldNumber, FAMISFoodStampInformation.LE.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LF.FieldNumber, FAMISFoodStampInformation.LF.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.ND.FieldNumber, FAMISFoodStampInformation.ND.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PK.FieldNumber, FAMISIandAInformation.PK.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PC.FieldNumber, FAMISIandAInformation.PC.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WA.FieldNumber, FAMISMedicaidInformation.WA.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WB.FieldNumber, FAMISMedicaidInformation.WB.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WC.FieldNumber, FAMISMedicaidInformation.WC.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WD.FieldNumber, FAMISMedicaidInformation.WD.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WE.FieldNumber, FAMISMedicaidInformation.WE.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WF.FieldNumber, FAMISMedicaidInformation.WF.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WG.FieldNumber, FAMISMedicaidInformation.WG.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WL.FieldNumber, FAMISMedicaidInformation.WL.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WW.FieldNumber, FAMISMedicaidInformation.WW.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NQ.FieldNumber, FAMISFoodStampInformation.NQ.GetData)
        Thread.Sleep(100)
    End Sub
    Private Sub Submit_Page2()
        glapiTP8.SubmitField(FAMISApplicationInformation.BA.FieldNumber, FAMISApplicationInformation.BA.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BB.FieldNumber, FAMISApplicationInformation.BB.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BC.FieldNumber, FAMISApplicationInformation.BC.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BD.FieldNumber, FAMISApplicationInformation.BD.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BE.FieldNumber, FAMISApplicationInformation.BE.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BF.FieldNumber, FAMISApplicationInformation.BF.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BG.FieldNumber, FAMISApplicationInformation.BG.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BH.FieldNumber, FAMISApplicationInformation.BH.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BI.FieldNumber, FAMISApplicationInformation.BI.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BS.FieldNumber, FAMISApplicationInformation.BS.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BT.FieldNumber, FAMISApplicationInformation.BT.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BU.FieldNumber, FAMISApplicationInformation.BU.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BV.FieldNumber, FAMISApplicationInformation.BV.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.B1.FieldNumber, FAMISIndividualsInformation.B1.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.B2.FieldNumber, FAMISIndividualsInformation.B2.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.B3.FieldNumber, FAMISIndividualsInformation.B3.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.B4.FieldNumber, FAMISIndividualsInformation.B4.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.B5.FieldNumber, FAMISIndividualsInformation.B5.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.B6.FieldNumber, FAMISIndividualsInformation.B6.GetData)

        glapiTP8.SubmitField(FAMISApplicationInformation.BJ.FieldNumber, FAMISApplicationInformation.BJ.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BK.FieldNumber, FAMISApplicationInformation.BK.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BL.FieldNumber, FAMISApplicationInformation.BL.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BM.FieldNumber, FAMISApplicationInformation.BM.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BN.FieldNumber, FAMISApplicationInformation.BN.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BO.FieldNumber, FAMISApplicationInformation.BO.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BP.FieldNumber, " ")
        glapiTP8.SubmitField(FAMISApplicationInformation.BQ.FieldNumber, FAMISApplicationInformation.BQ.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BR.FieldNumber, FAMISApplicationInformation.BR.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BW.FieldNumber, FAMISApplicationInformation.BW.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BX.FieldNumber, FAMISApplicationInformation.BX.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BY1.FieldNumber, FAMISApplicationInformation.BY1.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.BZ.FieldNumber, FAMISApplicationInformation.BZ.GetData)

        glapiTP8.SubmitField(FAMISApplicationInformation.CA.FieldNumber, FAMISApplicationInformation.CA.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CB.FieldNumber, FAMISApplicationInformation.CB.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CC.FieldNumber, FAMISApplicationInformation.CC.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CD1.FieldNumber, FAMISApplicationInformation.CD1.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CD2.FieldNumber, FAMISApplicationInformation.CD2.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CE.FieldNumber, FAMISApplicationInformation.CE.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CF.FieldNumber, FAMISApplicationInformation.CF.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.CG.FieldNumber, FAMISApplicationInformation.CG.GetData)

        glapiTP8.SubmitField(FAMISApplicationInformation.DA1.FieldNumber, FAMISApplicationInformation.DA1.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DA2.FieldNumber, FAMISApplicationInformation.DA2.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DA3.FieldNumber, FAMISApplicationInformation.DA3.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DB.FieldNumber, FAMISApplicationInformation.DB.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DC.FieldNumber, FAMISApplicationInformation.DC.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DD1.FieldNumber, FAMISApplicationInformation.DD1.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DD2.FieldNumber, FAMISApplicationInformation.DD2.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DE.FieldNumber, FAMISApplicationInformation.DE.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.DF.FieldNumber, FAMISApplicationInformation.DF.GetData)

        glapiTP8.SubmitField(FAMISApplicationInformation.EA.FieldNumber, FAMISApplicationInformation.EA.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EB.FieldNumber, FAMISApplicationInformation.EB.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EC.FieldNumber, FAMISApplicationInformation.EC.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.ED1.FieldNumber, FAMISApplicationInformation.ED1.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.ED2.FieldNumber, FAMISApplicationInformation.ED2.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EE.FieldNumber, FAMISApplicationInformation.EE.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EF.FieldNumber, FAMISApplicationInformation.EF.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EG.FieldNumber, FAMISApplicationInformation.EG.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EH.FieldNumber, FAMISApplicationInformation.EH.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EI.FieldNumber, FAMISApplicationInformation.EI.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EJ.FieldNumber, FAMISApplicationInformation.EJ.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EK.FieldNumber, FAMISApplicationInformation.EK.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EL.FieldNumber, FAMISApplicationInformation.EL.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EM.FieldNumber, FAMISApplicationInformation.EM.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.EN.FieldNumber, FAMISApplicationInformation.EN.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XM.FieldNumber, FAMISApplicationInformation.XM.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XN.FieldNumber, FAMISApplicationInformation.XN.GetData)

        glapiTP8.SubmitField(FAMISApplicationInformation.XA.FieldNumber, FAMISApplicationInformation.XA.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XB.FieldNumber, FAMISApplicationInformation.XB.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XC.FieldNumber, FAMISApplicationInformation.XC.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XD.FieldNumber, FAMISApplicationInformation.XD.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XE.FieldNumber, FAMISApplicationInformation.XE.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XF.FieldNumber, FAMISApplicationInformation.XF.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XG.FieldNumber, FAMISApplicationInformation.XG.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XH.FieldNumber, FAMISApplicationInformation.XH.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XI.FieldNumber, FAMISApplicationInformation.XI.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XJ.FieldNumber, FAMISApplicationInformation.XJ.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XK.FieldNumber, FAMISApplicationInformation.XK.GetData)
        glapiTP8.SubmitField(FAMISApplicationInformation.XL.FieldNumber, FAMISApplicationInformation.XL.GetData)
        Thread.Sleep(100)
    End Sub
    Private Sub Submit_Page3()
        glapiTP8.SubmitField(FAMISIndividualsInformation.FA.FieldNumber, FAMISIndividualsInformation.FA.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FB.FieldNumber, FAMISIndividualsInformation.FB.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FC.FieldNumber, FAMISIndividualsInformation.FC.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FD.FieldNumber, FAMISIndividualsInformation.FD.GetData)
        'glapiTP8.SubmitField(FAMISIndividualsInformation.FD2.FieldNumber, FAMISIndividualsInformation.FA.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FE1.FieldNumber, FAMISIndividualsInformation.FE1.GetData & FAMISIndividualsInformation.FE2.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FF.FieldNumber, FAMISIndividualsInformation.FF.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FG.FieldNumber, FAMISIndividualsInformation.FG.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FH.FieldNumber, FAMISIndividualsInformation.FH.GetData)

        glapiTP8.SubmitField(FAMISIndividualsInformation.FI.FieldNumber, FAMISIndividualsInformation.FI.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FJ.FieldNumber, FAMISIndividualsInformation.FJ.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FK.FieldNumber, FAMISIndividualsInformation.FK.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FL.FieldNumber, FAMISIndividualsInformation.FL.GetData)
        'glapiTP8.SubmitField(FAMISIndividualsInformation.FL2.FieldNumber, FAMISIndividualsInformation.FL2.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIndividualsInformation.FM1.FieldNumber, FAMISIndividualsInformation.FM1.GetData & FAMISIndividualsInformation.FM2.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FN.FieldNumber, FAMISIndividualsInformation.FN.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FO.FieldNumber, FAMISIndividualsInformation.FO.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.FP.FieldNumber, FAMISIndividualsInformation.FP.GetData)

        glapiTP8.SubmitField(FAMISIndividualsInformation.GA.FieldNumber, FAMISIndividualsInformation.GA.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GB.FieldNumber, FAMISIndividualsInformation.GB.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GC.FieldNumber, FAMISIndividualsInformation.GC.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GD.FieldNumber, FAMISIndividualsInformation.GD.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GE.FieldNumber, FAMISIndividualsInformation.GE.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GF.FieldNumber, FAMISIndividualsInformation.GF.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GG.FieldNumber, FAMISIndividualsInformation.GG.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GH.FieldNumber, FAMISIndividualsInformation.GH.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GI.FieldNumber, FAMISIndividualsInformation.GI.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GJ.FieldNumber, FAMISIndividualsInformation.GJ.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GK.FieldNumber, FAMISIndividualsInformation.GK.GetData)
        glapiTP8.SubmitField(FAMISIndividualsInformation.GL.FieldNumber, FAMISIndividualsInformation.GL.GetData)

        glapiTP8.SubmitField(FAMISMedicaidInformation.HA.FieldNumber, FAMISMedicaidInformation.HA.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HB.FieldNumber, FAMISMedicaidInformation.HB.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HC.FieldNumber, FAMISMedicaidInformation.HC.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HD.FieldNumber, FAMISMedicaidInformation.HD.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HE.FieldNumber, FAMISMedicaidInformation.HE.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HF.FieldNumber, FAMISMedicaidInformation.HF.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HG.FieldNumber, FAMISMedicaidInformation.HG.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.HH.FieldNumber, FAMISMedicaidInformation.HH.GetData) --Protected--
        glapiTP8.SubmitField(FAMISMedicaidInformation.HI.FieldNumber, FAMISMedicaidInformation.HI.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HJ.FieldNumber, FAMISMedicaidInformation.HJ.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.HK.FieldNumber, FAMISMedicaidInformation.HK.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISMedicaidInformation.HL.FieldNumber, FAMISMedicaidInformation.HL.GetData) --Protected--

        glapiTP8.SubmitField(FAMISMedicaidInformation.HM.FieldNumber, FAMISMedicaidInformation.HM.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HN.FieldNumber, FAMISMedicaidInformation.HN.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HO.FieldNumber, FAMISMedicaidInformation.HO.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.HP.FieldNumber, FAMISMedicaidInformation.HP.GetData) --Protected--
        glapiTP8.SubmitField(FAMISMedicaidInformation.HQ.FieldNumber, FAMISMedicaidInformation.HQ.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HR.FieldNumber, FAMISMedicaidInformation.HR.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HU.FieldNumber, FAMISMedicaidInformation.HU.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.HV.FieldNumber, FAMISMedicaidInformation.HV.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.HS.FieldNumber, FAMISMedicaidInformation.HS.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISMedicaidInformation.HT.FieldNumber, FAMISMedicaidInformation.HT.GetData) --Protected--
        Thread.Sleep(100)
    End Sub
    Private Sub Submit_Page4()
        glapiTP8.SubmitField(FAMISTANFInformation.IH.FieldNumber, FAMISTANFInformation.IH.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.II.FieldNumber, FAMISTANFInformation.II.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IJ.FieldNumber, FAMISTANFInformation.IJ.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IK.FieldNumber, FAMISTANFInformation.IK.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IL.FieldNumber, FAMISTANFInformation.IL.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IM.FieldNumber, FAMISTANFInformation.IM.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IN1.FieldNumber, FAMISTANFInformation.IN1.GetData)
        glapiTP8.SubmitField(FAMISTANFInformation.IO.FieldNumber, FAMISTANFInformation.IO.GetData)
        'glapiTP8.SubmitField(FAMISTANFInformation.IP.FieldNumber, FAMISTANFInformation.Ip.GetData) --Protected--

        glapiTP8.SubmitField(FAMISIncomeInformation.JA.FieldNumber, FAMISIncomeInformation.JA.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JB.FieldNumber, FAMISIncomeInformation.JB.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JC.FieldNumber, FAMISIncomeInformation.JC.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JD.FieldNumber, FAMISIncomeInformation.JD.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JE.FieldNumber, FAMISIncomeInformation.JE.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JF.FieldNumber, FAMISIncomeInformation.JF.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JG.FieldNumber, FAMISIncomeInformation.JG.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JH.FieldNumber, FAMISIncomeInformation.JH.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JI.FieldNumber, FAMISIncomeInformation.JI.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JJ.FieldNumber, FAMISIncomeInformation.JJ.GetData)
        'glapiTP8.SubmitField(FAMISIncomeInformation.JK.FieldNumber, FAMISIncomeInformation.JK.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIncomeInformation.JL.FieldNumber, FAMISIncomeInformation.JL.GetData)
        'glapiTP8.SubmitField(FAMISIncomeInformation.JM.FieldNumber, FAMISIncomeInformation.JM.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIncomeInformation.JN.FieldNumber, FAMISIncomeInformation.JN.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JO.FieldNumber, FAMISIncomeInformation.JO.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JP.FieldNumber, FAMISIncomeInformation.JP.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JQ.FieldNumber, FAMISIncomeInformation.JQ.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JR.FieldNumber, FAMISIncomeInformation.JR.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JS.FieldNumber, FAMISIncomeInformation.JS.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JT.FieldNumber, FAMISIncomeInformation.JT.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JU.FieldNumber, FAMISIncomeInformation.JU.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JV.FieldNumber, " ")
        glapiTP8.SubmitField(FAMISIncomeInformation.JW.FieldNumber, FAMISIncomeInformation.JW.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.JX.FieldNumber, FAMISIncomeInformation.JX.GetData)

        glapiTP8.SubmitField(FAMISIncomeInformation.KA.FieldNumber, FAMISIncomeInformation.KA.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KB.FieldNumber, FAMISIncomeInformation.KB.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KC.FieldNumber, FAMISIncomeInformation.KC.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KD.FieldNumber, FAMISIncomeInformation.KD.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KE.FieldNumber, FAMISIncomeInformation.KE.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KF.FieldNumber, FAMISIncomeInformation.KF.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KG.FieldNumber, FAMISIncomeInformation.KG.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KH.FieldNumber, FAMISIncomeInformation.KH.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KI.FieldNumber, FAMISIncomeInformation.KI.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KJ.FieldNumber, FAMISIncomeInformation.KJ.GetData)
        'glapiTP8.SubmitField(FAMISIncomeInformation.KK.FieldNumber, FAMISIncomeInformation.KK.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIncomeInformation.KL.FieldNumber, FAMISIncomeInformation.KL.GetData)
        'glapiTP8.SubmitField(FAMISIncomeInformation.KM.FieldNumber, FAMISIncomeInformation.KM.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIncomeInformation.KN.FieldNumber, FAMISIncomeInformation.KN.GetData)
        'glapiTP8.SubmitField(FAMISIncomeInformation.KO.FieldNumber, FAMISIncomeInformation.KO.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIncomeInformation.KP.FieldNumber, "       ")
        'glapiTP8.SubmitField(FAMISIncomeInformation.KQ.FieldNumber, FAMISIncomeInformation.KQ.GetData) --Protected--
        glapiTP8.SubmitField(FAMISIncomeInformation.KR.FieldNumber, FAMISIncomeInformation.KR.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KS.FieldNumber, FAMISIncomeInformation.KS.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KT.FieldNumber, " ")
        glapiTP8.SubmitField(FAMISIncomeInformation.KU.FieldNumber, FAMISIncomeInformation.KU.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KV.FieldNumber, FAMISIncomeInformation.KV.GetData)
        glapiTP8.SubmitField(FAMISIncomeInformation.KU.FieldNumber, FAMISIncomeInformation.KU.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.VB.FieldNumber, FAMISFoodStampInformation.VB.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.VF.FieldNumber, FAMISFoodStampInformation.VF.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.VH.FieldNumber, FAMISFoodStampInformation.VH.GetData)

        If FAMISApplicationInformation.CG.GetData <> " " Then glapiTP8.SubmitField(FAMISFoodStampInformation.WX.FieldNumber, FAMISFoodStampInformation.WX.GetData)
        If FAMISApplicationInformation.CG.GetData <> " " Then glapiTP8.SubmitField(FAMISFoodStampInformation.WY.FieldNumber, FAMISFoodStampInformation.WY.GetData)
        Thread.Sleep(100)
    End Sub
    Private Sub Submit_Page5()
        glapiTP8.SubmitField(FAMISFoodStampInformation.LG.FieldNumber, FAMISFoodStampInformation.LG.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LH.FieldNumber, FAMISFoodStampInformation.LH.GetData)
        'glapiTP8.SubmitField(FAMISFoodStampInformation.LI.FieldNumber, FAMISFoodStampInformation.LI.GetData) --Protected--
        glapiTP8.SubmitField(FAMISFoodStampInformation.LJ.FieldNumber, FAMISFoodStampInformation.LJ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LK.FieldNumber, FAMISFoodStampInformation.LK.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LL.FieldNumber, FAMISFoodStampInformation.LL.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LM.FieldNumber, FAMISFoodStampInformation.LM.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LN.FieldNumber, FAMISFoodStampInformation.LN.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LO.FieldNumber, FAMISFoodStampInformation.LO.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LP.FieldNumber, FAMISFoodStampInformation.LP.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LQ.FieldNumber, FAMISFoodStampInformation.LQ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LR.FieldNumber, FAMISFoodStampInformation.LR.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LS.FieldNumber, FAMISFoodStampInformation.LS.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.LT.FieldNumber, FAMISFoodStampInformation.LT.GetData)

        glapiTP8.SubmitField(FAMISFoodStampInformation.MA.FieldNumber, FAMISFoodStampInformation.MA.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MB.FieldNumber, FAMISFoodStampInformation.MB.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MC.FieldNumber, FAMISFoodStampInformation.MC.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MD.FieldNumber, FAMISFoodStampInformation.MD.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.ME1.FieldNumber, FAMISFoodStampInformation.ME1.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MF.FieldNumber, FAMISFoodStampInformation.MF.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MG.FieldNumber, FAMISFoodStampInformation.MG.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MH.FieldNumber, FAMISFoodStampInformation.MH.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MI.FieldNumber, FAMISFoodStampInformation.MI.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MJ.FieldNumber, FAMISFoodStampInformation.MJ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MK.FieldNumber, FAMISFoodStampInformation.MK.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.ML.FieldNumber, FAMISFoodStampInformation.ML.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MM.FieldNumber, FAMISFoodStampInformation.MM.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MN.FieldNumber, FAMISFoodStampInformation.MN.GetData)
        'glapiTP8.SubmitField(FAMISFoodStampInformation.MO.FieldNumber, FAMISFoodStampInformation.MO.GetData) --Protected--
        glapiTP8.SubmitField(FAMISFoodStampInformation.MP.FieldNumber, FAMISFoodStampInformation.MP.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MQ.FieldNumber, FAMISFoodStampInformation.MQ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MR.FieldNumber, FAMISFoodStampInformation.MR.GetData)

        glapiTP8.SubmitField(FAMISFoodStampInformation.NA.FieldNumber, FAMISFoodStampInformation.NA.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NB.FieldNumber, FAMISFoodStampInformation.NB.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NC.FieldNumber, FAMISFoodStampInformation.NC.GetData)
        'glapiTP8.SubmitField(FAMISFoodStampInformation.ND.FieldNumber, FAMISFoodStampInformation.ND.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NE.FieldNumber, FAMISFoodStampInformation.NE.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NF.FieldNumber, FAMISFoodStampInformation.NF.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NG.FieldNumber, FAMISFoodStampInformation.NG.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NH.FieldNumber, FAMISFoodStampInformation.NH.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NI.FieldNumber, FAMISFoodStampInformation.NI.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NJ.FieldNumber, FAMISFoodStampInformation.NJ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NK.FieldNumber, FAMISFoodStampInformation.NK.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NL.FieldNumber, FAMISFoodStampInformation.NL.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NM.FieldNumber, FAMISFoodStampInformation.NM.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NN.FieldNumber, FAMISFoodStampInformation.NN.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NO.FieldNumber, FAMISFoodStampInformation.NO.GetData)
        'glapiTP8.SubmitField(FAMISFoodStampInformation.NP.FieldNumber, FAMISFoodStampInformation.NP.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISFoodStampInformation.NQ.FieldNumber, FAMISFoodStampInformation.NQ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NR.FieldNumber, FAMISFoodStampInformation.NR.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NS.FieldNumber, FAMISFoodStampInformation.NS.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NT.FieldNumber, FAMISFoodStampInformation.NT.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NU.FieldNumber, FAMISFoodStampInformation.NU.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NV.FieldNumber, FAMISFoodStampInformation.NV.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.NW.FieldNumber, FAMISFoodStampInformation.NW.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MS.FieldNumber, FAMISFoodStampInformation.MS.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MT.FieldNumber, FAMISFoodStampInformation.MT.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.MU.FieldNumber, FAMISFoodStampInformation.MU.GetData)
    End Sub
    Private Sub Submit_Page6()
        glapiTP8.SubmitField(FAMISFoodStampInformation.OA.FieldNumber, FAMISFoodStampInformation.OA.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OB.FieldNumber, FAMISFoodStampInformation.OB.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OC.FieldNumber, FAMISFoodStampInformation.OC.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OD.FieldNumber, FAMISFoodStampInformation.OD.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OE.FieldNumber, FAMISFoodStampInformation.OE.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OF1.FieldNumber, FAMISFoodStampInformation.OF1.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OG.FieldNumber, FAMISFoodStampInformation.OG.GetData)
        'glapiTP8.SubmitField(FAMISFoodStampInformation.OH.FieldNumber, FAMISFoodStampInformation.OH.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISFoodStampInformation.OI.FieldNumber, FAMISFoodStampInformation.OI.GetData) --Protected--
        glapiTP8.SubmitField(FAMISFoodStampInformation.OJ.FieldNumber, FAMISFoodStampInformation.OJ.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OK.FieldNumber, FAMISFoodStampInformation.OK.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.OM.FieldNumber, FAMISFoodStampInformation.OM.GetData)
        glapiTP8.SubmitField(FAMISFoodStampInformation.ON1.FieldNumber, FAMISFoodStampInformation.ON1.GetData)
        'glapiTP8.SubmitField(FAMISFoodStampInformation.OO.FieldNumber, FAMISFoodStampInformation.OO.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISFoodStampInformation.OP.FieldNumber, FAMISFoodStampInformation.OP.GetData) --Protected--
        glapiTP8.SubmitField(FAMISFoodStampInformation.OQ1.FieldNumber, FAMISFoodStampInformation.OQ1.GetData & FAMISFoodStampInformation.OQ2.GetData)

        glapiTP8.SubmitField(FAMISIandAInformation.PA.FieldNumber, FAMISIandAInformation.PA.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PB.FieldNumber, FAMISIandAInformation.PB.GetData)
        'glapiTP8.SubmitField(FAMISIandAInformation.PC.FieldNumber, FAMISIandAInformation.PC.GetData) --Not Used--
        glapiTP8.SubmitField(FAMISIandAInformation.PD.FieldNumber, FAMISIandAInformation.PD.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PE.FieldNumber, FAMISIandAInformation.PE.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PF.FieldNumber, FAMISIandAInformation.PF.GetData)
        'glapiTP8.SubmitField(FAMISIandAInformation.PG.FieldNumber, FAMISIandAInformation.PG.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISIandAInformation.PH.FieldNumber, FAMISIandAInformation.PH.GetData) --Protected--

        glapiTP8.SubmitField(FAMISIandAInformation.PI.FieldNumber, FAMISIandAInformation.PI.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PJ.FieldNumber, FAMISIandAInformation.PJ.GetData)
        'glapiTP8.SubmitField(FAMISIandAInformation.PK.FieldNumber, FAMISIandAInformation.PK.GetData) --Not Used--
        glapiTP8.SubmitField(FAMISIandAInformation.PL.FieldNumber, FAMISIandAInformation.PL.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PM.FieldNumber, FAMISIandAInformation.PM.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PN.FieldNumber, FAMISIandAInformation.PN.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PP.FieldNumber, FAMISIandAInformation.PP.GetData)
        Thread.Sleep(100)
    End Sub
    Private Sub Submit_RedoChild()
        Dim i As Integer
        Dim QAEntered As String = Nothing
        Dim fromPage As String
        For i = 0 To Redo_totalChildren - 1
            If Redo_ChildNum(i) <> Nothing Then
                If isContinue Then
                    ChildIndex = i
                    QAEntered = FAMISCaseChild(Redo_ChildNum(i)).QA.GetData
                    fromPage = glapiTP8.GetString(77, 1, 78, 1)
                    If i > 0 Then glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                    glapiTP8.SendCommand(FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                    glapiTP8.TransmitPage()
                    Thread.Sleep(500)
                    If glapiTP8.GetString(1, 2, 15, 2) = "INVALID CONTROL" Then
                        glapiTP8.SendCommand("06")
                        glapiTP8.TransmitPage()
                        glapiTP8.TransmitPage()
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                        glapiTP8.SendKeysTransmit(FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                        If glapiTP8.GetString(1, 2, 22, 2) = "PERSON CODE NOT ON OTF" Then
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QA.FieldNumber, "+")
                            glapiTP8.SendCommand("    ")
                        End If
                        setStatusLabel("Submitting Page 07" & vbCrLf & "Person " & FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QF.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QJ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QJ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QK.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QM.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QM.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QN.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QO.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QU.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QU.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RD.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).RE.FieldNumber, FAMISCaseChild(redo_childnum(i)).RE.GetData) --Protected--
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).RF.FieldNumber, FAMISCaseChild(redo_childnum(i)).RF.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RH.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).RH2.FieldNumber, FAMISCaseChild(redo_childnum(i)).RH2.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RJ1.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RJ1.GetData & FAMISCaseChild(Redo_ChildNum(i)).RJ2.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RK.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RM.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RM.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RN.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RO.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RP.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RQ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RR.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RR.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RS.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RS.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RT.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RT.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SF.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SJ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SJ.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).SK.FieldNumber, FAMISCaseChild(redo_childnum(i)).SK.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SL.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).SM.FieldNumber, FAMISCaseChild(redo_childnum(i)).SM.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SN.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SO.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SP.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SQ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SR.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SR.GetData & " ")
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SS.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SS.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).ST.FieldNumber, FAMISCaseChild(redo_childnum(i)).ST.GetData) --Protected--

                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("07", "08", False)
                        If isContinue Then
                            setStatusLabel("Submitting Page 08" & vbCrLf & "Person " & FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                            BGW_ProcessFAMIS.ReportProgress(5)

                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TA.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TB.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TC.FieldNumber, "       ")
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TD.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TE.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TF.FieldNumber, " ")
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TG.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TH.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TI.GetData)
                            'glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TJ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TJ.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TK.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TK.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TL.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TM.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TM.GetData)
                            'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).TN.FieldNumber, FAMISCaseChild(redo_childnum(i)).TN.GetData) --Not Used--
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TO1.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TO1.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TP.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TP.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TQ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TQ.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TR.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TR.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TS.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TS.GetData)

                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UA.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UB.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UC.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UD.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UE.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UF.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UF.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UG.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UH.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UI.GetData)

                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("08", "07", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                    Else
                        GLink_PageErrorCheck(fromPage, "07", False)
                        setStatusLabel("Submitting Page 07" & vbCrLf & "Person " & FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QF.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QJ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QJ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QK.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QM.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QM.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QN.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QO.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).QU.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).QU.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RD.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).RE.FieldNumber, FAMISCaseChild(redo_childnum(i)).RE.GetData) --Protected--
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).RF.FieldNumber, FAMISCaseChild(redo_childnum(i)).RF.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RH.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).RH2.FieldNumber, FAMISCaseChild(redo_childnum(i)).RH2.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RJ1.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RJ1.GetData & FAMISCaseChild(Redo_ChildNum(i)).RJ2.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RK.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RM.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RM.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RN.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RO.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RP.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RQ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RR.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RR.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RS.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RS.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).RT.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).RT.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SF.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SJ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SJ.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).SK.FieldNumber, FAMISCaseChild(redo_childnum(i)).SK.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SL.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).SM.FieldNumber, FAMISCaseChild(redo_childnum(i)).SM.GetData) --Protected--
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SN.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SN.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SO.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SO.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SP.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SQ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SR.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SR.GetData & " ")
                        glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).SS.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).SS.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).ST.FieldNumber, FAMISCaseChild(redo_childnum(i)).ST.GetData) --Protected--

                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("07", "08", False)
                        If isContinue Then
                            setStatusLabel("Submitting Page 08" & vbCrLf & "Person " & FAMISCaseChild(Redo_ChildNum(i)).QA.GetData)
                            BGW_ProcessFAMIS.ReportProgress(5)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TA.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TB.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TC.FieldNumber, "       ")
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TD.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TE.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TF.FieldNumber, " ")
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TG.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TH.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TI.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TJ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TJ.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TK.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TK.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TL.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TL.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TM.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TM.GetData)
                            'glapiTP8.SubmitField(FAMISCaseChild(redo_childnum(i)).TN.FieldNumber, FAMISCaseChild(redo_childnum(i)).TN.GetData) --Not Used--
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TO1.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TO1.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TP.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TP.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TQ.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TQ.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TR.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TR.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).TS.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).TS.GetData)

                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UA.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UA.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UB.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UB.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UC.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UC.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UD.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UD.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UE.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UE.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UF.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UF.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UG.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UG.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UH.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UH.GetData)
                            glapiTP8.SubmitField(FAMISCaseChild(Redo_ChildNum(i)).UI.FieldNumber, FAMISCaseChild(Redo_ChildNum(i)).UI.GetData)

                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("08", "07", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                    End If
                End If
            End If
        Next
        glapiTP8.SendCommand(QAEntered)
        glapiTP8.SubmitField(FAMISCaseChild(0).QA.FieldNumber, QAEntered)
        glapiTP8.TransmitPage()
    End Sub
    Private Sub Submit_Child()
        Dim i As Integer
        Dim QAEntered As String = Nothing
        For i = 0 To numChildren - 1
            If isGoToChild(i) Then
                If isContinue Then
                    ChildIndex = i
                    QAEntered = FAMISCaseChild(i).QA.GetData
                    setStatusLabel("Submitting Page 07" & vbCrLf & "Person " & FAMISCaseChild(i).QA.GetData)
                    glapiTP8.SendCommand(FAMISCaseChild(i).QA.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QA.FieldNumber, FAMISCaseChild(i).QA.GetData)
                    glapiTP8.TransmitPage()
                    If glapiTP8.GetString(1, 2, 22, 2) = "PERSON CODE NOT ON OTF" Then
                        glapiTP8.SubmitField(FAMISCaseChild(i).QA.FieldNumber, "+")
                        glapiTP8.SendCommand("    ")
                    ElseIf glapiTP8.GetField(FAMISCaseChild(i).QA.FieldNumber) <> "+" Then
                        glapiTP8.SubmitField(FAMISCaseChild(i).QA.FieldNumber, FAMISCaseChild(i).QA.GetData)
                        glapiTP8.SendCommand(FAMISCaseChild(i).QA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).QB.FieldNumber, FAMISCaseChild(i).QB.GetData)
                        glapiTP8.TransmitPage()
                    End If
                    glapiTP8.SubmitField(FAMISCaseChild(i).QB.FieldNumber, FAMISCaseChild(i).QB.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QC.FieldNumber, FAMISCaseChild(i).QC.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QD.FieldNumber, FAMISCaseChild(i).QD.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QE.FieldNumber, FAMISCaseChild(i).QE.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QF.FieldNumber, FAMISCaseChild(i).QF.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QG.FieldNumber, FAMISCaseChild(i).QG.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QH.FieldNumber, FAMISCaseChild(i).QH.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QI.FieldNumber, FAMISCaseChild(i).QI.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QJ.FieldNumber, FAMISCaseChild(i).QJ.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QK.FieldNumber, FAMISCaseChild(i).QK.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QL.FieldNumber, FAMISCaseChild(i).QL.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QM.FieldNumber, FAMISCaseChild(i).QM.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QN.FieldNumber, FAMISCaseChild(i).QN.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QO.FieldNumber, FAMISCaseChild(i).QO.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).QU.FieldNumber, FAMISCaseChild(i).QU.GetData)

                    glapiTP8.SubmitField(FAMISCaseChild(i).RA.FieldNumber, FAMISCaseChild(i).RA.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RB.FieldNumber, FAMISCaseChild(i).RB.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RC.FieldNumber, FAMISCaseChild(i).RC.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RD.FieldNumber, FAMISCaseChild(i).RD.GetData)
                    'glapiTP8.SubmitField(FAMISCaseChild(i).RE.FieldNumber, FAMISCaseChild(i).RE.GetData) --Protected--
                    'glapiTP8.SubmitField(FAMISCaseChild(i).RF.FieldNumber, FAMISCaseChild(i).RF.GetData) --Protected--
                    glapiTP8.SubmitField(FAMISCaseChild(i).RG.FieldNumber, FAMISCaseChild(i).RG.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RH.FieldNumber, FAMISCaseChild(i).RH.GetData)
                    'glapiTP8.SubmitField(FAMISCaseChild(i).RH2.FieldNumber, FAMISCaseChild(i).RH2.GetData) --Protected--
                    glapiTP8.SubmitField(FAMISCaseChild(i).RI.FieldNumber, FAMISCaseChild(i).RI.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RJ1.FieldNumber, FAMISCaseChild(i).RJ1.GetData & FAMISCaseChild(i).RJ2.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RK.FieldNumber, FAMISCaseChild(i).RK.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RL.FieldNumber, FAMISCaseChild(i).RL.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RM.FieldNumber, FAMISCaseChild(i).RM.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RN.FieldNumber, FAMISCaseChild(i).RN.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RO.FieldNumber, FAMISCaseChild(i).RO.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RP.FieldNumber, FAMISCaseChild(i).RP.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RQ.FieldNumber, FAMISCaseChild(i).RQ.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RR.FieldNumber, FAMISCaseChild(i).RR.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RS.FieldNumber, FAMISCaseChild(i).RS.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).RT.FieldNumber, FAMISCaseChild(i).RT.GetData)

                    glapiTP8.SubmitField(FAMISCaseChild(i).SA.FieldNumber, FAMISCaseChild(i).SA.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SB.FieldNumber, FAMISCaseChild(i).SB.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SC.FieldNumber, FAMISCaseChild(i).SC.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SD.FieldNumber, FAMISCaseChild(i).SD.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SE.FieldNumber, FAMISCaseChild(i).SE.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SF.FieldNumber, FAMISCaseChild(i).SF.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SG.FieldNumber, FAMISCaseChild(i).SG.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SH.FieldNumber, FAMISCaseChild(i).SH.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SI.FieldNumber, FAMISCaseChild(i).SI.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SJ.FieldNumber, FAMISCaseChild(i).SJ.GetData)
                    'glapiTP8.SubmitField(FAMISCaseChild(i).SK.FieldNumber, FAMISCaseChild(i).SK.GetData) --Protected--
                    glapiTP8.SubmitField(FAMISCaseChild(i).SL.FieldNumber, FAMISCaseChild(i).SL.GetData)
                    'glapiTP8.SubmitField(FAMISCaseChild(i).SM.FieldNumber, FAMISCaseChild(i).SM.GetData) --Protected--
                    glapiTP8.SubmitField(FAMISCaseChild(i).SN.FieldNumber, FAMISCaseChild(i).SN.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SO.FieldNumber, FAMISCaseChild(i).SO.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SP.FieldNumber, FAMISCaseChild(i).SP.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SQ.FieldNumber, FAMISCaseChild(i).SQ.GetData)
                    glapiTP8.SubmitField(FAMISCaseChild(i).SR.FieldNumber, FAMISCaseChild(i).SR.GetData & " ")
                    glapiTP8.SubmitField(FAMISCaseChild(i).SS.FieldNumber, FAMISCaseChild(i).SS.GetData)
                    'glapiTP8.SubmitField(FAMISCaseChild(i).ST.FieldNumber, FAMISCaseChild(i).ST.GetData) --Protected--

                    glapiTP8.TransmitPage()                    
                    GLink_PageErrorCheck("07", "08", False)
                    setStatusLabel("Submitting Page 08" & vbCrLf & "Person " & FAMISCaseChild(i).QA.GetData)
                    If isContinue Then
                        BGW_ProcessFAMIS.ReportProgress(5)

                        glapiTP8.SubmitField(FAMISCaseChild(i).TA.FieldNumber, FAMISCaseChild(i).TA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TB.FieldNumber, FAMISCaseChild(i).TB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TC.FieldNumber, "       ")
                        glapiTP8.SubmitField(FAMISCaseChild(i).TD.FieldNumber, FAMISCaseChild(i).TD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TE.FieldNumber, FAMISCaseChild(i).TE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TF.FieldNumber, " ")
                        glapiTP8.SubmitField(FAMISCaseChild(i).TG.FieldNumber, FAMISCaseChild(i).TG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TH.FieldNumber, FAMISCaseChild(i).TH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TI.FieldNumber, FAMISCaseChild(i).TI.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TJ.FieldNumber, FAMISCaseChild(i).TJ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TK.FieldNumber, FAMISCaseChild(i).TK.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TL.FieldNumber, FAMISCaseChild(i).TL.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TM.FieldNumber, FAMISCaseChild(i).TM.GetData)
                        'glapiTP8.SubmitField(FAMISCaseChild(i).TN.FieldNumber, FAMISCaseChild(i).TN.GetData) --Not Used--
                        glapiTP8.SubmitField(FAMISCaseChild(i).TO1.FieldNumber, FAMISCaseChild(i).TO1.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TP.FieldNumber, FAMISCaseChild(i).TP.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TQ.FieldNumber, FAMISCaseChild(i).TQ.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TR.FieldNumber, FAMISCaseChild(i).TR.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TS.FieldNumber, FAMISCaseChild(i).TS.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TT.FieldNumber, FAMISCaseChild(i).TT.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).TU.FieldNumber, FAMISCaseChild(i).TU.GetData)

                        glapiTP8.SubmitField(FAMISCaseChild(i).UA.FieldNumber, FAMISCaseChild(i).UA.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UB.FieldNumber, FAMISCaseChild(i).UB.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UC.FieldNumber, FAMISCaseChild(i).UC.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UD.FieldNumber, FAMISCaseChild(i).UD.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UE.FieldNumber, FAMISCaseChild(i).UE.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UF.FieldNumber, FAMISCaseChild(i).UF.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UG.FieldNumber, FAMISCaseChild(i).UG.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UH.FieldNumber, FAMISCaseChild(i).UH.GetData)
                        glapiTP8.SubmitField(FAMISCaseChild(i).UI.FieldNumber, FAMISCaseChild(i).UI.GetData)
                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("08", "07", False)
                        BGW_ProcessFAMIS.ReportProgress(5)
                    End If
                End If
            End If
        Next
        If isContinue Then
            glapiTP8.SendCommand(QAEntered)
            glapiTP8.SubmitField(FAMISCaseChild(0).QA.FieldNumber, QAEntered)
            glapiTP8.TransmitPage()
        End If
    End Sub
    Private Sub Submit_Page9()
        Dim i As Integer
        glapiTP8.SubmitField(FAMISFoodStampInformation.OL.FieldNumber, FAMISFoodStampInformation.OL.GetData)
        glapiTP8.SubmitField(FAMISIandAInformation.PO.FieldNumber, FAMISIandAInformation.PO.GetData)
        glapiTP8.SubmitField(FAMISVRPInformation(0).VA.FieldNumber, FAMISVRPInformation(0).VA.GetData)
        glapiTP8.SubmitField(FAMISVRPInformation(0).VC.FieldNumber, FAMISVRPInformation(0).VC.GetData)
        For i = 0 To numVRP - 1
            VRPIndex = i
            glapiTP8.SubmitField(FAMISVRPInformation(i).VE.FieldNumber, FAMISVRPInformation(i).VE.GetData)
            glapiTP8.SubmitField(FAMISVRPInformation(i).VG.FieldNumber, FAMISVRPInformation(i).VG.GetData)
            glapiTP8.SubmitField(FAMISVRPInformation(i).VI.FieldNumber, FAMISVRPInformation(i).VI.GetData)
            glapiTP8.SubmitField(FAMISVRPInformation(i).VK.FieldNumber, FAMISVRPInformation(i).VK.GetData)
            glapiTP8.SubmitField(FAMISVRPInformation(i).VM.FieldNumber, FAMISVRPInformation(i).VM.GetData)
            glapiTP8.SubmitField(FAMISVRPInformation(i).VO.FieldNumber, FAMISVRPInformation(i).VO.GetData)
            glapiTP8.SubmitField(FAMISVRPInformation(i).VQ.FieldNumber, FAMISVRPInformation(i).VQ.GetData)
        Next
        Thread.Sleep(100)
    End Sub
    Private Sub Submit_Page10()
        glapiTP8.SubmitField(FAMISMedicaidInformation.WH.FieldNumber, FAMISMedicaidInformation.WH.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WI.FieldNumber, FAMISMedicaidInformation.WI.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.WK.FieldNumber, FAMISMedicaidInformation.WK.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISMedicaidInformation.WM.FieldNumber, FAMISMedicaidInformation.WM.GetData) --Protected--
        glapiTP8.SubmitField(FAMISMedicaidInformation.WN.FieldNumber, FAMISMedicaidInformation.WN.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WO.FieldNumber, FAMISMedicaidInformation.WO.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WP.FieldNumber, FAMISMedicaidInformation.WP.GetData)
        glapiTP8.SubmitField(FAMISMedicaidInformation.WQ.FieldNumber, FAMISMedicaidInformation.WQ.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.WR.FieldNumber, FAMISMedicaidInformation.WR.GetData) --Protected--
        glapiTP8.SubmitField(FAMISMedicaidInformation.WS.FieldNumber, FAMISMedicaidInformation.WS.GetData)
        'glapiTP8.SubmitField(FAMISMedicaidInformation.WT.FieldNumber, FAMISMedicaidInformation.WT.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISMedicaidInformation.WU.FieldNumber, FAMISMedicaidInformation.WU.GetData) --Protected--
        'glapiTP8.SubmitField(FAMISMedicaidInformation.WV.FieldNumber, FAMISMedicaidInformation.WV.GetData) --Protected--
        Thread.Sleep(100)
    End Sub

    Private Sub CloseCase()
        glapiTP8.SendCommand("ENDCASE")
        glapiTP8.TransmitPage()
        If glapiTP8.GetString(30, 2, 51, 2) <> "BATCH BALANCING SCREEN" Then
            GLink_PageErrorCheck("10", "11", False)
        End If
        BGW_ProcessFAMIS.ReportProgress(85)
    End Sub
    Private Sub CloseBatch()
        BGW_ProcessFAMIS.ReportProgress(21)
        If glapiTP8.GetString(30, 2, 51, 2) = "BATCH BALANCING SCREEN" Then
            Dim CheckBalance As String
            Dim ATPAmount As String = glapiTP8.GetString(45, 11, 55, 11)
            Dim CheckAmount As String = glapiTP8.GetString(45, 10, 55, 10)
            Thread.Sleep(250)
            glapiTP8.SubmitField(26, "00" & CASENUMBER.Substring(1, 6))
            glapiTP8.SubmitField(38, CheckAmount)
            glapiTP8.SubmitField(44, ATPAmount)
            glapiTP8.SendCommand("CHANGE")
            glapiTP8.TransmitPage()
            CheckBalance = glapiTP8.GetString(27, 5, 34, 5)
            If CheckBalance = "BALANCED" Then
                glapiTP8.SendCommand("ENDBATCH")
                glapiTP8.TransmitPage()
                Thread.Sleep(500)
                glapiTP8.SendCommand("HOLD")
                glapiTP8.SubmitField(6, BATCHNUMBER)
                glapiTP8.TransmitPage()
                Thread.Sleep(500)
                glapiTP8.SendCommand("RELE")
                glapiTP8.SubmitField(6, BATCHNUMBER)
                glapiTP8.TransmitPage()
            End If
        End If
    End Sub
    Private Sub CancelCase()
        GLink_Start()
        glapiTP8.SendKeysTransmit("BTCH,DELE," & BATCHNUMBER)
        glapiTP8.Disconnect()
    End Sub
    Private Sub DeleteBatch()
        GLink_Start()
        glapiTP8.SendKeysTransmit("BTCH,DELE," & BATCHNUMBER)
        glapiTP8.Disconnect()
    End Sub
    Private Sub ErrorScreen(ByVal PageError As Boolean)
        Dim Form As New display105Form
        Dim i As Integer

        If Not PageError Then
            isAutoOverride = True
        Else
            isAutoOverride = False
        End If

        For i = 5 To 20
            '--Loop through Page 11 and get each line--
            If glapiTP8.GetString(1, i, 1, i) = "*" Then
                ErrorMessage += glapiTP8.GetString(1, i, 50, i) & vbCrLf
            End If
            If glapiTP8.GetString(2, i, 2, i) = "-" Then
                If glapiTP8.GetString(3, i, 3, i) <> "W" Or glapiTP8.GetString(3, i, 5, i) = "W41" Or glapiTP8.GetString(3, i, 5, i) = "W06" Then
                    isAutoOverride = False
                End If
                If isAutoOverride = False Then ErrorMessage += glapiTP8.GetString(1, i, 80, i) & vbCrLf
            End If
        Next

        If Not isAutoOverride Then
            If isCaseExist() Then Form.isCaseOnSQL = True Else Form.isCaseOnSQL = False
            Form.BATCHNUMBER = BATCHNUMBER
            Form.CASENUMBER = CASENUMBER
            Form.glapiTP8 = glapiTP8
            Form.ErrorMessage = ErrorMessage
            Form.isPreview = isView105
            Form.isFileSource = isFileSource
            Form.BATCHNUMBER = BATCHNUMBER
            Form.isPageError = PageError
            If PageError Then Form.isOverride = False Else Form.isOverride = True
            Form.ParentForm_Processing = Me
            BGW_ProcessFAMIS.ReportProgress(2)
            Form.ShowDialog()
            BGW_ProcessFAMIS.ReportProgress(3)
        Else
            isOverride = True
        End If
    End Sub

    Private Sub setStatusLabel(ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the
        ' calling thread to the thread ID of the creating thread.
        ' If these threads are different, it returns true.
        If lbl_Status.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf setStatusLabel)
            Me.Invoke(d, New Object() {[text]})
        Else
            lbl_Status.Text = [text]
        End If
    End Sub
    Private Sub setProgressMax()
        Dim tempVRP, tempChildren As Integer
        If numChildren > 0 Then tempChildren = 1 Else tempChildren = 0
        If numVRP > 0 Then tempVRP = 1 Else tempVRP = 0
        progressbar_FAMIS.Maximum = (10 + (2 * numChildren + tempChildren) + (tempVRP)) * 10  '--Progress bar steps are in units of 10 and we set the max to the number of steps needed-- 
        If progressbar_FAMIS.Maximum < 100 Then progressbar_FAMIS.Maximum = 100 '--Progress bar has a minimum value of 100--  
    End Sub

    Private Sub AbortProcessing()
        BGW_ProcessFAMIS.ReportProgress(77)
        BGW_ProcessFAMIS.CancelAsync()
    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click
        If MessageBox.Show("Cancel Case: " & CASENUMBER & "?", "Cancel Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            progressbar_FAMIS.Style = ProgressBarStyle.Marquee
            btn_Cancel.Enabled = False
            lbl_Status.Text = "Canceling Case: " & CASENUMBER
            glapiTP8.bool_Cancel = True
            glapiTP8.Disconnect()
            BGW_Cancel.RunWorkerAsync()
            ProcessingThread.Abort()
        End If
    End Sub

    Private Sub BGW_Cancel_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_Cancel.DoWork
        Thread.Sleep(1000)
        CancelCase()
        glapiTP8 = Nothing
    End Sub
    Private Sub BGW_Cancel_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGW_Cancel.RunWorkerCompleted
        ParentForm_Put105.isCaseCancel = True
        Me.Close()
    End Sub
    Private Sub BGW_ProcessFAMIS__DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_ProcessFAMIS.DoWork
        Dim fromPage As String '--Keeps track of the FAMIS page we transmit from--
        Dim Timeout As Integer = 0
        Dim isVRP As Boolean
        Dim i As Integer
        '--Check whether OL, PO, VC contain data to enter VRP information--
        For i = 0 To numVRP - 1
            If FAMISFoodStampInformation.OL.GetData = "       " And FAMISIandAInformation.PO.GetData = "       " And FAMISVRPInformation(i).VC.GetData = " " Then
                isVRP = False
            Else
                isVRP = True
                Exit For
            End If
        Next
        '--Capture the Background Worker thread so we can abort it instantly if the user cancels--
        ProcessingThread = Thread.CurrentThread
        isContinue = True
        ParentForm_Put105.isCaseCancel = False
        ParentForm_Put105.isCaseSuccessful = False
        ParentForm_Put105.isLoginError = False
        ParentForm_Put105.isCaseError = False
        isError = False
        isContinue = True

        BGW_ProcessFAMIS.ReportProgress(10)
        ParentForm_Put105.BlankOutMedicaidFields()
        GLink_Start()
        If Not BGW_ProcessFAMIS.CancellationPending And Not isError Then
            'ParentForm_Put105.isLoginError = False '--11/30/07 Removed because I dont know why it's here--
            CheckCaseStatus()
            BGW_ProcessFAMIS.ReportProgress(20)
            CreateBatch()
            Thread.Sleep(500)
            BGW_ProcessFAMIS.ReportProgress(30)
            If isContinue Then
                Dim isClockAlert As Boolean = True  '--The clock warning on Page 1 of FAMIS requires an extra transmit to go through--
                Submit_Page1()
                glapiTP8.TransmitPage()
                While isClockAlert
                    If glapiTP8.GetString(1, 2, 6, 2) = "B-ADLT" Or glapiTP8.GetString(1, 2, 5, 2) = "AADLT" Or glapiTP8.GetString(1, 2, 6, 2) = "BSANC=" Or glapiTP8.GetString(1, 2, 6, 2) = "ASANC=" Then
                        glapiTP8.TransmitPage()
                        Thread.Sleep(1000)
                    Else
                        isClockAlert = False
                    End If
                End While
                GLink_PageErrorCheck("01", "02", True)
                BGW_ProcessFAMIS.ReportProgress(5)
            End If

            If isContinue Then
                Submit_Page2()
                glapiTP8.TransmitPage()
                GLink_PageErrorCheck("02", "03", True)
                BGW_ProcessFAMIS.ReportProgress(5)
            End If

            If isContinue Then
                Submit_Page3()
                glapiTP8.TransmitPage()
                GLink_PageErrorCheck("03", "04", True)
                BGW_ProcessFAMIS.ReportProgress(5)
            End If

            If isContinue Then
                Submit_Page4()
                glapiTP8.TransmitPage()
                GLink_PageErrorCheck("04", "05", True)
                BGW_ProcessFAMIS.ReportProgress(5)
            End If

            If isContinue Then
                Submit_Page5()
                glapiTP8.TransmitPage()
                GLink_PageErrorCheck("05", "06", True)
                BGW_ProcessFAMIS.ReportProgress(5)
            End If

            If isContinue Then
                Submit_Page6()
            End If

            If isContinue Then
                If numChildren > 0 Then
                    glapiTP8.TransmitPage()
                    GLink_PageErrorCheck("06", "07", True)
                    BGW_ProcessFAMIS.ReportProgress(5)
                    Submit_Child()
                End If
            End If

            If isContinue Then
                If numVRP > 0 And isVRP Then
                    glapiTP8.SendCommand("09")
                    glapiTP8.TransmitPage()
                    If numChildren = 0 Then
                        '--Did we enter child information or are here from page 6--
                        GLink_PageErrorCheck("06", "09", True)
                    Else
                        GLink_PageErrorCheck("07", "09", True)
                    End If
                    BGW_ProcessFAMIS.ReportProgress(5)

                    If isContinue Then
                        Submit_Page9()
                        glapiTP8.TransmitPage()
                        GLink_PageErrorCheck("09", "10", True)
                        BGW_ProcessFAMIS.ReportProgress(5)
                    End If
                End If
            End If

            If isContinue Then
                If numVRP = 0 Or Not isVRP Then
                    '--If there is no VRP then just proceed to Page 10--
                    'BGW_ProcessFAMIS.ReportProgress(32)
                    glapiTP8.SendCommand("10")
                    glapiTP8.TransmitPage()
                    GLink_PageErrorCheck("06", "10", True)
                    BGW_ProcessFAMIS.ReportProgress(5)
                End If
            End If

            If isContinue Then
                Submit_Page10()
                BGW_ProcessFAMIS.ReportProgress(5)
            End If
            While isContinue
                CloseCase()
                ErrorMessage = Nothing
                isContinue = False
                Thread.Sleep(500)
                If glapiTP8.GetString(77, 1, 78, 1) = "11" Then
                    BGW_ProcessFAMIS.ReportProgress(90)
                    ErrorScreen(False)
                    BGW_ProcessFAMIS.ReportProgress(15)
                    If isOverride Then
                        If glapiTP8.bool_Visible Then glapiTP8.SetVisible(False)
                        If isAutoOverride = True Then BGW_ProcessFAMIS.ReportProgress(22) Else BGW_ProcessFAMIS.ReportProgress(23)
                        Thread.Sleep(1500)
                        If glapiTP8.bool_Visible Then glapiTP8.bool_Visible = False
                        glapiTP8.SendKeysTransmit("OVER")
                        If glapiTP8.GetString(30, 2, 51, 2) = "BATCH BALANCING SCREEN" Then
                            CloseBatch()
                            ParentForm_Put105.isCaseSuccessful = True
                            Try
                                glapiTP8.Disconnect()
                            Catch ex As Exception
                                'Dim SQLConn As New SqlConnection(My.Settings.phxSQLConn)
                                'Dim SQLComm As New SqlCommand
                                'SQLComm.Connection = SQLConn
                                'Try
                                '    SQLConn.Open()
                                '    SQLComm.CommandText = "INSERT INTO BugReport VALUES ('" & My.Settings.FAMISOperatorID & "', '" & Date.Now.Month & "/" & Date.Now.Day & "/" & Date.Now.Year & "', '" & Date.Now.Hour & ":" & Date.Now.Minute & "', '" & CASENUMBER & "', 'Disconnect Error - BOOYA!')"
                                '    SQLComm.ExecuteNonQuery()
                                'Catch et As Exception
                                'Finally
                                '    SQLConn.Close()
                                'End Try
                            End Try
                            StoreSQL()
                        End If
                    ElseIf isCancelCase Then
                        isContinue = False
                        BGW_ProcessFAMIS.ReportProgress(99)
                    ElseIf isContinue Then
                        If glapiTP8.bool_Visible Then glapiTP8.SetVisible(False)
                        If isRedo_Page1 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("01")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "01", True)
                            Submit_Page1()
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("01", "02", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page2 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("02")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "02", True)
                            Submit_Page2()
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("02", "03", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page3 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("03")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "03", True)
                            Submit_Page3()
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("03", "04", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page4 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("04")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "04", True)
                            Submit_Page4()
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("04", "05", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page5 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("05")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "05", True)
                            Submit_Page5()
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("05", "06", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page6 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("06")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "06", True)
                            Submit_Page6()
                            glapiTP8.SendCommand("10")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("06", "10", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If Redo_ChildNum(0) <> Nothing Then
                            isRedo = True
                            Submit_RedoChild()
                            'BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page9 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("09")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "09", True)
                            Submit_Page9()
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck("09", "10", False)
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                        If isRedo_Page10 Then
                            isRedo = True
                            fromPage = glapiTP8.GetString(77, 1, 78, 1)
                            glapiTP8.SendCommand("10")
                            glapiTP8.TransmitPage()
                            GLink_PageErrorCheck(fromPage, "10", False)
                            BGW_ProcessFAMIS.ReportProgress(31)
                            Submit_Page10()
                            BGW_ProcessFAMIS.ReportProgress(5)
                        End If
                    End If
                    'ParentForm_Put105.TEMPLocation = "finished retrying case entry"
                Else
                    CloseBatch()
                    ParentForm_Put105.isCaseSuccessful = True
                    glapiTP8.Disconnect()
                    StoreSQL()
                End If
            End While
        Else
            glapiTP8.Disconnect()
            ParentForm_Put105.isLoginError = True
        End If
        If ParentForm_Put105.isCaseSuccessful Then VerifySQL()
    End Sub
    Private Sub BGW_ProcessFAMIS_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BGW_ProcessFAMIS.ProgressChanged
        Select Case e.ProgressPercentage
            Case 2      '--Minimize form--
                If WindowState = FormWindowState.Normal Then WindowState = FormWindowState.Minimized
            Case 3      '--Restate form--
                If WindowState = FormWindowState.Minimized Then WindowState = FormWindowState.Normal
            Case 5      '--Increment progressbar
                progressbar_FAMIS.Value += 10
            Case 10     '--Set progress bar to Marque and report connection to FAMIS--
                progressbar_FAMIS.Style = ProgressBarStyle.Marquee
                setStatusLabel("Connecting to FAMIS...")
            Case 15
                progressbar_FAMIS.Value = 0
                progressbar_FAMIS.Maximum = (Redo_totalPages + (Redo_totalChildren * 2)) * 10
            Case 18
                lbl_Status.Text = "Case not sent to SQL. Trying again..."
            Case 19
                lbl_Status.Text = "Verifying Case..."
            Case 20     '--Set progress bar back to blocks for showing steps during processing--
                progressbar_FAMIS.Style = ProgressBarStyle.Blocks
            Case 21     '--Set progress bar to marque and let the user know we are finishing the case--
                progressbar_FAMIS.MarqueeAnimationSpeed = 100
                progressbar_FAMIS.Style = ProgressBarStyle.Marquee
                lbl_Status.Text = "Finalizing Case..."
                btn_Cancel.Enabled = False
            Case 22     '--Auto overriding case--
                progressbar_FAMIS.Style = ProgressBarStyle.Marquee
                lbl_Status.Text = "Auto Overriding 'W' Level Errors..."
            Case 23
                lbl_Status.Text = "Overriding..."
            Case 30
                btn_Cancel.Enabled = True
                lbl_Status.Text = "Submitting Page 01"
            Case 31
                lbl_Status.Text = "Submitting Page 10"
            Case 32
                lbl_Status.Text = "Submitting Page 06"
            Case 77     '--Processing cancelled--
                progressbar_FAMIS.Style = ProgressBarStyle.Blocks
                progressbar_FAMIS.Value = 0
            Case 85     '--Reset progressbar max after retry--
                progressbar_FAMIS.Value = progressbar_FAMIS.Maximum
            Case 90
                lbl_Status.Text = "Field Errors Found in Case"
            Case 97 '--Cancel case from verifying SQL--
                isContinue = False
                lbl_Status.Text = "Canceling Case: " & CASENUMBER
                ParentForm_Put105.WriteLog("Cancelled case: " & CASENUMBER, False)
                ParentForm_Put105.isCaseCancel = True
            Case 98     '--SQL error--
                lbl_Status.Text = "Database Error!"
                ParentForm_Put105.isCaseError = True
            Case 99     '--Cancel case from 105 form--
                progressbar_FAMIS.MarqueeAnimationSpeed = 100
                progressbar_FAMIS.Style = ProgressBarStyle.Marquee
                isContinue = False
                btn_Cancel.Enabled = False
                lbl_Status.Text = "Canceling Case: " & CASENUMBER
                ParentForm_Put105.WriteLog("Cancelled case: " & CASENUMBER, False)
                ParentForm_Put105.isCaseCancel = True
                glapiTP8.Disconnect()
                BGW_Cancel.RunWorkerAsync()
                While BGW_Cancel.IsBusy
                    Application.DoEvents()
                    Thread.Sleep(500)
                End While
        End Select
    End Sub
    Private Sub BGW_ProcessFAMIS_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGW_ProcessFAMIS.RunWorkerCompleted
        Me.Close()
    End Sub
End Class