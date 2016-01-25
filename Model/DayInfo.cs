using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunlive.Entities
{
    public class DayInfo
    {
        public string PageName { get; set; }

        public int TotalPages { get; set; }
        public DateTime Date { get; set; }

        public int SelectedHour { get; set; }

        public int SelectedMin { get; set; }

        public int CurrentPage { get; set; }

        public List<FanPost> Posts { get; set; }

        public string Source { get; set; }

        public long TodaysCount { get; set; }

        public bool Latest { get; set; }
    }
}
