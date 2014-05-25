using FluentNHibernate.Mapping;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Models.Mappings.Medicaid
{
    public sealed class Opt66QueueMap : ClassMap<Opt66Queue>
    {
        public Opt66QueueMap()
        {
            Schema("Phoenix");
            Table("Opt66Queue");

            CompositeId()
                .KeyProperty(opt => opt.CaseNumber)
                .KeyProperty(opt => opt.PersonNumber);

            Map(opt => opt.Opt66QueueId).ReadOnly();
            Map(opt => opt.Supv);
            Map(opt => opt.Worker);
            Map(opt => opt.ProgramStatus);
            Map(opt => opt.CaseRedetDate);
            Map(opt => opt.DisabilityRedetDate);
            Map(opt => opt.ActionCode);
        }
    }
}
