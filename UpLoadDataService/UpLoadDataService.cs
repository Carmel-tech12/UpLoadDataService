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


namespace UpLoadDataService
{
    
    public partial class UpLoadDataService : ServiceBase
    {
        public static int numService = 0;
        bool degelStart = false, degel = false;
        string hour = "", minute = "";
        System.Timers.Timer runCommands_timer = new System.Timers.Timer();
        Engine _hashEngine;
        public UpLoadDataService()
        {
            InitializeComponent();
        }


        // The main entry point for the process
        static void Main()
        {
         //   System.Diagnostics.Debugger.Launch();
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new UpLoadDataService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);

        }

        protected override void OnStart(string[] args)
        {            
            try
            {
                File.AppendAllText(@"C:\orly\logUpload.txt", "try load hour" + Environment.NewLine);
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
            File.AppendAllText(@"C:\orly\logUpload.txt", "before new hashengine" + Environment.NewLine);
            _hashEngine = new Engine();
            runCommands_timer.Interval = 1000 * 60  ;
            runCommands_timer.AutoReset = true;
            runCommands_timer.Elapsed += new System.Timers.ElapsedEventHandler(RunCommands_timer_Elapsed);
            runCommands_timer.Start();
            RunCommands_timer_Elapsed(null, null);        
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
                    runCommands_timer.Interval = 5000;
                    degel = false;
                }
                else
                {
                    runCommands_timer.Stop();
                    //Sets the timer to start working when it reaches real time according to time in app.config
                    //EventManager.WriteEventInfoMessage("start while");
                    File.AppendAllText(@"C:\orly\logUpload.txt", "before while" + Environment.NewLine);
                    while ((DateTime.Now.Hour.ToString() != hour|| DateTime.Now.Minute.ToString() != minute)) { }
                    File.AppendAllText(@"C:\orly\logUpload.txt", "after while" + Environment.NewLine);
                    //EventManager.WriteEventInfoMessage("finish while");
                    degelStart = false;
                    runCommands_timer.Interval = 1000 * 60  ;
                    runCommands_timer.Start();
                    RunCommands_timer_Elapsed(null, null);

                }
            }
            else
            {
                numService++;
                ReadAllProcedures();
                ReadAllQueries();
            }            
        }

        //ADDED by Raz 24/1/2019
        //void SendCallsForEmailsAndSMSs()
        //{
        //    _hashEngine.SendCallsForEmailsAndSMSs();
        //}

        void ReadAllProcedures()
        {
            //EventManager.WriteEventInfoMessage("start read all procerures");
            File.AppendAllText(@"C:\orly\logUpload.txt", "before proceduress" + Environment.NewLine);
            //System.Diagnostics.Debugger.Launch();
            _hashEngine.LoadDataOfProcedurs();
            //EventManager.WriteEventInfoMessage("finish read all procerures");
        }

        void ReadAllQueries()
        {
            //EventManager.WriteEventInfoMessage("start read all queries");
            File.AppendAllText(@"C:\orly\logUpload.txt", "before queries" + Environment.NewLine);
            _hashEngine.LoadDataOfQueries();
            //EventManager.WriteEventInfoMessage("finish read all queries");
        }

    }
}
