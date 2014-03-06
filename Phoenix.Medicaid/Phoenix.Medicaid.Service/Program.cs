using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Medicaid.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {NHibernateProfilerBootstrapper.PreStart();

            var servicesToRun = new ServiceBase[]
                                {
                                    new MedicaidService(),
                                };
            ServiceBase.Run(servicesToRun);
        }
    }
}

