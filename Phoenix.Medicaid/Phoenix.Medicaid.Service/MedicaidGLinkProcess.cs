using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Models.Consants;
using Phoenix.Medicaid.Service;
using Phoenix.Medicaid.Service.Configuration;
using Phoenix.Medicaid.Service.Factories;

namespace Phoenix.Medicaid.Service
{
    public interface IMedicaidGLinkProcess
    {
        void ConnectToMedicaid();
        void StartConsoleMonitorThread();
        void EndConsoleMonitorThread();
        void LoginToMedicaid(string userName, string password);
    }
}

public class MedicaidGLinkProcess : IMedicaidGLinkProcess
{
    private IGLinkFactory _glinkFactory { get; set; }

    private IGLinkFactory GLinkFactory
    {
        get { return _glinkFactory ?? (_glinkFactory = new GLinkFactory()); }
    }

    public void ConnectToMedicaid()
    {
        try
        {
            var retryCounter = 0;
            GLinkFactory.MedicaidAutomation.Connect();
            GLinkFactory.MedicaidAutomation.SetVisible(true);
            while (!GLinkFactory.MedicaidAutomation.IsConnected)
            {
                if (retryCounter == 4) throw new Exception("Cannot connect to Glink!");
                GLinkFactory.MedicaidAutomation.Connect();
                retryCounter++;
            }
            GLinkFactory.MedicaidAutomation.SendStringToCursorAndTransmit("LABCICSZ");
            while (!GLinkFactory.MedicaidAutomation.GetStringAtLocation(7, 12, 13, 12).Contains("LOGONID"))
            {
                Thread.Sleep(500);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("{0}: {1} (ConnectingToMedicaid)", DateTime.Now, ex.Message);
        }
    }

    public void LoginToMedicaid(string userName, string password)
    {
        var retryCounter = 0;
        GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.UserName.ToInt(), userName);
        GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.Password.ToInt(), password);
        GLinkFactory.MedicaidAutomation.TransmitPage();
        if (GLinkFactory.MedicaidAutomation.GetStringAtLocation(1, 20, 3, 20).Contains("ACF"))
        {
            var tempErrorMessage = GLinkFactory.MedicaidAutomation.GetStringAtLocation(10, 20, 65, 20);
            if (tempErrorMessage.Contains("R94"))
                tempErrorMessage = GLinkFactory.MedicaidAutomation.GetStringAtLocation(10, 21, 65, 21);
            throw new Exception(tempErrorMessage);
        }
        while (!GLinkFactory.MedicaidAutomation.GetStringAtLocation(1, 4, 3, 4).Contains("ACF"))
        {
            if(retryCounter == 20) throw new Exception("Error logging in to Medicaid");
            retryCounter++;
            Thread.Sleep(500);
        }
        GLinkFactory.MedicaidAutomation.SendStringToCursorAndTransmit("ELIG");
        GLinkFactory.MedicaidAutomation.SendStringToCursorAndTransmit("05");
    }

    public void StartConsoleMonitorThread()
    {
        try
        {
            GLinkFactory.MedicaidAutomation.StartConsoleMonitor();
        }
        catch (Exception ex)
        {
            Console.WriteLine("{0}: {1} (StartConsoleMonitorThread)", DateTime.Now, ex.Message);
        }
    }

    public void EndConsoleMonitorThread()
    {
        try
        {
            GLinkFactory.MedicaidAutomation.EndConsoleMonitor();
        }
        catch (Exception ex)
        {
            Console.WriteLine("{0}: {1} (EndConsoleMonitorThread)", DateTime.Now, ex.Message);
        }
    }
}
