using System;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service;
using Phoenix.Models.Models.Events;
using Phoenix.Models.NHibernate;

namespace Phoenix.Medicaid.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = new MedicaidProcess();
            process.LogEvent("Started", EventTypes.Events.ApplicationStarted);
            process.RunMedicaidProcess();
            process.LogEvent("Started", EventTypes.Events.ApplicationStopped);

            Console.ReadLine();
        }
    }
}
