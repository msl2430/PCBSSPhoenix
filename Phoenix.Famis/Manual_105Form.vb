Public Class Manual_105Form
    Public ParentForm_Put105 As Put105Thru
    Public ParentForm_Processing As processingFAMIS

    Public numChildren, numVRP As Integer       '--Number of total children and VRP payments--
    Public ChildNumber, VRPNumber As Integer    '--Index of the children and VRP payment being shown on the form--
    Public ErrorMessage As String
    Public BATCHNUMBER As String
    Public glapiTP8 As connGLinkTP8
    Public CASENUMBER As String
    Public isPageError As Boolean
    Public isCaseOnSQL As Boolean
    Public isOverride As Boolean
    Public isPreview As Boolean                 '--Tracks whether this is a preview or not--

    Private isChildChanging As Boolean = False  '--Tracks when the selected child is being changed by the user--
    Private isSourceChanging As Boolean = False '--Tracks when the data source is changed by the user--
    Private isFirstRun_Child As Boolean         '--Tracks if it's the first time the form is loaded for the Child page--
    Private isFirstRun_VRP As Boolean           '--Tracks if it's the first time the form is loaded for the VRP page--
    Private isFirstRun_Load As Boolean = True   '--Tracks if it's the first time the form is loaded--

    Private TotalChangeCount As Integer = 0     '--Number of total pages that have been changed--
    Private ChildChangeCount As Integer = 0     '--Number of children that information has been changed by the user--
    Private ClosingCondition As String
    Private PersonCodes() As String = {"C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}

    Private Sub Manual_105Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If numChildren > 0 Then ChildNumber = 0
        If numVRP > 0 Then VRPNumber = 0
        isFirstRun_Child = True
        isFirstRun_VRP = True
        isFirstRun_Load = True

        Fill_FormB_VRP()

        GetFromDatabase()
        If numChildren > 0 Then FillFormB(ChildNumber)
        If numVRP > 0 Then FillFormC(VRPNumber)

        TabControl1.SelectedTab = tab_FoodIandA
        TabControl1.SelectedTab = tab_IndivMedi
        TabControl1.SelectedTab = tab_TanfIncome
        TabControl1.SelectedTab = tab_CaseApp
        Transfer105A()
        Transfer105A1()

        AddEvents(Me)
        isFirstRun_Load = False
    End Sub
    Private Sub Manual_105FormFormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Dim i As Integer
        Transfer105A()
        Transfer105A1()
        If numChildren > 0 Then TransferFormB(ChildNumber)
        'If numVRP > 0 Then TransferFormC(VRPNumber)
        'ParentForm_Put105.numChildren = numChildren
        'ParentForm_Put105.numVRP = numVRP
        'ParentForm_Put105.FAMISApplicationInformation = FAMISApplicationInformation
        'ParentForm_Put105.FAMISCaseInformation = FAMISCaseInformation
        'ParentForm_Put105.FAMISFoodStampInformation = FAMISFoodStampInformation
        'ParentForm_Put105.FAMISIandAInformation = FAMISIandAInformation
        'ParentForm_Put105.FAMISIncomeInformation = FAMISIncomeInformation
        'ParentForm_Put105.FAMISIndividualsInformation = FAMISIndividualsInformation
        'ParentForm_Put105.FAMISMedicaidInformation = FAMISMedicaidInformation
        'ParentForm_Put105.FAMISTANFInformation = FAMISTANFInformation
        'If numChildren > 0 Then
        '    For i = 0 To numChildren - 1
        '        ParentForm_Put105.FAMISCaseChild(i) = FamisCaseChild(i)
        '    Next
        'End If
        'If numVRP > 0 Then
        '    For i = 0 To numVRP - 1
        '        ParentForm_Put105.FAMISVRPInformation(i) = FAMISVRPInformation(i)
        '    Next
        'End If
    End Sub

    Private Sub Fill_FormB_VRP()
        Dim SQLConn As New SqlConnection(My.Settings.phxSQLConn)
        Dim SQLComm As New SqlCommand
        Dim SQLReader As SqlDataReader
        Dim i As Integer
        If numChildren > 0 Then
            For i = 0 To numChildren - 1
                FamisCaseChild(i) = New CaseChild
            Next
            SQLConn.Open()
            i = 0
            SQLComm.Connection = SQLConn
            SQLComm.CommandText = "SELECT CaseNumber, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR, TA, TB, TI, TJ, TF, TK, TD, UB, RN, RO, RQ, SA, SB, SC, SD, SE, SF, SG, SH, SJ, SK, SL, SM, SO, SP, SI, SS, TG, TH, TL, TM, TO1, TP, TQ, TR, TS, UA, UC, UD, UE, UF, UG, UH, UK, UL, TC, TE, UI FROM FAMISCaseChild WHERE CASENUMBER = '" + CASENUMBER + "'"
            SQLReader = SQLComm.ExecuteReader
            While SQLReader.Read
                FamisCaseChild(i).QA.SetData(SQLReader.GetString(1))
                FamisCaseChild(i).QB.SetData(SQLReader.GetString(2))
                FamisCaseChild(i).QC.SetData(SQLReader.GetString(3))
                FamisCaseChild(i).QD.SetData(SQLReader.GetString(4))
                FamisCaseChild(i).QE.SetData(SQLReader.GetString(5))
                FamisCaseChild(i).QF.SetData(SQLReader.GetString(6))
                FamisCaseChild(i).QG.SetData(SQLReader.GetString(7))
                FamisCaseChild(i).QH.SetData(SQLReader.GetString(8))
                FamisCaseChild(i).QI.SetData(SQLReader.GetString(9))
                FamisCaseChild(i).QJ.SetData(SQLReader.GetString(10))
                FamisCaseChild(i).QK.SetData(SQLReader.GetString(11))
                FamisCaseChild(i).QL.SetData(SQLReader.GetString(12))
                FamisCaseChild(i).QM.SetData(SQLReader.GetString(13))
                FamisCaseChild(i).QN.SetData(SQLReader.GetString(14))
                FamisCaseChild(i).QO.SetData(SQLReader.GetString(15))
                FamisCaseChild(i).RA.SetData(SQLReader.GetString(16))
                FamisCaseChild(i).RB.SetData(SQLReader.GetString(17))
                FamisCaseChild(i).RC.SetData(SQLReader.GetString(18))
                FamisCaseChild(i).RD.SetData(SQLReader.GetString(19))
                FamisCaseChild(i).RE.SetData(SQLReader.GetString(20))
                FamisCaseChild(i).RF.SetData(SQLReader.GetString(21))
                FamisCaseChild(i).RG.SetData(SQLReader.GetString(22))
                FamisCaseChild(i).RH.SetData(SQLReader.GetString(23))
                'FamisCaseChild(i).RH2.SetData(SQLReader.GetString(24))      '--Temp until complete removal--
                FamisCaseChild(i).RI.SetData(SQLReader.GetString(25))
                FamisCaseChild(i).RJ1.SetData(SQLReader.GetString(26))
                FamisCaseChild(i).RJ2.SetData(SQLReader.GetString(27))
                FamisCaseChild(i).RK.SetData(SQLReader.GetString(28))
                FamisCaseChild(i).RL.SetData(SQLReader.GetString(29))
                FamisCaseChild(i).RM.SetData(SQLReader.GetString(30))
                FamisCaseChild(i).RP.SetData(SQLReader.GetString(31))
                FamisCaseChild(i).RR.SetData(SQLReader.GetString(32))
                FamisCaseChild(i).SN.SetData(SQLReader.GetString(33))
                FamisCaseChild(i).SQ.SetData(SQLReader.GetString(34))
                FamisCaseChild(i).SR.SetData(SQLReader.GetString(35))
                FamisCaseChild(i).TA.SetData(SQLReader.GetString(36))
                FamisCaseChild(i).TB.SetData(SQLReader.GetString(37))
                FamisCaseChild(i).TI.SetData(SQLReader.GetString(38))
                FamisCaseChild(i).TJ.SetData(SQLReader.GetString(39))
                FamisCaseChild(i).TF.SetData(SQLReader.GetString(40))
                FamisCaseChild(i).TK.SetData(SQLReader.GetString(41))
                FamisCaseChild(i).TD.SetData(SQLReader.GetString(42))
                FamisCaseChild(i).UB.SetData(SQLReader.GetString(43))
                FamisCaseChild(i).RN.SetData(SQLReader.GetString(44))
                FamisCaseChild(i).RO.SetData(SQLReader.GetString(45))
                FamisCaseChild(i).RQ.SetData(SQLReader.GetString(46))
                FamisCaseChild(i).SA.SetData(SQLReader.GetString(47))
                FamisCaseChild(i).SB.SetData(SQLReader.GetString(48))
                FamisCaseChild(i).SC.SetData(SQLReader.GetString(49))
                FamisCaseChild(i).SD.SetData(SQLReader.GetString(50))
                FamisCaseChild(i).SE.SetData(SQLReader.GetString(51))
                FamisCaseChild(i).SF.SetData(SQLReader.GetString(52))
                FamisCaseChild(i).SG.SetData(SQLReader.GetString(53))
                FamisCaseChild(i).SH.SetData(SQLReader.GetString(54))
                FamisCaseChild(i).SJ.SetData(SQLReader.GetString(55))
                FamisCaseChild(i).SK.SetData(SQLReader.GetString(56))
                FamisCaseChild(i).SL.SetData(SQLReader.GetString(57))
                FamisCaseChild(i).SM.SetData(SQLReader.GetString(58))
                FamisCaseChild(i).SO.SetData(SQLReader.GetString(59))
                FamisCaseChild(i).SP.SetData(SQLReader.GetString(60))
                FamisCaseChild(i).SI.SetData(SQLReader.GetString(61))
                FamisCaseChild(i).SS.SetData(SQLReader.GetString(62))
                FamisCaseChild(i).TG.SetData(SQLReader.GetString(63))
                FamisCaseChild(i).TH.SetData(SQLReader.GetString(64))
                FamisCaseChild(i).TL.SetData(SQLReader.GetString(65))
                FamisCaseChild(i).TM.SetData(SQLReader.GetString(66))
                FamisCaseChild(i).TO1.SetData(SQLReader.GetString(67))
                FamisCaseChild(i).TP.SetData(SQLReader.GetString(68))
                FamisCaseChild(i).TQ.SetData(SQLReader.GetString(69))
                FamisCaseChild(i).TR.SetData(SQLReader.GetString(70))
                FamisCaseChild(i).TS.SetData(SQLReader.GetString(71))
                FamisCaseChild(i).UA.SetData(SQLReader.GetString(72))
                FamisCaseChild(i).UC.SetData(SQLReader.GetString(73))
                FamisCaseChild(i).UD.SetData(SQLReader.GetString(74))
                FamisCaseChild(i).UE.SetData(SQLReader.GetString(75))
                FamisCaseChild(i).UF.SetData(SQLReader.GetString(76))
                FamisCaseChild(i).UG.SetData(SQLReader.GetString(77))
                FamisCaseChild(i).UH.SetData(SQLReader.GetString(78))
                FamisCaseChild(i).UK.SetData(SQLReader.GetString(79))
                FamisCaseChild(i).UL.SetData(SQLReader.GetString(80))
                FamisCaseChild(i).TC.SetData(SQLReader.GetString(81))
                FamisCaseChild(i).TE.SetData(SQLReader.GetString(82))
                FamisCaseChild(i).UI.SetData(SQLReader.GetString(83))
                i += 1
            End While
            SQLReader.Close()
            SQLConn.Close()
            set_cmbChild()
            cmb_Child.SelectedIndex = 0
            ChildNumber = 0
        Else
            tab_105B.Enabled = False
        End If
        If numVRP > 0 Then
            For i = 0 To numVRP - 1
                FAMISVRPInformation(i) = New VRPInformation(i)
            Next
            SQLConn.Open()
            i = 0
            SQLComm.CommandText = "SELECT VA, VC, VE, VG, VI, VQ, VK, VM, VO FROM FAMISVRPInformation WHERE CASENUMBER = '" + CASENUMBER + "'"
            SQLReader = SQLComm.ExecuteReader
            While SQLReader.Read
                FAMISVRPInformation(i).VA.SetData(SQLReader.GetString(0))
                FAMISVRPInformation(i).VC.SetData(SQLReader.GetString(1))
                FAMISVRPInformation(i).VE.SetData(SQLReader.GetString(2))
                FAMISVRPInformation(i).VG.SetData(SQLReader.GetString(3))
                FAMISVRPInformation(i).VI.SetData(SQLReader.GetString(4))
                FAMISVRPInformation(i).VQ.SetData(SQLReader.GetString(5))
                FAMISVRPInformation(i).VK.SetData(SQLReader.GetString(6))
                FAMISVRPInformation(i).VM.SetData(SQLReader.GetString(7))
                FAMISVRPInformation(i).VO.SetData(SQLReader.GetString(8))
                i += 1
            End While
            SQLReader.Close()
            SQLConn.Close()
            set_cmbVRP()
            cmb_VRP.SelectedIndex = 0
            VRPNumber = 0
        Else
            txt_VE.Enabled = False
            txt_VG.Enabled = False
            txt_VI.Enabled = False
            txt_VK.Enabled = False
            txt_VM.Enabled = False
            txt_VO.Enabled = False
            txt_VQ.Enabled = False
            cmb_VRP.Enabled = False
        End If
        SQLConn.Close()
    End Sub

    Private Sub set_cmbChild()
        Dim i As Integer
        For i = 0 To numChildren - 1
            cmb_Child.Items.Add(FamisCaseChild(i).QA.GetData & ": " & FamisCaseChild(i).QC.GetData & " " & FamisCaseChild(i).QB.GetData)
        Next
    End Sub
    Private Sub set_cmbVRP()
        Dim i As Integer
        For i = 0 To numVRP - 1
            cmb_VRP.Items.Add(i + 1)
        Next
    End Sub

    Private Sub AddEvents(ByVal ctrlparent As Control)
        '--Adds events to pad a textbox with spaces when user-- 
        '--exits, provides a tooltip with block information, and tracks--
        '--which blocks the user has changed the data--
        Dim ctrl As Control
        For Each ctrl In ctrlparent.Controls
            If TypeOf ctrl Is TextBox Then
                AddHandler ctrl.Leave, AddressOf LoseFocus
                AddHandler ctrl.MouseEnter, AddressOf ToolTip
            End If
            If ctrl.HasChildren Then
                AddEvents(ctrl)
            End If
        Next
    End Sub
    Private Sub LoseFocus(ByVal sender As Object, ByVal e As EventArgs)
        DirectCast(sender, TextBox).Text = DirectCast(sender, TextBox).Text.PadRight(DirectCast(sender, TextBox).MaxLength).ToUpper
        '  If DirectCast(sender, TextBox).Name = "txt_OL" Or DirectCast(sender, TextBox).Name = "txt_PO" Then ParentForm.isPage9 = True
    End Sub
    Private Sub ToolTip(ByVal sender As Object, ByVal e As EventArgs)
        Dim Note As String
        Select Case DirectCast(sender, TextBox).Name.Substring(4, 2)
            Case "AA"
                Note = "CASE NUMBER (CA/FS/MED)" & vbCrLf & "   Enter the case number as assigned by county registration.  This " & vbCrLf & "number should contain a 'C' prefix followed by a 6 digit case " & vbCrLf & "number or it should contain an 'S4', 'S5', 'S6', 'S7' or 'S8' prefix " & vbCrLf & "followed by a 5 digit case number."
            Case "AB"
                Note = "SEGMENT CODE (CA/FS) (Pre TANF/WFNJ)" & vbCrLf & "   An identification assigned by the county to designate the category of" & vbCrLf & "assistance for the case."
            Case "AC"
                Note = "CASE NAME – LAST (CA/FS/MED)" & vbCrLf & "   TANF cases; enter the last name of the applicant from the PA-1J." & vbCrLf & "NPA cases; enter the last name of the Head of Household from the FSP-901."
            Case "AD"
                Note = "CASE NAME – FIRST (CA/FS/MED)" & vbCrLf & "   Enter the first name of the person coded in block AC."
            Case "AE"
                Note = "CASE NAME – MIDDLE INITIAL (CA/FS/MED)" & vbCrLf & "   Enter the middle initial (if any) of the person coded in block AC"
            Case "AF"
                Note = "COUNTY NUMBER (CA/FS/MED)" & vbCrLf & "   Enter the number to identify the county in which benefits are being requested by the individual coded in block AC."
            Case "AG"
                Note = "EBT STATUS (CA/FS)" & vbCrLf & "   Computer Generated" & vbCrLf & "1ST position identifies EBT status, 2ND position identifies EBT benefit available day."
            Case "AH"
                Note = "SUPERVISOR GROUP (CA/FS/MED)" & vbCrLf & "   Enter the 2 character alpha code designated for the unit supervisor." & vbCrLf & "NOTE:  In agencies with multiple offices, the first character of this " & vbCrLf & "block indicates the office responsible for the case."
            Case "AI"
                Note = "WORKER NUMBER (CA/FS/MED)" & vbCrLf & "   Enter the worker’s assigned 2 digit numeric code."
            Case "AJ"
                Note = "CLIENT CONFIDENTIALITY INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code for the case.  See Unit Supervisor " & vbCrLf & "for further information."
            Case "AK"
                Note = "DATE OF LAST CHANGE (CA/FS/MED)" & vbCrLf & "   Computer Generated"
            Case "AL"
                Note = "GRANT AMOUNT (CA)" & vbCrLf & "   Enter the grant amount."
            Case "AM"
                Note = "COUPON ALLOTMENT AMOUNT (FS)" & vbCrLf & "   Enter the coupon allotment."
            Case "AN"
                Note = "MEDICAID EXTENSION STOP DATE (MED)" & vbCrLf & "   Enter the final month and year (MMCCYY) of Medicaid Extension " & vbCrLf & "eligibility."
            Case "BA"
                Note = "FEMALE INDIVIDUAL NAME – LAST (CA/FS/MED)" & vbCrLf & "   Enter the last name of the female adult who will be coded in block BD."
            Case "BB"
                Note = "FEMALE INDIVIDUAL NAME – FIRST (CA/FS/MED)" & vbCrLf & "   Enter the first name of the female adult coded in block BA."
            Case "BC"
                Note = "FEMALE INDIVIDUAL NAME – MIDDLE INITIAL (CA/FS/MED)" & vbCrLf & "   Enter the middle initial (if any) of the female adult coded in block BA."
            Case "BD"
                Note = "PAYEE INDICATOR – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to show the status of the female adult " & vbCrLf & "coded in block BA."
            Case "BE"
                Note = "RACIAL CATEGORY – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to designate the racial group for the " & vbCrLf & "female adult coded in block BA as she is regarded in the " & vbCrLf & "community."
            Case "BF"
                Note = "MARITAL STATUS – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to show the marital status of the last " & vbCrLf & "name of the female adult coded in block BA."
            Case "BG"
                Note = "SSN INDICATOR – FEMALE (CA/FS/MED)" & vbCrLf & "   Computer Generated"
            Case "BH"
                Note = "IV-D INDICATOR – FEMALE (CA/MED)" & vbCrLf & "   Enter the appropriate code to indicate a IV-D parent."
            Case "BI"
                Note = "SANCTION INDICATOR – FEMALE (CA/FS/MED)" & vbCrLf & "   Refer to Riverside Rule Instruction and FAMIS " & vbCrLf & "Sanction Coding Desk Guide for specific coding instructions."
            Case "BJ"
                Note = "MALE INDIVIDUAL NAME – LAST (CA/FS/MED)" & vbCrLf & "   Enter the last name of the male adult who will be coded in block BM."
            Case "BK"
                Note = "MALE INDIVIDUAL NAME – FIRST (CA/FS/MED)" & vbCrLf & "   Enter the first name of the male adult coded in block BJ."
            Case "BL"
                Note = "MALE INDIVIDUAL NAME – MIDDLE INITIAL (CA/FS/MED)	" & vbCrLf & "   Enter the middle initial (if any) of the male adult coded in block BJ."
            Case "BM"
                Note = "PAYEE INDICATOR – MALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to show the status of the male adult " & vbCrLf & "coded in block BJ."
            Case "BN"
                Note = "RACIAL CATEGORY – MALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to designate the racial group for the " & vbCrLf & "male adult coded in block BJ as he is regarded in the community."
            Case "BO"
                Note = "MARITAL STATUS – MALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to show the marital status of the male " & vbCrLf & "adult coded in block BJ."
            Case "BP"
                Note = "SSN INDICATOR – MALE (CA/FS/MED)" & vbCrLf & "   Computer Generated"
            Case "BQ"
                Note = "IV-D INDICATOR – MALE (CA/MED)" & vbCrLf & "   Enter the appropriate code to indicate a IV-D parent"
            Case "BR"
                Note = "SANCTION INDICATOR – MALE (CA/FS/MED)" & vbCrLf & "   Refer to Riverside Rule Instruction and FAMIS Sanction Coding " & vbCrLf & "Desk Guide for specific coding instructions."
            Case "BS"
                Note = "ALIEN REGISTRATION NUMBER – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the alien registration number assigned to this individual."
            Case "BT"
                Note = "ALIEN TYPE – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter alien status"
            Case "BU"
                Note = "INS VERIFICATION INDICATOR – FEMALE" & vbCrLf & "   Future Development – Field Protected"
            Case "BV"
                Note = "FELON INDICATOR – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter indicator if convicted.  "
            Case "BW"
                Note = "ALIEN REGISTRATION NUMBER – MALE (CA/FS/MED)" & vbCrLf & "   Enter the alien registration number assigned to this individual."
            Case "BX"
                Note = "ALIEN TYPE – MALE (CA/FS/MED)" & vbCrLf & "   Enter alien status."
            Case "BY"
                Note = "INS VERIFICATION INDICATOR – MALE" & vbCrLf & "   Future Development – Field Protected"
            Case "BZ"
                Note = "FELON INDICATOR – MALE (CA/FS/MED)" & vbCrLf & "   Enter indicator if convicted."
            Case "CA"
                Note = "EXTRA ADDRESS/REFERENCE (CA/FS/MED)" & vbCrLf & "   Enter the additional address (e.g., apartment number, basement, " & vbCrLf & "first floor, etc.) if applicable, or use for reference."
            Case "CB"
                Note = "ADDRESS INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter an 'X' if block CA contains an extra address.  If block CA is " & vbCrLf & "used for cross-reference, leave this block blank."
            Case "CC"
                Note = "MAILING ADDRESS – STREET (CA/FS/MED)" & vbCrLf & "   Enter the street address, box number or rural route for receiving " & vbCrLf & "mail."
            Case "CD"
                Note = "MAILING ADDRESS – CITY, STATE (CA/FS/MED)" & vbCrLf & "   Enter the city and state for the mailing address."
            Case "CE"
                Note = "MAILING ADDRESS – ZIP CODE (CA/FS/MED)" & vbCrLf & "   Enter the zip code for the mailing address."
            Case "CF"
                Note = "MUNICIPALITY CODE (CA/FS/MED)" & vbCrLf & "   Enter the code, indicating the municipality in which the eligible unit " & vbCrLf & "or household resides within the county."
            Case "CG"
                Note = "HEA INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter the code that indicates the type of heating arrangement for " & vbCrLf & "the household as per HEA addendum"
            Case "DA"
                Note = "SUBSTITUTE PAYEE/OTHER PAYEE (CA/FS/MED)" & vbCrLf & "   Enter the name of the Substitute Payee, and code block DB."
            Case "DB"
                Note = "SUBSTITUTE PAYEE INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to identify the substitute payee whose " & vbCrLf & "name appears in block DA."
            Case "DC"
                Note = "RESIDENT ADDRESS – STREET (CA/FS/MED)" & vbCrLf & "   Enter the street address (number and street) where the client " & vbCrLf & "resides if it is different from the mailing address."
            Case "DD"
                Note = "RESIDENT ADDRESS – CITY, STATE (CA/FS/MED)" & vbCrLf & "   Enter the city and state for the resident address."
            Case "DE"
                Note = "RESIDENT ADDRESS – ZIP CODE (CA/FS/MED)" & vbCrLf & "   Enter the resident address zip code."
            Case "DF"
                Note = "NO. KINSHIP CHILDREN – CASE (CA/FS/MED)" & vbCrLf & "   Enter the number of Kinship children on the case.  Valid entries are " & vbCrLf & "'00' through '24'."
            Case "EA"
                Note = "CROSS REFERENCE/SUB PAY DOB (CA/FS/MED)" & vbCrLf & "   When used for cross reference enter the number of any present or " & vbCrLf & "past public assistance case that can serve as an information source " & vbCrLf & "to identify data on the client whose name appears in block AC.  " & vbCrLf & "Enter the entire number, including any suffix or prefix."
            Case "EB"
                Note = "CROSS REFERENCE – AFN/PAN (CA/FS/MED)" & vbCrLf & "   If the client receives Food Stamps through another TANF case, " & vbCrLf & "enter that 7 character case number with the prefix 'AFN'."
            Case "EC"
                Note = "CROSS REFERENCE (CA/FS/MED)" & vbCrLf & "   This block may be left blank or be used for coding any additional " & vbCrLf & "information which would not be coded elsewhere on the 105 form."
            Case "ED"
                Note = "STATE/FEDERAL FS SHARES (FS)" & vbCrLf & "   Computer Generated"
            Case "EE"
                Note = "ERROR PRONE PROFILE (EPP) INDICATOR (CA/MED)" & vbCrLf & "   (Hudson and Essex only)" & vbCrLf & "Enter the appropriate error prone profile code."
            Case "EF"
                Note = "LANGUAGE INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter the code that reflects the primary language spoken in the " & vbCrLf & "household."
            Case "EG"
                Note = "FUTURE DEVELOPMENT (CA/FS)"
            Case "EH"
                Note = "HOUSING INDICATOR (CA/FS)" & vbCrLf & "   Enter the type of housing in which each recipient family resides."
            Case "EI"
                Note = "PER CAPITA REDUCTION (CA/FS/MED)" & vbCrLf & "   Computer Generated sanction amount derived from per capita " & vbCrLf & "calculation."
            Case "EJ"
                Note = "EMERGENCY ASSISTANCE (EA) INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to identify the type of emergency " & vbCrLf & "assistance."
            Case "EK"
                Note = "WORK START DATE – FEMALE (MED)" & vbCrLf & "   Enter the month, century and year that the $30 and 1/3 earned " & vbCrLf & "income disregard is applied against the female adult earnings coded " & vbCrLf & "in block JA."
            Case "EL"
                Note = "WORK STOP DATE – FEMALE (MED)" & vbCrLf & "   Enter the last month, century and year for which the $30 and 1/3 or " & vbCrLf & "$30 earning income disregard is applied against the female adult " & vbCrLf & "earnings coded in block JA."
            Case "EM"
                Note = "WORK START DATE – MALE (MED)" & vbCrLf & "   Enter the month, century and year that the $30 and 1/3 earning " & vbCrLf & "income disregard is applied against the male adult earnings coded " & vbCrLf & "in block KA."
            Case "EN"
                Note = "WORK STOP DATE – MALE (MED)" & vbCrLf & "   Enter the last month, century and year for which the $30 and 1/3 or " & vbCrLf & "$30 earned income disregard is applied against the male adult " & vbCrLf & "earnings coded in block KA."
            Case "FA"
                Note = "TYPE PERSON INDICATOR – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the code that indicates the type of benefits received by the " & vbCrLf & "female adult coded in block BA."
            Case "FB"
                Note = "BIRTH DATE – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the birth date of the female adult coded in block BA.  This " & vbCrLf & "block must be completed when the female is a TANF, Medicaid " & vbCrLf & "and/or Food Stamp recipient."
            Case "FC"
                Note = "SOCIAL SECURITY NUMBER – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the Social Security number of the female adult, if known."
            Case "FD"
                Note = "WIN REGISTRATION – FEMALE (CA) (WSP)" & vbCrLf & "   Future(Development)"
            Case "FE"
                Note = "EDUCATION LEVEL – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to designate the education level of the " & vbCrLf & "female adult coded in block BA."
            Case "FF"
                Note = "FS INDICATOR – FEMALE (FS)" & vbCrLf & "   Enter the indicator for the amount shown in block MQ."
            Case "FG"
                Note = "WFNJ IDENTIFIER – FEMALE (CA/MED)" & vbCrLf & "   Enter the appropriate WFNJ Status Code for the female adult."
            Case "FH"
                Note = "WFNJ CLOCK – FEMALE (CA)" & vbCrLf & "   Computer generated.  Field protected.  System generated values " & vbCrLf & "are '01' through '99'."
            Case "FI"
                Note = "TYPE PERSON INDICATOR – MALE (CA/FS/MED)" & vbCrLf & "   Enter the code that indicates the type of benefits received by the " & vbCrLf & "male adult coded in block BJ."
            Case "FJ"
                Note = "BIRTH DATE – MALE (CA/FS/MED)" & vbCrLf & "   Enter the birth date of the male adult coded in block BJ.  This block " & vbCrLf & "must be completed when the male is a TANF, Medicaid and/or Food " & vbCrLf & "Stamp recipient."
            Case "FK"
                Note = "SOCIAL SECURITY NUMBER – MALE (CA/FS/MED)" & "   Enter the Social Security number of the male adult, if known."
            Case "FL"
                Note = "WIN REGISTRATION – MALE (CA) (WSP)" & vbCrLf & "   Future(Development)"
            Case "FM"
                Note = "EDUCATION LEVEL – MALE (CA/FS)" & vbCrLf & "   Enter the appropriate code to designate the education level of the " & vbCrLf & "male adult coded in block BJ."
            Case "FN"
                Note = "FS INDICATOR – MALE (FS)" & vbCrLf & "   Enter an indicator for the amount shown in block NK."
            Case "FO"
                Note = "WFNJ CLOCK – MALE (CA)" & vbCrLf & "   Computer generated.  Field protected.  System generated values " & vbCrLf & "are '01' through '99'."
            Case "FP"
                Note = "WFNJ IDENTIFIER – MALE (CA/MED)" & vbCrLf & "   Enter the appropriate WFNJ Status Code for the male adult."
            Case "GA"
                Note = "SOCIAL SECURITY (OR OTHER) CLAIM NUMBER - FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the alpha-numeric claim number associated with the benefit, " & vbCrLf & "RSDI, Railroad Retirement, public or private pension or Medicare, " & vbCrLf & "that the female adult coded in block BA is receiving.  This number " & vbCrLf & "will contain 9 numeric and 1 to 3 alpha characters."
            Case "GB"
                Note = "ADDED/REMOVED DATE – FEMALE (CA/MED)" & vbCrLf & "   Enter the date the female adult coded in block BA was added to or " & vbCrLf & "removed from the grant and/or Medicaid.  A hand entry in this block " & vbCrLf & "requires a hand entry in block GC."
            Case "GC"
                Note = "ADDED/REMOVED INDICATOR – FEMALE (CA/MED)" & vbCrLf & "   Enter one of the codes to indicate the status of the female " & vbCrLf & "coded in block BA."
            Case "GD"
                Note = "SEGMENT CODE – FEMALE (CA/MED)" & vbCrLf & "   In companion cases, and all TANF/MED type refugee cases, this " & vbCrLf & "block will describe the category of assistance which applies to the " & vbCrLf & "female adult coded in block BA."
            Case "GE"
                Note = "SSI REFERENCE – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to indicate whether or not the female " & vbCrLf & "adult receives SSI."
            Case "GF"
                Note = "DATE OF ENTRY – FEMALE (CA/FS/MED)" & vbCrLf & "   When the female adult is a refugee, enter the date of entry into the " & vbCrLf & "country."
            Case "GG"
                Note = "SOCIAL SECURITY (OR OTHER) CLAIM NUMBER – MALE (CA/FS/MED)" & vbCrLf & "   Enter the alpha-numeric claim number associated with the benefit, " & vbCrLf & "RSDI, Railroad Retirement, public or private pension or Medicare, " & vbCrLf & "that the male adult coded in block BJ is receiving.  This number will " & vbCrLf & "contain 9 numeric and 1 to 3 alpha characters."
            Case "GH"
                Note = "ADDED/REMOVED DATE – MALE (CA/MED)" & vbCrLf & "   Enter the date the male adult coded in block BJ was added to or " & vbCrLf & "removed from the grant and/or Medicaid."
            Case "GI"
                Note = "ADDED/REMOVED INDICATOR – MALE (CA/MED)" & vbCrLf & "   Enter one of the following codes to indicate the status of the male " & vbCrLf & "coded in block BJ."
            Case "GJ"
                Note = "SEGMENT CODE – MALE (CA/MED)" & vbCrLf & "   In companion cases, and all TANF/MED type refugee cases, this " & vbCrLf & "block will describe the category of assistance which applies to the " & vbCrLf & "male adult coded in block BJ."
            Case "GK"
                Note = "SSI REFERENCE – MALE (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to indicate whether or not the male adult " & vbCrLf & "receives SSI."
            Case "GL"
                Note = "DATE OF ENTRY – MALE (CA/FS/MED)" & vbCrLf & "   When the male adult is a refugee, enter the date of entry into the " & vbCrLf & "country."
            Case "HA"
                Note = "FAMILY CAP (EXCLUDED CHILD) COUNT (CA/MED)" & vbCrLf & "   Enter the number children coded as Family Cap children (QL/36 equals 'Y' and QJ/35 " & vbCrLf & "equals 'CD')."
            Case "HB"
                Note = "100% MEDICAID DISREGARD STOP DATE (CA/MED)" & vbCrLf & "   Enter the month and year (MMCCYY) which the 100% " & vbCrLf & "earned income Medicaid disregard will end."
            Case "HC"
                Note = "MEDICAID EXTENSION START DATE (MED)" & vbCrLf & "   Enter the month and year (MMCCYY) for the start of Medicaid " & vbCrLf & "Extension."
            Case "HD"
                Note = "HOSPITAL INSURANCE INDICATOR (CA/MED)" & vbCrLf & "   Enter the appropriate code to show whether or not any members of " & vbCrLf & "the case have private insurance coverage."
            Case "HE"
                Note = "MEDICAID PERSON CODE – FEMALE (CA/MED)" & vbCrLf & "   Enter the Medicaid person number ('01'-'09') for the female adult " & vbCrLf & "coded in block BA."
            Case "HF"
                Note = "HEALTH INSURANCE POLICY NUMBER – FEMALE (CA/MED)" & vbCrLf & "   Enter the number of the insurance policy that covers the female " & vbCrLf & "adult coded in block BA."
            Case "HG"
                Note = "HOSPITAL INSURANCE TYPE – FEMALE (CA/MED)" & vbCrLf & "   Enter the appropriate two digit code ('00' – '99') to identify the insurance " & vbCrLf & "company providing the coverage for the female adult coded " & vbCrLf & "in block BA."
            Case "HH"
                Note = "EBT CLIENT CODE – FEMALE (CA/FS)" & vbCrLf & "   Computer Generated"
            Case "HI"
                Note = "FUTURE DEVELOPMENT"
            Case "HJ"
                Note = "MEDICARE – FEMALE (MED)" & vbCrLf & "   Enter the appropriate code indicating the extent of Medicare " & vbCrLf & "coverage received by the female adult coded in block BA."
            Case "HK"
                Note = "DRUGS – FEMALE (MED)" & vbCrLf & "   Enter the appropriate code to identify the insurance company providing" & vbCrLf & "the coverage for the female adult coded in block BA."
            Case "HL"
                Note = "PROGRAM STATUS CODE INDICATOR - FEMALE (MED)" & vbCrLf & "   Computer generated field based on Medicaid calculation."
            Case "HM"
                Note = "MEDICAID PERSON CODE – MALE (MED)" & vbCrLf & "   Enter the Medicaid person number ('01' – '09') for the male adult " & vbCrLf & "coded in block BJ."
            Case "HN"
                Note = "HEALTH INSURANCE POLICY NUMBER – MALE (MED)" & vbCrLf & "   Enter the number of the insurance policy that covers the male adult " & vbCrLf & "coded in block BJ."
            Case "HO"
                Note = "HOSPITAL INSURANCE TYPE – MALE (MED)" & "   Enter the appropriate two digit code ('00' – '99') to identify the insurance company " & vbCrLf & "providing the coverage for the male adult coded in block BJ."
            Case "HP"
                Note = "EBT CLIENT CODE (CA/FS)" & vbCrLf & "   Computer Generated"
            Case "HQ"
                Note = "FUTURE DEVELOPMENT"
            Case "HR"
                Note = "MEDICARE – MALE (MED)" & vbCrLf & "   Enter the appropriate code indicating the extent of Medicare " & vbCrLf & "coverage received by the male adult coded in block BJ."
            Case "HS"
                Note = "DRUGS – MALE (MED)" & vbCrLf & "   Enter the appropriate code to identify the insurance company providing " & vbCrLf & "the coverage for the male adult coded in block BJ."
            Case "HT"
                Note = "PROGRAM STATUS CODE INDICATOR – MALE (MED)" & vbCrLf & "   Computer generated field based on Medicaid calculation."
            Case "IA"
                Note = "TYPE OF ACTION TAKEN (CA)" & vbCrLf & "   Enter the appropriate code to show the type of TANF action taken."
            Case "IB"
                Note = "REASON FOR ACTION TAKEN (CA)" & vbCrLf & "   Enter the appropriate code to show the reason for the action code in " & vbCrLf & "block IA."
            Case "IC"
                Note = "EFFECTIVE DATE OF ACTION (CA)" & vbCrLf & "   Enter the actual date that the action taken in block IA occurs."
            Case "ID"
                Note = "APPLICATION DATE (CA)" & vbCrLf & "   Enter the date that the PA-1J was filed by the applicant or the date " & vbCrLf & "the PA-1C was signed, whichever is earlier."
            Case "IE"
                Note = "APPLICATION TYPE (CA)" & vbCrLf & "   This block is used to identify pending applications (PA-1Js) and " & vbCrLf & "reopened cases."
            Case "IF"
                Note = "VALIDATION DATE (CA)" & vbCrLf & "   If the case is in Application or Presumptive Eligibility status, this " & vbCrLf & "block must be blank.  Enter the first day of the " & vbCrLf & "month following the month in which the worker has received all the materials required " & vbCrLf & "for validation of the case."
            Case "IG"
                Note = "LAST REDETERMINATION DATE (CA)" & vbCrLf & "   Enter the date the first benefit is issued for a new or reopened case."
            Case "IH"
                Note = "ELIGIBLE UNIT – CHILDREN (CA)" & vbCrLf & "   The number of children entered in this block must be the same as " & vbCrLf & "the number of 'A' or 'T' indicators in the 'Added or Removed' block " & vbCrLf & "QL on the 105B form."
            Case "II"
                Note = "ELIGIBLE UNIT – ADULTS (CA)" & vbCrLf & "   Enter the number of adults included in the eligible unit as indicated " & vbCrLf & "by entries of 'A' or 'T' in block GC and/or GI and P, S or G entered " & vbCrLf & "in block BD and/or BM."
            Case "IJ"
                Note = "HOUSEHOLD SIZE (CA)" & vbCrLf & "   Enter the number of persons in the household including persons not " & vbCrLf & "in the eligible unit."
            Case "IK"
                Note = "OTHER (CA)" & vbCrLf & "   Enter the total monthly amount of other monthly deductions " & vbCrLf & "excluding recoupment.  The system will subtract this amount from " & vbCrLf & "the grant automatically."
            Case "IL"
                Note = "TEMPORARY REDUCTION TO GRANT INDICATOR (CA)" & vbCrLf & "   Enter the code that shows the type of temporary reduction to the " & vbCrLf & "grant."
            Case "IM"
                Note = "RECOUPMENT AMOUNT (CA)" & vbCrLf & "   Enter the monthly amount of recoupment to be deducted from the " & vbCrLf & "grant.  This block should not contain Other deductions not described " & vbCrLf & "as Recoupment."
            Case "IN"
                Note = "GRANT RECOUPMENT BALANCE (CA)" & vbCrLf & "   Enter the balance of recoupment to be collected.  The system will " & vbCrLf & "automatically reduce the balance at cutoff processing every time a " & vbCrLf & "computer generated benefit is issued with a recoupment reduction."
            Case "IO"
                Note = "RECOUPMENT COLLECTED TO DATE (CA)" & vbCrLf & "   Enter the amount of recoupment, if any, collected to date.  The " & vbCrLf & "system will automatically increment the amount in this block at " & vbCrLf & "cutoff processing when a check is issued with a recoupment " & vbCrLf & "deduction."
            Case "IP"
                Note = "FS GRANT AMOUNT (CA)" & vbCrLf & "   Computer Generated"
            Case "JA"
                Note = "EARNED INCOME – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly gross amount of income earned by the female " & vbCrLf & "adult coded in block BA.  If self-employed, enter the adjusted gross " & vbCrLf & "earned income (gross minus business expenses)."
            Case "JB"
                Note = "SSI – FEMALE (FS)" & vbCrLf & "   Enter the monthly amount of any SSI benefits applicable to FS " & vbCrLf & "income received by the female adult."
            Case "JC"
                Note = "RSDI – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any RSDI benefits received by the " & vbCrLf & "female adult coded in block BD.  Do not include lump sum " & vbCrLf & "payments.  When an amount is entered in this block, the female " & vbCrLf & "adult’s Social Security Claim Number must be entered in block GA."
            Case "JD"
                Note = "VA – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any VA pension or compensation " & vbCrLf & "received by the female adult coded in block BD.  When an amount " & vbCrLf & "is entered in this block, a claim number must be entered in block " & vbCrLf & "GA."
            Case "JE"
                Note = "PENSION – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any pensions received by the female " & vbCrLf & "adult coded in block BD.  Do not include RSDI, SSI or VA.  When an " & vbCrLf & "amount is entered in this block, a claim number must be entered in " & vbCrLf & "block GA."
            Case "JF"
                Note = "UIB – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any contributions received by the " & vbCrLf & "female adult coded in block BA."
            Case "JG"
                Note = "CONTRIBUTIONS – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any contributions received by the " & vbCrLf & "female adult coded in block BA."
            Case "JH"
                Note = "OTHER INCOME – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any unearned income that the female " & vbCrLf & "adult coded in block BA receives from a source not identified in the " & vbCrLf & "other unearned income blocks (JC through JG).  Prorated lump sum " & vbCrLf & "payments should be entered here.  The income of the female adult " & vbCrLf & "coded 'T' in block BD should be manually calculated and entered in " & vbCrLf & "this block."
            Case "JI"
                Note = "TANF DISREGARD IND – FEMALE (CA)" & vbCrLf & "   Enter 'A' when 100% TANF disregard should be applied.  A 'space' " & vbCrLf & "will incur the normal 50% TANF disregard."
            Case "JJ"
                Note = "CHILD CARE EXPENSES – FEMALE (MED)" & vbCrLf & "   Enter the allowable monthly amount of child care expenses paid by " & vbCrLf & "the female adult with income in block JA.  Only 1 adult can show " & vbCrLf & "child care expenses."
            Case "JK"
                Note = "DISREGARDS – FEMALE (CA)" & vbCrLf & "   Computer Generated"
            Case "JL"
                Note = "DIVERTED INCOME – FEMALE (CA/MED)" & vbCrLf & "   Enter the monthly amount of court-ordered support payments made " & vbCrLf & "to dependents living elsewhere."
            Case "JM"
                Note = "TOTAL NET INCOME – FEMALE" & vbCrLf & "   Computer Generated"
            Case "JN"
                Note = "INITIAL GROSS INCOME (CA)" & vbCrLf & "   Enter the monthly amount of the total countable gross earned and " & vbCrLf & "unearned income applicable to the entire CA case.  This amount will " & vbCrLf & "be used in conjunction with the daily I&A system (Line P) for the " & vbCrLf & "performance of the gross eligibility test."
            Case "JO"
                Note = "INITIAL NET INCOME (CA)" & vbCrLf & "   Enter the monthly amount of the total net income applicable to the " & vbCrLf & "entire CA case.  This amount will be used in conjunction with the " & vbCrLf & "daily I&A system (Line P) for the calculation of the initial grant."
            Case "JP"
                Note = "INITIAL ELIGIBILITY DISREGARD INDICATOR (CA)" & vbCrLf & "   Enter an 'X' in this block if the system is to bypass the Initial " & vbCrLf & "Eligibility Test.  Leave blank and the system will use the income " & vbCrLf & "entered in 'Initial Eligibility Income' block KN and to perform the " & vbCrLf & "test."
            Case "JQ"
                Note = "MEDICAID EXTENSION INDICATOR (MED)" & vbCrLf & "   Code block with type of medicaid extension for which the case is " & vbCrLf & "eligible."
            Case "JR"
                Note = "KINSHIP SUBSIDY AMOUNT – PA (CA/FS/MED)" & vbCrLf & "   Enter the total subsidy amount for the number of Kinship children on " & vbCrLf & "the case."
            Case "JS"
                Note = "WORK INDICATOR – FEMALE (MED)" & vbCrLf & "   The appropriate code must be entered to indicate the employment " & vbCrLf & "status of the female adult with earned income coded in block JA."
            Case "JT"
                Note = "WORK INDICATOR – MALE (MED)" & vbCrLf & "   The appropriate code must be entered to indicate the employment " & vbCrLf & "status of the male adult with earned income coded in block KA."
            Case "JU"
                Note = "GA INCOME – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any General Assistance income " & vbCrLf & "received by the Female Adult coded in BA."
            Case "JV"
                Note = "FAMILY CAP INDICATOR – FEMALE (CA/MED)" & vbCrLf & "   Enter the reason the family cap female adult is included in the " & vbCrLf & "eligible unit."
            Case "JW"
                Note = "WORKMEN’S COMPENSATION INCOME – FEMALE " & vbCrLf & "(CA/FS/MED)" & vbCrLf & "   Enter monthly amount of any Workmen’s Compensation income " & vbCrLf & "received."
            Case "JX"
                Note = "TEMPORARY DISABILITY INCOME – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter monthly amount of any Temporary Disability income received."
            Case "KA"
                Note = "EARNED INCOME – MALE (CA/FS)" & vbCrLf & "   Enter the monthly gross amount of income earned by the male adult " & vbCrLf & "coded in block BJ.  If self-employed, enter the adjusted gross " & vbCrLf & "earned income (gross minus business expenses)."
            Case "KB"
                Note = "SSI – MALE (FS)" & vbCrLf & "   Enter the monthly amount of SSI benefits applicable to FS income " & vbCrLf & "received by the male adult."
            Case "KC"
                Note = "RSDI – MALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any RSDI benefits received by the " & vbCrLf & "male adult coded in block BM.  Do not include lump sum payments.  " & vbCrLf & "When an amount is entered in this block, the male adult’s Social " & vbCrLf & "Security Claim Number must be entered in block GG."
            Case "KD"
                Note = "VA - MALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any VA pension or compensation " & vbCrLf & "received by the male adult coded in block BM.  When an amount is entered in this block, a claim number must be " & vbCrLf & "entered in block GG."
            Case "KE"
                Note = "PENSION – MALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any pensions received by the male " & vbCrLf & "adult coded in block BM."
            Case "KF"
                Note = "UIB – MALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any UIB received by the male adult " & vbCrLf & "coded in block BM."
            Case "KG"
                Note = "CONTRIBUTIONS – MALE (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any regular contributions received by " & vbCrLf & "the male adult coded in block BM."
            Case "KH"
                Note = "OTHER INCOME – MALE (CA/FS)" & vbCrLf & "   Enter the monthly amount of any unearned income that the male" & vbCrLf & "adult coded in block BJ receives from a source not identified in the " & vbCrLf & "other unearned income blocks (KC through KG).  Prorated lump " & vbCrLf & "sum payments should be entered here.  The income of the male " & vbCrLf & "adult coded 'T' in block BM should be manually calculated and " & vbCrLf & "entered in this block."
            Case "KI"
                Note = "TANF DISREGARD INDICATOR – MALE (CA)" & vbCrLf & "   Enter 'A' when 100% disregard should be applied.  A 'space' will " & vbCrLf & "incur the normal 50% TANF disregard."
            Case "KJ"
                Note = "CHILD CARE EXPENSES – MALE (CA)" & vbCrLf & "   Enter the allowable monthly amount of child care expenses paid by " & vbCrLf & "the male adult with income in block KA.  Only 1 adult can show child " & vbCrLf & "care expenses."
            Case "KK"
                Note = "DISREGARDS – MALE (CA)" & vbCrLf & "   Computer Generated"
            Case "KL"
                Note = "DIVERTED INCOME – MALE (CA/MED)" & vbCrLf & "   Enter the amount of court ordered support payments made to " & vbCrLf & "dependents living elsewhere."
            Case "KM"
                Note = "TOTAL NET INCOME – MALE (CA)" & vbCrLf & "   Computer Generated"
            Case "KN"
                Note = "INITIAL ELIGIBILITY INCOME (CA)" & vbCrLf & "   Enter the monthly amount of the total countable income earned and " & vbCrLf & "unearned applicable to the entire CA case.  This amount will be " & vbCrLf & "used in conjunction with the initial eligibility test."
            Case "KO"
                Note = "TOTAL NET INCOME (CA)" & vbCrLf & "   Computer Generated"
            Case "KP"
                Note = "TOTAL REQUIRED (CA)" & vbCrLf & "   Computer Generated"
            Case "KQ"
                Note = "REDUCTION TO PAYMENT (CA)" & vbCrLf & "   Computer Generated"
            Case "KR"
                Note = "CHILD SUPPORT AMOUNT (CA)" & vbCrLf & "   Enter the child support payments, whether actually received by the " & vbCrLf & "eligible unit or collected through the CSP process.  The amount in " & vbCrLf & "this block will be added to the case gross income in order to perform " & vbCrLf & "the gross eligibility test."
            Case "KS"
                Note = "GA INCOME (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any General Assistance income " & vbCrLf & "received by the Female Adult coded in BA."
            Case "KT"
                Note = "FAMILY CAP INDICATOR – MALE (CA)" & vbCrLf & "   Enter the reason the family cap male adult is included in the eligible " & vbCrLf & "unit."
            Case "KU"
                Note = "WORKMEN’S COMPENSATION INCOME – MALE (CA/FS/MED)" & vbCrLf & "   Enter monthly amount of any Workmen’s Compensation income " & vbCrLf & "received."
            Case "KV"
                Note = "TEMPORARY DISABILITY INCOME – MALE (CA/FS/MED)" & vbCrLf & "   Enter monthly amount of any Temporary Disability income received."
            Case "LA"
                Note = "TYPE OF ACTION TAKEN (FS)" & vbCrLf & "   Enter the appropriate code to show the type of action taken."
            Case "LB"
                Note = "REASON FOR ACTION TAKEN (FS)" & vbCrLf & "   Enter the appropriate code to show the reason for the action code in " & vbCrLf & "block LA.  "
            Case "LC"
                Note = "EFFECTIVE DATE (FS)" & vbCrLf & "   Enter the effective date of the action taken in block LA.  Effective " & vbCrLf & "date cannot precede application date."
            Case "LD"
                Note = "APPLICATION DATE (FS)" & vbCrLf & "   This is the date the application was presented to the CWA by the " & vbCrLf & "client.  A new application date must be hand entered whenever the " & vbCrLf & "case is being recertified."
            Case "LE"
                Note = "CERTIFICATION DATE (FS)" & vbCrLf & "   Enter the date of certification or the date of the most current " & vbCrLf & "recertification.  Do not enter a date that is prior to the date of " & vbCrLf & "application.  When a case is being recertified, this block must have " & vbCrLf & "a hand entry."
            Case "LF"
                Note = "CERTIFICATION PERIOD (FS)" & vbCrLf & "   Enter the number of months (01 thru 12) indicating the length of the " & vbCrLf & "certification period."
            Case "LG"
                Note = "HOUSEHOLD DEFINITIONS (FS)" & vbCrLf & "   Enter the appropriate code to indicate the definition of the " & vbCrLf & "household."
            Case "LH"
                Note = "HOUSEHOLD TYPE (FS)" & vbCrLf & "   Enter the appropriate code for household type, based on income " & vbCrLf & "received by the entire household.  'Other income' is any type of " & vbCrLf & "earned or unearned income not listed (e.g., UIB or Wages)."
            Case "LI"
                Note = "EBT SUB PAYEE CLIENT CODE" & vbCrLf & "   Computed Generated"
            Case "LJ"
                Note = "NUMBER OF CHILDREN IN FOOD STAMP ELIGIBLE " & vbCrLf & "HOUSEHOLD (FS)" & vbCrLf & "   Enter the total number of children included in the Food Stamp " & vbCrLf & "eligible household."
            Case "LK"
                Note = "NUMBER OF ADULTS IN FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   Enter the total number of adults included in the Food Stamp eligible " & vbCrLf & "household."
            Case "LL"
                Note = "NUMBER OF ADULTS – AGE 60 OR OVER IN FOOD STAMP " & vbCrLf & "ELIGIBLE HOUSEHOLD (FS)" & vbCrLf & "   Enter the number of adults age 60 or over who are included in the " & vbCrLf & "Food Stamp eligible household."
            Case "LM"
                Note = "STUDENT/STRIKER IN FS ELIGIBLE HOUSEHOLD (FS)" & vbCrLf & "   Enter the code that designates whether a student and/or a striker is " & vbCrLf & "in the Food Stamp eligible household.  This is not a numeric total of " & vbCrLf & "students or strikers."
            Case "LN"
                Note = "CERTIFICATION TYPE" & vbCrLf & "   Enter a 'M' if the 3-month certification should be a mail-in.  A " & vbCrLf & "'space' in this field will indicate a walk-in."
            Case "LO"
                Note = "GUARDIAN INDICATOR (FS)" & vbCrLf & "   Computer Generated."
            Case "LP"
                Note = "WORK REGISTRATION – FEMALE (FS)" & vbCrLf & "   Enter the appropriate code from the list in Appendix D to show the " & vbCrLf & "current work registration status of the female adult coded in block " & vbCrLf & "BA.  An entry is required only when the female adult is in the Food " & vbCrLf & "Stamp eligible household."
            Case "LQ"
                Note = "WORK REGISTRATION – MALE (FS)" & vbCrLf & "   Enter the appropriate code from the list in Appendix D to show the " & vbCrLf & "current work registration status of the male adult coded in block BJ.  " & vbCrLf & "An entry is required only when the male adult is in the Food Stamp " & vbCrLf & "eligible household."
            Case "LR"
                Note = "UTILITY ALLOWANCE INDICATOR (FS)" & vbCrLf & "   Enter the code to indicate if the household has " & vbCrLf & "elected to use a utility allowance as defined by the Food Stamp " & vbCrLf & "Program."
            Case "LS"
                Note = "MEDICAL DEDUCTION (FS)" & vbCrLf & "   Enter the total amount of all allowable medical expenses.  The " & vbCrLf & "computer system will automatically subtract the deductible amount."
            Case "LT"
                Note = "MARRIAGE DATE" & vbCrLf & "   Enter the date of marriage."
            Case "MA"
                Note = "TOTAL RESOURCE AMOUNT – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   Enter the amount of the Food Stamp household’s total resources " & vbCrLf & "that have not been classified as resource exclusions."
            Case "MB"
                Note = "EARNED INCOME – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the earned income blocks " & vbCrLf & "JA, KA or SA.  Any hand entered amount will be overridden."
            Case "MC"
                Note = "RSDI – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the RSDI income blocks JC, " & vbCrLf & "KC or SC.  Any hand entered amount will be overridden."
            Case "MD"
                Note = "SPECIAL SHELTER CALCULATION INDICATOR (FS)" & vbCrLf & "   Enter the code from the chart on the next page that identifies the " & vbCrLf & "type of RSDI/VA benefits received by the food stamp household.  " & vbCrLf & "This code will also identify whether the household is " & vbCrLf & "qualified or not qualified for the unlimited shelter calculation."
            Case "ME"
                Note = "VA – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the VA income blocks JD, " & vbCrLf & "KD or SD.  Any hand entered amount will be overridden."
            Case "MF"
                Note = "PENSION – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the Pension income blocks " & vbCrLf & "JE, KE or SE.  Any hand entered amount will be overridden."
            Case "MG"
                Note = "SSI – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the SSI income blocks JB, " & vbCrLf & "B or SO.  Any hand entered amount will be overridden."
            Case "MH"
                Note = "GENERAL ASSISTANCE – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   The General Assistance amount is entered in the FS Income " & vbCrLf & "Adjustment blocks for GA income (female) entered in block MQ " & vbCrLf & "indicator code block FF; GA income (male) entered in block NK " & vbCrLf & "indicator code block FN; or GA income (individual) block SP.  Any " & vbCrLf & "hand entered amount will be initialized."
            Case "MI"
                Note = "UIB – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the UIB income blocks JF, " & vbCrLf & "KF or SF.  Any hand entered amount will be overridden."
            Case "MJ"
                Note = "CONTRIBUTIONS – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the contribution blocks JG, " & vbCrLf & "KG or SG.  Any hand entered amount will be overridden."
            Case "MK"
                Note = "OTHER INCOME – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the other income blocks JH, " & vbCrLf & "KH or SH.  Any hand entered amount will be overridden."
            Case "ML"
                Note = "DEPENDENT CARE COSTS – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   Enter the total monthly amount of all allowable dependent care " & vbCrLf & "costs paid by the Food Stamp household."
            Case "MM"
                Note = "SHELTER COSTS – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   Enter the total monthly amount of all allowable shelter costs paid by " & vbCrLf & "the Food Stamp household.  If the Standard Utility Allowance or the " & vbCrLf & "Heating Utility Allowance has been utilized, include it here together " & vbCrLf & "with any other allowable shelter costs and enter a code in block LR."
            Case "MN"
                Note = "EXEMPT PORTION OF TANF GRANT (FS)" & vbCrLf & "   For persons included in the TANF eligible unit but not the Food " & vbCrLf & "Stamp household, enter the portion of the TANF grant to be exempt " & vbCrLf & "from the Food Stamp income calculation."
            Case "MO"
                Note = "NET INCOME – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   Computer Generated."
            Case "MP"
                Note = "TOTAL FOOD STAMP DEDUCTION AMOUNT (FS)" & vbCrLf & "   Computer Generated."
            Case "MQ"
                Note = "FS INCOME ADJUSTMENT – FEMALE (FS)" & vbCrLf & "   Enter the total monthly amount of the following adjustments to " & vbCrLf & "represent FS income for the female adult."
            Case "MR"
                Note = "UTILITY ALLOWANCE AMOUNT (FS)"
            Case "MS"
                Note = "WORKMEN’S COMP INCOME – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the WC income blocks JW, " & vbCrLf & "KU or UH.  Any hand-entered amount will be overridden."
            Case "MT"
                Note = "TEMPORARY DISABILITY INCOME – FOOD STAMP HOUSEHOLD (FS)" & vbCrLf & "   This block is computer generated from the TDI income blocks JX, " & vbCrLf & "KV or UG.  Any hand-entered amount will be overridden."
            Case "NA"
                Note = "RESTORATION TOTAL AMOUNT/BALANCE (FS)" & vbCrLf & "   Enter the total amount of restoration owed to the Food Stamp " & vbCrLf & "household unless restoration is to be manually issued or produced " & vbCrLf & "via an overnight machine issuance.  If restoration is to be in a lump " & vbCrLf & "sum, this amount will be added to the monthly allotment at cutoff " & vbCrLf & "processing.  Whether restoration is to be issued in a lump sum " & vbCrLf & "amount or monthly installments, block NB must be completed.  " & vbCrLf & "Computer generated restorations will be issued only for cases in " & vbCrLf & "Active or Closed/Restoration status.  This issuance will be reflected " & vbCrLf & "in block AM."
            Case "NB"
                Note = "RESTORATION AMOUNT MONTHLY INSTALLMENT (FS)" & vbCrLf & "   Enter the restoration monthly installment amount, whether an " & vbCrLf & "installment or lump sum payment."
            Case "NC"
                Note = "RESTORATION AMOUNT ISSUED TO DATE (FS)" & vbCrLf & "   The amount in this block represents the total restorations issued to " & vbCrLf & "the food stamp household.  At cutoff processing, this block will be " & vbCrLf & "updated automatically if block NA is greater than zero."
            Case "ND"
                Note = "ISSUANCE CODE (FS)" & vbCrLf & "   Enter one of the following codes that indicates the type of FS benefit " & vbCrLf & "issued and/or cancelled."
            Case "NE"
                Note = "REPLACEMENT ACTION/NUMBER OF FS BENEFITS TO BE ISSUED (FS)" & vbCrLf & "   Enter the code which indicates the action taken.  Acceptable codes " & vbCrLf & "for block NE are dependent upon the issuance code and the time of " & vbCrLf & "transaction with respect to cutoff."
            Case "NF"
                Note = "REPLACEMENT REASON (FS)" & vbCrLf & "   Enter one of the following codes that indicates the reason for " & vbCrLf & "replacement of the FS benefit or coupons."
            Case "NG"
                Note = "FS ISSUANCE DATE (FS)" & vbCrLf & "   For all machine issuances, enter month, day and year of FS " & vbCrLf & "issuance."
            Case "NH"
                Note = "ISSUANCE AMOUNT (FS)" & vbCrLf & "   Enter the amount of the FS benefit to be issued.  For all " & vbCrLf & "cancellations, leave this block blank."
            Case "NI"
                Note = "REPLACED OR CANCELLED FS ISSUANCE (FS)" & vbCrLf & "   Enter the number of the FS issuance that is being replaced or " & vbCrLf & "cancelled."
            Case "NJ"
                Note = "FS ISSUANCE NUMBER (FS)" & vbCrLf & "   Enter the FS authorization number of the manual issuance.  An " & vbCrLf & "entry in this block is mandatory for all manual transactions except " & vbCrLf & "'Cancel FS ISSUANCE'."
            Case "NK"
                Note = "FS INCOME ADJUSTMENT – MALE (FS)" & vbCrLf & "   Enter the total monthly amount of the following adjustments to " & vbCrLf & "represent FS income for the male adult."
            Case "NL"
                Note = "FS CATEGORICALLY ELIGIBLE (FS)" & vbCrLf & "   Enter 'E' to indicate the FS household is entitled to receive Post-" & vbCrLf & "TANF food stamp benefits, otherwise leave blank."
            Case "NM"
                Note = "CSP DISREGARD (CA/FS)" & vbCrLf & "   Computer Generated."
            Case "NN"
                Note = "EXEMPT CLOCK INDICATOR – FEMALE (CA)" & vbCrLf & "   Enter appropriate code to identify when female is exempt from the " & vbCrLf & "60 month clock."
            Case "NO"
                Note = "CSP DEDUCTION" & vbCrLf & "   Enter amount of CSP to be used as a deduction to food stamp " & vbCrLf & "income."
            Case "NP"
                Note = "DCN – FEMALE (CA/FS)" & vbCrLf & "   Computer Generated."
            Case "OA"
                Note = "FOOD STAMP CLAIMS INDICATOR (FS)" & vbCrLf & "   In order to perform the proper Food Stamp claim calculation a 2 " & vbCrLf & "character code must be entered.  The first character will be used to " & vbCrLf & "calculate the first set of claim blocks.  The second character will be " & vbCrLf & "used to calculate the second set of claim blocks."
            Case "OB"
                Note = "FOOD STAMP CLAIM BALANCE/INTENTIONAL PROGRAM VIOLATION (FS)" & vbCrLf & "   Enter the amount of the claim before the next reduction.  The " & vbCrLf & "system will reduce the balance automatically each time a computer " & vbCrLf & "generated first-of-the-month allotment is issued with an allotment " & vbCrLf & "claim reduction."
            Case "OC"
                Note = "FOOD STAMP CLAIM AMOUNT MONTHLY/INTENTIONAL PROGRAM VIOLATION (FS)" & vbCrLf & "   Enter the amount of the claim reduction to be subtracted from the " & vbCrLf & "first-of-the-month allotment (block AM) whether an installment or " & vbCrLf & "lump sum collection."
            Case "OD"
                Note = "FOOD STAMP CLAIM AMOUNT COLLECTED TO " & vbCrLf & "DATE/INTENTIONAL PROGRAM VIOLATION (FS)" & vbCrLf & "   At cutoff, this block will be increased automatically by the lesser " & vbCrLf & "amount in block OB or OC."
            Case "OE"
                Note = "FOOD STAMP CLAIM BALANCE/OTHER TYPES (FS)" & vbCrLf & "   Enter the amount of the claim before the next reduction.  The " & vbCrLf & "system will reduce the balance automatically each time a computer " & vbCrLf & "generated first-of-the-month allotment is issued with an allotment " & vbCrLf & "claim reduction."
            Case "OF"
                Note = "FOOD STAMP CLAIM AMOUNT MONTHLY/OTHER TYPES (FS)" & vbCrLf & "   Enter the amount of the claim reduction to be subtracted from the " & vbCrLf & "first-of-the-month allotment (block AM) whether an installment or " & vbCrLf & "lump sum collection."
            Case "OG"
                Note = "FOOD STAMP CLAIM AMOUNT COLLECTED TO DATE/OTHER TYPES (FS)" & vbCrLf & "   At cutoff, this block will be increased automatically by the lesser " & vbCrLf & "amount in block OE or OF."
            Case "OH"
                Note = "FOOD STAMP CLOSING ACTION REASON (FS)" & vbCrLf & "   System Generated – Future Development"
            Case "OI"
                Note = "FOOD STAMP CLOSING DATE (FS)" & vbCrLf & "   System Generated – Future Development"
            Case "OJ"
                Note = "NO. OF SANCTIONED INDIVIDUALS (CA/FS/MED)" & vbCrLf & "   Enter 01 – 26 for the number of active sanctioned individuals on the " & vbCrLf & "case."
            Case "OK"
                Note = "TELEPHONE NUMBER (CA/FS/MED)	" & vbCrLf & "   Enter the seven digits of the household’s telephone number."
            Case "OL"
                Note = "TOTAL VRP AMOUNT (CA)" & vbCrLf & "   Computer Generated/Hand Entered"
            Case "OM"
                Note = "EXEMPT CLOCK INDICATOR – MALE (CA)" & vbCrLf & "   Enter appropriate code to identify when the male is exempt from the " & vbCrLf & "60 month clock."
            Case "ON"
                Note = "HOLD INDICATOR (CA/FS)" & vbCrLf & "   Enter the appropriate code to indicate cases where a HOLD has " & vbCrLf & "been requested for TANF or FS issuances or Medicaid Cards."
            Case "OO"
                Note = "DCN – MALE (CA/FS)" & vbCrLf & "   Computer Generated"
            Case "OP"
                Note = "FUTURE DEVELOPMENT"
            Case "PA"
                Note = "I & A ISSUE DATE (CA/MED)" & vbCrLf & "   Enter the date of issuance of the I & A check or DCS payment."
            Case "PB"
                Note = "I & A PAY TYPE (CA/MED)" & vbCrLf & "   Enter the code to indicate the pay type."
            Case "PC"
                Note = "I & A INDICATOR (CA/MED)" & vbCrLf & "   Indicates the type of daily issuance.  Enter the appropriate code to " & vbCrLf & "indicate the type of daily issuance."
            Case "PD"
                Note = "I & A WARRANT NUMBER (CA)" & vbCrLf & "   Enter the warrant number, also referred to as the authorization " & vbCrLf & "number.  This block must have an entry if issuance type block PF " & vbCrLf & "equals 'T'."
            Case "PE"
                Note = "I & A ISSUANCE AMOUNT (CA)" & vbCrLf & "   Enter the amount of the authorization.  This block must be filled in " & vbCrLf & "for all I & A transactions on the 105A form (whether the " & vbCrLf & "authorization is generated by the computer or typewritten in the " & vbCrLf & "county)."
            Case "PF"
                Note = "I & A ISSUANCE TYPE (CA/MED)" & vbCrLf & "   Enter the appropriate code to show if the issuance is to be computer " & vbCrLf & "generated or typewritten in the county."
            Case "PG"
                Note = "PA CLOSING REASON (CA)" & vbCrLf & "   Computer Generated"
            Case "PH"
                Note = "PA CLOSING DATE (CA)" & vbCrLf & "   Computer Generated"
            Case "PI"
                Note = "I & A ISSUE DATE (CA/MED)" & vbCrLf & "   Enter the date of issuance of the I & A check or DCS payment."
            Case "PJ"
                Note = "I & A PAY TYPE (CA/MED)" & vbCrLf & "   Enter the code to indicate the pay type code."
            Case "PK"
                Note = "I & A INDICATOR (CA/MED)" & vbCrLf & "   Indicates the type of daily issuance."
            Case "PL"
                Note = "I & A WARRANT NUMBER (CA)" & vbCrLf & "   Enter the warrant number, also referred to as the authorization " & vbCrLf & "number.  This block must have an entry if issuance type block PN " & vbCrLf & "equals 'T'."
            Case "PM"
                Note = "I & A ISSUANCE AMOUNT (CA)" & vbCrLf & "   Enter the amount of the issuance.  This block must be filled in for all " & vbCrLf & "I & A transactions on the 105A form (whether the authorization is " & vbCrLf & "generated by the computer or typewritten in the county)."
            Case "PN"
                Note = "I & A ISSUANCE TYPE (CA/MED)" & vbCrLf & "   Enter the appropriate code to show if the issuance is to be computer " & vbCrLf & "generated or typewritten in the county."
            Case "PO"
                Note = "RESIDUAL AMOUNT (CA)" & vbCrLf & "   Computer Generated/Hand Entered"
            Case "PP"
                Note = "PA COUNTABLE RESOURCES (CA)" & vbCrLf & "   Enter the amount of the Eligible Unit’s total resources that have not " & vbCrLf & "been classified as resource exclusions."
            Case "QA"
                Note = "PERSON CODE (CA/FS/MED)" & vbCrLf & "   Computer Generated"
            Case "QB"
                Note = "INDIVIDUAL NAME – LAST (CA/FS/MED)" & vbCrLf & "   Enter the last name."
            Case "QC"
                Note = "INDIVIDUAL NAME – FIRST (CA/FS/MED)" & vbCrLf & "   Enter the first name."
            Case "QD"
                Note = "INDIVIDUAL NAME – MIDDLE INITIAL (CA/FS/MED)" & vbCrLf & "   Enter the middle initial, if any."
            Case "QE"
                Note = "SEX (CA/FS/MED)" & vbCrLf & "   Enter 'M' for male or 'F' for female."
            Case "QF"
                Note = "RACIAL CATEGORY (CA/FS/MED)" & vbCrLf & "   Report the person in only one racial group as they are regarded in " & vbCrLf & "the community."
            Case "QG"
                Note = "BIRTH DATE (CA/FS/MED)" & vbCrLf & "   Enter the date of birth.  For a Medicaid special case on behalf of an " & vbCrLf & "unborn child, enter the expected date of birth."
            Case "QH"
                Note = "MEDICAID PERSON NUMBER (MED)" & vbCrLf & "   Enter the appropriate Medicaid person number between '20' and " & vbCrLf & "'49' according to program regulations."
            Case "QI"
                Note = "PAYEE RELATION (CA/FS/MED)" & vbCrLf & "   Enter numeric code to show the individual’s relationship to payee.  If " & vbCrLf & "applicable, add appropriate alpha code suffix to signify relationship."
            Case "QJ"
                Note = "REASON INDIVIDUAL WAS REMOVED FROM GRANT (CA/MED)" & vbCrLf & "   Enter the code which indicates why the individual was removed " & vbCrLf & "from the grant and/or Medicaid."
            Case "QK"
                Note = "ADDED/REMOVED DATE (CA/MED)" & vbCrLf & "   Enter the date the individual was added or removed from the grant " & vbCrLf & "and/or Medicaid.  When program eligibility requires a person " & vbCrLf & "number change (block QH) a new date must be entered.  A hand " & vbCrLf & "entry in this block requires a hand entry in block QL."
            Case "QL"
                Note = "ADDED/REMOVED INDICATOR – CHILD (CA/MED)" & vbCrLf & "   Enter one of the following codes to indicate status of the individual.  " & vbCrLf & "When an entry is made in this block a date must be hand entered in " & vbCrLf & "block QK."
            Case "QM"
                Note = "SOCIAL SECURITY NUMBER (CA/FS/MED)" & vbCrLf & "   Enter one of the following for the individual coded in block QB."
            Case "QN"
                Note = "TYPE PERSON INDICATOR (CA/FS/MED)" & vbCrLf & "   Enter the code that indicates the type of benefits received."
            Case "QO"
                Note = "SEGMENT CODE – INDIVIDUAL (CA/MED)" & vbCrLf & "   In companion cases and all TANF refugee cases, this block will " & vbCrLf & "describe the category of assistance which applies to the individual " & vbCrLf & "coded in block QB."
            Case "RA"
                Note = "SOCIAL SECURITY (OR OTHER) CLAIM NUMBER (CA/FS/MED)" & vbCrLf & "   Enter the alpha-numeric claim number associated with the benefit, " & vbCrLf & "RSDI, Railroad Retirement, public or private pension or Medicare " & vbCrLf & "that the individual coded in block QB is receiving.  This number will " & vbCrLf & "contain 9 numeric and 1 to 3 alpha characters.  The alpha " & vbCrLf & "character(s) will indicate the type of benefit received."
            Case "RB"
                Note = "HOSPITAL INSURANCE TYPE (MED)" & vbCrLf & "Enter the appropriate two digit code ('00' – '99')."
            Case "RC"
                Note = "HEALTH INSURANCE POLICY NUMBER (MED)" & vbCrLf & "   Enter the number of the insurance policy that covers the individual " & vbCrLf & "coded in block QB."
            Case "RD"
                Note = "PRIVATE HEALTH INSURANCE INDICATOR (MED)" & vbCrLf & "   Enter '01' if the child does not have any private health insurance."
            Case "RE"
                Note = "DRUGS (MED)" & vbCrLf & "   Enter the appropriate code to identify the insurance company providing the coverage for the " & vbCrLf & "individual coded in block QB."
            Case "RF"
                Note = "PROGRAM STATUS CODE INDICATOR (MED)" & vbCrLf & "   System generated field based on Medicaid calculation."
            Case "RG"
                Note = "ADOLESCENT PARENT/CHILD (CA/MED)" & vbCrLf & "To identify an adolescent parent:" & vbCrLf & "   Enter an 'A' in the first position.  The second position should be " & vbCrLf & "coded with a number '1' through '9', to indicate the number of " & vbCrLf & "children the parent has.  For example, 'A3' identifies this individual " & vbCrLf & "as an adolescent parent on the case, who has 3 children."
            Case "RH"
                Note = "WIN REGISTRATION (CA) (WSP)" & vbCrLf & "   Enter RG1 through RG9 (RG0 is a computer generated value) to " & vbCrLf & "indicate participation in the Work Supplementation Program (WSP)."
            Case "RI"
                Note = "WORK REGISTRATION (FS)" & vbCrLf & "   Enter the appropriate code from the list in Appendix D to show the " & vbCrLf & "current work registration status of the individual coded in block QB.  " & vbCrLf & "An entry is required only when the individual is in the Food Stamp " & vbCrLf & "eligible household."
            Case "RJ"
                Note = "EDUCATION LEVEL (CA/FS/MED)" & vbCrLf & "   Enter the appropriate code to designate the education level of the " & vbCrLf & "individual coded in block QB."
            Case "RK"
                Note = "SCHOOL DISTRICT (CA)" & vbCrLf & "   Enter the code, indicating the school district."
            Case "RL"
                Note = "SSI REFERENCE (CA/FS)" & vbCrLf & "   Enter the appropriate code to indicate whether or not the individual " & vbCrLf & "receives SSI."
            Case "RM"
                Note = "DEPRIVATION FACTOR (MED)" & vbCrLf & "   Enter the appropriate code to indicate the reason for deprivation " & vbCrLf & "relating to the natural or adoptive parent for the child coded in block " & vbCrLf & "QB."
            Case "RN"
                Note = "WORK START DATE – CHILD (MED)" & vbCrLf & "   Enter the month and year that the $30 and 1/3 earned income " & vbCrLf & "disregard is applied against the earnings coded in block SA, or " & vbCrLf & "when block SR is coded '0' thru '7', 'G', 'F', 'P', 'X' or 'Y'."
            Case "RO"
                Note = "WORK STOP DATE – CHILD (MED)" & vbCrLf & "   Enter the last month and year for which the $30 and 1/3 or $30 " & vbCrLf & "earned income disregard is applied against the earnings coded in " & vbCrLf & "block SA."
            Case "RP"
                Note = "DEPENDENT CARE INDICATOR (MED)" & vbCrLf & "   This block is used to show the type of care the child in block QB is receiving.  "
            Case "RQ"
                Note = "DEPENDENT CARE AMOUNT (CA)" & vbCrLf & "   Enter the amount of child or dependent care costs allowed."
            Case "RR"
                Note = "SSN INDICATOR (CA/FS)" & vbCrLf & "   Computer Generated"
            Case "SA"
                Note = "EARNED INCOME (CA/FS/MED)" & vbCrLf & "   Enter the monthly gross amount of the income earned by the " & vbCrLf & "individual coded in block QB.  If self-employed, enter the adjusted " & vbCrLf & "gross earned income (gross minus business expenses)."
            Case "SB"
                Note = "FS INCOME ADJUSTMENT – INDIVIDUAL (FS)" & vbCrLf & "   Enter the total monthly amount of the following adjustments to " & vbCrLf & "represent FS income for the individual coded in block QB."
            Case "SC"
                Note = "RSDI (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of RSDI benefits received by the " & vbCrLf & "individual coded in block QB."
            Case "SD"
                Note = "VA (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any VA pension or compensation " & vbCrLf & "received by the individual coded in block QB."
            Case "SE"
                Note = "PENSION (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any pensions received by the " & vbCrLf & "individual coded in block QB.  Do not include RSDI, SSI or VA."
            Case "SF"
                Note = "UIB (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any UIB received by the individual " & vbCrLf & "coded in block QB."
            Case "SG"
                Note = "CONTRIBUTIONS (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any contributions received by the " & vbCrLf & "individual coded in block QB."
            Case "SH"
                Note = "OTHER INCOME (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of any unearned income that the " & vbCrLf & "individual coded in block QB receives from a source not identified in " & vbCrLf & "the other unearned income blocks, SC, SD, SE, SF, SG, SO and " & vbCrLf & "SP."
            Case "SI"
                Note = "DISREGARD INDICATOR (CA)" & vbCrLf & "   Enter 'A' when 100% TANF disregard should be applied.  A 'space' " & vbCrLf & "will incur the normal 50% TANF disregard."
            Case "SJ"
                Note = "CHILD CARE EXPENSES (CA/MED)" & vbCrLf & "   Enter the monthly amount of any child care expenses paid by the " & vbCrLf & "parent minor coded in block QB."
            Case "SK"
                Note = "DISREGARDS (CA)" & vbCrLf & "Computer Generated"
            Case "SL"
                Note = "DIVERTED INCOME (CA/MED)" & vbCrLf & "   Enter the monthly amount of court ordered support payments made " & vbCrLf & "by the individual coded in block QB to dependents living elsewhere"
            Case "SM"
                Note = "TOTAL NET INCOME (CA)" & vbCrLf & "Computer Generated"
            Case "SN"
                Note = "DATE OF ENTRY (CA/FS/MED)" & vbCrLf & "   If the individual is a refugee, enter the date of entry into the country."
            Case "SO"
                Note = "SSI (CA/FS)" & vbCrLf & "   Enter the monthly amount of SSI received by the individual coded in " & vbCrLf & "block QB."
            Case "SP"
                Note = "GA (CA/FS/MED)" & vbCrLf & "   Enter the monthly amount of General Assistance received by the " & vbCrLf & "individual coded in block QB."
            Case "SQ"
                Note = "FS INDICATOR – INDIVIDUAL (FS)" & vbCrLf & "   Enter an indicator for the amount shown in block SB."
            Case "SR"
                Note = "CHILD’S INCOME INDICATOR (MED)" & vbCrLf & "   There must be an entry in this block when a child has earned " & vbCrLf & "income in block SA indicating whether the income is applied to the " & vbCrLf & "eligibility test only, or to the gross income eligibility test and the case " & vbCrLf & "net income total. This block must be left blank when there is no " & vbCrLf & "earned income for this individual."
            Case "SS"
                Note = "FS HEAD OF HOUSEHOLD (FS)" & vbCrLf & "   Enter Y – Yes or N – No to identify the FS Head of Household."
            Case "ST"
                Note = "WEP INDICATOR" & vbCrLf & "   Future Development"
            Case "TA"
                Note = "DCN – CHILD (CA)" & vbCrLf & "   Computer Generated"
            Case "TB"
                Note = "GOOD CAUSE INDICATOR (CA)" & vbCrLf & "   Enter the appropriate code to indicate if there is a Good Cause or " & vbCrLf & "Deprivation Factor.  When incapacity is the sole deprivation factor " & vbCrLf & "for a child, that child will not be referred to ACSES."
            Case "TC"
                Note = "CHILD SUPPORT AMOUNT (CA)" & vbCrLf & "   Computer Generated"
            Case "TD"
                Note = "FUTURE DEVELOPMENT"
            Case "TE"
                Note = "KINSHIP REDETEMINATION DATE – CHILD (CA/FS/MED)" & vbCrLf & "   Enter a valid date (MMDDCCYY) when the Kinship child is due for " & vbCrLf & "redetermination."
            Case "TF"
                Note = "EBT CLIENT CODE" & vbCrLf & "   Computer Generated"
            Case "TG"
                Note = "TEEN PARENT INDICATOR (CA/FS)" & vbCrLf & "   Enter one of the following codes to identify the teen parents living " & vbCrLf & "arrangements."
            Case "TH"
                Note = "TEEN PARENT REASON (CA/FS)" & vbCrLf & "   Enter one of the following codes to identify the reason for the living " & vbCrLf & "arrangements.  Spaces are allowed."
            Case "TI"
                Note = "WFNJ IDENTIFIER – CHILD (CA/MED)" & vbCrLf & "   Enter the appropriate WFNJ Status Code for the individual."
            Case "TJ"
                Note = "FUTURE DEVELOPMENT"
            Case "TK"
                Note = "SANCTION INDICATOR – CHILD (CA/FS/MED)" & vbCrLf & "   Refer to Riverside Rule Instruction and FAMIS Sanction " & vbCrLf & "Coding Desk Guide for specific coding instructions."
            Case "TL"
                Note = "SANCTION EFFECTIVE – CHILD (CA/FS/MED)" & vbCrLf & "   Enter the effective date of the action taken in the Sanction Indicator " & vbCrLf & "field (TK/90) (MMDDCCYY)."
            Case "TM"
                Note = "NUMBER OF MONTHS IN PRIOR STATE (CA/FS)" & vbCrLf & "   Enter 01 – 99."
            Case "TN"
                Note = "FUTURE DEVELOPMENT"
            Case "TO"
                Note = "SPOUSE OF TEEN PARENT (CA/FS)" & vbCrLf & "   Enter Person Code 'C – Z' to identify the teen parent."
            Case "TP"
                Note = "ETHNICITY" & vbCrLf & "   Enter the appropriate code for the ethnic category the person is " & vbCrLf & "regarded in the community."
            Case "TQ"
                Note = "EXEMPT CLOCK INDICATOR – CHILD (CA/FS)" & vbCrLf & "   Enter appropriate code to identify when the individual is exempt " & vbCrLf & "from the 60 month clock."
            Case "TR"
                Note = "FAMILY CAP INDICATOR (CA/FS)" & vbCrLf & "   If entered reason the family cap child is included in the eligible unit."
            Case "TS"
                Note = "PRIOR STATE OF RESIDENCE (CA/FS)" & vbCrLf & "   Future Development"
            Case "UA"
                Note = "ALIEN REGISTRATION NUMBER - CHILD (CA/FS/MED)" & vbCrLf & "   Enter the alien registration assigned by immigration."
            Case "UB"
                Note = "ALIEN TYPE CHILD (CA/FS/MED)" & vbCrLf & "   Enter alien status.  "
            Case "UC"
                Note = "INS VERIFICATION INDICATOR" & vbCrLf & "   Future Development – Field Protected"
            Case "UD"
                Note = "FELON INDICATOR – CHILD (CA/FS/MED)" & vbCrLf & "   Enter indicator if convicted.  "
            Case "UE"
                Note = "SPONSOR/LRR RESOURCE AMOUNT (CA/FS)" & vbCrLf & "   Enter resource amount of alien sponsor or legally responsible " & vbCrLf & "relative."
            Case "UF"
                Note = "SPONSOR/LRR INDICATOR (CA/FS)" & vbCrLf & "   Enter to identify a sponsor or legally responsible relative and if " & vbCrLf & "deeming of income is required."
            Case "UG"
                Note = "TEMPORARY DISABILITY INCOME – CHILD (CA/FS/MED)" & vbCrLf & "   Enter monthly amount of any Temporary Disability income received."
            Case "UH"
                Note = "WORKMEN’S COMPENSATION INCOME – CHILD (CA/FS/MED)" & vbCrLf & "   Enter monthly amount of any Workmen’s Compensation income " & vbCrLf & "received."
            Case "UI"
                Note = "MEDICAID ELIGIBLE INDICATOR - CHILD"
            Case "UJ"
                Note = "FUTURE DEVELOPMENT"
            Case "UK"
                Note = "MEDICAID DISREGARD AMOUNT (MED)" & vbCrLf & "   Computer generated in the Medicaid calculation."
            Case "UL"
                Note = "MEDICAID TOTAL NET INCOME (MED)" & vbCrLf & "   Computer generated in the Medicaid calculation."
            Case "VA"
                Note = "SERVICE ID" & vbCrLf & "   Enter the social service supervisor/worker codes."
            Case "VC"
                Note = "VRP REASON" & vbCrLf & "   Enter the reason the voluntary restricted payment is necessary."
            Case "VE"
                Note = "PROVIDER CODE" & vbCrLf & "   Enter the valid provider code of the vendor that the payment is " & vbCrLf & "being established."
            Case "VG"
                Note = "PROVIDER TYPE" & vbCrLf & "   Enter the provider type for the vendor that the payment is being " & vbCrLf & "established."
            Case "VI"
                Note = "SSN/Federal ID" & vbCrLf & "   Enter the numeric provider file SSN/Federal ID."
            Case "VK"
                Note = "VRP/RA Amount" & vbCrLf & "   Enter the amount of the voluntary restricted or rental assistance " & vbCrLf & "payment made to the individual provider."
            Case "VM"
                Note = "START DATE" & vbCrLf & "   Enter the issuance date of the first typewritten or computer " & vbCrLf & "generated restricted or rental assistance payment."
            Case "VO"
                Note = "END DATE" & vbCrLf & "   Enter the issuance date of the last typewritten or computer " & vbCrLf & "generated restricted or rental assistance payment to be generated."
            Case "VQ"
                Note = "ACCOUNT NUMBER" & vbCrLf & "   Enter the client’s account number (i.e., electric company account " & vbCrLf & "number)."
            Case "WA"
                Note = "MEDICAID ACTION (MED)" & vbCrLf & "   Enter appropriate code to show type action taken on a Medicaid " & vbCrLf & "case.  This block cannot be deleted."
            Case "WB"
                Note = "MEDICAID – REASON (MED)" & vbCrLf & "   Enter appropriate code to show reason for the action code in block " & vbCrLf & "WA.  At the present time these reasons will correspond to TANF " & vbCrLf & "reasons.  This block cannot be deleted."
            Case "WC"
                Note = "MEDICAID EFFECTIVE DATE (MED)" & vbCrLf & "   Enter date the action taken in block WA occurs.  Must equal " & vbCrLf & "'MMDDCCYY'.  Cannot be deleted."
            Case "WD"
                Note = "MEDICAID APPLICATION DATE (MED)" & vbCrLf & "   Enter the Medicaid application date the PA-1J was filed by " & vbCrLf & "applicant.  Must equal 'MMDDCCYY'."
            Case "WE"
                Note = "MEDICAID APPLICATION TYPE (MED)" & vbCrLf & "   Required entry.  "
            Case "WF"
                Note = "MEDICAID VALIDATION DATE (MED)" & vbCrLf & "   Enter the Medicaid validation date.  Must equal 'MMDDCCYY'.  " & vbCrLf & "Cannot precede the Application Date (WD/406) or exceed date of " & vbCrLf & "next issuance.  Enter the first day of the following month " & vbCrLf & "(MMDDCCYY) in which the worker has received all the materials " & vbCrLf & "required for the validation of the Medicaid."
            Case "WG"
                Note = "MEDICAID DATE LAST REVIEWED (MED)" & vbCrLf & "   Enter the date the Medicaid stub is to be issued.  Must equal " & vbCrLf & "'MMDDCCYY'."
            Case "WH"
                Note = "NUMBER OF ELIGIBLE MEDICAID – CHILD (MED)" & vbCrLf & "   Enter the number of Medicaid eligible children as indicated by " & vbCrLf & "entries of 'A', 'Y' or 'U' and Female Medicaid Person Code " & vbCrLf & "(HE666) = '05' in block QL."
            Case "WI"
                Note = "NUMBER OF ELIGIBLE MEDICAID ADULTS (MED)" & vbCrLf & "   Enter the number of Medicaid eligible adults as indicated by entries " & vbCrLf & "of 'A' or 'B' in blocks GC and/or GI."
            Case "WK"
                Note = "MEDICAID DISREGARD AMOUNT – FEMALE (MED)" & vbCrLf & "   Computer generated.  Calculated in Medicaid pre-calculation."
            Case "WL"
                Note = "MEDICAID AMOUNT (MED)" & vbCrLf & "   This block is a hand entry/computer generated field used to enter " & vbCrLf & "the Medicaid grant amount."
            Case "WM"
                Note = "MEDICAID TOTAL NET INCOME – FEMALE (MED)" & vbCrLf & "   Computer generated.  Calculated in Medicaid pre-calculation."
            Case "WN"
                Note = "MEDICAID INITIAL GROSS (MED)" & vbCrLf & "   Computer generated/hand entry must be numeric."
            Case "WO"
                Note = "MEDICAID INITIAL NET INCOME (MED)" & vbCrLf & "   Computer generated/hand entry must be numeric."
            Case "WP"
                Note = "FUTURE DEVELOPMENT"
            Case "WQ"
                Note = "MEDICAID INITIAL ELIGIBILITY (MED)" & vbCrLf & "   Computer generated/hand entry must be numeric."
            Case "WR"
                Note = "MEDICAID CASE COUNTABLE NET INCOME (MED)" & vbCrLf & "   Computer generated/calculated in Medicaid pre-calculation."
            Case "WS"
                Note = "MEDICAID TOTAL REQUIRED (MED)" & vbCrLf & "   Computer generated/hand entry."
            Case "WT"
                Note = "MEDICAID REDUCTION TO PAYMENT (MED)" & vbCrLf & "   Computer generated/calculated in Medicaid calculation."
            Case "WU"
                Note = "MEDICAID DISREGARD AMOUNT – MALE (MED)" & vbCrLf & "   Computer generated/calculated in Medicaid pre-calculation."
            Case "WW"
                Note = "MEDICAID SEGMENT CODE" & vbCrLf & "   Enter the Medicaid segment code.  Allowable codes are:  C, F, K, L, " & vbCrLf & "CR, FR, KR, LR, RR, CH, FH, KH, LH, RH."
            Case "WV"
                Note = "MEDICAID TOTAL NET INCOME – MALE (MED)" & vbCrLf & "   Computer generated/calculated in Medicaid pre-calculation."
            Case "WX"
                Note = "HEA ACCOUNT NUMBER (FS)" & vbCrLf & "   Hand entry.  When cases are coded K – electricity or N – gas in the " & vbCrLf & "Heating Indicator block (CG/757), code utility company and HEA " & vbCrLf & "account number."
            Case "WY"
                Note = "HEA PROVIDER NUMBER (FS)" & vbCrLf & "Hand entry.  "
            Case "XA"
                Note = "SANCTION EFFECTIVE DATE – FEMALE (CA/FS/MED)" & vbCrLf & "   Enter the effective date (MMDDCCYY) of the action taken in the " & vbCrLf & "sanction indicator field (BR/803)."
            Case "XB"
                Note = "NUMBER OF MONTHS IN PRIOR STATE – FEMALE" & vbCrLf & "   Enter 01 – 99."
            Case "XC"
                Note = "TEEN PARENT INDICATOR – FEMALE (CA/FS)" & vbCrLf & "   Enter one of the following codes to identify the reason for the teen " & vbCrLf & "parent’s living arrangement."
            Case "XD"
                Note = "TEEN PARENT REASON – FEMALE (CA/FS)" & vbCrLf & "   Enter one of the following codes to identify the reason for the living " & vbCrLf & "arrangements.  Spaces are allowed."
            Case "XE"
                Note = "ETHNICITY – FEMALE (CA/FS)" & vbCrLf & "   Enter the appropriate code for the ethnic category the female " & vbCrLf & "person is regarded in the community."
            Case "XF"
                Note = "FS HEAD OF HOUSEHOLD – FEMALE" & vbCrLf & "   Enter 'Y' – Yes or 'N' – No to identify the FS Head of Household."
            Case "XG"
                Note = "SANCTION EFFECTIVE DATE – MALE (CA/FS/MED)" & vbCrLf & "   Enter the effective date (MMDDCCYY) of the action taken in the " & vbCrLf & "sanction indicator field (BI/760)."
            Case "XH"
                Note = "NUMBER OF MONTHS IN PRIOR STATE – MALE" & vbCrLf & " Enter 01 – 99."
            Case "XI"
                Note = "TEEN PARENT INDICATOR – MALE (CA/FS)" & vbCrLf & "   Enter one of the following codes to identify the reason for the teen " & vbCrLf & "parents living arrangement."
            Case "XJ"
                Note = "TEEN PARENT REASON – MALE (CA/FS)" & vbCrLf & "   Enter one of the following codes to identify the reason for the teen " & vbCrLf & "parent living arrangement.  Spaces are allowed."
            Case "XK"
                Note = "ETHNICITY – MALE" & vbCrLf & "   Enter the appropriate code for the ethnic category the male person " & vbCrLf & "is regarded in the community."
            Case "XL"
                Note = "FS HEAD OF HOUSEHOLD – MALE (FS)" & vbCrLf & "   Enter 'Y' – Yes or 'N' – No to identify the FS Head of Household."
            Case "XM"
                Note = "PRIOR STATE OF RESIDENCE – FEMALE" & vbCrLf & "   Future Development"
            Case "XN"
                Note = "PRIOR STATE OF RESIDENCE – MALE" & vbCrLf & "   Future Development"
            Case Else
                Note = "No Information Available"
        End Select
        ToolTips.SetToolTip(DirectCast(sender, TextBox), Note)
    End Sub

    Private Sub Fill105A()
        txt_AA.Text = FAMISCaseInformation.AA.GetData
        txt_AB.Text = FAMISCaseInformation.AB.GetData
        txt_AC.Text = FAMISCaseInformation.AC.GetData
        txt_AD.Text = FAMISCaseInformation.AD.GetData
        txt_AE.Text = FAMISCaseInformation.AE.GetData
        txt_AF.Text = FAMISCaseInformation.AF.GetData
        txt_AG.Text = FAMISCaseInformation.AG.GetData
        txt_AH.Text = FAMISCaseInformation.AH.GetData
        txt_AI.Text = FAMISCaseInformation.AI.GetData
        txt_AJ.Text = FAMISCaseInformation.AJ.GetData
        txt_AK.Text = FAMISCaseInformation.AK.GetData
        txt_AL.Text = FAMISCaseInformation.AL.GetData
        txt_AM.Text = FAMISCaseInformation.AM.GetData
        txt_AN.Text = FAMISCaseInformation.AN.GetData

        txt_BA.Text = FAMISApplicationInformation.BA.GetData
        txt_BB.Text = FAMISApplicationInformation.BB.GetData
        txt_BC.Text = FAMISApplicationInformation.BC.GetData
        txt_BD.Text = FAMISApplicationInformation.BD.GetData
        txt_BE.Text = FAMISApplicationInformation.BE.GetData
        txt_BF.Text = FAMISApplicationInformation.BF.GetData
        txt_BG.Text = FAMISApplicationInformation.BG.GetData
        txt_BH.Text = FAMISApplicationInformation.BH.GetData
        txt_BI.Text = FAMISApplicationInformation.BI.GetData
        txt_BJ.Text = FAMISApplicationInformation.BJ.GetData
        txt_BK.Text = FAMISApplicationInformation.BK.GetData
        txt_BL.Text = FAMISApplicationInformation.BL.GetData
        txt_BM.Text = FAMISApplicationInformation.BM.GetData
        txt_BN.Text = FAMISApplicationInformation.BN.GetData
        txt_BO.Text = FAMISApplicationInformation.BO.GetData
        txt_BP.Text = FAMISApplicationInformation.BP.GetData
        txt_BQ.Text = FAMISApplicationInformation.BQ.GetData
        txt_BR.Text = FAMISApplicationInformation.BR.GetData

        txt_CA.Text = FAMISApplicationInformation.CA.GetData
        txt_CB.Text = FAMISApplicationInformation.CB.GetData
        txt_CC.Text = FAMISApplicationInformation.CC.GetData
        txt_CD.Text = FAMISApplicationInformation.CD1.GetData
        txt_CD2.Text = FAMISApplicationInformation.CD2.GetData
        txt_CE.Text = FAMISApplicationInformation.CE.GetData
        txt_CF.Text = FAMISApplicationInformation.CF.GetData
        txt_CG.Text = FAMISApplicationInformation.CG.GetData

        txt_DA1.Text = FAMISApplicationInformation.DA1.GetData
        txt_DA2.Text = FAMISApplicationInformation.DA2.GetData
        txt_DA3.Text = FAMISApplicationInformation.DA3.GetData
        txt_DB.Text = FAMISApplicationInformation.DB.GetData
        txt_DC.Text = FAMISApplicationInformation.DC.GetData
        txt_DD1.Text = FAMISApplicationInformation.DD1.GetData
        txt_DD2.Text = FAMISApplicationInformation.DD2.GetData
        txt_DE.Text = FAMISApplicationInformation.DE.GetData
        txt_DF.Text = FAMISApplicationInformation.DF.GetData

        txt_EA.Text = FAMISApplicationInformation.EA.GetData
        txt_EB.Text = FAMISApplicationInformation.EB.GetData
        txt_EC.Text = FAMISApplicationInformation.EC.GetData
        txt_ED1.Text = FAMISApplicationInformation.ED1.GetData
        txt_ED2.Text = FAMISApplicationInformation.ED2.GetData
        txt_EE.Text = FAMISApplicationInformation.EE.GetData
        txt_EF.Text = FAMISApplicationInformation.EF.GetData
        txt_EG.Text = FAMISApplicationInformation.EG.GetData
        txt_EH.Text = FAMISApplicationInformation.EH.GetData
        txt_EI.Text = FAMISApplicationInformation.EI.GetData
        txt_EJ.Text = FAMISApplicationInformation.EJ.GetData
        txt_EK.Text = FAMISApplicationInformation.EK.GetData
        txt_EL.Text = FAMISApplicationInformation.EL.GetData
        txt_EM.Text = FAMISApplicationInformation.EM.GetData
        txt_EN.Text = FAMISApplicationInformation.EN.GetData

        txt_FA.Text = FAMISIndividualsInformation.FA.GetData
        txt_FB.Text = FAMISIndividualsInformation.FB.GetData
        txt_FC.Text = FAMISIndividualsInformation.FC.GetData
        txt_FD.Text = FAMISIndividualsInformation.FD.GetData
        txt_FE1.Text = FAMISIndividualsInformation.FE1.GetData
        txt_FE2.Text = FAMISIndividualsInformation.FE2.GetData
        txt_FF.Text = FAMISIndividualsInformation.FF.GetData
        txt_FG1.Text = FAMISIndividualsInformation.FG.GetData
        txt_FH.Text = FAMISIndividualsInformation.FH.GetData
        txt_FI.Text = FAMISIndividualsInformation.FI.GetData
        txt_FJ.Text = FAMISIndividualsInformation.FJ.GetData
        txt_FK.Text = FAMISIndividualsInformation.FK.GetData
        txt_FL.Text = FAMISIndividualsInformation.FL.GetData
        txt_FM1.Text = FAMISIndividualsInformation.FM1.GetData
        txt_FM2.Text = FAMISIndividualsInformation.FM2.GetData
        txt_FN.Text = FAMISIndividualsInformation.FN.GetData
        txt_FO.Text = FAMISIndividualsInformation.FO.GetData
        txt_FP1.Text = FAMISIndividualsInformation.FP.GetData

        txt_GA.Text = FAMISIndividualsInformation.GA.GetData
        txt_GB.Text = FAMISIndividualsInformation.GB.GetData
        txt_GC.Text = FAMISIndividualsInformation.GC.GetData
        txt_GD.Text = FAMISIndividualsInformation.GD.GetData
        txt_GE.Text = FAMISIndividualsInformation.GE.GetData
        txt_GF.Text = FAMISIndividualsInformation.GF.GetData
        txt_GG.Text = FAMISIndividualsInformation.GG.GetData
        txt_GH.Text = FAMISIndividualsInformation.GH.GetData
        txt_GI.Text = FAMISIndividualsInformation.GI.GetData
        txt_GJ.Text = FAMISIndividualsInformation.GJ.GetData
        txt_GK.Text = FAMISIndividualsInformation.GK.GetData
        txt_GL.Text = FAMISIndividualsInformation.GL.GetData

        txt_HA.Text = FAMISMedicaidInformation.HA.GetData
        txt_HB.Text = FAMISMedicaidInformation.HB.GetData
        txt_HC.Text = FAMISMedicaidInformation.HC.GetData
        txt_HD.Text = FAMISMedicaidInformation.HD.GetData
        txt_HE.Text = FAMISMedicaidInformation.HE.GetData
        txt_HF.Text = FAMISMedicaidInformation.HF.GetData
        txt_HG.Text = FAMISMedicaidInformation.HG.GetData
        txt_HH.Text = FAMISMedicaidInformation.HH.GetData
        txt_HI.Text = FAMISMedicaidInformation.HI.GetData
        txt_HJ.Text = FAMISMedicaidInformation.HJ.GetData
        txt_HK.Text = FAMISMedicaidInformation.HK.GetData
        txt_HL.Text = FAMISMedicaidInformation.HL.GetData
        txt_HM.Text = FAMISMedicaidInformation.HM.GetData
        txt_HN.Text = FAMISMedicaidInformation.HN.GetData
        txt_HO.Text = FAMISMedicaidInformation.HO.GetData
        txt_HP.Text = FAMISMedicaidInformation.HP.GetData
        txt_HQ.Text = FAMISMedicaidInformation.HQ.GetData
        txt_HR.Text = FAMISMedicaidInformation.HR.GetData
        txt_HS.Text = FAMISMedicaidInformation.HS.GetData
        txt_HT.Text = FAMISMedicaidInformation.HT.GetData

        txt_IA.Text = FAMISTANFInformation.IA.GetData
        txt_IB.Text = FAMISTANFInformation.IB.GetData
        txt_IC.Text = FAMISTANFInformation.IC.GetData
        txt_ID.Text = FAMISTANFInformation.ID.GetData
        txt_IE.Text = FAMISTANFInformation.IE.GetData
        txt_IF.Text = FAMISTANFInformation.IF1.GetData
        txt_IG.Text = FAMISTANFInformation.IG.GetData
        txt_IH.Text = FAMISTANFInformation.IH.GetData
        txt_II.Text = FAMISTANFInformation.II.GetData
        txt_IJ.Text = FAMISTANFInformation.IJ.GetData
        txt_IK.Text = FAMISTANFInformation.IK.GetData
        txt_IL.Text = FAMISTANFInformation.IL.GetData
        txt_IM.Text = FAMISTANFInformation.IM.GetData
        txt_IN.Text = FAMISTANFInformation.IN1.GetData
        txt_IO.Text = FAMISTANFInformation.IO.GetData

        txt_JA.Text = FAMISIncomeInformation.JA.GetData
        txt_JB.Text = FAMISIncomeInformation.JB.GetData
        txt_JC.Text = FAMISIncomeInformation.JC.GetData
        txt_JD.Text = FAMISIncomeInformation.JD.GetData
        txt_JE.Text = FAMISIncomeInformation.JE.GetData
        txt_JF.Text = FAMISIncomeInformation.JF.GetData
        txt_JG.Text = FAMISIncomeInformation.JG.GetData
        txt_JH.Text = FAMISIncomeInformation.JH.GetData
        txt_JI.Text = FAMISIncomeInformation.JI.GetData
        txt_JJ.Text = FAMISIncomeInformation.JJ.GetData
        txt_JK.Text = FAMISIncomeInformation.JK.GetData
        txt_JL.Text = FAMISIncomeInformation.JL.GetData
        txt_JM.Text = FAMISIncomeInformation.JM.GetData
        txt_JN.Text = FAMISIncomeInformation.JN.GetData
        txt_JO.Text = FAMISIncomeInformation.JO.GetData
        txt_JP.Text = FAMISIncomeInformation.JP.GetData
        txt_JQ.Text = FAMISIncomeInformation.JQ.GetData
        txt_JR.Text = FAMISIncomeInformation.JR.GetData
        txt_JS.Text = FAMISIncomeInformation.JS.GetData
        txt_JT.Text = FAMISIncomeInformation.JT.GetData
        txt_JU.Text = FAMISIncomeInformation.JU.GetData

        txt_KA.Text = FAMISIncomeInformation.KA.GetData
        txt_KB.Text = FAMISIncomeInformation.KB.GetData
        txt_KC.Text = FAMISIncomeInformation.KC.GetData
        txt_KD.Text = FAMISIncomeInformation.KD.GetData
        txt_KE.Text = FAMISIncomeInformation.KE.GetData
        txt_KF.Text = FAMISIncomeInformation.KF.GetData
        txt_KG.Text = FAMISIncomeInformation.KG.GetData
        txt_KH.Text = FAMISIncomeInformation.KH.GetData
        txt_KI.Text = FAMISIncomeInformation.KI.GetData
        txt_KJ.Text = FAMISIncomeInformation.KJ.GetData
        txt_KK.Text = FAMISIncomeInformation.KK.GetData
        txt_KL.Text = FAMISIncomeInformation.KL.GetData
        txt_KM.Text = FAMISIncomeInformation.KM.GetData
        txt_KN.Text = FAMISIncomeInformation.KN.GetData
        txt_KO.Text = FAMISIncomeInformation.KO.GetData
        txt_KP.Text = FAMISIncomeInformation.KP.GetData
        txt_KQ.Text = FAMISIncomeInformation.KQ.GetData
        txt_KR.Text = FAMISIncomeInformation.KR.GetData
        txt_KS.Text = FAMISIncomeInformation.KS.GetData

        txt_LA.Text = FAMISFoodStampInformation.LA.GetData
        txt_LB.Text = FAMISFoodStampInformation.LB.GetData
        txt_LC.Text = FAMISFoodStampInformation.LC.GetData
        txt_LD.Text = FAMISFoodStampInformation.LD.GetData
        txt_LE.Text = FAMISFoodStampInformation.LE.GetData
        txt_LF.Text = FAMISFoodStampInformation.LF.GetData
        txt_LG.Text = FAMISFoodStampInformation.LG.GetData
        txt_LH.Text = FAMISFoodStampInformation.LH.GetData
        txt_LI.Text = FAMISFoodStampInformation.LI.GetData
        txt_LJ.Text = FAMISFoodStampInformation.LJ.GetData
        txt_LK.Text = FAMISFoodStampInformation.LK.GetData
        txt_LL.Text = FAMISFoodStampInformation.LL.GetData
        txt_LM.Text = FAMISFoodStampInformation.LM.GetData
        txt_LN.Text = FAMISFoodStampInformation.LN.GetData
        txt_LO.Text = FAMISFoodStampInformation.LO.GetData
        txt_LP.Text = FAMISFoodStampInformation.LP.GetData
        txt_LQ.Text = FAMISFoodStampInformation.LQ.GetData
        txt_LR.Text = FAMISFoodStampInformation.LR.GetData
        txt_LS.Text = FAMISFoodStampInformation.LS.GetData
        txt_LT.Text = FAMISFoodStampInformation.LT.GetData

        txt_MA.Text = FAMISFoodStampInformation.MA.GetData
        txt_MB.Text = FAMISFoodStampInformation.MB.GetData
        txt_MC.Text = FAMISFoodStampInformation.MC.GetData
        txt_MD.Text = FAMISFoodStampInformation.MD.GetData
        txt_ME.Text = FAMISFoodStampInformation.ME1.GetData
        txt_MF.Text = FAMISFoodStampInformation.MF.GetData
        txt_MG.Text = FAMISFoodStampInformation.MG.GetData
        txt_MH.Text = FAMISFoodStampInformation.MH.GetData
        txt_MI.Text = FAMISFoodStampInformation.MI.GetData
        txt_MJ.Text = FAMISFoodStampInformation.MJ.GetData
        txt_MK.Text = FAMISFoodStampInformation.MK.GetData
        txt_ML.Text = FAMISFoodStampInformation.ML.GetData
        txt_MM.Text = FAMISFoodStampInformation.MM.GetData
        txt_MN.Text = FAMISFoodStampInformation.MN.GetData
        txt_MO.Text = FAMISFoodStampInformation.MO.GetData
        txt_MP.Text = FAMISFoodStampInformation.MP.GetData
        txt_MQ.Text = FAMISFoodStampInformation.MQ.GetData
        txt_MR.Text = FAMISFoodStampInformation.MR.GetData

        txt_NA.Text = FAMISFoodStampInformation.NA.GetData
        txt_NB.Text = FAMISFoodStampInformation.NB.GetData
        txt_NC.Text = FAMISFoodStampInformation.NC.GetData
        txt_ND.Text = FAMISFoodStampInformation.ND.GetData
        txt_NE.Text = FAMISFoodStampInformation.NE.GetData
        txt_NF.Text = FAMISFoodStampInformation.NF.GetData
        txt_NG.Text = FAMISFoodStampInformation.NG.GetData
        txt_NH.Text = FAMISFoodStampInformation.NH.GetData
        txt_NI.Text = FAMISFoodStampInformation.NI.GetData
        txt_NJ.Text = FAMISFoodStampInformation.NJ.GetData
        txt_NK.Text = FAMISFoodStampInformation.NK.GetData
        txt_NL.Text = FAMISFoodStampInformation.NL.GetData
        txt_NM.Text = FAMISFoodStampInformation.NM.GetData
        txt_NN.Text = FAMISFoodStampInformation.NN.GetData
        txt_NO.Text = FAMISFoodStampInformation.NO.GetData
        txt_NP.Text = FAMISFoodStampInformation.NP.GetData

        txt_OA.Text = FAMISFoodStampInformation.OA.GetData
        txt_OB.Text = FAMISFoodStampInformation.OB.GetData
        txt_OC.Text = FAMISFoodStampInformation.OC.GetData
        txt_OD.Text = FAMISFoodStampInformation.OD.GetData
        txt_OE.Text = FAMISFoodStampInformation.OE.GetData
        txt_OF.Text = FAMISFoodStampInformation.OF1.GetData
        txt_OG.Text = FAMISFoodStampInformation.OG.GetData
        txt_OJ.Text = FAMISFoodStampInformation.OJ.GetData
        txt_OK1.Text = FAMISFoodStampInformation.OK1.GetData
        txt_OK.Text = FAMISFoodStampInformation.OK.GetData
        txt_OL.Text = FAMISFoodStampInformation.OL.GetData
        txt_OM.Text = FAMISFoodStampInformation.OM.GetData
        txt_ON.Text = FAMISFoodStampInformation.ON1.GetData
        txt_OO.Text = FAMISFoodStampInformation.OO.GetData

        txt_PA.Text = FAMISIandAInformation.PA.GetData
        txt_PB.Text = FAMISIandAInformation.PB.GetData
        txt_PC.Text = FAMISIandAInformation.PC.GetData
        txt_PD.Text = FAMISIandAInformation.PD.GetData
        txt_PE.Text = FAMISIandAInformation.PE.GetData
        txt_PF.Text = FAMISIandAInformation.PF.GetData
        txt_PG.Text = FAMISIandAInformation.PG.GetData
        txt_PH.Text = FAMISIandAInformation.PH.GetData
        txt_PI.Text = FAMISIandAInformation.PI.GetData
        txt_PJ.Text = FAMISIandAInformation.PJ.GetData
        txt_PK.Text = FAMISIandAInformation.PK.GetData
        txt_PL.Text = FAMISIandAInformation.PL.GetData
        txt_PM.Text = FAMISIandAInformation.PM.GetData
        txt_PN.Text = FAMISIandAInformation.PN.GetData
        txt_PO.Text = FAMISIandAInformation.PO.GetData
        txt_PP.Text = FAMISIandAInformation.PP.GetData
    End Sub
    Private Sub Fill105A1()
        txt_BS.Text = FAMISApplicationInformation.BS.GetData
        txt_BT.Text = FAMISApplicationInformation.BT.GetData
        txt_BU.Text = FAMISApplicationInformation.BU.GetData
        txt_BV.Text = FAMISApplicationInformation.BV.GetData
        txt_XA.Text = FAMISApplicationInformation.XA.GetData
        txt_XB.Text = FAMISApplicationInformation.XB.GetData
        txt_XC.Text = FAMISApplicationInformation.XC.GetData
        txt_XD.Text = FAMISApplicationInformation.XD.GetData
        txt_XE.Text = FAMISApplicationInformation.XE.GetData
        txt_XF.Text = FAMISApplicationInformation.XF.GetData
        txt_XM.Text = FAMISApplicationInformation.XM.GetData
        txt_JW.Text = FAMISIncomeInformation.JW.GetData
        txt_JX.Text = FAMISIncomeInformation.JX.GetData

        txt_BW.Text = FAMISApplicationInformation.BW.GetData
        txt_BX.Text = FAMISApplicationInformation.BX.GetData
        txt_BY.Text = FAMISApplicationInformation.BY1.GetData
        txt_BZ.Text = FAMISApplicationInformation.BZ.GetData
        txt_XG.Text = FAMISApplicationInformation.XG.GetData
        txt_XH.Text = FAMISApplicationInformation.XH.GetData
        txt_XI.Text = FAMISApplicationInformation.XI.GetData
        txt_XJ.Text = FAMISApplicationInformation.XJ.GetData
        txt_XK.Text = FAMISApplicationInformation.XK.GetData
        txt_XL.Text = FAMISApplicationInformation.XL.GetData
        txt_XN.Text = FAMISApplicationInformation.XN.GetData
        txt_KU.Text = FAMISIncomeInformation.KU.GetData
        txt_KV.Text = FAMISIncomeInformation.KV.GetData

        txt_WX.Text = FAMISFoodStampInformation.WX.GetData
        txt_WY.Text = FAMISFoodStampInformation.WY.GetData

        txt_WL.Text = FAMISMedicaidInformation.WL.GetData
        txt_WW.Text = FAMISMedicaidInformation.WW.GetData
        txt_WA.Text = FAMISMedicaidInformation.WA.GetData
        txt_WB.Text = FAMISMedicaidInformation.WB.GetData
        txt_WC.Text = FAMISMedicaidInformation.WC.GetData
        txt_WD.Text = FAMISMedicaidInformation.WD.GetData
        txt_WE.Text = FAMISMedicaidInformation.WE.GetData
        txt_WF.Text = FAMISMedicaidInformation.WF.GetData
        txt_WG.Text = FAMISMedicaidInformation.WG.GetData
        txt_WH.Text = FAMISMedicaidInformation.WH.GetData
        txt_WI.Text = FAMISMedicaidInformation.WI.GetData
        txt_WK.Text = FAMISMedicaidInformation.WK.GetData
        txt_WM.Text = FAMISMedicaidInformation.WM.GetData
        'txt_WN.Text = FAMISMedicaidInformation.WN.GetData
        'txt_WO.Text = FAMISMedicaidInformation.WO.GetData
        'txt_WP.Text = FAMISMedicaidInformation.WP.GetData
        'txt_WQ.Text = FAMISMedicaidInformation.WQ.GetData
        txt_WR.Text = FAMISMedicaidInformation.WR.GetData
        txt_WS.Text = FAMISMedicaidInformation.WS.GetData
        txt_WT.Text = FAMISMedicaidInformation.WT.GetData
        txt_WU.Text = FAMISMedicaidInformation.WU.GetData
        txt_WV.Text = FAMISMedicaidInformation.WV.GetData
    End Sub
    Private Sub FillFormB(ByVal PersonNumber As Integer)
        txt_QA.Text = FamisCaseChild(PersonNumber).QA.GetData
        txt_QB.Text = FamisCaseChild(PersonNumber).QB.GetData
        txt_QC.Text = FamisCaseChild(PersonNumber).QC.GetData
        txt_QD.Text = FamisCaseChild(PersonNumber).QD.GetData
        txt_QE.Text = FamisCaseChild(PersonNumber).QE.GetData
        txt_QF.Text = FamisCaseChild(PersonNumber).QF.GetData
        txt_QG.Text = FamisCaseChild(PersonNumber).QG.GetData
        txt_QH.Text = FamisCaseChild(PersonNumber).QH.GetData
        txt_QI.Text = FamisCaseChild(PersonNumber).QI.GetData
        txt_QJ.Text = FamisCaseChild(PersonNumber).QJ.GetData
        txt_QK.Text = FamisCaseChild(PersonNumber).QK.GetData
        txt_QL.Text = FamisCaseChild(PersonNumber).QL.GetData
        txt_QM.Text = FamisCaseChild(PersonNumber).QM.GetData
        txt_QN.Text = FamisCaseChild(PersonNumber).QN.GetData
        txt_QO.Text = FamisCaseChild(PersonNumber).QO.GetData

        txt_RA.Text = FamisCaseChild(PersonNumber).RA.GetData
        txt_RB.Text = FamisCaseChild(PersonNumber).RB.GetData
        txt_RC.Text = FamisCaseChild(PersonNumber).RC.GetData
        txt_RD.Text = FamisCaseChild(PersonNumber).RD.GetData
        txt_RE.Text = FamisCaseChild(PersonNumber).RE.GetData
        txt_RF.Text = FamisCaseChild(PersonNumber).RF.GetData
        txt_RG.Text = FamisCaseChild(PersonNumber).RG.GetData
        txt_RH.Text = FamisCaseChild(PersonNumber).RH.GetData
        txt_RI.Text = FamisCaseChild(PersonNumber).RI.GetData
        txt_RJ1.Text = FamisCaseChild(PersonNumber).RJ1.GetData & FamisCaseChild(PersonNumber).RJ2.GetData
        txt_RK.Text = FamisCaseChild(PersonNumber).RK.GetData
        txt_RL.Text = FamisCaseChild(PersonNumber).RL.GetData
        txt_RM.Text = FamisCaseChild(PersonNumber).RM.GetData
        txt_RN.Text = FamisCaseChild(PersonNumber).RN.GetData
        txt_RO.Text = FamisCaseChild(PersonNumber).RO.GetData
        txt_RP.Text = FamisCaseChild(PersonNumber).RP.GetData
        txt_RQ.Text = FamisCaseChild(PersonNumber).RQ.GetData
        txt_RR.Text = " "

        txt_SA.Text = FamisCaseChild(PersonNumber).SA.GetData
        txt_SB.Text = FamisCaseChild(PersonNumber).SB.GetData
        txt_SC.Text = FamisCaseChild(PersonNumber).SC.GetData
        txt_SD.Text = FamisCaseChild(PersonNumber).SD.GetData
        txt_SE.Text = FamisCaseChild(PersonNumber).SE.GetData
        txt_SF.Text = FamisCaseChild(PersonNumber).SF.GetData
        txt_SG.Text = FamisCaseChild(PersonNumber).SG.GetData
        txt_SH.Text = FamisCaseChild(PersonNumber).SH.GetData
        txt_SI.Text = FamisCaseChild(PersonNumber).SI.GetData
        txt_SJ.Text = FamisCaseChild(PersonNumber).SJ.GetData
        txt_SK.Text = FamisCaseChild(PersonNumber).SK.GetData
        txt_SL.Text = FamisCaseChild(PersonNumber).SL.GetData
        txt_SM.Text = FamisCaseChild(PersonNumber).SM.GetData
        txt_SN.Text = FamisCaseChild(PersonNumber).SN.GetData
        txt_SO.Text = FamisCaseChild(PersonNumber).SO.GetData
        txt_SP.Text = FamisCaseChild(PersonNumber).SP.GetData
        txt_SQ.Text = FamisCaseChild(PersonNumber).SQ.GetData
        txt_SR.Text = FamisCaseChild(PersonNumber).SR.GetData

        txt_TA.Text = FamisCaseChild(PersonNumber).TA.GetData
        txt_TB.Text = FamisCaseChild(PersonNumber).TB.GetData
        txt_TC.Text = "       "
        txt_TD.Text = FamisCaseChild(PersonNumber).TD.GetData
        txt_TE.Text = FamisCaseChild(PersonNumber).TE.GetData
        txt_TF.Text = FamisCaseChild(PersonNumber).TF.GetData
        txt_TG.Text = FamisCaseChild(PersonNumber).TG.GetData
        txt_TH.Text = FamisCaseChild(PersonNumber).TH.GetData
        txt_TI.Text = FamisCaseChild(PersonNumber).TI.GetData '& FamisCaseChild(PersonNumber).TI2.GetData & FamisCaseChild(PersonNumber).TI3.GetData
        txt_TJ.Text = FamisCaseChild(PersonNumber).TJ.GetData
        txt_TK.Text = FamisCaseChild(PersonNumber).TK.GetData
        txt_TL.Text = FamisCaseChild(PersonNumber).TL.GetData
        txt_TM.Text = FamisCaseChild(PersonNumber).TM.GetData

        txt_SS.Text = FamisCaseChild(PersonNumber).SS.GetData
        txt_TO1.Text = FamisCaseChild(PersonNumber).TO1.GetData
        txt_TP.Text = FamisCaseChild(PersonNumber).TP.GetData
        txt_TQ.Text = FamisCaseChild(PersonNumber).TQ.GetData
        txt_TR.Text = FamisCaseChild(PersonNumber).TR.GetData
        txt_TS.Text = FamisCaseChild(PersonNumber).TS.GetData
        txt_UA.Text = FamisCaseChild(PersonNumber).UA.GetData
        txt_UB.Text = FamisCaseChild(PersonNumber).UB.GetData
        txt_UC.Text = FamisCaseChild(PersonNumber).UC.GetData
        txt_UD.Text = FamisCaseChild(PersonNumber).UD.GetData
        txt_UE.Text = FamisCaseChild(PersonNumber).UE.GetData
        txt_UF.Text = FamisCaseChild(PersonNumber).UF.GetData
        txt_UG.Text = FamisCaseChild(PersonNumber).UG.GetData
        txt_UH.Text = FamisCaseChild(PersonNumber).UH.GetData
        txt_UI.Text = FamisCaseChild(PersonNumber).UI.GetData
        txt_UK.Text = FamisCaseChild(PersonNumber).UK.GetData
        txt_UL.Text = FamisCaseChild(PersonNumber).UL.GetData
    End Sub
    Private Sub FillFormC(ByVal PaymentNumber As Integer)
        txt_VA.Text = FAMISVRPInformation(PaymentNumber).VA.GetData
        txt_VC.Text = FAMISVRPInformation(PaymentNumber).VC.GetData
        txt_OL.Text = FAMISFoodStampInformation.OL.GetData
        txt_PO.Text = FAMISIandAInformation.PO.GetData
        txt_VE.Text = FAMISVRPInformation(PaymentNumber).VE.GetData
        txt_VG.Text = FAMISVRPInformation(PaymentNumber).VG.GetData
        txt_VI.Text = FAMISVRPInformation(PaymentNumber).VI.GetData
        txt_VK.Text = FAMISVRPInformation(PaymentNumber).VK.GetData
        txt_VM.Text = FAMISVRPInformation(PaymentNumber).VM.GetData
        txt_VO.Text = FAMISVRPInformation(PaymentNumber).VO.GetData
        txt_VQ.Text = FAMISVRPInformation(PaymentNumber).VQ.GetData
    End Sub

    Private Sub Transfer105A()
        FAMISCaseInformation.AA.SetData(txt_AA.Text)
        FAMISCaseInformation.AB.SetData(txt_AB.Text)
        FAMISCaseInformation.AC.SetData(txt_AC.Text)
        FAMISCaseInformation.AD.SetData(txt_AD.Text)
        FAMISCaseInformation.AE.SetData(txt_AE.Text)
        FAMISCaseInformation.AF.SetData(txt_AF.Text)
        FAMISCaseInformation.AG.SetData(txt_AG.Text)
        FAMISCaseInformation.AH.SetData(txt_AH.Text)
        FAMISCaseInformation.AI.SetData(txt_AI.Text)
        FAMISCaseInformation.AJ.SetData(txt_AJ.Text)
        FAMISCaseInformation.AK.SetData(txt_AK.Text)
        FAMISCaseInformation.AL.SetData(txt_AL.Text)
        FAMISCaseInformation.AM.SetData(txt_AM.Text)
        FAMISCaseInformation.AN.SetData(txt_AN.Text)

        FAMISApplicationInformation.BA.SetData(txt_BA.Text)
        FAMISApplicationInformation.BB.SetData(txt_BB.Text)
        FAMISApplicationInformation.BC.SetData(txt_BC.Text)
        FAMISApplicationInformation.BD.SetData(txt_BD.Text)
        FAMISApplicationInformation.BE.SetData(txt_BE.Text)
        FAMISApplicationInformation.BF.SetData(txt_BF.Text)
        FAMISApplicationInformation.BG.SetData(txt_BG.Text)
        FAMISApplicationInformation.BH.SetData(txt_BH.Text)
        FAMISApplicationInformation.BI.SetData(txt_BI.Text)
        FAMISApplicationInformation.BJ.SetData(txt_BJ.Text)
        FAMISApplicationInformation.BK.SetData(txt_BK.Text)
        FAMISApplicationInformation.BL.SetData(txt_BL.Text)
        FAMISApplicationInformation.BM.SetData(txt_BM.Text)
        FAMISApplicationInformation.BN.SetData(txt_BN.Text)
        FAMISApplicationInformation.BO.SetData(txt_BO.Text)
        FAMISApplicationInformation.BP.SetData(txt_BP.Text)
        FAMISApplicationInformation.BQ.SetData(txt_BQ.Text)
        FAMISApplicationInformation.BR.SetData(txt_BR.Text)

        FAMISApplicationInformation.CA.SetData(txt_CA.Text)
        FAMISApplicationInformation.CB.SetData(txt_CB.Text)
        FAMISApplicationInformation.CC.SetData(txt_CC.Text)
        FAMISApplicationInformation.CD1.SetData(txt_CD.Text)
        FAMISApplicationInformation.CD2.SetData(txt_CD2.Text)
        FAMISApplicationInformation.CE.SetData(txt_CE.Text)
        FAMISApplicationInformation.CF.SetData(txt_CF.Text)
        FAMISApplicationInformation.CG.SetData(txt_CG.Text)

        FAMISApplicationInformation.DA1.SetData(txt_DA1.Text)
        FAMISApplicationInformation.DA2.SetData(txt_DA2.Text)
        FAMISApplicationInformation.DA3.SetData(txt_DA3.Text)
        FAMISApplicationInformation.DB.SetData(txt_DB.Text)
        FAMISApplicationInformation.DC.SetData(txt_DC.Text)
        FAMISApplicationInformation.DD1.SetData(txt_DD1.Text)
        FAMISApplicationInformation.DD2.SetData(txt_DD2.Text)
        FAMISApplicationInformation.DE.SetData(txt_DE.Text)
        FAMISApplicationInformation.DF.SetData(txt_DF.Text)

        FAMISApplicationInformation.EA.SetData(txt_EA.Text)
        FAMISApplicationInformation.EB.SetData(txt_EB.Text)
        FAMISApplicationInformation.EC.SetData(txt_EC.Text)
        FAMISApplicationInformation.ED1.SetData(txt_ED1.Text)
        FAMISApplicationInformation.ED2.SetData(txt_ED2.Text)
        FAMISApplicationInformation.EE.SetData(txt_EE.Text)
        FAMISApplicationInformation.EF.SetData(txt_EF.Text)
        FAMISApplicationInformation.EG.SetData(txt_EG.Text)
        FAMISApplicationInformation.EH.SetData(txt_EH.Text)
        FAMISApplicationInformation.EI.SetData(txt_EI.Text)
        FAMISApplicationInformation.EJ.SetData(txt_EJ.Text)
        FAMISApplicationInformation.EK.SetData(txt_EK.Text)
        FAMISApplicationInformation.EL.SetData(txt_EL.Text)
        FAMISApplicationInformation.EM.SetData(txt_EM.Text)
        FAMISApplicationInformation.EN.SetData(txt_EN.Text)

        FAMISIndividualsInformation.FA.SetData(txt_FA.Text)
        FAMISIndividualsInformation.FB.SetData(txt_FB.Text)
        FAMISIndividualsInformation.FC.SetData(txt_FC.Text)
        FAMISIndividualsInformation.FD.SetData(txt_FD.Text)
        'FAMISIndividualsInformation.FD2.SetData(txt_FD2.Text)      '--Temp until complete removal--
        FAMISIndividualsInformation.FE1.SetData(txt_FE1.Text)
        FAMISIndividualsInformation.FE2.SetData(txt_FE2.Text)
        FAMISIndividualsInformation.FF.SetData(txt_FF.Text)
        FAMISIndividualsInformation.FG.SetData(txt_FG1.Text)
        FAMISIndividualsInformation.FH.SetData(txt_FH.Text)
        FAMISIndividualsInformation.FI.SetData(txt_FI.Text)
        FAMISIndividualsInformation.FJ.SetData(txt_FJ.Text)
        FAMISIndividualsInformation.FK.SetData(txt_FK.Text)
        FAMISIndividualsInformation.FL.SetData(txt_FL.Text)
        'FAMISIndividualsInformation.FL2.SetData(txt_FL2.Text)      '--Temp until complete removal--
        FAMISIndividualsInformation.FM1.SetData(txt_FM1.Text)
        FAMISIndividualsInformation.FM2.SetData(txt_FM2.Text)
        FAMISIndividualsInformation.FN.SetData(txt_FN.Text)
        FAMISIndividualsInformation.FO.SetData(txt_FO.Text)
        FAMISIndividualsInformation.FP.SetData(txt_FP1.Text)

        FAMISIndividualsInformation.GA.SetData(txt_GA.Text)
        FAMISIndividualsInformation.GB.SetData(txt_GB.Text)
        FAMISIndividualsInformation.GC.SetData(txt_GC.Text)
        FAMISIndividualsInformation.GD.SetData(txt_GD.Text)
        FAMISIndividualsInformation.GE.SetData(txt_GE.Text)
        FAMISIndividualsInformation.GF.SetData(txt_GF.Text)
        FAMISIndividualsInformation.GG.SetData(txt_GG.Text)
        FAMISIndividualsInformation.GH.SetData(txt_GH.Text)
        FAMISIndividualsInformation.GI.SetData(txt_GI.Text)
        FAMISIndividualsInformation.GJ.SetData(txt_GJ.Text)
        FAMISIndividualsInformation.GK.SetData(txt_GK.Text)
        FAMISIndividualsInformation.GL.SetData(txt_GL.Text)

        FAMISMedicaidInformation.HA.SetData(txt_HA.Text)
        FAMISMedicaidInformation.HB.SetData(txt_HB.Text)
        FAMISMedicaidInformation.HC.SetData(txt_HC.Text)
        FAMISMedicaidInformation.HD.SetData(txt_HD.Text)
        FAMISMedicaidInformation.HE.SetData(txt_HE.Text)
        FAMISMedicaidInformation.HF.SetData(txt_HF.Text)
        FAMISMedicaidInformation.HG.SetData(txt_HG.Text)
        FAMISMedicaidInformation.HH.SetData(txt_HH.Text)
        FAMISMedicaidInformation.HI.SetData(txt_HI.Text)
        FAMISMedicaidInformation.HJ.SetData(txt_HJ.Text)
        FAMISMedicaidInformation.HK.SetData(txt_HK.Text)
        FAMISMedicaidInformation.HL.SetData(txt_HL.Text)
        FAMISMedicaidInformation.HM.SetData(txt_HM.Text)
        FAMISMedicaidInformation.HN.SetData(txt_HN.Text)
        FAMISMedicaidInformation.HO.SetData(txt_HO.Text)
        FAMISMedicaidInformation.HP.SetData(txt_HP.Text)
        FAMISMedicaidInformation.HQ.SetData(txt_HQ.Text)
        FAMISMedicaidInformation.HR.SetData(txt_HR.Text)
        FAMISMedicaidInformation.HS.SetData(txt_HS.Text)
        FAMISMedicaidInformation.HT.SetData(txt_HT.Text)

        FAMISTANFInformation.IA.SetData(txt_IA.Text)
        FAMISTANFInformation.IB.SetData(txt_IB.Text)
        FAMISTANFInformation.IC.SetData(txt_IC.Text)
        FAMISTANFInformation.ID.SetData(txt_ID.Text)
        FAMISTANFInformation.IE.SetData(txt_IE.Text)
        FAMISTANFInformation.IF1.SetData(txt_IF.Text)
        FAMISTANFInformation.IG.SetData(txt_IG.Text)
        FAMISTANFInformation.IH.SetData(txt_IH.Text)
        FAMISTANFInformation.II.SetData(txt_II.Text)
        FAMISTANFInformation.IJ.SetData(txt_IJ.Text)
        FAMISTANFInformation.IK.SetData(txt_IK.Text)
        FAMISTANFInformation.IL.SetData(txt_IL.Text)
        FAMISTANFInformation.IM.SetData(txt_IM.Text)
        FAMISTANFInformation.IN1.SetData(txt_IN.Text)
        FAMISTANFInformation.IO.SetData(txt_IO.Text)

        FAMISIncomeInformation.JA.SetData(txt_JA.Text)
        FAMISIncomeInformation.JB.SetData(txt_JB.Text)
        FAMISIncomeInformation.JC.SetData(txt_JC.Text)
        FAMISIncomeInformation.JD.SetData(txt_JD.Text)
        FAMISIncomeInformation.JE.SetData(txt_JE.Text)
        FAMISIncomeInformation.JF.SetData(txt_JF.Text)
        FAMISIncomeInformation.JG.SetData(txt_JG.Text)
        FAMISIncomeInformation.JH.SetData(txt_JH.Text)
        FAMISIncomeInformation.JI.SetData(txt_JI.Text)
        FAMISIncomeInformation.JJ.SetData(txt_JJ.Text)
        FAMISIncomeInformation.JK.SetData(txt_JK.Text)
        FAMISIncomeInformation.JL.SetData(txt_JL.Text)
        FAMISIncomeInformation.JM.SetData(txt_JM.Text)
        FAMISIncomeInformation.JN.SetData(txt_JN.Text)
        FAMISIncomeInformation.JO.SetData(txt_JO.Text)
        FAMISIncomeInformation.JP.SetData(txt_JP.Text)
        FAMISIncomeInformation.JQ.SetData(txt_JQ.Text)
        FAMISIncomeInformation.JR.SetData(txt_JR.Text)
        FAMISIncomeInformation.JS.SetData(txt_JS.Text)
        FAMISIncomeInformation.JT.SetData(txt_JT.Text)
        FAMISIncomeInformation.JU.SetData(txt_JU.Text)

        FAMISIncomeInformation.KA.SetData(txt_KA.Text)
        FAMISIncomeInformation.KB.SetData(txt_KB.Text)
        FAMISIncomeInformation.KC.SetData(txt_KC.Text)
        FAMISIncomeInformation.KD.SetData(txt_KD.Text)
        FAMISIncomeInformation.KE.SetData(txt_KE.Text)
        FAMISIncomeInformation.KF.SetData(txt_KF.Text)
        FAMISIncomeInformation.KG.SetData(txt_KG.Text)
        FAMISIncomeInformation.KH.SetData(txt_KH.Text)
        FAMISIncomeInformation.KI.SetData(txt_KI.Text)
        FAMISIncomeInformation.KJ.SetData(txt_KJ.Text)
        FAMISIncomeInformation.KK.SetData(txt_KK.Text)
        FAMISIncomeInformation.KL.SetData(txt_KL.Text)
        FAMISIncomeInformation.KM.SetData(txt_KM.Text)
        FAMISIncomeInformation.KN.SetData(txt_KN.Text)
        FAMISIncomeInformation.KO.SetData(txt_KO.Text)
        FAMISIncomeInformation.KP.SetData(txt_KP.Text)
        FAMISIncomeInformation.KQ.SetData(txt_KQ.Text)
        FAMISIncomeInformation.KR.SetData(txt_KR.Text)
        FAMISIncomeInformation.KS.SetData(txt_KS.Text)

        FAMISFoodStampInformation.LA.SetData(txt_LA.Text)
        FAMISFoodStampInformation.LB.SetData(txt_LB.Text)
        FAMISFoodStampInformation.LC.SetData(txt_LC.Text)
        FAMISFoodStampInformation.LD.SetData(txt_LD.Text)
        FAMISFoodStampInformation.LE.SetData(txt_LE.Text)
        FAMISFoodStampInformation.LF.SetData(txt_LF.Text)
        FAMISFoodStampInformation.LG.SetData(txt_LG.Text)
        FAMISFoodStampInformation.LH.SetData(txt_LH.Text)
        FAMISFoodStampInformation.LI.SetData(txt_LI.Text)
        FAMISFoodStampInformation.LJ.SetData(txt_LJ.Text)
        FAMISFoodStampInformation.LK.SetData(txt_LK.Text)
        FAMISFoodStampInformation.LL.SetData(txt_LL.Text)
        FAMISFoodStampInformation.LM.SetData(txt_LM.Text)
        FAMISFoodStampInformation.LN.SetData(txt_LN.Text)
        FAMISFoodStampInformation.LO.SetData(txt_LO.Text)
        FAMISFoodStampInformation.LP.SetData(txt_LP.Text)
        FAMISFoodStampInformation.LQ.SetData(txt_LQ.Text)
        FAMISFoodStampInformation.LR.SetData(txt_LR.Text)
        FAMISFoodStampInformation.LS.SetData(txt_LS.Text)
        FAMISFoodStampInformation.LT.SetData(txt_LT.Text)

        FAMISFoodStampInformation.MA.SetData(txt_MA.Text)
        FAMISFoodStampInformation.MB.SetData(txt_MB.Text)
        FAMISFoodStampInformation.MC.SetData(txt_MC.Text)
        FAMISFoodStampInformation.MD.SetData(txt_MD.Text)
        FAMISFoodStampInformation.ME1.SetData(txt_ME.Text)
        FAMISFoodStampInformation.MF.SetData(txt_MF.Text)
        FAMISFoodStampInformation.MG.SetData(txt_MG.Text)
        FAMISFoodStampInformation.MH.SetData(txt_MH.Text)
        FAMISFoodStampInformation.MI.SetData(txt_MI.Text)
        FAMISFoodStampInformation.MJ.SetData(txt_MJ.Text)
        FAMISFoodStampInformation.MK.SetData(txt_MK.Text)
        FAMISFoodStampInformation.ML.SetData(txt_ML.Text)
        FAMISFoodStampInformation.MM.SetData(txt_MM.Text)
        FAMISFoodStampInformation.MN.SetData(txt_MN.Text)
        FAMISFoodStampInformation.MO.SetData(txt_MO.Text)
        FAMISFoodStampInformation.MP.SetData(txt_MP.Text)
        FAMISFoodStampInformation.MQ.SetData(txt_MQ.Text)
        FAMISFoodStampInformation.MR.SetData(txt_MR.Text)

        FAMISFoodStampInformation.NA.SetData(txt_NA.Text)
        FAMISFoodStampInformation.NB.SetData(txt_NB.Text)
        FAMISFoodStampInformation.NC.SetData(txt_NC.Text)
        FAMISFoodStampInformation.ND.SetData(txt_ND.Text)
        FAMISFoodStampInformation.NE.SetData(txt_NE.Text)
        FAMISFoodStampInformation.NF.SetData(txt_NF.Text)
        FAMISFoodStampInformation.NG.SetData(txt_NG.Text)
        FAMISFoodStampInformation.NH.SetData(txt_NH.Text)
        FAMISFoodStampInformation.NI.SetData(txt_NI.Text)
        FAMISFoodStampInformation.NJ.SetData(txt_NJ.Text)
        FAMISFoodStampInformation.NK.SetData(txt_NK.Text)
        FAMISFoodStampInformation.NL.SetData(txt_NL.Text)
        FAMISFoodStampInformation.NM.SetData(txt_NM.Text)
        FAMISFoodStampInformation.NN.SetData(txt_NN.Text)
        FAMISFoodStampInformation.NO.SetData(txt_NO.Text)
        FAMISFoodStampInformation.NP.SetData(txt_NP.Text)

        FAMISFoodStampInformation.OA.SetData(txt_OA.Text)
        FAMISFoodStampInformation.OB.SetData(txt_OB.Text)
        FAMISFoodStampInformation.OC.SetData(txt_OC.Text)
        FAMISFoodStampInformation.OD.SetData(txt_OD.Text)
        FAMISFoodStampInformation.OE.SetData(txt_OE.Text)
        FAMISFoodStampInformation.OF1.SetData(txt_OF.Text)
        FAMISFoodStampInformation.OG.SetData(txt_OG.Text)
        FAMISFoodStampInformation.OJ.SetData(txt_OJ.Text)
        FAMISFoodStampInformation.OK1.SetData(txt_OK1.Text)
        FAMISFoodStampInformation.OK.SetData(txt_OK.Text)
        FAMISFoodStampInformation.OL.SetData(txt_OL.Text)
        FAMISFoodStampInformation.OM.SetData(txt_OM.Text)
        FAMISFoodStampInformation.ON1.SetData(txt_ON.Text)
        FAMISFoodStampInformation.OO.SetData(txt_OO.Text)

        FAMISIandAInformation.PA.SetData(txt_PA.Text)
        FAMISIandAInformation.PB.SetData(txt_PB.Text)
        FAMISIandAInformation.PC.SetData(txt_PC.Text)
        FAMISIandAInformation.PD.SetData(txt_PD.Text)
        FAMISIandAInformation.PE.SetData(txt_PE.Text)
        FAMISIandAInformation.PF.SetData(txt_PF.Text)
        FAMISIandAInformation.PG.SetData(txt_PG.Text)
        FAMISIandAInformation.PH.SetData(txt_PH.Text)
        FAMISIandAInformation.PI.SetData(txt_PI.Text)
        FAMISIandAInformation.PJ.SetData(txt_PJ.Text)
        FAMISIandAInformation.PK.SetData(txt_PK.Text)
        FAMISIandAInformation.PL.SetData(txt_PL.Text)
        FAMISIandAInformation.PM.SetData(txt_PM.Text)
        FAMISIandAInformation.PN.SetData(txt_PN.Text)
        FAMISIandAInformation.PO.SetData(txt_PO.Text)
        FAMISIandAInformation.PP.SetData(txt_PP.Text)
    End Sub
    Private Sub Transfer105A1()
        FAMISApplicationInformation.BS.SetData(txt_BS.Text)
        FAMISApplicationInformation.BT.SetData(txt_BT.Text)
        FAMISApplicationInformation.BU.SetData(txt_BU.Text)
        FAMISApplicationInformation.BV.SetData(txt_BV.Text)
        FAMISApplicationInformation.XA.SetData(txt_XA.Text)
        FAMISApplicationInformation.XB.SetData(txt_XB.Text)
        FAMISApplicationInformation.XC.SetData(txt_XC.Text)
        FAMISApplicationInformation.XD.SetData(txt_XD.Text)
        FAMISApplicationInformation.XE.SetData(txt_XE.Text)
        FAMISApplicationInformation.XF.SetData(txt_XF.Text)
        FAMISApplicationInformation.XM.SetData(txt_XM.Text)
        FAMISIncomeInformation.JW.SetData(txt_JW.Text)
        FAMISIncomeInformation.JX.SetData(txt_JX.Text)

        FAMISApplicationInformation.BW.SetData(txt_BW.Text)
        FAMISApplicationInformation.BX.SetData(txt_BX.Text)
        FAMISApplicationInformation.BY1.SetData(txt_BY.Text)
        FAMISApplicationInformation.BZ.SetData(txt_BZ.Text)
        FAMISApplicationInformation.XG.SetData(txt_XG.Text)
        FAMISApplicationInformation.XH.SetData(txt_XH.Text)
        FAMISApplicationInformation.XI.SetData(txt_XI.Text)
        FAMISApplicationInformation.XJ.SetData(txt_XJ.Text)
        FAMISApplicationInformation.XK.SetData(txt_XK.Text)
        FAMISApplicationInformation.XL.SetData(txt_XL.Text)
        FAMISApplicationInformation.XN.SetData(txt_XN.Text)
        FAMISIncomeInformation.KU.SetData(txt_KU.Text)
        FAMISIncomeInformation.KV.SetData(txt_KV.Text)

        FAMISFoodStampInformation.WX.SetData(txt_WX.Text)
        FAMISFoodStampInformation.WY.SetData(txt_WY.Text)

        FAMISMedicaidInformation.WL.SetData(txt_WL.Text)
        FAMISMedicaidInformation.WW.SetData(txt_WW.Text)
        FAMISMedicaidInformation.WA.SetData(txt_WA.Text)
        FAMISMedicaidInformation.WB.SetData(txt_WB.Text)
        FAMISMedicaidInformation.WC.SetData(txt_WC.Text)
        FAMISMedicaidInformation.WD.SetData(txt_WD.Text)
        FAMISMedicaidInformation.WE.SetData(txt_WE.Text)
        FAMISMedicaidInformation.WF.SetData(txt_WF.Text)
        FAMISMedicaidInformation.WG.SetData(txt_WG.Text)
        FAMISMedicaidInformation.WH.SetData(txt_WH.Text)
        FAMISMedicaidInformation.WI.SetData(txt_WI.Text)
        FAMISMedicaidInformation.WK.SetData(txt_WK.Text)
        FAMISMedicaidInformation.WM.SetData(txt_WM.Text)
        'FAMISMedicaidInformation.WN.SetData(txt_WN.Text)
        'FAMISMedicaidInformation.WO.SetData(txt_WO.Text)
        'FAMISMedicaidInformation.WP.setdata(txt_WP.Text)
        'FAMISMedicaidInformation.WQ.SetData(txt_WQ.Text)
        FAMISMedicaidInformation.WR.SetData(txt_WR.Text)
        FAMISMedicaidInformation.WS.SetData(txt_WS.Text)
        FAMISMedicaidInformation.WT.SetData(txt_WT.Text)
        FAMISMedicaidInformation.WU.SetData(txt_WU.Text)
        FAMISMedicaidInformation.WV.SetData(txt_WV.Text)
    End Sub
    Private Sub TransferFormB(ByVal PersonNumber As Integer)
        FamisCaseChild(PersonNumber).QA.SetData(txt_QA.Text)
        FamisCaseChild(PersonNumber).QB.SetData(txt_QB.Text)
        FamisCaseChild(PersonNumber).QC.SetData(txt_QC.Text)
        FamisCaseChild(PersonNumber).QD.SetData(txt_QD.Text)
        FamisCaseChild(PersonNumber).QE.SetData(txt_QE.Text)
        FamisCaseChild(PersonNumber).QF.SetData(txt_QF.Text)
        FamisCaseChild(PersonNumber).QG.SetData(txt_QG.Text)
        FamisCaseChild(PersonNumber).QH.SetData(txt_QH.Text)
        FamisCaseChild(PersonNumber).QI.SetData(txt_QI.Text)

        FamisCaseChild(PersonNumber).QJ.SetData(txt_QJ.Text)
        FamisCaseChild(PersonNumber).QK.SetData(txt_QK.Text)
        FamisCaseChild(PersonNumber).QL.SetData(txt_QL.Text)
        FamisCaseChild(PersonNumber).QM.SetData(txt_QM.Text)
        FamisCaseChild(PersonNumber).QN.SetData(txt_QN.Text)
        FamisCaseChild(PersonNumber).QO.SetData(txt_QO.Text)

        FamisCaseChild(PersonNumber).RA.SetData(txt_RA.Text)
        FamisCaseChild(PersonNumber).RB.SetData(txt_RB.Text)
        FamisCaseChild(PersonNumber).RC.SetData(txt_RC.Text)
        FamisCaseChild(PersonNumber).RD.SetData(txt_RD.Text)
        FamisCaseChild(PersonNumber).RE.SetData(txt_RE.Text)
        FamisCaseChild(PersonNumber).RF.SetData(txt_RF.Text)
        FamisCaseChild(PersonNumber).RG.SetData(txt_RG.Text)

        FamisCaseChild(PersonNumber).RH.SetData(txt_RH.Text)
        'FamisCaseChild(PersonNumber).RH2.SetData(txt_RH2.Text)      '--Temp until complete removal--
        FamisCaseChild(PersonNumber).RI.SetData(txt_RI.Text)
        FamisCaseChild(PersonNumber).RJ1.SetData(txt_RJ1.Text.Substring(0, 1))
        FamisCaseChild(PersonNumber).RJ2.SetData(txt_RJ1.Text.Substring(1, 2))
        FamisCaseChild(PersonNumber).RK.SetData(txt_RK.Text)
        FamisCaseChild(PersonNumber).RL.SetData(txt_RL.Text)
        FamisCaseChild(PersonNumber).RM.SetData(txt_RM.Text)
        FamisCaseChild(PersonNumber).RN.SetData(txt_RN.Text)
        FamisCaseChild(PersonNumber).RO.SetData(txt_RO.Text)
        FamisCaseChild(PersonNumber).RP.SetData(txt_RP.Text)
        FamisCaseChild(PersonNumber).RQ.SetData(txt_RQ.Text)
        FamisCaseChild(PersonNumber).RR.SetData(txt_RR.Text)

        FamisCaseChild(PersonNumber).SA.SetData(txt_SA.Text)
        FamisCaseChild(PersonNumber).SB.SetData(txt_SB.Text)
        FamisCaseChild(PersonNumber).SC.SetData(txt_SC.Text)
        FamisCaseChild(PersonNumber).SD.SetData(txt_SD.Text)
        FamisCaseChild(PersonNumber).SE.SetData(txt_SE.Text)
        FamisCaseChild(PersonNumber).SF.SetData(txt_SF.Text)
        FamisCaseChild(PersonNumber).SG.SetData(txt_SG.Text)
        FamisCaseChild(PersonNumber).SH.SetData(txt_SH.Text)
        FamisCaseChild(PersonNumber).SI.SetData(txt_SI.Text)
        FamisCaseChild(PersonNumber).SJ.SetData(txt_SJ.Text)
        FamisCaseChild(PersonNumber).SK.SetData(txt_SK.Text)
        FamisCaseChild(PersonNumber).SL.SetData(txt_SL.Text)
        FamisCaseChild(PersonNumber).SM.SetData(txt_SM.Text)
        FamisCaseChild(PersonNumber).SN.SetData(txt_SN.Text)
        FamisCaseChild(PersonNumber).SO.SetData(txt_SO.Text)
        FamisCaseChild(PersonNumber).SP.SetData(txt_SP.Text)
        FamisCaseChild(PersonNumber).SQ.SetData(txt_SQ.Text)
        FamisCaseChild(PersonNumber).SR.SetData(txt_SR.Text)

        FamisCaseChild(PersonNumber).TA.SetData(txt_TA.Text)
        FamisCaseChild(PersonNumber).TB.SetData(txt_TB.Text)
        FamisCaseChild(PersonNumber).TC.SetData(txt_TC.Text)
        FamisCaseChild(PersonNumber).TD.SetData(txt_TD.Text)
        FamisCaseChild(PersonNumber).TE.SetData(txt_TE.Text)
        FamisCaseChild(PersonNumber).TF.SetData(txt_TF.Text)
        FamisCaseChild(PersonNumber).TG.SetData(txt_TG.Text)
        FamisCaseChild(PersonNumber).TH.SetData(txt_TH.Text)
        FamisCaseChild(PersonNumber).TI.SetData(txt_TI.Text)
        FamisCaseChild(PersonNumber).TJ.SetData(txt_TJ.Text)
        FamisCaseChild(PersonNumber).TK.SetData(txt_TK.Text)
        FamisCaseChild(PersonNumber).TL.SetData(txt_TL.Text)
        FamisCaseChild(PersonNumber).TM.SetData(txt_TM.Text)

        FamisCaseChild(PersonNumber).SS.SetData(txt_SS.Text)
        FamisCaseChild(PersonNumber).TO1.SetData(txt_TO1.Text)
        FamisCaseChild(PersonNumber).TP.SetData(txt_TP.Text)
        FamisCaseChild(PersonNumber).TQ.SetData(txt_TQ.Text)
        FamisCaseChild(PersonNumber).TR.SetData(txt_TR.Text)
        FamisCaseChild(PersonNumber).TS.SetData(txt_TS.Text)
        FamisCaseChild(PersonNumber).UA.SetData(txt_UA.Text)
        FamisCaseChild(PersonNumber).UB.SetData(txt_UB.Text)
        FamisCaseChild(PersonNumber).UC.SetData(txt_UC.Text)
        FamisCaseChild(PersonNumber).UD.SetData(txt_UD.Text)
        FamisCaseChild(PersonNumber).UE.SetData(txt_UE.Text)
        FamisCaseChild(PersonNumber).UF.SetData(txt_UF.Text)
        FamisCaseChild(PersonNumber).UG.SetData(txt_UG.Text)
        FamisCaseChild(PersonNumber).UH.SetData(txt_UH.Text)
        FamisCaseChild(PersonNumber).UI.SetData(txt_UI.Text)
        FamisCaseChild(PersonNumber).UK.SetData(txt_UK.Text)
        FamisCaseChild(PersonNumber).UL.SetData(txt_UL.Text)
    End Sub
    Private Sub TransferFormC(ByVal PaymentNumber As Integer)
        FAMISVRPInformation(PaymentNumber).VA.SetData(txt_VA.Text)
        FAMISVRPInformation(PaymentNumber).VC.SetData(txt_VC.Text)
        FAMISFoodStampInformation.OL.SetData(txt_OL.Text)
        FAMISIandAInformation.PO.SetData(txt_PO.Text)
        FAMISVRPInformation(PaymentNumber).VE.SetData(txt_VE.Text)
        FAMISVRPInformation(PaymentNumber).VG.SetData(txt_VG.Text)
        FAMISVRPInformation(PaymentNumber).VI.SetData(txt_VI.Text)
        FAMISVRPInformation(PaymentNumber).VK.SetData(txt_VK.Text)
        FAMISVRPInformation(PaymentNumber).VM.SetData(txt_VM.Text)
        FAMISVRPInformation(PaymentNumber).VO.SetData(txt_VO.Text)
        FAMISVRPInformation(PaymentNumber).VQ.SetData(txt_VQ.Text)
    End Sub

    Private Sub cmb_Child_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmb_Child.SelectedIndexChanged
        isChildChanging = True
        If isFirstRun_Child = False Then
            TransferFormB(ChildNumber)
            ChildNumber = cmb_Child.SelectedIndex
            FillFormB(ChildNumber)
        Else
            isFirstRun_Child = False
        End If
        isChildChanging = False
    End Sub
    Private Sub cmb_VRP_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmb_VRP.SelectedIndexChanged
        If isFirstRun_VRP = False Then
            TransferFormC(VRPNumber)
            VRPNumber = cmb_VRP.SelectedIndex
            FillFormC(VRPNumber)
        Else
            isFirstRun_VRP = False
        End If
    End Sub

    Private Sub getBatchNumber(ByVal BatchType As String)
        If BatchType = "A" Then BATCHNUMBER = BatchType & My.Settings.BatchNumber_Pre & My.Settings.BatchNumber_ANum Else BATCHNUMBER = BatchType & My.Settings.BatchNumber_Pre & My.Settings.BatchNumber_UNum
    End Sub

    Private Sub GetFromDatabase()
        FAMISCaseInformationTableAdapter.Fill(PhoenixDataSet.FAMISCaseInformation, CASENUMBER) '"C112249016") 
        FAMISApplicantInformationTableAdapter.Fill(PhoenixDataSet.FAMISApplicantInformation, CASENUMBER)
        FAMISAFDCInformationTableAdapter.Fill(PhoenixDataSet.FAMISAFDCInformation, CASENUMBER)
        FAMISIandAInformationTableAdapter.Fill(PhoenixDataSet.FAMISIandAInformation, CASENUMBER)
        FAMISIndividualsInformationTableAdapter.Fill(PhoenixDataSet.FAMISIndividualsInformation, CASENUMBER)
        FAMISFoodStampInformationTableAdapter.Fill(PhoenixDataSet.FAMISFoodStampInformation, CASENUMBER)
        FAMISIncomeInformationTableAdapter.Fill(PhoenixDataSet.FAMISIncomeInformation, CASENUMBER)
        FAMISMedicaidInformationTableAdapter.Fill(PhoenixDataSet.FAMISMedicaidInformation, CASENUMBER)
    End Sub

    'Private Sub btn_ABatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ABatch.Click
    '    ParentForm_Put105.LineNP = "ABatch"
    '    Me.Close()
    'End Sub
    'Private Sub btn_UBatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_UBatch.Click
    '    ParentForm_Put105.LineNP = "UBatch"
    '    Me.Close()
    'End Sub
    'Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click
    '    ParentForm_Put105.LineNP = "Cancel"
    '    Me.Close()
    'End Sub
End Class
