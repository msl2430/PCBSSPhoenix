using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Core.Constants
{
    public static class EventTypes
    {
        public enum Events
        {
            ApplicationStarted = 100,
            BeginCaseProcess = 101,

            //GLink Automation Events
            GLinkStarted = 200,
            GLinkConnected = 201,

            ApplicationStopped = 900
        }

        public enum MedicaidEvents
        {
            FileDiscoveryStarted = 1000,
            FileDiscoveryCancelled = 1001,
            FileDiscoveryStopped = 1002,
            FileDiscoveryFilesFound = 1003,
            LoggingInToMedicaid = 2000,
            ProcessOpt61 = 2001,
            AddCaseToQueue = 2002
        }
    }
}
