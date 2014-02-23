using Phoenix.GLinkAutomation.Core.ApplicationAutomation;
using Phoenix.GLinkAutomation.Core.ApplicationAutomation.Mock;

namespace Phoenix.Medicaid.Service.Factories
{
    public interface IGLinkFactory
    {
        IMedicaidAutomation Current();
    }

    public class GLinkFactory : IGLinkFactory
    {
        private IMedicaidAutomation MedicaidAutomation { get; set; }
        private bool IsDebug { get; set; }

        public GLinkFactory(bool isDebug)
        {
            IsDebug = isDebug;
        }

        public IMedicaidAutomation Current()
        {
            if (MedicaidAutomation != null) return MedicaidAutomation;

            if (IsDebug)
                MedicaidAutomation = new MockMedicaidAutomation();
            else
                MedicaidAutomation = new MedicaidAutomation(@"C:\GlPro\PhoenixMedi.02");

            return MedicaidAutomation;
        }
    }
}
