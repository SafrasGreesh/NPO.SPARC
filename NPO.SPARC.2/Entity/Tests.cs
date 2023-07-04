using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPO.SPARC._2.Entity
{
    public class Tests
    {
        public int TestId { get; set; }
        public DateTime TestDate { get; set; }
        public string BlockName { get; set; }
        public string Note { get; set; }

        public List<Parameters> Parameters { get; set; }
    }
}
