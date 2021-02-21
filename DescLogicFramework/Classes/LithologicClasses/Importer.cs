using DescLogicFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static IODPDataTable ImportDataTableFromFile(string filePath, IntervalHierarchyNames hierarchy)
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

        public static string GetFileNameWithoutExtension(string filePath)
        {
            return GetFileName(filePath).Split(".").First();
        }

        public static void ExportDataTableAsNewFile(string exportFilePath, DataTable dataTable)
        {
            var exporter = new CSVReader();
            exporter.WritePath = exportFilePath;

            exporter.Write(dataTable);
        }

        public static IntervalHierarchyValues GetHierarchyValuesFromDataRow(DataRow dataRow, IntervalHierarchyNames columnNames)
        {
            IntervalHierarchyValues result = new IntervalHierarchyValues();

            try
            {
                result.SampleID = DataRowContainsColumn(columnNames.SampleID, dataRow) ? dataRow[columnNames.SampleID].ToString() : result.SampleID;
                result.Expedition = DataRowContainsColumn(columnNames.Expedition, dataRow) ? dataRow[columnNames.Expedition].ToString() : result.Expedition;
                result.Site = DataRowContainsColumn(columnNames.Site, dataRow) ? dataRow[columnNames.Site].ToString() : result.Site;
                result.Hole = DataRowContainsColumn(columnNames.Hole, dataRow) ? dataRow[columnNames.Hole].ToString() : result.Hole;
                result.Core = DataRowContainsColumn(columnNames.Core, dataRow) ? dataRow[columnNames.Core].ToString() : result.Core;
                result.Type = DataRowContainsColumn(columnNames.Type, dataRow) ? dataRow[columnNames.Type].ToString() : result.Type;
                result.Section = DataRowContainsColumn(columnNames.Section, dataRow) ? dataRow[columnNames.Section].ToString() : result.Section;
                result.Half = DataRowContainsColumn(columnNames.Half, dataRow) ? dataRow[columnNames.Half].ToString() : result.Half;
                result.Offset = DataRowContainsColumn(columnNames.Offset, dataRow) ? dataRow[columnNames.Offset].ToString() : result.Offset;
                result.TopOffset = DataRowContainsColumn(columnNames.TopOffset, dataRow) ? dataRow[columnNames.TopOffset].ToString() : result.TopOffset;
                result.BottomOffset = DataRowContainsColumn(columnNames.BottomOffset, dataRow) ? dataRow[columnNames.BottomOffset].ToString() : result.BottomOffset;
                result.ArchiveTextID = DataRowContainsColumn(columnNames.ArchiveTextID, dataRow) ? dataRow[columnNames.ArchiveTextID].ToString() : result.ArchiveTextID;
                result.WorkingTextID = DataRowContainsColumn(columnNames.WorkingTextID, dataRow) ? dataRow[columnNames.WorkingTextID].ToString() : result.WorkingTextID;
                result.ParentTextID = DataRowContainsColumn(columnNames.ParentTextID, dataRow) ? dataRow[columnNames.ParentTextID].ToString() : result.ParentTextID;
                result.TextID = DataRowContainsColumn(columnNames.TextID, dataRow) ? dataRow[columnNames.TextID].ToString() : result.TextID;
                result.TestNumber = DataRowContainsColumn(columnNames.TestNumber, dataRow) ? dataRow[columnNames.TestNumber].ToString() : result.TestNumber;
                return result;
            }
            catch (Exception)
            {

                throw new Exception("Error trying to get hierarchy values from data row");
            }
        }

        public static bool DataRowContainsColumn(string columnName, DataRow dataRow)
        {
           return dataRow.Table.Columns.Contains(columnName);
        }

        public static IntervalHierarchyValues ParseSampleID(string sampleID)
        {
            _ = sampleID ?? throw new ArgumentNullException(nameof(sampleID));

            IntervalHierarchyValues hierarchy = new IntervalHierarchyValues();


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

        #region FileChecks

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

        #endregion

        /// <summary>
        /// Takes in a Datatable and prints all the headers, rows to the console
        /// </summary>
        /// <param name="dataTable"></param>
        public static void PrintDataTableToConsole(DataTable dataTable)
        {
            string output = " ";
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                output = output + " " + dataColumn.ColumnName;
            }

            foreach (DataRow dataRow in dataTable.Rows)
            {
                output = " ";
                foreach (var item in dataRow.ItemArray)
                {
                    output = output + " " + item.ToString();
                }
                Console.WriteLine(output);
            }
        }
    }
}
