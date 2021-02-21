using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

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

            var measurementCollection = Task.Run(async () => await MeasurementHandler.GetMeasurementsFromFileAsync(filename));
            measurementCollection.Wait();
            var measurements = measurementCollection.Result;


            using (DescDBContext dBContext = new DescDBContext())
            {
                var multipleLookupTasks = Task.Run(async () =>
                {
                    var descriptionArray = await MeasurementHandler.GetDescriptionsForMeasurementAsync(dBContext, measurements, columnNames);
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
                var x = Task.Run(async () => await MeasurementHandler.GetMeasurementsFromFileAsync(filename));
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

                var y = Task.Run(async () => await MeasurementHandler.GetDescriptionsWithinMeasurementIntervalAsync(dBContext, measurement, columns));
                y.Wait();
                var t = y.Result;

                foreach (var item in t)
                {
                     Console.WriteLine(string.Format("ID: {0}, Filename: {1}, StartOffset: {2}, EndOffset: {3}", item.ID, item.DescriptionReport, item.StartOffset.ToString(), item.EndOffset.ToString()));

                }
            }
        }



        //This needs to be a mashup of measurements from multiple files
        public static void GetMultipleMeasurementsOnOneRow()
        {

        }
    }
}
