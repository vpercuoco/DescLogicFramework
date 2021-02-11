using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using CsvHelper;
using System.Globalization;

namespace DescLogicFramework.DataAccess
{

    /// <summary>
    /// Used to convert a csv file into a DataTable.
    /// </summary>
    public class CSVReader : IDataTableHandler
    {

        public string ReadPath { get; set; }
        public string WritePath { get; set; }

        public DataTable Read()
        {
            using (DataTable dt = new DataTable())
            {
                using (StreamReader reader = new StreamReader(ReadPath))
                {
                    using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        using (var dr = new CsvDataReader(csvReader))
                        {
                            dt.Load(dr);
                        }
                    }
                    //reader.Close();
                }
                return dt;
            }
        }

        public void Write(DataTable dt)
        {
            using (StreamWriter writer = new StreamWriter(WritePath))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    //Write headers
                    foreach (DataColumn column in dt.Columns)
                    {
                        csvWriter.WriteField(column.ColumnName);
                    }
                    csvWriter.NextRecord();

                    //Write data
                    foreach (DataRow dr in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            csvWriter.WriteField(dr[i]);
                        }
                        csvWriter.NextRecord();
                    }
                }
                
               // writer.Close();
            }
        }


        /// <summary>
        /// Pivots a DataTable object into Column/Attribute pairs.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static DataTable PivotDataTable(DataTable dataTable)
        {
            DataTable pivotTable = new DataTable();
            pivotTable.Columns.Add("Column", typeof(string));
            pivotTable.Columns.Add("Attribute", typeof(string));
            foreach (DataRow dataRow in dataTable.Rows)
            {  
                for (int columnNumber = 0; columnNumber < dataTable.Columns.Count; columnNumber++)
                {
                    DataRow newRow = pivotTable.NewRow();
                    newRow["Column"] = dataTable.Columns[columnNumber];
                    newRow["Attribute"] = dataRow[columnNumber];
                    pivotTable.Rows.Add(newRow);
                }
            }
            return pivotTable;
        }

    }
}
