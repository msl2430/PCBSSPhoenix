using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Models.Models.Medicaid
{
    public class Opt61Queue
    {
        public virtual int Opt61QueueId { get; set; }
        public virtual int? CaseNumber { get; set; }
        public virtual int? PersonNumber { get; set; }
        public virtual string BatchNumber { get; set; }
        public virtual string ActionCode { get; set; }
        public virtual int? Office { get; set; }
        public virtual string ProviderWarning { get; set; }
        public virtual string CaseName { get; set; }
        public virtual string Address { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual int? Zip { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Middle { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual int? SocialSecurity { get; set; }
        public virtual string Sex { get; set; }
        public virtual string MaritalStatus { get; set; }
        public virtual string Race { get; set; }
        public virtual string PriorCase { get; set; }
        public virtual int? PriorPersonNumber { get; set; }
        public virtual string AlienType { get; set; }
        public virtual DateTime? TempDate { get; set; }
        public virtual DateTime? EffectiveDate { get; set; }
        public virtual DateTime? EffectiveDate2 { get; set; }
        public virtual DateTime? EffectiveDate3 { get; set; }
        public virtual DateTime? EffectiveDate4 { get; set; }
        public virtual DateTime? TermDate { get; set; }
        public virtual DateTime? TermDate2 { get; set; }
        public virtual DateTime? TermDate3 { get; set; }
        public virtual DateTime? TermDate4 { get; set; }
        public virtual string AddCode { get; set; }
        public virtual string AddCode2 { get; set; }
        public virtual string AddCode3 { get; set; }
        public virtual string AddCode4 { get; set; }
        public virtual string TrmCode { get; set; }
        public virtual string TrmCode2 { get; set; }
        public virtual string TrmCode3 { get; set; }
        public virtual string TrmCode4 { get; set; }
        public virtual string Pgm { get; set; }
        public virtual string Pgm2 { get; set; }
        public virtual string Pgm3 { get; set; }
        public virtual string Pgm4 { get; set; }
        public virtual string Supv { get; set; }
        public virtual string Supv2 { get; set; }
        public virtual string Supv3 { get; set; }
        public virtual string Supv4 { get; set; }
        public virtual string Res { get; set; }
        public virtual string Res2 { get; set; }
        public virtual string Res3 { get; set; }
        public virtual string Res4 { get; set; }
        public virtual string ExtType { get; set; }
        public virtual string ExtType2 { get; set; }
        public virtual string ExtType3 { get; set; }
        public virtual string ExtType4 { get; set; }
        public virtual DateTime? PregnancyDueDate { get; set; }
        public virtual DateTime? PregnancyDueDate2 { get; set; }
        public virtual DateTime? PregnancyDueDate3 { get; set; }
        public virtual DateTime? PregnancyDueDate4 { get; set; }
        public virtual string AddressAction { get; set; }
        public virtual string PersonAction { get; set; }
        public virtual string EligSeg { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual int? Worker { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
