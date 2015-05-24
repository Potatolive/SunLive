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

namespace SunLive.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            //var posts = new List<FanPost>()
            //{
            //    new FanPost() { PublishedBy = "Jabez", HashTag = "#Chutty TV", PublishedOn = DateTime.Now,
            //        ImageURL = "https://scontent-hkg3-1.xx.fbcdn.net/hphotos-prn2/v/t1.0-9/534026_390622481012854_988468111_n.png?oh=1eb29eadefbabbcee5f07db1eebcbea0&oe=55F9538B",
            //     TextContent = "இந்த இணைய தளத்தின் உள்ளடக்கங்கள் அனைத்தும் தமிழ்நாடு அரசாங்கத்தின் பள்ளிக்கல்வித்துறை"},
            //     new FanPost() { PublishedBy = "Jabez", HashTag = "#Chutty TV", PublishedOn = DateTime.Now,
            //        ImageURL = "https://scontent-hkg3-1.xx.fbcdn.net/hphotos-xpf1/v/t1.0-9/11018331_548036515335257_3977028581894298148_n.jpg?oh=e4c06fa46911dccf7ba7522a707cec7c&oe=56092CF5",
            //     TextContent = "இந்த இணைய தளத்தின் உள்ளடக்கங்கள் அனைத்தும் தமிழ்நாடு அரசாங்கத்தின் பள்ளிக்கல்வித்துறை"},
            //     new FanPost() { PublishedBy = "Jabez", HashTag = "#Chutty TV", PublishedOn = DateTime.Now,
            //        ImageURL = "https://scontent-hkg3-1.xx.fbcdn.net/hphotos-prn2/v/t1.0-9/534026_390622481012854_988468111_n.png?oh=1eb29eadefbabbcee5f07db1eebcbea0&oe=55F9538B",
            //     TextContent = "இந்த இணைய தளத்தின் உள்ளடக்கங்கள் அனைத்தும் தமிழ்நாடு அரசாங்கத்தின் பள்ளிக்கல்வித்துறை"},
            //     new FanPost() { PublishedBy = "Jabez", HashTag = "#Chutty TV", PublishedOn = DateTime.Now,
            //        ImageURL = "https://scontent-hkg3-1.xx.fbcdn.net/hphotos-prn2/v/t1.0-9/534026_390622481012854_988468111_n.png?oh=1eb29eadefbabbcee5f07db1eebcbea0&oe=55F9538B",
            //     TextContent = "இந்த இணைய தளத்தின் உள்ளடக்கங்கள் அனைத்தும் தமிழ்நாடு அரசாங்கத்தின் பள்ளிக்கல்வித்துறை"},
            //     new FanPost() { PublishedBy = "Jabez", HashTag = "#Chutty TV", PublishedOn = DateTime.Now,
            //        ImageURL = "https://scontent-hkg3-1.xx.fbcdn.net/hphotos-prn2/v/t1.0-9/534026_390622481012854_988468111_n.png?oh=1eb29eadefbabbcee5f07db1eebcbea0&oe=55F9538B",
            //     TextContent = "இந்த இணைய தளத்தின் உள்ளடக்கங்கள் அனைத்தும் தமிழ்நாடு அரசாங்கத்தின் பள்ளிக்கல்வித்துறை"}
            //};

            
            //StringWriter sww = new StringWriter();
            //XmlWriter writer = XmlWriter.Create(sww);
            //serializer.Serialize(writer, posts);
            //var xml = sww.ToString();

            /*XmlSerializer serializer = new XmlSerializer(typeof(List<FanPost>));
            var directoryPath = Server.MapPath("App_Data");
            var filename = Path.Combine(directoryPath, "input.xml");

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            var posts = (List<FanPost>)serializer.Deserialize(reader);
            fs.Close();*/

            //var client = new MongoClient(connectionString);
            //MongoServer server = client.GetServer(); //MongoServer.Create(ConfigurationManager.AppSettings["connectionString"]);
            //MongoDatabase myDB = server.GetDatabase("SunLive");
            //MongoCollection<FanPost> postCollection = myDB.GetCollection<FanPost>("fanposts");
            //return View(postCollection.FindAll().AsEnumerable());


            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase("SunLive");
            var collection = myDB.GetCollection<FanPost>("fanposts");

            FieldDefinition<FanPost> field = "FanPost";

            var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "New", "Approved", "Rejected" });

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

            string textcontent = string.Empty;

            if (result.Result.TextContent.Length > 140)
            {
               textcontent = result.Result.TextContent.Substring(0, 140) + " ...";
            }
            else
            {
                textcontent = result.Result.TextContent;
            }

            System.IO.File.WriteAllText(@"C:\TextContent.txt", textcontent + "-" + result.Result.PublishedBy.Split(' ')[0]);

            string localFilename = @"c:\ImageContent.jpg";
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(result.Result.ImageURL, localFilename);
            }

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
