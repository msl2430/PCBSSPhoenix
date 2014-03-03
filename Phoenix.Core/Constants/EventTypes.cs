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
    }
}
