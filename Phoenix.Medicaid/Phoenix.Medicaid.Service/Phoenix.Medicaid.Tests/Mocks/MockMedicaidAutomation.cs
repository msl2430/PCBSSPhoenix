using Phoenix.GLinkAutomation.Core.ApplicationAutomation;

namespace Phoenix.Medicaid.Tests.Mocks
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
