using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.Core.Services
{
    public abstract class BaseTaskService : ITaskService
    {
        protected bool isRunning { get; set; }
        public bool IsRunning { get { return isRunning; } }

        protected static CancellationTokenSource CancellationTokenSource { get; set; }

        protected BaseTaskService() { CancellationTokenSource = new CancellationTokenSource(); }

        public abstract void TaskToRun();

        public void CancelTask()
        {
                CancellationTokenSource.Cancel();
        }
    }
}
