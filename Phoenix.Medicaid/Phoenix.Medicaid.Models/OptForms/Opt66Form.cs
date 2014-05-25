using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Models.FormFields;
using Phoenix.Models.Models.Medicaid;

namespace Phoenix.Medicaid.Models.OptForms
{
    public sealed class Opt66Form : MedicaidOptForm
    {
        public MedicaidFormField Supv { get; set; }
        public MedicaidFormField Worker { get; set; }
        public MedicaidFormField ProgramStatus { get; set; }
        public MedicaidFormField CaseRedetDate { get; set; }
        public MedicaidFormField DisabilityRedetDate { get; set; }

        private IEnumerable<MedicaidField> Fields { get; set; }

        public Opt66Form(IList<MedicaidField> fields)
        {
            Fields = fields;
            base.Initialize(fields);
        }

        public Opt66Form(IList<MedicaidField> fields, Opt66Queue opt66Queue)
        {
            Fields = fields;
            base.Initialize(fields);
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType != typeof (MedicaidFormField)) continue;
                var field = (MedicaidFormField)prop.GetValue(this);
                if (field.StartIndex <= 0 || field.IsFieldEmpty()) continue;
                var queue = opt66Queue.GetType().GetProperty(prop.Name);
                if (queue == null || queue.GetValue(opt66Queue) == null)
                {
                    field.Data = " ".PadLeft(field.Length, ' ');
                    continue;
                }
                field.Data = queue.PropertyType != typeof (DateTime?) 
                    ? queue.GetValue(opt66Queue).ToString() 
                    : Convert.ToDateTime(queue.GetValue(opt66Queue)).ToShortDateString();
            }
        }

        public Opt66Queue ToOpt66Queue()
        {
            var opt66Queue = new Opt66Queue();
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType != typeof (MedicaidFormField)) continue;
                var field = (MedicaidFormField)prop.GetValue(this);
                if (field.StartIndex <= 0 || field.IsFieldEmpty()) continue;
                var queue = opt66Queue.GetType().GetProperty(prop.Name);
                if (queue == null) continue;
                queue.SetPropertyValueFromString(opt66Queue, field.Data);
            }

            return opt66Queue;
        }

        public override void PopulateFromCsv(string record)
        {
            foreach (var field in
                    GetType()
                        .GetProperties()
                        .Where(prop => prop.PropertyType == typeof (MedicaidFormField))
                        .Select(prop => (MedicaidFormField) prop.GetValue(this))
                        .Where(field => field.StartIndex > 0))
            {
                field.Data = record.Substring(field.StartIndex, field.Length);
            }
        }
    }
}
