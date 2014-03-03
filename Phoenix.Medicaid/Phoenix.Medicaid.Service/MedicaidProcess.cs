using System;
using System.ComponentModel;
using System.Threading;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service.Configuration;

namespace Phoenix.Medicaid.Service
{
    public class MedicaidProcess : BaseApplication
    {
        private BackgroundWorker BgWorker { get; set; }

        public void RunMedicaidProcess()
        {
            try
            {
                LogEvent("Running", EventTypes.Events.BeginCaseProcess);

                LogEvent("Starting GLink", EventTypes.Events.GLinkStarted);
                GLinkFactory.Current().Connect();
                GLinkFactory.Current().SetVisible(true);
                LogEvent("GLink Connected", EventTypes.Events.GLinkConnected);
                Console.ReadLine();
                GLinkFactory.Current().Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} ({2})", DateTime.Now, ex.Message, ex.Message);
            }
        }

        public void LogEvent(string message, EventTypes.Events eventType)
        {
            LoggingService.LogEvent(message, eventType, false);
        }

        private void StartMonitorThread()
        {
            BgWorker = new BackgroundWorker();
            BgWorker.DoWork += ConsoleMonitorThread;
            BgWorker.RunWorkerAsync();
        }

        private void ConsoleMonitorThread(object sender, DoWorkEventArgs arg)
        {
            var worker = sender as BackgroundWorker;
            GLinkFactory.Current().StartConsoleMonitor();
            while (!worker.CancellationPending)
            {
                Thread.Sleep(1000);
            }
            GLinkFactory.Current().EndConsoleMonitor();
        }
    }
}
