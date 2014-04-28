'--Developed by Michael Levine 5/2007--
Public Class IandAInformation
    '--FAMIS I and A Information Blocks--
    Public PB, PC, PD, PE, PF, PG, PH, PJ, PK, PL, PM, PN, PO, PP As FAMISBlock

    Public PA, PI As FAMISBlock_Date

    Public Sub New()
        '--Intialize each block individually--
        '--Passing the string representation of the variable allows for easy modular intialization--
        '--All Block format information is found the XML document which is looked at by the FAMISBLock class--
        PB = New FAMISBlock("PB")
        PC = New FAMISBlock("PC")
        PD = New FAMISBlock("PD")
        PE = New FAMISBlock("PE")
        PF = New FAMISBlock("PF")
        PG = New FAMISBlock("PG")
        PH = New FAMISBlock("PH")
        PJ = New FAMISBlock("PJ")
        PK = New FAMISBlock("PK")
        PL = New FAMISBlock("PL")
        PM = New FAMISBlock("PM")
        PN = New FAMISBlock("PN")
        PO = New FAMISBlock("PO")
        PP = New FAMISBlock("PP")

        PA = New FAMISBlock_Date("PA")
        PI = New FAMISBlock_Date("PI")
    End Sub
    Public Sub Dispose()
        '--Clean up when done--
        PB.Deconstructor()
        PC.Deconstructor()
        PD.Deconstructor()
        PE.Deconstructor()
        PF.Deconstructor()
        PG.Deconstructor()
        PH.Deconstructor()
        PJ.Deconstructor()
        PK.Deconstructor()
        PL.Deconstructor()
        PM.Deconstructor()
        PN.Deconstructor()
        PO.Deconstructor()
        PP.Deconstructor()

        PA.Deconstructor()
        PI.Deconstructor()
    End Sub

End Class
