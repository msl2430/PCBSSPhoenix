'--Designed by Michael Levine 5/2007--
Imports system.Data.SqlClient

Public Class PhoenixSQL
    '--SQL Connection and Query methods--

    '--Keep the SQL connection and command variables private to avoid errors--
    Private connSQL As New SqlConnection
    Private commandSQL As New SqlCommand

    Public Sub New(ByVal ServerPath As String, ByVal Login As String)
        '--Password is "password" for all Phoenix connections to the SQL server. Login determines which database is accessed--
        connSQL.ConnectionString = "Data Source=" & ServerPath & "\PHOENIX;Initial Catalog=PHOENIX;Persist Security Info=True;User ID=" & Login & ";Password=password"
    End Sub

    Public Function isRowExist(ByVal Value As String, ByVal Column As String, ByVal Location As String)
        '--Query just to find out if the data exists on the Server--
        '--Returns true if there false otherwise--
        Dim Reader As SqlDataReader
        commandSQL.CommandText = "SELECT * FROM " & Location & " WHERE " & Column & " = " & Value
        commandSQL.Connection = connSQL
        commandSQL.Connection.Open()
        Reader = commandSQL.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
        Reader.Read()
        If Reader.HasRows = True Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Query(ByVal CommandString As String) As SqlDataReader
        '--Query database and return results--
        Dim Reader As SqlDataReader
        commandSQL.CommandText = CommandString
        commandSQL.Connection = connSQL
        commandSQL.Connection.Open()
        Reader = commandSQL.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
        Return Reader
    End Function
    Public Sub NonQuery(ByVal commandstring As String)
        '--INSERT, UPDATE, DELETE statements to database--
        commandSQL.CommandText = commandstring
        commandSQL.Connection = connSQL
        commandSQL.Connection.Open()
        commandSQL.ExecuteNonQuery()
        If commandSQL.Connection.State = ConnectionState.Open Then commandSQL.Connection.Close()
    End Sub
End Class
