using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace DescLogicFramework
{
    /// <summary>
    /// A DataTable object with column names assigned by the standard Section hierarchy.
    /// </summary>
    public class IODPDataTable 
    {
        public string ExpeditionColumn { get; set; } = "Exp";
        public string SiteColumn { get; set; } = "Site";
        public string HoleColumn { get; set; } = "Hole";
        public string CoreColumn { get; set; } = "Core";
        public string TypeColumn { get; set; } = "Type";
        public string SectionColumn { get; set; } = "Sect";
        public string OffsetColumn { get; set; } = string.Empty;
        public string TopOffsetColumn { get; set; } = string.Empty;
        public string BottomOffsetColumn { get; set; } = string.Empty;
        public string SampleIDColumn { get; set; } = "Sample";
        public DataTable DataTable { get; set; }

        /// <summary>
        /// Creates a new IODPDataTable object
        /// </summary>
        public IODPDataTable(DataTable dataTable)
        {
            DataTable = dataTable;

            List<string> columnNames = GetColumnNames(dataTable);
            string match = string.Empty;

            CinnamonList lookupList = new CinnamonList("Expedition");
            match = lookupList.FindInList(columnNames);
            ExpeditionColumn = string.IsNullOrEmpty(match)? ExpeditionColumn : match;
            
            lookupList = new CinnamonList("Site");
            match = lookupList.FindInList(columnNames);
            SiteColumn = string.IsNullOrEmpty(match) ? SiteColumn : match;

            lookupList = new CinnamonList("Hole");
            match = lookupList.FindInList(columnNames);
            HoleColumn = string.IsNullOrEmpty(match) ? HoleColumn : match;

            lookupList = new CinnamonList("Core");
            match = lookupList.FindInList(columnNames);
            CoreColumn = string.IsNullOrEmpty(match) ? CoreColumn : match;

            lookupList = new CinnamonList("Type");
            match = lookupList.FindInList(columnNames);
            TypeColumn = string.IsNullOrEmpty(match) ? TypeColumn : match;

            lookupList = new CinnamonList("Section");
            match = lookupList.FindInList(columnNames);
            SectionColumn = string.IsNullOrEmpty(match) ? SectionColumn : match;

            lookupList = new CinnamonList("Offset");
            match = lookupList.FindInList(columnNames);
            OffsetColumn = string.IsNullOrEmpty(match) ? OffsetColumn : match;

            lookupList = new CinnamonList("TopOffset");
            match = lookupList.FindInList(columnNames);
            TopOffsetColumn = string.IsNullOrEmpty(match) ? TopOffsetColumn : match;

            lookupList = new CinnamonList("BottomOffset");
            match = lookupList.FindInList(columnNames);
            BottomOffsetColumn = string.IsNullOrEmpty(match) ? BottomOffsetColumn : match;

            lookupList = new CinnamonList("SampleID");
            match = lookupList.FindInList(columnNames);
            SampleIDColumn = string.IsNullOrEmpty(match) ? SampleIDColumn : match;

        }

        public IODPDataTable(DataTable dataTable, SampleHierarchy hierarchy)
        {
            DataTable = dataTable;

            ExpeditionColumn = hierarchy.Expedition;

            SiteColumn = hierarchy.Site;

            HoleColumn = hierarchy.Hole;

            CoreColumn = hierarchy.Core;

            TypeColumn = hierarchy.Type;

            SectionColumn = hierarchy.Section;

            TopOffsetColumn = hierarchy.TopOffset;

            BottomOffsetColumn = hierarchy.BottomOffset;

            SampleIDColumn = hierarchy.SampleID;
        }

        /// <summary>
        /// Returns a list of the column names of a Datatable
        /// </summary>
        private static List<string> GetColumnNames(DataTable dataTable)
        {
            List<string> columnNames = new List<string>();

            foreach(DataColumn dataColumn in dataTable.Columns)
            {
                columnNames.Add(dataColumn.ColumnName);
            }
            return columnNames;
        }
    }
}
