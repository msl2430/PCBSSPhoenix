using System.Collections.Generic;
using System.Linq;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.OptForms
{
    public class MedicaidOptForm
    {
        public MedicaidFormField ActionCode { get; set; }
        public MedicaidFormField CaseNumber { get; set; }
        public MedicaidFormField PersonNumber { get; set; }
        public MedicaidFormField BatchNumber { get; set; }

        public void Initialize(IList<MedicaidField> fields)
        {
            var properties = GetType().GetProperties();
            foreach (var propInfo in properties)
            {
                var field = fields.FirstOrDefault(f => f.FieldName == propInfo.Name);
                if (field != null)
                {
                    if (propInfo.PropertyType == typeof(IEnumerable<MedicaidFormField>))
                    {
                        propInfo.SetValue(this, new List<MedicaidFormField>(), null);
                    }
                    else
                    {
                        propInfo.SetValue(this, new MedicaidFormField(field), null);
                    }
                }
                
            }
        }
    }
}
