using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DescLogicFramework;
using DescLogicFramework.DataAccess;

namespace DescLogicFrameworkTests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void ReadDataFromDatabase()
        {
            using (var db = new DBReader())
            {
                DataTable Results = db.ReadData();
                db.WriteDataToConsole(Results);
                db.CloseDbConnection();
            }
        }

        [TestMethod]
        [Ignore]
        public void WriteDataToDatabase()
        {
            using (DataTable myDataTable = new DataTable())
            {
                myDataTable.Columns.Add("ID", typeof(int));
                myDataTable.Columns.Add("Myfilename", typeof(string));
                myDataTable.Columns.Add("Mysheetname", typeof(string));
                myDataTable.Columns.Add("Mycolumnname", typeof(string));
                myDataTable.Columns.Add("Result", typeof(string));

                for (int i = 0; i < 10; i++)
                {
                    DataRow dr = myDataTable.NewRow();
                    dr["ID"] = 2000 + i;
                    dr["Myfilename"] = "DerpingFromVS";
                    dr["Mysheetname"] = "DerpingFromVS";
                    dr["Mycolumnname"] = "DerpingFromVS";
                    dr["Result"] = "DerpingFromVS";

                    myDataTable.Rows.Add(dr);
                }
            }
        }
    }
}

