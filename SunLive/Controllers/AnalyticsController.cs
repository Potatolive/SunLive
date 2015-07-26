using Model.Charts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SunLive.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SunLive.Controllers
{
    public class AnalyticsController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }


        public ActionResult GetUniqueTimeSeries()
        {
            var result = new List<ChartSeries>();
            var rnd = new Random();
            var currentDate = DateTime.Now.Date;

            result.Add(new ChartSeries() { Name = "Messages" });
            result.Add(new ChartSeries() { Name = "Users" });
            foreach(var series in result)
            {
                var data = new List<ChartData>();
                for (int j = 100; j > 0; j--)
                {
                    data.Add(new ChartData()
                    {
                        X = MilliTimeStamp(currentDate.AddDays(-j)),
                        Y = rnd.Next(0, 100)
                    });
                }

                series.Data = data;

            }
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
                    DateTime = MilliTimeStamp(currentDate.AddDays(-j)),
                    UniqueUsers = rnd.Next(10, 20),
                    UniqueMessages = rnd.Next(20, 30),
                    //TimeSlotCategories = categories,
                    Messages = timeSlots

                });
            }

            return new JsonCamelCaseResult(new {PerDayInfo = result, Categories = categories}, JsonRequestBehavior.AllowGet);
        }

        public double MilliTimeStamp(DateTime TheDate)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = DateTime.SpecifyKind(TheDate, DateTimeKind.Utc);
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);

            return ts.TotalMilliseconds;
        }

      
    }

    

}
