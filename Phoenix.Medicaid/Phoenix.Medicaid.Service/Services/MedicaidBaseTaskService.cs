using System.Diagnostics;
using System.Threading;
using Phoenix.Core;
using Phoenix.Models.Repositories.Medicaid;
using Phoenix.Models.Services.Events;

namespace Phoenix.Medicaid.Service.Services
{
    public interface IMedicaidBaseTaskService
    {
        bool IsRunning { get; }
        EventLog MedicaidEventLog { get; set; }
        void TaskToRun();
        void CancelTask();
    }

    public abstract class MedicaidBaseTaskService : MedicaidBaseService, ITaskService, IMedicaidBaseTaskService
    {
        protected bool isRunning { get; set; }
        public bool IsRunning { get { return isRunning; } }

        protected static CancellationTokenSource CancellationTokenSource { get; set; }

        protected MedicaidBaseTaskService()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        public abstract void TaskToRun();

        public void CancelTask()
        {
            CancellationTokenSource.Cancel();
        }

        private IOpt61QueueRepository _opt61QueueRepository { get; set; }
        protected IOpt61QueueRepository Opt61QueueRepository
        {
            get { return _opt61QueueRepository ?? (_opt61QueueRepository = new Opt61QueueRepository()); }
        }

        private IOpt66QueueRepository _opt66QueueRepository { get; set; }
        protected IOpt66QueueRepository Opt66QuqRepository
        {
            get { return _opt66QueueRepository ?? (_opt66QueueRepository = new Opt66QueueRepository()); }
        }
    }
}
