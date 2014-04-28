' Developed by Hans Blomme (mail@hansblomme.com).
' Please do not hesitate to contact me in case of any problems. Also, do not forget
' to let me know if you want to stay up-to-date on any new fixes and/or releases.

Imports System.ComponentModel
Imports System.Runtime.InteropServices

Namespace HansBlomme.Windows.Forms
    <ToolboxBitmap(GetType(HansBlomme.Windows.Forms.NotifyIcon), "HansBlomme.Windows.Forms.NotifyIcon.bmp"), _
     System.ComponentModel.DefaultPropertyAttribute("Text"), _
     System.ComponentModel.DefaultEventAttribute("MouseDown"), _
     System.ComponentModel.ToolboxItemFilterAttribute("HansBlomme.Windows.Forms")> _
    Public NotInheritable Class NotifyIcon
        Inherits System.ComponentModel.Component

#Region " Message handler "
        Private Class MessageHandler
            Inherits Form

#Region " Interop Declarations "
            <DllImport("User32", CharSet:=CharSet.Auto)> _
            Private Shared Function RegisterWindowMessage(ByVal lpString As String) As Int32
            End Function

            Private Const WM_USER As Integer = &H400
            Private Const WM_USER_TRAY As Integer = WM_USER + 1
            Private Const WM_MOUSEMOVE = &H200
            Private Const WM_LBUTTONDOWN = &H201
            Private Const WM_LBUTTONUP = &H202
            Private Const WM_LBUTTONDBLCLK = &H203
            Private Const WM_RBUTTONDOWN = &H204
            Private Const WM_RBUTTONUP = &H205
            Private Const WM_RBUTTONDBLCLK = &H206
            Private Const WM_MBUTTONDOWN = &H207
            Private Const WM_MBUTTONUP = &H208
            Private Const WM_MBUTTONDBLCLK = &H209

            Private Const NIN_BALLOONSHOW As Int32 = &H402
            Private Const NIN_BALLOONHIDE As Int32 = &H403
            Private Const NIN_BALLOONTIMEOUT As Int32 = &H404
            Private Const NIN_BALLOONUSERCLICK As Int32 = &H405
#End Region

            Public Shadows Event Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Public Shadows Event DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
            Public Shadows Event MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            Public Shadows Event MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            Public Shadows Event MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            Public Event Reload()
            Public Event BalloonShow(ByVal sender As Object)
            Public Event BalloonHide(ByVal sender As Object)
            Public Event BalloonTimeout(ByVal sender As Object)
            Public Event BalloonClick(ByVal sender As Object)

            Private WM_TASKBARCREATED As Int32 = RegisterWindowMessage("TaskbarCreated")

            Public Sub New()
                ShowInTaskbar = False
                StartPosition = FormStartPosition.Manual
                Me.FormBorderStyle = FormBorderStyle.FixedToolWindow
                Size = New Size(100, 100)
                Location = New Point(-500, -500)
                Show()
            End Sub

            Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
                Select Case m.Msg
                    Case WM_USER_TRAY
                        Select Case m.LParam.ToInt32
                            Case WM_LBUTTONDBLCLK, WM_RBUTTONDBLCLK, WM_MBUTTONDBLCLK
                                RaiseEvent DoubleClick(Me, New MouseEventArgs(MouseButtons, 0, MousePosition.X, MousePosition.Y, 0))
                            Case WM_LBUTTONDOWN, WM_RBUTTONDOWN, WM_MBUTTONDOWN
                                RaiseEvent MouseDown(Me, New MouseEventArgs(MouseButtons, 0, MousePosition.X, MousePosition.Y, 0))
                            Case WM_MOUSEMOVE
                                RaiseEvent MouseMove(Me, New MouseEventArgs(MouseButtons, 0, MousePosition.X, MousePosition.Y, 0))
                            Case WM_LBUTTONUP
                                RaiseEvent MouseUp(Me, New MouseEventArgs(MouseButtons.Left, 0, MousePosition.X, MousePosition.Y, 0))
                                RaiseEvent Click(Me, New MouseEventArgs(MouseButtons.Left, 0, MousePosition.X, MousePosition.Y, 0))
                            Case WM_RBUTTONUP
                                RaiseEvent MouseUp(Me, New MouseEventArgs(MouseButtons.Right, 0, MousePosition.X, MousePosition.Y, 0))
                                RaiseEvent Click(Me, New MouseEventArgs(MouseButtons.Right, 0, MousePosition.X, MousePosition.Y, 0))
                            Case WM_MBUTTONUP
                                RaiseEvent MouseUp(Me, New MouseEventArgs(MouseButtons.Middle, 0, MousePosition.X, MousePosition.Y, 0))
                                RaiseEvent Click(Me, New MouseEventArgs(MouseButtons.Middle, 0, MousePosition.X, MousePosition.Y, 0))
                            Case NIN_BALLOONSHOW
                                RaiseEvent BalloonShow(Me)
                            Case NIN_BALLOONHIDE
                                RaiseEvent BalloonHide(Me)
                            Case NIN_BALLOONTIMEOUT
                                RaiseEvent BalloonTimeout(Me)
                            Case NIN_BALLOONUSERCLICK
                                RaiseEvent BalloonClick(Me)
                            Case Else
                                Debug.WriteLine(m.LParam.ToInt32)
                        End Select
                    Case WM_TASKBARCREATED
                        RaiseEvent Reload()
                    Case Else
                End Select
                MyBase.WndProc(m)
            End Sub
        End Class
#End Region
#Region " Component Designer generated code "

        Public Sub New(ByVal Container As System.ComponentModel.IContainer)
            MyClass.New()

            'Required for Windows.Forms Class Composition Designer support
            Container.Add(Me)

            Initialize()
        End Sub

        Public Sub New()
            MyBase.New()

            'This call is required by the Component Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call
            Initialize()
        End Sub

        'Component overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                Visible = False
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Component Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Component Designer
        'It can be modified using the Component Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container()
        End Sub

#End Region
#Region " Interop Declarations "
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
        Private Structure NOTIFYICONDATA
            Public cbSize As Int32
            Public hwnd As IntPtr
            Public uID As Int32
            Public uFlags As Int32
            Public uCallbackMessage As Int32
            Public hIcon As IntPtr
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)> _
            Public szTip As String
            Public dwState As Int32
            Public dwStateMask As Int32
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)> _
            Public szInfo As String
            Public uVersion As Int32
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)> _
            Public szInfoTitle As String
            Public dwInfoFlags As Int32
        End Structure

        <DllImport("Shell32", CharSet:=CharSet.Auto)> _
        Private Shared Function Shell_NotifyIcon(ByVal dwMessage As Integer, ByRef lpData As NOTIFYICONDATA) As Boolean
        End Function

        Private Const NIF_MESSAGE As Int32 = &H1
        Private Const NIF_ICON As Int32 = &H2
        Private Const NIF_STATE As Int32 = &H8
        Private Const NIF_INFO As Int32 = &H10
        Private Const NIF_TIP As Int32 = &H4
        Private Const NIM_ADD As Int32 = &H0
        Private Const NIM_MODIFY As Int32 = &H1
        Private Const NIM_DELETE As Int32 = &H2
        Private Const NIM_SETVERSION As Int32 = &H4
        Private Const NOTIFYICON_VERSION As Int32 = 5

        Private Const WM_USER = &H400
        Private Const WM_USER_TRAY = WM_USER + 1

        Public Enum EBalloonIcon
            None = &H0      ' NIIF_NONE
            [Error] = &H3   ' NIIF_ERROR
            Info = &H1      ' NIIF_INFO
            Warning = &H2   ' NIIF_WARNING
        End Enum
#End Region

        Private NID As NOTIFYICONDATA
        Private WithEvents Messages As MessageHandler = New MessageHandler()

        Private m_VisibleBeforeBalloon As Boolean
        Private m_Visible As Boolean
        Private m_Icon As System.Drawing.Icon
        Private m_ContextMenu As ContextMenu

        <DescriptionAttribute("Occurs when the user clicks the icon in the status area."), _
         System.ComponentModel.CategoryAttribute("Action")> _
        Public Event Click(ByVal sender As Object, ByVal e As System.EventArgs)
        <DescriptionAttribute("Occurs when the user double-clicks the icon in the status notification area of the taskbar."), _
         System.ComponentModel.CategoryAttribute("Action")> _
        Public Event DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
        <DescriptionAttribute("Occurs when the user presses the mouse button while the pointer is over the icon in the status notification area of the taskbar."), _
         CategoryAttribute("Mouse")> _
        Public Event MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        <DescriptionAttribute("Occurs when the user moves the mouse while the pointer is over the icon in the status notification area of the taskbar."), _
         CategoryAttribute("Mouse")> _
        Public Event MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        <DescriptionAttribute("Occurs when the user releases the mouse button while the pointer is over the icon in the status notification area of the taskbar."), _
         CategoryAttribute("Mouse")> _
        Public Event MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        <DescriptionAttribute("Occurs when the balloon is shown (balloons are queued)."), _
         System.ComponentModel.CategoryAttribute("Behavior")> _
        Public Event BalloonShow(ByVal sender As Object)
        <DescriptionAttribute("Occurs when the balloon disappears—for example, when the icon is deleted. This message is not sent if the balloon is dismissed because of a timeout or a mouse click."), _
         System.ComponentModel.CategoryAttribute("Behavior")> _
        Public Event BalloonHide(ByVal sender As Object)
        <DescriptionAttribute("Occurs when the balloon is dismissed because of a timeout."), _
         System.ComponentModel.CategoryAttribute("Behavior")> _
        Public Event BalloonTimeout(ByVal sender As Object)
        <DescriptionAttribute("Occurs when the balloon is dismissed because of a mouse click."), _
         System.ComponentModel.CategoryAttribute("Action")> _
        Public Event BalloonClick(ByVal sender As Object)

        Private Sub Initialize()
            With NID
                .hwnd = Messages.Handle
                .cbSize = Marshal.SizeOf(GetType(NOTIFYICONDATA))
                .uFlags = NIF_ICON Or NIF_TIP Or NIF_MESSAGE
                .uCallbackMessage = WM_USER_TRAY
                .uVersion = NOTIFYICON_VERSION
                .szTip = ""
                .uID = 0
            End With
        End Sub

        <Description("The icon to display in the system tray."), _
         CategoryAttribute("Appearance"), _
         System.ComponentModel.DefaultValueAttribute("")> _
        Public Property Icon() As System.Drawing.Icon
            Get
                Return m_Icon
            End Get
            Set(ByVal Value As System.Drawing.Icon)
                m_Icon = Value
                NID.uFlags = NID.uFlags Or NIF_ICON
                NID.hIcon = Icon.Handle
                If Visible Then
                    Shell_NotifyIcon(NIM_MODIFY, NID)
                End If
            End Set
        End Property

        <Description("The text that will be displayed when the mouse hovers over the icon."), _
         CategoryAttribute("Appearance")> _
        Public Property Text() As String
            Get
                Return NID.szTip
            End Get
            Set(ByVal Value As String)
                NID.szTip = Value
                If Visible Then
                    NID.uFlags = NID.uFlags Or NIF_TIP
                    Shell_NotifyIcon(NIM_MODIFY, NID)
                End If
            End Set
        End Property

        <Description("The pop-up menu to show when the user right-clicks the icon."), _
         CategoryAttribute("Behavior"), _
         System.ComponentModel.DefaultValueAttribute("")> _
        Public Property ContextMenu() As System.Windows.Forms.ContextMenu
            Get
                Return m_ContextMenu
            End Get
            Set(ByVal Value As System.Windows.Forms.ContextMenu)
                m_ContextMenu = Value
            End Set
        End Property

        <Description("Determines whether the control is visible or hidden."), _
         CategoryAttribute("Behavior"), _
         System.ComponentModel.DefaultValueAttribute(False)> _
        Public Property Visible() As Boolean
            Get
                Return m_Visible
            End Get
            Set(ByVal Value As Boolean)
                m_Visible = Value
                If Not DesignMode Then
                    If m_Visible Then
                        Shell_NotifyIcon(NIM_ADD, NID)
                    Else
                        Shell_NotifyIcon(NIM_DELETE, NID)
                    End If
                End If
            End Set
        End Property

        Public Sub ShowBalloon(ByVal Icon As EBalloonIcon, ByVal Text As String, ByVal Title As String, Optional ByVal Timeout As Integer = 15000)
            m_VisibleBeforeBalloon = m_Visible
            With NID
                .uFlags = .uFlags Or NIF_INFO
                .uVersion = Timeout
                .szInfo = Text
                .szInfoTitle = Title
                .dwInfoFlags = Convert.ToInt32(Icon)
            End With
            If Not Visible Then
                Visible = True
            Else
                Shell_NotifyIcon(NIM_MODIFY, NID)
            End If
            NID.uFlags = NID.uFlags And Not NIF_INFO
        End Sub

        Private Shadows Sub Messages_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Messages.Click
            RaiseEvent Click(Me, e)
        End Sub

        Private Shadows Sub Messages_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Messages.DoubleClick
            RaiseEvent DoubleClick(Me, e)
        End Sub

        Private Shadows Sub Messages_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Messages.MouseDown
            RaiseEvent MouseDown(Me, e)
        End Sub

        Private Shadows Sub Messages_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Messages.MouseMove
            RaiseEvent MouseMove(Me, e)
        End Sub

        Private Shadows Sub Messages_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Messages.MouseUp
            RaiseEvent MouseUp(Me, e)

            If e.Button = MouseButtons.Right Then
                Messages.Activate()
                Dim Position As Point = New Point(Cursor.Position.X - Messages.PointToScreen(New Point(0, 0)).X, Cursor.Position.Y - Messages.PointToScreen(New Point(0, 0)).Y)
                If Not m_ContextMenu Is Nothing Then m_ContextMenu.Show(Messages, Position)
            End If
        End Sub

        Private Sub Messages_Reload() Handles Messages.Reload
            If Visible Then Visible = True
        End Sub

        Private Sub Messages_BalloonShow(ByVal sender As Object) Handles Messages.BalloonShow
            RaiseEvent BalloonShow(Me)
        End Sub

        Private Sub Messages_BalloonHide(ByVal sender As Object) Handles Messages.BalloonHide
            RaiseEvent BalloonHide(Me)
        End Sub

        Private Sub Messages_BalloonTimeout(ByVal sender As Object) Handles Messages.BalloonTimeout
            If Not m_VisibleBeforeBalloon Then Visible = False
            RaiseEvent BalloonTimeout(Me)
        End Sub

        Private Sub Messages_BalloonClick(ByVal sender As Object) Handles Messages.BalloonClick
            If Not m_VisibleBeforeBalloon Then Visible = False
            RaiseEvent BalloonClick(Me)
        End Sub
    End Class
End Namespace