using System.Collections.Generic;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.OptForms
{
    public sealed class Opt66Form : MedicaidOptForm
    {
        public MedicaidFormField Supv { get; set; }
        public MedicaidFormField Worker { get; set; }
        public MedicaidFormField ProgramStatus { get; set; }
        public MedicaidFormField CaseRedetDate { get; set; }
        public MedicaidFormField DisabilityRedetDate { get; set; }

        public Opt66Form(IList<MedicaidField> fields)
        {
            base.Initialize(fields);
        }

        public override void PopulateFromCsv(string record)
        {
            throw new System.NotImplementedException();
        }
    }
}
