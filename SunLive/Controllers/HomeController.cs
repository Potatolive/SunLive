using SunLive.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SunLive.Controllers
{
    public class HomeController : Controller
    {
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

            
            XmlSerializer serializer = new XmlSerializer(typeof(List<FanPost>));
            //StringWriter sww = new StringWriter();
            //XmlWriter writer = XmlWriter.Create(sww);
            //serializer.Serialize(writer, posts);
            //var xml = sww.ToString();

            var directoryPath = Server.MapPath("App_Data");
            var filename = Path.Combine(directoryPath, "input.xml");

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            var posts = (List<FanPost>)serializer.Deserialize(reader);
            fs.Close();


            return View(posts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
