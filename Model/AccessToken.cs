using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunlive.Entities
{
    public class AccessToken
    {
        public double _id { get; set; }

        public string token { get; set; }

        public string userlist { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }

        public DateTime expires { get; set; }

        public string access_token { get; set; }
    }
}
