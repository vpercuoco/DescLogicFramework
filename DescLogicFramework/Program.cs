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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;



namespace DescLogicFramework
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Starting up at: {DateTime.Now.ToString()}");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(ConfigurationManager.AppSettings["LogFileLocation"])
                .CreateLogger();


            #region MergingCSVS
            /* 
             * 
             * Merge CSVs
          
             */
            /*
            string directory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AGU2019\";


            List<string> filenames = new List<string>();
            foreach (var item in Directory.GetFiles(directory, "*.csv", SearchOption.AllDirectories))
            {
                if (item.Contains("CARB.csv"))
                {
                    filenames.Add(item);
                }
            }
           

            string[] filePaths = filenames.ToArray();


            string sourceFolder = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\ConvertedMeasurements\MAD\";
            string destinationFile = @"C:\Users\vperc\Desktop\CombinedCsvs\CARB_Combined.csv";

            // Specify wildcard search to match CSV files that will be combined
            //string[] filePaths = Directory.GetFiles(sourceFolder, "*.csv");
            
            MergeFiles(filePaths, destinationFile);

            */
            #endregion

            /*
            // Cleaning description files: 

            string directory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\ErrorFilesWithData\CorrectedFIleThatCanNowBeUploaded\";
            string exportDirectory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\ErrorFilesWithData\SingleExport\";
            string errorDirectory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\ErrorFilesWithData\SingleError\";

            DescriptionHandler.CleanDescriptionFiles(directory, exportDirectory, errorDirectory);
            */


            /*
            //Uploading Descriptions to database
            
            string fileDirectory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\ErrorFilesWithData\SingleExport\";
            Workflow.AddAllDescriptionsToDatabase(fileDirectory);
            
            */



            /*
            //Generate subintervals for Descriptions
           
            var t = Task.Run(async () => await Workflow.EnsureAllLithologicDescriptionsHaveSubintervalsAsync().ConfigureAwait(true));
            t.Wait();
            */

            Workflow.AssociateSubIntervalsWithMeasurements();





            // Examples.GetMeasurementsWithDrillingDisturbances();

            //Examples.DisplayMeasurementsForALithology("clast");

            // Examples.AssociateSubIntervalsForMeasurements();

            //Examples.GetMeasurementsFromFileThenGetCertainDescriptionColumns();







            /*
            string filename = @"C:\Users\vperc\Desktop\X368_drilling_dist_macroscopic_macroscopic_U1502A.csv";
            var x = Examples.FormatDrillingDisturbanceFile(filename);

            Log.CloseAndFlush();

            Console.WriteLine($"Program finished at: {DateTime.Now.ToString(CultureInfo.CurrentCulture)}");
            */
        }


        public static void MergeFiles(string[] filePaths, string destinationFile)
        {
            StreamWriter fileDest = new StreamWriter(destinationFile, true);

            int i;
            for (i = 0; i < filePaths.Length; i++)
            {
                string file = filePaths[i];

                string[] lines = File.ReadAllLines(file);

                if (i > 0)
                {
                    lines = lines.Skip(1).ToArray(); // Skip header row for all but first file
                }

                foreach (string line in lines)
                {
                    fileDest.WriteLine(line);
                }
            }

            fileDest.Close();
        }
        public static void MergeCSVs(string sourceFolder, string destinationFile, [Optional] List<string> filenames)
        {

        }

    }

}
