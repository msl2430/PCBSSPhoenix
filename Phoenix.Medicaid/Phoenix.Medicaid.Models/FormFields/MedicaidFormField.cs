using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.FormFields
{
    public class MedicaidFormField
    {
        public string FieldName { get; set; }
        public int FieldNumber { get; set; }
        public int Length { get; set; }
        public string Data { get; set; }

        public MedicaidFormField(MedicaidField field)
        {
            FieldName = field.FieldName;
            FieldNumber = field.FieldNumber;
            Length = field.FieldLength;
            Data = " ".PadRight(Length, ' ');
        }
    }
}
