'--Designed by Michael Levine 5/2007--
Imports System.IO

Public Class stdLog
    '--Log file methods--

    Private pathLog As String
    Private logWriter As StreamWriter

    Public Sub New(ByVal path_Log As String)
        pathLog = path_Log & "PhoenixLog(" & Date.Today.Month & "_" & Date.Today.Day & "_" & Date.Today.Year & ").log"
        If File.Exists(pathLog) = False Then
            logWriter = New StreamWriter(pathLog, False)
            logWriter.WriteLine("Phoenix Log" & vbCrLf & Date.Today.Month & "_" & Date.Today.Day & "_" & Date.Today.Year & vbCrLf)
            logWriter.Close()
        End If
    End Sub

    Public Sub logMessage(ByVal logEvent As String, ByVal isError As Boolean)
        '--Open the file and write as a standard or error event--
        logWriter = New StreamWriter(pathLog, True)
        If isError = False Then
            logWriter.WriteLine(Date.Now.Hour & ":" & Date.Now.Minute & ":" & Date.Now.Second.ToString & " >> " & logEvent)
        ElseIf isError = True Then
            logWriter.WriteLine(Date.Now.Hour & ":" & Date.Now.Minute & ":" & Date.Now.Second.ToString & " >> ERROR: " & logEvent)
        End If
        logWriter.Close()
    End Sub
End Class
