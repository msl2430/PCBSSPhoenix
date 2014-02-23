using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.GLinkAutomation.Core.Constants
{
    public class Events
    {
        public enum GLinkEvents
        {
            Start = 1,
            Stopped,
            Connected,
            Disconnected,
            TurnReceived,
            TurnLost,
            StringReceived,
            Error = 99,
        }

        public const int WaitTime = 250;
    }
}
