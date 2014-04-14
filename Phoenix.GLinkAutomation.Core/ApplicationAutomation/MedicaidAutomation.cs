using System;
using System.Threading;
using Glink;
using Phoenix.GLinkAutomation.Core.Constants;
using Phoenix.GLinkAutomation.Core.GLinkEventHandling;
using Phoenix.Medicaid.Models.FormFields;

namespace Phoenix.GLinkAutomation.Core.ApplicationAutomation
{
    public interface IMedicaidAutomation
    {
        void Connect();
        bool IsConnected { get; }
        void Disconnect();
        void SetVisible(bool isVisible);
        void SubmitField(int fieldId, string stringToSend);
        void SubmitField(MedicaidFormField field);
        void TransmitPage();
        void SendStringToCursorAndTransmit(string stringToSend);

        string GetStringAtLocation(int x1, int y1, int x2, int y2);
        string GetField(int fieldId);
        void StartConsoleMonitor();
        void EndConsoleMonitor();
        void SendCommandKey(int key);
    }

    public class MedicaidAutomation : IMedicaidAutomation
    {
        private bool IsVisible { get; set; }
        private bool IsCancel { get; set; }
        private bool IsStarting { get; set; }
        private string SessionNamePath { get; set; }
        private Glink.GlinkApi GLinkApi { get; set; }
        private IGLinkEventHandling GLinkEventHandling { get; set; }        

        public MedicaidAutomation()
        {
            SharedInitialization();
            SessionNamePath = @"c:\GLPro\BullProd.cfg";
        }
        public MedicaidAutomation(string sessionName)
        { 
            SharedInitialization();
            SessionNamePath = sessionName;
        }
        private void SharedInitialization()
        {
            IsVisible = false;
            IsCancel = false;
            GLinkApi = new Glink.GlinkApi();
            GLinkEventHandling = new GLinkEventHandling.GLinkEventHandling(GLinkApi);
        }

        public void Connect()
        {
            IsStarting = true;
            GLinkApi.SessionName(SessionNamePath);
            GLinkApi.setVisible(IsVisible);
            GLinkApi.start();
            GLinkApi.noToolbar();
            GLinkEventHandling.Monitor(GlinkEventCodeEnum.GlinkEvent_STRING_RECEIVED);
            GLinkEventHandling.Monitor(GlinkEventCodeEnum.GlinkEvent_TURN_RECEIVED);
            IsStarting = false;
        }

        public bool IsConnected
        {
            get { return GLinkApi.isConnected(); }
        }

        public void Disconnect()
        {
            if(GLinkApi.isConnected())
                GLinkApi.Disconnect();
            GLinkApi = null;
        }

        public void SetVisible(bool isVisible)
        {
            GLinkApi.setVisible(isVisible);
        }

        public void SubmitField(int fieldId, string stringToSend)
        {
            var field = GLinkApi.getFields().item(fieldId);
            if (field != null)
                field.setString(stringToSend);
        }

        public void SubmitField(MedicaidFormField field)
        {
            var fld = GLinkApi.getFields().item(field.FieldNumber);
            if (fld != null)
                fld.setString(field.Data);
        }

        public void TransmitPage()
        {
            GLinkApi.sendCommandKey(Commands.GLinkCommands.Transmit);
            GLinkEventHandling.Monitor(GlinkEventCodeEnum.GlinkEvent_STRING_RECEIVED);
            Thread.Sleep(Events.WaitTime);
        }

        public void SendCommandKey(int key)
        {
            GLinkApi.sendCommandKey(key);
            GLinkEventHandling.Monitor(GlinkEventCodeEnum.GlinkEvent_STRING_RECEIVED);
            Thread.Sleep(Events.WaitTime);
        }

        /// <summary>
        /// Send string to cursor position and submit page
        /// </summary>
        /// <param name="stringToSend"></param>
        public void SendStringToCursorAndTransmit(string stringToSend)
        {
            GLinkApi.SendKeys(stringToSend);
            TransmitPage();
        }

        public string GetStringAtLocation(int x1, int y1, int x2, int y2)
        {
            return GLinkApi.getString(GLinkApi.GlinkPoint(x1, y1), GLinkApi.GlinkPoint(x2, y2));
        }

        public string GetField(int fieldId)
        {
            var field = GLinkApi.getFields().item(fieldId);
            return field != null ? field.getString() : string.Empty;
        }

        public void StartConsoleMonitor()
        {
            GLinkEventHandling.ConsoleMonitor();
        }

        public void EndConsoleMonitor()
        {
            GLinkEventHandling.CancelMonitor();
        }
    }
}
