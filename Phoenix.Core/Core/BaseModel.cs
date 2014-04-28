using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Core.Core
{
    public class BaseModel
    {
        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var prop in GetType().GetProperties())
            {
                str.AppendFormat("{0}: {1} ", prop.Name, prop.GetValue(this, null));
            }
            return str.ToString();
        }
    }
}
