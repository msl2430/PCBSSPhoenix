using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.GLinkAutomation.Core.ApplicationAutomation.Mock
{
    public class MockMedicaidAutomation : IMedicaidAutomation
    {
        public void Connect() {}

        public void Disconnect() {}
        
        public void SetVisible(bool isVisible) {}

        public void SubmitField(int fieldId, string stringToSend) {}

        public void TransmitPage() {}

        public void SendStringToCursorAndTransmit(string stringToSend) {}

        public string GetStringAtLocation(int x1, int y1, int x2, int y2)
        {
            return string.Empty;
        }

        public string GetField(int fieldId)
        {
            return string.Empty;
        }
    }
}
