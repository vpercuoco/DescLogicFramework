using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace DescLogicFramework
{
    public static class Workflow
    {

        #region UploadingData



        private static async Task<bool> UpdateDatabaseMeasurementsWithMeasurementData(ICollection<Measurement> measurements)
        {
            using (DescDBContext dBContext = new DescDBContext())
            {
                foreach (var measurement in measurements)
                {
                    var measurementExists = await dBContext.MeasurementDescriptions
                   .Where(x => x.SectionInfo.ID == measurement.SectionInfo.ID)
                   .Where(x => x.TextID == measurement.TextID)
                   .Where(x => x.TestNumber == measurement.TestNumber)
                   .Where(x => x.StartOffset == measurement.StartOffset && x.EndOffset == measurement.EndOffset)
                   .FirstOrDefaultAsync().ConfigureAwait(true);

                    
                    if (measurementExists != null)
                    {
                        foreach (var columnValuePair in measurement.MeasurementData)
                        {
                            if (!measurementExists.MeasurementData.Any(x=>x.ColumnName == columnValuePair.ColumnName))
                            {
                                measurementExists.MeasurementData.Add(columnValuePair);
                            }
                            
                        }

                        dBContext.Update(measurement);
                        await dBContext.SaveChangesAsync();
                        //dBContext.u
                    }
                }
            }


            return true;
        }

        /// <summary>
        /// Parses a file into Measurements, checks the database for existence, if records are new uploads to database.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="columnIdentifiers"></param>
        /// <returns></returns>
        private static async Task<bool> UploadMeasurementsFromFileToDatabaseAsync(string filename, [Optional] IntervalHierarchyNames columnIdentifiers)
        {

            var measurements = await MeasurementHandler.GetMeasurementsFromFileAsync(filename, columnIdentifiers).ConfigureAwait(true);

            ICollection<Measurement> measurementsToRemove = new HashSet<Measurement>();

            using (DescDBContext dBContext = new DescDBContext())
            {
                foreach (var measurement in measurements)
                {
                    var measurementExists = await DatabaseWorkflowHandler.FindMeasurementInDatabase(dBContext, measurement).ConfigureAwait(true);
                    
                    if (measurementExists)
                    {
                        measurementsToRemove.Add(measurement);
                    }
                }
            }

            foreach (var measurement in measurementsToRemove)
            {
                measurements.Remove(measurement);
            }
          

            if (measurements.Count == 0)
            {
                return false;
            }

            bool isDataUploaded;

            foreach (var measurement in measurements)
            {
                measurement.MeasurementData.Clear(); //Not uploading measurement report data at the moment
            }

            using (DescDBContext dbContext = new DescDBContext())
            {
                isDataUploaded = await DatabaseWorkflowHandler.AddMeasurementsToDataBaseAsync(dbContext, measurements).ConfigureAwait(true);
            }

            return isDataUploaded;
        }

        private static async Task<bool> UploadDescriptionsFromFileToDatabaseAsync(string filename, [Optional] IntervalHierarchyNames columnIdentifiers)
        {
            var descriptions = await DescriptionHandler.GetDescriptionsFromFileAsync(filename, columnIdentifiers).ConfigureAwait(true);


            try
            {
                using (DescDBContext dBContext = new DescDBContext())
                {
                    foreach (var description in descriptions)
                    {
                        if (await DatabaseWorkflowHandler.CheckForDescriptionAsync(dBContext, description).ConfigureAwait(true))
                        {
                            descriptions.Remove(description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            if (descriptions.Count == 0)
            {
                return false;
            }

            bool isDataUploaded;

            using (DescDBContext dbContext = new DescDBContext())
            {
                isDataUploaded = await DatabaseWorkflowHandler.AddDescriptionsToDataBaseAsync(dbContext, descriptions).ConfigureAwait(true);
            }

            return isDataUploaded;
        }

        public static async  Task<ICollection<string>> AddAllMeasurementsToDatabase()
        {
            //var list = new CinnamonList("ExpeditionList").Parameters;
            //list.Reverse();
            //list.Remove("X376");
            //list.Remove("X372");
            //list.Remove("X371");
            //list.Remove("X368");
            //list.Remove("X367");
            //list.Remove("X366"); //Need to upload RGB
            //list.Remove("X362"); //Need to upload RGB
            //list.Remove("X361"); //Need to upload RGB
            //list.Remove("X360"); //Need to delete then add all measurements; Issue with disposing with DBcontext
            //list.Remove("X359");
            //list.Remove("X356");
            //list.Remove("X355");

            var list = new List<string>() { "X375", "X374", "X363", "X351", "X324" };


            ICollection<string> ErrorFiles = new HashSet<string>();


            foreach (var expedition in list)
            {
                FileCollection fileCollection = new FileCollection();
                string fileDirectory = $@"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AGU2019\{expedition}\data";
                fileCollection.AddFiles(fileDirectory, "*.csv");

                foreach (var file in fileCollection.Filenames)
                {
                    Log.Information($"{file}: Trying to add measurements to database");
                    try
                    {
                      var uploadSuccessful =  await Workflow.UploadMeasurementsFromFileToDatabaseAsync(file).ConfigureAwait(true);

                        if (uploadSuccessful)
                        {
                            Log.Information($"{file}: Successfully added measurements database");
                        }
                        else
                        {
                            Log.Information($"{file}: The file was not successfully uploaded. Either the all the measurments have been previously uploaded");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"{file}: Could not add measurement file to database");
                        Log.Warning(ex.Message);
                        Log.Warning(ex.StackTrace);
                        ErrorFiles.Add(file);
                    }

                    Thread.Sleep(1000);
                }

            }

            return ErrorFiles;
        }

        public static void AddAllDescriptionsToDatabase(string fileDirectory, [Optional] IntervalHierarchyNames columnIdentifiers)
        {

                FileCollection fileCollection = new FileCollection();
               

                fileCollection.AddFiles(fileDirectory, "*.csv");

                foreach (var file in fileCollection.Filenames)
                {
                    Log.Information($"{file}: Trying to add descriptions the database");
                    try
                    {
                       var x = Task.Run( async () => await UploadDescriptionsFromFileToDatabaseAsync(file, columnIdentifiers).ConfigureAwait(true));
                        x.Wait();

                        Log.Information($"{file}: Successfully added descriptions the database");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"{file}: Could not add descriptions to database");
                        Log.Warning(ex.Message);
                        Log.Warning(ex.StackTrace);
                    }

                    Thread.Sleep(1000);
                }
            
        }

        #endregion

       

        #region CheckingData

        /// <summary>
        /// Searches the database for any descriptions that do not have subintervals, then generates subintervals for those descriptions and saves to the database.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> EnsureAllLithologicDescriptionsHaveSubintervalsAsync()
        {
            bool keepRunning = true;
            int i = 1;

            while (keepRunning)
            {
                keepRunning = await SubintervalCreator.CreateSubIntervalsForDescriptions(100).ConfigureAwait(false);

                Console.WriteLine(string.Format("Task has completed {0} records", (i * 100)).ToString());
                i++;
            }
            return true;
        }

        public static void AssociateSubIntervalsWithMeasurements()
        {
            var x = Task.Run(async () => await SubintervalCreator.GetLithologicSubIntervalsForMeasurements().ConfigureAwait(false));
            x.Wait();
        }

        #endregion


        #region QueryingData

        public static async Task<bool> GetMeasurementIDForMeasurementFile(string file, string exportFilePath)
        {
            ICollection<Measurement> measurements = await MeasurementHandler.GetMeasurementsFromFileAsync(file).ConfigureAwait(true);

            using (DescDBContext dBContext = new DescDBContext())
            {

                foreach (var measurement in measurements)
                {
                    measurement.ID = await DatabaseWorkflowHandler.GetMeasurementIDAsync(dBContext, measurement).ConfigureAwait(true);

                }

            }

            //Column Names
            HashSet<string> columns = measurements.First().MeasurementData.Select(x=>x.ColumnName).ToHashSet();


            //Check if all measurements have the same Columns
            foreach (var measurement in measurements)
            {
                var compareColumns = measurement.MeasurementData.Select(x => x.ColumnName).ToHashSet();
                if ( !columns.SetEquals(compareColumns))
                {
                    return false;
                }
            }


            //TODO: Construct new Datatable
            
            using (DataTable dataTable = new DataTable())
            {
                foreach (var column in columns)
                {
                    dataTable.Columns.Add(column);

                }
                dataTable.Columns.Add("MeasurementID").SetOrdinal(0);


                int currentRow = 0;
                foreach (var measurement in measurements)
                {
                    dataTable.ImportRow(measurement.DataRow);
                    var row = dataTable.Rows[currentRow];
                    row.BeginEdit();
                    row["MeasurementID"] = measurement.ID;
                    row.EndEdit();

                    currentRow++;
                }

                //AddID column


                //Export measurements to File

                Importer.ExportDataTableAsNewFile(exportFilePath, dataTable);
            }
            
           

            return true;

        }
        
        #endregion



    }
}
