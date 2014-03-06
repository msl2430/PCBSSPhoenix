using System.Collections.Generic;
using Phoenix.Models.Models.Medicaid;
using Phoenix.Models.NHibernate;

namespace Phoenix.Medicaid.Service.Services
{
    public sealed class MedicaidFormFieldService
    {
        private IList<MedicaidField> Fields { get; set; }
        private static readonly MedicaidFormFieldService Instance = new MedicaidFormFieldService();
        private MedicaidFormFieldService()
        {
            Fields = NHibernateHelper.CurrentSession.QueryOver<MedicaidField>().List<MedicaidField>();
        }

        public static MedicaidFormFieldService Current
        {
            get { return Instance; }
        }

        public IList<MedicaidField> GetMedicaidFields()
        {
            return Fields;
        }
    }
}
