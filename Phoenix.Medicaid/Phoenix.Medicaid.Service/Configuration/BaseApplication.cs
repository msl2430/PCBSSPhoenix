using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Medicaid.Service.Factories;
using Phoenix.Medicaid.Service.Logging;
using Phoenix.Medicaid.Service.Services;

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

        //private IMedicaidFormFieldService _medicaidFormFieldService { get; set; }
        //protected IMedicaidFormFieldService MedicaidFormFieldService
        //{
        //    get { return _medicaidFormFieldService = _medicaidFormFieldService ?? (_medicaidFormFieldService = new MedicaidFormFieldService()); }
        //}
    }
}
