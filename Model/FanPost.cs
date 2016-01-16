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

        private DateTime publishedOnUTC;

        [DisplayFormat(DataFormatString = "{0: d MMM yyyy hh:mm tt}")]
        public DateTime PublishedOn {
            get {
                return publishedOnUTC.ToUniversalTime().AddHours(5.5);
            }
            set {
                publishedOnUTC = value;
            }
        }
        public string PublishedBy { get; set; }

        public string PublishedById { get; set; }

        public string PublishedByUrl { get; set; }

        public string Status { get; set; }

        public string FileName { get; set; }

        public string OutputDir { get; set; }

        public string Source { get; set; }

        private DateTime convertPubishedOnToIstDateTime(DateTime dateUTC)
        {
            try
            {
                TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                return TimeZoneInfo.ConvertTime(PublishedOn.ToLocalTime(), TimeZoneInfo.Local, istZone);
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("The registry does not define the Indian Standard Time zone.");
                return DateTime.MinValue;
            }
            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("Registry data on the Indian Standard Time zone has been corrupted.");
                return DateTime.MinValue;
            }
        }
    }
}