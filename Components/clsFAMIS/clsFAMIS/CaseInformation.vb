'--Developed by Michael Levine 5/2007--
Public Class CaseInformation
    '--FAMIS Case Information Blocks--
    Public AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AL, AM, AQ As FAMISBlock
    Public AK, AN As FAMISBlock_Date
    Public DATEENTERED, P03, P05, OPERATORID, P09, P10 As String

    Public Sub New()
        '--Intialize each block individually--
        '--Passing the string representation of the variable allows for easy modular intialization--
        '--All Block format information is found the XML document which is looked at by the FAMISBLock class--
        AA = New FAMISBlock("AA")
        AB = New FAMISBlock("AB")
        AC = New FAMISBlock("AC")
        AD = New FAMISBlock("AD")
        AE = New FAMISBlock("AE")
        AF = New FAMISBlock("AF")
        AG = New FAMISBlock("AG")
        AH = New FAMISBlock("AH")
        AI = New FAMISBlock("AI")
        AJ = New FAMISBlock("AJ")
        AL = New FAMISBlock("AL")
        AM = New FAMISBlock("AM")
        AQ = New FAMISBlock("AQ")

        AK = New FAMISBlock_Date("AK")
        AN = New FAMISBlock_Date("AN")
    End Sub
    Public Sub Dispose()
        '--Clean up when done--
        AB.Deconstructor()
        AC.Deconstructor()
        AD.Deconstructor()
        AE.Deconstructor()
        AH.Deconstructor()
        AI.Deconstructor()
        AJ.Deconstructor()
        AL.Deconstructor()
        AM.Deconstructor()
        AQ.Deconstructor()

        AK.Deconstructor()
        AN.Deconstructor()
    End Sub

End Class
