using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    enum InteractionType
    {
        MESSAGE,
        PICTURE,
        VIDEO
    }

    enum FacebookFeedType
    {
        PAGE,
        POST,
        APP
    }

    class DownloadConfiguration
    {
        public int MaxLines { get; set; }
        public int MaxChar { get; set; }
        public bool MultiPart { get; set; }
        public string DownloadPath { get; set; }
    }

    class FacebookFeedConfiguration
    {
        public string FeedName { get; set; }
        public FacebookFeedType FeedType { get; set; }
        public List<InteractionType> InteractionTypes { get; set; }
        public string PageName { get; set; }
        public List<string> HashTagToFilter { get; set; }
        public List<string> PostHashTag { get; set; }
        public DownloadConfiguration DownloadConfiguration { get; set; }
    }

    class TwitterFeedConfiguration
    {
        public string FeedName { get; set; }
        public List<string> TwitterHandles { get; set; }
        public List<string> TwitterHashTag { get; set; }
        public DownloadConfiguration DownloadConfiguration { get; set; }
    }
}
