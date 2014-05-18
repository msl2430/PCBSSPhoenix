using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;
using Phoenix.Core.Extensions;

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

        public Opt61Form(IList<MedicaidField> fields, Opt61Queue opt61Queue)
        {
            Fields = fields;
            base.Initialize(fields);
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (IEnumerable<MedicaidFormField>))
                {
                    var fieldList = ((List<MedicaidFormField>)prop.GetValue(this)).ToList();
                    foreach (var field in fieldList)
                    {
                        var queue = opt61Queue.GetType().GetProperty(string.Concat(prop.Name, fieldList.IndexOf(field) + 1));
                        if (queue == null || queue.GetValue(opt61Queue) == null)
                        {
                            field.Data = " ".PadLeft(field.Length, ' ');
                            continue;
                        }
                        field.Data = queue.PropertyType != typeof(DateTime?)
                            ? queue.GetValue(opt61Queue).ToString()
                            : Convert.ToDateTime(queue.GetValue(opt61Queue)).ToShortDateString();
                    }     
                }
                else if (prop.PropertyType == typeof (MedicaidFormField))
                {
                    var field = (MedicaidFormField)prop.GetValue(this);
                    if (field.StartIndex <= 0 || field.IsFieldEmpty()) continue;
                    var queue = opt61Queue.GetType().GetProperty(prop.Name);
                    if (queue == null || queue.GetValue(opt61Queue) == null)
                    {
                        field.Data = " ".PadLeft(field.Length, ' ');
                        continue;
                    }
                    field.Data = queue.PropertyType != typeof (DateTime?) 
                        ? queue.GetValue(opt61Queue).ToString() 
                        : Convert.ToDateTime(queue.GetValue(opt61Queue)).ToShortDateString();
                }
            }
        }

        public Opt61Queue ToOpt61Queue() {
            var opt61Queue = new Opt61Queue();
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (IEnumerable<MedicaidFormField>))
                {
                    var fields = ((List<MedicaidFormField>)prop.GetValue(this)).ToList();
                    foreach (var field in fields)
                    {
                        var queue = opt61Queue.GetType().GetProperty(string.Concat(prop.Name, fields.IndexOf(field) + 1));
                        if (queue == null) continue;
                        queue.SetPropertyValueFromString(opt61Queue, field.Data);
                    }                                       
                }
                else if(prop.PropertyType == typeof(MedicaidFormField))
                {
                    var field = (MedicaidFormField)prop.GetValue(this);
                    if (field.StartIndex <= 0 || field.IsFieldEmpty()) continue;
                    var queue = opt61Queue.GetType().GetProperty(prop.Name);
                    if (queue == null) continue;
                    queue.SetPropertyValueFromString(opt61Queue, field.Data);
                }
            }

            return opt61Queue;
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
                        list.Add(field);
                    }
                }
                if(prop.PropertyType == typeof(MedicaidFormField))
                {
                    var field = (MedicaidFormField)prop.GetValue(this);
                    if (field.StartIndex > 0)
                    {
                        field.Data = record.Substring(field.StartIndex, field.Length);
                    }
                }
            }
        }
    } 
}
