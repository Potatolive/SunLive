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
using System.Web.Http;
using System.Web.Mvc;

namespace SunLive.Controllers
{
    //[Authorize]
    public class V2Controller : Controller
    {
        const int PAGE_SIZE = 50;
        //IMongoDatabase MongoDB = null;

        public IMongoDatabase GetDatabase(string pageName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            MongoClient client = new MongoClient(connectionString);

            return client.GetDatabase(pageName);
        }


        public V2Controller()
        {
            //Need to make this config driven
            //MongoDB = client.GetDatabase("AdithyaTV");
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public JsonResult Latest(DayInfo dayInfo)
        {
            string pageName = dayInfo.PageName;
            string publishedOn = dayInfo.Date.ToString("yyyy-MM-dd");
            int pageNumber = dayInfo.CurrentPage;

            var myDB = GetDatabase(pageName);

            var collection = myDB.GetCollection<FanPost>("fanposts");

            var endDate = String.IsNullOrWhiteSpace(publishedOn) ? DateTime.Today : Convert.ToDateTime(publishedOn).AddHours(dayInfo.SelectedHour).AddMinutes(dayInfo.SelectedMin + 1);
            var beginDate = endDate.AddDays(-1);

            beginDate = beginDate.AddHours(-5.5);
            endDate = endDate.AddHours(-5.5);

            FilterDefinition<FanPost> sourceFilter = null;
            if (dayInfo.Source.ToLower() == "facebook")
            {
                sourceFilter = (Builders<FanPost>.Filter.Exists("Source", false));
                beginDate = endDate.AddDays(-1);
            }
            else if (dayInfo.Source.ToLower() == "whatsapp")
            {
                sourceFilter = Builders<FanPost>.Filter.Eq("Source", "WHATZAPP");
            }

            List<string> status = new List<string>() { "New" };

            var filter = (Builders<FanPost>.Filter.In("Status", status)) &
                            (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate.ToString("yyyy-MM-ddTHH:mm:ss")) &
                            Builders<FanPost>.Filter.Lt("PublishedOn", endDate.ToString("yyyy-MM-ddTHH:mm:ss"))) |
                            (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate) &
                            Builders<FanPost>.Filter.Lt("PublishedOn", endDate));


            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            List<FanPost> posts = null;
            long totalPosts = 0;

            if (sourceFilter == null)
            {
                var postsHandle = collection.Find<FanPost>(filter).Skip(PAGE_SIZE * pageNumber).Limit(PAGE_SIZE).Sort(sort).ToListAsync();
                posts = postsHandle.Result;
                var totalPostsHandle = collection.Find<FanPost>(filter).CountAsync();
                totalPosts = totalPostsHandle.Result;
            }
            else
            {
                var postsHandle = collection.Find<FanPost>(filter & sourceFilter).Skip(PAGE_SIZE * pageNumber).Limit(PAGE_SIZE).Sort(sort).ToListAsync();
                posts = postsHandle.Result;
                var totalPostsHandle = collection.Find<FanPost>(filter & sourceFilter).CountAsync();
                totalPosts = totalPostsHandle.Result;
            }

            dayInfo.Posts = posts;
            dayInfo.TotalPages = 0;

            dayInfo.TodaysCount = TodaysCount(pageName);

            return Json(dayInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Posts([FromUri] DayInfo dayInfo)
        {
            if (dayInfo.Latest) return Latest(dayInfo);

            string pageName = dayInfo.PageName;
            string publishedOn = dayInfo.Date.ToString("yyyy-MM-dd"); 
            int pageNumber = dayInfo.CurrentPage;

            var myDB = GetDatabase(pageName);
                
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var endDate = String.IsNullOrWhiteSpace(publishedOn) ? DateTime.Today : Convert.ToDateTime(publishedOn).AddHours(dayInfo.SelectedHour).AddMinutes(dayInfo.SelectedMin + 1);
            var beginDate = endDate.AddMinutes(-5);

            beginDate = beginDate.AddHours(-5.5);
            endDate = endDate.AddHours(-5.5);

            FilterDefinition<FanPost> sourceFilter = null;
            if (dayInfo.Source.ToLower() == "facebook")
            {
                sourceFilter = (Builders<FanPost>.Filter.Exists("Source", false));
                beginDate = endDate.AddDays(-1);
            }
            else if (dayInfo.Source.ToLower() == "whatsapp")
            {
                sourceFilter = Builders<FanPost>.Filter.Eq("Source", "WHATZAPP");
            }

            List<string> status = new List<string>() { "New" };

            var filter = (Builders<FanPost>.Filter.In("Status", status)) &
                            (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate.ToString("yyyy-MM-ddTHH:mm:ss")) &
                            Builders<FanPost>.Filter.Lt("PublishedOn", endDate.ToString("yyyy-MM-ddTHH:mm:ss"))) |
                            (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate) &
                            Builders<FanPost>.Filter.Lt("PublishedOn", endDate));

            
            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            List<FanPost> posts = null;
            long totalPosts = 0;

            if(sourceFilter == null)
            {
                var postsHandle = collection.Find<FanPost>(filter).Skip(PAGE_SIZE * pageNumber).Limit(PAGE_SIZE).Sort(sort).ToListAsync();
                posts = postsHandle.Result;
                var totalPostsHandle = collection.Find<FanPost>(filter).CountAsync();
                totalPosts = totalPostsHandle.Result;
            }
            else
            {
                var postsHandle = collection.Find<FanPost>(filter & sourceFilter).Skip(PAGE_SIZE * pageNumber).Limit(PAGE_SIZE).Sort(sort).ToListAsync();
                posts = postsHandle.Result;
                var totalPostsHandle = collection.Find<FanPost>(filter & sourceFilter).CountAsync();
                totalPosts = totalPostsHandle.Result;
            }
            
            dayInfo.Posts = posts;
            dayInfo.TotalPages = (int) Math.Ceiling((double)(totalPosts / PAGE_SIZE));
            dayInfo.Date = beginDate;

            dayInfo.TodaysCount = TodaysCount(pageName);

            return Json(dayInfo, JsonRequestBehavior.AllowGet);
        }


        public long TodaysCount(string pageName)
        {
            try
            {
                var myDB = GetDatabase(pageName);

                var collection = myDB.GetCollection<FanPost>("fanposts");

                var beginDate = DateTime.Today;
                var endDate = beginDate.AddDays(1);

                beginDate = beginDate.AddHours(-5.5);
                endDate = endDate.AddHours(-5.5);

                var filter = (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate.ToString("yyyy-MM-dd")) &
                                Builders<FanPost>.Filter.Lt("PublishedOn", endDate.ToString("yyyy-MM-dd"))) |
                                (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate) &
                                Builders<FanPost>.Filter.Lt("PublishedOn", endDate));

                

                var sort = Builders<FanPost>.Sort.Ascending("PublishedOn");

                var totalPosts = collection.Find<FanPost>(filter).CountAsync();

                return totalPosts.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Internal error");
            }
        }
    }
}
