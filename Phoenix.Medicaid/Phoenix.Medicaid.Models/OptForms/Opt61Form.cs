using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.OptForms
{
    public sealed class Opt61Form : MedicaidOptForm
    {
        public MedicaidFormField Office { get; set; }
        public MedicaidFormField ProviderWarning { get; set; }
        public MedicaidFormField CaseName { get; set; }
        public MedicaidFormField Address { get; set; }
        public MedicaidFormField Address2 { get; set; }
        public MedicaidFormField Address3 { get; set; }
        public MedicaidFormField Address4 { get; set; }
        public MedicaidFormField City { get; set; }
        public MedicaidFormField State { get; set; }
        public MedicaidFormField Zip { get; set; }
        public MedicaidFormField LastName { get; set; }
        public MedicaidFormField FirstName { get; set; }
        public MedicaidFormField Middle { get; set; }
        public MedicaidFormField DateOfBirth { get; set; }
        public MedicaidFormField SocialSecurity { get; set; }
        public MedicaidFormField Sex { get; set; }
        public MedicaidFormField MaritalStatus { get; set; }
        public MedicaidFormField Race { get; set; }
        public MedicaidFormField PriorCase { get; set; }
        public MedicaidFormField PriorPersonNumber { get; set; }
        public MedicaidFormField AlienType { get; set; }
        public MedicaidFormField TempDate { get; set; }
        public IEnumerable<MedicaidFormField> EffectiveDate { get; set; }
        public IEnumerable<MedicaidFormField> TermDate { get; set; }
        public IEnumerable<MedicaidFormField> AddCode { get; set; }
        public IEnumerable<MedicaidFormField> TrmCode { get; set; }
        public IEnumerable<MedicaidFormField> Pgm { get; set; }
        public IEnumerable<MedicaidFormField> Supv { get; set; }
        public IEnumerable<MedicaidFormField> Res { get; set; }
        public IEnumerable<MedicaidFormField> ExtType { get; set; }
        public IEnumerable<MedicaidFormField> PregnancyDueDate { get; set; }
        public MedicaidFormField AddressAction { get; set; }
        public MedicaidFormField PersonAction { get; set; }
        public MedicaidFormField EligSeg { get; set; }
        public MedicaidFormField Supervisor { get; set; }
        public MedicaidFormField Worker { get; set; }

        private IEnumerable<MedicaidField> Fields { get; set; } 

        //public DateTime EntryDate { get { return !string.IsNullOrEmpty(TempDate.Data) ? Convert.ToDateTime(TempDate.Data.Insert(2, "/").Insert(5, "/")) : DateTime.Now;} }
        public DateTime EntryDate { get { return DateTime.Now; } }

        public Opt61Form(IList<MedicaidField> fields)
        {
            Fields = fields;
            base.Initialize(fields);
        }

        public override void PopulateFromCsv(string record)
        {
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (IEnumerable<MedicaidFormField>))
                {
                    var list = (List<MedicaidFormField>) prop.GetValue(this);
                    while (true)
                    {
                        var field = new MedicaidFormField(Fields.FirstOrDefault(f => f.FieldName.ToLower() == prop.Name.ToLower()));
                        if (field.StartIndex <= 0 || record.Length <= field.StartIndex + (list.Count*64) + field.Length)// || string.IsNullOrWhiteSpace(record.Substring(field.StartIndex + (list.Count*64), field.Length)))
                            break;

                        field.Data = record.Substring(field.StartIndex + (list.Count*64), field.Length);
                        Console.WriteLine("{0}: {1}", prop.Name, field.Data);
                        list.Add(field);
                    }
                }
                if(prop.PropertyType == typeof(MedicaidFormField))
                {
                    var field = (MedicaidFormField)prop.GetValue(this);
                    if (field.StartIndex > 0)
                    {
                        field.Data = record.Substring(field.StartIndex, field.Length);
                        Console.WriteLine("{0}: {1}", prop.Name, field.Data);
                    }
                }
            }
        }
    } 
}
