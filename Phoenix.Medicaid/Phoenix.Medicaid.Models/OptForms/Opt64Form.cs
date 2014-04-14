using System.Collections.Generic;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.OptForms
{
    public sealed class Opt64Form : MedicaidOptForm
    {
        public IEnumerable<MedicaidFormField> PgmNumber { get; set; }
        public IEnumerable<MedicaidFormField> EffectiveDate { get; set; }
        public IEnumerable<MedicaidFormField> TermDate { get; set; }

        public Opt64Form(IList<MedicaidField> fields)
        {
            base.Initialize(fields);
        }
    }
}
