using System;
using Phoenix.Core.Constants;
using Phoenix.GLinkAutomation.Core.ApplicationAutomation;
using Phoenix.Medicaid.Service;
using Phoenix.Medicaid.Service.Factories;
using Phoenix.Models.Models.Events;
using Phoenix.Models.NHibernate;

namespace Phoenix.Medicaid.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var process = new MedicaidProcess()
                          {
                              GLinkFactory = new GLinkFactory(true)
                          };
            process.LogEvent("Started", EventTypes.Events.ApplicationStarted);

            process.RunMedicaidProcess();

            process.LogEvent("Stopped", EventTypes.Events.ApplicationStopped);

            Console.ReadLine();
        }
    }
}
