using FluentNHibernate.Mapping;
using Phoenix.Models.Constants;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Models.Mappings.Medicaid
{
    public class MedicaidFieldMap : ClassMap<MedicaidField>
    {
        public MedicaidFieldMap()
        {
            ReadOnly();

            Schema("Phoenix");
            Table("MedicaidFields");

            Id(mf => mf.MedicaidFieldId);

            Map(mf => mf.FieldName);
            Map(mf => mf.MedicaidFormId).CustomType<FormConstants.MedicaidForms>();
            Map(mf => mf.FieldNumber);
            Map(mf => mf.FieldLength);
            Map(mf => mf.StartIndex);
            Map(mf => mf.RequiredFieldNumber).Nullable();
        }
    }
}
