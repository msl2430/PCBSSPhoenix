using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
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
                LogEvent("Running", EventTypes.Events.BeginCaseProcess.ToInt());               
                //var opt64Form = new Opt64Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt64).ToList());
                //Console.WriteLine("Opt 64 Form Initialized {0} fields created", opt64Form.GetType().GetProperties().Count());
                //var opt66Form = new Opt66Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt66).ToList());
                //Console.WriteLine("Opt 66 Form Initialized {0} fields created", opt66Form.GetType().GetProperties().Count());
                //StartMedicaidCaseSubmission();
                var fds = new FileDiscoveryService();
                Task.Run(() => fds.TaskToRun());
                Console.WriteLine("Called FileDiscoveryTask");
                Console.ReadLine();                
                fds.CancelTask();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} (Run Medicaid Process)", DateTime.Now, ex.Message);
            }
        }

        private void StartMedicaidCaseSubmission()
        {
            LogEvent("Starting GLink", EventTypes.Events.GLinkStarted.ToInt());
            MedicaidGLinkProcess.ConnectToMedicaid();
            LogEvent("GLink Connected", EventTypes.Events.GLinkConnected.ToInt());
            LogEvent("Logging in", EventTypes.Events.GLinkConnected.ToInt());
            MedicaidGLinkProcess.LoginToMedicaid("R94LEVI", "PHOENIX0");
            LogEvent("Logged in to Medicaid", EventTypes.MedicaidEvents.LoggingInToMedicaid.ToInt());
            LogEvent("Submit Opt 61", EventTypes.MedicaidEvents.ProcessOpt61.ToInt());
            SubmitOpt61(new Opt61Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61).ToList()));
        }

        private void SubmitOpt61(Opt61Form opt61Form)
        {
            if (opt61Form.AddressAction.Data == "A" && (opt61Form.PersonNumber.Data == "01" || opt61Form.PersonNumber.Data == "02" || opt61Form.PersonNumber.Data == "05"))
            {
                //TODO Process as family
            }
            if(opt61Form.AlienType.Data == "4" && (opt61Form.EntryDate - DateTime.Now).TotalDays <= 59)
            {
                //TODO Hold case 
            }
            MedicaidGLinkProcess.SubmitOpt61Form(opt61Form);
        }

        public void LogEvent(string message, int eventType)
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
