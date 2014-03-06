using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Models;
using Phoenix.Medicaid.Models.OptForms;
using Phoenix.Medicaid.Service.Configuration;
using Phoenix.Medicaid.Service.Services;
using Phoenix.Models.Constants;

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
                MedicaidGLinkProcess.ConnectToMedicaid();
                LogEvent("GLink Connected", EventTypes.Events.GLinkConnected);
                var opt61Form = new Opt61Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61).ToList());
                Console.WriteLine("Opt 61 Form Initialized {0} fields created", opt61Form.GetType().GetProperties().Count());
                var opt64Form = new Opt64Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt64).ToList());
                Console.WriteLine("Opt 64 Form Initialized {0} fields created", opt64Form.GetType().GetProperties().Count());
                var opt66Form = new Opt66Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt66).ToList());
                Console.WriteLine("Opt 66 Form Initialized {0} fields created", opt66Form.GetType().GetProperties().Count());
                Console.ReadLine();                
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} (Run Medicaid Process)", DateTime.Now, ex.Message);
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
            MedicaidGLinkProcess.StartConsoleMonitorThread();
            while (!worker.CancellationPending)
            {
                Thread.Sleep(1000);
            }
            MedicaidGLinkProcess.EndConsoleMonitorThread();
        }
    }
}
