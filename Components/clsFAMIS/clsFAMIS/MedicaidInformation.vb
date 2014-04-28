'--Developed by Michael Levine 5/2007--
Public Class MedicaidInformation
    '--FAMIS Medicaid Information Blocks--
    Public HA, HD, HE, HF, HG, HH, HI, HJ, HK, HL, HM, HN, HO, HP, HQ, HR, HS, HT As FAMISBlock
    Public WA, WB, WE, WH, WI, WK, WL, WM, WN, WO, WP, WQ, WR, WS, WT, WU, WV, WW As FAMISBlock
    Public XO, XP, XQ, XR, XS, XU, XV As FAMISBlock

    Public HB, HC, WC, WD, WF, WG As FAMISBlock_Date
    Public HU, HV, XT As FAMISBlock_Date

    Public Sub New()
        '--Intialize each block individually--
        '--Passing the string representation of the variable allows for easy modular intialization--
        '--All Block format information is found the XML document which is looked at by the FAMISBLock class--
        HA = New FAMISBlock("HA")
        HD = New FAMISBlock("HD")
        HE = New FAMISBlock("HE")
        HF = New FAMISBlock("HF")
        HG = New FAMISBlock("HG")
        HH = New FAMISBlock("HH")
        HI = New FAMISBlock("HI")
        HJ = New FAMISBlock("HJ")
        HK = New FAMISBlock("HK")
        HL = New FAMISBlock("HL")
        HM = New FAMISBlock("HM")
        HN = New FAMISBlock("HN")
        HO = New FAMISBlock("HO")
        HP = New FAMISBlock("HP")
        HQ = New FAMISBlock("HQ")
        HR = New FAMISBlock("HR")
        HS = New FAMISBlock("HS")
        HT = New FAMISBlock("HT")

        WA = New FAMISBlock("WA")
        WB = New FAMISBlock("WB")
        WE = New FAMISBlock("WE")
        WH = New FAMISBlock("WH")
        WI = New FAMISBlock("WI")
        WK = New FAMISBlock("WK")
        WL = New FAMISBlock("WL")
        WM = New FAMISBlock("WM")
        WN = New FAMISBlock("WN")
        WO = New FAMISBlock("WO")
        WP = New FAMISBlock("WP")
        WQ = New FAMISBlock("WQ")
        WR = New FAMISBlock("WR")
        WS = New FAMISBlock("WS")
        WT = New FAMISBlock("WT")
        WU = New FAMISBlock("WU")
        WV = New FAMISBlock("WV")
        WW = New FAMISBlock("WW")

        XO = New FAMISBlock("XO")
        XP = New FAMISBlock("XP")
        XQ = New FAMISBlock("XQ")
        XR = New FAMISBlock("XR")
        XS = New FAMISBlock("XS")
        XU = New FAMISBlock("XU")
        XV = New FAMISBlock("XV")

        HB = New FAMISBlock_Date("HB")
        HC = New FAMISBlock_Date("HC")
        WC = New FAMISBlock_Date("WC")
        WD = New FAMISBlock_Date("WD")
        WF = New FAMISBlock_Date("WF")
        WG = New FAMISBlock_Date("WG")

        HU = New FAMISBlock_Date("HU")
        HV = New FAMISBlock_Date("HV")
        XT = New FAMISBlock_Date("XT")
    End Sub
    Public Sub Dispose()
        '--Clean up when done--
        HA.Deconstructor()
        HB.Deconstructor()
        HC.Deconstructor()
        HD.Deconstructor()
        HE.Deconstructor()
        HF.Deconstructor()
        HG.Deconstructor()
        HH.Deconstructor()
        HI.Deconstructor()
        HJ.Deconstructor()
        HK.Deconstructor()
        HL.Deconstructor()
        HM.Deconstructor()
        HN.Deconstructor()
        HO.Deconstructor()
        HP.Deconstructor()
        HQ.Deconstructor()
        HR.Deconstructor()
        HS.Deconstructor()
        HT.Deconstructor()

        WA.Deconstructor()
        WB.Deconstructor()
        WC.Deconstructor()
        WD.Deconstructor()
        WE.Deconstructor()
        WF.Deconstructor()
        WG.Deconstructor()
        WH.Deconstructor()
        WI.Deconstructor()
        WK.Deconstructor()
        WL.Deconstructor()
        WM.Deconstructor()
        WN.Deconstructor()
        WO.Deconstructor()
        WP.Deconstructor()
        WQ.Deconstructor()
        WR.Deconstructor()
        WS.Deconstructor()
        WT.Deconstructor()
        WU.Deconstructor()
        WV.Deconstructor()
        WW.Deconstructor()
    End Sub

End Class

