Public Class FAMISBlock
    Public FieldNumber As Integer
    Public Data As String

    Private bool_SpaceAllowed As Boolean

    Public Sub New()
        FieldNumber = Nothing
        Data = Nothing
        bool_SpaceAllowed = False
    End Sub
    Public Sub New(ByVal xFieldNumber As Integer, ByVal xData As String)
        bool_SpaceAllowed = False
        CleanData(xData)
        FieldNumber = xFieldNumber
        Data = xData
    End Sub
    Public Sub New(ByVal xFieldNumber As Integer, ByVal xData As String, ByVal SpaceAllowed As Boolean)
        bool_SpaceAllowed = SpaceAllowed
        CleanData(xData)
        FieldNumber = xFieldNumber
        Data = xData
    End Sub

    Public Sub DateAdjust(ByVal Size As Integer)
        If Size = 8 Then
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
        If Size = 6 Then
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
    End Sub

    Private Sub CleanData(ByRef xData As String)
        xData = xData.Replace("'", "")
        xData = xData.Replace(",", "")
        xData = xData.Replace("!", "")
        xData = xData.Replace(";", "")
        xData = xData.Replace("/", "")
        If bool_SpaceAllowed = False Then xData = xData.Replace(" ", "")
        If xData.Length <> 1 Then
            While xData.Substring(0, 1) = " " And xData.Substring(1, 1) <> " "
                xData = xData.Remove(0, 1)
            End While
        End If
        xData = xData.PadRight(xData.Length, " ")
    End Sub
End Class

Public Class Child
    Public QA, QB, QC, QD, QE, QF, QK, QL, QM, QN, QO, RA, RB, RC, RD, RE, RF, RG, RH, RI As FAMISBlock
End Class