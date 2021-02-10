using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using Serilog;

namespace DescLogicFramework
{
    /// <summary>
    /// Class used to create LithologicDescriptions objects from a DataTable.
    /// </summary>
    public static class LithologyConvertor
    {

        /// <summary>
        /// Converts an IODPDataTable object into a collection of Lithologic Descriptions
        /// </summary>
        /// <param name="dataTable">The datatable to convert</param>
        /// <returns></returns>
        public static Dictionary<string, LithologicDescription> ConvertDatatableToDictionary(IODPDataTable dataTable, SectionInfoCollection SectionCollection)
        {
            _ = SectionCollection ?? throw new ArgumentNullException(nameof(SectionCollection));
            

            var LithologyCache = new Dictionary<string, LithologicDescription>();
            

            if (dataTable == null)
            {
                return LithologyCache;
            }
            //Add a column in the datatable to ensure consistency between files with and without descriptions:
            dataTable.DataTable.Columns.Add("LithologicID_VP", typeof(string)).SetOrdinal(0);


            foreach (DataRow dataTableRow in dataTable.DataTable.Rows)
            {
                dataTableRow["LithologicID_VP"] = "-1";

                if (!DataRowContainsDescription(dataTableRow, dataTable))
                    return LithologyCache;

                if (!DataRowContainsSampleIDColumn(dataTableRow, dataTable))
                    return LithologyCache;

                LithologicDescription description = new LithologicDescription(dataTableRow[dataTable.SampleIDColumn].ToString());

                description.SectionInfo = SectionCollection.GetExistingElseAddAndGetCurrentSection(description.SectionInfo);

                if (!DescriptionContainsSectionInfo(description))
                    return LithologyCache;

                description.DataRow = dataTableRow;

                double parsedOffset = 0;

                if (!DataRowContainsOffsetColumns(dataTableRow, dataTable))
                    return LithologyCache;

                if (!StartOffsetValuesAreValid(dataTableRow, dataTable, ref parsedOffset))
                    return LithologyCache;

                description.StartOffset = parsedOffset;

                if (!EndOffsetValuesAreValid(dataTableRow, dataTable, ref parsedOffset))
                    return LithologyCache;

                description.EndOffset = parsedOffset;

                LithologicIDGenerator IDGenerator = new LithologicIDGenerator();
                IDGenerator.GenerateID(description);

                if (description.OffsetsSet())
                {
                    description.GenerateSubintervals();
                }

                description.DataRow["LithologicID_VP"] = description.LithologicID;

                //Some descriptions are split in two rows. It's very uncommon, but throws an error
                //Only selecting the first row, despite the loss of data
                if (!LithologyCache.ContainsKey(description.LithologicID))
                {
                    LithologyCache.Add(description.LithologicID, description);
                }
                
            }

            return LithologyCache;

        }

        public static bool StartOffsetValuesAreValid(DataRow dataTableRow, IODPDataTable dataTable, ref double parsedOffset)
        {

            return double.TryParse(dataTableRow[dataTable.TopOffsetColumn].ToString(), out parsedOffset);
        
        }

        public static bool StartOffsetValuesAreValid(DataRow dataTableRow, IODPDataTable dataTable)
        {
 
            return double.TryParse(dataTableRow[dataTable.TopOffsetColumn].ToString(), out double parsedOffset);
        }

        public static bool EndOffsetValuesAreValid(DataRow dataTableRow, IODPDataTable dataTable, ref double parsedOffset)
        {

            return double.TryParse(dataTableRow[dataTable.BottomOffsetColumn].ToString(), out parsedOffset);

        }
        public static bool EndOffsetValuesAreValid(DataRow dataTableRow, IODPDataTable dataTable)
        {
        
            return double.TryParse(dataTableRow[dataTable.BottomOffsetColumn].ToString(), out double parsedOffset);

        }

        public  static bool DataRowContainsOffsetColumns(DataRow dataTableRow, IODPDataTable dataTable)
        {
            return dataTableRow.Table.Columns.Contains(dataTable.TopOffsetColumn) && dataTableRow.Table.Columns.Contains(dataTable.BottomOffsetColumn);

        }

        public static bool DescriptionContainsSectionInfo(LithologicDescription description)
        {
            return description.SectionInfo.Section != null ? true : false;

        }

        public static bool DataRowContainsDescription(DataRow dataTableRow, IODPDataTable dataTable)
        {
   
            CinnamonList noDataEntriesList = new CinnamonList("NoSampleEntries");

            return !noDataEntriesList.FindInList(dataTableRow[dataTable.SampleIDColumn].ToString().ToLower()) ? true : false;
 
        }

        public static bool DataRowContainsSampleIDColumn(DataRow dataTableRow, IODPDataTable dataTable)
        {

            return dataTableRow.Table.Columns.Contains(dataTable.SampleIDColumn);
        }
    }
}
