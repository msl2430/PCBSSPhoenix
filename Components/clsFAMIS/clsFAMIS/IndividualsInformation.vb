'--Developed by Michael Levine 5/2007--
Public Class IndividualsInformation
    '--FAMIS Individuals Information Blocks--
    Public FA, FC, FD, FD2, FE1, FE2, FF, FG, FG2, FG3, FH, FI, FK, FL, FL2, FM1, FM2, FN, FO, FP, FP2, FP3 As FAMISBlock
    Public GA, GC, GD, GE, GG, GI, GJ, GK As FAMISBlock
    Public B1, B2, B3, B4, FQ, FR As FAMISBlock
    Public FB, FJ, GB, GF, GH, GL, B5, B6 As FAMISBlock_Date

    Public Sub New()
        '--Intialize each block individually--
        '--Passing the string representation of the variable allows for easy modular intialization--
        '--All Block format information is found the XML document which is looked at by the FAMISBLock class--
        FA = New FAMISBlock("FA")
        FC = New FAMISBlock("FC")
        FD = New FAMISBlock("FD")
        FD2 = New FAMISBlock("FD2")
        FE1 = New FAMISBlock("FE1")
        FE2 = New FAMISBlock("FE2")
        FF = New FAMISBlock("FF")
        FG = New FAMISBlock("FG")
        FG2 = New FAMISBlock("FG2")
        FG3 = New FAMISBlock("FG3")
        FH = New FAMISBlock("FH")
        FI = New FAMISBlock("FI")
        FK = New FAMISBlock("FK")
        FL = New FAMISBlock("FL")
        FL2 = New FAMISBlock("FL2")
        FM1 = New FAMISBlock("FM1")
        FM2 = New FAMISBlock("FM2")
        FN = New FAMISBlock("FN")
        FO = New FAMISBlock("FO")
        FP = New FAMISBlock("FP")
        FP2 = New FAMISBlock("FP2")
        FP3 = New FAMISBlock("FP3")

        GA = New FAMISBlock("GA")
        GC = New FAMISBlock("GC")
        GD = New FAMISBlock("GD")
        GE = New FAMISBlock("GE")
        GG = New FAMISBlock("GG")
        GI = New FAMISBlock("GI")
        GJ = New FAMISBlock("GJ")
        GK = New FAMISBlock("GK")

        B1 = New FAMISBlock("B1")
        B2 = New FAMISBlock("B2")
        B3 = New FAMISBlock("B3")
        B4 = New FAMISBlock("B4")
        FQ = New FAMISBlock("FQ")
        FR = New FAMISBlock("FR")

        FB = New FAMISBlock_Date("FB")
        FJ = New FAMISBlock_Date("FJ")
        GB = New FAMISBlock_Date("GB")
        GF = New FAMISBlock_Date("GF")
        GH = New FAMISBlock_Date("GH")
        GL = New FAMISBlock_Date("GL")
        B5 = New FAMISBlock_Date("B5")
        B6 = New FAMISBlock_Date("B6")
    End Sub
    Public Sub Dispose()
        '--Clean up when done--
        FA.Deconstructor()
        FC.Deconstructor()
        FD.Deconstructor()
        FD2.Deconstructor()
        FE1.Deconstructor()
        FE2.Deconstructor()
        FF.Deconstructor()
        FG.Deconstructor()
        FG2.Deconstructor()
        FG3.Deconstructor()
        FH.Deconstructor()
        FI.Deconstructor()
        FK.Deconstructor()
        FL.Deconstructor()
        FL2.Deconstructor()
        FM1.Deconstructor()
        FM2.Deconstructor()
        FN.Deconstructor()
        FO.Deconstructor()
        FP.Deconstructor()
        FP2.Deconstructor()
        FP3.Deconstructor()

        GA.Deconstructor()
        GC.Deconstructor()
        GD.Deconstructor()
        GE.Deconstructor()
        GG.Deconstructor()
        GI.Deconstructor()
        GJ.Deconstructor()
        GK.Deconstructor()

        FB.Deconstructor()
        FJ.Deconstructor()
        GB.Deconstructor()
        GF.Deconstructor()
        GH.Deconstructor()
        GL.Deconstructor()
    End Sub

End Class
