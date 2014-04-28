'--Developed by Michael Levine-- 2/2008
Module GlobalVariables

    Friend Const isSecurityAvailable As Boolean = True '--If the county is using security or not--

    '--Module to hold variables used across all forms--
    Public FAMISCaseInformation, SQLCaseInformation As CaseInformation
    Public FAMISApplicationInformation, SQLApplicationInformation As ApplicationInformation
    Public FAMISIndividualsInformation, SQLIndividualsInformation As IndividualsInformation
    Public FAMISMedicaidInformation, SQLMedicaidInformation As MedicaidInformation
    Public FAMISTANFInformation, SQLTANFInformation As TANFInformation
    Public FAMISIncomeInformation, SQLIncomeInformation As IncomeInformation
    Public FAMISFoodStampInformation, SQLFoodStampInformation As FoodStampInformation
    Public FAMISIandAInformation, SQLIandAInformation As IandAInformation
    Public FAMISVRPInformation(35), SQLVRPInformation(35) As VRPInformation
    Public FAMISCaseChild(35), SQLCaseChild(35) As CaseChild
    Public FAMISAdjustments As FAMISDataAdjustments
    Public numChildren, numVRP As Integer
End Module
