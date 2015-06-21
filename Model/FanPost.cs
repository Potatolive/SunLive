using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunlive.Entities
{
    [Serializable]
    public class FanPost
    {

        public string _id { get; set; }
        public string PostId { get; set; }
        public string ImageURL { get; set; }
        public string CroppedImageURL { get; set; }
        public string TextContent { get; set; }
        public string HashTag { get; set; }
        [DisplayFormat(DataFormatString = "{0: d MMM yyyy hh:mm tt}")]
        public DateTime PublishedOn { get; set; }
        public string PublishedBy { get; set; }

        public string Status { get; set; }

        public string FileName { get; set; }

        public string OutputDir { get; set; }

        public void convertPubishedOnToIstDateTime()
        {
            try
            {
                TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                PublishedOn = TimeZoneInfo.ConvertTime(PublishedOn.ToLocalTime(), TimeZoneInfo.Local, istZone);
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("The registry does not define the Indian Standard Time zone.");
                PublishedOn = DateTime.MinValue;
            }
            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("Registry data on the Indian Standard Time zone has been corrupted.");
                PublishedOn = DateTime.MinValue;
            }
        }
    }
}