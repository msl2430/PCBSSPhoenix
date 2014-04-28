Public Class viewBatch

    Private File_Reader As StreamReader
    Private PageNumber As Integer           '--Keep track of the page number when printing-
    Private printFont As Font

    Private Sub viewBatch_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DateChoice.Value = Date.Today
        If DataGridView1.RowCount > 0 Then btn_Print.Enabled = True Else btn_Print.Enabled = False
        DataGridView1.Columns.Item(0).HeaderText = "Case Number"
        DataGridView1.Columns.Item(1).HeaderText = "Operator"
        DataGridView1.Columns.Item(2).HeaderText = "Batch Number"
    End Sub

    Private Sub DateChoice_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateChoice.ValueChanged
        FAMISCaseInformationTableAdapter.FillGrid(PhoenixDataSet.FAMISCaseInformation, DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year)
        If DataGridView1.RowCount > 0 Then btn_Print.Enabled = True Else btn_Print.Enabled = False
    End Sub

    Private Sub btn_Print_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Print.Click
        Dim FileWriter As StreamWriter
        Dim SQLConn As New SqlConnection(My.Settings.phxSQLConn)
        Dim SQLComm As New SqlCommand
        Dim SQLReader As SqlDataReader
        Dim FilePath As String = My.Application.Info.DirectoryPath & "\Phoenix - Batch List.txt"
        SQLComm.Connection = SQLConn

        If File.Exists(FilePath) Then File.Delete(FilePath)
        FileWriter = New StreamWriter(FilePath, True)
        FileWriter.WriteLine("Phoenix - GUMP To FAMIS" & vbCrLf & vbCrLf & "Case Number           Operator          Batch Number" & vbCrLf)
        SQLConn.Open()
        SQLComm.CommandText = "SELECT CaseNumber, Operator, BatchNumber FROM FAMISCaseInformation WHERE DateEntered = '" & DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year & "' ORDER BY Operator, BatchNumber"
        SQLReader = SQLComm.ExecuteReader
        While SQLReader.Read
            If SQLReader.HasRows = True Then
                FileWriter.WriteLine(SQLReader.GetString(0) & "            " & SQLReader.GetString(1) & "            " & SQLReader.GetString(2))
            End If
        End While
        SQLConn.Close()
        FileWriter.Close()

        'Dim oWord As Microsoft.Office.Interop.Word.Application
        'Dim FootNote As String = Microsoft.VisualBasic.Mid("Phoenix - Batch List", 1)
        'oWord = New Microsoft.Office.Interop.Word.ApplicationClass
        'oWord.Documents.Add(FilePath)
        'oWord.ActiveWindow.ActivePane.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdPrintView
        'oWord.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter
        'oWord.Selection.TypeText("Phoenix - Batch List          " & DateChoice.Value.Month & "/" & DateChoice.Value.Day & "/" & DateChoice.Value.Year & "                                                                            Page: ")
        'oWord.Selection.Fields.Add(oWord.Selection.Range, Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage)

        'oWord.PrintOut(False)
        'oWord.Quit(0)
        'oWord = Nothing
        File_Reader = New StreamReader(My.Application.Info.DirectoryPath & "\Phoenix - Batch List.txt", True)
        PageNumber = 1
        PrintDoc.Print()
        File_Reader.Close()
        If File.Exists(My.Application.Info.DirectoryPath & "\Phoenix - Batch List.txt") Then File.Delete(My.Application.Info.DirectoryPath & "\Phoenix - Batch List.txt")
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