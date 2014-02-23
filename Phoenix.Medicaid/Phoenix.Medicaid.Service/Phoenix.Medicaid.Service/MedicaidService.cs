using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service.Factories;

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
                                   GLinkFactory = new GLinkFactory(Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"]))
                               };
        }

        protected override void OnStart(string[] args)
        {
            _medicaidProcess.LogEvent("Phoenix Medicaid service started", EventTypes.Events.ApplicationStarted);
            _processRunning = false;
            Thread.Sleep(5000);
            _callback = new Timer(RunMedicaidProcess, null, 5000, _interval);
        }

        protected override void OnStop()
        {
            _medicaidProcess.LogEvent("Phoenix Medicaid service started", EventTypes.Events.ApplicationStarted);
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
