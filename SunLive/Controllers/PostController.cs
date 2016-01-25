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
using Renci.SshNet;

namespace SunLive.Controllers
{
    //[Authorize]
    public class PostController : Controller
    {
        string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
        const int PAGE_SIZE = 25;

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

                List<string> status = new List<string>() { "New" };

                var filter = (Builders<FanPost>.Filter.In("Status", status)) &
                               (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate.ToString("yyyy-MM-dd")) &
                                Builders<FanPost>.Filter.Lt("PublishedOn", endDate.ToString("yyyy-MM-dd"))) |
                                (Builders<FanPost>.Filter.Gte("PublishedOn", beginDate) &
                                Builders<FanPost>.Filter.Lt("PublishedOn", endDate));
                var sort = Builders<FanPost>.Sort.Descending("PublishedOn");

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

            var filter = Builders<FanPost>.Filter.Eq("_id", id.Replace("/", "\\/"));
            var update = Builders<FanPost>.Update.Set("Status", status);

            var result = collection.FindOneAndUpdateAsync<FanPost>(filter, update);

            return true;
        }

        [HttpPost]
        public string ClearWhatsApp()
        {
            try {
                string keyPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Resources/key/openssh.ppk");

                string key = "-----BEGIN RSA PRIVATE KEY-----" + "\n" +
"MIIEowIBAAKCAQEAkD/L00L5cIUrqoAsUg6Ifb0O4ExdXIfO6XVub0Of/DppuBtd" + "\n" +
"/VXz138iUtP87K8ZaKNCFB9ZOtQjOyB5FWXnlL6q3uprZEKcZohczhx3jWIw+6BR" + "\n" +
"Uen8QBRAvdVhiElEBYtx98F78DAOJlLD9wV4kCHSV/hNAasB37EtS8EQa/yn7ruf" + "\n" +
"K30Q1J+bk5x/Vte+4kYx6wM6xGxlmu6f4rVMquJF/ejghor+AAuxw7+NjCXZ4lid" + "\n" +
"NXA2LpCZ5hSf4mOmm2eoTJaZs3AXHZY9fCJRvnYcXbbCugKneIZg/lEj18AZsm8F" + "\n" +
"6BXKnpfhQzm420o/0Q2dAmKvOJJHw6kKKwj5PQIDAQABAoIBAQCJwZMqtyw168eu" + "\n" +
"tWceGij5Q6LAS81hP4U3aOFFOqS/oR0zKFeTRxFufEhQJ4jEk9fFGRxS0TlKuCrJ" + "\n" +
"HZLk/4OwhoHyDpbukbqBJLrUT1VQ3TQAKbNfcgBnRbAqWmwhVi6yyN/XBp0Q3SO2" + "\n" +
"L5ZcAoqHwvT48/S+ogxRDwg97yt6o7u8iecltKbKpuALf5eTkqH4F3DesUzyCs6A" + "\n" +
"o5DVknXgN5BqfcaJSml4f4V7g7inaoFsKRTQXVFwUCS9PGOp5LhW1nN+AYaKKaBT" + "\n" +
"n62pEQqy+ILBu1QFJVObZtg9aCEQmyetjfsXJWJMgrgV/SGoiZcLWdzt0bKrKqt2" + "\n" +
"y9VaJ9lhAoGBAPqrrIn8nWLAsmM9rL/BEfi+KsmfnHwCIdRwYzE9lSFlaWITWrZk" + "\n" +
"eDL97jFbWbdYzZREsJsi2wS0ZmWFjzQdf7q9o5+zYpiQ8wx8xuFts6VD+gGkp88v" + "\n" +
"pDVyLT9EiNt5/vskdIj8x4+DTqw/zbxBpy7HPh0EdvZnW9GNRROWMlYpAoGBAJNQ" + "\n" +
"5ui3AyH6wZoutkNN9D/66XLUo3vMmpyz/8Zit+Sc1uD8wrAm1r13M0Y9SgrawZ1F" + "\n" +
"wOfovyC9vhXXYowkiVYjaYnOJeL1LOPUoUwojNYsy/bytPMBv0fDziLsUyNyvadZ" + "\n" +
"EEY+V0j1/IP3NRBDe1n02imJT9BXVR7AaqmbIOT1AoGAAVewCeEneqLrkap/5VsE" + "\n" +
"XJ+wHPpU3TkpsziS322kAdTINrVB1B4/oo5Hm04Q8fFw0G15wKr0H1dUARExDidm" + "\n" +
"Srq/SJiuW4DTPGriqcxrnOP7T8zw9SQdLggZg/A7B2nk2rV8RkuMShF692M0F+EG" + "\n" +
"IzL/+ynN9U3iaQHnr84rINkCgYBWDuPlvMvitMcWmAU3ijmOiriHsXqTnrIPqwNX" + "\n" +
"VGIS9iB9LItbNkUqR5E3jiRL9QE4LACGOaw1p0J9JebW8Z0dKfDEZR4y2IFR0uwr" + "\n" +
"PmEP2PmKGLzmXPXuKY+pTR9ATQ5Hzbq5HkAFSlYqjWZ9Sr6rjWNI8oMitXHvVf65" + "\n" +
"d/seZQKBgHyfHdCBikac/i3ynq4nprtWwsX4kikisIEHFeBBtpJdIwgRekyrrzT7" + "\n" +
"SQROCtLtfkCe5bcPvcGmfZmaLX0uzYpzd/EK759ayqVWffNdtrJ3DQZ4zW4zYWMk" + "\n" +
"NsBPp+gj9ruToZH3a7/4Eq2TrhJO9vPdeeJrrTMuoY8jN9WFZpQl" + "\n" +
"-----END RSA PRIVATE KEY-----";
                ;


                //var keyFile = new PrivateKeyFile(@"E:\Ganesan\connectivity\openssh.ppk");

                using (Stream s = GenerateStreamFromString(key))
                {
                    var keyFile = new PrivateKeyFile(s);
                    var keyFiles = new[] { keyFile };
                    var username = "ubuntu";

                    var methods = new List<AuthenticationMethod>();
                    methods.Add(new PasswordAuthenticationMethod(username, "#infy123"));
                    methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

                    var con = new ConnectionInfo("52.32.74.255", 22, username, methods.ToArray());
                    SshCommand cmd;
                    using (var client = new SshClient(con))
                    {
                        client.Connect();

                        cmd = client.CreateCommand("/home/ubuntu/clearwhatsapp/workaround.sh");
                        cmd.Execute();

                        client.Disconnect();
                    }

                    return cmd.ExitStatus.ToString();
                }
                
            }
            catch (Exception ex)
            {
                return ex.StackTrace.ToString() + "\n" + ex.ToString();
            }

            
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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

                                if (data.imageWidth <= 0 && data.imageHeight <= 0)
                                {
                                    data.imageWidth = OriginalImage.Width;
                                    data.imageHeight = OriginalImage.Height;
                                }

                                if (data.imageWidth > 0 && data.imageHeight <= 0)
                                {
                                    data.imageHeight = (int) ((float)OriginalImage.Height / ((float)OriginalImage.Width / (float)data.imageWidth));
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
