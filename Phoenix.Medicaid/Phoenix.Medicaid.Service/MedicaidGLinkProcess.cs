using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Medicaid.Service.Configuration;
using Phoenix.Medicaid.Service.Factories;

namespace Phoenix.Medicaid.Service
{
    public class MedicaidGLinkProcess
    {
        private static IGLinkFactory GLinkFactory { get; set; }

        public void ConnectToMedicaid()
        {
            try
            {
                GLinkFactory.Current().Connect();
                GLinkFactory.Current().SetVisible(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1} (Connecting to Medicaid)", DateTime.Now, ex.Message);
            }
        }
    }
}
