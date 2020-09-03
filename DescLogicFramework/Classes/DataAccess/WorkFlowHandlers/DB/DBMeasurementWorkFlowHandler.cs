using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;


namespace DescLogicFramework.DataAccess
{
    class DBMeasurementWorkFlowHandler
    {
        public DBMeasurementWorkFlowHandler()
        {

        }
        public DataTable ConvertToDatabaseSchema(Cache<int, Measurement> Measurements)
        {

            DataTable DataTableToSendToDatabase = new DataTable();

            DataTableToSendToDatabase.Columns.Add("measurement_key", typeof(long));
            DataTableToSendToDatabase.Columns.Add("instrument_system", typeof(string));
            DataTableToSendToDatabase.Columns.Add("measurement_column_name", typeof(string));
            DataTableToSendToDatabase.Columns.Add("measurement_column_value", typeof(string));
            DataTableToSendToDatabase.Columns.Add("instrument_report", typeof(string));
            DataTableToSendToDatabase.Columns.Add("report_record", typeof(long));
            DataTableToSendToDatabase.Columns.Add("lithologicID", typeof(string));
            DataTableToSendToDatabase.Columns.Add("lithologic_subID", typeof(string));

            long i = 1;
            foreach (KeyValuePair<int, Measurement> measurement in Measurements.GetCollection())
            {
                List<string> ColumnNames = measurement.Value.DataRow.Table.Columns.Cast<DataColumn>()
                   .Select(c => c.ColumnName)
                   .ToList();

                //A primary key column
                ColumnNames.Remove("LithologicID_VP");
                ColumnNames.Remove("LithologicSubID_VP");

                foreach (string dc in ColumnNames)
                {
                    DataRow row = DataTableToSendToDatabase.NewRow();


                    // row["measurement_key"] is set by the database constraints
                    row["instrument_system"] = measurement.Value.InstrumentSystem;
                    row["measurement_column_name"] = dc;
                    row["measurement_column_value"] = measurement.Value.DataRow[dc];
                    row["instrument_report"] = measurement.Value.InstrumentReport;
                    row["report_record"] = i;
                    row["lithologicID"] = measurement.Value.DataRow["LithologicID_VP"];
                    row["lithologic_subID"] = measurement.Value.DataRow["LithologicSubID_VP"];

                    DataTableToSendToDatabase.Rows.Add(row);


                }
                i++;
            }
            i = 1;

            return DataTableToSendToDatabase;

        }

        public bool SendDataTableToDatabase(Cache<int, Measurement> MeasurementCache)
        {


            DataTable dt = this.ConvertToDatabaseSchema(MeasurementCache);
            //DBReader dbConnector = new DBReader();
            try
            {
               // dbConnector.WriteData(dt, "DATAMINE.dbo.measurements");
                return true;
            }
            catch (Exception)
            {

                return false;
            }


        }


    }
}
