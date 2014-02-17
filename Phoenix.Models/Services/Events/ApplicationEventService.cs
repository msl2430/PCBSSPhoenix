using System;
using Phoenix.Core.Constants;
using Phoenix.Models.Models.Events;
using Phoenix.Models.NHibernate;

namespace Phoenix.Models.Services.Events
{
    public interface IApplicationEventService
    {
        void SaveEvent(string message, ApplicationConstants.Applications application, EventTypes.Events eventType);
    }

    public class ApplicationEventService : IApplicationEventService
    {
        public void SaveEvent(string message, ApplicationConstants.Applications application, EventTypes.Events eventType)
        {
            var newEvent = new ApplicationEvent()
            {
                ApplicationId = (int)ApplicationConstants.Applications.PhoenixMedicaidService,
                EventTypeId = (int)EventTypes.Events.ApplicationStarted,
                Details = "Started Console App",
                EventTimestamp = DateTime.Now
            };
            NHibernateHelper.CurrentSession.Save(newEvent);
            NHibernateHelper.CurrentSession.Flush();
            NHibernateHelper.CurrentSession.Transaction.Commit();
        }
    }
}
