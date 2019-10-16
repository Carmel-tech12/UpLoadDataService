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
using System.Data.SqlClient;
using System.Threading;
using System.Xml.Linq;


namespace UpLoadDataService
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
        // by shira
        //   public static string m_nameView = string.Empty;
        XmlDocument doc = new XmlDocument();



        public Engine()
        {
            LoadGlobalParameters();
        }



        private void LoadGlobalParameters()
        {
            /*try
            {
               // doc.Load("Config.xml");
               // doc.Load(@"C:\Program Files\UpLoadDataService\UpLoadDataService\bin\Debug\Config.xml");
                
                doc.Load(@""+ConfigurationManager.AppSettings["pathXml"]);
            }
            catch (Exception ex)
            {
                
               EventManager.WriteEventErrorMessage("can not load the doc", ex);
            }
            
            m_ConnectionString = doc.SelectSingleNode(@"Parameters/ConenctionString").InnerText+"&quot;";
            m_user = doc.SelectSingleNode(@"Parameters/Name").InnerText;
            m_pass = doc.SelectSingleNode(@"Parameters/Pass").InnerText;
            m_url = doc.SelectSingleNode(@"Parameters/Url").InnerText;
           
          //  m_nameView = doc.SelectSingleNode(@"Parameters/nameView").InnerText;
           
            m_folderpath = doc.SelectSingleNode(@"Parameters/folderpath").InnerText;
            ConnectionManager.ConnectionString = m_ConnectionString;
            ConnectionManager.Provider = doc.SelectSingleNode(@"Parameters/Provider").InnerText;*/
            try
            {
                // doc.Load("Config.xml");
                // doc.Load(@"C:\Program Files\UpLoadDataService\UpLoadDataService\bin\Debug\Config.xml");
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "try load pathxml" + Environment.NewLine);
                doc.Load(@"" + ConfigurationManager.AppSettings["pathXml"]);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail load pathxml" + Environment.NewLine);
                EventManager.WriteEventErrorMessage("can not load the doc", ex);
            }

            m_ConnectionString = doc.SelectSingleNode(@"Parameters/ConenctionString").InnerText + "&quot;";
            m_user = doc.SelectSingleNode(@"Parameters/Name").InnerText;
            m_pass = doc.SelectSingleNode(@"Parameters/Pass").InnerText;
            m_url = doc.SelectSingleNode(@"Parameters/Url").InnerText;
            //  m_nameView = doc.SelectSingleNode(@"Parameters/nameView").InnerText;
            m_folderpath = doc.SelectSingleNode(@"Parameters/folderpath").InnerText;
            ConnectionManager.ConnectionString = m_ConnectionString;
            ConnectionManager.Provider = doc.SelectSingleNode(@"Parameters/Provider").InnerText;
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "afterprovider" + Environment.NewLine);
            //   csv_file_path = ConfigurationManager.AppSettings["pathofcsvfile"].ToString();

        }

        //ADDED by Raz 24/1/2019
        //public void SendCallsForEmailsAndSMSs()
        //{
        //    try
        //    {
        //        DataSet dataSet = new DataSet();
        //        File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in SendCallsForEmailsAndSMSs" + Environment.NewLine);
        //        var command = "";



        //    }
        //    catch(Exception ex)
        //    {
        //        File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Failed SendCallsForEmailsAndSMSs ex: "+ ex + Environment.NewLine);
        //    }

        //    //עדכון סטטוס לשליחה= 2
        //    UpdateCA_RequestDetails_temp();
        //    UpdateCA_RequestLineDetails_temp();
        //}


        #region  Procedures
        public void LoadDataOfProcedurs()
        {
            try
            {


                string command;
                XmlNodeList prodedursList = doc.GetElementsByTagName("procedure");  // XmlNodeList prodedursList = doc.GetElementsByTagName("Prodedurs");
                //File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "procedures found: " + prodedursList.ToString() + Environment.NewLine);
                foreach (XmlNode pro in prodedursList)
                {
                    //specificUrl contain the sequel of the url
                    string specificUrl = pro.Attributes["myUrl"].Value;

                    command = pro.InnerText;

                    //int timeProcedure = int.Parse(doc.SelectSingleNode(@"Parameters/times/" +command).InnerText);



                    string id = doc.SelectSingleNode(@"Parameters/ID/" + command).InnerText;
                    //string temp = XmlNode.SelectSingleNode("field[@name='post_title']").InnerText;
                    //command = pro.InnerText;

                    int timeInterval = int.Parse(doc.SelectSingleNode(@"Parameters/times/execute[@id =" + id + "]").InnerText);
                    //int hour = int.Parse(ConfigurationManager.AppSettings["hour"].ToString());
                    //int minute = int.Parse(ConfigurationManager.AppSettings["Minute"].ToString());
                    //int h = DateTime.Now.Hour;
                    //int m = DateTime.Now.Minute;
                    //int time = 0;
                    //if(h-hour == 1)
                    //{
                    //    time = 60;
                    //}
                    //time = time + (m - minute);

                    ////if it's not the time to execute this procedure, continue
                    //if(time < timeProcedure)
                    //{
                    //    continue;
                    //}
                    //if(UpLoadDataService.numService % timeProcedure != 0)
                    //{
                    //    continue;
                    //}

                    //if (UpLoadDataService.numService % timeProcedure != 0)
                    //{
                    //    continue;
                    //}
                    //connect to DB to get the last update of the procedure
                    DateTime last = getLastDate(id);
                    DateTime n = DateTime.Now;
                    //timeProcedure = 1440;
                    TimeSpan diff = (n - last).Duration();
                    if (diff.TotalMinutes <= timeInterval)
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            string file = "";
                            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Try procedure ::Date : " + DateTime.Now.ToString() + "procedure Number: " + id + Environment.NewLine);
                            if (command.Equals("sap"))
                            {
                                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "toXML command ::Date : " + DateTime.Now.ToString() + "procedure Number: " + command + Environment.NewLine);
                                toXml(command);
                                file = @"C:\Carmel\sapNum.xml";
                                //sapXml(specificUrl);
                                //return;

                            }
                            //ADDED by Raz 24/1/2019
                            //else if (command.Equals("workCards"))
                            //{
                            //    toXml(command);
                            //    file = @"C:\Carmel\workCards.xml";
                            //    //עדכון סטטוס לשליחה= 2
                            //    UpdateCA_RequestDetails_temp();
                            //    UpdateCA_RequestLineDetails_temp();
                            //}
                            else
                            {
                                DataSet dataSet = new DataSet();
                                dataSet = ConnectionManager.GetDataForProcedures(command);
                                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before savefile procedure" + Environment.NewLine);

                                file = Savefile(ToCSV2(dataSet.Tables[0], ",", true), "from_Prodedure_" + command);
                                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after savefile procedure: " + command + Environment.NewLine);
                                //string file = Savefile(ToCSV2(dataSet.Tables[0], ",", true), "from_Prodedure_" + DateTime.Now.ToFileTime() + ".csv");
                                //my test//string file = @"C:\rooti\temp\hash\Procedur_131474298261702609.csv";
                            }
                            try
                            {
                                //EventManager.WriteEventInfoMessage("try do login");
                                doLogin(m_user, m_pass, m_url);

                                //if success do login
                                try
                                {
                                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Try upload file ::Date : " + DateTime.Now.ToString() + "procedure Number: " + id + Environment.NewLine);
                                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Try upload file, the file is: " + file + " ::Date Now : " + DateTime.Now.ToString() + Environment.NewLine);
                                    //EventManager.WriteEventInfoMessage("try upload file");
                                    uploadfile(file, specificUrl);
                                    //EventManager.WriteEventInfoMessage("finish upload file");
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail uploadfile ex: " + ex + Environment.NewLine);
                                    EventManager.WriteEventInfoMessage("cath upload file");
                                }
                                try
                                {
                                    //EventManager.WriteEventInfoMessage("try do logout");
                                    doLogout(m_url);
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail do logOut ex: " + ex + Environment.NewLine);
                                    EventManager.WriteEventInfoMessage("cath do loout");
                                }


                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail login ex: " + ex + Environment.NewLine);
                                EventManager.WriteEventErrorMessage("catch login", ex);
                            }

                            updateLastDate(id, n);

                            //--commented By Raz and Yura 1/4/2019 -- update StatusFlag to 2 moved to the sap procedure----
                            // UpdateCA_RequestDetails_temp();
                            // UpdateCA_RequestLineDetails_temp();

                        }
                        catch (Exception ex)
                        {
                            EventManager.WriteEventErrorMessage("Error while reading procedur from China", ex);

                        }
                    }


                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail LoadDataOfProcedurs, ex: " + ex + Environment.NewLine);

            }
        }


        #endregion
        public void sapXml(string specifyUrl)
        {
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before sap" + Environment.NewLine);
            uploadfile(@"C:\Carmel\sapNum.xml", specifyUrl);
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after sap" + Environment.NewLine);

        }


        #region  Queries 
        DataSet dataSet;
        public void LoadDataOfQueries()
        {

            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in LoadDataOfQueries::Date : " + DateTime.Now.ToString() + Environment.NewLine);
            try
            {


                DataSet dataSet;
                string command;
                XmlNodeList QueriesList = doc.GetElementsByTagName("Query");  // XmlNodeList prodedursList = doc.GetElementsByTagName("Prodedurs");
                //File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "procedures found: " + QueriesList.ToString() + Environment.NewLine);                                       //EventManager.WriteEventInfoMessage("QueriesList");
                foreach (XmlNode pro in QueriesList)
                {
                    command = pro.InnerText;
                    //specificUrl contain the sequel of the url
                    string specificUrl = pro.Attributes["myUrl"].Value;
                    string nameView = pro.Attributes["myUrl"].Value;
                    string nView = nameView;
                    //EventManager.WriteEventInfoMessage("nameView");

                    string name = nameView.Split('/').Last();
                    if (!name[0].Equals('r'))
                    {
                        int x = nView.IndexOf("/", 2);
                        //int x = nameView.IndexOf("/", 4);
                        //EventManager.WriteEventInfoMessage("x" + " " + x.ToString());
                        //nameView = nameView.Replace("/", " ");
                        //EventManager.WriteEventInfoMessage("nameView" + " " + nameView);
                        //nameView=nameView.Substring(0,x);
                        nView = nView.Substring(1, x - 1);
                        //nameView = nameView.Split('/').Last();
                    }
                    else
                    {
                        nView = name;
                    }
                    //string name = nameView.Substring(1);
                    ////string time =
                    ////EventManager.WriteEventInfoMessage("nameView" + " " + nameView);
                    //int timeProcedure = int.Parse(doc.SelectSingleNode(@"Parameters/times/"+name).InnerText);
                    ////int timeProcedure = 5;
                    //if (UpLoadDataService.numService % timeProcedure != 0)
                    //{
                    //    continue;
                    //}

                    string id = doc.SelectSingleNode(@"Parameters/ID/" + nView).InnerText;


                    //string temp = XmlNode.SelectSingleNode("field[@name='post_title']").InnerText;
                    command = pro.InnerText;
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "In LoadDataOfQueries, ID is: " + id + " ::Date Now : " + DateTime.Now.ToString() + Environment.NewLine);
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "In LoadDataOfQueries, the command is: " + command + " ::Date Now : " + DateTime.Now.ToString() + Environment.NewLine);
                    int timeInterval = int.Parse(doc.SelectSingleNode(@"Parameters/times/execute[@id =" + id + "]").InnerText);

                    DateTime last = getLastDate(id);
                    DateTime n = DateTime.Now;
                    //timeProcedure = 1440;
                    TimeSpan diff = (n - last).Duration();
                    if (diff.TotalMinutes <= timeInterval)
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {

                            //DataSet dataSet = new DataSet();
                            dataSet = new DataSet();
                            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in try query, query: " + id + Environment.NewLine);
                            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before get data queries" + Environment.NewLine);
                            dataSet = ConnectionManager.GetDataForQueries(command);
                            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after get data queries" + Environment.NewLine);
                            //string file = Savefile(ToCSV2(dataSet.Tables[0], ",", true), );
                            //by shira

                            string file = "";
                            try
                            {
                                file = Savefile(ToCSV2(dataSet.Tables[0], ",", true), nView);
                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail Save file, ex: " + ex + Environment.NewLine);
                                continue;
                            }
                            try
                            {
                                //EventManager.WriteEventInfoMessage("try do login");
                                doLogin(m_user, m_pass, m_url);

                                //if success do login
                                try
                                {
                                    //EventManager.WriteEventInfoMessage("try upload file");
                                    uploadfile(file, specificUrl);
                                    //EventManager.WriteEventInfoMessage("finish upload file");
                                }
                                catch (Exception ex)
                                {
                                    EventManager.WriteEventInfoMessage("cath upload file");
                                }
                                try
                                {
                                    //EventManager.WriteEventInfoMessage("try do logout");
                                    doLogout(m_url);
                                }
                                catch (Exception ex)
                                {
                                    EventManager.WriteEventInfoMessage("cath do loout");
                                }
                            }
                            catch (Exception ex)
                            {
                                EventManager.WriteEventErrorMessage("catch login", ex);
                            }

                            updateLastDate(id, n);


                        }
                        catch (Exception ex)
                        {
                            EventManager.WriteEventErrorMessage("Error while reading Query from China", ex);
                            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Error while reading Query from China, ex: " + ex + Environment.NewLine);
                        }
                    }



                }


                //עדכון סטטוס לשליחה= 2
                //UpdateCA_RequestDetails_temp();
                //UpdateCA_RequestLineDetails_temp();
            }
            catch (Exception ex)
            {

                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "fail LoadDataOfQueries, ex: " + ex + Environment.NewLine);
            }
        }


        #endregion
        public void UpdateCA_RequestDetails_temp()
        {


            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in update requestDetails::Date : " + DateTime.Now.ToString() + Environment.NewLine);
            //https://stackoverflow.com/questions/15246182/sql-update-statement-in-c-sharp
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {

                String st = "UPDATE CA_RequestDetails SET StatusFlag = " + 2 +
                     "WHERE StatusFlag = " + 1;
                //String st = "UPDATE CA_RequestDetails SET StatusFlag = " + 1 +
                //     "WHERE StatusFlag = " + 0;

                SqlCommand sqlcom = new SqlCommand(st, connection);
                try
                {
                    connection.Open();
                    sqlcom.ExecuteNonQuery();
                    //EventManager.WriteEventInfoMessage("update successful");
                }
                catch (SqlException ex)
                {
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "faile update requestDetails, ex: " + ex + Environment.NewLine);
                    EventManager.WriteEventInfoMessage("Update CA_RequestDetails_temp failed" + ex.Message);
                }
                connection.Close();
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "After update requestLineDetails::Date : " + DateTime.Now.ToString() + Environment.NewLine);
            }
        }
        public void UpdateCA_RequestLineDetails_temp()
        {

            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in update requestLineDetails::Date : " + DateTime.Now.ToString() + Environment.NewLine);
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {

                String st = "UPDATE CA_RequestLineDetails SET StatusFlag = " + 2 +
                     "WHERE StatusFlag = " + 1;

                //String st = "UPDATE CA_RequestDetails SET StatusFlag = " + 1 +
                //     "WHERE StatusFlag = " + 0;

                SqlCommand sqlcom = new SqlCommand(st, connection);
                try
                {
                    connection.Open();
                    sqlcom.ExecuteNonQuery();
                    //EventManager.WriteEventInfoMessage("update successful");
                }
                catch (SqlException ex)
                {
                    EventManager.WriteEventInfoMessage("Update CA_RequestLineDetails_temp failed" + ex.Message);
                }

                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "After update requestLineDetails::Date : " + DateTime.Now.ToString() + Environment.NewLine);
                connection.Close();
            }
        }

        //add by orly
        public void toXml(string commandName)
        {


            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Start of Function toXml ::Date : " + DateTime.Now.ToString() + Environment.NewLine);
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "toXml, command: " + commandName + Environment.NewLine);
            string connStr = @"" + m_ConnectionString;//.Substring(pos, m_ConnectionString.Length -pos - 1 );
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            connection = null;
            connection = new SqlConnection(connStr);
            connection.Open();
            SqlCommand cmd = new SqlCommand(commandName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "toXml, connectionstring: " + connStr + Environment.NewLine);
            //cmd.Parameters.Add(new SqlParameter("@CustomerID", custId));

            //System.Xml.XmlReader myXmlReader = cmd.ExecuteXmlReader();
            //System.Xml.XmlReader myXmlReader = cmd.ExecuteXmlReader();
            try
            {
                using (XmlReader reader = cmd.ExecuteXmlReader())
                {

                    XmlDocument dom = new XmlDocument();


                    dom.CreateXmlDeclaration("1.0", "UTF-8", null);

                    dom.Load(reader);
                    //dom.Declaration = new XDeclaration("1.0", "utf-8", null);
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "dom: " + dom.ToString() + ";;; date: " + DateTime.Now.ToString() + Environment.NewLine);
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.OmitXmlDeclaration = true;
                    settings.Encoding = Encoding.UTF8;


                    //adding sapNum back upadded by Yura carmel-tech 14.3.19
                    if (dom.ChildNodes.Count > 0)
                    {
                        using (var writer = XmlTextWriter.Create(@"C:\Carmel\SapNumXmlBackUps\" + "SapNumBackUp" + "-" + DateTime.Now.Date.ToString("dd_MM_yyy") + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".xml", settings))
                        {
                            dom.WriteContentTo(writer);
                        }
                    }
                    else
                    {
                        File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "sapXml is empty; date: " + DateTime.Now.ToString() + Environment.NewLine);
                    }
                    //try
                    //{
                    //    File.WriteAllText(@"C:\Carmel\SapNumXmlBackUps\" + "SapNumBackUp" + "-" + DateTime.Now.Date.ToString("dd_MM_yyy") + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".xml", sapXmlBackUpDoc.InnerXml);
                    //}
                    //catch (Exception ex)
                    //{
                    //    File.AppendAllText(@"C:\carmel\sapNumBackError.txt", "fail save backup Xml Sap" + ex + Environment.NewLine);
                    //}
                    //changed by Raz 28/1/2019
                    //if (commandName.Equals("sap"))
                    //{
                    using (var writer = XmlTextWriter.Create(@"C:\Carmel\sapNum.xml", settings))
                    {
                        dom.WriteContentTo(writer);
                    }
                    //}
                    //else if(commandName.Equals("workCards"))
                    //{
                    //    using (var writer = XmlTextWriter.Create(@"C:\Carmel\workCards.xml", settings))
                    //    {
                    //        dom.WriteContentTo(writer);
                    //    }
                    //}


                    //reader.close();
                }
                //reader.close();
                //myXmlReader.Close();
                //return myXmlReader;
                connection.Close();
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "faild to create sapXml; date: " + DateTime.Now.ToString() + " ex: " + ex + Environment.NewLine);

            }
        }

        public string ToCSV2(DataTable table, string delimiter, bool includeHeader)
        {
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in ToCSV2" + Environment.NewLine);
            var result = new StringBuilder();

            if (includeHeader)
            {
                foreach (DataColumn column in table.Columns)
                {
                    result.Append(column.ColumnName);
                    result.Append(delimiter);
                }

                result.Remove(--result.Length, 0);
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before ToCSV2 append" + Environment.NewLine);
                result.Append(Environment.NewLine);
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after ToCSV2 append" + Environment.NewLine);
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    if (item is DBNull)
                        result.Append(delimiter);
                    else
                    {
                        string itemAsString = item.ToString();
                        // Double up all embedded double quotes
                        itemAsString = itemAsString.Replace("\"", "\"\"");

                        // To keep things simple, always delimit with double-quotes
                        // so we don't have to determine in which cases they're necessary
                        // and which cases they're not.
                        itemAsString = "\"" + itemAsString + "\"";

                        result.Append(itemAsString + delimiter);
                    }
                }

                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        private string Savefile(string text, string nameFile)
        {
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in saveFile" + Environment.NewLine);
            bool folderExists = Directory.Exists(m_folderpath);
            if (!folderExists)
                Directory.CreateDirectory(m_folderpath);

            //  string fileName = Path.Combine(m_folderpath, nameFile + "_" + DateTime.Now.ToString().Replace("/", "-").Replace(":", "-") + ".csv");
            string fileName = Path.Combine(m_folderpath, nameFile + ".csv");
            File.Create(fileName).Dispose();
            using (TextWriter tw = new StreamWriter(fileName, true, Encoding.GetEncoding("windows-1255")))
            {
                tw.WriteLine(text);
                tw.Close();
            }



            return fileName;



        }

        //private string SaveXml(string text, string nameFile)
        //{
        //    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in saveFile" + Environment.NewLine);
        //    bool folderExists = Directory.Exists(m_folderpath);
        //    if (!folderExists)
        //        Directory.CreateDirectory(m_folderpath);

        //    //  string fileName = Path.Combine(m_folderpath, nameFile + "_" + DateTime.Now.ToString().Replace("/", "-").Replace(":", "-") + ".csv");
        //    string fileName = Path.Combine(m_folderpath, nameFile + ".xml");
        //    File.Create(fileName).Dispose();
        //    using (TextWriter tw = new StreamWriter(fileName, true, Encoding.GetEncoding("windows-1255")))
        //    {
        //        tw.WriteLine(text);
        //        tw.Close();
        //    }



        //    return fileName;



        //}

        private void uploadfile(string filePath, string specificUrl)
        {
            try
            {


                string url = m_url + specificUrl;//@"importcustomercards/exportcustomercards/importcsv";
                Console.WriteLine(url);
                //Identificate separator
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                //Encoding
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                //Creation and specification of the request
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url); //sVal is id for the webService
                wr.CookieContainer = cookieContainer;
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

                //string sAuthorization = "login:password";//AUTHENTIFICATION BEGIN
                //byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sAuthorization);
                //string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
                //wr.Headers.Add("Authorization: Basic " + returnValue); //AUTHENTIFICATION END
                Stream rs = wr.GetRequestStream();


                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}"; //For the POST's format

                //Writting of the file
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(filePath);
                rs.Write(formitembytes, 0, formitembytes.Length);

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string f = filePath;
                string check = f.Substring(f.Length - 3);
                string header;
                if (check.Equals("xml"))
                {
                    header = string.Format(headerTemplate, "file", "AAAAAAA.xml", wr.ContentType);
                }
                else
                {
                    header = string.Format(headerTemplate, "file", "AAAAAAA.csv", wr.ContentType);
                }
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();
                rs = null;

                WebResponse wresp = null;
                try
                {
                    //Get the response
                    wresp = wr.GetResponse();
                    Stream stream2 = wresp.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    string responseData = reader2.ReadToEnd();
                    //EventManager.WriteEventInfoMessage("response is:\n" + responseData);
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "GetResponse for url: " + specificUrl + "; resp: " + responseData + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "failed GetResponse for url: " + specificUrl + "; ex: " + ex + Environment.NewLine);
                }
                finally
                {
                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }
                    wr = null;
                }

            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "failed upload file ex: " + ex + Environment.NewLine);
            }

        }

        public bool doLogin(string uid, string pwd, string url)
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
                response.Close();
            }
            catch (WebException we)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "login failed ex: " + we + Environment.NewLine);
            }
            catch (Exception we)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "login failed ex: " + we + Environment.NewLine);
            }
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
            //response.Close();
            return true;

        }

        public void doLogout(string url)
        {
            try
            {
                // Create a request using a URL that can receive a post. 
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.CookieContainer = cookieContainer;

                // Set the Method property of the request to POST.
                request.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException we)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "logout failed ex: " + we + Environment.NewLine);
            }
            catch (Exception we)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "logout failed ex: " + we + Environment.NewLine);
            }

        }

        public DateTime getLastDate(string pName)
        {
            DateTime d = new DateTime();
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "In getLastDate, ID is: " + pName + " ::Date Now : " + DateTime.Now.ToString() + Environment.NewLine);
            //File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in update requestDetails" + Environment.NewLine);
            //https://stackoverflow.com/questions/15246182/sql-update-statement-in-c-sharp
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {

                String st = "select allTables.lastUpdate from allTables WHERE configId = @pName";

                SqlCommand sqlcom = new SqlCommand(st, connection);

                try
                {
                    sqlcom.Parameters.AddWithValue("@pName", pName);
                    connection.Open();
                    SqlDataReader dr = sqlcom.ExecuteReader();
                    //dr = sqlcom.ExecuteReader();
                    //EventManager.WriteEventInfoMessage("update successful");
                    while (dr.Read())
                    {
                        d = Convert.ToDateTime(dr["lastUpdate"]);
                    }
                }
                catch (SqlException ex)
                {
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "faile select allTables" + ex.Message + Environment.NewLine);
                    //EventManager.WriteEventInfoMessage("Update allTables failed" + ex.Message);
                }
                connection.Close();
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after getLastDate" + Environment.NewLine);
                return d;
            }
        }

        public void updateLastDate(string pName, DateTime n)
        {
            DateTime d = new DateTime();
            d = DateTime.Now;
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "In updateLastDate, ID is: " + pName + " ::Date Now : " + DateTime.Now.ToString() + Environment.NewLine);
            //https://stackoverflow.com/questions/15246182/sql-update-statement-in-c-sharp
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {

                String st = "update allTables set lastUpdate =  @d WHERE configId = @pName";

                SqlCommand sqlcom = new SqlCommand(st, connection);

                try
                {
                    sqlcom.Parameters.AddWithValue("@pName", pName);
                    sqlcom.Parameters.AddWithValue("@d", n);
                    connection.Open();
                    sqlcom.ExecuteNonQuery();
                    //SqlDataReader dr = sqlcom.ExecuteReader();
                    //dr = sqlcom.ExecuteReader();
                    //EventManager.WriteEventInfoMessage("update successful");
                    //while (dr.Read())
                    //{
                    //    d = Convert.ToDateTime(dr["lastUpdate"]);
                    //}
                }
                catch (SqlException ex)
                {
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "faile update last date allTables" + ex.Message + Environment.NewLine);
                    //EventManager.WriteEventInfoMessage("Update date allTables failed" + ex.Message);
                }
                connection.Close();
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after updateLastDate, id: " + pName + Environment.NewLine);
                //return d;
            }
        }








    }
}
