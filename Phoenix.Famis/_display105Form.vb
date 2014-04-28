Public Class _display105Form

    Private Sub FAMISCaseInformationBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Me.Validate()
        'Me.FAMISCaseInformationBindingSource.EndEdit()
        'Me.FAMISCaseInformationTableAdapter.Update(Me.Phx_FAMISDataSet.FAMISCaseInformation)

    End Sub

    Private Sub display105Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'Phx_FAMISDataSet.FAMISIandAInformation' table. You can move, or remove it, as needed.
        Me.FAMISIandAInformationTableAdapter.Fill(Me.Phx_FAMISDataSet.FAMISIandAInformation)
        'TODO: This line of code loads data into the 'Phx_FAMISDataSet.FAMISFoodStampInformation' table. You can move, or remove it, as needed.
        Me.FAMISFoodStampInformationTableAdapter.Fill(Me.Phx_FAMISDataSet.FAMISFoodStampInformation)
        'TODO: This line of code loads data into the 'Phx_FAMISDataSet.FAMISIncomeInformation' table. You can move, or remove it, as needed.
        Me.FAMISIncomeInformationTableAdapter.Fill(Me.Phx_FAMISDataSet.FAMISIncomeInformation)
        ''TODO: This line of code loads data into the 'Phx_FAMISDataSet.FAMISIndividualsInformation' table. You can move, or remove it, as needed.
        'Me.FAMISIndividualsInformationTableAdapter.Fill(Me.Phx_FAMISDataSet.FAMISIndividualsInformation)
        ''TODO: This line of code loads data into the 'Phx_FAMISDataSet.FAMISApplicantInformation' table. You can move, or remove it, as needed.
        'FAMISApplicantInformationTableAdapter.Fill(Phx_FAMISDataSet.FAMISApplicantInformation, "C112249016")
        ''TODO: This line of code loads data into the 'Phx_FAMISDataSet.FAMISCaseInformation' table. You can move, or remove it, as needed.
        'FAMISCaseInformationTableAdapter.Fill(Phx_FAMISDataSet.FAMISCaseInformation, "C112249016")
    End Sub
End Class