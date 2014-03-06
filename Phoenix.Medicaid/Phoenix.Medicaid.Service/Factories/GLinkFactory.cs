using System;
using System.Configuration;
using Phoenix.GLinkAutomation.Core.ApplicationAutomation;
using Phoenix.GLinkAutomation.Core.ApplicationAutomation.Mock;

namespace Phoenix.Medicaid.Service.Factories
{
    public interface IGLinkFactory
    {
        IMedicaidAutomation MedicaidAutomation { get; }
    }

    public class GLinkFactory : IGLinkFactory
    {
        private IMedicaidAutomation _medicaidAutomation { get; set; }
        private bool IsDebug { get; set; }

        public GLinkFactory()
        {
            IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"]);
        }

        public IMedicaidAutomation MedicaidAutomation
        {
            get
            {
                if (_medicaidAutomation != null) return _medicaidAutomation;

                if (IsDebug)
                    _medicaidAutomation = new MockMedicaidAutomation();
                else
                    _medicaidAutomation = new MedicaidAutomation(@"C:\GlPro\PhoenixMedi.02");

                return _medicaidAutomation;
            }
        }
    }
}
