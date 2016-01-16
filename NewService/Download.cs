using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Sunlive.Entities;
using Downloader;
using System.Timers;
using System.Threading;

namespace NewService
{
    public partial class Download : ServiceBase
    {
        string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
        string pageName = ConfigurationManager.AppSettings["pageName"].ToString();
        string directoryPath = ConfigurationManager.AppSettings["DIRECTORY_PATH"].ToString();
        string HASHTAG = ConfigurationManager.AppSettings["HASHTAG"].ToString();
        int MAX_CHAR = Int32.Parse(ConfigurationManager.AppSettings["MAX_CHAR"]);
        int MAX_LINE = Int32.Parse(ConfigurationManager.AppSettings["MAX_LINE"]);
        bool multiParts = bool.Parse(ConfigurationManager.AppSettings["multiParts"]);
        int TIMER_INTERVAL = Int32.Parse(ConfigurationManager.AppSettings["TIMER_INTERVAL"]);
        bool downloadProfileInfo = bool.Parse(ConfigurationManager.AppSettings["downloadprofileinfo"]);

        string eventSource = "SunLive";
        string eventLog = "Application";

        System.Timers.Timer timer;

        public Download()
        {
            InitializeComponent();
        }

        private void createLogSource()
        {
            if (!EventLog.SourceExists(eventSource))
                EventLog.CreateEventSource(eventSource, eventLog);
        }

        private static readonly object _locker = new object();

        public void process(object source, ElapsedEventArgs e)
        {
            createLogSource();

            if (!Monitor.TryEnter(_locker))
            {
                EventLog.WriteEntry(eventSource, "Another instance running ...", EventLogEntryType.Information);
                return;
            }

            EventLog.WriteEntry(eventSource, "Processing started ...", EventLogEntryType.Information);

            try
            {
                var client = new MongoClient(connectionString);

                List<string> pages = pageName.Split(',').ToList();

                foreach (string page in pages)
                {
                    var myDB = client.GetDatabase(page);
                    Downloader.DownLoad.processDownload(myDB, directoryPath, MAX_CHAR, MAX_LINE, HASHTAG, multiParts, downloadProfileInfo);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(eventSource, ex.StackTrace, EventLogEntryType.Error);
            }
            finally
            {
                Monitor.Exit(_locker);
            }

            EventLog.WriteEntry("Processing over ...", EventLogEntryType.Information);
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer(TIMER_INTERVAL);
            {
                timer.Elapsed += new ElapsedEventHandler(process);
                timer.Enabled = true;

                timer.Start();

                EventLog.WriteEntry(eventSource, "Timer Started ...", EventLogEntryType.SuccessAudit);
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(eventSource, "OnStop triggered ...", EventLogEntryType.Information);
        }
    }
}
