using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SunLive
{
    public class AppConstants
    {

        public class AppSettings
        {
            protected static String GetAppSetting(String key)
            {
                return ConfigurationManager.AppSettings[key];
            }

            protected static void SetAppSetting(String key, string value)
            {
                ConfigurationManager.AppSettings[key] = value;
            }


            protected static readonly String twitterConsumerKey = "TwitterConsumerKey";
            public static string TwitterConsumerKey
            {
                get { return GetAppSetting(twitterConsumerKey); }
            }

            protected static readonly string twitterConsumerSecret = "TwitterConsumerSecret";

            public static string TwitterConsumerSecret
            {
                get { return GetAppSetting(twitterConsumerSecret); }
            }

            protected static readonly String twitterAccessToken = "TwitterAccessToken";
            public static string TwitterAccessToken
            {
                get { return GetAppSetting(twitterAccessToken); }
            }

            protected static readonly String twitterAccessTokenSecret = "TwitterAccessTokenSecret";
            public static string TwitterAccessTokenSecret
            {
                get { return GetAppSetting(twitterAccessTokenSecret); }
            }


            protected static readonly String fbAppId = "FBAppId";

            public static string FBAppId
            {
                get { return GetAppSetting(fbAppId); }
            }

            protected static readonly String fbAppSecret = "FBAppSecret";

            public static string FBAppSecret
            {
                get { return GetAppSetting(fbAppSecret); }
            }


            protected static readonly string microsoftClientId = "MicrosoftClientId";

            public static string MicrosoftClientId
            {
                get { return GetAppSetting(microsoftClientId); }
            }

            protected static readonly string microsoftClientSecret = "MicrosoftClientSecret";

            public static string MicrosoftClientSecret
            {
                get { return GetAppSetting(microsoftClientSecret); }
            }
        }
    }
}