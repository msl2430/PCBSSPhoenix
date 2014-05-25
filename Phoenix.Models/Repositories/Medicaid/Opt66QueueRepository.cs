using Phoenix.Models.Models.Medicaid;
using Phoenix.Models.NHibernate;

namespace Phoenix.Models.Repositories.Medicaid
{
    public interface IOpt66QueueRepository
    {
        Opt66Queue GetNextAvailableOpt66();
    }

    public sealed class Opt66QueueRepository : IOpt66QueueRepository
    {
        public Opt66Queue GetNextAvailableOpt66()
        {
            return NHibernateHelper.CurrentSession.QueryOver<Opt66Queue>()
                .OrderBy(opt => opt.Opt66QueueId).Asc
                .Take(1)
                .SingleOrDefault<Opt66Queue>();
        }
    }
}
