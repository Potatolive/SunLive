using Model.Charts;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sunlive.Entities;
using SunLive.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SunLive.Controllers
{
    public class AnalyticsController : Controller
    {
        IMongoDatabase MongoDB = null;

        public AnalyticsController()
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            MongoClient client = new MongoClient(connectionString);

            //Need to make this config driven
            MongoDB = client.GetDatabase("Testing");


        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        ChartSeries FetchDailyMessages(string startDate, string endDate)
        {
            var coll = MongoDB.GetCollection<FanPost>("fanposts");

            var temp = coll.Aggregate();

            if (!string.IsNullOrEmpty(startDate)
          && !string.IsNullOrEmpty(endDate))
            {
                temp = temp.Match("{ PublishedOn: { $gt: \"" + startDate + "\", $lte: \"" + endDate + "\" } }");
            }

            var result = temp.Project("{_id : \"$_id\", dt : { $substr: [\"$PublishedOn\", 0, 10] } }")
            .Group("{ _id: \"$dt\", messages: { $sum: 1 } }")
            .Sort("{ _id : 1 }");

            var list = result.ToListAsync();

            var series = new ChartSeries() { Name = "Messages" };
            series.Data = new List<ChartData>();

            foreach (var item in list.Result)
            {
                var dateString = item.Elements.ElementAt(0).Value.AsString;

                var date = Convert.ToDateTime(dateString);
                var count = item.Elements.ElementAt(1).Value.AsInt32;
                series.Data.Add(new ChartData()
                    {
                        X = GetUTCNumberFromDate(date),
                        Y = count
                    });
            }

            return series;
        }

        ChartSeries FetchDailyUsers(string startDate, string endDate)
        {
            var coll = MongoDB.GetCollection<FanPost>("fanposts");

            var temp = coll.Aggregate();

            if (!string.IsNullOrEmpty(startDate)
                && !string.IsNullOrEmpty(endDate))
            {
                temp = temp.Match("{ PublishedOn: { $gt: \"" + startDate + "\", $lte: \"" + endDate + "\" } }");
            }

            var result = temp.Project("{_id : \"$_id\",user: \"$PublishedBy\",dt : { $substr: [\"$PublishedOn\", 0, 10] }}")
            .Group("{_id : { dt: \"$dt\", user: \"$user\" },count: { $sum: 1 }}")
            .Group("{_id: { dt: \"$_id.dt\" },uniqueUsers : { $sum: 1 }}")
            .Sort("{ _id : 1 }");

            var list = result.ToListAsync();

            var series = new ChartSeries() { Name = "Users" };
            series.Data = new List<ChartData>();

            foreach (var item in list.Result)
            {
                var dateString = item.Elements.ElementAt(0).Value.AsBsonDocument.Elements.ElementAt(0).Value.AsString;

                var date = Convert.ToDateTime(dateString);
                var count = item.Elements.ElementAt(1).Value.AsInt32;
                series.Data.Add(new ChartData()
                    {
                        X = GetUTCNumberFromDate(date),
                        Y = count
                    });
            }

            return series;
        }

        public List<BsonDocument> FetchMessagePerTimeSlot(string startDate, string endDate)
        {
            var coll = MongoDB.GetCollection<FanPost>("fanposts");

            var temp = coll.Aggregate();

            if (!string.IsNullOrEmpty(startDate)
                && !string.IsNullOrEmpty(endDate))
            {
                temp = temp.Match("{ PublishedOnDateTime: { $gt: ISODate(\"" + startDate + "\"), $lte: ISODate(\"" + endDate + "\") } }");
            }

            var result = temp.Project("{_id : \"$_id\",dt : { $substr: [\"$PublishedOnDateTime\", 0, 13] },bucket: \"$bucket12\"}")
                .Group("{_id: { dt: \"$dt\", bucket: \"$bucket\"},messages: { $sum: 1 }}")
                .Sort("{ _id : 1 }");

            var list = result.ToListAsync();


            foreach (var item in list.Result)
            {
            }

            return list.Result;
        }

        public ActionResult GetUniqueTimeSeries()
        {
            var result = new List<ChartSeries>();

            result.Add(FetchDailyUsers(null, null)); //"2015-06-16", "2015-07-16"));
            result.Add(FetchDailyMessages(null, null)); //"2015-06-16", "2015-07-16"));

            return new JsonCamelCaseResult(result, JsonRequestBehavior.AllowGet);
        }

        public long GetTotalMessages(double fromDate, double toDate)
        {
            //Include both from and to dates in calculation.. 
            var rnd = new Random();
            return rnd.Next(0, 100);
        }

        public ActionResult GetPerDayInfo()
        {
            var result = new List<PerDayInfo>();
            var rnd = new Random();
            var currentDate = DateTime.Now.Date;

            var categories = new List<string>()
            {
                "6", "6.30","7","7.30",
                "8", "8.30","9","9.30",
                "10", "10.30","11","11.30",
                "12", "12.30","1","1.30",
                "2", "2.30","3","3.30",
                "4", "4.30","5","5.30",
                "6", "6.30","7","7.30",
                "8", "8.30","9","9.30",
                "10", "10.30","11","11.30",
            };

            for (int j = 100; j > 0; j--)
            {
                var timeSlots = new List<int>();

                foreach(var category in categories)
                {
                    timeSlots.Add(rnd.Next(0, 20));
                }

                result.Add(new PerDayInfo()
                {
                    DateTime = GetUTCNumberFromDate(currentDate.AddDays(-j)),
                    UniqueUsers = rnd.Next(10, 20),
                    UniqueMessages = rnd.Next(20, 30),
                    //TimeSlotCategories = categories,
                    Messages = timeSlots

                });
            }

            return new JsonCamelCaseResult(new {PerDayInfo = result, Categories = categories}, JsonRequestBehavior.AllowGet);
        }

        private double GetUTCNumberFromDate(DateTime TheDate)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = DateTime.SpecifyKind(TheDate, DateTimeKind.Utc);
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);

            return ts.TotalMilliseconds;
        }

        private DateTime GetDateTimeFromUTC(double milliSeconds)
        {
            var original = TimeSpan.FromMilliseconds(milliSeconds);
            DateTime d1 = new DateTime(1970, 1, 1);
            original = original.Add(TimeSpan.FromTicks(d1.Ticks));
            
            DateTime result = new DateTime(original.Ticks , DateTimeKind.Utc);

            return result;
        }
      
    }
}
