Public Class processing105
    Public FAMISCaseInformation As CaseInformation
    Public FAMISApplicationInformation As ApplicationInformation
    Public FAMISIndividualsInformation As IndividualsInformation
    Public FAMISMedicaidInformation As MedicaidInformation
    Public FAMISTANFInformation As TANFInformation
    Public FAMISIncomeInformation As IncomeInformation
    Public FAMISFoodStampInformation As FoodStampInformation
    Public FAMISIandAInformation As IandAInformation
    Public FAMISVRPInformation(35) As VRPInformation
    Public FAMISCaseChild(35) As CaseChild
    Public numChildren, numVRP As Integer
    Public LineNP As String             '--String variable holding the user choice of A or U Batch 

    Private TEXT_CaseInformation, TEXT_ApplicationInformation, TEXT_IndividualsInformation, TEXT_MedicaidInformation, TEXT_AFDCInformation, TEXT_IncomeInformation, TEXT_FoodStampInformation, TEXT_IandAInformation, TEXT_VRPInformation(35), TEXT_CaseChild(35) As String
    Private FileName As String          '--Text file name--
    Private CTRL As Control             '--Global setting for invoke procedure--
    Private BATCHNUMBER As String       '--Batch Number

    Public isCaseCancel As Boolean      '--Boolean to track if the user canceled the case--

    Private Sub frm105_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        setStatus("Stopped", Color.OrangeRed)
    End Sub

    Private Sub setInfo(ByVal StringToSend As String, ByVal isError As Boolean)
        If isError = False Then
            txt_Info.Text = Date.Today.Month.ToString & "/" & Date.Today.Day.ToString & "/" & Date.Today.Year.ToString & " " & Date.Now.TimeOfDay.Hours.ToString & ":" & Date.Now.TimeOfDay.Minutes.ToString & ":" & Date.Now.Second & ">> " & StringToSend & vbCrLf & txt_Info.Text
            'setText(txt_Info, StringToSend)
        Else
            txt_Info.Text = Date.Today.Month.ToString & "/" & Date.Today.Day.ToString & "/" & Date.Today.Year.ToString & " " & Date.Now.TimeOfDay.Hours.ToString & ":" & Date.Now.TimeOfDay.Minutes.ToString & ":" & Date.Now.Second & ">> ERROR: " & StringToSend & vbCrLf & txt_Info.Text
            'setText(txt_Info, StringToSend)
        End If
        txt_Info.Select(0, 0)
    End Sub
    Private Sub setStatus(ByVal Message As String, ByVal bgColor As Color)
        txt_Status.Text = Message
        txt_Status.BackColor = bgColor
        txt_Info.Focus()
        txt_Info.Select(0, 0)
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
    End Sub

#Region "GUMP Functions"
    Private Sub BGW_GUMPProcess_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_GUMPProcess.DoWork
        Dim Directory As New DirectoryInfo(My.Settings.FilePath_GUMP)
        Dim FileList() As FileInfo
        Dim i, z, MAX, timeout As Integer
        Dim frmProcess As processingFAMIS
        Dim frmNPDialog As NPDialog
        While Not BGW_GUMPProcess.CancellationPending
            If Directory.Exists = True Then
                FileList = Directory.GetFiles("*.txt")
                If FileList.Length > 0 Then
                    MAX = FileList.Length
                    For i = 0 To MAX - 1
                        FileName = FileList(i).Name
                        If FileName.Substring(0, 6) = My.Settings.FAMISOperatorID Then
                            '--Start reading in the file in the background to save time--
                            BGW_ReadGUMP.RunWorkerAsync()
                            '--Ask the user what kind of case this is--
                            frmNPDialog = New NPDialog
                            frmNPDialog.CaseNumber = FileName.Replace(".TXT", "").Substring(5)
                            frmNPDialog.ParentForm = Me
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
                            '--Waiting for the case to finish being read in--
                            timeout = 0
                            While BGW_ReadGUMP.IsBusy
                                Application.DoEvents()
                                Thread.Sleep(500)
                                timeout += 1
                                If timeout = 30 Then
                                    '--Background Worker seems to be stuck setting the GUMP information--
                                    BGW_GUMPProcess.ReportProgress(30)
                                End If
                            End While
                            If LineNP <> "Cancel" Then
                                '--Sending the GUMP info to FAMIS for processing--
                                BGW_GUMPProcess.ReportProgress(40)
                                frmProcess = New processingFAMIS
                                frmProcess.FAMISApplicationInformation = FAMISApplicationInformation
                                frmProcess.FAMISCaseInformation = FAMISCaseInformation
                                frmProcess.FAMISFoodStampInformation = FAMISFoodStampInformation
                                frmProcess.FAMISIandAInformation = FAMISIandAInformation
                                frmProcess.FAMISIncomeInformation = FAMISIncomeInformation
                                frmProcess.FAMISIndividualsInformation = FAMISIndividualsInformation
                                frmProcess.FAMISMedicaidInformation = FAMISMedicaidInformation
                                frmProcess.FAMISTANFInformation = FAMISTANFInformation
                                For z = 0 To numChildren - 1
                                    frmProcess.FAMISCaseChild(z) = FAMISCaseChild(z)
                                Next
                                For z = 0 To numVRP - 1
                                    frmProcess.FAMISVRPInformation(z) = FAMISVRPInformation(z)
                                Next
                                frmProcess.numChildren = numChildren
                                frmProcess.numVRP = numVRP
                                frmProcess.BATCHNUMBER = BATCHNUMBER
                                frmProcess.ParentForm = Me
                                frmProcess.ShowDialog()
                                BGW_GUMPProcess.ReportProgress(50)
                                If isCaseCancel = True Then
                                    '--Case was canceled by the user--
                                    BGW_GUMPProcess.ReportProgress(60)
                                Else
                                    '--Case successful--
                                    setBatchNumber(BATCHNUMBER.Substring(0, 1))
                                End If
                                If BGW_GUMPProcess.CancellationPending Then
                                    '--The user or application pressed the stop button--
                                    '--Force the for loop to end--
                                    i = MAX
                                End If
                            End If
                        End If
                        If LineNP <> "Cancel" Then
                            If File.Exists(My.Settings.FilePath_GUMP & FileName) Then
                                File.Delete(My.Settings.FilePath_GUMP & FileName)
                            End If
                        Else
                            If BGW_ReadGUMP.IsBusy Then BGW_ReadGUMP.CancelAsync()
                        End If
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
            Case 10     '--NP Dialog starting unenable main form--
                Me.Enabled = False
            Case 20     '--NP Dialog error--
                BGW_GUMPProcess.CancelAsync()
                setInfo("Unknown error in Line N/P dialog!", True)
            Case 30     '--BGW ReadGUMP error--
                BGW_GUMPProcess.CancelAsync()
                setInfo("Unable to read GUMP file!", True)
            Case 40     '--Begin processing case--
                setInfo("Processing GUMP Case: " & FileName.Replace(".TXT", "").Substring(5), False)
            Case 50     '--Case finished processing--
                Me.Enabled = True
                Me.Focus()
            Case 60     '--Report case was cancelled--
                setInfo(FAMISCaseInformation.AA.GetData & " was canceled.", False)
            Case 70     '--Halt on directory error--
                setInfo("Directory does not exist!", True)
            Case 100    '--Thread done reset main screens buttons--
                btn_GUMPStop.Enabled = False
                btn_GUMPStart.Enabled = True
                rdo_GUMP.Enabled = True
                rdo_Exist.Enabled = True
                rdo_Manual.Enabled = True
                Me.Enabled = True
                Me.Focus()
                setInfo("GUMP processing stopped.", False)
                setStatus("Stopped", Color.OrangeRed)
        End Select
    End Sub
    Private Sub BGW_ReadGUMP_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGW_ReadGUMP.DoWork
        Read_TextFile()
        setBlockData_Regular()
        setBlockData_Child()
        setBlockData_VRP()
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
        getBlockData(FAMISApplicationInformation.BY)
        getBlockData(FAMISApplicationInformation.BZ)

        getBlockData(FAMISApplicationInformation.CA)
        'getBlockData(FAMISApplicationInformation.CB)
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
        getBlockData(FAMISIndividualsInformation.FG1)
        getBlockData(FAMISIndividualsInformation.FG2)
        getBlockData(FAMISIndividualsInformation.FG3)
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
        getBlockData(FAMISIndividualsInformation.FP1)
        getBlockData(FAMISIndividualsInformation.FP2)
        getBlockData(FAMISIndividualsInformation.FP3)

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
        getBlockData(FAMISMedicaidInformation.WD)
        getBlockData(FAMISMedicaidInformation.WE)
        getBlockData(FAMISMedicaidInformation.WF)
        getBlockData(FAMISMedicaidInformation.WG)
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
        getBlockData(FAMISFoodStampInformation.OM)
        getBlockData(FAMISFoodStampInformation.ON1)
        getBlockData(FAMISFoodStampInformation.OO)
        'getBlockData(FAMISFoodStampInformation.OP) --Not in XML or Text File--

        getBlockData(FAMISFoodStampInformation.WX)
        getBlockData(FAMISFoodStampInformation.WY)

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
    End Sub
    Private Sub setBlockData_Child()
        Dim i As Integer
        For i = 0 To numChildren - 1
            FAMISCaseChild(i) = New CaseChild

            getBlockData(FAMISCaseChild(i).QA, i)
            getBlockData(FAMISCaseChild(i).QB, i)
            getBlockData(FAMISCaseChild(i).QC, i)
            getBlockData(FAMISCaseChild(i).QD, i)
            getBlockData(FAMISCaseChild(i).QE1, i)
            getBlockData(FAMISCaseChild(i).QE2, i)
            getBlockData(FAMISCaseChild(i).QF, i)
            getBlockData(FAMISCaseChild(i).QG, i)
            getBlockData(FAMISCaseChild(i).QH, i)
            getBlockData(FAMISCaseChild(i).QI1, i)
            getBlockData(FAMISCaseChild(i).QI2, i)
            getBlockData(FAMISCaseChild(i).QK, i)
            getBlockData(FAMISCaseChild(i).QL, i)
            getBlockData(FAMISCaseChild(i).QM, i)
            getBlockData(FAMISCaseChild(i).QN, i)
            getBlockData(FAMISCaseChild(i).QO, i)

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
            getBlockData(FAMISCaseChild(i).TI1, i)
            getBlockData(FAMISCaseChild(i).TI2, i)
            getBlockData(FAMISCaseChild(i).TI3, i)
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
            If FAMISCaseChild(i).UF.GetData = "ap" Then FAMISCaseChild(i).UF.SetData("  ")
            getBlockData(FAMISCaseChild(i).UG, i)
            getBlockData(FAMISCaseChild(i).UH, i)
            getBlockData(FAMISCaseChild(i).UI, i)
            getBlockData(FAMISCaseChild(i).UJ, i)
            getBlockData(FAMISCaseChild(i).UK, i)
            getBlockData(FAMISCaseChild(i).UL, i)

            ' getBlockData(FAMISCaseChild(i).YA) --Not in XML or Text File--
        Next
    End Sub
    Private Sub setBlockData_VRP()
        Dim i As Integer
        For i = 0 To numVRP - 1
            FAMISVRPInformation(i) = New VRPInformation(i)

            getBlockData(FAMISVRPInformation(i).VE, i)
            getBlockData(FAMISVRPInformation(i).VG, i)
            getBlockData(FAMISVRPInformation(i).VI, i)
            getBlockData(FAMISVRPInformation(i).VK, i)
            getBlockData(FAMISVRPInformation(i).VM, i)
            getBlockData(FAMISVRPInformation(i).VO, i)
        Next
    End Sub
#End Region

    Private Sub rdo_GUMP_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdo_GUMP.CheckedChanged
        grp_ManualControls.Visible = False
        grp_ExistingControls.Visible = False
        grp_GUMPControls.Visible = True
        Me.AcceptButton = btn_GUMPStart
        txt_Status.Text = "Stopped"
        txt_Status.BackColor = Color.Red
        btn_GUMPStop.Enabled = False
    End Sub
    Private Sub rdo_Exist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdo_Exist.CheckedChanged
        grp_ManualControls.Visible = False
        grp_ExistingControls.Visible = True
        grp_GUMPControls.Visible = False
        Me.AcceptButton = btn_Search
    End Sub
    Private Sub rdo_Manual_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdo_Manual.CheckedChanged
        grp_ManualControls.Visible = True
        grp_ExistingControls.Visible = False
        grp_GUMPControls.Visible = False
        Me.AcceptButton = btn_ManualStart_A
    End Sub

    Private Sub btn_GUMPStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_GUMPStart.Click
        btn_GUMPStart.Enabled = False
        btn_GUMPStop.Enabled = True
        rdo_GUMP.Enabled = False
        rdo_Exist.Enabled = False
        rdo_Manual.Enabled = False
        setInfo("GUMP processing started.", False)
        setStatus("Running", Color.Green)
        BGW_GUMPProcess.RunWorkerAsync()
    End Sub
    Private Sub btn_GUMPStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_GUMPStop.Click
        btn_GUMPStop.Enabled = False
        setStatus("Stopping", Color.Orange)
        BGW_GUMPProcess.CancelAsync()
    End Sub
    Private Sub btn_Search_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Search.Click

    End Sub
    Private Sub btn_ManualStart_A_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ManualStart_A.Click

    End Sub
    Private Sub btn_ManulStart_U_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ManulStart_U.Click

    End Sub
End Class