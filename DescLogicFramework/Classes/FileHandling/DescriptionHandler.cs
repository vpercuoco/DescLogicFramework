using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DescLogicFramework.DataAccess;
using Serilog;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DescLogicFramework
{

    //TODO: Output warnings from the parsing into a LOG file that I can consult when dealing with error files.
    //TODO: Make bAtch CleanUpdescriptionFIles ASYNC

    public static class DescriptionHandler
    {
        /// <summary>
        /// Gets a collection of LithologicDescriptions from a corrected file. 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="columnIdentifiers"></param>
        /// <returns></returns>
        public static async Task<ICollection<LithologicDescription>> GetDescriptionsFromFileAsync(string filename, [Optional] IntervalHierarchyNames columnIdentifiers)
        {
            columnIdentifiers = columnIdentifiers ?? new IntervalHierarchyNames()
            {

                Expedition = "Expedition_VP",
                Site = "Site_VP",
                Hole = "Hole_VP",
                Core = "Core_VP",
                Type = "Type_VP",
                Section = "Section_VP",
                Half = "SectionHalf_VP",
                TopOffset = "TopOffset_VP",
                BottomOffset = "BottomOffset_VP",
                ArchiveTextID = "ArchiveSectionTextID_VP",
                WorkingTextID = "WorkingSectionTextID_VP",
                ParentTextID = "ParentSectionTextID_VP"

            };

            IODPDataTable iODPDataTable = Importer.ImportDataTableFromFile(filename, columnIdentifiers);

            ICollection<LithologicDescription> descriptions = new HashSet<LithologicDescription>();

            using (DescDBContext dbContext = new DescDBContext())
            {
                try
                {
                    foreach (DataRow row in iODPDataTable.DataTable.Rows)
                    {
                        SectionInfo section = new SectionInfo(Importer.GetHierarchyValuesFromDataRow(row, columnIdentifiers));
                        LithologicDescription description = new LithologicDescription();
                       // description.SectionInfo = section;
                        description.SectionInfo = await DatabaseWorkflowHandler.GetSectionInfoFromDatabaseForIntervalAsync(dbContext, section).ConfigureAwait(true);
                        description.LithologicID = row["LithologicID_VP"].ToString();
                        description.DataRow = row;
                        description.DescriptionReport = row["Filename_VP"].ToString(); ;
                        description.StartOffset = double.TryParse(row[columnIdentifiers.TopOffset].ToString(), out double startOffset) ? startOffset : -1;
                        description.EndOffset = double.TryParse(row[columnIdentifiers.BottomOffset].ToString(), out double endOffset) ? endOffset : -1;

                        descriptions.Add(description);
                    }
                }
                catch (Exception)
                {

                    throw new Exception("Error creating lithologic description from data row");
                }
            }

            return descriptions;

        }


        #region CleaningUpDescriptionFiles

        public static void CleanDescriptionFiles(string directory, string exportDirectory, string errorExportDirectory)
        {

            FileCollection FileCollection = new FileCollection();

            FileCollection.AddFiles(directory, "*.csv");

            foreach (string file in FileCollection.Filenames)
            {
                string currentFileName = Importer.GetFileName(file);
                string exportFileName = exportDirectory + currentFileName;
                string errorFileName = errorExportDirectory + currentFileName;


                try
                {
                    CleanupDescriptionFile(file, exportFileName, errorFileName);
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Format("{0}: Could not clean file", currentFileName));
                    Log.Warning(string.Format("{0}: {1}", currentFileName, ex.Message));
                    Log.Warning(string.Format("{0}: Stack Trace: {1}", currentFileName, ex.StackTrace));
                }
            }
        }

        private static void CleanupDescriptionFile(string filePath, string exportFilePath, string errorExportFilePath)
        {

            var iodpDataTable = Importer.ImportDataTableFromFile(filePath);

            string currentFileName = Importer.GetFileName(filePath);
            
            try
            {
                Importer.CheckFile(iodpDataTable);
                AddMissingColumnsToDescriptionTable(iodpDataTable.DataTable);
                AddDataToHierarchyColumns(iodpDataTable, Importer.GetFileName(filePath), SectionInfoCollection.ImportAllSections(ConfigurationManager.AppSettings["AllSectionsFile"]));
                Log.Information($"{currentFileName}: Processed successfully");
            }
           catch (Exception ex)
            {
                Log.Warning($"{currentFileName}: {ex.Message}");

                exportFilePath = errorExportFilePath;
            }


            try
            {
                Importer.ExportDataTableAsNewFile(exportFilePath, iodpDataTable.DataTable);
            }
            catch (Exception)
            {
                throw new Exception($"{currentFileName}: Error exporting to file");
            }

        }

        private static void AddMissingColumnsToDescriptionTable(DataTable dataTable)
        {
         
            string[] columnNames = new string[] 
            { 
                "Filename_VP", 
                "LithologicID_VP", 
                "ArchiveSectionTextID_VP", 
                "WorkingSectionTextID_VP", 
                "ParentSectionTextID_VP", 
                "Expedition_VP", 
                "Site_VP", 
                "Hole_VP", 
                "Core_VP", 
                "Type_VP", 
                "Section_VP", 
                "SectionHalf_VP", 
                "TopOffset_VP", 
                "BottomOffset_VP" 
            };

            foreach (string name in columnNames.Reverse())
            {
                if (!dataTable.Columns.Contains(name))
                {
                    dataTable.Columns.Add(name, typeof(string)).SetOrdinal(0);
                }
                
            }

        }

        private static (string Archive, string Working, string Parent) GetSectionTextIDsForDescription(DataTable allSections, LithologicDescription description, IntervalHierarchyNames columnNames)
        {
         
                var matchingTextID = allSections.AsEnumerable().Where(x => x.Field<string>(columnNames.Expedition) == description.SectionInfo.Expedition
                                                       && x.Field<string>(columnNames.Site) == description.SectionInfo.Site
                                                       && x.Field<string>(columnNames.Hole) == description.SectionInfo.Hole
                                                       && x.Field<string>(columnNames.Core) == description.SectionInfo.Core
                                                       && x.Field<string>(columnNames.Type) == description.SectionInfo.Type
                                                       && x.Field<string>(columnNames.Section) == description.SectionInfo.Section).FirstOrDefault();


                if (matchingTextID == null)
                {
                    return (Archive: "-1", Working: "-1", Parent: "-1");
                }

                (string Archive, string Working, string Parent) textids = (matchingTextID["Text ID of archive half"].ToString(), matchingTextID["Text ID of working half"].ToString(), matchingTextID["Text ID of section"].ToString());
                return textids;

        }

        private static void AddDataToHierarchyColumns(IODPDataTable IODPDataTable, string fileName, DataTable allSectionsDataTable)
        {
         
            int rowNumber = 1;

            IntervalHierarchyNames sectionTableColumnNames = new IntervalHierarchyNames()
            {
                Expedition = "Exp",
                Site = "Site",
                Hole = "Hole",
                Core = "Core",
                Type = "Type",
                Section = "Sect"
            };

            foreach (DataRow row in IODPDataTable.DataTable.Rows)
            {

                LithologicDescription description = new LithologicDescription(row[IODPDataTable.SampleIDColumn].ToString());

                double parsedOffset = 0;

                Importer.StartOffsetValuesAreValid(row, IODPDataTable, ref parsedOffset);

                description.StartOffset = parsedOffset;

                Importer.EndOffsetValuesAreValid(row, IODPDataTable, ref parsedOffset);

                description.EndOffset = parsedOffset;

                LithologicIDGenerator idGenerator = new LithologicIDGenerator();

                var textids =  GetSectionTextIDsForDescription(allSectionsDataTable, description, sectionTableColumnNames);

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

        #endregion
    }
}
