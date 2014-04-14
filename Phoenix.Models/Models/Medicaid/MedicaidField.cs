using Phoenix.Models.Constants;

namespace Phoenix.Models.Models.Medicaid
{
    /// <summary>
    /// Phoenix.MedicaidField
    /// </summary>
    public class MedicaidField
    {
        public virtual int MedicaidFieldId { get; set; }

        public virtual FormConstants.MedicaidForms MedicaidFormId { get; set; }

        public virtual string FieldName { get; set; }

        public virtual int FieldNumber { get; set; }

        public virtual int FieldLength { get; set; }

        public virtual int StartIndex { get; set; }

        public virtual int? RequiredFieldNumber { get; set; }
    }
}
