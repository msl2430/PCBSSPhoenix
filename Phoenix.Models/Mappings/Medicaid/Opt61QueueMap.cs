using FluentNHibernate.Mapping;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Models.Mappings.Medicaid
{
    public class Opt61QueueMap : ClassMap<Opt61Queue>
    {
        public Opt61QueueMap()
        {
            Schema("Phoenix");
            Table("Opt61Queue");

            CompositeId()
                .KeyProperty(opt => opt.CaseNumber)
                .KeyProperty(opt => opt.PersonNumber);

            Map(opt => opt.Opt61QueueId).ReadOnly();
            Map(opt => opt.BatchNumber);
            Map(opt => opt.ActionCode);
            Map(opt => opt.Office);
            Map(opt => opt.ProviderWarning);
            Map(opt => opt.CaseName);
            Map(opt => opt.Address);
            Map(opt => opt.Address2);
            Map(opt => opt.Address3);
            Map(opt => opt.Address4);
            Map(opt => opt.City);
            Map(opt => opt.State);
            Map(opt => opt.Zip);
            Map(opt => opt.LastName);
            Map(opt => opt.FirstName);
            Map(opt => opt.Middle);
            Map(opt => opt.DateOfBirth);
            Map(opt => opt.SocialSecurity);
            Map(opt => opt.Sex);
            Map(opt => opt.MaritalStatus);
            Map(opt => opt.Race);
            Map(opt => opt.PriorCase);
            Map(opt => opt.PriorPersonNumber);
            Map(opt => opt.AlienType);
            Map(opt => opt.TempDate);
            Map(opt => opt.EffectiveDate);
            Map(opt => opt.EffectiveDate2);
            Map(opt => opt.EffectiveDate3);
            Map(opt => opt.EffectiveDate4);
            Map(opt => opt.TermDate);
            Map(opt => opt.TermDate2);
            Map(opt => opt.TermDate3);
            Map(opt => opt.TermDate4);
            Map(opt => opt.AddCode);
            Map(opt => opt.AddCode2);
            Map(opt => opt.AddCode3);
            Map(opt => opt.AddCode4);
            Map(opt => opt.TrmCode);
            Map(opt => opt.TrmCode2);
            Map(opt => opt.TrmCode3);
            Map(opt => opt.TrmCode4);
            Map(opt => opt.Pgm);
            Map(opt => opt.Pgm2);
            Map(opt => opt.Pgm3);
            Map(opt => opt.Pgm4);
            Map(opt => opt.Supv);
            Map(opt => opt.Supv2);
            Map(opt => opt.Supv3);
            Map(opt => opt.Supv4);
            Map(opt => opt.Res);
            Map(opt => opt.Res2);
            Map(opt => opt.Res3);
            Map(opt => opt.Res4);
            Map(opt => opt.ExtType);
            Map(opt => opt.ExtType2);
            Map(opt => opt.ExtType3);
            Map(opt => opt.ExtType4);
            Map(opt => opt.PregnancyDueDate);
            Map(opt => opt.PregnancyDueDate2);
            Map(opt => opt.PregnancyDueDate3);
            Map(opt => opt.PregnancyDueDate4);
            Map(opt => opt.AddressAction);
            Map(opt => opt.PersonAction);
            Map(opt => opt.EligSeg);
            Map(opt => opt.Supervisor);
            Map(opt => opt.Worker);            
        }
    }
}
