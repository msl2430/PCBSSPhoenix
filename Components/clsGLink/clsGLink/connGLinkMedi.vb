Imports System.Threading

Public Class connGLinkMedi
    '--GLink Methods and Variables for connecting and controlling the Medicaid screens--

    '--Public Variables--
    Public WithEvents GLAPI As Glink.GlinkApi
    Public path_SessionName As String
    Public bool_Visible As Boolean  '--GLink visibility setting--
    Public bool_Cancel As Boolean

    '--Private Variables--
    Private isStarting As Boolean
    Private isConnected As Boolean

    '--Constants for handling GLink events and commands--
    Public GLAPIEvent As Integer
    Private Const TIMEOUT As Integer = 60
    Private Const START As Integer = 1
    Private Const STOPPED As Integer = 2
    Private Const CONNECTED As Integer = 3
    Private Const DISCONNECTED As Integer = 4
    Private Const TURN_RECEIVED As Integer = 5
    Private Const TURN_LOST As Integer = 6
    Private Const STRING_RECEIVED As Integer = 7
    Private Const GLINK_ERROR As Integer = 99
    Private Const FAMIS_CONTROLFIELD As Integer = 4
    Private Const BLINKING As String = "-536861665"
    Private Const RED As String = "-1073741700"
    Private Const TRANSMIT As Glink.GlinkKeyEnum = Glink.GlinkKeyEnum.GlinkKey_TRANSMIT

    Public Sub New()
        bool_Visible = False
        bool_Cancel = False
        path_SessionName = "C:\GLPro\BullProd.cfg"
        '--Look in default GLPro directory if none is given--
    End Sub
    Public Sub New(ByVal SessionName As String)
        bool_Visible = False
        bool_Cancel = False
        path_SessionName = SessionName
    End Sub

    Public Function isConnect() As Boolean
        '--Starts, connects, and sets visibility--
        '--Provides a return boolean on whether GLink started correctly--
        isStarting = True
        GLAPI = New Glink.GlinkApi
        GLAPI.SessionName(path_SessionName)
        GLAPI.setVisible(bool_Visible)
        GLAPI.start()
        GLAPI.noToolbar()
        Monitor(STRING_RECEIVED)
        isStarting = False
        Return isConnected
    End Function
    Public Sub Connect()
        '--Starts, connects, and sets visibility--
        isStarting = False
        GLAPI = New Glink.GlinkApi
        GLAPI.SessionName(path_SessionName)
        GLAPI.setVisible(bool_Visible)
        GLAPI.start()
        GLAPI.noToolbar()
        Monitor(STRING_RECEIVED)
    End Sub
    Public Sub Retry()
        GLAPI = New Glink.GlinkApi
        GLAPI.SessionName(path_SessionName)
        GLAPI.setVisible(bool_Visible)
        GLAPI.start()
        GLAPI.noToolbar()
    End Sub
    Public Sub Disconnect()
        '--Disconnects GLink--
        If GLAPI.isConnected Then GLAPI.Disconnect()
        GLAPI.SessionName(Nothing)
        bool_Visible = Nothing
        path_SessionName = Nothing
        GLAPI = Nothing
    End Sub
    Public Sub SetVisible(ByVal boolVisible As Boolean)
        GLAPI.setVisible(boolVisible)
        bool_Visible = boolVisible
    End Sub
    Public Sub SubmitField(ByVal FieldName As Integer, ByVal StringToSend As String)
        '--Sends the desired string to the Field Number given--
        Dim FIELDS As Glink.GlinkFields
        FIELDS = GLAPI.getFields
        FIELDS.item(FieldName).setString(StringToSend)
    End Sub
    Public Sub TransmitPage()
        Dim COUNTER As Integer = 0
        GLAPI.sendCommandKey(TRANSMIT)
        While GLAPIEvent <> STRING_RECEIVED Or bool_Cancel
            COUNTER += 1
            Thread.Sleep(500)
            If COUNTER = TIMEOUT Or GLAPIEvent = GLINK_ERROR Then
                '--FAMIS appears to be frozen or the keyboard is locked--
                '--show screen and wait to see if user can transmit the--
                '--screen manually or user cancels process--
                GLAPI.setVisible(True)
                COUNTER = 0
                Exit Sub
            End If
        End While
        GLAPIEvent = Nothing
        Thread.Sleep(250)
    End Sub
    Public Sub SendKeysTransmit(ByVal StringToSend As String)
        '--Sends desired string to GLink and sends the Transmit command--
        GLAPI.SendKeys(StringToSend)
        TransmitPage()
    End Sub
    Public Sub SendCommand(ByVal CommandToSend As String)
        '--Sends a command string to the control field in FAMIS--
        SubmitField(FAMIS_CONTROLFIELD, CommandToSend)
    End Sub
    Public Sub SendCommandKey(ByVal Command As Glink.GlinkKeyEnum)
        Dim COUNTER As Integer = 0
        GLAPI.sendCommandKey(Command)
        While GLAPIEvent <> STRING_RECEIVED Or bool_Cancel
            COUNTER += 1
            Thread.Sleep(500)
            If COUNTER = TIMEOUT Or GLAPIEvent = GLINK_ERROR Then
                '--FAMIS appears to be frozen or the keyboard is locked--
                '--show screen and wait to see if user can transmit the--
                '--screen manually or user cancels process--
                GLAPI.setVisible(True)
                COUNTER = 0
            End If
        End While
        GLAPIEvent = Nothing
        Thread.Sleep(250)
    End Sub
    Public Sub PrintScreen()
        GLAPI.sendCommandKey(Glink.GlinkKeyEnum.GlinkKey_PRINT_SCREEN)
    End Sub
    Public Function isPageError(ByVal toPageNumber As String) As Boolean
        '--Check to see if the page turned to the next page or if there is a page error--
        '--Page number field in FAMIS is number 10--
        If GetField(10) <> toPageNumber Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetField(ByVal FieldNumber As Integer) As String
        '--Simple function to get a desired fields data--
        Dim FIELDS As Glink.GlinkFields
        FIELDS = GLAPI.getFields
        Return FIELDS.item(FieldNumber).getString()
    End Function
    Public Function GetField_Blinking() As String
        '--Return the fields that are blinking in FAMIS--
        Dim FIELDS As Glink.GlinkFields
        Dim FIELD As Glink.GlinkField
        Dim i As Integer
        Dim Result As String = Nothing
        Dim tempField As String
        FIELDS = GLAPI.getFields
        For i = 0 To FIELDS.getCount - 1
            FIELD = FIELDS.item(i + 1)
            If FIELD.getAttribute = BLINKING Then
                tempField = FIELDS.item(i).getString.Replace("(", "")
                tempField = tempField.Replace(")", "")
                tempField = tempField.Replace(".", "")
                tempField = tempField.Replace(" ", "")
                Result += vbCrLf & tempField & FIELD.getString
            End If
        Next
        Return Result
    End Function
    Public Function GetField_Red() As String
        Dim FIELDS As Glink.GlinkFields
        Dim FIELD As Glink.GlinkField
        Dim i As Integer
        Dim Result As String = Nothing
        Dim tempField As String
        FIELDS = GLAPI.getFields
        For i = 0 To FIELDS.getCount - 1
            FIELD = FIELDS.item(i + 1)
            If FIELD.getAttribute = RED Then
                tempField = FIELDS.getFieldIndex(FIELD)
                Result += vbCrLf & tempField & FIELD.getString
            End If
        Next
        Return Result
    End Function
    Public Function GetString(ByVal X1 As Integer, ByVal Y1 As Integer, ByVal X2 As Integer, ByVal Y2 As Integer) As String
        Return GLAPI.getString(GLAPI.GlinkPoint(X1, Y1), GLAPI.GlinkPoint(X2, Y2))
    End Function

    Private Sub Monitor(ByVal MonitorEvent As Integer)
        '--Waits until GLink sends the event that the page is loaded--
        Dim COUNTER As Integer = 0
        While GLAPIEvent <> MonitorEvent And bool_Cancel = False
            COUNTER += 1
            System.Threading.Thread.Sleep(500)
            If COUNTER = TIMEOUT Then
                '--FAMIS appears to be frozen or the keyboard is locked--
                '--show screen and wait to see if user can transmit the--
                '--screen manually or user cancels process--
                GLAPI.setVisible(True)
                Exit Sub
            ElseIf GLAPIEvent = GLINK_ERROR And isStarting Then
                isConnected = False
                Exit Sub
            End If
        End While
        isConnected = True
        GLAPIEvent = Nothing
    End Sub
    Private Sub GLAPI_onGlinkEvent(ByVal GLEvent As Glink.GlinkEvent) Handles GLAPI.onGlinkEvent
        '--Fired off of GLINK events--
        Dim EventCode As Integer = GLEvent.getEventCode

        Select Case EventCode
            Case 1
                GLAPIEvent = START
            Case 2
                GLAPIEvent = STOPPED
            Case 3
                GLAPIEvent = CONNECTED
            Case 4
                GLAPIEvent = DISCONNECTED
            Case 5
                GLAPIEvent = TURN_RECEIVED
            Case 6
                GLAPIEvent = TURN_LOST
            Case 7
                GLAPIEvent = STRING_RECEIVED
            Case 99
                GLAPIEvent = GLINK_ERROR
            Case Else
                '--A code we're unsure of the meaning was called. Do nothing--
                GLAPIEvent = Nothing
        End Select
    End Sub
End Class
