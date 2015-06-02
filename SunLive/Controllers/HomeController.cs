using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SunLive.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Net;
using Sunlive.Entities;

namespace SunLive.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        public ActionResult Index()
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase("SunLive");
            var collection = myDB.GetCollection<FanPost>("fanposts");

            FieldDefinition<FanPost> field = "FanPost";

            var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "New", "Approved", "Rejected", "Downloaded", "Delete", "Deleted" });

            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();

            return View(posts.Result.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public RedirectToRouteResult Details(string id)
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase("SunLive");
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var filter = Builders<FanPost>.Filter.Eq("_id", id);
            var update = Builders<FanPost>.Update.Set("Status", "Approved");

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            /*string textcontent = string.Empty;

            if (result.Result.TextContent.Length > 140)
            {
               textcontent = result.Result.TextContent.Substring(0, 140) + " ...";
            }
            else
            {
                textcontent = result.Result.TextContent;
            }

            var directoryPath = Server.MapPath("~");
            var textContentFilename = Path.Combine(directoryPath, "Output/TextContent.txt");
            var imageFilename = Path.Combine(directoryPath, "Output/ImageContent.jpg");

            System.IO.File.WriteAllText(textContentFilename, textcontent + " -" + result.Result.PublishedBy.Split(' ')[0]);

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(result.Result.ImageURL, imageFilename);
            }*/

            return RedirectToAction("Index");
        }

        public RedirectToRouteResult Reject(string id)
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase("SunLive");
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var filter = Builders<FanPost>.Filter.Eq("_id", id);
            var update = Builders<FanPost>.Update.Set("Status", "Rejected");

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            return RedirectToAction("Index");
        }

        public RedirectToRouteResult Delete(string id)
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase("SunLive");
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var filter = Builders<FanPost>.Filter.Eq("_id", id);
            var update = Builders<FanPost>.Update.Set("Status", "Delete");

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Search(FanPost post)
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase("SunLive");
            var collection = myDB.GetCollection<FanPost>("fanposts");
            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            if (!string.IsNullOrEmpty(post.TextContent))
            {

                var filter = Builders<FanPost>.Filter.Regex("TextContent",
                    BsonRegularExpression.Create(
                        new Regex(post.TextContent, RegexOptions.IgnoreCase
                    )));
                var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();
                return View("Index", posts.Result.ToList());
            }
            else
            {
                var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "Approved", "Rejected", "New" });
                var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();
                return View("Index", posts.Result.ToList());
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
