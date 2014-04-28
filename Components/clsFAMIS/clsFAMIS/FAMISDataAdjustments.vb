'--Developed by Michael Levine 7/2007--
Public Class FAMISDataAdjustments
    '--Data adjusments to correct GUMP inaccuracies--
    Private FAMISCaseInformation As CaseInformation
    Private FAMISApplicationInformation As ApplicationInformation
    Private FAMISIndividualsInformation As IndividualsInformation
    Private FAMISMedicaidInformation As MedicaidInformation
    Private FAMISTANFInformation As TANFInformation
    Private FAMISIncomeInformation As IncomeInformation
    Private FAMISFoodStampInformation As FoodStampInformation
    Private FAMISIandAInformation As IandAInformation

    Private GB_Date, GH_Date, PA_Date, PI_Date, NG_Date, AK_Date, IC_Date, LC_Date, WC_Date, QK_Date As Date

    Public Sub New(ByRef CaseInfo As CaseInformation, ByRef AppInfo As ApplicationInformation, ByRef IndInfo As IndividualsInformation, ByRef MediInfo As MedicaidInformation, ByRef TANFInfo As TANFInformation, ByRef IncInfo As IncomeInformation, ByRef FoodInfo As FoodStampInformation, ByRef IandAInfo As IandAInformation)
        FAMISCaseInformation = CaseInfo
        FAMISApplicationInformation = AppInfo
        FAMISIndividualsInformation = IndInfo
        FAMISMedicaidInformation = MediInfo
        FAMISTANFInformation = TANFInfo
        FAMISIncomeInformation = IncInfo
        FAMISFoodStampInformation = FoodInfo
        FAMISIandAInformation = IandAInfo

        setDates(FAMISIndividualsInformation.GB.GetData, GB_Date)
        setDates(FAMISIndividualsInformation.GH.GetData, GH_Date)
        setDates(FAMISFoodStampInformation.NG.GetData, NG_Date)
        setDates(FAMISCaseInformation.AK.GetData, AK_Date)
        setDates(FAMISIandAInformation.PA.GetData, PA_Date)
        setDates(FAMISIandAInformation.PI.GetData, PI_Date)
        setDates(FAMISTANFInformation.IC.GetData, IC_Date)
        setDates(FAMISFoodStampInformation.LC.GetData, LC_Date)
        setDates(FAMISMedicaidInformation.WC.GetData, WC_Date)

        DataAdjustments()
        DateAdjustments()
    End Sub

    Public Sub DataAdjustments_Child(ByRef ChildInfo As CaseChild)
        With ChildInfo
            setDates(.QK.GetData, QK_Date)
            .QO.SetData("  ")
            If .RL.GetData = "@" Then .RL.SetData("R")
            If FAMISCaseInformation.AB.GetData <> "C  " Then .TB.SetData("  ")
            .TB.SetData(.TB.GetData.Substring(1, 1) & .TB.GetData.Substring(0, 1))
            If .TL.GetData.Substring(4, 4) = "1912" Or .TL.GetData.Substring(4, 4) = "2216" Or .TL.GetData.Substring(4, 4) = "2162" Or .TL.GetData.Substring(4, 4) = "2022" Or .TL.GetData.Substring(4, 4) = "2132" Or .TL.GetData.Substring(4, 4) = "2172" Or .TL.GetData.Substring(4, 4) = "2052" Then .TL.SetData("        ")
            If .TM.GetData = " 0" Or .TM.GetData = "00" Then .TM.SetData("  ")
            .TQ.SetData(" ")
            If .UF.GetData = "00" Then .UF.SetData("  ")
            If .QM.GetData = "777" Then .UB.SetData("Z7")
            If FAMISTANFInformation.IA.GetData = "6" Or FAMISTANFInformation.IA.GetData = "2" Or FAMISTANFInformation.IA.GetData = " " Then
                If FAMISTANFInformation.IA.GetData <> "1" Then .TI.SetData(" ")
            End If
            If .QL.GetData = "R" And QK_Date < AK_Date Then
                .QL.SetData(" ")
                .QK.SetData("        ")
            End If
            If .TM.GetData = "0 " Or .TM.GetData = " 0" Or .TM.GetData = "00" Then .TM.SetData("  ")
            If .TS.GetData = "00" Then .TS.SetData("  ")
            If .TP.GetData = "0" Then .TP.SetData(" ")
            If QK_Date <> Nothing Then
                If DateDiff(DateInterval.Month, QK_Date, Date.Now.AddMonths(2)) > 23 Then
                    .QK.SetData("        ")
                    .QL.SetData(" ")
                End If
            End If
        End With
    End Sub
    Public Sub del_LineN(ByRef FoodInfo As FoodStampInformation)
        With FoodInfo
            .NA.SetData("        ")
            .NB.SetData("       ")
            .NC.SetData("        ")
            .ND.SetData(" ")
            .NE.SetData(" ")
            .NF.SetData(" ")
            .NG.SetData("        ")
            .NH.SetData("       ")
            .NI.SetData("        ")
            .NJ.SetData("        ")
        End With
    End Sub
    Public Sub del_LineP(ByRef IandAInfo As IandAInformation, ByVal sidePA As Boolean, ByVal sidePI As Boolean)
        With IandAInfo
            If sidePA Then
                .PA.SetData("        ")
                .PB.SetData("  ")
                .PC.SetData(" ")
                .PD.SetData("      ")
                .PE.SetData("       ")
                .PF.SetData(" ")
            End If
            If sidePI Then
                .PI.SetData("        ")
                .PJ.SetData("  ")
                .PK.SetData(" ")
                .PL.SetData("      ")
                .PM.SetData("       ")
                .PN.SetData(" ")
            End If
        End With
    End Sub
    Public Sub del_LineW(ByRef MediInfo As MedicaidInformation)
        With MediInfo
            .WA.SetData(" ")
            .WB.SetData("   ")
            .WC.SetData("        ")
            .WD.SetData("        ")
            .WE.SetData(" ")
            .WF.SetData("        ")
            .WG.SetData("        ")
        End With
    End Sub
    Public Sub del_LineL(ByRef FoodInfo As FoodStampInformation)
        With FoodInfo
            .LA.SetData(" ")
            .LB.SetData("   ")
            .LC.SetData("        ")
            .LD.SetData("        ")
            .LE.SetData("        ")
            .LF.SetData("  ")
            .LG.SetData(" ")
            .LH.SetData(" ")
            .LI.SetData(" ")
            .LJ.SetData("  ")
            .LK.SetData("  ")
            .LL.SetData(" ")
            .LM.SetData(" ")
            .LO.SetData(" ")
            .LP.SetData("   ")
            .LQ.SetData("   ")
            .LR.SetData(" ")
            .LT.SetData("        ")
        End With
    End Sub
    Public Sub del_LineI(ByRef TANFInfo As TANFInformation)
        With TANFInfo
            .IA.SetData(" ")
            .IB.SetData("   ")
            .IC.SetData("        ")
            .ID.SetData("        ")
            .IE.SetData(" ")
            .IF1.SetData("        ")
            .IG.SetData("        ")
            .IH.SetData("  ")
            .II.SetData(" ")
            .IJ.SetData("  ")
            .IK.SetData("       ")
            .IL.SetData(" ")
            .IM.SetData("       ")
            .IN1.SetData("        ")
            .IO.SetData("        ")
            .IP.SetData("       ")
        End With
    End Sub

    Private Sub setDates(ByRef dateString As String, ByRef tempDate As Date)
        If dateString <> "        " And dateString <> "00000000" Then
            dateString = dateString.Insert(2, "/")
            dateString = dateString.Insert(5, "/")
            tempDate = dateString
        Else
            tempdate = Nothing
        End If
    End Sub
    Private Sub DataAdjustments()
        '--Multiple Changes--
        If FAMISCaseInformation.AB.GetData = "NPA" Or FAMISCaseInformation.AB.GetData = "NP6" Or FAMISCaseInformation.AB.GetData = "NP7" Then
            FAMISFoodStampInformation.NN.SetData(" ")
            FAMISFoodStampInformation.OM.SetData(" ")
            If FAMISTANFInformation.IA.GetData <> "1" Then
                FAMISIndividualsInformation.FG.SetData(" ")
                FAMISIndividualsInformation.FP.SetData(" ")
            End If
        End If
        If FAMISCaseInformation.AB.GetData = "NPA" Or FAMISCaseInformation.AB.GetData = "NP6" Or FAMISCaseInformation.AB.GetData = "NP7" Or FAMISCaseInformation.AB.GetData = "NP8" Then
            FAMISIncomeInformation.JN.SetData("       ")
            FAMISIncomeInformation.JO.SetData("       ")
        End If
        If FAMISIndividualsInformation.FA.GetData = "6" Or FAMISIndividualsInformation.GC.GetData = "6" Or FAMISTANFInformation.IA.GetData = "6" Or FAMISTANFInformation.IA.GetData = "2" Or FAMISTANFInformation.IA.GetData = " " Then
            If FAMISTANFInformation.IA.GetData <> "1" Then FAMISIndividualsInformation.FG.SetData(" ")
        End If
        If FAMISIndividualsInformation.FI.GetData = "6" Or FAMISIndividualsInformation.GI.GetData = "6" Then
            If FAMISTANFInformation.IA.GetData <> "1" Then FAMISIndividualsInformation.FP.SetData(" ")
        End If
        If FAMISCaseInformation.AL.GetData = "       " Then
            FAMISFoodStampInformation.OL.SetData("       ")
            FAMISIandAInformation.PO.SetData("       ")
            FAMISApplicationInformation.ED1.SetData("    ")
            FAMISApplicationInformation.ED2.SetData("    ")
        End If
        If (FAMISIandAInformation.PA.GetData() = "          " Or FAMISIandAInformation.PA.GetData() = "        ") And (FAMISIandAInformation.PI.GetData() = "          " Or FAMISIandAInformation.PI.GetData = "        ") Then
            del_LineP(FAMISIandAInformation, True, True)
        End If
        If FAMISTANFInformation.IA.GetData = "A" Then
            FAMISTANFInformation.IF1.SetData("-")
            FAMISTANFInformation.IG.SetData("-")
        End If
        If FAMISFoodStampInformation.LA.GetData = "A" Then
            FAMISFoodStampInformation.LE.SetData("-")
            FAMISFoodStampInformation.LF.SetData("-")
        End If

        '--Single Changes--
        If FAMISApplicationInformation.BR.GetData = " Y" Or FAMISApplicationInformation.BR.GetData = " 1" Then FAMISApplicationInformation.BR.SetData("  ")
        If FAMISIndividualsInformation.FD.GetData = "MU" Or FAMISIndividualsInformation.FD.GetData = "0 " Or FAMISIndividualsInformation.FD.GetData = " 0" Then FAMISIndividualsInformation.FD.SetData("  ")
        If FAMISTANFInformation.IM.GetData = "       " And FAMISTANFInformation.IN1.GetData = "        " Then FAMISTANFInformation.IL.SetData(" ")
        If FAMISApplicationInformation.EL.GetData = "999999" Then FAMISApplicationInformation.EL.SetData("      ")
        If FAMISApplicationInformation.EN.GetData = "999999" Then FAMISApplicationInformation.EN.SetData("      ")
        If FAMISFoodStampInformation.LN.GetData = "E" Then FAMISFoodStampInformation.LN.SetData(" ")
        If FAMISMedicaidInformation.HB.GetData = "-" Then FAMISMedicaidInformation.HB.SetData("-     ")
        If FAMISMedicaidInformation.HC.GetData = "-" Then FAMISMedicaidInformation.HC.SetData("-     ")
        If FAMISIncomeInformation.JO.GetData = " " Then FAMISIncomeInformation.JO.SetData("       ")
        If FAMISFoodStampInformation.MC.GetData = "-" Then FAMISFoodStampInformation.MC.SetData("-      ")
        If FAMISApplicationInformation.ED1.GetData = "0000" Then FAMISApplicationInformation.ED1.SetData("    ")
        If FAMISApplicationInformation.ED2.GetData = "0000" Then FAMISApplicationInformation.ED2.SetData("    ")
        If FAMISApplicationInformation.XB.GetData = " 0" Or FAMISApplicationInformation.XB.GetData = "0 " Or FAMISApplicationInformation.XB.GetData = "00" Then FAMISApplicationInformation.XB.SetData("  ")
        If FAMISApplicationInformation.XH.GetData = " 0" Or FAMISApplicationInformation.XH.GetData = "0 " Or FAMISApplicationInformation.XB.GetData = "00" Then FAMISApplicationInformation.XH.SetData("  ")
        If FAMISIndividualsInformation.FO.GetData = "4" Or FAMISIndividualsInformation.FO.GetData = "0" Then FAMISIndividualsInformation.FO.SetData(" ")
        If FAMISApplicationInformation.XC.GetData = "00" Then FAMISApplicationInformation.XC.SetData("  ")
        If FAMISApplicationInformation.XD.GetData = "00" Then FAMISApplicationInformation.XD.SetData("  ")
        If FAMISApplicationInformation.XE.GetData = "0" Then FAMISApplicationInformation.XE.SetData(" ")
        If FAMISApplicationInformation.XM.GetData = "00" Then FAMISApplicationInformation.XM.SetData("  ")
        If FAMISApplicationInformation.XI.GetData = "00" Then FAMISApplicationInformation.XI.SetData("  ")
        If FAMISApplicationInformation.XJ.GetData = "00" Then FAMISApplicationInformation.XJ.SetData("  ")
        If FAMISApplicationInformation.XK.GetData = "0" Then FAMISApplicationInformation.XK.SetData(" ")
        If FAMISApplicationInformation.XN.GetData = "00" Then FAMISApplicationInformation.XN.SetData("  ")
        FAMISFoodStampInformation.NM.SetData("       ")
        If FAMISTANFInformation.IA.GetData = " " And FAMISTANFInformation.IB.GetData = "   " And FAMISTANFInformation.IC.GetData = "        " Then FAMISIncomeInformation.JI.SetData(" ") : FAMISIncomeInformation.KI.SetData(" ")
        If FAMISMedicaidInformation.WW.GetData = "0  " Or FAMISMedicaidInformation.WW.GetData = "  0" Or _
           FAMISMedicaidInformation.WW.GetData = "NPA" Or FAMISMedicaidInformation.WW.GetData = "NP7" Or _
           FAMISMedicaidInformation.WW.GetData = "NP8" Then FAMISMedicaidInformation.WW.SetData("   ")
        'If FAMISApplicationInformation.DF.GetData.Substring(0, 1) <> "0" And FAMISApplicationInformation.DF.GetData.Substring(0, 1) <> " " Then FAMISApplicationInformation.DF.SetData("0" & FAMISApplicationInformation.DF.GetData.Substring(0, 2))
        If FAMISFoodStampInformation.LR.GetData = "D" Then FAMISFoodStampInformation.MR.SetData("454") '--changed from 411 3/22/11-- --changed from 435 9/12/2012--
        If FAMISApplicationInformation.EG.GetData = "1" Or _
           FAMISApplicationInformation.EG.GetData = "C" Or _
           FAMISApplicationInformation.EG.GetData = "I" Or _
           FAMISApplicationInformation.EG.GetData = "S" Or _
           FAMISApplicationInformation.EG.GetData = "T" Then
            FAMISApplicationInformation.EG.SetData(" ")
        End If
        If FAMISFoodStampInformation.LE.GetData = Date.Now.Month & "/1/" & Date.Now.Year Or FAMISFoodStampInformation.LE.GetData = Date.Now.AddMonths(1).Month & "/1/" & Date.Now.AddMonths(1).Year Then FAMISApplicationInformation.EG.SetData("-")
        If FAMISApplicationInformation.CE.GetData.Substring(5, 4) = "0000" Then FAMISApplicationInformation.CE.SetData(FAMISApplicationInformation.CE.GetData.Substring(0, 5))
        If FAMISApplicationInformation.DE.GetData.Substring(5, 4) = "0000" Then FAMISApplicationInformation.DE.SetData(FAMISApplicationInformation.DE.GetData.Substring(0, 5))

        '--Unique Changes--
        If FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "1912" Or FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "2216" Or FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "2162" Or FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "2022" Or FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "2132" Or FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "2172" Or FAMISApplicationInformation.XA.GetData.Substring(4, 4) = "2052" Then FAMISApplicationInformation.XA.SetData(FAMISTANFInformation.ID.GetData)
        If FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "1912" Or FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "2216" Or FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "2162" Or FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "2022" Or FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "2132" Or FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "2172" Or FAMISApplicationInformation.XG.GetData.Substring(4, 4) = "2052" Then FAMISApplicationInformation.XG.SetData(FAMISTANFInformation.ID.GetData)

        If FAMISIndividualsInformation.FC.GetData.Substring(0, 3) = "777" Or FAMISIndividualsInformation.FC.GetData.Substring(0, 3) = "888" Then FAMISApplicationInformation.BT.SetData("Z7")
        If FAMISIndividualsInformation.FK.GetData.Substring(0, 3) = "777" Or FAMISIndividualsInformation.FK.GetData.Substring(0, 3) = "888" Then FAMISApplicationInformation.BX.SetData("Z7")
    End Sub
    Private Sub DateAdjustments()
        If GB_Date <> Nothing Then
            If DateDiff(DateInterval.Month, GB_Date, Date.Now.AddMonths(2)) > 25 Then
                With FAMISIndividualsInformation
                    .GB.SetData("        ")
                    .GC.SetData(" ")
                End With
            End If
        End If
        If GH_Date <> Nothing Then
            If DateDiff(DateInterval.Month, GH_Date, Date.Now.AddMonths(2)) > 25 Then
                With FAMISIndividualsInformation
                    .GH.SetData("        ")
                    .GI.SetData(" ")
                End With
            End If
        End If

        If NG_Date <> Nothing And AK_Date <> Nothing Then
            If NG_Date <= AK_Date Then del_LineN(FAMISFoodStampInformation)
        End If
        If PA_Date <> Nothing And AK_Date <> Nothing Then
            If PA_Date <= AK_Date Then del_LineP(FAMISIandAInformation, True, False)
        End If
        If PI_Date <> Nothing And AK_Date <> Nothing Then
            If PI_Date <= AK_Date Then del_LineP(FAMISIandAInformation, False, True)
        End If

        If IC_Date <> Nothing And AK_Date <> Nothing Then 'And FAMISTANFInformation.IC.GetData <> "01012006" Then
            If IC_Date < AK_Date Then
                If FAMISCaseInformation.AB.GetData <> "NP8" And FAMISTANFInformation.IB.GetData <> "PT " Then
                    If PA_Date < AK_Date And (FAMISIandAInformation.PB.GetData <> "01" Or FAMISIandAInformation.PB.GetData <> "33") And FAMISTANFInformation.IA.GetData <> "1" Then del_LineI(FAMISTANFInformation)
                End If
            End If
        End If
        If LC_Date <> Nothing And AK_Date <> Nothing Then 'And FAMISFoodStampInformation.LC.GetData <> "01012006" And FAMISFoodStampInformation.LC.GetData <> "02012006" Then
            If LC_Date < AK_Date And FAMISFoodStampInformation.LA.GetData <> "1" Then del_LineL(FAMISFoodStampInformation)
        End If
        If WC_Date <> Nothing And AK_Date <> Nothing Then 'And FAMISMedicaidInformation.WC.GetData <> "01012006" Then
            If WC_Date < AK_Date Then
                If FAMISMedicaidInformation.WB.GetData = "EC" And (FAMISIndividualsInformation.GE.GetData = "A" Or FAMISIndividualsInformation.GI.GetData = "B") And FAMISMedicaidInformation.WA.GetData <> "M" Then del_LineW(FAMISMedicaidInformation)
            End If
        End If
    End Sub
End Class
