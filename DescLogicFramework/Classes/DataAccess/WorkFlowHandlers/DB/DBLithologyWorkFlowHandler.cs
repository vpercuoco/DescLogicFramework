using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

/*
namespace DescLogicFramework.DataAccess
{
    class DBLithologyWorkFlowHandler
    {

        public DBLithologyWorkFlowHandler()
        {

        }

        public DataTable ConvertToDatabaseSchema(Cache<string, LithologicDescription> Descriptions)
        {

            DataTable DataTableToSendToDatabase = new DataTable();

            DataTableToSendToDatabase.Columns.Add("description_key", typeof(long));
            DataTableToSendToDatabase.Columns.Add("description_column_name", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_value", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_value_adjusted", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_column_name_adjusted", typeof(string));
            DataTableToSendToDatabase.Columns.Add("lithologicID", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_report", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_tab", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_group", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_type", typeof(string));
            DataTableToSendToDatabase.Columns.Add("lithologic_subID", typeof(string));
            DataTableToSendToDatabase.Columns.Add("description_record", typeof(long));



           long i = 1;
            foreach (KeyValuePair<string,LithologicDescription> ld in Descriptions.GetCollection())
            {
                List<string> ColumnNames = ld.Value.DataRow.Table.Columns.Cast<DataColumn>()
                   .Select(c => c.ColumnName)
                   .ToList();

                //A primary key column
                ColumnNames.Remove("Lithologic_ID");

                foreach (string dc in ColumnNames)
                {
                    DataRow row = DataTableToSendToDatabase.NewRow();

                   
                    row["description_column_name"] = dc;
                    row["description_value"] = ld.Value.DataRow[dc];
                    row["description_value_adjusted"] = "";
                    row["description_column_name_adjusted"] = "";
                    row["lithologicID"] = ld.Value.DataRow["LithologicID_VP"];
                    row["description_report"] = ld.Value.DescriptionReport;
                    row["description_tab"] = ld.Value.DescriptionTab;
                    row["description_group"] = ld.Value.DescriptionGroup;
                    row["description_type"] = ld.Value.DescriptionType;
                    row["lithologic_subID"] = i.ToString();
                    row["description_record"] = i;

                    DataTableToSendToDatabase.Rows.Add(row);
                   
                  
                }
                i++;
            }
            i = 1;

            return DataTableToSendToDatabase;

        }

        public bool SendDataTableToDatabase(Cache<string, LithologicDescription> LithologyCache)
        {
            DataTable dt = this.ConvertToDatabaseSchema(LithologyCache);
            DBReader dbConnector = new DBReader();
            try
            {
                dbConnector.WriteData(dt, "DATAMINE.dbo.descriptions");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
*/