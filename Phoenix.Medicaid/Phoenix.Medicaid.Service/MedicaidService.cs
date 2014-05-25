using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;

namespace Phoenix.Medicaid.Service
{
    partial class MedicaidService : ServiceBase
    {
        private Timer _callback;
        private bool _processRunning;
        private readonly int _interval = Int16.Parse(ConfigurationManager.AppSettings["TimerInterval"]);
        private readonly MedicaidProcess _medicaidProcess;    

        public MedicaidService()
        {
            InitializeComponent();
            _processRunning = false;
            _callback = null;            
            _medicaidProcess = new MedicaidProcess()
                               {
                                   MedicaidEventLog = medicaidEventLogger,
                               };
        }

        protected override void OnStart(string[] args)
        {
            _medicaidProcess.LogEvent("Phoenix Medicaid service started", EventTypes.Events.ApplicationStarted.ToInt());
            _processRunning = false;
            Thread.Sleep(5000);
            _callback = new Timer(RunMedicaidProcess, null, 5000, _interval);
        }

        protected override void OnStop()
        {
            _medicaidProcess.LogEvent("Phoenix Medicaid service stopped", EventTypes.Events.ApplicationStopped.ToInt());
            _callback.Dispose();
        }

        protected void RunMedicaidProcess(object state)
        {
            try
            {
                if (_processRunning) return;
                _processRunning = true;
                _medicaidProcess.RunMedicaidProcess();
                _processRunning = false;
            }
            catch (Exception ex)
            {
                Stop();
                throw;
            }
        }
    }
}
