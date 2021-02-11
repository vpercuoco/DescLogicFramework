using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DescLogicFramework.DataAccess;
using Serilog;
using System.Linq;
using System.IO;
using System.Configuration;

namespace DescLogicFramework
{

    //TODO: Output warnings from the parsing into a LOG file that I can consult when dealing with error files.


    public class DescriptionFileCleaner
    {

        private static string CurrentFileName = "";

        private static SectionInfoCollection SectionInfoCollection = new SectionInfoCollection();

        FileCollection FileCollection = new FileCollection();
        public DescriptionFileCleaner(string directory, string expedition)
        {
            FileCollection.AddFiles(directory, "*.csv");

            foreach (string file in FileCollection.Filenames)
            {
                try
                {
                    
                    CurrentFileName = GetFileName(file);
                    CleanupDescriptionFile(file, expedition);
                    
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Format("{0}: Could not clean file", CurrentFileName));
                    Log.Warning(string.Format("{0}: {1}", CurrentFileName, ex.Message));
                    Log.Warning(string.Format("{0}: Stack Trace: {1}", CurrentFileName, ex.StackTrace));
                }
            }
           

        }

        public void CleanupDescriptionFile(string filePath, string expedition)
        {

            var iodpDataTable = ImportDataTableFromFile(filePath);

            string exportFilePath =  Directory.CreateDirectory(ConfigurationManager.AppSettings["ExportDirectory"] + expedition + @"\").FullName + GetFileName(filePath);

            try
            {
                CheckFile(iodpDataTable);
                AddMissingColumns(iodpDataTable.DataTable);
                AddDataToHierarchyColumns(iodpDataTable, GetFileName(filePath), SectionInfoCollection.SectionsDatatable);
                Log.Information(string.Format("{0}: Processed successfully", CurrentFileName));
            }
           catch (Exception ex)
            {
                Log.Warning(string.Format("{0}: {1}", CurrentFileName, ex.Message));
               
                exportFilePath = Directory.CreateDirectory(ConfigurationManager.AppSettings["ErrorExportDirectory"] + expedition+ @"\").FullName + GetFileName(filePath);
            }


            try
            {
                ExportDataTableAsNewFile(exportFilePath, iodpDataTable.DataTable);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}: Error exporting to file",CurrentFileName));
            }
            

        }

        /// <summary>
        /// Imports a datatable from a csv file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IODPDataTable ImportDataTableFromFile(string filePath) 
        {
            var dataTableReader = new CSVReader();
            dataTableReader.ReadPath = filePath;

            using (DataTable dataTable = dataTableReader.Read() )
            {
                return new IODPDataTable(dataTable);
            }
        }

        public string GetFileName(string filePath)
        {
            return filePath.Split(@"\").Last();
        }
        public void CheckFile(IODPDataTable IODPDataTable) 
        {

            int rowNumber = 1;
            
            foreach (DataRow row in IODPDataTable.DataTable.Rows)
            {
                

                if (!LithologyConvertor.DataRowContainsDescription(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain description", rowNumber));
                }

                if (!LithologyConvertor.DataRowContainsSampleIDColumn(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain SampleID column", rowNumber));
                }

                if (!LithologyConvertor.DataRowContainsOffsetColumns(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Data row does not contain Offset columns", rowNumber));
                }

                if (!LithologyConvertor.StartOffsetValuesAreValid(row, IODPDataTable))
                {
                    throw new Exception(string.Format("Row {0}: Does not contain correct Start Offset values", rowNumber));
                }

                if (!LithologyConvertor.EndOffsetValuesAreValid(row, IODPDataTable))
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
                    throw new Exception(string.Format("Row {0}: Could not parse SampleID. Error Message: {1}",rowNumber, ex.Message));
                }
  

                if (!LithologyConvertor.DescriptionContainsSectionInfo(Description))
                {
                    throw new Exception(string.Format("Row {0}: Description does not contain section information", rowNumber));
                }

                rowNumber++;
                
            }    
        }

        /// <summary>
        /// Add necessary hierarchy columns
        /// </summary>
        /// <param name="dataTable"></param>
        public void AddMissingColumns(DataTable dataTable)
        {
         
            string[] columnNames = new string[] { "Filename_VP", "LithologicID_VP", "ArchiveSectionTextID_VP", "WorkingSectionTextID_VP", "ParentSectionTextID_VP", "Expedition_VP", "Site_VP", "Hole_VP", "Core_VP", "Type_VP", "Section_VP", "SectionHalf_VP", "TopOffset_VP", "BottomOffset_VP" };

            foreach (string name in columnNames.Reverse())
            {
                if (!dataTable.Columns.Contains(name))
                {
                    dataTable.Columns.Add(name, typeof(string)).SetOrdinal(0);
                }
                
            }

        }

        public (string Archive, string Working, string Parent) GetTextID(DataTable allSections, LithologicDescription description)
        {
         
                var matchingTextID = allSections.AsEnumerable().Where(x => x.Field<string>("Exp") == description.SectionInfo.Expedition
                                                       && x.Field<string>("Site") == description.SectionInfo.Site
                                                       && x.Field<string>("Hole") == description.SectionInfo.Hole
                                                       && x.Field<string>("Core") == description.SectionInfo.Core
                                                       && x.Field<string>("Type") == description.SectionInfo.Type
                                                       && x.Field<string>("Sect") == description.SectionInfo.Section).FirstOrDefault();


                if (matchingTextID == null)
                {
                    return (Archive: "-1", Working: "-1", Parent: "-1");
                }

                (string Archive, string Working, string Parent) textids = (matchingTextID["Text ID of archive half"].ToString(), matchingTextID["Text ID of working half"].ToString(), matchingTextID["Text ID of section"].ToString());
                return textids;

        }
        

        public void AddDataToHierarchyColumns(IODPDataTable IODPDataTable, string fileName, DataTable allSections)
        {
         
            int rowNumber = 1;

            foreach (DataRow row in IODPDataTable.DataTable.Rows)
            {

                LithologicDescription description = new LithologicDescription(row[IODPDataTable.SampleIDColumn].ToString());

                double parsedOffset = 0;

                LithologyConvertor.StartOffsetValuesAreValid(row, IODPDataTable, ref parsedOffset);

                description.StartOffset = parsedOffset;

                LithologyConvertor.EndOffsetValuesAreValid(row, IODPDataTable, ref parsedOffset);

                description.EndOffset = parsedOffset;

                LithologicIDGenerator idGenerator = new LithologicIDGenerator();


                var textids =  GetTextID(allSections, description);

                try
                {
                   var descriptionID = idGenerator.GenerateID(description);

                    row.BeginEdit();
                    row["Filename_VP"] = fileName;
                    row["LithologicID_VP"] = descriptionID;
                    row["ArchiveSectionTextID_VP"] = textids.Archive;
                    row["WorkingSectionTextID_VP"] = textids.Working;
                    row["ParentSectionTextID_VP"] = textids.Parent;
                    row["Expedition_VP"] = description.SectionInfo.Expedition;
                    row["Site_VP"] = description.SectionInfo.Site;
                    row["Hole_VP"] = description.SectionInfo.Hole;
                    row["Core_VP"] = description.SectionInfo.Core;
                    row["Type_VP"] = description.SectionInfo.Type;
                    row["Section_VP"] = description.SectionInfo.Section;
                    row["SectionHalf_VP"] = description.SectionInfo.Half;
                    row["TopOffset_VP"] = description.StartOffset.ToString();
                    row["BottomOffset_VP"] = description.EndOffset.ToString();
                    row.EndEdit();
                }
                catch (Exception)
                {
                    Log.Warning(string.Format("Row {0}: Unable to populate data for description", rowNumber.ToString()));
                }

                rowNumber++;
            }
        }

       public void ExportDataTableAsNewFile(string exportFilePath, DataTable dataTable) 
        {
            var exporter = new CSVReader();
            exporter.WritePath = exportFilePath;

            exporter.Write(dataTable);
        }

    }
}
