using MongoDB.Driver;
using Sunlive.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Downloader
{
    public static class DownLoad
    {
        public static void processDownload(IMongoDatabase myDB, string directoryPath, int MAX_CHAR, int MAX_LINE, string HASHTAG, bool multiParts)
        {
            var collection = myDB.GetCollection<FanPost>("fanposts");

            FieldDefinition<FanPost> field = "FanPost";

            var filter = Builders<FanPost>.Filter.In("Status", new List<String>() { "Approved" });

            var posts = collection.Find<FanPost>(filter).ToListAsync();

            if (posts != null && posts.Result != null)
            {
                foreach (FanPost post in posts.Result)
                {

                    Guid guid = Guid.NewGuid();

                    var filterPost = Builders<FanPost>.Filter.Eq("_id", post._id);
                    var update = Builders<FanPost>.Update.Set("Status", "Downloaded").Set("FileName", guid.ToString());
                    var result = collection.FindOneAndUpdateAsync<FanPost>(filterPost, update);

                    if (!string.IsNullOrEmpty(post.ImageURL))
                    {

                        string outputDir = "output";

                        if (!string.IsNullOrWhiteSpace(post.OutputDir))
                        {
                            outputDir = post.OutputDir;
                        }

                        string url = string.Empty;

                        if (!string.IsNullOrEmpty(post.CroppedImageURL))
                        {
                            url = post.CroppedImageURL;
                        }
                        else
                        {
                            url = post.ImageURL;
                        }

                        string textContent = removeHashTag(post.TextContent, HASHTAG);

                        List<List<string>> parts = TextParts.getParts(textContent, post.PublishedBy.Split(' ')[0], MAX_CHAR, MAX_LINE, multiParts);

                        int partNumber = 0;

                        foreach (var part in parts)
                        {
                            
                            try
                            {
                                string directory = directoryPath + "/" + outputDir + "/";
                                string fileName = guid + "_" + partNumber++;

                                var imageFilename = Path.Combine(directory, fileName + ".jpg");
                                downloadImage(imageFilename, url);

                                var textContentFilename = Path.Combine(directory, fileName + ".txt");
                                downloadText(textContentFilename, part);
                            }
                            catch (Exception ex)
                            {
                                
                            }

                            result.Wait();
                        }

                        
                    }
                }
            }
        }

        private static string removeHashTag(string textcontent, string HASHTAG)
        {
            if (string.IsNullOrWhiteSpace(textcontent) || string.IsNullOrWhiteSpace(HASHTAG)) return textcontent;

            Regex regEx = new Regex(@"(?<=#)\w+",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string withoutHashtag = regEx.Replace(textcontent, string.Empty);

            return withoutHashtag.Replace('#', ' ');
        }

        private static void downloadText(string textContentFilename, List<string> lines)
        {
            System.IO.File.WriteAllLines(textContentFilename, lines);
        }

        private static void downloadImage(string imageFilename, string url)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, imageFilename);
            }
        }
    }
}
