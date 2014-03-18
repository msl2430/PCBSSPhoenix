using System;
using System.Reflection;

namespace Phoenix.Core.Attributes
{
    public class StringValueAttribute : Attribute
    {
        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public static string GetStringValue(Enum value)
        {
            string output = null;
            var type = value.GetType();

            var fi = type.GetField(value.ToString());
            var attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            if (attrs.Length > 0)
                output = attrs[0].Value;

            return output;
        }
    }
}