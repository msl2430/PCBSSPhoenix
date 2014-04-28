Public Class DeleteBatch

    Private glapiTP8 As connGLinkTP8
    Private BatchNumber As String
    Private Message As String
    Private CaseNumber As String
    Private isDeleting As Boolean

    Private SQLConn, SQLDHConn As SqlClient.SqlConnection '--SQL connection--
    Private SQLCommand, SQLDHCommand As SqlClient.SqlCommand       '--SQL command string--
    Private SQLReader As SqlClient.SqlDataReader     '--SQL data reader--

    Private Sub DeleteBatch_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        btn_Submit.Enabled = False
    End Sub
    Private Sub DeleteBatch_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If isDeleting = True Then e.Cancel = True Else e.Cancel = False
    End Sub

    Private Sub GLink_Start()
        Dim RetryCounter As Integer = 0
        Dim isError As Boolean = False
        Dim isLogonError As Boolean = False
        Dim isPasswordError As Boolean = False
        glapiTP8 = New connGLinkTP8(My.Settings.GLinkDirectory & "BullProd.cfg")
        While RetryCounter < 3
            glapiTP8.bool_Visible = My.Settings.GLinkVisible
            glapiTP8.Connect()
            glapiTP8.SendKeysTransmit("HSA")
            glapiTP8.SendKeysTransmit("LOGON")
            glapiTP8.SubmitField(4, My.Settings.FAMISOperatorID)
            glapiTP8.SubmitField(6, My.Settings.FAMISPassword)
            glapiTP8.SubmitField(8, My.Settings.FAMISKeyword)
            glapiTP8.TransmitPage()
            Message = glapiTP8.GetString(10, 22, 40, 22)
            If glapiTP8.GetString(30, 4, 37, 4) = "PASSWORD" Then
                glapiTP8.SetVisible(True)
                isPasswordError = True
                '--WAIT UNTIL USER HITS CONTINUE--
            ElseIf Message.Substring(0, 5) = "     " Then
                RetryCounter += 1
                isError = True
                isLogonError = False
                Message = "Unknown GLink error. Please restart."
            ElseIf Message.Substring(0, 5) <> "LOGON" Then
                RetryCounter += 1
                isError = True
                isLogonError = True
            Else
                RetryCounter = 3
                isError = False
                isLogonError = False
            End If
            If isPasswordError = True Then
                isPasswordError = False
                isError = False
                isLogonError = False
                bgw_DeleteBatch.ReportProgress(5)
            ElseIf isError = True And isLogonError = False Then
                If RetryCounter < 3 Then
                    bgw_DeleteBatch.ReportProgress(6)
                    bgw_DeleteBatch.ReportProgress(6)
                Else
                    bgw_DeleteBatch.ReportProgress(7)
                    bgw_DeleteBatch.CancelAsync()
                End If
            ElseIf isError = True And isLogonError = True Then
                RetryCounter = 3
                bgw_DeleteBatch.ReportProgress(7)
                bgw_DeleteBatch.CancelAsync()
            End If
            glapiTP8.TransmitPage()
            Thread.Sleep(500)
        End While
    End Sub
    Private Sub DeleteBatch()
        glapiTP8.SendKeysTransmit("BTCH,DELE," & BatchNumber)
        If glapiTP8.GetString(1, 1, 12, 1) = "SCRN ( BTCH)" Then
            bgw_DeleteBatch.ReportProgress(10)
            ResetSQLData()
        ElseIf glapiTP8.GetString(3, 2, 28, 2) = "BATCH WAS DELETED FROM OTF" Then
            bgw_DeleteBatch.ReportProgress(11)
        ElseIf glapiTP8.GetString(3, 2, 24, 2) = "BATCH NOT FOUND ON OTF" Then
            bgw_DeleteBatch.ReportProgress(12)
        Else
            bgw_DeleteBatch.ReportProgress(4)
        End If
    End Sub
    Private Sub ResetSQLData()
        SQLConn = New SqlConnection(My.Settings.phxSQLConn) '"Data Source=" & My.Settings.SQLAddress & "\SQLEXPRESS;Initial Catalog=Phoenix;Integrated Security=True") ';Persist Security Info=True;User ID=FAMISUser;Password=password")
        SQLDHConn = New SqlConnection(My.Settings.phxSQLConn) '"Data Source=" & My.Settings.SQLAddress & "\SQLEXPRESS;Initial Catalog=Phoenix;Integrated Security=True") ';Persist Security Info=True;User ID=FAMISUser;Password=password")
        SQLCommand = New SqlCommand()
        SQLCommand.Connection = SQLConn
        SQLDHCommand = New SqlCommand
        SQLDHCommand.Connection = SQLDHConn
        Try
            SQLConn.Open()
            SQLDHConn.Open()

            SQLCommand.CommandText() = "SELECT CASENUMBER FROM DAILYHOLD_CaseInformation WHERE BATCHNUMBER = '" + BatchNumber + "'"
            SQLReader = SQLCommand.ExecuteReader
            SQLReader.Read()
            If SQLReader.HasRows Then
                CaseNumber = SQLReader.GetString(0)
                SQLReader.Close()
                SQLCommand.CommandText() = "DELETE FROM FAMISCASECHILD WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISCASEINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISAFDCINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISAPPLICANTINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISFOODSTAMPINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISIANDAINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISINCOMEINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISINDIVIDUALSINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISMEDICAIDINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISVRPINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLDHCommand.CommandText() = "SELECT CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW FROM DAILYHOLD_CaseInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText() = "INSERT INTO FAMISCaseInformation (CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetDateTime(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetDateTime(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "')" '"INSERT INTO FAMISCaseInformation (CASENUMBER, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, DATEENTERED, P03, P05, OPERATOR, P09, BATCHNUMBER, P10, WW) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetDateTime(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, HA, IA, IB, IC, ID, IE, IF1, IG, IH, II, IJ, IL, IK, IP, OJ, IM, IN1, IO FROM DAILYHOLD_AFDCInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISAFDCInformation (CASENUMBER, HA, IA, IB, IC, ID, IE, IF1, IG, IH, II, IJ, IL, IK, IP, OJ, IM, IN1, IO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetDateTime(4) + "', '" + SQLReader.GetDateTime(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetDateTime(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "',  '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "')" '"INSERT INTO FAMISAFDCInformation (CASENUMBER, HA, IA, IB, IC, ID, IE, IF1, IG, IH, II, IJ, IL, IK, IP, OJ, IM, IN1, IO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "',  '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, CA, CB, CC, CD1, CD2, CE, CF, CG, DA1, DA2, DA3, DB, DC, DD1, DD2, DE, DF FROM DAILYHOLD_ApplicantInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISApplicantInformation (CASENUMBER, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, CA, CB, CC, CD1, CD2, CE, CF, CG, DA1, DA2, DA3, DB, DC, DD1, DD2, DE, DF) VALUES  ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "',  '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "')" '"INSERT INTO FAMISApplicantInformation (CASENUMBER, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, CA, CB, CC, CD1, CD2, CE, CF, CG, DA1, DA2, DA3, DB, DC, DD1, DD2, DE, DF) VALUES  ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "',  '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT EA, EB, EC, ED1, ED2, EE, EF, EG, EH, EJ, NN, BR, OM, EI, EK, EL, EM, EN, XA, XB, XC, XD, XE, XF, XG, XH, XI, XJ, XK, XL, XM, XN FROM DAILYHOLD_ApplicantInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "UPDATE FAMISApplicantInformation SET EA = '" + SQLReader.GetString(0) + "', EB = '" + SQLReader.GetString(1) + "', EC = '" + SQLReader.GetString(2) + "', ED1 = '" + SQLReader.GetString(3) + "', ED2 = '" + SQLReader.GetString(4) + "', EE = '" + SQLReader.GetString(5) + "', EF = '" + SQLReader.GetString(6) + "', EG = '" + SQLReader.GetString(7) + "', EH = '" + SQLReader.GetString(8) + "', EJ = '" + SQLReader.GetString(9) + "', NN = '" + SQLReader.GetString(10) + "', BR = '" + SQLReader.GetString(11) + "', OM = '" + SQLReader.GetString(12) + "', EI = '" + SQLReader.GetString(13) + "', EK = '" + SQLReader.GetString(14) + "', EL = '" + SQLReader.GetString(15) + "', EM = '" + SQLReader.GetString(16) + "', EN = '" + SQLReader.GetString(17) + "', XA = '" + SQLReader.GetDateTime(18) + "', XB = '" + SQLReader.GetString(19) + "', XC = '" + SQLReader.GetString(20) + "', XD = '" + SQLReader.GetString(21) + "', XE = '" + SQLReader.GetString(22) + "', XF = '" + SQLReader.GetString(23) + "',  XG = '" + SQLReader.GetString(24) + "', XH = '" + SQLReader.GetString(25) + "', XI = '" + SQLReader.GetString(26) + "', XJ = '" + SQLReader.GetString(27) + "', XK = '" + SQLReader.GetString(28) + "', XL = '" + SQLReader.GetString(29) + "', XM = '" + SQLReader.GetString(30) + "', XN = '" + SQLReader.GetString(31) + "' WHERE CASENUMBER = '" + CaseNumber + "'" '"UPDATE FAMISApplicantInformation SET EA = '" + SQLReader.GetString(0) + "', EB = '" + SQLReader.GetString(1) + "', EC = '" + SQLReader.GetString(2) + "', ED1 = '" + SQLReader.GetString(3) + "', ED2 = '" + SQLReader.GetString(4) + "', EE = '" + SQLReader.GetString(5) + "', EF = '" + SQLReader.GetString(6) + "', EG = '" + SQLReader.GetString(7) + "', EH = '" + SQLReader.GetString(8) + "', EJ = '" + SQLReader.GetString(9) + "', NN = '" + SQLReader.GetString(10) + "', BR = '" + SQLReader.GetString(11) + "', OM = '" + SQLReader.GetString(12) + "', EI = '" + SQLReader.GetString(13) + "', EK = '" + SQLReader.GetString(14) + "', EL = '" + SQLReader.GetString(15) + "', EM = '" + SQLReader.GetString(16) + "', EN = '" + SQLReader.GetString(17) + "', XA = '" + SQLReader.GetString(18) + "', XB = '" + SQLReader.GetString(19) + "', XC = '" + SQLReader.GetString(20) + "', XD = '" + SQLReader.GetString(21) + "', XE = '" + SQLReader.GetString(22) + "', XF = '" + SQLReader.GetString(23) + "',  XG = '" + SQLReader.GetString(24) + "', XH = '" + SQLReader.GetString(25) + "', XI = '" + SQLReader.GetString(26) + "', XJ = '" + SQLReader.GetString(27) + "', XK = '" + SQLReader.GetString(28) + "', XL = '" + SQLReader.GetString(29) + "', XM = '" + SQLReader.GetString(30) + "', XN = '" + SQLReader.GetString(31) + "' WHERE CASENUMBER = '" + CaseNumber + "'"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, BS, BT, BX, FA, FB, BU, FC, BV, FD, FD2, BW, FE2, FE1, BY1, FF, BZ, FG, FI, FJ, FK, FL, FL2, FM1, FM2, FN, FP, GA, GB, GC, GD, GE, GF, GG, GH, GI, GJ, GK, GL, FH, FO FROM DAILYHOLD_IndividualsInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISIndividualsInformation (CASENUMBER, BS, BT, BX, FA, FB, BU, FC, BV, FD, FD2, BW, FE2, FE1, BY1, FF, BZ, FG, FI, FJ, FK, FL, FL2, FM1, FM2, FN, FP, GA, GB, GC, GD, GE, GF, GG, GH, GI, GJ, GK, GL, FH, FO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetDateTime(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetDateTime(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "',  '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetDateTime(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetDateTime(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetDateTime(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetDateTime(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "')" '"INSERT INTO FAMISIndividualsInformation (CASENUMBER, BS, BT, BX, FA, FB, BU, FC, BV, FD, FD2, BW, FE2, FE1, BY1, FF, BZ, FG, FI, FJ, FK, FL, FL2, FM1, FM2, FN, FP, GA, GB, GC, GD, GE, GF, GG, GH, GI, GJ, GK, GL, FH, FO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "',  '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, JP, JQ, JS, JT, JW, JX, KU, KV, JA, JB, JC, JD, JE, JF, JG, JH, JI, JK, JL, JM, JN, JO, JR, JU, KA, KB, KC, KD, KE, KF, KG, KH, KI, KJ, KK, KL, KM, KN, KO, KP, KQ, KR, KS, JJ FROM DAILYHOLD_IncomeInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISIncomeInformation (CASENUMBER, JP, JQ, JS, JT, JW, JX, KU, KV, JA, JB, JC, JD, JE, JF, JG, JH, JI, JK, JL, JM, JN, JO, JR, JU, KA, KB, KC, KD, KE, KF, KG, KH, KI, KJ, KK, KL, KM, KN, KO, KP, KQ, KR, KS, JJ) VALUES ('" + CaseNumber + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "', '" + SQLReader.GetString(41) + "', '" + SQLReader.GetString(42) + "', '" + SQLReader.GetString(43) + "', '" + SQLReader.GetString(44) + "')" '"INSERT INTO FAMISIncomeInformation (CASENUMBER, JP, JQ, JS, JT, JW, JX, KU, KV, JA, JB, JC, JD, JE, JF, JG, JH, JI, JK, JL, JM, JN, JO, JR, JU, KA, KB, KC, KD, KE, KF, KG, KH, KI, KJ, KK, KL, KM, KN, KO, KP, KQ, KR, KS, JJ) VALUES ('" + CaseNumber + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "', '" + SQLReader.GetString(41) + "', '" + SQLReader.GetString(42) + "', '" + SQLReader.GetString(43) + "', '" + SQLReader.GetString(44) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, HD, HE, HF, HG, HH, HI, HJ, HK, HL, HM, HN, HO, HP, HQ, HR, HS, HT, HB, HC, WL, WA, WB, WE, WH, WI, WK, WM, WN, WO, WP, WQ, WR, WS, WT, WU, WV, WC, WD, WF, WG FROM FAMISMedicaidInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISMedicaidInformation (CASENUMBER, HD, HE, HF, HG, HH, HI, HJ, HK, HL, HM, HN, HO, HP, HQ, HR, HS, HT, HB, HC, WL, WA, WB, WE, WH, WI, WK, WM, WN, WO, WP, WQ, WR, WS, WT, WU, WV, WC, WD, WF, WG) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetDateTime(37) + "', '" + SQLReader.GetDateTime(38) + "', '" + SQLReader.GetDateTime(39) + "', '" + SQLReader.GetDateTime(40) + "')" ', '" + FAMISApplicationInformation.XQ.getdata() + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, LA, LB, LC, LD, LE, LF, LG, LH, LI, LJ, LK, LL, LM, LO, LP, LQ, LR, LT, MD, OA, OH, OI, ON1, OO, OK, WX, WY, LS FROM DAILYHOLD_FoodStampInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISFoodStampInformation (CASENUMBER, LA, LB, LC, LD, LE, LF, LG, LH, LI, LJ, LK, LL, LM, LO, LP, LQ, LR, LT, MD, OA, OH, OI, ON1, OO, OK, WX, WY, LS) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetDateTime(3) + "', '" + SQLReader.GetDateTime(4) + "', '" + SQLReader.GetDateTime(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetDateTime(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "')" '"INSERT INTO FAMISFoodStampInformation (CASENUMBER, LA, LB, LC, LD, LE, LF, LG, LH, LI, LJ, LK, LL, LM, LO, LP, LQ, LR, LT, MD, OA, OH, OI, ON1, OO, OK, WX, WY, LS) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT MA, MB, MC, ME1, MF, MG, MH, MI, MJ, MK, ML, MM, MN, MO, MP, MQ, MR, NB, OB, OC, OD, OE, OF1, OG, OL, NA, LN FROM DAILYHOLD_FoodStampInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "UPDATE FAMISFoodStampInformation SET MA = '" + SQLReader.GetString(0) + "', MB = '" + SQLReader.GetString(1) + "', MC = '" + SQLReader.GetString(2) + "',  ME1 = '" + SQLReader.GetString(3) + "', MF = '" + SQLReader.GetString(4) + "', MG = '" + SQLReader.GetString(5) + "', MH = '" + SQLReader.GetString(6) + "', MI = '" + SQLReader.GetString(7) + "', MJ = '" + SQLReader.GetString(8) + "', MK = '" + SQLReader.GetString(9) + "', ML = '" + SQLReader.GetString(10) + "', MM = '" + SQLReader.GetString(11) + "', MN = '" + SQLReader.GetString(12) + "', MO = '" + SQLReader.GetString(13) + "', MP = '" + SQLReader.GetString(14) + "', MQ = '" + SQLReader.GetString(15) + "', MR = '" + SQLReader.GetString(16) + "', NB = '" + SQLReader.GetString(17) + "', OB = '" + SQLReader.GetString(18) + "', OC = '" + SQLReader.GetString(19) + "', OD = '" + SQLReader.GetString(20) + "', OE = '" + SQLReader.GetString(21) + "', OF1 = '" + SQLReader.GetString(22) + "', OG = '" + SQLReader.GetString(23) + "', OL = '" + SQLReader.GetString(24) + "', NA = '" + SQLReader.GetString(25) + "', LN = '" + SQLReader.GetString(26) + "' WHERE CASENUMBER = '" + CaseNumber + "'" '"UPDATE FAMISFoodStampInformation SET MA = '" + SQLReader.GetString(0) + "', MB = '" + SQLReader.GetString(1) + "', MC = '" + SQLReader.GetString(2) + "',  ME1 = '" + SQLReader.GetString(3) + "', MF = '" + SQLReader.GetString(4) + "', MG = '" + SQLReader.GetString(5) + "', MH = '" + SQLReader.GetString(6) + "', MI = '" + SQLReader.GetString(7) + "', MJ = '" + SQLReader.GetString(8) + "', MK = '" + SQLReader.GetString(9) + "', ML = '" + SQLReader.GetString(10) + "', MM = '" + SQLReader.GetString(11) + "', MN = '" + SQLReader.GetString(12) + "', MO = '" + SQLReader.GetString(13) + "', MP = '" + SQLReader.GetString(14) + "', MQ = '" + SQLReader.GetString(15) + "', MR = '" + SQLReader.GetString(15) + "', NB = '" + SQLReader.GetString(16) + "', OB = '" + SQLReader.GetString(17) + "', OC = '" + SQLReader.GetString(18) + "', OD = '" + SQLReader.GetString(19) + "', OE = '" + SQLReader.GetString(20) + "', OF1 = '" + SQLReader.GetString(21) + "', OG = '" + SQLReader.GetString(22) + "', OL = '" + SQLReader.GetString(23) + "', NA = '" + SQLReader.GetString(24) + "', LN = '" + SQLReader.GetString(25) + "' WHERE CASENUMBER = '" + CaseNumber + "'"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT NB, NC, NE, NF, NG, NH, NI, NJ, NK, NL, NM, NO, NP, ND FROM DAILYHOLD_FoodStampInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "UPDATE FAMISFoodStampInformation SET NB = '" + SQLReader.GetString(0) + "', NC = '" + SQLReader.GetDateTime(1) + "', NE = '" + SQLReader.GetString(2) + "',  NF = '" + SQLReader.GetString(3) + "', NG = '" + SQLReader.GetDateTime(4) + "', NH = '" + SQLReader.GetString(5) + "', NI = '" + SQLReader.GetString(6) + "', NJ = '" + SQLReader.GetString(7) + "', NK = '" + SQLReader.GetString(8) + "', NL = '" + SQLReader.GetString(9) + "', NM = '" + SQLReader.GetString(10) + "', NO = '" + SQLReader.GetString(11) + "', NP = '" + SQLReader.GetString(12) + "', ND = '" + SQLReader.GetString(13) + "' WHERE CASENUMBER = '" + CaseNumber + "'" '"UPDATE FAMISFoodStampInformation SET NB = '" + SQLReader.GetString(0) + "', NC = '" + SQLReader.GetString(1) + "', NE = '" + SQLReader.GetString(2) + "',  NF = '" + SQLReader.GetString(3) + "', NG = '" + SQLReader.GetString(4) + "', NH = '" + SQLReader.GetString(5) + "', NI = '" + SQLReader.GetString(6) + "', NJ = '" + SQLReader.GetString(7) + "', NK = '" + SQLReader.GetString(8) + "', NL = '" + SQLReader.GetString(9) + "', NM = '" + SQLReader.GetString(10) + "', NO = '" + SQLReader.GetString(11) + "', NP = '" + SQLReader.GetString(12) + "' WHERE CASENUMBER = '" + CaseNumber + "'"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, PA, PB, PC, PD, PF, PG, PI, PJ, PK, PL, PN, PE, PH, PM, PO, PP FROM DAILYHOLD_IandAInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then
                    SQLCommand.CommandText = "INSERT INTO FAMISIandAInformation (CASENUMBER, PA, PB, PC, PD, PF, PG, PI, PJ, PK, PL, PN, PE, PH, PM, PO, PP) VALUES  ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetDateTime(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetDateTime(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "')" '"INSERT INTO FAMISIandAInformation (CASENUMBER, PA, PB, PC, PD, PF, PG, PI, PJ, PK, PL, PN, PE, PH, PM, PO, PP) VALUES  ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "')"
                    SQLCommand.ExecuteNonQuery()
                End If
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CaseNumber, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RH2, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR, TA, TB, TI, TJ, TF, TK, TD, UB, RN, RO, RQ, SA, SB, SC, SD, SE, SF, SG, SH, SJ, SK, SL, SM, SO, SP, SI, SS, TG, TH, TL, TM, TO1, TP, TQ, TR, TS, UA, UC, UD, UE, UF, UG, UH, UK, UL, TC, TE, UI FROM DAILYHOLD_CaseChild WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                While SQLReader.Read()
                    If SQLReader.HasRows Then
                        SQLCommand.CommandText = "INSERT INTO FAMISCaseChild (CaseNumber, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RH2, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR, TA, TB, TI, TJ, TF, TK, TD, UB, RN, RO, RQ, SA, SB, SC, SD, SE, SF,  SG, SH, SJ, SK, SL, SM, SO, SP, SI, SS, TG, TH, TL, TM, TO1, TP, TQ, TR, TS, UA, UC, UD, UE, UF, UG, UH, UK, UL, TC, TE, UI) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetDateTime(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetDateTime(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetDateTime(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "', '" + SQLReader.GetString(41) + "', '" + SQLReader.GetString(42) + "', '" + SQLReader.GetString(43) + "', '" + SQLReader.GetString(44) + "', '" + SQLReader.GetString(45) + "', '" + SQLReader.GetString(46) + "', '" + SQLReader.GetString(47) + "', '" + SQLReader.GetString(48) + "', '" + SQLReader.GetString(49) + "', '" + SQLReader.GetString(50) + "', '" + SQLReader.GetString(51) + "', '" + SQLReader.GetString(52) + "', '" + SQLReader.GetString(53) + "', '" + SQLReader.GetString(54) + "', '" + SQLReader.GetString(55) + "', '" + SQLReader.GetString(56) + "', '" + SQLReader.GetString(57) + "', '" + SQLReader.GetString(58) + "', '" + SQLReader.GetString(59) + "', '" + SQLReader.GetString(60) + "', '" + SQLReader.GetString(61) + "', '" + SQLReader.GetString(62) + "', '" + SQLReader.GetString(63) + "', '" + SQLReader.GetString(64) + "', '" + SQLReader.GetDateTime(65) + "', '" + SQLReader.GetString(66) + "', '" + SQLReader.GetString(67) + "', '" + SQLReader.GetString(68) + "', '" + SQLReader.GetString(69) + "', '" + SQLReader.GetString(70) + "', '" + SQLReader.GetString(71) + "', '" + SQLReader.GetString(72) + "', '" + SQLReader.GetString(73) + "', '" + SQLReader.GetString(74) + "', '" + SQLReader.GetString(75) + "', '" + SQLReader.GetString(76) + "', '" + SQLReader.GetString(77) + "', '" + SQLReader.GetString(78) + "', '" + SQLReader.GetString(79) + "', '" + SQLReader.GetString(80) + "', '" + SQLReader.GetString(81) + "', '" + SQLReader.GetDateTime(82) + "', '" + SQLReader.GetString(83) + "')" '"INSERT INTO FAMISCaseChild (CaseNumber, QA, QB, QC, QD, QE, QF, QG, QH, QI, QJ, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RH2, RI, RJ1, RJ2, RK, RL, RM, RP, RR, SN, SQ, SR, TA, TB, TI, TJ, TF, TK, TD, UB, RN, RO, RQ, SA, SB, SC, SD, SE, SF,  SG, SH, SJ, SK, SL, SM, SO, SP, SI, SS, TG, TH, TL, TM, TO1, TP, TQ, TR, TS, UA, UC, UD, UE, UF, UG, UH, UK, UL, TC, TE, UI) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "', '" + SQLReader.GetString(11) + "', '" + SQLReader.GetString(12) + "', '" + SQLReader.GetString(13) + "', '" + SQLReader.GetString(14) + "', '" + SQLReader.GetString(15) + "', '" + SQLReader.GetString(16) + "', '" + SQLReader.GetString(17) + "', '" + SQLReader.GetString(18) + "', '" + SQLReader.GetString(19) + "', '" + SQLReader.GetString(20) + "', '" + SQLReader.GetString(21) + "', '" + SQLReader.GetString(22) + "', '" + SQLReader.GetString(23) + "', '" + SQLReader.GetString(24) + "', '" + SQLReader.GetString(25) + "', '" + SQLReader.GetString(26) + "', '" + SQLReader.GetString(27) + "', '" + SQLReader.GetString(28) + "', '" + SQLReader.GetString(29) + "', '" + SQLReader.GetString(30) + "', '" + SQLReader.GetString(31) + "', '" + SQLReader.GetString(32) + "', '" + SQLReader.GetString(33) + "', '" + SQLReader.GetString(34) + "', '" + SQLReader.GetString(35) + "', '" + SQLReader.GetString(36) + "', '" + SQLReader.GetString(37) + "', '" + SQLReader.GetString(38) + "', '" + SQLReader.GetString(39) + "', '" + SQLReader.GetString(40) + "', '" + SQLReader.GetString(41) + "', '" + SQLReader.GetString(42) + "', '" + SQLReader.GetString(43) + "', '" + SQLReader.GetString(44) + "', '" + SQLReader.GetString(45) + "', '" + SQLReader.GetString(46) + "', '" + SQLReader.GetString(47) + "', '" + SQLReader.GetString(48) + "', '" + SQLReader.GetString(49) + "', '" + SQLReader.GetString(50) + "', '" + SQLReader.GetString(51) + "', '" + SQLReader.GetString(52) + "', '" + SQLReader.GetString(53) + "', '" + SQLReader.GetString(54) + "', '" + SQLReader.GetString(55) + "', '" + SQLReader.GetString(56) + "', '" + SQLReader.GetString(57) + "', '" + SQLReader.GetString(58) + "', '" + SQLReader.GetString(59) + "', '" + SQLReader.GetString(60) + "', '" + SQLReader.GetString(61) + "', '" + SQLReader.GetString(62) + "', '" + SQLReader.GetString(63) + "', '" + SQLReader.GetString(64) + "', '" + SQLReader.GetString(65) + "', '" + SQLReader.GetString(66) + "', '" + SQLReader.GetString(67) + "', '" + SQLReader.GetString(68) + "', '" + SQLReader.GetString(69) + "', '" + SQLReader.GetString(70) + "', '" + SQLReader.GetString(71) + "', '" + SQLReader.GetString(72) + "', '" + SQLReader.GetString(73) + "', '" + SQLReader.GetString(74) + "', '" + SQLReader.GetString(75) + "', '" + SQLReader.GetString(76) + "', '" + SQLReader.GetString(77) + "', '" + SQLReader.GetString(78) + "', '" + SQLReader.GetString(79) + "', '" + SQLReader.GetString(80) + "', '" + SQLReader.GetString(81) + "', '" + SQLReader.GetString(82) + "', '" + SQLReader.GetString(83) + "')"
                        SQLCommand.ExecuteNonQuery()
                    End If
                End While
                SQLReader.Close()
                SQLDHCommand.CommandText = "SELECT CASENUMBER, VRPNumber, VA, VC, VE, VG, VI, VQ, VK, VM, VO FROM DAILYHOLD_VRPInformation WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLReader = SQLDHCommand.ExecuteReader
                While SQLReader.Read()
                    If SQLReader.HasRows Then
                        SQLCommand.CommandText = "INSERT INTO FAMISVRPInformation (CASENUMBER, VRPNumber, VA, VC, VE, VG, VI, VQ, VK, VM, VO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "', '" + SQLReader.GetString(10) + "')" '"INSERT INTO FAMISVRPInformation (CASENUMBER, VA, VC, VE, VG, VI, VQ, VK, VM, VO) VALUES ('" + SQLReader.GetString(0) + "', '" + SQLReader.GetString(1) + "', '" + SQLReader.GetString(2) + "', '" + SQLReader.GetString(3) + "', '" + SQLReader.GetString(4) + "', '" + SQLReader.GetString(5) + "', '" + SQLReader.GetString(6) + "', '" + SQLReader.GetString(7) + "', '" + SQLReader.GetString(8) + "', '" + SQLReader.GetString(9) + "')"
                        SQLCommand.ExecuteNonQuery()
                    End If
                End While
                SQLReader.Close()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_CASECHILD WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_CASEINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_AFDCINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_APPLICANTINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_FOODSTAMPINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_IANDAINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_INCOMEINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_INDIVIDUALSINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_MEDICAIDINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM DAILYHOLD_VRPINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
            Else
                SQLReader.Close()
                SQLCommand.CommandText() = "SELECT CASENUMBER FROM FAMISCaseInformation WHERE BATCHNUMBER = '" + BatchNumber + "' AND DATEENTERED = '" & Date.Now.Month & "/" & Date.Now.Day & "/" & Date.Now.Year & "'"
                SQLReader = SQLCommand.ExecuteReader
                SQLReader.Read()
                If SQLReader.HasRows Then CaseNumber = SQLReader.GetString(0)
                SQLReader.Close()
                SQLCommand.CommandText() = "DELETE FROM FAMISCASECHILD WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISCASEINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISAFDCINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISAPPLICANTINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISFOODSTAMPINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISIANDAINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISINCOMEINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISINDIVIDUALSINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISMEDICAIDINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
                SQLCommand.CommandText() = "DELETE FROM FAMISVRPINFORMATION WHERE CASENUMBER = '" + CaseNumber + "'"
                SQLCommand.ExecuteNonQuery()
            End If
        Catch ex As Exception
            MessageBox.Show("Error!" & vbCrLf & "'" & ex.Message.ToString & "'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txt_Batch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_Batch.TextChanged
        If txt_Batch.Text.Length < 7 Then btn_Submit.Enabled = False Else btn_Submit.Enabled = True
    End Sub
    Private Sub btn_Submit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Submit.Click
        BatchNumber = txt_Batch.Text.ToUpper
        bgw_DeleteBatch.RunWorkerAsync()
    End Sub

    Private Sub bgw_DeleteBatch_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgw_DeleteBatch.DoWork
        isDeleting = True
        bgw_DeleteBatch.ReportProgress(1)
        bgw_DeleteBatch.ReportProgress(2)
        Thread.Sleep(500)
        If Not bgw_DeleteBatch.CancellationPending Then GLink_Start()
        Thread.Sleep(500)
        If Not bgw_DeleteBatch.CancellationPending Then DeleteBatch()
        glapiTP8.Disconnect()
        bgw_DeleteBatch.ReportProgress(3)
        isDeleting = False
    End Sub
    Private Sub bgw_DeleteBatch_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bgw_DeleteBatch.ProgressChanged
        Select Case e.ProgressPercentage
            Case 1 : lbl_Status.Text = "Deleting Batch..."
            Case 2 : btn_Submit.Enabled = False : Cursor = Cursors.WaitCursor : txt_Batch.Enabled = False
            Case 3 : btn_Submit.Enabled = True : Cursor = Cursors.Default : txt_Batch.Enabled = True
            Case 4 : lbl_Status.Text = "Invalid Control"
            Case 5 : lbl_Status.Text = "Retrying..."
            Case 6 : lbl_Status.Text = "GLink Error. Retrying..."
            Case 7 : lbl_Status.Text = Message
            Case 10 : lbl_Status.Text = "Batch Deleted"
            Case 11 : lbl_Status.Text = "Batch Already Deleted"
            Case 12 : lbl_Status.Text = "Batch Not Found"
            Case 99 : lbl_Status.Text = "SQL Error"
        End Select
    End Sub
End Class