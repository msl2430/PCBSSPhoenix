using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Context;
using NHibernate.Dialect;
using Phoenix.Models.Models.Events;

namespace Phoenix.Models.NHibernate
{
    public static class NHibernateHelper
    {
        public static readonly ISessionFactory SessionFactory = null;

        static NHibernateHelper()
        {
            SessionFactory = Fluently
                .Configure()
                .Database
                (
                    MsSqlConfiguration
                        .MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("default"))
                        .Dialect<MsSql2008Dialect>()
                )
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ApplicationEvent>())
                .Cache(c => c.UseQueryCache().ProviderClass<HashtableCacheProvider>())
                .ExposeConfiguration(x => x.SetProperty("current_session_context_class", "call"))
                .ExposeConfiguration(x => x.SetProperty("show_sql", "false"))
                .ExposeConfiguration(x => x.SetProperty("connection.isolation", "ReadUncommitted"))
                .BuildConfiguration()
                .BuildSessionFactory();
        }

        public static ISession CurrentSession
        {
            get
            {
                if (!CurrentSessionContext.HasBind(SessionFactory))
                {
                    var session = SessionFactory.OpenSession();
                    session.BeginTransaction();
                    CurrentSessionContext.Bind(session);
                }

                return SessionFactory.GetCurrentSession();
            }
        }

        public static ISession OpenSession() 
        {
            return SessionFactory.OpenSession();
        }

        public static void FlushAndCommit()
        {
            SessionFactory.GetCurrentSession().Flush();
            SessionFactory.GetCurrentSession().Transaction.Commit();
            SessionFactory.GetCurrentSession().BeginTransaction();
        }
    }
}
