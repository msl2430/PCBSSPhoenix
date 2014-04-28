Public Class Options

    Private Sub Options_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AddEvents(Me)
        If My.Settings.isArchive = False Then
            chk_Archive.Checked = False
            txt_Archive.Enabled = False
        Else
            chk_Archive.Checked = True
            txt_Archive.Enabled = True
        End If
        TabControl1.SelectedTab = TabPage2
        TabControl1.SelectedTab = TabPage1
    End Sub

    Private Sub AddEvents(ByVal ctrlparent As Control)
        Dim ctrl As Control
        For Each ctrl In ctrlparent.Controls
            If TypeOf ctrl Is TextBox Then
                AddHandler ctrl.TextChanged, AddressOf txtChanged
                AddHandler ctrl.Leave, AddressOf LoseFocus
                AddHandler ctrl.KeyPress, AddressOf NumericOnly
            ElseIf TypeOf ctrl Is CheckBox Then
                AddHandler ctrl.Click, AddressOf txtChanged
            End If
            If ctrl.HasChildren Then
                AddEvents(ctrl)
            End If
        Next
    End Sub
    Private Sub txtChanged(ByVal sender As Object, ByVal e As EventArgs)
        btn_Apply.Enabled = True
    End Sub
    Private Sub LoseFocus(ByVal sender As Object, ByVal e As EventArgs)
        If DirectCast(sender, TextBox).Name = "txt_APre" Or DirectCast(sender, TextBox).Name = "txt_ANum" Or DirectCast(sender, TextBox).Name = "txt_UPre" Or DirectCast(sender, TextBox).Name = "txt_UNum" Then
            If DirectCast(sender, TextBox).Text.Length < 3 Then
                DirectCast(sender, TextBox).Text = DirectCast(sender, TextBox).Text.PadLeft(3, "0")
            End If
        End If
    End Sub
    Private Sub NumericOnly(ByVal sender As Object, ByVal e As KeyPressEventArgs)
        If DirectCast(sender, TextBox).Name = "txt_APre" Or DirectCast(sender, TextBox).Name = "txt_ANum" Or DirectCast(sender, TextBox).Name = "txt_UPre" Or DirectCast(sender, TextBox).Name = "txt_UNum" Then
            If Char.IsNumber(e.KeyChar) = True Or Asc(e.KeyChar) = 8 Then
                e.Handled = False
            Else
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub btn_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OK.Click
        My.Settings.BatchNumber_Pre = txt_APre.Text
        My.Settings.BatchNumber_ANum = txt_ANum.Text
        My.Settings.BatchNumber_UNum = txt_UNum.Text
        My.Settings.FAMISOperatorID = txt_Operator.Text
        My.Settings.FAMISPassword = txt_Password.Text
        My.Settings.SQLAddress = txt_ServerAddy.Text
        My.Settings.isArchive = chk_Archive.Checked
        My.Settings.ArchiveDirectory = txt_Archive.Text
        My.Settings.phxSQLConn = "User ID=PhoenixUser;Data Source=" & My.Settings.SQLAddress & "\Phoenix;FailOver Partner=192.168.204.3\Phoenix;Password=password;Initial Catalog=PhoenixData;" & _
           "Connect Timeout=3;Integrated Security=False;Persist Security Info=True;"
        My.Settings.UpdateAddress = txt_UpdateAddress.Text
        Me.Close()
    End Sub
    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click
        Me.Close()
    End Sub
    Private Sub btn_Apply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Apply.Click
        My.Settings.BatchNumber_Pre = txt_APre.Text
        My.Settings.BatchNumber_ANum = txt_ANum.Text
        My.Settings.BatchNumber_UNum = txt_UNum.Text
        My.Settings.FAMISOperatorID = txt_Operator.Text
        My.Settings.FAMISPassword = txt_Password.Text
        My.Settings.SQLAddress = txt_ServerAddy.Text
        My.Settings.isArchive = chk_Archive.Checked
        My.Settings.ArchiveDirectory = txt_Archive.Text
        My.Settings.UpdateAddress = txt_UpdateAddress.Text
        btn_Apply.Enabled = False
    End Sub

    Private Sub chk_Archive_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_Archive.CheckedChanged
        If chk_Archive.Checked Then txt_Archive.Enabled = True Else txt_Archive.Enabled = False
    End Sub

    Private Sub Options_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
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
            MessageBox.Show("Location: Settings" & vbCrLf & ex.Message)
        End Try
        'End If
    End Sub
End Class