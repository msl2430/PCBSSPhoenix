using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service.Configuration;
using Phoenix.Medicaid.Service.Factories;

namespace Phoenix.Medicaid.Service
{
    public interface IMedicaidGLinkProcess
    {
        void ConnectToMedicaid();
        void StartConsoleMonitorThread();
        void EndConsoleMonitorThread();
    }

    public class MedicaidGLinkProcess : IMedicaidGLinkProcess
    {
        private IGLinkFactory _glinkFactory { get; set; }
        private IGLinkFactory GLinkFactory { get { return _glinkFactory ?? (_glinkFactory = new GLinkFactory()); } }

        public void ConnectToMedicaid()
        {
            try
            {
                GLinkFactory.MedicaidAutomation.Connect();
                GLinkFactory.MedicaidAutomation.SetVisible(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} (ConnectingToMedicaid)", DateTime.Now, ex.Message);
            }
        }

        public void StartConsoleMonitorThread()
        {
            try
            {
                GLinkFactory.MedicaidAutomation.StartConsoleMonitor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} (StartConsoleMonitorThread)", DateTime.Now, ex.Message);
            }
        }

        public void EndConsoleMonitorThread()
        {
            try
            {
                GLinkFactory.MedicaidAutomation.EndConsoleMonitor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} (EndConsoleMonitorThread)", DateTime.Now, ex.Message);
            }
        }
    }
}
