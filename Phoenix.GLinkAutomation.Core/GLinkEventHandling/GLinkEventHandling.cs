using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glink;
using Phoenix.GLinkAutomation.Core.Constants;
using GlinkEvent = GlinkApi.GlinkEvent;

namespace Phoenix.GLinkAutomation.Core.GLinkEventHandling
{
    public interface IGLinkEventHandling
    {
        int? GetGlinkEvent();
        bool Monitor(GlinkEventCodeEnum monitorEvent);
        void CancelMonitor();
    }

    public class GLinkEventHandling : IGLinkEventHandling
    {
        private Glink.GlinkApi GLinkApi { get; set; }
        private int? GLinkEvent { get; set; }
        private bool CancelEventMonitor { get; set; }

        private const int MonitorTimeout = 60;

        public GLinkEventHandling(Glink.GlinkApi glinkApi)
        {
            GLinkApi = glinkApi;
            GLinkEvent = null;
            GLinkApi.onGlinkEvent += OnGLinkEvent;
            CancelEventMonitor = false;
        }        

        public int? GetGlinkEvent()
        {
            return GLinkEvent; 
        }

        public bool Monitor(GlinkEventCodeEnum monitorEvent)
        {
            for (var i = 0; i < MonitorTimeout; i++)
            {
                if (GLinkEvent != (int) monitorEvent && !CancelEventMonitor)
                {
                    Thread.Sleep(Events.WaitTime);
                    continue;
                }
                GLinkEvent = null;
                return true;
            }
            return false;
        }

        public void CancelMonitor()
        {
            CancelEventMonitor = true;
        }

        private void OnGLinkEvent(Glink.GlinkEvent glevent)
        {
            var eventCode = glevent.getEventCode();

            switch (eventCode)
            {
                case GlinkEventCodeEnum.GlinkEvent_STARTED:
                case GlinkEventCodeEnum.GlinkEvent_STOPPED:
                case GlinkEventCodeEnum.GlinkEvent_CONNECTED:
                case GlinkEventCodeEnum.GlinkEvent_DISCONNECTED:
                case GlinkEventCodeEnum.GlinkEvent_TURN_RECEIVED:
                case GlinkEventCodeEnum.GlinkEvent_TURN_LOST:
                case GlinkEventCodeEnum.GlinkEvent_STRING_RECEIVED:
                case GlinkEventCodeEnum.GlinkEvent_ERROR_DETECTED:
                    GLinkEvent = (int)eventCode;
                    break;
                default:
                    GLinkEvent = null;
                    break;
            }
        }
    }
}
