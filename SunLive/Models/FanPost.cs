using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SunLive.Models
{
    [Serializable]
    public class FanPost
    {
        public int PostId { get; set; }
        public string ImageURL { get; set; }
        public string TextContent { get; set; }
        public string HashTag { get; set; }
        [DisplayFormat(DataFormatString = "{0:ddd, MMM d, yyyy}")]
        public DateTime PublishedOn { get; set; }
        public string PublishedBy { get; set; }
    }
}