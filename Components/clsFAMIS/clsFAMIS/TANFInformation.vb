'--Developed by Michael Levine 5/2007--
Public Class TANFInformation
    '--FAMIS TANF Information Blocks--
    Public IA, IB, IE, IH, II, IJ, IK, IL, IM, IN1, IO, IP As FAMISBlock

    Public IC, ID, IF1, IG As FAMISBlock_Date

    Public Sub New()
        '--Intialize each block individually--
        '--Passing the string representation of the variable allows for easy modular intialization--
        '--All Block format information is found the XML document which is looked at by the FAMISBLock class--
        IA = New FAMISBlock("IA")
        IB = New FAMISBlock("IB")
        IE = New FAMISBlock("IE")
        IH = New FAMISBlock("IH")
        II = New FAMISBlock("II")
        IJ = New FAMISBlock("IJ")
        IK = New FAMISBlock("IK")
        IL = New FAMISBlock("IL")
        IM = New FAMISBlock("IM")
        IN1 = New FAMISBlock("IN1")
        IO = New FAMISBlock("IO")
        IP = New FAMISBlock("IP")

        IC = New FAMISBlock_Date("IC")
        ID = New FAMISBlock_Date("ID")
        IF1 = New FAMISBlock_Date("IF1")
        IG = New FAMISBlock_Date("IG")
    End Sub
    Public Sub Dispose()
        '--Clean up when done--
        IA.Deconstructor()
        IB.Deconstructor()
        IE.Deconstructor()
        IH.Deconstructor()
        II.Deconstructor()
        IJ.Deconstructor()
        IK.Deconstructor()
        IL.Deconstructor()
        IM.Deconstructor()
        IN1.Deconstructor()
        IO.Deconstructor()
        IP.Deconstructor()

        IC.Deconstructor()
        ID.Deconstructor()
        IF1.Deconstructor()
        IG.Deconstructor()
    End Sub

End Class
