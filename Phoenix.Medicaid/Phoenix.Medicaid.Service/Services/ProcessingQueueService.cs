using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Models.OptForms;
using Phoenix.Medicaid.Service.Logging;
using Phoenix.Models.Constants;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.Medicaid.Service.Services
{
    public sealed class ProcessingQueueService : MedicaidBaseTaskService
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
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var nextOpt61 = Opt61QueueRepository.GetNextAvailableOpt61();
                    if (nextOpt61 != null)
                    {
                        var opt61 = new Opt61Form(
                            MedicaidFormFieldService.Current.GetMedicaidFields()
                                .Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61)
                                .ToList(),
                            nextOpt61);
                        StartMedicaidCaseSubmission();
                        SubmitOpt61(opt61);
                    }
                    var nextOpt66 = Opt66QuqRepository.GetNextAvailableOpt66();
                    if (nextOpt66 != null)
                    {
                        var opt66 =
                            new Opt66Form(
                                MedicaidFormFieldService.Current.GetMedicaidFields()
                                    .Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt66)
                                    .ToList(),
                                nextOpt66);
                        StartMedicaidCaseSubmission();
                        //SubmitOpt66(opt66);
                    }

                    base.CancelTask();
                    Thread.Sleep(ProcessingQueueWaitTime);
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(string.Format("Error in Processing Queue: {0}", ex.Message), ex.InnerException.Message);
            }
            LoggingService.LogEvent("Cancelled Proecssing Queue Service.", EventTypes.MedicaidEvents.ProcessingQueueStopped.ToInt(), false);
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
        }

        private void SubmitOpt61(Opt61Form opt61Form)
        {
            try
            {
                if (opt61Form.AddressAction.Data == "A" &&
                    (opt61Form.PersonNumber.Data == "01" || opt61Form.PersonNumber.Data == "02" || opt61Form.PersonNumber.Data == "05"))
                {
                    //TODO Process as family
                }
                if (opt61Form.AlienType.Data == "4" && (opt61Form.EntryDate - DateTime.Now).TotalDays <= 59)
                {
                    //TODO Hold case 
                }
                MedicaidGLinkProcess.SubmitOpt61Form(opt61Form);
            }
            catch (Exception ex)
            {
                LoggingService.LogError(string.Format("Error in Submit Opt 61: {0}", ex.Message), ex.InnerException.Message);
            }
        }
    }
}
