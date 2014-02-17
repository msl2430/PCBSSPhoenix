using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service.Configuration;
using Phoenix.Medicaid.Service.Logging;

namespace Phoenix.Medicaid.Service
{
    public class MedicaidProcess : BaseApplication
    {
        public void RunMedicaidProcess()
        {
            LogEvent("Running", EventTypes.Events.BeginCaseProcess);
        }

        public void LogEvent(string message, EventTypes.Events eventType)
        {
            LoggingService.LogEvent(message, eventType, false);
        }
    }
}
