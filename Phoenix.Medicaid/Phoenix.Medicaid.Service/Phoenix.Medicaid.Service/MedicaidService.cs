using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service.Logging;

namespace Phoenix.Medicaid.Service
{
    partial class MedicaidService : ServiceBase
    {
        private Timer _callback;
        private bool _processRunning;
        private readonly int _interval = Int16.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"]);
        private MedicaidProcess _medicaidProcess;

        public MedicaidService()
        {
            InitializeComponent();
            _processRunning = false;
            _callback = null;
            _medicaidProcess = new MedicaidProcess() { MedicaidEventLog = medicaidEventLogger };
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
