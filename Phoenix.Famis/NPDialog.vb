Public Class NPDialog
    Public CaseNumber As String
    Public NPParentForm_105 As Put105Thru
    Public LastChoice As String

    Private Sub NPDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        btn_Yes.Text = "Yes" & vbCrLf & "(A Batch)"
        btn_No.Text = "No" & vbCrLf & "(U Batch)"
        btn_Cancel.Text = "Cancel"
        Me.Text = "Case: " & CaseNumber
        Me.WindowState = FormWindowState.Minimized
        Me.WindowState = FormWindowState.Normal
        Me.Activate()
        Select Case LastChoice
            Case "ABatch"
                btn_Yes.Focus()
            Case "UBatch"
                btn_No.Focus()
            Case "Cancel"
                btn_Cancel.Focus()
            Case Else
                btn_Yes.Focus()
        End Select
    End Sub

    Private Sub btn_Yes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Yes.Click
        NPParentForm_105.LineNP = "ABatch"
        NPParentForm_105.isView105 = chk_View105.Checked
        Me.Close()
    End Sub
    Private Sub btn_No_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_No.Click
        NPParentForm_105.LineNP = "UBatch"
        NPParentForm_105.isView105 = chk_View105.Checked
        Me.Close()
    End Sub
    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click
        NPParentForm_105.LineNP = "Cancel"
        NPParentForm_105.isView105 = chk_View105.Checked
        Me.Close()
    End Sub
End Class