using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Medicaid.Service.Logging;

namespace Phoenix.Medicaid.Service.Configuration
{
    public abstract class BaseApplication
    {
        public EventLog MedicaidEventLog { get; set; }

        private ILoggingService _loggingService { get; set; }
        protected ILoggingService LoggingService
        {
            get
            {
                _loggingService = MedicaidEventLog == null ? new LoggingService() : new LoggingService(MedicaidEventLog);
                return _loggingService;
            }
        }
    }
}
