using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Core
{
    public interface ITaskService
    {
        bool IsRunning { get; }

        void TaskToRun();
        void CancelTask();
    }
}
