using Phoenix.Models.Models.Medicaid;
using Phoenix.Models.NHibernate;

namespace Phoenix.Models.Repositories.Medicaid
{
    public interface IOpt61QueueRepository
    {
        Opt61Queue GetNextAvailableOpt61();
    }

    public sealed class Opt61QueueRepository : IOpt61QueueRepository
    {
        public Opt61Queue GetNextAvailableOpt61()
        {
            return NHibernateHelper.CurrentSession.QueryOver<Opt61Queue>()
                .OrderBy(opt => opt.Opt61QueueId).Asc
                .Take(1)
                .SingleOrDefault<Opt61Queue>();
        }
    }
}
