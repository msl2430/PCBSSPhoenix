using System;

namespace Phoenix.Models.Models.Medicaid
{
    public class Opt66Queue
    {
        public virtual int Opt66QueueId { get; set; }
        public virtual int CaseNumber { get; set; }
        public virtual int PersonNumber { get; set; }
        public virtual string Supv { get; set; }
        public virtual int Worker { get; set; }
        public virtual int ProgramStatus { get; set; }
        public virtual DateTime CaseRedetDate { get; set; }
        public virtual DateTime DisabilityRedetDate { get; set; }
        public virtual string ActionCode { get; set; }

        public override bool Equals(object obj)
        {
            var right = obj as Opt66Queue;
            return right.CaseNumber == CaseNumber && right.PersonNumber == PersonNumber;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
