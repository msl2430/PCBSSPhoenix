using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Core.Attributes;

namespace Phoenix.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(StringValueAttribute)) as StringValueAttribute;

            return attribute == null ? value.ToString() : attribute.Value;
        }

        public static int ToInt(this Enum enumValue)
        {
            return (int)((object)enumValue);
        }
    }
}
