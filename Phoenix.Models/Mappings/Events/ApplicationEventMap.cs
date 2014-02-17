using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Phoenix.Models.Models.Events;

namespace Phoenix.Models.Mappings.Events
{
    public class ApplicationEventMap : ClassMap<ApplicationEvent>
    {
        public ApplicationEventMap()
        {
            Schema("Phoenix");
            Table("ApplicationEvents");

            Id(ae => ae.ApplicationEventLogId);

            Map(ae => ae.ApplicationId);
            Map(ae => ae.EventTypeId);
            Map(ae => ae.Details);
            Map(ae => ae.EventTimestamp);
        }
    }
}
