using System.Diagnostics;
using Phoenix.Medicaid.Service.Logging;

namespace Phoenix.Medicaid.Service.Configuration
{
    public abstract class BaseApplication
    {
       public EventLog MedicaidEventLog { get; set; }

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
