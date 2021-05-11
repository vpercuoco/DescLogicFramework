using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;

using System.Threading;
using System.Threading.Tasks;
using ServiceStack;
using System.IO;
using DescLogicFramework.DataAccess;
using System.Configuration;
using Serilog;

namespace DescLogicFramework
{
    public static class Examples
    {
        //TODO: You have one measurement file, can you:
        //1. Get a single description column for all the measurements and spit out the file
        //2. Get multiple description column for all the measurement and spit out the file
        public static void GetMeasurementsFromFileThenGetCertainDescriptionColumns()
        {
            string filename = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AGU2019\X376\data\MAD.csv";

            List<string> columnNames = new List<string>() { "Expedition_VP", "Site_VP", "Hole_VP" };

            var measurementCollection = Task.Run(async () => await MeasurementHandler.GetMeasurementsFromFileAsync(filename).ConfigureAwait(false));
            measurementCollection.Wait();
            var measurements = measurementCollection.Result;


            using (DescDBContext dBContext = new DescDBContext())
            {
                var multipleLookupTasks = Task.Run(async () =>
                {
                    var descriptionArray = await MeasurementHandler.GetDescriptionsForMeasurementAsync(dBContext, measurements, columnNames).ConfigureAwait(false);
                    return descriptionArray;
                });

                multipleLookupTasks.Wait();
                var descriptions = multipleLookupTasks.Result;

                /*Output to console
                foreach (var description in descriptions)
                {
                    Console.WriteLine(string.Format("ID: {0}, Filename: {1}, StartOffset: {2}, EndOffset: {3}", description.ID, description.DescriptionReport, description.StartOffset.ToString(), description.EndOffset.ToString()));
                    foreach (var columnValue in description.DescriptionColumnValues)
                    {
                        Console.WriteLine(string.Format("Column: {0}, Value: {1}", columnValue.ColumnName, columnValue.Value));
                    }
                }
                */

                //Need to output a datatable with the desccription columns added to the end of the measurements.
                //What to do if there are more than one description for a measurement? 
                //Description columns:
                    //1. "Filename" (As header), File description Came from (as row)
                    //2. specific Column (as header), value (as row)
            }
        }

        public static void GetOneMeasurementFromFileThenGetCertainDescriptionColumns()
        {
            string filename = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AGU2019\X376\data\MAD.csv";

            ICollection<Measurement> measurements;
            try
            {
                var x = Task.Run(async () => await MeasurementHandler.GetMeasurementsFromFileAsync(filename).ConfigureAwait(false));
                x.Wait();
                measurements = x.Result;
            }
            catch (Exception)
            {

                throw;
            }

            var measurement = measurements.First();

            List<string> columnNames = new List<string>() { "Expedition_VP", "Site_VP", "Hole_VP" };

            using (DescDBContext dBContext = new DescDBContext())
            {
                ICollection<string> columns = new HashSet<string>() { "Expedition_VP", "Site_VP", "Hole_VP" };

                var y = Task.Run(async () => await MeasurementHandler.GetDescriptionsWithinMeasurementIntervalAsync(dBContext, measurement, columns).ConfigureAwait(false));
                y.Wait();
                var t = y.Result;

                foreach (var item in t)
                {
                     Console.WriteLine(string.Format("ID: {0}, Filename: {1}, StartOffset: {2}, EndOffset: {3}", item.ID, item.DescriptionReport, item.StartOffset.ToString(), item.EndOffset.ToString()));

                }
            }
        }

        public static void DisplayMeasurementsForALithology(string principalLithology)
        {
            var t = Task.Run(async () => await QueryPlayground.GetMeasurementIDsForLithology(principalLithology).ConfigureAwait(true));
            t.Wait();
            foreach (var item in t.Result)
            {
                Console.WriteLine(item.ID);
            }
        }


        /*
        public static void GetMeasurementIDsForMeasurementFiles()
        {

            FileCollection fileCollection = new FileCollection();
            fileCollection.AddFiles(@"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\MAD\", "*.csv");
            // fileCollection.Filenames.Reverse();
            foreach (var file in fileCollection.Filenames)
            {
                var t = Task.Run(async () => await Workflow.GetMeasurementIDForMeasurementFile(file, @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\ConvertedMeasurements\" + Importer.GetFileName(file)).ConfigureAwait(true));
                t.Wait();
            }

           // DescriptionHandler.CleanDescriptionFiles(@"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\ErrorFilesWithData\X372", "372");

            fileCollection = new FileCollection();
            fileCollection.AddFiles(@"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AdditionalDataSets", "*.csv");
            fileCollection.Filenames.Reverse();
            foreach (var filename in fileCollection.Filenames)
            {
                var x = Task.Run(async () => await Workflow.UploadMeasurementsFromFileToDatabaseAsync(filename).ConfigureAwait(true));
                x.Wait();
            }
        }
        */

        public static void GetMeasurementsWithDrillingDisturbances()
        {
           var x = Task.Run(async() => await QueryPlayground.GetMeasurementsWithDrillingDisturbances("GRA").ConfigureAwait(true));
           x.Wait();
           ICollection<Mashup> measurements = x.Result;
            HashSet<string> values = new HashSet<string>(); 

            //Console.WriteLine("MeasurementID,LithologicSubID,DescriptionColumnValuePairID,ColumnName,ColumnValue");
            foreach (var item in measurements)
            {
                // Console.WriteLine($"{item.MeasurementID},{item.LithologicSubID},{item.ColumnValuePairID},{item.ColumnName},{item.ColumnValue}");
                values.Add(item.WriteString());
            }

            File.AppendAllLines(@"C:\Users\vperc\Desktop\GRA_Mashup\GRA_DrillingDisturbances", values);
           
        }


        public static void CorrectDrillingDisturbanceFiles()
        {

            string folder = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\ErrorFilesWithData\DrillingDisturbancesFilesToBeCorrected\";

            string newFolder = @"C:\Users\vperc\Desktop\AddedSampleID\";
            string drillingExportsFolder = @"C:\Users\vperc\Desktop\CorrectedDrilling\";

            FileCollection files = new FileCollection();
            files.AddFiles(folder, "*.csv");


            foreach (var file in files.Filenames)
            {
                string currentFileName = Path.GetFileName(file);
                string newFileLocation = Path.Combine(drillingExportsFolder, currentFileName);
                //Examples.AddSampleIDColumnToFileAndExport(file, newFileLocation);
                DrillingCorrection.FormatDrillingDisturbanceFile(file, newFileLocation);

            }
        }
        
        
    }
}
