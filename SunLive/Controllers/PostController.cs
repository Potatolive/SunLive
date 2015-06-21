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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SunLive.Controllers
{
    public class PostController : Controller
    {
        string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
        string pageName = ConfigurationManager.AppSettings["pageName"].ToString();

        public ActionResult Index()
        {
            var client = new MongoClient(connectionString);
            
            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");

            FieldDefinition<FanPost> field = "FanPost";

            var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "New", "Approved", "Rejected", "Downloaded", "Delete", "Deleted" });

            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();

            return View(posts.Result.ToList().Take(40));
        }

        public ActionResult Partial(string id)
        {
            var client = new MongoClient(connectionString);

            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");

            FieldDefinition<FanPost> field = "FanPost";

            var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "New", "Approved", "Rejected", "Downloaded", "Delete", "Deleted" });

            var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

            var posts = collection.Find<FanPost>(filter).Sort(sort).ToListAsync();

            return View("PartialPost", posts.Result.FirstOrDefault());
        }

       

       

        public bool Approve(string id)
        {
            return UpdateStatus(id, "Approved");
        }

        public bool Reject(string id)
        {
            return UpdateStatus(id, "Rejected");
        }

        public bool Delete(string id)
        {
            return UpdateStatus(id, "Delete");
        }

        private bool UpdateStatus(string id, string status)
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase(pageName);
            var collection = myDB.GetCollection<FanPost>("fanposts");

            var filter = Builders<FanPost>.Filter.Eq("_id", id);
            var update = Builders<FanPost>.Update.Set("Status", status);

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            return true;
        }

        [HttpPost]
        public ActionResult Search(FanPost post)
        {
            var client = new MongoClient(connectionString);
            var myDB = client.GetDatabase(pageName);
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


        [HttpPost]
        public string Crop(CropData data)
        {

            string ImgId = data.ImgId;

            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase myDB = client.GetDatabase(pageName);

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
                                var imageFilename = Path.Combine(directoryPath, "Output/" + fileName + ".jpg");

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
                var croppedImageURL = dns + "Output/" + fileName + ".jpg";
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
            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase myDB = client.GetDatabase(pageName);

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
    }
}
