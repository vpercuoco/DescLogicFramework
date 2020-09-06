using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace DescLogicFramework
{
    /// <summary>
    /// Class used to create LithologicDescriptions objects from a DataTable.
    /// </summary>
    public class LithologyConvertor
    {

        /// <summary>
        /// Converts an IODPDataTable object into a collection of Lithologic Descriptions
        /// </summary>
        /// <param name="dataTable">The datatable to convert</param>
        /// <returns></returns>
        public Cache<string, LithologicDescription> Convert(IODPDataTable dataTable, ref SectionInfoCollection SectionCollection)
        {
            var LithologyCache = new Cache<string, LithologicDescription>();
          
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

                LithologicDescription description = new LithologicDescription(dataTableRow[dataTable.SampleID].ToString());

                #region GlobalSectionList
                description.SectionInfo = SectionCollection.GetExistingElseAddAndGetCurrentSection(description.SectionInfo);
                description.StartOffset.SectionInfo = description.SectionInfo;
                description.EndOffset.SectionInfo = description.SectionInfo;
                #endregion

                if (!DescriptionContainsSectionInfo(description))
                    return LithologyCache;

                description.DataRow = dataTableRow;

                double parsedOffset = 0;

                if (!DataRowContainsOffsetColumns(dataTableRow, dataTable))
                    return LithologyCache;

                if (!StartOffsetValuesAreValid(dataTableRow, dataTable, ref parsedOffset))
                    return LithologyCache;

                description.StartOffset.Offset = parsedOffset;

                if (!EndOffsetValuesAreValid(dataTableRow, dataTable, ref parsedOffset))
                    return LithologyCache;

                description.EndOffset.Offset = parsedOffset;

                LithologicIDGenerator IDGenerator = new LithologicIDGenerator();
                IDGenerator.GenerateID(description);

                if (description.OffsetsSet())
                {
                    description.GenerateSubintervals();
                }

                description.DataRow["LithologicID_VP"] = description.LithologicID;
                LithologyCache.Add(description.LithologicID, description);
            }

            return LithologyCache;

        }

        private bool StartOffsetValuesAreValid(DataRow dataTableRow, IODPDataTable dataTable, ref double parsedOffset)
        {

            if (double.TryParse(dataTableRow[dataTable.OffsetIntervals[0]].ToString(), out parsedOffset))
            {
                return true;
            }
            else
            {
                Console.WriteLine("There are erroneous top offset values in this file");
                return false;
            }
        }

        private bool EndOffsetValuesAreValid(DataRow dataTableRow, IODPDataTable dataTable, ref double parsedOffset)
        {

            if (double.TryParse(dataTableRow[dataTable.OffsetIntervals[1]].ToString(), out parsedOffset))
            {
                return true;
            }
            else
            {
                Console.WriteLine("There are erroneous bottom offset values in this file");
                return false;
            }
        }

        private bool DataRowContainsOffsetColumns(DataRow dataTableRow, IODPDataTable dataTable)
        {
            if (dataTableRow.Table.Columns.Contains(dataTable.OffsetIntervals[0]) && dataTableRow.Table.Columns.Contains(dataTable.OffsetIntervals[1]))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Could not identify an offset column in this file");
                return false;
            }
        }

        private bool DescriptionContainsSectionInfo(LithologicDescription description)
        {
            //need to determine if the file is a Hole Summary, which means that there is no section identifier in the Sample Data field.
            if (description.SectionInfo.Section == null)
            {
                Console.WriteLine("Could not identify the section numbers in this file");
                return false;
            }
            return true;
        }

        private bool DataRowContainsDescription(DataRow dataTableRow, IODPDataTable dataTable)
        {
            //Reject files with no datarows
            CinnamonList noDataEntriesList = new CinnamonList("NoSampleEntries");

            if (noDataEntriesList.FindInList(dataTableRow[dataTable.SampleID].ToString().ToLower()))
            {
                Console.WriteLine("There are no description entries in this description file");
                return false;
            }

            return true;
        }

        private bool DataRowContainsSampleIDColumn(DataRow dataTableRow, IODPDataTable dataTable)
        {

            if (dataTableRow.Table.Columns.Contains(dataTable.SampleID))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Could not identify the Sample ID column in this file");
                return false;
            }
        }
    }
}
