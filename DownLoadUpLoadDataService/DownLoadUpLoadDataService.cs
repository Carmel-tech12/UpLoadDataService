using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace DownLoadUpLoadDataService
{
    public partial class DownLoadUpLoadDataService : ServiceBase
    {

        bool degelStart = false, degel = false;
        string hour = "", minute = "";
        System.Timers.Timer runCommands_timer = new System.Timers.Timer();
        Engine _hashEngine;
        public DownLoadUpLoadDataService()
        {
            InitializeComponent();
        }
        static void Main()
        {
            //EventLog eventLog =new EventLog();

            //eventLog.Source ="NewSource";

 

            //// Write an entry in the event log.

            //eventLog.WriteEntry("Carmel\logs checking", EventLogEntryType.Warning, 1001);
            //System.Diagnostics.Debugger.Launch();
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new DownLoadUpLoadDataService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);



        }
        protected override void OnStart(string[] args)
        {//לתקן שעה ודקות בכונפיג
            try
            {
                hour = ConfigurationManager.AppSettings["hour"].ToString();
                minute = ConfigurationManager.AppSettings["Minute"].ToString();
            }
            catch
            {
                hour = "2";
                minute = "30";
            }
            degelStart = true;
            degel = true;
            //System.Diagnostics.Debugger.Launch();          
            _hashEngine = new Engine();
            //EventLog eventLog = new EventLog();
            File.AppendAllText(@"C:\Carmel\logs\download.txt", "in on start" + Environment.NewLine);

            //eventLog.Source = "NewSource";
            //eventLog.WriteEntry("afterHashEngine", EventLogEntryType.Warning, 1001);

            runCommands_timer.Interval = 1000 * 60 ;
            runCommands_timer.AutoReset = true;
            runCommands_timer.Elapsed += new System.Timers.ElapsedEventHandler(RunCommands_timer_Elapsed);
            runCommands_timer.Start();
            RunCommands_timer_Elapsed(null, null);

            //EventLog eventLog = new EventLog();

            //eventLog.Source = "NewSource";
            //eventLog.WriteEntry("afterOnStart", EventLogEntryType.Warning, 1001);
        }

        protected override void OnStop()
        {
        }
        void RunCommands_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //EventManager.WriteEventInfoMessage("elapsed now");
            if (degelStart == true)//this if is true only one time (after do start to service)
            {
                if (degel == true)
                {
                    //runCommands_timer.Interval = 5000;
                    runCommands_timer.Interval = 5000;
                    degel = false;
                }
                else
                {
                    runCommands_timer.Stop();
                    //Sets the timer to start working when it reaches real time according to time in app.config
                    //EventManager.WriteEventInfoMessage("start while");
                    //EventLog eventLog = new EventLog();

                    //eventLog.Source = "NewSource";
                    //eventLog.WriteEntry("beforeWhile", EventLogEntryType.Warning, 1001);

                   while ((DateTime.Now.Hour.ToString() != hour || DateTime.Now.Minute.ToString() != minute)) { }
                    //EventLog eventLog = new EventLog();

                    //eventLog.Source = "NewSource";
                    //eventLog.WriteEntry("ENDWHILE", EventLogEntryType.Warning, 1001);
                    //EventManager.WriteEventInfoMessage("finish while");
                    degelStart = false;
                    runCommands_timer.Interval = 1000 * 60 ;
                    runCommands_timer.Start();
                    RunCommands_timer_Elapsed(null, null);

                }
            }
            else
            {
                ReadAllTxtFile();
            }
        }

        void ReadAllTxtFile()
        {
            //EventManager.WriteEventInfoMessage("start read all TxtFile");
            _hashEngine.InsertIntoSp();
            //EventManager.WriteEventInfoMessage("finish read all TxtFile");
        }
    }
}
