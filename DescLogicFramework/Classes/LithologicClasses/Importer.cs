using DescLogicFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DescLogicFramework
{
    public static class Importer
    {

        /// <summary>
        /// Imports a datatable from a csv file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IODPDataTable ImportDataTableFromFile(string filePath)
        {
            var dataTableReader = new CSVReader();
            dataTableReader.ReadPath = filePath;

            using (DataTable dataTable = dataTableReader.Read())
            {
                return new IODPDataTable(dataTable);
            }
        }

        public static IODPDataTable ImportDataTableFromFile(string filePath, SampleHierarchy hierarchy)
        {
            var dataTableReader = new CSVReader();
            dataTableReader.ReadPath = filePath;

            using (DataTable dataTable = dataTableReader.Read())
            {
                return new IODPDataTable(dataTable, hierarchy);
            }
        }

        public static string GetFileName(string filePath)
        {
            return filePath.Split(@"\").Last();
        }

        public static void ExportDataTableAsNewFile(string exportFilePath, DataTable dataTable)
        {
            var exporter = new CSVReader();
            exporter.WritePath = exportFilePath;

            exporter.Write(dataTable);
        }

        public static SampleHierarchy ParseSampleID(string sampleID)
        {
            _ = sampleID ?? throw new ArgumentNullException(nameof(sampleID));

            SampleHierarchy hierarchy = new SampleHierarchy();


            string[] sampleIDComponents = sampleID.Split("-");
            try
            {
                switch (sampleIDComponents.Length)
                {
                    case 0:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = null;
                        hierarchy.Site = null;
                        hierarchy.Hole = null;
                        hierarchy.Core = null;
                        hierarchy.Type = null;
                        hierarchy.Section = null;
                        hierarchy.Half = null;
                        break;
                    case 1:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = sampleIDComponents[0]; //I may want to change in the iteration because I keep getting "No Sample" in the Exedition Column
                        hierarchy.Site = null;
                        hierarchy.Hole = null;
                        hierarchy.Core = null;
                        hierarchy.Type = null;
                        hierarchy.Section = null;
                        hierarchy.Half = null;
                        break;
                    case 2:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = sampleIDComponents[0];
                        hierarchy.Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        hierarchy.Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        hierarchy.Core = null;
                        hierarchy.Type = null;
                        hierarchy.Section = null;
                        hierarchy.Half = null;
                        break;
                    case 3:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = sampleIDComponents[0];
                        hierarchy.Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        hierarchy.Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        hierarchy.Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        hierarchy.Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        hierarchy.Section = null;
                        hierarchy.Half = null;
                        break;
                    case 4:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = sampleIDComponents[0];
                        hierarchy.Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        hierarchy.Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        hierarchy.Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        hierarchy.Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        hierarchy.Section = sampleIDComponents[3];
                        hierarchy.Half = null;
                        break;
                    case 5:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = sampleIDComponents[0];
                        hierarchy.Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        hierarchy.Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        hierarchy.Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        hierarchy.Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        hierarchy.Section = sampleIDComponents[3];
                        hierarchy.Half = CleanUpSectionHalf(sampleIDComponents[4].Split(" ")[0]);
                        break;
                    default:
                        hierarchy.SampleID = sampleID;
                        hierarchy.Expedition = sampleIDComponents[0];
                        hierarchy.Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        hierarchy.Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        hierarchy.Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        hierarchy.Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        hierarchy.Section = sampleIDComponents[3];
                        hierarchy.Half = CleanUpSectionHalf(sampleIDComponents[4].Split(" ")[0]);
                        break;

                }
                return hierarchy;
            }
            catch (Exception)
            {

                throw new Exception("Error Parsing SampleID");
            }

        }
        private static string CleanUpSectionHalf(string sectionHalf)
        {

            if (sectionHalf.Contains("PAL"))
            {
                return "PAL";
            }
            else
            {
                return sectionHalf;
            }
        }


        public static void CheckFile(IODPDataTable IODPDataTable)
        {

            int rowNumber = 1;

            foreach (DataRow row in IODPDataTable.DataTable.Rows)
            {


                if (!DataRowContainsDescription(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain description", rowNumber));
                }

                if (!DataRowContainsSampleIDColumn(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain SampleID column", rowNumber));
                }

                if (!DataRowContainsOffsetColumns(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Data row does not contain Offset columns", rowNumber));
                }

                if (!StartOffsetValuesAreValid(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain correct Start Offset values", rowNumber));
                }

                if (!EndOffsetValuesAreValid(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain correct End Offset values", rowNumber));
                }

                LithologicDescription Description = new LithologicDescription();


                try
                {
                    Description = new LithologicDescription(row[IODPDataTable.SampleIDColumn].ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Row {0}: Could not parse SampleID. Error Message: {1}", rowNumber, ex.Message));
                }


                if (!DescriptionContainsSectionInfo(Description))
                {
                    throw new Exception(string.Format("Row {0}: Description does not contain section information", rowNumber));
                }

                rowNumber++;

            }
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

        public static bool DataRowContainsOffsetColumns(DataRow dataTableRow, IODPDataTable dataTable)
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
