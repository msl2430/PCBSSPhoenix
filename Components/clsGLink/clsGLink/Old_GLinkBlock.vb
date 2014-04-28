'--Designed by Michael Levine 5/2007--
Imports System.Xml

Public Class Old_GLinkBlock
    '--Standard FAMIS block structure--

    '--Public Variables--
    Public FieldNumber As Integer '--FAMIS Field Number--
    Public Length As Integer      '--Field Length--
    Public StartIndex As Integer  '--Start Index in Textfile--
    Public FileTable As String    '--Table Location--

    '--Private Data Variable--
    Friend Data As String
    Friend DateSize As Integer

    '--Boolean to determine if the block allows spaces inbetween letters--
    Friend bool_SpaceAllowed As Boolean

    Public Sub New()
        FieldNumber = Nothing
        Length = Nothing
        StartIndex = Nothing
        FileTable = Nothing
        DateSize = Nothing
        Data = " "
        bool_SpaceAllowed = False
    End Sub
    Public Sub New(ByVal Name As String)
        FieldNumber = Nothing
        Length = Nothing
        StartIndex = Nothing
        FileTable = Nothing
        DateSize = Nothing
        Data = " "
        bool_SpaceAllowed = False
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
        CleanData(xData)
        Data = xData
    End Sub
    Public Function GetData() As String
        Return Data
    End Function

    Friend Sub SetupBlock(ByVal Name As String)
        '--Reads in Length and FieldNumber stored in the XML document--
        Dim xmlDoc As XmlDocument = New XmlDocument
        Dim xReader As XmlNodeReader
        Dim xNode As XmlNode
        xmlDoc.Load("C:\FAMISBlock.xml") 'xmldoc.load(my.application.info.directorypath & "\FAMISBlock.xml")
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
                        bool_SpaceAllowed = xReader.ReadString()
                End Select
            End If
        End While
        'SetData(GetData.PadRight(Length, " "))
        Data = Data.PadRight(Length, " ")
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
        If bool_SpaceAllowed = False Then xData = xData.Replace(" ", "")
        If xData.Length > 1 Then
            While xData.Substring(0, 1) = " " And xData.Substring(1, 1) <> " "
                xData = xData.Remove(0, 1)
            End While
        End If
        xData = xData.PadRight(tempLength, " ")
    End Sub
End Class
