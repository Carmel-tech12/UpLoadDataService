using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Net;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using System.IO.Compression;


using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using System.Data.SqlClient;

namespace DownLoadUpLoadDataService
{
    class Engine
    {

        private HttpWebRequest request;
        private CookieContainer cookieContainer = new CookieContainer();
        private static string m_ConnectionString = string.Empty;
        public static string m_folderpath = string.Empty;
        public static string m_user = string.Empty;
        public static string m_pass = string.Empty;
        public static string m_url = string.Empty;
        public static string csv_file_path = string.Empty;
        public static string m_folderpathforzip = string.Empty;
        public static string m_userforzip = string.Empty;
        public static string m_passforzip = string.Empty;
        public static string m_urlxml = string.Empty;
        public static string m_zipPath = string.Empty;
        XmlDocument doc = new XmlDocument();

        public Engine()
        {
            LoadGlobalParameters();
        }

        private void LoadGlobalParameters()
        {
            try
            {
                // doc.Load("Config.xml");
                // doc.Load(@"C:\Program Files\UpLoadDataService\UpLoadDataService\bin\Debug\Config.xml");
                File.AppendAllText(@"C:\carmel\logs\download.txt", "try load pathXml" + Environment.NewLine);
                doc.Load(@"" + ConfigurationManager.AppSettings["pathXml"]);
                File.AppendAllText(@"C:\carmel\logs\download.txt", "success load pathXml" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\carmel\logs\download.txt", "fail load pathXml" + ex.Message + Environment.NewLine);
                //EventManager.WriteEventErrorMessage("can not load the doc", ex);
            }
            try
            {

                File.AppendAllText(@"C:\carmel\logs\download.txt", "before parameters" + Environment.NewLine);
                m_ConnectionString = doc.SelectSingleNode(@"Parameters/ConenctionString").InnerText + "&quot;";
                m_user = doc.SelectSingleNode(@"Parameters/Name").InnerText;
                m_pass = doc.SelectSingleNode(@"Parameters/Pass").InnerText;
                m_url = doc.SelectSingleNode(@"Parameters/Url").InnerText;
                m_zipPath = doc.SelectSingleNode(@"Parameters/folderpathforzip").InnerText;
                m_userforzip = doc.SelectSingleNode(@"Parameters/Nameforzip").InnerText;
                m_passforzip = doc.SelectSingleNode(@"Parameters/Passforzip").InnerText;
                m_urlxml = doc.SelectSingleNode(@"Parameters/UrlXml").InnerText;
                m_folderpath = doc.SelectSingleNode(@"Parameters/folderpath").InnerText;
                m_folderpathforzip = doc.SelectSingleNode(@"Parameters/folderpathforzip").InnerText;
                ConnectionManager.ConnectionString = m_ConnectionString;
                ConnectionManager.Provider = doc.SelectSingleNode(@"Parameters/Provider").InnerText;
                csv_file_path = ConfigurationManager.AppSettings["pathofcsvfile"].ToString();
                File.AppendAllText(@"C:\carmel\logs\download.txt", "after parameters" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\carmel\logs\download.txt", "fail parameters, ex: " + ex + Environment.NewLine);
            }
        }
        public static bool degel = true;
        public void InsertIntoSp()
        {
            try
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "enter InsertIntoSp" + Environment.NewLine);
                //EventManager.WriteEventInfoMessage("start ReadXml");
                // DataTable tblML = ConvertToDataTableML(csv_file_path, 25);
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "before ReadXmlForCA_RequestDetails" + Environment.NewLine);
                DataTable tblML = ReadXmlForCA_RequestDetails();
                // EventManager.WriteEventInfoMessage("finish ConvertToDataTable ML from TxtFile");
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "after ReadXmlForCA_RequestDetails" + Environment.NewLine);
                //EventManager.WriteEventInfoMessage("start insert the data to PsRequestDetails from xml");

                File.AppendAllText(@"C:\Carmel\logs\download.txt", "before AddIntoPsRequestDetails" + Environment.NewLine);
                AddIntoPsRequestDetails(tblML);
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "after AddIntoPsRequestDetails" + Environment.NewLine);
                //EventManager.WriteEventInfoMessage("finish insert the data to PsRequestDetails from xml");

                //need to open zip file here and extract
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "before zipFiles" + Environment.NewLine);
                zipFiles("");
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "after zipFiles" + Environment.NewLine);
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "end InsertIntoSp" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail InsertIntoSp ex: " + ex + Environment.NewLine);
            }
            //  EventManager.WriteEventInfoMessage("start ConvertToDataTable CL from TxtFile");
            //DataTable tblCL = ConvertToDataTableCL(csv_file_path, 11);
            //  EventManager.WriteEventInfoMessage("finish ConvertToDataTable CL from TxtFile");

            //EventManager.WriteEventInfoMessage("start AddIntoPsRequestDetails CL from TxtFile");
            //   CA_RequestLineDetailsPs(tblCL);
            //   EventManager.WriteEventInfoMessage("finish AddIntoPsRequestDetails CL from TxtFile");

            //EventManager.WriteEventInfoMessage("start InsertToFiles from TxtFile");
            //DataTable tfiles = InsertToFiles(tblCL);
            //EventManager.WriteEventInfoMessage("finish InsertToFiles from TxtFile");
        }
        public static DataTable ReadXmlForCA_RequestDetailsBack()
        {
            string url = @"https://chaina-motorsltd.dira2.co.il/chainamotors/systems/integrationFile.xml";
            //  string get = p1.getData(m_userforzip, m_passforzip, url);
            Engine e = new Engine();
            // EventManager.WriteEventInfoMessage("try to do login");
            string get = e.getData(m_userforzip, m_passforzip, url);
            // EventManager.WriteEventInfoMessage("finish to do login");

            // EventManager.WriteEventInfoMessage("start to loadxml");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(get);
            //xmlDoc.Load(get);
            // EventManager.WriteEventInfoMessage("finish to loadxml");
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("datum");
            //   string id = "", idezer = "", car_mile_rage = "", failure_request_type = "", failure_request_rsn = "", failure_request_desc = "", description = "", time_car_entered = "";
            string ReqNo = "", RequestStts = "", ApproverName = "", LicNum = "", KM = "", RequestDt = "", RequestTypeCd = "", RequestTypeNm = "", RequestRsnCd = "", RequestRsnNm = "", RequestCd = "", RequestDesc = "", Notes = "", InternalNotes = "", CampaignCd = "", CampaignNm = "", OpenDt = "", FirstRspnsDt = "", EntryDt = "", EntryHr = "", ExitDt = "", ExitHr = "", Garage = "", EstCost = "";


            DataTable dt = new DataTable();
            for (int col = 0; col < 24; col++)
                dt.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            //     DataTable dt = ConvertToDataTableML(csv_file_path, 6);
            DataRow dr, drLine;
            //מתבצעת ריצה על סוג בקשה בקריאה
            DataTable dtLine = new DataTable();
            for (int col = 0; col < 8; col++)
                dtLine.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            foreach (XmlNode node in nodeList)
            {
                dr = dt.NewRow();
                ReqNo = node.SelectSingleNode("ReqNo").InnerText;
                //מתבצעת ריצה על סוג בקשה בקריאה
                // XmlNodeList nodeListline = xmlDoc.GetElementsByTagName("orderline");

                XmlNodeList nodeListline = node.SelectNodes("orderlines"); //  can also use XPath here

                foreach (XmlNode nodeline in nodeListline)
                {
                    XmlNodeList nodeListline2 = nodeline.SelectNodes("orderline");
                    foreach (XmlNode nodeli in nodeListline2)
                    {

                        // string idreq = "", product_id = "", quantity = "", prodname = "", issue_type = "", status = "", item_key = "";
                        string LineId = "", ItemCode = "", Qntty = "", RequestSttsline = "", RejectRsn = "", BillTo = "", InternalNotesline = "";
                        LineId = nodeli.SelectSingleNode("LineId").InnerText;
                        ItemCode = nodeli.SelectSingleNode("ItemCode").InnerText;
                        Qntty = nodeli.SelectSingleNode("Qntty").InnerText;
                        RequestSttsline = nodeli.SelectSingleNode("RequestSttsline").InnerText;
                        RejectRsn = nodeli.SelectSingleNode("RejectRsn").InnerText;
                        BillTo = nodeli.SelectSingleNode("BillTo").InnerText;
                        InternalNotesline = nodeli.SelectSingleNode("InternalNotesline").InnerText;

                        drLine = dtLine.NewRow();
                        //הכנסת נתונים לטבלה
                        drLine[0] = ReqNo;
                        drLine[1] = LineId;
                        drLine[2] = ItemCode;
                        drLine[3] = Qntty;
                        drLine[4] = RequestSttsline;
                        drLine[5] = RejectRsn;
                        drLine[6] = BillTo;
                        drLine[7] = InternalNotesline;
                        dtLine.Rows.Add(drLine);
                    }
                }
                RequestStts = node.SelectSingleNode("RequestStts").InnerText;
                ApproverName = node.SelectSingleNode("ApproverName").InnerText;
                LicNum = node.SelectSingleNode("LicNum").InnerText;
                KM = node.SelectSingleNode("KM").InnerText;
                RequestDt = node.SelectSingleNode("RequestDt").InnerText;
                RequestTypeCd = node.SelectSingleNode("RequestTypeCd").InnerText;
                RequestTypeNm = node.SelectSingleNode("RequestTypeNm").InnerText;
                RequestRsnCd = node.SelectSingleNode("RequestRsnCd").InnerText;
                RequestRsnNm = node.SelectSingleNode("RequestRsnNm").InnerText;
                RequestCd = node.SelectSingleNode("RequestCd").InnerText;
                RequestDesc = node.SelectSingleNode("RequestDesc").InnerText;
                Notes = node.SelectSingleNode("Notes").InnerText;
                InternalNotes = node.SelectSingleNode("InternalNotes").InnerText;
                CampaignCd = node.SelectSingleNode("CampaignCd").InnerText;
                CampaignNm = node.SelectSingleNode("CampaignNm").InnerText;
                OpenDt = node.SelectSingleNode("OpenDt").InnerText;
                FirstRspnsDt = node.SelectSingleNode("FirstRspnsDt").InnerText;
                EntryDt = node.SelectSingleNode("EntryDt").InnerText;
                EntryHr = node.SelectSingleNode("EntryHr").InnerText;
                ExitDt = node.SelectSingleNode("ExitDt").InnerText;
                ExitHr = node.SelectSingleNode("ExitHr").InnerText;
                Garage = node.SelectSingleNode("Garage").InnerText;
                EstCost = node.SelectSingleNode("EstCost").InnerText;
                dr[0] = ReqNo;
                dr[1] = RequestStts;
                dr[2] = ApproverName;
                dr[3] = LicNum;
                dr[4] = KM;
                dr[5] = RequestDt;
                dr[6] = RequestTypeCd;
                dr[7] = RequestTypeNm;
                dr[8] = RequestRsnCd;
                dr[9] = RequestRsnNm;
                dr[10] = RequestCd;
                dr[11] = RequestDesc;
                dr[12] = Notes;
                dr[13] = InternalNotes;
                dr[14] = CampaignCd;
                dr[15] = CampaignNm;
                dr[16] = OpenDt;
                dr[17] = FirstRspnsDt;
                dr[18] = EntryDt;
                dr[19] = EntryHr;
                dr[20] = ExitDt;
                dr[21] = ExitHr;
                dr[22] = Garage;
                dr[23] = EstCost;
                dt.Rows.Add(dr);
            }
            CA_RequestLineDetailsPs(dtLine);
            //InsertToFiles(dtLine);
            return dt;
        }
        public static DataTable ReadXmlForCA_RequestDetails()
        {
            EventLog eventLog = new EventLog();

            eventLog.Source = "NewSource";
            //eventLog.WriteEntry("in ReadXMLFORCA", EventLogEntryType.Warning, 1001);
            string url = @"https://chaina-motorsltd.dira2.co.il/chainamotors/systems/integrationFile.xml";
            //  string get = p1.getData(m_userforzip, m_passforzip, url);
            Engine e = new Engine();
            //EventManager.WriteEventInfoMessage("try to do login");
            string get = e.getData(m_userforzip, m_passforzip, url);
            //EventManager.WriteEventInfoMessage("finish to do login");

            //EventManager.WriteEventInfoMessage("start to loadxml");
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(get);
            try
            {
                //xmlDoc.Load(@"C:\Users\Doctor\Desktop\noLine.xml");
                //xmlDoc.Load(@"C:\Users\Doctor\Desktop\integrationFile.xml");
                //xmlDoc.Load(@"C:\Users\Doctor\Desktop\2LINES.xml");
                File.AppendAllText(@"C:\carmel\logs\download.txt", "before load Xml" + Environment.NewLine);
                xmlDoc.LoadXml(get);
                File.AppendAllText(@"C:\carmel\logs\download.txt", "after load xml" + Environment.NewLine);
                File.WriteAllText(@"C:\Carmel\linkXml.xml", xmlDoc.InnerXml);

            }
            catch (Exception exs)
            {
                File.AppendAllText(@"C:\carmel\logs\download.txt", "fail load Xml::Date: " + DateTime.Now.ToString() + "; ex:" + exs.Message + Environment.NewLine);
                //EventManager.WriteEventErrorMessage("failed to loadxml", exs);
            }
            //XmlNodeList nodeList = xmlDoc.GetElementsByTagName("item");
            XmlNodeList nodeList = xmlDoc.SelectNodes("//CA_RequestDetail");
            //   string id = "", idezer = "", car_mile_rage = "", failure_request_type = "", failure_request_rsn = "", failure_request_desc = "", description = "", time_car_entered = "";
            string ReqNo = "", RequestStts = "", ApproverName = "", LicNum = "", KM = "", RequestDt = "", RequestTypeCd = "", RequestTypeNm = "", RequestRsnCd = "", RequestRsnNm = "", RequestCd = "", RequestDesc = "", Notes = "", InternalNotes = "", CampaignCd = "", CampaignNm = "", OpenDt = "", StatusFlag = "", FirstRspnsDt = "", EntryDt = "", EntryHr = "", ExitDt = "", ExitHr = "", Garage = "", EstCost = "", cancelReq = "";


            DataTable dt = new DataTable();
            for (int col = 0; col < 25; col++)
                dt.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            //     DataTable dt = ConvertToDataTableML(csv_file_path, 6);
            DataRow dr, drLine;
            //מתבצעת ריצה על סוג בקשה בקריאה
            DataTable dtLine = new DataTable();
            for (int col = 0; col < 11; col++)
                dtLine.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            File.AppendAllText(@"C:\carmel\logs\download.txt", "bEFORE FOREACH XMLNODE ::Date: " + DateTime.Now.ToString() + Environment.NewLine);


            foreach (XmlNode node in nodeList)
            {
                dr = dt.NewRow();
                ReqNo = node.SelectSingleNode("ReqNo").InnerText;

                try
                {
                    File.WriteAllText(@"C:\Carmel\XmlBackUps\" + ReqNo + "-" + DateTime.Now.Date.ToString("dd_MM_yyy") + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".xml", xmlDoc.InnerXml);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(@"C:\carmel\logs\download.txt", "fail save backup Xml" + ex + Environment.NewLine);
                }
                //cancelReq = node.SelectSingleNode("Cancel").InnerText;
                //if (cancelReq.Equals("1"))
                //{
                //    //deleteRequest(ReqNo);
                //    continue;
                //}
                //מתבצעת ריצה על סוג בקשה בקריאה
                // XmlNodeList nodeListline = xmlDoc.GetElementsByTagName("orderline");

                XmlNodeList nodeListline = node.SelectNodes("CA_RequestLineDetails"); //  can also use XPath here
                File.AppendAllText(@"C:\carmel\logs\download.txt", "before nodeline ::Date: " + DateTime.Now.ToString() + Environment.NewLine);
                foreach (XmlNode nodeline in nodeListline)
                {

                    XmlNodeList nodeListline2 = nodeline.SelectNodes("CA_RequestLineDetail");
                    foreach (XmlNode nodeli in nodeListline2)
                    {

                        // string idreq = "", product_id = "", quantity = "", prodname = "", issue_type = "", status = "", item_key = "";
                        string LineId = "", ItemCode = "", Qntty = "", RequestSttsline = "", RejectRsn = "", BillTo = "", InternalNotesline = "", cancelLine = "", statusFlagLine = "", SpclApprvl = "", TimeStamp = "";
                        LineId = nodeli.SelectSingleNode("LineId").InnerText;
                        RequestSttsline = nodeli.SelectSingleNode("RequestStts").InnerText;
                        statusFlagLine = nodeli.SelectSingleNode("StatusFlag").InnerText;
                        //cancelLine = nodeli.SelectSingleNode("Cancel").InnerText;
                        //if (statusFlagLine.Equals("1") && RequestSttsline.Equals("5"))
                        File.AppendAllText(@"C:\carmel\logs\download.txt", "before if for statusflag Xml::Date: " + DateTime.Now.ToString() + Environment.NewLine);
                        if (statusFlagLine.Equals("0") && RequestSttsline.Equals("5"))
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "SEND TO DELETE LINE ::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                            deleteLine(ReqNo, LineId);
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "after deleteLine ::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "RequestSttsline : " + RequestSttsline + Environment.NewLine);
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "statusFlagLine : " + statusFlagLine + Environment.NewLine);
                            continue;
                        }

                        File.AppendAllText(@"C:\carmel\logs\download.txt", "after if status flag::Date: " + DateTime.Now.ToString() + Environment.NewLine);
                        ItemCode = nodeli.SelectSingleNode("ItemCode").InnerText;
                        Qntty = nodeli.SelectSingleNode("Qntty").InnerText;
                        //RequestSttsline = nodeli.SelectSingleNode("RequestStts").InnerText;
                        RejectRsn = nodeli.SelectSingleNode("RejectRsn").InnerText;
                        BillTo = nodeli.SelectSingleNode("BillTo").InnerText;
                        InternalNotesline = nodeli.SelectSingleNode("InternalNotes").InnerText;
                        SpclApprvl = nodeli.SelectSingleNode("SpclApprvl").InnerText;
                        TimeStamp = nodeli.SelectSingleNode("TimeStamp").InnerText;


                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "before insert to drLine ::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "RequestSttsline : " + RequestSttsline + Environment.NewLine);
                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "statusFlagLine : " + statusFlagLine + Environment.NewLine);
                        drLine = dtLine.NewRow();
                        //הכנסת נתונים לטבלה
                        drLine[0] = ReqNo;
                        drLine[1] = LineId;
                        drLine[2] = ItemCode;
                        drLine[3] = Qntty;
                        drLine[4] = RequestSttsline;
                        drLine[5] = RejectRsn;
                        drLine[6] = BillTo;
                        drLine[7] = InternalNotesline;
                        drLine[8] = statusFlagLine;
                        drLine[9] = SpclApprvl;
                        drLine[10] = TimeStamp;
                        dtLine.Rows.Add(drLine);
                    }
                }
                RequestStts = node.SelectSingleNode("RequestStts").InnerText;
                ApproverName = node.SelectSingleNode("ApproverName").InnerText;
                LicNum = node.SelectSingleNode("LicNum").InnerText;
                KM = node.SelectSingleNode("KM").InnerText;
                RequestDt = node.SelectSingleNode("RequestDt").InnerText;
                RequestTypeCd = node.SelectSingleNode("RequestTypeCd").InnerText;
                RequestTypeNm = node.SelectSingleNode("RequestTypeNm").InnerText;
                RequestRsnCd = node.SelectSingleNode("RequestRsnCd").InnerText;
                RequestRsnNm = node.SelectSingleNode("RequestRsnNm").InnerText;
                RequestCd = node.SelectSingleNode("RequestCd").InnerText;
                RequestDesc = node.SelectSingleNode("RequestDesc").InnerText;
                Notes = node.SelectSingleNode("Notes").InnerText;
                InternalNotes = node.SelectSingleNode("InternalNotes").InnerText;
                CampaignCd = node.SelectSingleNode("CampaignCd").InnerText;
                CampaignNm = node.SelectSingleNode("CampaignNm").InnerText;
                OpenDt = node.SelectSingleNode("OpenDt").InnerText;
                StatusFlag = node.SelectSingleNode("StatusFlag").InnerText;
                FirstRspnsDt = node.SelectSingleNode("FirstRspnsDt").InnerText;
                EntryDt = node.SelectSingleNode("EntryDt").InnerText;
                EntryHr = node.SelectSingleNode("EntryHr").InnerText;
                ExitDt = node.SelectSingleNode("ExitDt").InnerText;
                ExitHr = node.SelectSingleNode("ExitHr").InnerText;
                Garage = node.SelectSingleNode("Garage").InnerText;
                EstCost = node.SelectSingleNode("EstCost").InnerText;
                dr[0] = ReqNo;
                dr[1] = RequestStts;
                dr[2] = ApproverName;
                dr[3] = LicNum;
                dr[4] = KM;
                dr[5] = RequestDt;
                dr[6] = RequestTypeCd;
                dr[7] = RequestTypeNm;
                dr[8] = RequestRsnCd;
                dr[9] = RequestRsnNm;
                dr[10] = RequestCd;
                dr[11] = RequestDesc;
                dr[12] = Notes;
                dr[13] = InternalNotes;
                dr[14] = CampaignCd;
                dr[15] = CampaignNm;
                dr[16] = OpenDt;
                dr[17] = FirstRspnsDt;
                dr[18] = EntryDt;
                dr[19] = EntryHr;
                dr[20] = ExitDt;
                dr[21] = ExitHr;
                dr[22] = Garage;
                dr[23] = EstCost;
                dr[24] = StatusFlag;
                dt.Rows.Add(dr);
            }
            //EventLog eventLog = new EventLog();

            //eventLog.Source = "NewSource";
            // eventLog.WriteEntry("beforeRequestLineDetails", EventLogEntryType.Warning, 1001);
            File.AppendAllText(@"C:\Carmel\logs\download.txt", "before CA_RequestLineDetailsPs" + Environment.NewLine);
            CA_RequestLineDetailsPs(dtLine);
            File.AppendAllText(@"C:\Carmel\logs\download.txt", "after CA_RequestLineDetailsPs" + Environment.NewLine);
            //InsertToFiles(dtLine);
            return dt;
        }
        public static DataTable ReadXmlForCA_RequestDetails1()
        {
            // string url = @"https://chaina-motorsltd.dira2.co.il/chainamotors/systems/integrationFile.xml";
            //  string get = p1.getData(m_userforzip, m_passforzip, url);
            Engine e = new Engine();
            //EventManager.WriteEventInfoMessage("try to do login");
            string get = e.getData(m_userforzip, m_passforzip, m_urlxml);
            //EventManager.WriteEventInfoMessage("finish to do login");

            //EventManager.WriteEventInfoMessage("start to loadxml");
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(get);
            string path = @"https://chaina-motorsltd.dira2.co.il/chainamotors/systems/integrationFile.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            //EventManager.WriteEventInfoMessage("finish to loadxml");
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("item");
            //   string id = "", idezer = "", car_mile_rage = "", failure_request_type = "", failure_request_rsn = "", failure_request_desc = "", description = "", time_car_entered = "";
            string docNum = "", docEntry = "", u_licnum = "", date = "", statusCode = "", notes = "", u_km = "";


            DataTable dt = new DataTable();
            for (int col = 0; col < 7; col++)
                dt.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            //     DataTable dt = ConvertToDataTableML(csv_file_path, 6);
            DataRow dr, drLine;
            //מתבצעת ריצה על סוג בקשה בקריאה
            DataTable dtLine = new DataTable();
            for (int col = 0; col < 3; col++)
                dtLine.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            foreach (XmlNode node in nodeList)
            {
                dr = dt.NewRow();
                docNum = node.SelectSingleNode("docnum").InnerText;
                //מתבצעת ריצה על סוג בקשה בקריאה
                // XmlNodeList nodeListline = xmlDoc.GetElementsByTagName("orderline");

                XmlNodeList nodeListline = node.SelectNodes("orderlines"); //  can also use XPath here

                //foreach (XmlNode nodeline in nodeListline)
                //{
                //    XmlNodeList nodeListline2 = nodeline.SelectNodes("orderline");
                //    foreach (XmlNode nodeli in nodeListline2)
                //    {

                //       // string idreq = "", product_id = "", quantity = "", prodname = "", issue_type = "", status = "", item_key = "";
                //        string id = "", U_PartCode = "", U_Qntty = "";
                //        id = nodeli.SelectSingleNode("id").InnerText;
                //        U_PartCode = nodeli.SelectSingleNode("U_PartCode").InnerText;
                //        U_Qntty = nodeli.SelectSingleNode("U_Qntty").InnerText;

                //        drLine = dtLine.NewRow();
                //        //הכנסת נתונים לטבלה
                //        drLine[0] = id;
                //        drLine[1] = U_PartCode;
                //        drLine[2] = U_Qntty;
                //        dtLine.Rows.Add(drLine);
                //    }
                //}
                docEntry = node.SelectSingleNode("DocEntry").InnerText;
                u_licnum = node.SelectSingleNode("U_licnum").InnerText;
                date = node.SelectSingleNode("Date").InnerText;
                notes = node.SelectSingleNode("Notes").InnerText;
                u_km = node.SelectSingleNode("U_Km").InnerText;
                statusCode = node.SelectSingleNode("StatusCode").InnerText;

                dr[0] = docNum;
                dr[1] = docEntry;
                dr[2] = u_licnum;
                dr[3] = date;
                dr[4] = notes;
                dr[5] = u_km;
                dr[6] = statusCode;
                dt.Rows.Add(dr);
            }
            //CA_RequestLineDetailsPs(dtLine);
            //InsertToFiles(dtLine);
            return dt;
        }
        public static DataTable InsertToFiles(DataTable tblCL)
        {
            DataTable tblFiles = new DataTable();
            for (int col = 0; col < 11; col++)
                tblFiles.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));
            int ReqNo, LineId;
            string Descrptn = "", FileLink, FileName = "", AddTime = "";
            DateTime AddDate = new DateTime();
            degel = true;
            foreach (DataRow dr in tblCL.Rows)
            {

                ReqNo = int.Parse(dr[0].ToString());
                LineId = int.Parse(dr[1].ToString());
                //  Descrptn = (dr[10].ToString().Substring(dr[12].ToString().IndexOf(".")));
                FileLink = "";
                if (dr[9].ToString() != "" && dr[10].ToString() != "")
                {
                    degel = true;
                    Descrptn = (dr[9].ToString().Substring(dr[9].ToString().IndexOf(".") + 1));
                    FileName = (dr[9].ToString().Substring(0, dr[9].ToString().IndexOf(".")));
                    AddDate = DateTime.Parse(dr[10].ToString().Substring(0, 11));
                    AddTime = dr[10].ToString().Substring(11);
                }
                else
                    degel = false;
                DataRow workRow = tblFiles.NewRow();
                workRow[0] = ReqNo;
                workRow[1] = LineId;
                workRow[2] = Descrptn;
                workRow[3] = FileLink;
                if (dr[9].ToString() != "" && dr[10].ToString() != "")
                {
                    degel = true;
                    workRow[4] = FileName;
                    workRow[5] = AddDate;
                    workRow[6] = AddTime;
                }
                else
                    degel = false;

                tblFiles.Rows.Add(workRow);

                if (degel)
                    AddIntoPsFiles(tblFiles);
                else
                    AddIntoPsFiles2(tblFiles);
            }
            return tblFiles;
        }

        //  https://stackoverflow.com/questions/20860101/how-to-read-text-file-to-datatable


        //  https://social.msdn.microsoft.com/Forums/vstudio/en-US/f14523f8-3a40-451b-983b-ae4f5fd12697/how-to-load-data-from-csv-file-in-temp-table-in-c?forum=csharpgeneral
        public void AddIntoPsRequestDetailsBack(DataTable dt)
        {
            // https://stackoverflow.com/questions/29046715/extract-values-from-datatable-with-single-row

            int i = 0, count = dt.Rows.Count;
            //פרמטרים של טבלת CA_RequestDetails
            string CampaignCd, EstCost;
            int ReqNo, KM, RequestTypeCd, JCNum, RequestChangeFlag, RequestRsnCd, RequestCd, StatusFlag;//EstCost
            string ApproverName, LicNum, RequestTypeNm, RequestDesc, CampaignNm, EntryHr, ExitHr, Garage, RequestStts, RequestRsnNm, Notes;
            DateTime RequestDt, EntryDt, ExitDt, TimeStamp, OpenDt, FirstRspnsDt;
            while (dt.Rows.Count > 0 && count > 0)
            {
                count--;
                DataRow row = dt.Rows[i++];
                //זימון פונקציה שתבדוק עבור סוג בקשה אם קיים אם כן תעדכן תאור אחרת תוסיף לטבלה
                ReqNo = int.Parse(row[0].ToString());
                RequestStts = row[1].ToString();
                ApproverName = row[2].ToString();
                LicNum = row[3].ToString();
                KM = int.Parse(row[4].ToString());
                RequestDt = DateTime.Parse(row[5].ToString());
                RequestTypeCd = int.Parse(row[6].ToString());
                RequestTypeNm = row[7].ToString();
                RequestRsnCd = int.Parse(row[8].ToString());
                RequestRsnNm = row[9].ToString();
                RequestCd = int.Parse(row[10].ToString());
                RequestDesc = row[11].ToString();
                Notes = row[12].ToString();
                CampaignCd = row[13].ToString();
                CampaignNm = row[14].ToString();
                OpenDt = DateTime.Parse(row[15].ToString());
                FirstRspnsDt = DateTime.Parse(row[16].ToString());
                EntryDt = DateTime.Parse(row[17].ToString());
                EntryHr = row[18].ToString();
                ExitDt = DateTime.Parse(row[19].ToString());
                ExitHr = row[20].ToString();
                Garage = row[21].ToString();
                EstCost = row[22].ToString();
                // JCNum = int.Parse(row[16].ToString());
                // RequestChangeFlag = int.Parse(row[17].ToString()); 
                // TimeStamp = DateTime.Parse(row[18].ToString());
                StatusFlag = int.Parse(row[23].ToString());
                TimeStamp = DateTime.Parse(row[24].ToString());
                //EventManager.WriteEventInfoMessage("start ConnectionString to db for CA_RequestDetails table");
                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("RequestDetails_temp", con))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                            cmd.Parameters.Add("@RequestStts", SqlDbType.NVarChar).Value = RequestStts;
                            cmd.Parameters.Add("@ApproverName", SqlDbType.NVarChar).Value = ApproverName;
                            cmd.Parameters.Add("@LicNum", SqlDbType.NVarChar).Value = LicNum;
                            cmd.Parameters.Add("@KM", SqlDbType.Int).Value = KM;
                            cmd.Parameters.Add("@RequestDt", SqlDbType.DateTime).Value = RequestDt;
                            cmd.Parameters.Add("@RequestTypeCd", SqlDbType.Int).Value = RequestTypeCd;
                            cmd.Parameters.Add("@RequestTypeNm", SqlDbType.NVarChar).Value = RequestTypeNm;
                            cmd.Parameters.Add("@RequestRsnCd", SqlDbType.Int).Value = RequestRsnCd;
                            cmd.Parameters.Add("@RequestRsnNm", SqlDbType.NVarChar).Value = RequestRsnNm;
                            cmd.Parameters.Add("@RequestCd", SqlDbType.Int).Value = RequestCd;
                            cmd.Parameters.Add("@RequestDesc", SqlDbType.NVarChar).Value = RequestDesc;
                            cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                            cmd.Parameters.Add("@CampaignCd", SqlDbType.NVarChar).Value = CampaignCd;
                            cmd.Parameters.Add("@CampaignNm", SqlDbType.NVarChar).Value = CampaignNm;
                            cmd.Parameters.Add("@OpenDt", SqlDbType.DateTime).Value = OpenDt;
                            cmd.Parameters.Add("@FirstRspnsDt", SqlDbType.DateTime).Value = FirstRspnsDt;
                            cmd.Parameters.Add("@EntryDt", SqlDbType.DateTime).Value = EntryDt;
                            cmd.Parameters.Add("@EntryHr", SqlDbType.NVarChar).Value = EntryHr;
                            cmd.Parameters.Add("@ExitDt", SqlDbType.DateTime).Value = ExitDt;
                            cmd.Parameters.Add("@ExitHr", SqlDbType.NVarChar).Value = ExitHr;
                            cmd.Parameters.Add("@Garage", SqlDbType.NVarChar).Value = Garage;
                            cmd.Parameters.Add("@EstCost", SqlDbType.NVarChar).Value = EstCost;
                            //cmd.Parameters.Add("@JCNum", SqlDbType.Int).Value = JCNum;
                            //cmd.Parameters.Add("@RequestChangeFlag", SqlDbType.Int).Value = RequestChangeFlag;
                            //    cmd.Parameters.Add("@StatusFlag", SqlDbType.Int).Value = RequestChangeFlag;
                            // '@StatusFlag'
                            //  cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = TimeStamp;
                            cmd.Parameters.Add("@StatusFlag", SqlDbType.Int).Value = StatusFlag;
                            cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = TimeStamp;
                        }
                        catch (Exception e)
                        {
                            //EventManager.WriteEventErrorMessage("failed to put values for CA_RequestDetails table", e);
                        }
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            //EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestDetails table", e);
                        }
                        try
                        {
                            con.Close();
                        }
                        catch (Exception e)
                        {
                            //EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestDetails table", e);
                        }
                    }
                }


            }
        }
        public void AddIntoPsRequestDetails(DataTable dt)
        {
            // https://stackoverflow.com/questions/29046715/extract-values-from-datatable-with-single-row

            int i = 0, count = dt.Rows.Count;
            //פרמטרים של טבלת CA_RequestDetails
            string EstCost;
            DateTime d = new DateTime(1753, 10, 10); //deafult
            int ReqNo, KM, RequestTypeCd, JCNum, RequestChangeFlag, RequestRsnCd, RequestCd, StatusFlag, CampaignCd;//EstCost
            string ApproverName, LicNum, RequestTypeNm, RequestDesc, CampaignNm, EntryHr, ExitHr, Garage, RequestStts, RequestRsnNm, Notes, InternalNotes;
            DateTime RequestDt, EntryDt, ExitDt, TimeStamp, OpenDt, FirstRspnsDt;
            while (dt.Rows.Count > 0 && count > 0)
            {
                count--;
                DataRow row = dt.Rows[i++];
                //זימון פונקציה שתבדוק עבור סוג בקשה אם קיים אם כן תעדכן תאור אחרת תוסיף לטבלה
                try //if there is no ReqNo do not insert the request
                {
                    ReqNo = int.Parse(row[0].ToString());
                }
                catch
                {
                    continue;
                }
                //ReqNo = int.Parse(row[0].ToString());
                RequestStts = row[1].ToString();
                ApproverName = row[2].ToString();
                LicNum = row[3].ToString();
                KM = int.Parse(row[4].ToString());
                RequestDt = DateTime.Parse(row[5].ToString());
                try
                {
                    RequestTypeCd = int.Parse(row[6].ToString());
                }
                catch //if empty field
                {
                    RequestTypeCd = 0;
                }
                //RequestTypeCd = int.Parse(row[6].ToString());
                RequestTypeNm = row[7].ToString();
                try
                {
                    RequestRsnCd = int.Parse(row[8].ToString());
                }
                catch //if empty field
                {
                    RequestRsnCd = 0;
                }
                //RequestRsnCd = int.Parse(row[8].ToString());
                RequestRsnNm = row[9].ToString();
                try
                {
                    RequestCd = int.Parse(row[10].ToString());
                }
                catch //if empty field
                {
                    RequestCd = 0;
                }
                //RequestCd = int.Parse(row[10].ToString());
                RequestDesc = row[11].ToString();
                Notes = row[12].ToString();
                InternalNotes = row[13].ToString();
                try
                {
                    CampaignCd = int.Parse(row[14].ToString());
                }
                catch //if empty field
                {
                    CampaignCd = 0;
                }
                CampaignNm = row[15].ToString();
                OpenDt = DateTime.Parse(row[16].ToString());
                try
                {
                    FirstRspnsDt = DateTime.Parse(row[17].ToString());
                }
                catch
                {
                    FirstRspnsDt = d;
                }
                EntryDt = DateTime.Parse(row[18].ToString());
                EntryHr = row[19].ToString();
                try
                {
                    ExitDt = DateTime.Parse(row[20].ToString());
                }
                catch
                {
                    ExitDt = d;
                }
                ExitHr = row[21].ToString();
                Garage = row[22].ToString();
                EstCost = row[23].ToString();
                // JCNum = int.Parse(row[16].ToString());
                // RequestChangeFlag = int.Parse(row[17].ToString()); 
                // TimeStamp = DateTime.Parse(row[18].ToString());
                StatusFlag = int.Parse(row[24].ToString());
                //TimeStamp = DateTime.Parse(row[24].ToString());
                //EventManager.WriteEventInfoMessage("start ConnectionString to db for CA_RequestDetails table");
                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("RequestDetails", con))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                            cmd.Parameters.Add("@RequestStts", SqlDbType.NVarChar).Value = RequestStts;
                            cmd.Parameters.Add("@ApproverName", SqlDbType.NVarChar).Value = ApproverName;
                            cmd.Parameters.Add("@LicNum", SqlDbType.NVarChar).Value = LicNum;
                            cmd.Parameters.Add("@KM", SqlDbType.Int).Value = KM;
                            cmd.Parameters.Add("@RequestDt", SqlDbType.DateTime).Value = RequestDt;
                            cmd.Parameters.Add("@RequestTypeCd", SqlDbType.Int).Value = RequestTypeCd;
                            cmd.Parameters.Add("@RequestTypeNm", SqlDbType.NVarChar).Value = RequestTypeNm;
                            cmd.Parameters.Add("@RequestRsnCd", SqlDbType.Int).Value = RequestRsnCd;
                            cmd.Parameters.Add("@RequestRsnNm", SqlDbType.NVarChar).Value = RequestRsnNm;
                            cmd.Parameters.Add("@RequestCd", SqlDbType.Int).Value = RequestCd;
                            cmd.Parameters.Add("@RequestDesc", SqlDbType.NVarChar).Value = RequestDesc;
                            cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                            cmd.Parameters.Add("@InternalNotes", SqlDbType.NVarChar).Value = InternalNotes;
                            cmd.Parameters.Add("@CampaignCd", SqlDbType.NVarChar).Value = CampaignCd;
                            cmd.Parameters.Add("@CampaignNm", SqlDbType.NVarChar).Value = CampaignNm;
                            cmd.Parameters.Add("@OpenDt", SqlDbType.DateTime).Value = OpenDt;
                            cmd.Parameters.Add("@FirstRspnsDt", SqlDbType.DateTime).Value = FirstRspnsDt;
                            cmd.Parameters.Add("@EntryDt", SqlDbType.DateTime).Value = EntryDt;
                            cmd.Parameters.Add("@EntryHr", SqlDbType.NVarChar).Value = EntryHr;
                            cmd.Parameters.Add("@ExitDt", SqlDbType.DateTime).Value = ExitDt;
                            cmd.Parameters.Add("@ExitHr", SqlDbType.NVarChar).Value = ExitHr;
                            cmd.Parameters.Add("@Garage", SqlDbType.NVarChar).Value = Garage;
                            cmd.Parameters.Add("@EstCost", SqlDbType.NVarChar).Value = EstCost;
                            //cmd.Parameters.Add("@JCNum", SqlDbType.Int).Value = JCNum;
                            //cmd.Parameters.Add("@RequestChangeFlag", SqlDbType.Int).Value = RequestChangeFlag;
                            //cmd.Parameters.Add("@StatusFlag", SqlDbType.Int).Value = RequestChangeFlag;
                            // '@StatusFlag'
                            //cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = TimeStamp;
                            cmd.Parameters.Add("@StatusFlag", SqlDbType.Int).Value = StatusFlag;
                            //cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = TimeStamp;
                        }
                        catch (Exception e)
                        {
                            EventManager.WriteEventErrorMessage("failed to put values for CA_RequestDetails table", e);
                        }
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestDetails table", e);
                        }
                        try
                        {

                            con.Close();
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "done AddIntoPsRequestDetails" + Environment.NewLine);
                        }
                        catch (Exception e)
                        {
                            EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestDetails table", e);
                        }
                    }
                }


            }
        }
        //public void AddIntoPsRequestDetails1(DataTable dt)
        //{
        //    // https://stackoverflow.com/questions/29046715/extract-values-from-datatable-with-single-row

        //    int i = 0, count = dt.Rows.Count;
        //    //פרמטרים של טבלת CA_RequestDetails
        //    //string CampaignCd, EstCost;
        //    //int ReqNo, KM, RequestTypeCd, JCNum, RequestChangeFlag, RequestRsnCd, RequestCd, StatusFlag;//EstCost
        //    //string ApproverName, LicNum, RequestTypeNm, RequestDesc, CampaignNm, EntryHr, ExitHr, Garage, RequestStts, RequestRsnNm, Notes;
        //    DateTime date;
        //    int docNum, U_Km, DocEntry, StatusCode;
        //    string U_licnum, notes;
        //    while (dt.Rows.Count > 0 && count > 0)
        //    {
        //        count--;
        //        DataRow row = dt.Rows[i++];
        //        //זימון פונקציה שתבדוק עבור סוג בקשה אם קיים אם כן תעדכן תאור אחרת תוסיף לטבלה
        //        docNum = int.Parse(row[0].ToString());
        //        U_Km = int.Parse(row[5].ToString());
        //        DocEntry = int.Parse(row[1].ToString());
        //        StatusCode = int.Parse(row[6].ToString());
        //        U_licnum = row[2].ToString();
        //        notes = row[4].ToString();
        //        date = DateTime.Parse(row[3].ToString());

        //        EventManager.WriteEventInfoMessage("start ConnectionString to db for CA_RequestDetails table");
        //        using (SqlConnection con = new SqlConnection(m_ConnectionString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("RequestDetails_test", con))
        //            {
        //                try
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = docNum;
        //                    //cmd.Parameters.Add("@RequestStts", SqlDbType.NVarChar).Value = RequestStts;
        //                    //cmd.Parameters.Add("@ApproverName", SqlDbType.NVarChar).Value = ApproverName;
        //                    cmd.Parameters.Add("@LicNum", SqlDbType.NVarChar).Value = U_licnum;
        //                    cmd.Parameters.Add("@KM", SqlDbType.Int).Value = U_Km;
        //                    cmd.Parameters.Add("@RequestDt", SqlDbType.DateTime).Value = date;
        //                    cmd.Parameters.Add("@RequestTypeCd", SqlDbType.Int).Value = DocEntry;
        //                    //cmd.Parameters.Add("@RequestTypeNm", SqlDbType.NVarChar).Value = RequestTypeNm;
        //                    //cmd.Parameters.Add("@RequestRsnCd", SqlDbType.Int).Value = RequestRsnCd;
        //                    //cmd.Parameters.Add("@RequestRsnNm", SqlDbType.NVarChar).Value = RequestRsnNm;
        //                    cmd.Parameters.Add("@RequestCd", SqlDbType.Int).Value = StatusCode;
        //                    //cmd.Parameters.Add("@RequestDesc", SqlDbType.NVarChar).Value = RequestDesc;
        //                    cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = notes;
        //                }
        //                catch (Exception e)
        //                {
        //                    EventManager.WriteEventErrorMessage("failed to put values for CA_RequestDetails table", e);
        //                }
        //                try
        //                {
        //                    con.Open();
        //                    cmd.ExecuteNonQuery();
        //                }
        //                catch (Exception e)
        //                {
        //                    EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestDetails table", e);
        //                }
        //                try
        //                {
        //                    con.Close();
        //                }
        //                catch (Exception e)
        //                {
        //                    EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestDetails table", e);
        //                }
        //            }
        //        }


        //    }
        //}
        public static void AddIntoPsRequestLineDetails(DataTable dt, int NumOfRow)
        {
            try
            {

                EventLog eventLog = new EventLog();

                eventLog.Source = "NewSource";
                //eventLog.WriteEntry("in RequestLineDetails", EventLogEntryType.Warning, 1001);
                int i, count = dt.Rows.Count;
                //פרמטרים של טבלת CA_RequestLineDetails
                float Qntty;
                int ReqNo, LineId, BillTo, JCNum, StatusFlag, Docentry, SBOLineId;
                string ItemCode, RequestStts, RejectRsn, RjctMsg, FileName, InternalNotes, SpclApprvl;
                DateTime Timestamp, AddTime;
                i = count - NumOfRow;
                count = NumOfRow;
                File.AppendAllText(@"C:\carmel\logs\download.txt", "AddIntoPsRequestLineDetails: " + DateTime.Now.ToString() + Environment.NewLine);
                while (dt.Rows.Count > 0 && count > 0)
                {
                    count--;
                    DataRow row = dt.Rows[i++];
                    ReqNo = int.Parse(row[0].ToString());
                    LineId = int.Parse(row[1].ToString());
                    ItemCode = row[2].ToString();
                    Qntty = float.Parse(row[3].ToString());
                    RequestStts = row[4].ToString();
                    RejectRsn = row[5].ToString();
                    eventLog.Source = "NewSource";
                    //eventLog.WriteEntry("before billto", EventLogEntryType.Warning, 1001);
                    try
                    {
                        BillTo = int.Parse(row[6].ToString());
                    }
                    catch
                    {
                        BillTo = 0;
                    }
                    //BillTo = int.Parse(row[6].ToString());
                    InternalNotes = row[7].ToString();
                    eventLog.Source = "NewSource";
                    //eventLog.WriteEntry("afterbillto", EventLogEntryType.Warning, 1001);
                    /*  JCNum = int.Parse(row[7].ToString());
                            RjctMsg = row[8].ToString();*/
                    StatusFlag = int.Parse(row[8].ToString());
                    SpclApprvl = row[9].ToString();
                    Timestamp = DateTime.Parse(row[10].ToString());
                    //   Docentry=int.Parse(row[7].ToString());
                    //SBOLineId=int.Parse(row[8].ToString());
                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "Inside AddIntoPsRequestLineDetails ::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "RequestStts : " + RequestStts + Environment.NewLine);
                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "StatusFlag : " + StatusFlag + Environment.NewLine);
                    using (SqlConnection con = new SqlConnection(m_ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("RequestLineDetails", con))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                                cmd.Parameters.Add("@LineId", SqlDbType.Int).Value = LineId;
                                cmd.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = ItemCode;
                                cmd.Parameters.Add("@Qntty", SqlDbType.Float).Value = Qntty;
                                cmd.Parameters.Add("@RequestStts", SqlDbType.NVarChar).Value = RequestStts;
                                cmd.Parameters.Add("@RejectRsn", SqlDbType.NVarChar).Value = RejectRsn;
                                cmd.Parameters.Add("@BillTo", SqlDbType.Int).Value = BillTo;
                                cmd.Parameters.Add("@InternalNotes", SqlDbType.NVarChar).Value = InternalNotes;
                                /* cmd.Parameters.Add("@JCNum", SqlDbType.Int).Value = JCNum;
                                   cmd.Parameters.Add("@RjctMsg", SqlDbType.NVarChar).Value = RjctMsg;*/
                                cmd.Parameters.Add("@StatusFlag", SqlDbType.Int).Value = StatusFlag;
                                cmd.Parameters.Add("@SpclApprvl", SqlDbType.NVarChar).Value = SpclApprvl;
                                cmd.Parameters.Add("@Timestamp", SqlDbType.DateTime).Value = Timestamp;
                                //cmd.Parameters.Add("@Docentry", SqlDbType.Int).Value = Docentry;
                                //cmd.Parameters.Add("@SBOLineId", SqlDbType.Int).Value = SBOLineId;
                            }
                            catch (Exception e)
                            {
                                File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL put values CA_RequestLineDetails table::Date : " + DateTime.Now.ToString() + ";;" + e + Environment.NewLine);
                                //EventManager.WriteEventErrorMessage("failed to put values for CA_RequestLineDetails table", e);

                            }
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL execute CA_RequestLineDetails table::Date : " + DateTime.Now.ToString() + ";;" + e + Environment.NewLine);
                                //EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestLineDetails table", e);
                                //File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL execute CA_RequestLineDetails table" + e + Environment.NewLine);
                            }
                            try
                            {
                                con.Close();
                                File.AppendAllText(@"C:\Carmel\logs\download.txt", "done CA_RequestLineDetails table::Date : " + DateTime.Now.ToString() + Environment.NewLine);

                            }
                            catch (Exception e)
                            {
                                File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL close CA_RequestLineDetails table::Date : " + DateTime.Now.ToString() + ";;" + e + Environment.NewLine);
                                //EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestLineDetails table", e);
                                //File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL close CA_RequestLineDetails table" + e + Environment.NewLine);
                            }
                        }
                    }


                }
            }
            catch (Exception e)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL AddIntoPsRequestLineDetails table::Date : " + DateTime.Now.ToString() + ";;" + e + Environment.NewLine);
                //EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestLineDetails table", e);
                //File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL close CA_RequestLineDetails table" + e + Environment.NewLine);
            }
        }
        public static void CA_RequestLineDetailsPs(DataTable dt)
        {
            try
            {
                File.AppendAllText(@"C:\carmel\logs\download.txt", "Inside CA_RequestLineDetailsPs: " + DateTime.Now.ToString() + Environment.NewLine);
                int count = dt.Rows.Count, mone = 0;
                string connectionString = m_ConnectionString;
                // https://stackoverflow.com/questions/9648934/insert-into-if-not-exists-sql-server
                /* string insertQuery = " IF NOT EXISTS (select * from CA_RequestLineDetails_temp where ReqNo = @ReqNo) \n\r" +
                    " BEGIN \n\r" +
                    "     INSERT into CA_RequestLineDetails_temp(ReqNo,LineId,ItemCode,Qntty,RequestStts,RejectRsn,BillTo) VALUES(@ReqNo,@LineId,@ItemCode,@Qntty,@RequestStts,@RejectRsn,@BillTo) \n\r" +
                    " END \n\r " +
                    " ELSE SELECT 0";*/
                string insertQuery = " IF EXISTS (select * from CA_RequestLineDetails where (ReqNo = @ReqNo and LineId = @LineId)) \n\r" +
                  " BEGIN \n\r" +
                  "     update CA_RequestLineDetails set ItemCode = @ItemCode, Qntty = @Qntty, BillTo =@BillTo  ,RequestStts =@RequestStts ,RejectRsn = @RejectRsn,StatusFlag = @StatusFlag ,InternalNotes= @InternalNotes, SpclApprvl= @SpclApprvl, TimeStamp= @TimeStamp where (ReqNo = @ReqNo and LineId = @LineId); \n\r" +
                  " END \n\r " +
                  " ELSE" +
                  " BEGIN \n\r" +
                  "     INSERT into CA_RequestLineDetails(ReqNo,LineId,ItemCode,Qntty,RequestStts, RejectRsn,BillTo, StatusFlag, InternalNotes, SpclApprvl, TimeStamp) VALUES(@ReqNo,@LineId,@ItemCode,@Qntty,@RequestStts, @RejectRsn,@BillTo, @StatusFlag, @InternalNotes, @SpclApprvl, @TimeStamp) \n\r" +
                  " END  \n\r";
                //"IF EXISTS (select * from CA_Log where (ReqNo = @ReqNo) and (LineId = @LineId))) \n\r"+
                //"BEGIN \n\r"+
                //"     update CA_Log set ItemCode = @ItemCode, Qntty = @Qntty, BillTo =@BillTo  ,RequestStts =@RequestStts ,RejectRsn = @RejectRsn,StatusFlag = @StatusFlag ,InternalNotes= @InternalNotes, EventKind='Update' where (ReqNo = @ReqNo and LineId = @LineId); \n\r" + 
                //"END \n \r"+
                //"ELSE \n\r" +
                //"BEGIN \n\r"+
                //"     INSERT INTO CA_Log(ReqNo,LineId,ItemCode,Qntty,RequestStts, RejectRsn,BillTo, StatusFlag, InternalNotes, EventKind) VALUES(@ReqNo,@LineId,@ItemCode,@Qntty,@RequestStts, @RejectRsn,@BillTo, @StatusFlag, @InternalNotes, 'Add') \n\r" +
                //"END";
                using (SqlConnection connection = new SqlConnection(connectionString))
                    while (count > 0 && count > 3)
                    {
                        DataRow row = dt.Rows[mone];
                        try
                        {
                            CA_LogPs(dt, mone);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL CA_LogPs  ::Date: " + DateTime.Now.ToString() + "; ex: " + ex + Environment.NewLine);
                        }

                        count = count - 3;
                        File.AppendAllText(@"C:\carmel\logs\download.txt", "Inside CA_RequestLineDetailsPs BEFORE USING SQL: " + DateTime.Now.ToString() + Environment.NewLine);
                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            // define your parameters ONCE outside the loop, and use EXPLICIT typing
                            command.Parameters.Add("@ReqNo", SqlDbType.Int);
                            command.Parameters.Add("@LineId", SqlDbType.Int);
                            command.Parameters.Add("@ItemCode", SqlDbType.NVarChar);
                            command.Parameters.Add("@Qntty", SqlDbType.Float);
                            command.Parameters.Add("@RequestStts", SqlDbType.NVarChar);
                            command.Parameters.Add("@RejectRsn", SqlDbType.NVarChar);
                            command.Parameters.Add("@BillTo", SqlDbType.Int);
                            command.Parameters.Add("@InternalNotes", SqlDbType.NVarChar);
                            /* command.Parameters.Add("@JCNum", SqlDbType.Int);
                             command.Parameters.Add("@RjctMsg", SqlDbType.NVarChar);*/
                            command.Parameters.Add("@StatusFlag", SqlDbType.Int);
                            command.Parameters.Add("@SpclApprvl", SqlDbType.NVarChar);
                            command.Parameters.Add("@Timestamp", SqlDbType.DateTime);
                            // command.Parameters.Add("@Docentry", SqlDbType.Int);
                            //command.Parameters.Add("@SBOLineId", SqlDbType.Int);
                            connection.Open();
                            //     for (int t = 0; t < row.ItemArray.Count()-2; t++)

                            for (int t = mone; t < (mone + 3); t++)
                            {
                                //  SET the values
                                try
                                {
                                    command.Parameters["@ReqNo"].Value = int.Parse((dt.Rows[t][0]).ToString());
                                    command.Parameters["@LineId"].Value = int.Parse((dt.Rows[t][1]).ToString());
                                }
                                catch //if ReqNo or LineId are empty do not insert the line
                                {
                                    continue;
                                }
                                command.Parameters["@ItemCode"].Value = (dt.Rows[t][2]).ToString();
                                //command.Parameters["@Qntty"].Value = int.Parse((dt.Rows[t][3]).ToString());
                                command.Parameters["@Qntty"].Value = float.Parse((dt.Rows[t][3]).ToString());
                                command.Parameters["@RequestStts"].Value = (dt.Rows[t][4]).ToString();
                                command.Parameters["@RejectRsn"].Value = (dt.Rows[t][5]).ToString();
                                try
                                {
                                    command.Parameters["@BillTo"].Value = int.Parse((dt.Rows[t][6]).ToString());
                                }
                                catch
                                {
                                    command.Parameters["@BillTo"].Value = 0;
                                }
                                //command.Parameters["@BillTo"].Value = int.Parse((dt.Rows[t][6]).ToString());
                                command.Parameters["@InternalNotes"].Value = (dt.Rows[t][7]).ToString();
                                /* command.Parameters["@JCNum"].Value = int.Parse((dt.Rows[t][7]).ToString());
                                 command.Parameters["@RjctMsg"].Value = (dt.Rows[t][8]).ToString();*/
                                command.Parameters["@StatusFlag"].Value = int.Parse((dt.Rows[t][8]).ToString());
                                command.Parameters["@SpclApprvl"].Value = (dt.Rows[t][9]).ToString();
                                command.Parameters["@Timestamp"].Value = DateTime.Parse(dt.Rows[t][10].ToString());
                                //command.Parameters["@Docentry"].Value = int.Parse((dt.Rows[t][7]).ToString());
                                //command.Parameters["@SBOLineId"].Value = int.Parse((dt.Rows[t][8]).ToString());
                                try
                                {
                                    File.AppendAllText(@"C:\Carmel\logs\download.txt", Environment.NewLine + "::Date : " + DateTime.Now.ToString() + "query params: ReqNo = " + (dt.Rows[t][0]).ToString() + Environment.NewLine +
                                                                                                 "LineId = " + (dt.Rows[t][1]).ToString() + Environment.NewLine +
                                                                                                 "ItemCode = " + (dt.Rows[t][2]).ToString() + Environment.NewLine +
                                                                                                 "Qntty = " + (dt.Rows[t][3]).ToString() + Environment.NewLine +
                                                                                                 "RequestStts = " + (dt.Rows[t][4]).ToString() + Environment.NewLine +
                                                                                                 "StatusFlag = " + (dt.Rows[t][8]).ToString() + Environment.NewLine +
                                                                                                 "RejectRsn = " + (dt.Rows[t][5]).ToString() + Environment.NewLine +
                                                                                                 "BillTo = " + (dt.Rows[t][6]).ToString() + Environment.NewLine +
                                                                                                 "InternalNotes = " + (dt.Rows[t][7]).ToString() + Environment.NewLine + Environment.NewLine);
                                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "before run inserQuery::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                                    command.ExecuteNonQuery();
                                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "after run inserQuery::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL exec inserQuery - CA_RequestLineDetails ::Date : " + DateTime.Now.ToString() + "; ex: " + ex + Environment.NewLine);
                                }
                            }
                            connection.Close();
                        }
                        mone = mone + 3;
                    }
                if (count > 0)
                //יש פחות שורות מ3 ולכן ישלחו אחת אחרי השניה
                {
                    File.AppendAllText(@"C:\carmel\logs\download.txt", "Inside IF less than 3 lines: " + DateTime.Now.ToString() + Environment.NewLine);
                    AddIntoPsRequestLineDetails(dt, count);
                }
                   // AddIntoPsRequestLineDetails(dt, count);

            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "FAIL CA_RequestLineDetails ::Date : " + DateTime.Now.ToString() + "; ex: " + ex + Environment.NewLine);
            }
        }
        public static void CA_LogPs(DataTable dt, int NumOfRow)
        {
            int count = dt.Rows.Count, i = NumOfRow;
            DataRow row = dt.Rows[i];
            if (count > 0)
            {
                string connectionString = m_ConnectionString;
                // var insertQuery = "INSERT into CA_Log_temp(ReqNo,LineId,ItemCode,Qntty,RequestStts,RejectRsn,BillTo,JCNum,RjctMsg,StatusFlag,Timestamp) VALUES(@ReqNo,@LineId,@ItemCode,@Qntty,@RequestStts,@RejectRsn,@BillTo,@JCNum,@RjctMsg,@StatusFlag,@Timestamp)";
                // var insertQuery = "INSERT into CA_Log_temp(ReqNo,LineId,ItemCode,Qntty,RequestStts,RejectRsn,BillTo) VALUES(@ReqNo,@LineId,@ItemCode,@Qntty,@RequestStts,@RejectRsn,@BillTo)";
                //https://stackoverflow.com/questions/9648934/insert-into-if-not-exists-sql-server
                var insertQuery = " IF EXISTS (select * from CA_Log where ReqNo = @ReqNo and LineId = @LineId) \n\r" +
               " BEGIN \n\r" +
               "     update CA_Log set ItemCode = @ItemCode, Qntty =@Qntty, BillTo =@BillTo  ,RequestStts =@RequestStts ,RejectRsn = @RejectRsn, StatusFlag = @StatusFlag,InternalNotes= @InternalNotes, EventKind = 'Update', SpclApprvl=@SpclApprvl, TimeStamp= @TimeStamp where (ReqNo = @ReqNo and LineId = @LineId); \n\r" +
               " END \n\r " +
               " ELSE" +
               " BEGIN \n\r" +
               "     INSERT into CA_Log(ReqNo,LineId,ItemCode,Qntty,RequestStts, RejectRsn,BillTo,StatusFlag, InternalNotes, EventKind, SpclApprvl, TimeStamp) VALUES(@ReqNo,@LineId,@ItemCode,@Qntty,@RequestStts,@RejectRsn,@BillTo,@StatusFlag,@InternalNotes, 'Add', @SpclApprvl, @TimeStamp) \n\r" +
               " END";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    // define your parameters ONCE outside the loop, and use EXPLICIT typing
                    command.Parameters.Add("@ReqNo", SqlDbType.Int);
                    command.Parameters.Add("@LineId", SqlDbType.Int);
                    command.Parameters.Add("@ItemCode", SqlDbType.NVarChar);
                    //command.Parameters.Add("@Qntty", SqlDbType.Char);
                    command.Parameters.Add("@Qntty", SqlDbType.Float);
                    command.Parameters.Add("@RequestStts", SqlDbType.NVarChar);
                    command.Parameters.Add("@RejectRsn", SqlDbType.NVarChar);
                    command.Parameters.Add("@BillTo", SqlDbType.Int);
                    command.Parameters.Add("@InternalNotes", SqlDbType.NVarChar);
                    /*   command.Parameters.Add("@JCNum", SqlDbType.Int);
                       command.Parameters.Add("@RjctMsg", SqlDbType.NVarChar);*/
                    command.Parameters.Add("@StatusFlag", SqlDbType.Int);
                    command.Parameters.Add("@SpclApprvl", SqlDbType.NVarChar);
                    command.Parameters.Add("@Timestamp", SqlDbType.DateTime);
                    //command.Parameters.Add("@Docentry", SqlDbType.Int);
                    //command.Parameters.Add("@SBOLineId", SqlDbType.Int);
                    connection.Open();

                    for (int t = i; t < (i + 3); t++)
                    {
                        //  SET the values
                        try
                        {
                            command.Parameters["@ReqNo"].Value = int.Parse((dt.Rows[t][0]).ToString());
                            command.Parameters["@LineId"].Value = int.Parse((dt.Rows[t][1]).ToString());
                        }
                        catch //if ReqNo or LineId are empty do not insert the line
                        {
                            continue;
                        }
                        //command.Parameters["@ReqNo"].Value = int.Parse((dt.Rows[t][0]).ToString());
                        //command.Parameters["@LineId"].Value = int.Parse((dt.Rows[t][1]).ToString());
                        command.Parameters["@ItemCode"].Value = (dt.Rows[t][2]).ToString();
                        //command.Parameters["@Qntty"].Value = int.Parse((dt.Rows[t][3]).ToString()); 
                        command.Parameters["@Qntty"].Value = float.Parse((dt.Rows[t][3]).ToString());
                        command.Parameters["@RequestStts"].Value = (dt.Rows[t][4]).ToString();
                        command.Parameters["@RejectRsn"].Value = (dt.Rows[t][5]).ToString();
                        try
                        {
                            command.Parameters["@BillTo"].Value = int.Parse((dt.Rows[t][6]).ToString());
                        }
                        catch
                        {
                            command.Parameters["@BillTo"].Value = 0;
                        }
                        //command.Parameters["@BillTo"].Value = int.Parse((dt.Rows[t][6]).ToString());
                        command.Parameters["@InternalNotes"].Value = (dt.Rows[t][7]).ToString();
                        /*    command.Parameters["@JCNum"].Value = int.Parse((dt.Rows[t][7]).ToString());
                            command.Parameters["@RjctMsg"].Value = (dt.Rows[t][8]).ToString();*/
                        command.Parameters["@StatusFlag"].Value = int.Parse((dt.Rows[t][8]).ToString());
                        command.Parameters["@SpclApprvl"].Value = (dt.Rows[t][9]).ToString();
                        command.Parameters["@Timestamp"].Value = DateTime.Parse(dt.Rows[t][10].ToString());
                        //command.Parameters["@Docentry"].Value = int.Parse((dt.Rows[t][7]).ToString());
                        //command.Parameters["@SBOLineId"].Value = int.Parse((dt.Rows[t][8]).ToString());
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
        }
        public static void AddIntoPsFiles(DataTable dt)
        {
            int i = 0, count = dt.Rows.Count;
            //פרמטרים של טבלת CA_Files
            int ReqNo, LineId;
            string Descrptn, FileLink, FileName, AddTime;
            DateTime AddDate;
            //  DateTime AddDate, AddTime;
            while (dt.Rows.Count > 0 && count > 0)
            {

                count--;
                DataRow row = dt.Rows[i++];
                if (row[6].ToString() != "")
                {
                    ReqNo = int.Parse(row[0].ToString());
                    LineId = int.Parse(row[1].ToString());
                    Descrptn = row[2].ToString();
                    FileLink = row[3].ToString();
                    FileName = row[4].ToString();
                    AddDate = DateTime.Parse(row[5].ToString());
                    AddTime = row[6].ToString();

                    using (SqlConnection con = new SqlConnection(m_ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("Files_temp", con))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                                cmd.Parameters.Add("@LineId", SqlDbType.Int).Value = LineId;
                                cmd.Parameters.Add("@Descrptn", SqlDbType.NVarChar).Value = Descrptn;
                                cmd.Parameters.Add("@FileLink", SqlDbType.NVarChar).Value = FileLink;
                                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = FileName;
                                cmd.Parameters.Add("@AddDate", SqlDbType.DateTime).Value = AddDate;
                                cmd.Parameters.Add("@AddTime", SqlDbType.NVarChar).Value = AddTime;
                            }
                            catch (Exception e)
                            {
                                EventManager.WriteEventErrorMessage("failed to put values for CA_Files table ::Date: " + DateTime.Now.ToString() + ";", e);
                            }
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_Files table", e);
                            }
                            try
                            {
                                con.Close();
                            }
                            catch (Exception e)
                            {
                                EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_Files table", e);
                            }
                        }
                    }
                }
            }
        }
        public static void AddIntoPsFiles2(DataTable dt)
        {
            int i = 0, count = dt.Rows.Count;
            //פרמטרים של טבלת CA_Files
            int ReqNo, LineId;
            string Descrptn, FileLink, FileName;

            //  DateTime AddDate, AddTime;
            while (dt.Rows.Count > 0 && count > 0)
            {
                count--;
                DataRow row = dt.Rows[i++];
                if (row[6].ToString() == "")
                {
                    ReqNo = int.Parse(row[0].ToString());
                    LineId = int.Parse(row[1].ToString());
                    Descrptn = row[2].ToString();
                    FileLink = row[3].ToString();
                    FileName = row[4].ToString();

                    using (SqlConnection con = new SqlConnection(m_ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("Files_temp2", con))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                                cmd.Parameters.Add("@LineId", SqlDbType.Int).Value = LineId;
                                cmd.Parameters.Add("@Descrptn", SqlDbType.NVarChar).Value = Descrptn;
                                cmd.Parameters.Add("@FileLink", SqlDbType.NVarChar).Value = FileLink;
                                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = FileName;
                            }
                            catch (Exception e)
                            {
                                EventManager.WriteEventErrorMessage("failed to put values for CA_Files table", e);
                            }
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_Files table", e);
                            }
                            try
                            {
                                con.Close();
                            }
                            catch (Exception e)
                            {
                                EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_Files table", e);
                            }
                        }
                    }
                }

            }
        }

        //פונקציה שמבצעת התחברות ל
        //URL
        // עם יוזר וסיסמא
        public String getData(string uid, string pwd, string url)
        {
            // Create a request using a URL that can receive a post. 
            request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.CookieContainer = cookieContainer;
            //added by liat in case customer has proxy 25/04/2016  
            IWebProxy wp = WebRequest.DefaultWebProxy;
            wp.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = wp;
            //end Change
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Create POST data and convert it to a byte array.
            string postData = "email=" + uid + "&password=" + pwd;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            request.KeepAlive = true;

            // Get the request stream.
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                String resptext = "";
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    resptext = reader.ReadToEnd();
                }


                response.Close();
                return resptext;
            }
            catch (WebException we)
            {

            }
            catch (Exception we)
            {
            }
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
            //response.Close();
            return "";

        }

        // add by orly

        //public static String deleteRequest(string reqNo)
        //{
        //    int ReqNo = int.Parse(reqNo);
        //    string connectionString = m_ConnectionString;
        //    string deleteQuery = " IF EXISTS (select * from CA_RequestDetails where ReqNo = @ReqNo) \n\r" +
        //                         " BEGIN \n\r" +
        //                         "     DELETE from CA_RequestDetails where ReqNo = @ReqNo \n\r" +
        //                         " END \n\r ";
        //    string deleteLinesQuery = " IF EXISTS (select * from CA_RequestLineDetails where ReqNo = @ReqNo) \n\r" +
        //                         " BEGIN \n\r" +
        //                         "     DELETE from CA_RequestLineDetails where ReqNo = @ReqNo \n\r" +
        //                         " END \n\r ";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand command = new SqlCommand(deleteQuery, connection);
        //        command.Parameters.AddWithValue("@ReqNo", ReqNo);
        //        try
        //        {
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //        catch (Exception e)
        //        {
        //            EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestDetails table", e);
        //        }
        //        try
        //        {
        //            connection.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestDetails table", e);
        //        }
        //    }

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand command = new SqlCommand(deleteLinesQuery, connection);
        //        command.Parameters.AddWithValue("@ReqNo", ReqNo);
        //        try
        //        {
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //        catch (Exception e)
        //        {
        //            EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestLineDetails table", e);
        //        }
        //        try
        //        {
        //            connection.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestLineDetails table", e);
        //        }

        //    }

        //    return "";

        //}

        public static String deleteLine(string reqNo, string lineId)
        {
            int ReqNo = int.Parse(reqNo);
            int LineId = int.Parse(lineId);
            string connectionString = m_ConnectionString;
            string RequestStts = "5";
            string EventKind = "'Delete'";

            //string deleteQuery = " IF EXISTS (select * from CA_RequestLineDetails where (ReqNo = @ReqNo and LineId = @LineId)) \n\r" +
            //                     " BEGIN \n\r" +
            //                     "     DELETE from CA_RequestLineDetails where (ReqNo = @ReqNo and LineId = @LineId) \n\r" +
            //                     " END \n\r ";
            string deleteQuery = " IF EXISTS (select * from CA_RequestLineDetails where (ReqNo = @ReqNo and LineId = @LineId)) \n\r" +
                                 " BEGIN \n\r" +
                                 "     UPDATE CA_RequestLineDetails SET RequestStts = 5, StatusFlag = 0 where (ReqNo = @ReqNo and LineId = @LineId) \n\r" +
                                 " END \n\r " +
                                 " IF EXISTS (select * from CA_Log where (ReqNo = @ReqNo and LineId = @LineId)) \n\r" +
                                 " BEGIN \n\r" +
                                 "     UPDATE CA_Log SET RequestStts = " + RequestStts + " , EventKind = " + EventKind + " where (ReqNo = @ReqNo and LineId = @LineId) \n\r" +
                                 " END \n\r "; ;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@ReqNo", ReqNo);
                command.Parameters.AddWithValue("@LineId", LineId);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "failed delete line,::Date: " + DateTime.Now.ToString() + "; ex: " + e + Environment.NewLine);
                    EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_RequestLineDetails table", e);
                }
                try
                {
                    connection.Close();
                }
                catch (Exception e)
                {
                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "failed close conn,::Date : " + DateTime.Now.ToString() + "; ex: " + e + Environment.NewLine);
                    EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_RequestLineDetails table", e);
                }
            }
            return "";
        }

        public static string zipFiles(string path)
        {
            File.AppendAllText(@"C:\Carmel\logs\download.txt", "enter zipFiles" + Environment.NewLine);
            //string zipPath = @"c:\Carmel\calldata.zip";
            Engine e = new Engine();
            string get = "";
            try
            {
                get = e.getDownlaodZipFile(m_userforzip, m_passforzip, @"https://chaina-motorsltd.dira2.co.il/systems/downloadAllZipFiles");
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail download zipFile,::Date : " + DateTime.Now.ToString() + "; ex: " + ex + Environment.NewLine);
            }
            //string extractPath = @"C:\temp\callFiles";
            //string zipPath = @"c:\Carmel\calldata.zip";
            //string extractPath = @"c:\extract";
            //ZipFile zipFile = new ZipFile(@"C:\Users\Doctor\Downloads\calldata.zip");
            //Array.ForEach(Directory.GetFiles(@"C:\temp\callFiles\"), File.Delete); //for files
            //Directory.Delete(@"C:\temp\callFiles");
            //Directory.Delete(@"C:\temp\callFiles", true);
            try
            {
                Directory.Delete(m_zipPath, true);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail delete directory" + m_zipPath + " | ex: " + ex + Environment.NewLine);
            }
            try
            {
                //ZipFile.ExtractToDirectory(@"C:\Carmel\logs\calldata.zip", extractPath);
                // File.Copy(@"C:\Carmel\calldata.zip", @"C:\Carmel\zipBackups\");
                ZipFile.ExtractToDirectory(@"C:\Carmel\calldata.zip", m_zipPath);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail extract zipFile, ex: " + ex + Environment.NewLine);
                Directory.CreateDirectory(m_zipPath);
                return "";
            }
            //ZipFile.ExtractToDirectory(@"C:\Carmel\calldata.zip", m_zipPath);
            //string extractPath = @"c:\extract";
            string ReqNo = "", lineId = "";
            //string sub = input.Substring(0, 3);
            int count = 0, countF = 0;

            //ZipFile.ExtractToDirectory(zipPath, m_zipPath);

            DataTable dt = new DataTable();
            DataRow dr;

            for (int col = 0; col < 8; col++)
                dt.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            string[] filesindirectory = Directory.GetDirectories(m_zipPath);
            string[] orderExport = Directory.GetDirectories(filesindirectory[0]);
            try
            {
                if (!Directory.Exists(@"C:\Carmel\zipBackups"))
                    Directory.CreateDirectory(@"C:\Carmel\zipBackups");

                foreach (string subdir in orderExport)
                {
                    var destFileName = System.IO.Path.GetFileNameWithoutExtension(subdir);
                    try
                    {
                        destFileName = destFileName + '_' + DateTime.Now.Day.ToString() + '-' + DateTime.Now.Month.ToString() + '-' + DateTime.Now.Year.ToString() + '_' + DateTime.Now.Hour.ToString() + '-' + DateTime.Now.Minute.ToString() + '-' + DateTime.Now.Second.ToString();

                        destFileName = destFileName + '.' + System.IO.Path.GetExtension(subdir);
                        ZipFile.CreateFromDirectory(subdir, @"C:\Carmel\zipBackups\" + destFileName);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "can't zip the directory " + subdir + " ex: " + ex + Environment.NewLine);
                    }
                    ReqNo = System.IO.Path.GetFileName(subdir);
                    ReqNo = ReqNo.Substring(6);
                    countF++;
                    foreach (string img in Directory.GetFiles(subdir))
                    {
                        int StatusFlag = 0;
                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "enter foreach img" + Environment.NewLine);

                        string imgName = System.IO.Path.GetFileName(img);
                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "imgName: " + imgName + Environment.NewLine);
                        //string line = imgName.Remove(imgName.Length - 4);
                        string line = imgName.Split('-')[1];
                        dr = dt.NewRow();
                        lineId = line;
                        File.AppendAllText(@"C:\Carmel\logs\download.txt", "lineId: " + lineId + Environment.NewLine);
                        try
                        {
                            //lineId = "0";
                            dr[0] = ReqNo;
                            dr[1] = lineId;
                            dr[2] = ""; //description?
                            dr[3] = destFileName;//img; //fileLink
                            dr[4] = imgName; //fileName
                            dr[5] = DateTime.Today.ToString("dd-MM-yyyy");//currDate
                            dr[6] = DateTime.Now.ToString("HH:mm:ss");  //currTime
                            dr[7] = StatusFlag;

                            dt.Rows.Add(dr);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail on pic: " + imgName + " ex: " + ex + Environment.NewLine);
                        }
                    }
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail foreach subdir; ex: " + ex + Environment.NewLine);
            }

            insertFiles(dt);
            return "";
        }

        public static string insertFiles(DataTable dt)
        {
            File.AppendAllText(@"C:\Carmel\logs\download.txt", "enter insertFiles" + Environment.NewLine);
            int ReqNo, LineIds;
            string filename;
            //string deleteQuery = "IF EXISTS (select * from CA_Files where ReqNo=@ReqNo) BEGIN DELETE from CA_Files where ReqNo=@ReqNo END";
            string deleteQuery = "IF EXISTS (select * from CA_Files where fileName=@fileName) BEGIN DELETE from CA_Files where fileName=@fileName END";
            using (SqlConnection con = new SqlConnection(m_ConnectionString))
            {
                int s = 0, countS = dt.Rows.Count;
                //changed 2/7/19 by Yura Korovkin - while production
                //while (dt.Rows.Count > 0 && countS > 0)
                {
                    countS--;
                    File.AppendAllText(@"C:\Carmel\logs\download.txt", "dt.Rows.Count= " + dt.Rows.Count  + Environment.NewLine);
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        try
                        {
                            DataRow row = dt.Rows[s++];
                            ReqNo = int.Parse(row[0].ToString());
                            //LineIds = int.Parse(row[1].ToString());
                            filename = row[4].ToString();
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "row[1].ToString()= " + row[1].ToString() + " row[2].ToString(): " + row[2].ToString() + Environment.NewLine);
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "row[3].ToString()= " + row[3].ToString() + " row[5].ToString(): " + row[5].ToString() + Environment.NewLine);
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "ReqNo= " + ReqNo + " filename: " + filename + Environment.NewLine);
                            //File.AppendAllText(@"C:\Carmel\logs\download.txt", "in cmd: ReqNo=" + ReqNo + ", LineId=" + LineId + ", Desc=" + Descrptn + ", fileLink=" + Filelink + ", fileName=" + FileName + Environment.NewLine);
                            //cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                            //cmd.Parameters.Add("@LineId", SqlDbType.Int).Value = LineIds;


                            //changed 2/7/19 by Yura Korovkin - while production
                            //cmd.Parameters.Add("@LineId", SqlDbType.NVarChar).Value = filename;
                            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = filename;
                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail delete all old req pics in CA_Files table, ex: " + e + Environment.NewLine);
                            EventManager.WriteEventErrorMessage("failed to delete all old req pics in CA_Files table", e);
                        }
                        try
                        {
                            con.Open();
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "befor deleting old imgs" + Environment.NewLine);
                            cmd.ExecuteNonQuery();
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "after deleting old imgs" + Environment.NewLine);
                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "failed to open ConnectionString for delete in CA_Files table. ex: " + e + Environment.NewLine);
                            EventManager.WriteEventErrorMessage("failed to open ConnectionString for delete in CA_Files table", e);
                        }
                        try
                        {
                            con.Close();
                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "failed to close ConnectionString for delete in  CA_Files table. ex: " + e + Environment.NewLine);
                            EventManager.WriteEventErrorMessage("failed to close ConnectionString for delete in  CA_Files table", e);
                        }
                    }
                }
            }



            string insertQuery = " IF NOT EXISTS (select * from CA_Files where fileName = @fileName) \n\r" +//" IF NOT EXISTS (select * from CA_Files where fileName = @fileName) \n\r" +
" BEGIN \n\r" +
//"DELETE from CA_Files where fileName = @fileName \n\r" +
"     INSERT into CA_Files(ReqNo,LineId,Descrptn,Filelink,FileName,AddDate,AddTime,StatusFlag) VALUES(@ReqNo,@LineId,@Descrptn,@Filelink,@FileName,@AddDate,@AddTime,@StatusFlag) \n\r" +
" END \n\r " +
" ELSE SELECT 0";
            int i = 0, count = dt.Rows.Count;
            DateTime d = new DateTime(1753, 10, 10); //deafult
            int LineId, StatusFlag;//EstCost
            string Descrptn, Filelink, FileName, AddTime;
            DateTime AddDate;

            while (dt.Rows.Count > 0 && count > 0)
            {
                count--;
                DataRow row = dt.Rows[i++];
                //זימון פונקציה שתבדוק עבור סוג בקשה אם קיים אם כן תעדכן תאור אחרת תוסיף לטבלה
                ReqNo = int.Parse(row[0].ToString());
                LineId = int.Parse(row[1].ToString());
                Descrptn = row[2].ToString();
                Filelink = @"C:\Carmel\zipBackups\" + row[3].ToString();
                FileName = row[4].ToString();
                AddDate = DateTime.Parse(row[5].ToString());
                //AddTime = DateTime.Parse(row[6].ToString());
                AddTime = row[6].ToString();
                StatusFlag = int.Parse(row[7].ToString());
                // EventManager.WriteEventInfoMessage("start ConnectionString to db for CA_Files table");
                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        try
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "in cmd: ReqNo=" + ReqNo + ", LineId=" + LineId + ", Desc=" + Descrptn + ", fileLink=" + Filelink + ", fileName=" + FileName + Environment.NewLine);
                            //cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@ReqNo", SqlDbType.Int).Value = ReqNo;
                            cmd.Parameters.Add("@LineId", SqlDbType.Int).Value = LineId;
                            cmd.Parameters.Add("@Descrptn", SqlDbType.NVarChar).Value = Descrptn;
                            cmd.Parameters.Add("@Filelink", SqlDbType.NText).Value = Filelink;
                            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = FileName;
                            cmd.Parameters.Add("@AddDate", SqlDbType.DateTime).Value = AddDate;
                            cmd.Parameters.Add("@AddTime", SqlDbType.NVarChar).Value = AddTime;
                            cmd.Parameters.Add("@StatusFlag", SqlDbType.Int).Value = StatusFlag;
                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail on line: " + ReqNo + "-" + LineId + " ex: " + e + Environment.NewLine);
                            EventManager.WriteEventErrorMessage("failed to put values for CA_Files table", e);
                        }
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "failed to open ConnectionString for CA_Files table. ex: " + e + Environment.NewLine);
                            EventManager.WriteEventErrorMessage("failed to open ConnectionString for CA_Files table", e);
                        }
                        try
                        {
                            con.Close();
                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(@"C:\Carmel\logs\download.txt", "failed to close ConnectionString for CA_Files table. ex: " + e + Environment.NewLine);
                            EventManager.WriteEventErrorMessage("failed to close ConnectionString for CA_Files table", e);
                        }
                    }
                }
            }
            File.AppendAllText(@"C:\Carmel\logs\download.txt", "exit insertFiles" + Environment.NewLine);
            return "";
        }

        public String getDownlaodZipFile(string uid, string pwd, string url)
        {
            try
            {
                if (File.Exists(@"C:\Carmel\calldata.zip"))
                    File.Delete(@"C:\Carmel\calldata.zip");
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail delete Zipfile: " + ex + Environment.NewLine);
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail delete Zipfile inner excep: " + ex.InnerException + Environment.NewLine);
            }
            // Create a request using a URL that can receive a post. 
            request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.CookieContainer = cookieContainer;
            //added by liat in case customer has proxy 25/04/2016  
            IWebProxy wp = WebRequest.DefaultWebProxy;
            wp.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = wp;
            //end Change
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Create POST data and convert it to a byte array.
            string postData = "email=" + uid + "&password=" + pwd;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            request.KeepAlive = true;

            // Get the request stream.
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                //   response.ContentType = "application/zip"; //@todo this line kills download find better way to do this
                String resptext = "";
                using (Stream output = File.OpenWrite(@"C:\Carmel\calldata.zip"))
                using (Stream stream = response.GetResponseStream())
                {
                    stream.CopyTo(output);
                }


                response.Close();
                return resptext;
            }
            catch (WebException we)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail copy files: " + we + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\download.txt", "fail copy files: " + ex + Environment.NewLine);
            }
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
            //response.Close();
            return "";

        }

    }
}

