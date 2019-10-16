using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using MSDASC;
using System.IO;

namespace UpLoadDataService
{
    class ConnectionManager
    {
        private static string m_ConnectionString = string.Empty;
        private static string provider = string.Empty;


        private static void ConenctToDDB()
        {
            try
            {
                if (m_ConnectionString.Length == 0)
                {
                    object _con = null;
                    MSDASC.DataLinks _link = new MSDASC.DataLinks();
                    _con = _link.PromptNew();

                    if (_con != null)
                    {
                        m_ConnectionString = ((ADODB._Connection)_con).ConnectionString;
                    }
                }
                else
                {
                    MSDASC.DataLinks _link = new MSDASC.DataLinks();
                    ADODB.Connection _con = new ADODB.Connection();
                    _con.ConnectionString = m_ConnectionString;
                    object obj = _con;

                    if (_link.PromptEdit(ref obj))
                    {
                        m_ConnectionString = _con.ConnectionString;
                    }
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteEventErrorMessage("SetDataConnection Error", ex);
            }
            finally
            {

            }

        }

        public static string ConnectionString
        {
            get
            {
                ConnectionManager.ConenctToDDB();
                return ConnectionManager.m_ConnectionString;
            }
            set
            {

                ConnectionManager.m_ConnectionString = value;
            }
        }
        public static string Provider
        {
            get
            {

                return ConnectionManager.provider;
            }
            set
            {

                ConnectionManager.provider = value;
            }
        }


        //public static dataset getdatafromhash(string command)
        public static DataSet GetDataForQueries(string command)
        {

            //string connStr = m_ConnectionString;//.Substring(pos, m_ConnectionString.Length -pos - 1 );         
            //string connStr = @"Data Source=PC-500772\SQLEXPRESS;initial catalog=test1;user id=sa;password=1212;multipleactiveresultsets=True;App=EntityFramework&quot;Provider=SQLOLEDB";//in shyam pc, the connection is:@"Data Source=EC2AMAZ-FJGKIU7\SQLEXPRESS;Initial Catalog=NituritNew;Persist Security Info=True;User ID=sa;Password=3Frtgh2; Provider=SQLOLEDB;";


            DataSet dataSet = new DataSet();
            //int pos = m_ConnectionString.IndexOf("DSN");
            string connStr = m_ConnectionString + "Provider=" + provider + ";";
            using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(connStr))
            {
                
                string queryString = command;
                OleDbDataAdapter adapter = new OleDbDataAdapter(queryString, connection);

                connection.Open();
                try
                {
                    //adapter.SelectCommand.CommandTimeout = 999999999;
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before adapter.Fill ::Date : " + DateTime.Now.ToString() + "Query name: " + command + Environment.NewLine);
                    adapter.Fill(dataSet);
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after adapter.Fill" + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Error happened while open connection to db" + Environment.NewLine + "commend: " + queryString + Environment.NewLine + "conString: " + connStr + Environment.NewLine + "conn: " + connection.ToString() + Environment.NewLine + "ex: " + ex + Environment.NewLine);
                    EventManager.WriteEventErrorMessage("Error happened while open connection to db", ex);

                }
                connection.Close();

            }

            return dataSet;
        }

        public static DataSet GetDataForProcedures(string commandName)
        {
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "in , commend: GetDataForProcedures, comm: " + commandName + Environment.NewLine);
            DataSet dataSet = new DataSet();
            //int pos = m_ConnectionString.IndexOf("DSN");
            string connStr = @"" + m_ConnectionString;//.Substring(pos, m_ConnectionString.Length -pos - 1 );
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            connection = null;
            connection = new SqlConnection(connStr);
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after declare connection" + Environment.NewLine);
            connection.Open();
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after open connection" + Environment.NewLine);
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = commandName;
            command.CommandTimeout = 999999999;
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after connection params" + Environment.NewLine);
            adapter = new SqlDataAdapter(command);
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after declare adapter" + Environment.NewLine);
            try
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before adapter.Fill ::Date : " + DateTime.Now.ToString() + "procedure name: " + commandName + Environment.NewLine);
                //adapter.SelectCommand.CommandTimeout = 999999999;
                adapter.Fill(dataSet);
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after adapter.Fill" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "Error happened while open connection to db, commend: " + command + " ex: " + ex + Environment.NewLine);
                EventManager.WriteEventErrorMessage("Error happened while reading data from China", ex);
            }
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "before close connection" + Environment.NewLine);
            connection.Close();
            File.AppendAllText(@"C:\Carmel\logs\logUpload.txt", "after close connection" + Environment.NewLine);

            return dataSet;
        }





        public static DataSet GetDataFromHashODBC(string command)
        {
            string queryString;

            DataSet dataSet = new DataSet();
            int pos = m_ConnectionString.IndexOf("DSN");
            string connStr = m_ConnectionString.Substring(pos, m_ConnectionString.Length - pos - 1);

            using (OdbcConnection cnn = new OdbcConnection(connStr))
            {
                queryString = command;
                OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, cnn);
                try
                {
                    cnn.Open();
                    adapter.Fill(dataSet);
                }
                catch (Exception ex)
                {
                    EventManager.WriteEventErrorMessage("Error happened while reading data from China", ex);
                }
            }
            return dataSet;
        }
    }

}
