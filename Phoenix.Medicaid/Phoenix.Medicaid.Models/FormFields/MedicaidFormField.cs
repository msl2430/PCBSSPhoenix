using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.FormFields
{
    public class MedicaidFormField
    {
        public string FieldName { get; set; }
        public int FieldNumber { get; set; }
        public int Length { get; set; }
        public string Data { get; set; }
        /// <summary>
        /// Field needed to input into Medicaid
        /// </summary>
        public MedicaidFormField RequiredField { get; set; }

        public MedicaidFormField(MedicaidField field)
        {
            FieldName = field.FieldName;
            FieldNumber = field.FieldNumber;
            Length = field.FieldLength;
            Data = " ".PadRight(Length, ' ');
            RequiredField = null;
        }

        public bool IsFieldEmpty()
        {
            return string.IsNullOrEmpty(Data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MedicaidFormField) obj);
        }

        protected bool Equals(MedicaidFormField other)
        {
            return string.Equals(FieldName, other.FieldName) && FieldNumber == other.FieldNumber && Length == other.Length && string.Equals(Data, other.Data);
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
