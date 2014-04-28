'--Designed by Michael Levine 5/2007--
Public Class FAMISBlock_Date
    Inherits FAMISBlock

    Public Sub New(ByVal NameDate As String)
        FieldNumber = Nothing
        Length = Nothing
        StartIndex = Nothing
        DateSize = Nothing
        Data = " "
        isSpaceAllowed = False
        isDeleteAllowed = True
        BlockName = NameDate
        SetupBlock(NameDate)
    End Sub

    Public Overrides Sub SetData(ByRef xData As String)
        If xData.Length > 0 Then
            Data = xData
            DateAdjust()
            CleanData(xData)
        End If
    End Sub

    Private Sub DateAdjust()
        '--GUMP sends dates in 'MM/DD/YYYY' format and FAMIS wants MMDDYYYY or MMYYYY--
        '--DateSize indicates whether FAMIS wants an 8 digit format for the block or a 6 digit format for the block--
        If DateSize = 8 Then
            If Data = "          " Then
                Data = "        "
            Else
                If Data = "-         " Then
                    Data = "-       "
                Else
                    Data = Data.Replace("/", "")
                End If
            End If
        End If
        If DateSize = 6 Then
            If Data = "          " Then
                Data = "      "
            Else
                If Data = "-       " Then
                    Data = "-     "
                Else
                    Data = Data.Replace("/", "")
                    Data = Data.Replace(" ", "")
                End If
            End If
        End If
        If Data = "00000000  " Then Data = "00000000"
    End Sub

End Class
