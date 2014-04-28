'--Designed by Michael Levine 5/2007--
Imports System.Xml

Public Class FAMISBlock
    '--Standard FAMIS block structure--

    Public BlockName As String            '--Name of block--
    Public isDeleteAllowed As Boolean     '--Boolean to show if FAMIS takes '-' for this block--
    Public FieldNumber As Integer         '--FAMIS Field Number--
    Public Length As Integer              '--Field Length--
    Public StartIndex As Integer          '--Start Index in Textfile--
    Public FileTable As String            '--Table Location--

    Friend Data As String                 '--Data--
    Friend DateSize As Integer            '--Size of Final Date String--
    Friend isSpaceAllowed As Boolean      '--Boolean to determine if the block allows spaces inbetween letters--

    Public Sub New()
        FieldNumber = Nothing
        Length = Nothing
        StartIndex = Nothing
        FileTable = Nothing
        DateSize = Nothing
        Data = " "
        isSpaceAllowed = False
        isDeleteAllowed = False
    End Sub
    Public Sub New(ByVal Name As String)
        FieldNumber = Nothing
        Length = Nothing
        StartIndex = Nothing
        FileTable = Nothing
        DateSize = Nothing
        Data = " "
        isSpaceAllowed = False
        isDeleteAllowed = False
        BlockName = Name
        SetupBlock(Name)
    End Sub
    Public Sub Deconstructor()
        FieldNumber = Nothing
        Length = Nothing
        StartIndex = Nothing
        FileTable = Nothing
        DateSize = Nothing
        Data = Nothing
    End Sub

    Public Overridable Sub SetData(ByRef xData As String)
        If xData.Length > 0 Then
            CleanData(xData)
            Data = xData
        End If
    End Sub
    Public Function GetData() As String
        Return Data
    End Function

    Friend Sub SetupBlock(ByVal Name As String)
        '--Reads in Length and FieldNumber stored in the XML document--
        Dim xmlDoc As XmlDocument = New XmlDocument
        Dim xReader As XmlNodeReader
        Dim xNode As XmlNode
        Try     '--TEMPORARY--
            xmlDoc.Load(My.Application.Info.DirectoryPath & "\FAMISBlock.xml")
            xNode = xmlDoc.DocumentElement.SelectSingleNode(Name)
            xReader = New XmlNodeReader(xNode)
            While xReader.Read
                If xReader.NodeType = XmlNodeType.Element Then
                    Select Case xReader.Name
                        Case "FieldNumber"
                            FieldNumber = xReader.ReadString()
                        Case "FieldLength"
                            Length = xReader.ReadString()
                        Case "StartIndex"
                            StartIndex = xReader.ReadString()
                        Case "FileTable"
                            FileTable = xReader.ReadString
                        Case "DateSize"
                            DateSize = xReader.ReadString()
                        Case "SpaceAllowed"
                            isSpaceAllowed = xReader.ReadString()
                        Case "DeleteAllowed"
                            isDeleteAllowed = xReader.ReadString
                    End Select
                End If
            End While
            Data = Data.PadRight(Length, " ")
        Catch ex As Exception
            '--Do nothing. No need for this once all blocks are in the XML file--
            Console.Write(Name & vbCrLf & ex.Message.ToString)
        End Try
        '--Set the data to all spaces--
    End Sub
    Friend Sub CleanData(ByRef xData As String)
        '--Parses and eliminates all symbols FAMIS throws errors for that come through GUMP--
        Dim tempLength As Integer = xData.Length
        xData = xData.Replace("'", "")
        xData = xData.Replace(",", "")
        xData = xData.Replace("!", "")
        xData = xData.Replace(";", "")
        xData = xData.Replace("/", "")
        'If BlockName <> "CA" And BlockName <> "CC" And BlockName <> "DC" And xData.IndexOf("-") > 0 Then xData = xData.Replace("-", " ")
        If xData.IndexOf("-") > 0 Then xData = xData.Replace("-", " ")
        If isSpaceAllowed = False Then xData = xData.Replace(" ", "")
        If xData.Length > 1 Then
            While xData.Substring(0, 1) = " " And xData.Substring(1, 1) <> " "
                xData = xData.Remove(0, 1)
                If xData.Length < 2 Then Exit While
            End While
        End If
        xData = xData.PadRight(tempLength, " ")
    End Sub
End Class
