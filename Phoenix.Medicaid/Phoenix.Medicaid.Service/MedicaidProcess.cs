using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Service.Configuration;
using System;
using System.Threading.Tasks;

namespace Phoenix.Medicaid.Service
{
    public class MedicaidProcess : BaseApplication
    {
        public void RunMedicaidProcess()
        {

            LogEvent("Running", EventTypes.Events.BeginCaseProcess.ToInt());
            try
            {
                Task.Run(() => FileDiscoveryService.TaskToRun());
                Task.Run(() => ProcessingQueueService.TaskToRun());
            }
            catch (Exception ex)
            {
                LoggingService.LogError(string.Format("Error in Medicaid Process: {0}", ex.Message), ex.InnerException.Message);
            }
            
            Console.ReadLine();
            FileDiscoveryService.CancelTask();
            ProcessingQueueService.CancelTask();
        }

        public void LogEvent(string message, int eventType)
        {
            LoggingService.LogEvent(message, eventType, false);
        }
    }
}
