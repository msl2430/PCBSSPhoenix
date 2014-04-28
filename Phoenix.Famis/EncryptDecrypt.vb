'--Designed by Michael Levine 2/2008--
Imports System.Security.Cryptography
Imports System.Text

Module EncryptDecrypt
    Public Sub EncryptFile(ByVal sInputFilename As String, ByVal sOutputFilename As String, ByVal sKey As String)
        Dim fsInput As New FileStream(sInputFilename, FileMode.Open, FileAccess.Read)
        Dim fsEncrypted As New FileStream(sOutputFilename, FileMode.Create, FileAccess.Write)
        Dim DES As New DESCryptoServiceProvider()
        'Set secret key for DES algorithm.
        'A 64-bit key and an IV are required for this provider.
        DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey)
        'Set the initialization vector.
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey)
        'Create the DES encryptor from this instance.
        Dim DESEncrypt As ICryptoTransform = DES.CreateEncryptor()
        'Create the crypto stream that transforms the file stream by using DES encryption.
        Dim CryptoStream As New CryptoStream(fsEncrypted, DESEncrypt, CryptoStreamMode.Write)
        'Read the file text to the byte array.
        Dim ByteArrayInput(fsInput.Length - 1) As Byte
        fsInput.Read(ByteArrayInput, 0, ByteArrayInput.Length)
        'Write out the DES encrypted file.
        CryptoStream.Write(ByteArrayInput, 0, ByteArrayInput.Length)
        CryptoStream.Close()
        fsEncrypted.Close()
        fsInput.Close()
    End Sub
    Public Sub DecryptFile(ByVal sInputFilename As String, ByVal sOutputFilename As String, ByVal sKey As String)
        Dim DES As New DESCryptoServiceProvider()
        'A 64-bit key and an IV are required for this provider.
        'Set the secret key for the DES algorithm.
        DES.Key() = ASCIIEncoding.ASCII.GetBytes(sKey)
        'Set the initialization vector.
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey)
        'Create the file stream to read the encrypted file back.
        Dim fsRead As New FileStream(sInputFilename, FileMode.Open, FileAccess.Read)
        'Create the DES decryptor from the DES instance.
        Dim DESDecrypt As ICryptoTransform = DES.CreateDecryptor()
        'Create the crypto stream set to read and to do a DES decryption transform on incoming bytes.
        Dim CryptoStreamDecr As New CryptoStream(fsRead, DESDecrypt, CryptoStreamMode.Read)
        'Print out the contents of the decrypted file.
        Dim fsDecrypted As New StreamWriter(sOutputFilename)
        fsDecrypted.Write(New StreamReader(CryptoStreamDecr).ReadToEnd)
        fsDecrypted.Flush()
        fsDecrypted.Close()
        fsRead.Close()
    End Sub
End Module
