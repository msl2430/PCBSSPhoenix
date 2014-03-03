using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Models.Services.Events;

namespace Phoenix.Medicaid.Service.Configuration
{
    public abstract class BaseService
    {
        public EventLog MedicaidEventLog { get; set; }
        
        private IApplicationEventService _applicationEventService { get; set; }
        public IApplicationEventService ApplicationEventService
        {
            get { return _applicationEventService ?? (_applicationEventService = new ApplicationEventService()); }
        }
        
    }
}
