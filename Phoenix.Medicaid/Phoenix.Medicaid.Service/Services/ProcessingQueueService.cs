using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Models.OptForms;
using Phoenix.Medicaid.Service.Logging;
using Phoenix.Models.Constants;
using Phoenix.Models.Services.Events;

namespace Phoenix.Medicaid.Service.Services
{
    public interface IProcessingQueueService
    {
        void TaskToRun();
        bool IsRunning { get; }
        IApplicationEventService ApplicationEventService { get; }
        EventLog MedicaidEventLog { get; set; }
        void CancelTask();
    }

    public sealed class ProcessingQueueService : MedicaidBaseTaskService, IProcessingQueueService
    {
        private const int ProcessingQueueWaitTime = 5000;// 300000;

        public ProcessingQueueService(ILoggingService loggingService)
        {
            LoggingService = loggingService;
        }

        public override void TaskToRun()
        {
            if (isRunning)
                throw new Exception("Processing Queues Service already running!");

            isRunning = true;
            Task.Run(() => ProcessingQueue(CancellationTokenSource.Token));
            LoggingService.LogEvent("Processing Queue Service started", EventTypes.MedicaidEvents.ProcessingQueueStarted.ToInt(), false);
        }

        public void CancelTask()
        {
            LoggingService.LogEvent("Processing Queue Service cancelled.", EventTypes.MedicaidEvents.ProcessingQueueCancelled.ToInt(), false);
        }

        private void ProcessingQueue(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var nextOpt61 = Opt61QueueRepository.GetNextAvailableOpt61();
                if (nextOpt61 != null)
                {
                    var opt61 = new Opt61Form(
                        MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61).ToList(),
                        nextOpt61);
                    StartMedicaidCaseSubmission();
                    SubmitOpt61(opt61);
                }
                
                base.CancelTask();
                Thread.Sleep(ProcessingQueueWaitTime);
            }
            LoggingService.LogEvent("Cancelled Proecssing Queue Service .", EventTypes.MedicaidEvents.ProcessingQueueStopped.ToInt(), false);
        }

        private void StartMedicaidCaseSubmission()
        {
            LoggingService.LogEvent("Starting GLink", EventTypes.Events.GLinkStarted.ToInt(), false);
            MedicaidGLinkProcess.ConnectToMedicaid();
            LoggingService.LogEvent("GLink Connected", EventTypes.Events.GLinkConnected.ToInt(), false);
            LoggingService.LogEvent("Logging in", EventTypes.Events.GLinkConnected.ToInt(), false);
            MedicaidGLinkProcess.LoginToMedicaid("R94LEVI", "PHOENIX0");
            LoggingService.LogEvent("Logged in to Medicaid", EventTypes.MedicaidEvents.LoggingInToMedicaid.ToInt(), false);
            LoggingService.LogEvent("Submit Opt 61", EventTypes.MedicaidEvents.ProcessOpt61.ToInt(), false);
            //SubmitOpt61(new Opt61Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61).ToList()));
        }

        private void SubmitOpt61(Opt61Form opt61Form)
        {
            if (opt61Form.AddressAction.Data == "A" && (opt61Form.PersonNumber.Data == "01" || opt61Form.PersonNumber.Data == "02" || opt61Form.PersonNumber.Data == "05"))
            {
                //TODO Process as family
            }
            if (opt61Form.AlienType.Data == "4" && (opt61Form.EntryDate - DateTime.Now).TotalDays <= 59)
            {
                //TODO Hold case 
            }
            MedicaidGLinkProcess.SubmitOpt61Form(opt61Form);
        }
    }
}
