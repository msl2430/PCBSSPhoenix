Public Class DeleteFile

    Private Directory As DirectoryInfo
    Private FileList() As FileInfo

    Private Sub DeleteFile_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GetList()
        btn_Submit.Enabled = False
    End Sub
    Private Sub GetList()
        Dim i As Integer
        cmb_FileList.Items.Clear()
        Directory = New DirectoryInfo(My.Settings.FilePath_GUMP)
        Directory.Refresh()
        If Directory.Exists Then
            FileList = Directory.GetFiles("*.txt")
            If FileList.Length > 0 Then
                For i = 0 To FileList.Length - 1
                    cmb_FileList.Items.Add(FileList(i).Name)
                Next
            Else
                cmb_FileList.Text = "No GUMP Files"
                cmb_FileList.Enabled = False
                btn_Submit.Enabled = False
            End If
        End If
    End Sub

    Private Sub cmb_FileList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmb_FileList.SelectedIndexChanged
        btn_Submit.Enabled = True
    End Sub

    Private Sub btn_Submit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Submit.Click
        If MessageBox.Show("Delete GUMP File?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = System.Windows.Forms.DialogResult.Yes Then
            If File.Exists(Directory.FullName & cmb_FileList.SelectedItem) Then File.Delete(Directory.FullName & cmb_FileList.SelectedItem)
            GetList()
            cmb_FileList.Text = "Select A File"
        End If
    End Sub
End Class