using Phoenix.Core.Services;
using Phoenix.Medicaid.Service.Logging;
using Phoenix.Models.Services.Events;

namespace Phoenix.Medicaid.Service.Services
{
    public class MedicaidBaseService : BaseService
    {
        protected ILoggingService LoggingService { get; set; }

        private IApplicationEventService _applicationEventService { get; set; }
        public IApplicationEventService ApplicationEventService
        {
            get { return _applicationEventService ?? (_applicationEventService = new ApplicationEventService()); }
        }

        private IMedicaidGLinkProcess _medicaidGLinkProcess;
        public IMedicaidGLinkProcess MedicaidGLinkProcess
        {
            get { return _medicaidGLinkProcess ?? (_medicaidGLinkProcess = new MedicaidGLinkProcess()); }
        }
    }
}
