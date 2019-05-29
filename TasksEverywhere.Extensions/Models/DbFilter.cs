using TasksEverywhere.Extensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Extensions.Models
{
    public class DbFilter : Abstract.IDbFilter
    {
        public string PropertyName { get; set; }
        public CompareSign Sign { get; set; } = CompareSign.Equals;
        public object Value { get; set; } = null;
    }
}
