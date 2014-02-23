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
            
            LogEvent("Starting GLink", EventTypes.Events.GLinkStarted);
            GLinkFactory.Current().SetVisible(true);
            GLinkFactory.Current().Connect();
            GLinkFactory.Current().SetVisible(true);
            Console.ReadLine();
            GLinkFactory.Current().Disconnect();
        }

        public void LogEvent(string message, EventTypes.Events eventType)
        {
            LoggingService.LogEvent(message, eventType, false);
        }
    }
}
