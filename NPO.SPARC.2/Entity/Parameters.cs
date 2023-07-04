using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPO.SPARC._2.Entity
{
    public class Parameters
    {
        public int ParameterId { get; set; }
        public int TestId { get; set; }
        public string ParameterName { get; set; }
        public decimal RequiredValue { get; set; }
        public decimal MeasuredValue { get; set; }

        public Tests Tests { get; set; }
    }
}
