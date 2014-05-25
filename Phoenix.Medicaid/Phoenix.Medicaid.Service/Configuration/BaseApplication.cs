using System.Diagnostics;
using Phoenix.Medicaid.Service.Logging;
using Phoenix.Medicaid.Service.Services;

namespace Phoenix.Medicaid.Service.Configuration
{
    public abstract class BaseApplication
    {
       public EventLog MedicaidEventLog { get; set; }

       private IMedicaidBaseTaskService _processingQueueService { get; set; }
        public IMedicaidBaseTaskService ProcessingQueueService
        {
            get { return _processingQueueService ?? (_processingQueueService = new ProcessingQueueService(LoggingService));}
        }

        private IMedicaidBaseTaskService _fileDiscoveryService { get; set; }
        public IMedicaidBaseTaskService FileDiscoveryService
        {
            get { return _fileDiscoveryService ?? (_fileDiscoveryService = new FileDiscoveryService(LoggingService)); }
        }

        private IMedicaidGLinkProcess _medicaidGLinkProcess;
        public IMedicaidGLinkProcess MedicaidGLinkProcess
        {
            get { return _medicaidGLinkProcess ?? (_medicaidGLinkProcess = new MedicaidGLinkProcess()); }
        }

        private ILoggingService _loggingService { get; set; }
        protected ILoggingService LoggingService
        {
            get
            {
                _loggingService = MedicaidEventLog == null ? new LoggingService() : new LoggingService(MedicaidEventLog);
                return _loggingService;
            }
        }
    }
}
