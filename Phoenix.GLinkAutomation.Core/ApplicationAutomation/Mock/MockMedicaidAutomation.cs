﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Medicaid.Models.FormFields;

namespace Phoenix.GLinkAutomation.Core.ApplicationAutomation.Mock
{
    public class MockMedicaidAutomation : IMedicaidAutomation
    {
        public void Connect() {}

        public bool IsConnected
        {
            get { return true; }
        }

        public void Disconnect() {}
        
        public void SetVisible(bool isVisible) {}

        public void SubmitField(int fieldId, string stringToSend) {}

        public void SubmitField(MedicaidFormField field) { }

        public void TransmitPage() {}

        public void SendStringToCursorAndTransmit(string stringToSend) {}

        public string GetStringAtLocation(int x1, int y1, int x2, int y2)
        {
            if (x1 == 7 && y1 == 12 && x2 == 13 && y2 == 12) return "LOGONID";
            if (x1 == 1 && y1 == 4 && x2 == 3 && y2 == 4) return "ACF";
            return string.Empty;
        }

        public string GetField(int fieldId)
        {
            return string.Empty;
        }

        public void StartConsoleMonitor()
        {
            throw new NotImplementedException();
        }

        public void EndConsoleMonitor()
        {
            throw new NotImplementedException();
        }

        public void SendCommandKey(int key)
        {
            
        }
    }
}
