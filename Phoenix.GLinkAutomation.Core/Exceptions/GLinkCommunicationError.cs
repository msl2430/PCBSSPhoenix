using System;

namespace Phoenix.GLinkAutomation.Core.Exceptions
{
    public class GLinkCommunicationError : Exception
    {
        public override string Message
        {
            get { return "GLink Communication Error"; }
        }
    }
}
