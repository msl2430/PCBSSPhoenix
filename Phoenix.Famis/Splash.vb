Public NotInheritable Class Splash

    Public isSplash As Boolean
    Public isClosing As Boolean
    Public isUpdating As Boolean

    Private Sub Splash_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        isUpdating = True
        Version.Text = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Revision.ToString
        If isSplash Then
            If File.Exists(My.Application.Info.DirectoryPath & "\phxConfig.dat") Then
                Try
                    DecryptFile(My.Application.Info.DirectoryPath & "\phxConfig.dat", My.Application.Info.DirectoryPath & "\phxConfig.phx", "PhOeNiX9")
                    Dim ReadFile As New StreamReader(My.Application.Info.DirectoryPath & "\phxConfig.phx")
                    Dim Reader As String
                    While ReadFile.Peek <> -1
                        Reader = ReadFile.ReadLine
                        Select Case Reader.Substring(0, Reader.IndexOf(" "))
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
                    WriteFile.WriteLine("FAMISOperatorID = 16A999") '& My.Settings.FAMISOperatorID)
                    WriteFile.WriteLine("FAMISPassword = 123456") ' & My.Settings.FAMISPassword)
                    WriteFile.WriteLine("Keyword = UAPSUP") ' & My.Settings.FAMISKeyword)
                    WriteFile.WriteLine("BatchNumber_Pre = 999") ' & My.Settings.BatchNumber_Pre)
                    WriteFile.WriteLine("BatchNumber_ANum = 001") ' & My.Settings.BatchNumber_ANum)
                    WriteFile.WriteLine("BatchNumber_UNum = 001") ' & My.Settings.BatchNumber_UNum)
                    WriteFile.WriteLine("FilePath_GUMP = G:\") ' & My.Settings.FilePath_GUMP)
                    WriteFile.WriteLine("isArchive = False") ' & My.Settings.isArchive)
                    WriteFile.WriteLine("ArchiveDirectory = C:\Program Files\Phoenix\Old Text Files") ' & My.Settings.ArchiveDirectory)
                    WriteFile.WriteLine("SQLAddress = 172.16.8.15") ' & My.Settings.SQLAddress)
                    WriteFile.WriteLine("UpdateAddress = 172.16.10.21") ' & My.Settings.UpdateAddress)
                    WriteFile.Close()
                Catch ex As Exception
                    MessageBox.Show("Location: Get Config" & vbCrLf & ex.Message)
                End Try
            End If
            isUpdating = True
            CheckVersion()
            isUpdating = False
        End If
    End Sub
    Private Sub CheckVersion()
        If My.Settings.UpdateAddress <> "" Or My.Settings.UpdateAddress <> Nothing Then
            If My.Computer.Network.Ping(My.Settings.UpdateAddress) Then
                If File.Exists("\\" & My.Settings.UpdateAddress & "\Update\UpdateInfo.txt") Then
                    Dim fileVersion As New StreamReader("\\" & My.Settings.UpdateAddress & "\Update\UpdateInfo.txt")
                    If My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Revision.ToString <> fileVersion.ReadLine.Substring(8, 5) Then
                        isUpdating = True
                        If MessageBox.Show("You are using an older version." & vbCrLf & "Would you like to update?", "Phoenix - Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                            Try
                                Thread.Sleep(500)
                                Process.Start(My.Application.Info.DirectoryPath & "\Phoenix - Update.exe")
                                Me.Close()
                            Catch ex As Exception
                                MessageBox.Show("Location: CheckVersion" & vbCrLf & ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Try
                        End If
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
    Private Sub Splash_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Click
        If Not isSplash Then Me.Close()
    End Sub
End Class
