using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Configuration;

/*
namespace DescLogicFramework.DataAccess

{
    public class DBReader : IDisposable
    {
        //Creates a connection class
        SqlConnection conn = new SqlConnection();

        //A cmd class controls the 
        SqlCommand cmd = new SqlCommand();

        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt = new DataTable();

        string query;

        /// <summary>
        /// Sets up a new database connection
        /// </summary>
        public DBReader()
        {
            //set up the database connection
            conn.ConnectionString = @"Data Source=(localdb)\ProjectsV13; Initial Catalog=DATAMINE; Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            conn.ConnectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;


        }

        public void WriteDataViaWorkerThread()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Writes the contents of a data table to the console.
        /// </summary>
        /// <param name="dt"></param>
        public void WriteDataToConsole(DataTable dt)
        {
            StaticClassHoard.PrintDataTableToConsole(dt);
        }

        /// <summary>
        /// Writes data to the database
        /// </summary>
        public void WriteData(DataTable myDataTable, string DestinationTableName)
        {
            using (cmd.Connection = conn) 
            {

            cmd.Connection.Open();
                //Here is an example of bulk coping data from a datatable to a database table
                //The table stuctures (columns, columntypes) must be identical to work properly.
                using (SqlBulkCopy bc = new SqlBulkCopy(conn))
                {
                    bc.DestinationTableName = DestinationTableName;
                    bc.WriteToServer(myDataTable);
                    this.CloseDbConnection();

                    /*
                 //Below is an example of writing to a database via a query string.
                 //This seems the typical way of sending data
                //query = string.Format("INSERT INTO iodp.Results (Id, Myfilename, Mysheetname, Mycolumnname, Result) VALUES({0},'{1}','{2}','{3}','{4}')", dr["ID"], dr["Myfilename"], dr["Mysheetname"], dr["Mycolumnname"], dr["Result"]);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                    

                }

            }
        }

        /// <summary>
        /// Reads data from the database
        /// </summary>
        public DataTable ReadData()
        {
            using (cmd.Connection = conn)
            {
                cmd.Connection.Open();

                query = "SELECT * FROM dbo.measurements";

                cmd.CommandText = query;
                da.SelectCommand = cmd;
                da.Fill(dt);
                return dt;
            }
        }


        public void CloseDbConnection()
        {
            da.Dispose();
            conn.Close();

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                        conn = null;
                    }
                    if (cmd != null)
                    {                  
                      
                        cmd.Dispose();
                        cmd = null;
                    }
                    if (da != null)
                    {
                        da.Dispose();
                        da = null;                        
                    }
                    if (dt != null)
                    {
                        dt.Dispose();
                        dt = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DBReader()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
*/