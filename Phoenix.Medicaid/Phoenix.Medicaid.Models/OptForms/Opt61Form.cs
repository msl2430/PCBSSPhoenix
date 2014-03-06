using System.Collections.Generic;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.OptForms
{
    public sealed class Opt61Form : MedicaidOptForm
    {
        public MedicaidFormField Office { get; set; }
        public MedicaidFormField ProviderWarning { get; set; }
        public MedicaidFormField CaseName { get; set; }
        public MedicaidFormField Address { get; set; }
        public MedicaidFormField Address2 { get; set; }
        public MedicaidFormField Address3 { get; set; }
        public MedicaidFormField Address4 { get; set; }
        public MedicaidFormField City { get; set; }
        public MedicaidFormField State { get; set; }
        public MedicaidFormField Zip { get; set; }
        public MedicaidFormField LastName { get; set; }
        public MedicaidFormField FirstName { get; set; }
        public MedicaidFormField Middle { get; set; }
        public MedicaidFormField DateOfBirth { get; set; }
        public MedicaidFormField SocialSecurity { get; set; }
        public MedicaidFormField Sex { get; set; }
        public MedicaidFormField MaritalStatus { get; set; }
        public MedicaidFormField Race { get; set; }
        public MedicaidFormField PriorCase { get; set; }
        public MedicaidFormField PriorPersonNumber { get; set; }
        public MedicaidFormField AlienType { get; set; }
        public MedicaidFormField TempDate { get; set; }
        public MedicaidFormField EffectiveDate { get; set; }
        public MedicaidFormField TermDate { get; set; }
        public MedicaidFormField AddCode { get; set; }
        public MedicaidFormField TrmCode { get; set; }
        public MedicaidFormField Pgm { get; set; }
        public MedicaidFormField Supv { get; set; }
        public MedicaidFormField Res { get; set; }
        public MedicaidFormField ExtType { get; set; }
        public MedicaidFormField PregnancyDueDate { get; set; }
        public MedicaidFormField AddressAction { get; set; }
        public MedicaidFormField PersonAction { get; set; }
        public MedicaidFormField EligSeg { get; set; }
        public MedicaidFormField Supervisor { get; set; }
        public MedicaidFormField Worker { get; set; }

        public Opt61Form(IList<MedicaidField> fields)
        {
            base.Initialize(fields);
        } 

    }
}
