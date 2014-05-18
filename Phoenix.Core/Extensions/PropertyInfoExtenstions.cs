using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Core.Extensions
{
    public static class PropertyInfoExtenstions
    {
        public static void SetPropertyValueFromString(this PropertyInfo self, Object parentClass, string value)
        {
            if (self.PropertyType == typeof(int) || self.PropertyType == typeof(Int32?))
            {
                self.SetValue(parentClass, value.TryParseNullableInt(), null);
            }
            else if (self.PropertyType == typeof(string))
            {
                self.SetValue(parentClass, value, null);
            }
            else if (self.PropertyType == typeof(DateTime))
            {
                self.SetValue(parentClass, value.TryParseNullableDateTime(), null);
            }
        }
    }
}
