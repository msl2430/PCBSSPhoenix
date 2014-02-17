using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Models.Models.Events
{
    public class ApplicationEvent
    {
        public virtual int ApplicationEventLogId { get; set; }
        public virtual int ApplicationId { get; set; }
        public virtual int EventTypeId { get; set; }
        public virtual string Details { get; set; }
        public virtual DateTime EventTimestamp { get; set; }
    }
}
