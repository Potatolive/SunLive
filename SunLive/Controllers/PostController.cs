using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SunLive.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
        const int PAGE_SIZE = 50;

        public ActionResult Yesterday(string pageName, string publishedOn)
        {
            var beginDate = String.IsNullOrWhiteSpace(publishedOn)?DateTime.Today:Convert.ToDateTime(publishedOn);
            return new RedirectResult("~/Post/Index/?pageName=" + pageName +  "&publishedOn=" + beginDate.AddDays(-1).ToString("yyyy-MM-dd"));
        }

        public ActionResult Tomorrow(string pageName, string publishedOn)
        {
            var beginDate = String.IsNullOrWhiteSpace(publishedOn) ? DateTime.Today : Convert.ToDateTime(publishedOn);
            return new RedirectResult("~/Post/Index/?pageName=" + pageName + "&publishedOn=" + beginDate.AddDays(1).ToString("yyyy-MM-dd"));
        }

        public ActionResult Next(string pageName, string publishedOn, string page)
        {
            var beginDate = String.IsNullOrWhiteSpace(publishedOn) ? DateTime.Today : Convert.ToDateTime(publishedOn);
            return new RedirectResult("~/Post/Index/?pageName=" + pageName + "&publishedOn=" + beginDate.ToString("yyyy-MM-dd") 
                + "&page=" + (string.IsNullOrEmpty(page)?1:Int32.Parse(page) + 1).ToString());
        }

        public ActionResult Prev(string pageName, string publishedOn, string page)
        {
            var beginDate = String.IsNullOrWhiteSpace(publishedOn) ? DateTime.Today : Convert.ToDateTime(publishedOn);
            return new RedirectResult("~/Post/Index/?pageName=" + pageName + "&publishedOn=" + beginDate.ToString("yyyy-MM-dd") 
                + "&page=" + ((string.IsNullOrEmpty(page) || Int32.Parse(page) <= 0) ? 0 : Int32.Parse(page) - 1).ToString());
        }


        public ActionResult Index(string pageName, string publishedOn, string page)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return View();
            }

            try
            {
                var client = new MongoClient(connectionString);

                var myDB = client.GetDatabase(pageName);
                var collection = myDB.GetCollection<FanPost>("fanposts");

                var beginDate = String.IsNullOrWhiteSpace(publishedOn)?DateTime.Today:Convert.ToDateTime(publishedOn);
                var endDate = beginDate.AddDays(1);

                var filter = (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate.ToString("yyyy-MM-dd")) &
                                Builders<FanPost>.Filter.Lt("PublishedOn", endDate.ToString("yyyy-MM-dd"))) |
                                (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate) &
                                Builders<FanPost>.Filter.Lt("PublishedOn", endDate));
                var sort = Builders<FanPost>.Sort.Ascending("PublishedOn");

                int pageNumber = 0;

                if (!string.IsNullOrWhiteSpace(page)) Int32.TryParse(page, out pageNumber);

                var posts = collection.Find<FanPost>(filter).Skip(PAGE_SIZE * pageNumber).Limit(PAGE_SIZE).Sort(sort).ToListAsync();

                var totalPosts = collection.Find<FanPost>(filter).CountAsync();

                if (((PAGE_SIZE * pageNumber) + posts.Result.Count) > 0)
                {
                    ViewBag.PageInfo = ((PAGE_SIZE * pageNumber) + 1) + "-" + ((PAGE_SIZE * pageNumber) + posts.Result.Count) + " of " + totalPosts.Result;
                }

                ViewBag.PublishedOn = beginDate;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PostCount = posts.Result.Count;
                ViewBag.TodaysCount = TodaysCount(pageName);
                ViewBag.PageName = pageName;

                return View(posts.Result.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return View("Error", model: "Internal Server error. Please try again after sometime. If the problem persists contact admin!");
            }
        }

        public ActionResult Partial(string pageName, string id)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return View("PartialPost");
            }

            var client = new MongoClient(connectionString);

            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");

            FieldDefinition<FanPost> field = "FanPost";

            var filter = Builders<FanPost>.Filter.Eq("_id", id);

            var posts = collection.Find<FanPost>(filter).ToListAsync(); ;

            return View("PartialPost", posts.Result.FirstOrDefault());
        }

        public bool Approve(string pageName, string id)
        {
            return UpdateStatus(pageName, id, "Approved");
        }

        public bool Prioritize(string pageName, string id)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return false;
            }

            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var filter = Builders<FanPost>.Filter.Eq("_id", id);
            var update = Builders<FanPost>.Update.Set("OutputDir", "PriorityOutput");

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            return UpdateStatus(pageName, id, "Approved");
        }

        public bool Reject(string pageName, string id)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return false;
            }

            return UpdateStatus(pageName, id, "Rejected");
        }

        public bool Delete(string pageName, string id)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return false;
            }

            return UpdateStatus(pageName, id, "Delete");
        }

        private bool UpdateStatus(string pageName, string id, string status)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return false;
            }

            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var filter = Builders<FanPost>.Filter.Eq("_id", id);
            var update = Builders<FanPost>.Update.Set("Status", status);

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            return true;
        }

        [HttpPost]
        public ActionResult Search(string pageName, FanPost post)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return View("Index");
            }

            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");
            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            if (!string.IsNullOrEmpty(post.TextContent))
            {

                var filter = Builders<FanPost>.Filter.Regex("TextContent",
                    BsonRegularExpression.Create(
                        new Regex(post.TextContent, RegexOptions.IgnoreCase
                    ))) | Builders<FanPost>.Filter.Regex("PublishedBy",
                    BsonRegularExpression.Create(
                        new Regex(post.TextContent, RegexOptions.IgnoreCase
                    )));
                var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();

                int pageNumber = 0;
                if (((PAGE_SIZE * pageNumber) + posts.Result.Count) > 0)
                {
                    ViewBag.PageInfo = ((PAGE_SIZE * pageNumber) + 1) + "-" + ((PAGE_SIZE * pageNumber) + posts.Result.Count) + " of " + posts.Result.Count;
                }
                
                ViewBag.PublishedOn = DateTime.Today;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PostCount = posts.Result.Count;
                ViewBag.TodaysCount = TodaysCount(pageName);
                ViewBag.PageName = pageName;

                return View("Index", posts.Result.ToList());
            }
            else
            {
                var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "Approved", "Rejected", "New" });
                var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();
                return View("Index", posts.Result.ToList());
            }
        }


        [HttpPost]
        public string Crop(CropData data)
        {

            if (string.IsNullOrWhiteSpace(data.pageName))
            {
                return null;
            }

            string ImgId = data.ImgId;

            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase myDB = client.GetDatabase(data.pageName);

            string fileName = Guid.NewGuid().ToString();
            FanPost post = null;
            try
            {
                var collection = myDB.GetCollection<FanPost>("fanposts");
                FieldDefinition<FanPost> field = "FanPost";
                var filter = Builders<FanPost>.Filter.Eq("_id", ImgId);

                var posts = collection.Find<FanPost>(filter).ToListAsync(); ;

                if (posts != null && posts.Result.Count > 0)
                {
                    post = posts.Result.FirstOrDefault();

                    if (post != null)
                    {
                        if (!string.IsNullOrEmpty(post.ImageURL))
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                Image OriginalImage = null;

                                var directoryPath = Server.MapPath("~");
                                var imageFilename = Path.Combine(directoryPath, "Content/" + fileName + ".jpg");

                                try
                                {
                                    byte[] image = webClient.DownloadData(post.CroppedImageURL);

                                    using (MemoryStream originalFile = new MemoryStream(image))
                                    {
                                        OriginalImage = Image.FromStream(originalFile);
                                        originalFile.Close();
                                    }
                                }
                                catch(Exception ex)
                                {                                
                                    byte[] image = webClient.DownloadData(post.ImageURL);

                                    using (MemoryStream originalFile = new MemoryStream(image))
                                    {
                                        OriginalImage = Image.FromStream(originalFile);
                                        originalFile.Close();
                                    }
                                }

                                float scaledHeight = (float)OriginalImage.Height / (float)data.imageHeight;
                                float scaledWidth = (float)OriginalImage.Width / (float)data.imageWidth;

                                float Width = (float)data.W * (float)scaledWidth;
                                float Height = (float)data.H * (float)scaledHeight;
                                float X = (float)data.X * (float)scaledWidth;
                                float Y = (float)data.Y * (float)scaledHeight;


                                using (Bitmap bmp = new Bitmap((int)Width, (int)Height))
                                {
                                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                                    using (Graphics Graphic = Graphics.FromImage(bmp))
                                    {
                                        Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                        Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, (int)Width, (int)Height),
                                            new Rectangle((int)X, (int)Y, (int)Width, (int)Height), GraphicsUnit.Pixel);
                                        using (MemoryStream outputMs = new MemoryStream())
                                        {
                                            bmp.Save(outputMs, OriginalImage.RawFormat);

                                            using (FileStream file = new FileStream(imageFilename, FileMode.Create, FileAccess.Write))
                                            {
                                                outputMs.WriteTo(file);
                                                file.Close();
                                            }

                                            outputMs.Close();
                                            OriginalImage.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                string dns = ConfigurationManager.AppSettings["dns"].ToString();
                var croppedImageURL = dns + "Content/" + fileName + ".jpg";
                var update = Builders<FanPost>.Update.Set("CroppedImageURL", croppedImageURL);

                collection.FindOneAndUpdateAsync(filter, update);

                //Update url temporarily
                post.CroppedImageURL = croppedImageURL + "?" + Guid.NewGuid().ToString();

                //return PartialView("PartialPost", post);

                return croppedImageURL;

            }
            catch (Exception ex)
            {
                throw new Exception("Image could not be cropped. Internal error. Please try again!");
            }

            return null;


        }

        [HttpPost]
        public string RevertCrop(CropData data)
        {
            if (string.IsNullOrWhiteSpace(data.pageName))
            {
                return null;
            }

            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase myDB = client.GetDatabase(data.pageName);

            string fileName = Guid.NewGuid().ToString();

            try
            {
                var collection = myDB.GetCollection<FanPost>("fanposts");
                FieldDefinition<FanPost> field = "FanPost";
                var filter = Builders<FanPost>.Filter.Eq("_id", data.ImgId);
                var update = Builders<FanPost>.Update.Set("CroppedImageURL", "");

                var posts = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

                if (posts != null && posts.Result != null)
                {
                    FanPost post = posts.Result;
                    return post.ImageURL;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not revert to original image!");
            }

            return string.Empty;
        }

        public long TodaysCount(string pageName)
        {
            try
            {
                var client = new MongoClient(connectionString);

                var myDB = client.GetDatabase(pageName);
                var collection = myDB.GetCollection<FanPost>("fanposts");

                var beginDate = DateTime.Today;
                var endDate = beginDate.AddDays(1);

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
