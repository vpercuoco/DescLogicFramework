using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.ComponentModel.Design;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Transactions;
using System.Globalization;
using Serilog;
using Serilog.Sinks;
using Serilog.Sinks.File;



namespace DescLogicFramework
{
    public class Program
    {
        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(ConfigurationManager.AppSettings["LogFileLocation"])
                .CreateLogger();

            CleanDescriptionFiles();

            Log.CloseAndFlush();



            Console.WriteLine("Starting up at " + DateTime.Now.ToString());

            var conn = ConfigurationManager.ConnectionStrings["DBconnection"];

            ProgramSettings.SendDataToDESCDataBase = false;

            ProgramSettings.SendDataToLithologyDataBase = false;

            ProgramSettings.ExportCachesToFiles = false;

            ProgramSettings.ProcessMeaurements = false;

            _ = new ProgramWorkFlowHandler();
            Console.WriteLine("Program finished at " + DateTime.Now.ToString(CultureInfo.CurrentCulture));

        }

        private static void CleanDescriptionFiles()
        {
            var list = new CinnamonList("ExpeditionList").Parameters;
            list.Reverse();

            foreach (var expedition in list)
            {
                DescriptionFileCleaner cleaner = new DescriptionFileCleaner(@"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AGU2019\" + expedition + @"\output\extracted_csv", expedition);
            }
        }

        //TODO: Continue working on adding descriptions to DB
        private static void AddDescriptionsToDatabase()
        {
            ICollection<LithologicDescription> descriptions = new HashSet<LithologicDescription>();


            //Import Descriptions from my files
            //Disregard my headers
            //Send to database





            using (DescDBContext dbContext = new DescDBContext())
            {
                foreach (var description in descriptions)
                {
                    SectionInfo section = GetSectionInfoFromDatabase(dbContext, description);

                    description.SectionInfo = section ?? description.SectionInfo;

                    dbContext.Add(description);
                }

                //  dbContext.SaveChanges();
            }
        }

        

        private static SectionInfo GetSectionInfoFromDatabase(DescDBContext dbContext, LithologicDescription description)
        {
            return dbContext.Sections.Where(record => record.Expedition == description.SectionInfo.Expedition
                                              && record.Site == description.SectionInfo.Site
                                              && record.Hole == description.SectionInfo.Hole
                                              && record.Core == description.SectionInfo.Core
                                              && record.Type == description.SectionInfo.Type
                                              && record.Section == description.SectionInfo.Section).FirstOrDefault();
        }

        private static void AddAllSectionsToDatabase()
        {

            SectionInfoCollection sectionCollection = new SectionInfoCollection();

            SampleHierarchy columnNames = new SampleHierarchy { Expedition = "Exp", Site = "Site", Hole = "Hole", Core = "Core", Type = "Type", Section = "Sect", ParentTextID = "Text ID of section", ArchiveTextID = "Text ID of archive half", WorkingTextID = "Text ID of working half" };

            sectionCollection.ParseSectionInfoFromDataTable(SectionInfoCollection.ImportAllSections(), columnNames);


            using (DescDBContext dbContext = new DescDBContext())
            {

                dbContext.AddRange(sectionCollection.Sections);
                dbContext.SaveChanges();
            }
        }
    }

    public static class ProgramSettings
    {
        public static bool SendDataToDESCDataBase { get; set; } = false;
        public static bool SendDataToLithologyDataBase { get; set; } = false;
        public static bool ExportCachesToFiles { get; set; } = false;
        public static bool ProcessMeaurements { get; set; } = false;
    }

}
