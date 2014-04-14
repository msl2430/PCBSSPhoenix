using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glink;

namespace Phoenix.GLinkAutomation.Core.Constants
{
    public class Commands
    {
        public const int FAMISControlField = 4;
        public class GLinkCommands
        {
            public const string Blinking = @"-536861665";
            public const string Red = @"-1073741700";
            public const int Transmit = (int)GlinkKeyEnum.GlinkKey_ENTER;
            public const int F9 = (int) GlinkKeyEnum.GlinkKey_F9;
        }
    }
}
