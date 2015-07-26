
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Charts
{
    public class PerDayInfo
    {
        public double DateTime { get; set; }
        public double UniqueUsers { get; set; }
        public double UniqueMessages { get; set; }
        public List<int> Messages { get; set; }
    }

  
}
