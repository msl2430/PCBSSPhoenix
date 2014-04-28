'--Developed by Michael Levine 5/2007--
Public Class VRPInformation
    '--FAMIS VRP Information Blocks--
    Public VA, VC, VE, VG, VI, VK, VQ As FAMISBlock

    Public VM, VO As FAMISBlock_Date

    Public Sub New(ByVal VRPNumber As Integer)
        '--Intialize each block individually--
        '--Passing the string representation of the variable allows for easy modular intialization--
        '--All Block format information is found the XML document which is looked at by the FAMISBLock class--
        VA = New FAMISBlock("VA")
        VC = New FAMISBlock("VC")
        VE = New FAMISBlock("VE")
        VG = New FAMISBlock("VG")
        VI = New FAMISBlock("VI")
        VK = New FAMISBlock("VK")
        VQ = New FAMISBlock("VQ")

        VM = New FAMISBlock_Date("VM")
        VO = New FAMISBlock_Date("VO")

        '--Format VPR Data--
        '--VA and VC are blank and not used--
        VA.SetData("    ")
        VC.SetData(" ")
        '--Field numbers increment by 14 for each additional VRP--
        VE.FieldNumber += 14 * VRPNumber
        VG.FieldNumber += 14 * VRPNumber
        VI.FieldNumber += 14 * VRPNumber
        VK.FieldNumber += 14 * VRPNumber
        VQ.FieldNumber += 14 * VRPNumber
        VM.FieldNumber += 14 * VRPNumber
        VO.FieldNumber += 14 * VRPNumber
    End Sub
    Public Sub Dispose()
        '--Clean up when done--
        VA.Deconstructor()
        VC.Deconstructor()
        VE.Deconstructor()
        VG.Deconstructor()
        VI.Deconstructor()
        VK.Deconstructor()
        VQ.Deconstructor()

        VM.Deconstructor()
        VO.Deconstructor()
    End Sub

End Class
