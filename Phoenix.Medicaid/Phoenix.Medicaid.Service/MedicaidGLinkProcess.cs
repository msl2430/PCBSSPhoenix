using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.GLinkAutomation.Core.Constants;
using Phoenix.Medicaid.Models.Consants;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Medicaid.Models.OptForms;
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
        void SubmitOpt61Form(Opt61Form opt61Form);
    }
}

public class MedicaidGLinkProcess : IMedicaidGLinkProcess
{
    private IGLinkFactory _glinkFactory { get; set; }
    private string ErrorMessage { get; set; }

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

    public void SubmitOpt61Form(Opt61Form opt61Form)
    {
        GLinkFactory.MedicaidAutomation.SendStringToCursorAndTransmit("061");
        if (opt61Form.PersonAction.Data == @"A")
        {
            //TODO: Process new person with Opt 66
        }
        else
        {
            GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.CaseNumberScreen.ToInt(), opt61Form.CaseNumber.Data);
            GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.PersonNumberScreen.ToInt(), opt61Form.PersonNumber.Data);
            GLinkFactory.MedicaidAutomation.TransmitPage();
            if (IsOptError() && ErrorMessage.Contains("HIGHLIGHT"))
            {
                //TODO: Handled error opt
            }
            else
            {
                TransmitOpt61(opt61Form);
                GLinkFactory.MedicaidAutomation.TransmitPage();
                if (IsOptError())
                {
                    if (ErrorMessage.Contains("SUPERVISOR") && !opt61Form.Supervisor.IsFieldEmpty())
                    {
                        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Supervisor);
                        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Worker);
                        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.PersonAction.FieldNumber, "C");
                        GLinkFactory.MedicaidAutomation.TransmitPage();
                        if (IsOptError())
                        {
                            //TODO: Handle error opt
                        }
                        else
                        {
                            StoreSuccesfulOpt(Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt61);
                        }
                    }
                    else
                    {
                        //TODO: Handle error opt
                    }
                }
                else
                {
                    StoreSuccesfulOpt(Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt61);
                }
            }
        }
    }
    private void TransmitOpt61(Opt61Form opt61Form)
    {
        var segIndex = new List<int?>();
        if (!opt61Form.ActionCode.IsFieldEmpty())
        {
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.ActionCode);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Office);
        }
        if (!opt61Form.AddressAction.IsFieldEmpty())
        {
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.AddressAction);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.CaseName);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Address);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Address2);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Address3);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Address4);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.City);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.State);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Zip);
        }
        if (!opt61Form.PersonAction.IsFieldEmpty())
        {
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.PersonAction);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.LastName);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.FirstName);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Middle);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.DateOfBirth);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.SocialSecurity);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Sex);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.MaritalStatus);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Race);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.PriorCase);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.PriorPersonNumber);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.AlienType);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.TempDate);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Supervisor);
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Worker);
        }

        if (opt61Form.EligSeg.IsFieldEmpty()) return;

        var activeEffectiveDates = opt61Form.EffectiveDate.Where(ed => !ed.IsFieldEmpty()).ToList();
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.EligSeg);
        var lineToEnter1 = activeEffectiveDates.Count() - 1;
        var lineToEnter2 = activeEffectiveDates.Count();
        if (!string.IsNullOrEmpty(GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 26, 10, 26)) && lineToEnter2 > 0)
        {
            if (lineToEnter1 > 0)
            {
                foreach (var effDate in activeEffectiveDates.Where(effDate => GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 26, 10, 26) != effDate.Data &&
                                                                              GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 27, 10, 27) != effDate.Data &&
                                                                              GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 28, 10, 28) != effDate.Data &&
                                                                              GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 29, 10, 29) != effDate.Data &&
                                                                              GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 30, 10, 30) != effDate.Data &&
                                                                              GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 31, 10, 31) != effDate.Data))
                {
                    segIndex.Add(activeEffectiveDates.IndexOf(effDate));
                    if (segIndex.Count == 2) break;
                }
                if (segIndex[1] != null)
                {
                    if (!opt61Form.TermDate.ElementAt((int)segIndex[0]).IsFieldEmpty())
                    {
                        SubmitEligSeg(opt61Form, 0, segIndex[0], false);
                        SubmitEligSeg(opt61Form, 1, segIndex[1], false);
                    }
                    else
                    {
                        SubmitEligSeg(opt61Form, 0, segIndex[1], false);
                        SubmitEligSeg(opt61Form, 1, segIndex[0], false);
                    }
                }
                else if (segIndex[0] != null)
                {
                    SubmitEligSeg(opt61Form, 1, segIndex[0], false);
                }
                else
                {
                    if (string.IsNullOrEmpty(GLinkFactory.MedicaidAutomation.GetStringAtLocation(12, 26, 19, 26)) && !opt61Form.TermDate.FirstOrDefault().IsFieldEmpty())
                    {
                        SubmitEligSeg(opt61Form, 2, 0, true);
                    }
                    else
                    {
                        SubmitEligSeg(opt61Form, 2, lineToEnter2, true);
                    }
                }
            }
            else if (opt61Form.EffectiveDate.ElementAt(lineToEnter2).Data == GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 26, 10, 26))
            {
                if (string.IsNullOrEmpty(opt61Form.TermDate.ElementAt(lineToEnter2).Data))
                {
                    SubmitEligSeg(opt61Form, 1, lineToEnter2, false);
                    SubmitEligSeg(opt61Form, 2, lineToEnter1, false);
                }
                else if (opt61Form.TermDate.ElementAt(lineToEnter2).Data == GLinkFactory.MedicaidAutomation.GetStringAtLocation(12, 26, 19, 26))
                {
                    SubmitEligSeg(opt61Form, 1, lineToEnter1, false);
                }
                else
                {
                    SubmitEligSeg(opt61Form, 2, lineToEnter2, true);
                }
            }
            else if (opt61Form.EffectiveDate.ElementAt(lineToEnter1).Data == GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 26, 10, 26))
            {
                if (opt61Form.TermDate.ElementAt(lineToEnter1).Data == GLinkFactory.MedicaidAutomation.GetStringAtLocation(12, 26, 19, 26))
                {
                    SubmitEligSeg(opt61Form, 1, lineToEnter2, false);
                }
                else
                {
                    SubmitEligSeg(opt61Form, 1, lineToEnter2, false);
                    SubmitEligSeg(opt61Form, 2, lineToEnter1, false);
                }
            }
            else
            {
                if ((!opt61Form.TermDate.ElementAt(lineToEnter2).IsFieldEmpty() || Convert.ToDateTime(opt61Form.TermDate.ElementAt(lineToEnter1).Data) < Convert.ToDateTime(opt61Form.TermDate.ElementAt(lineToEnter2).Data)) &&
                    !opt61Form.TermDate.ElementAt(lineToEnter1).IsFieldEmpty())
                {
                    SubmitEligSeg(opt61Form, 0, lineToEnter2, false);
                    SubmitEligSeg(opt61Form, 1, lineToEnter1, false);
                }
                else
                {
                    SubmitEligSeg(opt61Form, 0, lineToEnter1, false);
                    SubmitEligSeg(opt61Form, 1, lineToEnter2, false);
                }
            }
        }
        else if (lineToEnter2 > 0 && opt61Form.EligSeg.Data == "A")
        {
            if ((!opt61Form.TermDate.ElementAt(lineToEnter2).IsFieldEmpty() || Convert.ToDateTime(opt61Form.TermDate.ElementAt(lineToEnter1).Data) < Convert.ToDateTime(opt61Form.TermDate.ElementAt(lineToEnter2).Data)) &&
                !opt61Form.TermDate.ElementAt(lineToEnter1).IsFieldEmpty())
            {
                SubmitEligSeg(opt61Form, 0, lineToEnter2, false);
                SubmitEligSeg(opt61Form, 1, lineToEnter1, false);
            }
            else
            {
                SubmitEligSeg(opt61Form, 0, lineToEnter1, false);
                SubmitEligSeg(opt61Form, 1, lineToEnter2, false);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(GLinkFactory.MedicaidAutomation.GetStringAtLocation(12, 26, 19, 26)) && opt61Form.EligSeg.Data != "A" && !string.IsNullOrEmpty(GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 26, 10, 26)))
            {
                SubmitEligSeg(opt61Form, 2, lineToEnter2, true);
            }
            else if (GLinkFactory.MedicaidAutomation.GetStringAtLocation(3, 26, 10, 26) == opt61Form.EffectiveDate.ElementAt(lineToEnter2).Data)
            {
                SubmitEligSeg(opt61Form, 2, lineToEnter2, false);
            }
            else
            {
                SubmitEligSeg(opt61Form, 1, lineToEnter2, false);
            }
        }
    }
    /// <summary>
    /// Submit Eligibility Segment
    /// </summary>
    /// <param name="opt61Form">Form being processed</param>
    /// <param name="fieldIndex">Index of block that represents where we want to enter the data in Medicaid</param>
    /// <param name="lineIndex">The actual block we're submitting</param>
    /// <param name="skipEffectiveDate"></param>
    private void SubmitEligSeg(Opt61Form opt61Form, int fieldIndex, int? lineIndex, bool skipEffectiveDate)
    {
        var line = Convert.ToInt32(lineIndex);
        if (!skipEffectiveDate)
            GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.EffectiveDate.ElementAt(fieldIndex).FieldNumber, opt61Form.EffectiveDate.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.TermDate.ElementAt(fieldIndex).FieldNumber, opt61Form.TermDate.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.AddCode.ElementAt(fieldIndex).FieldNumber, opt61Form.AddCode.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.TrmCode.ElementAt(fieldIndex).FieldNumber, opt61Form.TrmCode.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Pgm.ElementAt(fieldIndex).FieldNumber, opt61Form.Pgm.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Supv.ElementAt(fieldIndex).FieldNumber, opt61Form.Supv.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.Res.ElementAt(fieldIndex).FieldNumber, opt61Form.Res.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.ExtType.ElementAt(fieldIndex).FieldNumber, opt61Form.ExtType.ElementAt(line).Data);
        GLinkFactory.MedicaidAutomation.SubmitField(opt61Form.PregnancyDueDate.ElementAt(fieldIndex).FieldNumber, opt61Form.PregnancyDueDate.ElementAt(line).Data);
    }

    public void SubmitOpt64Form(Opt64Form opt64Form)
    {
        GLinkFactory.MedicaidAutomation.SendStringToCursorAndTransmit("064");
        GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.CaseNumberScreen.ToInt(), opt64Form.CaseNumber.Data);
        GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.PersonNumberScreen.ToInt(), opt64Form.PersonNumber.Data);
        GLinkFactory.MedicaidAutomation.TransmitPage();
        if (IsOptError() && ErrorMessage.Contains("HIGHLIGHT"))
        {
            //TODO: Handled error opt
        }
        else
        {
            TransmitOpt64(opt64Form);
            GLinkFactory.MedicaidAutomation.TransmitPage();
            if (IsOptError())
            {
                //TODO: Handled error opt
            }
            else
            {
                StoreSuccesfulOpt(Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt64);
            }
        }
    }
    private void TransmitOpt64(Opt64Form opt64Form)
    {
        GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.ActionCode);
        foreach (var index in opt64Form.PgmNumber.Where(p => !p.IsFieldEmpty()).Select(pgm => opt64Form.PgmNumber.ToList().IndexOf(pgm)))
        {
            if (string.IsNullOrEmpty(GLinkFactory.MedicaidAutomation.GetStringAtLocation(19, 26, 20, 26)))
            {
                GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.PgmNumber.ElementAt(index));
                GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.EffectiveDate.ElementAt(index));
                GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.TermDate.ElementAt(index));
            }
            else
            {
                GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.PgmNumber.ElementAt(index + 1).FieldNumber, opt64Form.PgmNumber.ElementAt(index).Data);
                GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.EffectiveDate.ElementAt(index + 1).FieldNumber, opt64Form.EffectiveDate.ElementAt(index).Data);
                GLinkFactory.MedicaidAutomation.SubmitField(opt64Form.TermDate.ElementAt(index + 1).FieldNumber, opt64Form.TermDate.ElementAt(index).Data);
            }
        }
    }

    public void SubmitOp66Form(Opt66Form opt66Form)
    {
        GLinkFactory.MedicaidAutomation.SendStringToCursorAndTransmit("066");
        GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.CaseNumberScreen.ToInt(), opt66Form.CaseNumber.Data);
        GLinkFactory.MedicaidAutomation.SubmitField(FormConstants.PersonNumberScreen.ToInt(), opt66Form.PersonNumber.Data);
        GLinkFactory.MedicaidAutomation.TransmitPage();
        if (IsOptError() && ErrorMessage.Contains("HIGHLIGHT"))
        {
            //TODO: Handled error opt
        }
        else
        {
            TransmitOpt66Form(opt66Form);
            GLinkFactory.MedicaidAutomation.TransmitPage();
            if (IsOptError())
            {
                //TODO: Handled error opt
            }
            else
            {
                StoreSuccesfulOpt(Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt66);
            }
        }
    }
    private void TransmitOpt66Form(Opt66Form opt66Form)
    {
        GLinkFactory.MedicaidAutomation.SubmitField(opt66Form.ActionCode);
        if (opt66Form.ActionCode.Data == "D")
        {
            GLinkFactory.MedicaidAutomation.TransmitPage();
            if (GLinkFactory.MedicaidAutomation.GetStringAtLocation(2, 42, 8, 42) == "DEPRESS")
                GLinkFactory.MedicaidAutomation.SendCommandKey(Commands.GLinkCommands.F9);
        }
        else
        {
            GLinkFactory.MedicaidAutomation.SubmitField(opt66Form.Supv);
            GLinkFactory.MedicaidAutomation.SubmitField(opt66Form.Worker);
            GLinkFactory.MedicaidAutomation.SubmitField(opt66Form.ProgramStatus);
            if(opt66Form.CaseRedetDate.Data != "000000")
                GLinkFactory.MedicaidAutomation.SubmitField(opt66Form.CaseRedetDate);
            if (opt66Form.DisabilityRedetDate.Data != "000000")
                GLinkFactory.MedicaidAutomation.SubmitField(opt66Form.DisabilityRedetDate);   
        }
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

    private bool IsOptError()
    {
        var line1Error = GLinkFactory.MedicaidAutomation.GetStringAtLocation(1, 41, 69, 41);
        var line2Error = GLinkFactory.MedicaidAutomation.GetStringAtLocation(1, 42, 69, 42);
        if (!string.IsNullOrEmpty(line2Error) && !line2Error.Contains("NO RDET D") &&
            !line2Error.Contains("DEPRESS") && !line2Error.Contains("MORE DA") &&
            !line2Error.Contains("NO SPEC P") && !line2Error.Contains("DEATH"))
        {
            ErrorMessage = line2Error;
            return true;
        }
        if (!string.IsNullOrEmpty(line1Error) && !line1Error.Contains("PF3"))
        {
            ErrorMessage = line1Error;
            return true;
        }
        ErrorMessage = string.Empty;
        return false;
    }
    private void StoreSuccesfulOpt(Phoenix.Models.Constants.FormConstants.MedicaidForms form)
    {
        switch (form)
        {
            case Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt61:
            case Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt64:
            case Phoenix.Models.Constants.FormConstants.MedicaidForms.Opt66:
                Console.WriteLine("Case done");
                break;

        }
    }
    
    

    

    
}
