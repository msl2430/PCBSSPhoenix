Public Class viewIMPs

    Private CASENUMBER, BATCHNUMBER, ERRORMESSAGE1, ERRORMESSAGE2, ERRORMESSAGE3, ERRORMESSAGE4, ERRORMESSAGE5 As String
    Private File_Reader As StreamReader
    Private PageNumber As Integer           '--Keep track of the page number when printing-
    Private printFont As Font

    Private Sub viewIMPs_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DateChoice.Value = Date.Today
        If DataGridView1.RowCount > 0 Then btn_Print.Enabled = True Else btn_Print.Enabled = False
        DataGridView1.Columns.Item(0).HeaderText = "Case Number"
        DataGridView1.Columns.Item(1).HeaderText = "Batch Number"
    End Sub

    Private Sub DateChoice_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateChoice.ValueChanged
        IMPSInformationTableAdapter.Fill(PhoenixDataSet.IMPSInformation, DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year)
        If DataGridView1.RowCount > 0 Then btn_Print.Enabled = True Else btn_Print.Enabled = False
    End Sub

    Private Sub btn_Print_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Print.Click
        GetInfo()
        PrintStats()
        If File.Exists(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt") Then File.Delete(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt")
    End Sub

    Sub GetInfo()
        Dim SQLConn As New SqlConnection(My.Settings.phxSQLConn)
        Dim SQLComm As New SqlCommand
        Dim SQLReader As SqlDataReader
        Dim File_Writer As StreamWriter
        Try
            SQLComm.Connection = SQLConn
            SQLConn.Open()

            File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
            File_Writer.WriteLine("Phoenix - IMPS Cases" & vbCrLf & vbCrLf & DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year)
            File_Writer.Close()

            File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
            File_Writer.WriteLine(vbCrLf & "--CASES DROPPED TODAY--" & vbCrLf)
            File_Writer.Close()

            SQLComm.CommandText = "SELECT CASENUMBER, REASON, REASON2, REASON3, REASON4, REASON5 FROM IMPSInformation WHERE Dropped = 'True' and dateentered = '" & DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year & "'"
            SQLReader = SQLComm.ExecuteReader
            If SQLReader.HasRows = True Then
                While SQLReader.Read
                    If SQLReader.IsDBNull(0) = False Then CASENUMBER = SQLReader.GetString(0)
                    If SQLReader.IsDBNull(1) = False Then ERRORMESSAGE1 = SQLReader.GetString(1)
                    If SQLReader.IsDBNull(2) = False Then ERRORMESSAGE2 = SQLReader.GetString(2)
                    If SQLReader.IsDBNull(3) = False Then ERRORMESSAGE3 = SQLReader.GetString(3)
                    If SQLReader.IsDBNull(4) = False Then ERRORMESSAGE4 = SQLReader.GetString(4)
                    If SQLReader.IsDBNull(5) = False Then ERRORMESSAGE5 = SQLReader.GetString(5)
                    WriteDropSheet()
                End While
                SQLReader.Close()
                SQLConn.Close()
            Else
                SQLConn.Close()

                File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
                File_Writer.WriteLine("*****No Dropped Cases*****")
                File_Writer.Close()
            End If

            File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
            File_Writer.WriteLine(vbCrLf & vbCrLf & "--CASES COMPLETED TODAY--" & vbCrLf)
            File_Writer.Close()

            SQLConn.Open()
            SQLComm.CommandText = "SELECT CASENUMBER, BATCHNUMBER FROM IMPSInformation WHERE Dropped = 'False' and DateEntered = '" & DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year & "' ORDER BY BatchNumber"
            SQLReader = SQLComm.ExecuteReader
            If SQLReader.HasRows = True Then
                While SQLReader.Read
                    If SQLReader.IsDBNull(0) = False Then CASENUMBER = SQLReader.GetString(0)
                    If SQLReader.IsDBNull(1) = False Then BATCHNUMBER = SQLReader.GetString(1)
                    WriteSuccessSheet()
                End While
                SQLReader.Close()
                SQLConn.Close()
            Else
                SQLConn.Close()

                File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
                File_Writer.WriteLine("*****No Completed Cases Today*****")
                File_Writer.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
        End Try
    End Sub
    Sub WriteDropSheet()
        Dim File_Writer As StreamWriter

        File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
        File_Writer.WriteLine("Case Number: " & CASENUMBER)
        If ERRORMESSAGE1.Substring(0, 10) <> "          " Then
            File_Writer.WriteLine("        Error Reason: " & ERRORMESSAGE1)
        End If
        If ERRORMESSAGE2.Substring(0, 10) <> "          " Then
            File_Writer.WriteLine("        Error Reason: " & ERRORMESSAGE2)
        End If
        If ERRORMESSAGE3.Substring(0, 10) <> "          " Then
            File_Writer.WriteLine("        Error Reason: " & ERRORMESSAGE3)
        End If
        If ERRORMESSAGE4.Substring(0, 10) <> "          " Then
            File_Writer.WriteLine("        Error Reason: " & ERRORMESSAGE4)
        End If
        If ERRORMESSAGE5.Substring(0, 10) <> "          " Then
            File_Writer.WriteLine("        Error Reason: " & ERRORMESSAGE5)
        End If
        File_Writer.Close()
    End Sub
    Sub WriteSuccessSheet()
        Dim File_Writer As StreamWriter

        File_Writer = New StreamWriter(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
        File_Writer.WriteLine("Case Number: " & CASENUMBER & "    Batch Number: " & BATCHNUMBER)
        File_Writer.Close()
    End Sub
    Sub PrintStats()
        'Dim oWord As Microsoft.Office.Interop.Word.Application
        'oWord = New Microsoft.Office.Interop.Word.ApplicationClass
        'oWord.Documents.Add(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt")
        'oWord.ActiveWindow.ActivePane.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdPrintView
        'oWord.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter
        'oWord.Selection.TypeText("Phoenix - IMPS Processing " & DateChoice.Value & "                                                                            Page: ")
        'oWord.Selection.Fields.Add(oWord.Selection.Range, Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage)

        'oWord.PrintOut(False)
        'oWord.Quit(0)
        'oWord = Nothing
        File_Reader = New StreamReader(My.Application.Info.DirectoryPath & "\Phoenix - IMPS Drops.txt", True)
        PageNumber = 1
        PrintDoc.Print()
        File_Reader.Close()
    End Sub

    Private Sub PrintDoc_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDoc.PrintPage
        Dim linesPerPage As Single = 0
        Dim yPosition As Single = e.MarginBounds.Top / 2
        Dim count As Integer = 1
        Dim leftMargin As Single = e.MarginBounds.Left / 2
        Dim topMargin As Single = e.MarginBounds.Top / 2
        Dim line As String = Nothing
        Dim myBrush As New SolidBrush(Color.Black)
        printFont = txt_Font.Font
        linesPerPage = (e.MarginBounds.Height + topMargin) / printFont.GetHeight(e.Graphics)
        If PageNumber > 1 Then
            '--Add header to multiple pages--
            e.Graphics.DrawString("Page: " & PageNumber.ToString, printFont, myBrush, leftMargin, yPosition, New StringFormat)
            yPosition = topMargin + (count * printFont.GetHeight(e.Graphics))
            count += 1
            e.Graphics.DrawString(" ", printFont, myBrush, leftMargin, yPosition, New StringFormat)
            yPosition = topMargin + (count * printFont.GetHeight(e.Graphics))
            count += 1
        End If
        While count < linesPerPage
            line = File_Reader.ReadLine
            e.Graphics.DrawString(line, printFont, myBrush, leftMargin, yPosition, New StringFormat)
            yPosition = topMargin + (count * printFont.GetHeight(e.Graphics))
            count += 1
        End While
        If File_Reader.Peek <> -1 Then
            PageNumber += 1
            e.HasMorePages = True
        Else
            e.HasMorePages = False
            myBrush.Dispose()
        End If
    End Sub
End Class