using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DownLoadUpLoadDataService
{
    class EventManager
    {
        //private static bool m_CancelPending = false;
        private static string m_EventSource = "DownLoadUpLoadDataService Events";
        private static string m_EventLog = "DownLoadUpLoadDataService Event Viewer";


        public static void WriteEventErrorMessage(string message, Exception exp)
        {

            if (!EventLog.SourceExists(m_EventSource))
                EventLog.CreateEventSource(m_EventSource, m_EventLog);

            string info = string.Format("{0}\n\n\n{1}\n\n\n{2}", message, exp.Message, exp.StackTrace);
            EventLog.WriteEntry(m_EventSource, info, EventLogEntryType.Error);
        }
        public static void WriteEventInfoMessage(string message)
        {

            if (!EventLog.SourceExists(m_EventSource))
                EventLog.CreateEventSource(m_EventSource, m_EventLog);

            EventLog.WriteEntry(m_EventSource, message, EventLogEntryType.Information);
        }
    }
}
