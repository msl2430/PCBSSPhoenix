using System;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Service;

namespace Phoenix.Medicaid.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var process = new MedicaidProcess();

            process.LogEvent("Started", EventTypes.Events.ApplicationStarted.ToInt());

            process.RunMedicaidProcess();

            process.LogEvent("Stopped", EventTypes.Events.ApplicationStopped.ToInt());

            Console.ReadLine();
        }
    }
}
