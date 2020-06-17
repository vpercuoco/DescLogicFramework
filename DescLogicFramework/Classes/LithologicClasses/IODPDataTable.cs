using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace DescLogicFramework
{
    /// <summary>
    /// A DataTable object with column names assigned by the standard Section hierarchy.
    /// </summary>
    public class IODPDataTable 
    {
        //These properties are the column headers
        public string Expedition { get; set; } = "Exp";
        public string Site { get; set; } = "Site";
        public string Hole { get; set; } = "Hole";
        public string Core { get; set; } = "Core";
        public string Type { get; set; } = "Type";
        public string Section { get; set; } = "Sect";
        public string Offset { get; set; } = string.Empty;
        public string[] OffsetIntervals { get; set; } = { string.Empty, string.Empty };

        public string SampleID { get; set; } = "Sample";
        public DataTable DataTable { get; set; }

        /// <summary>
        /// Creates a new IODPDataTable object
        /// </summary>
        /// <param name="dt">A DataTable object</param>
        public IODPDataTable(DataTable dt)
        {
            DataTable = dt;

            //Find the correct column headers in the datatable and assign to properties
            List<string> columnNames = getColumnNames(dt);
            string match = null;

            CinnamonList lookupList = new CinnamonList("Expedition");
            match = lookupList.FindInList(columnNames);
            Expedition = string.IsNullOrEmpty(match)? Expedition : match;
            
            lookupList = new CinnamonList("Site");
            match = lookupList.FindInList(columnNames);
            Site = string.IsNullOrEmpty(match) ? Site : match;

            lookupList = new CinnamonList("Hole");
            match = lookupList.FindInList(columnNames);
            Hole = string.IsNullOrEmpty(match) ? Hole : match;

            lookupList = new CinnamonList("Core");
            match = lookupList.FindInList(columnNames);
            Core = string.IsNullOrEmpty(match) ? Core : match;

            lookupList = new CinnamonList("Type");
            match = lookupList.FindInList(columnNames);
            Type = string.IsNullOrEmpty(match) ? Type : match;

            lookupList = new CinnamonList("Section");
            match = lookupList.FindInList(columnNames);
            Section = string.IsNullOrEmpty(match) ? Section : match;

            lookupList = new CinnamonList("Offset");
            match = lookupList.FindInList(columnNames);
            Offset = string.IsNullOrEmpty(match) ? Offset : match;

            lookupList = new CinnamonList("TopOffset");
            match = lookupList.FindInList(columnNames);
            OffsetIntervals[0] = string.IsNullOrEmpty(match) ? OffsetIntervals[0] : match;

            lookupList = new CinnamonList("BottomOffset");
            match = lookupList.FindInList(columnNames);
            OffsetIntervals[1] = string.IsNullOrEmpty(match) ? OffsetIntervals[1] : match;

            lookupList = new CinnamonList("SampleID");
            match = lookupList.FindInList(columnNames);
            SampleID = string.IsNullOrEmpty(match) ? SampleID : match;

        }

        /// <summary>
        /// Returns a list of the column names of a Datatable
        /// </summary>
        /// <param name="dt">The DataTable in which to search</param>
        /// <returns>A List of strings</returns>
        public List<string> getColumnNames(DataTable dt)
        {
            List<string> columnNames = new List<string>();
            foreach(DataColumn dc in dt.Columns)
            {
                columnNames.Add(dc.ColumnName);
            }
            return columnNames;
        }

     /*   /// <summary>
        /// Adds a DataColumn to the current DataTable
        /// </summary>
        /// <param name="dataColumn"></param>
        /// <returns></returns>
        public DataTable AddColumn(string columnName)
        {

            if (!this.DataTable.Columns.Contains(columnName))
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = columnName;
                dc.AllowDBNull = true;
                dc.DefaultValue = -1;
            }
            else
            {
                Console.WriteLine("Datatable already contains datacolumn");
              
            }
            return this.DataTable;
        }
        */
    }
}
