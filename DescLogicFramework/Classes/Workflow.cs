using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Linq;
using System.Data;

namespace DescLogicFramework
{
    public static class Workflow
    {

        #region UploadingData

        /// <summary>
        /// Parses a file into Measurements, checks the database for existence, if records are new uploads to database.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="columnIdentifiers"></param>
        /// <returns></returns>
        public static async Task<bool> UploadMeasurementsFromFileToDatabaseAsync(string filename, [Optional] IntervalHierarchyNames columnIdentifiers)
        {

            var measurements = await MeasurementHandler.GetMeasurementsFromFileAsync(filename, columnIdentifiers);

            ICollection<Measurement> measurementsToRemove = new HashSet<Measurement>();

            using (DescDBContext dBContext = new DescDBContext())
            {
                foreach (var measurement in measurements)
                {
                    var measurementExists = await DatabaseWorkflowHandler.DoesMeasurementExistInDatabase(dBContext, measurement).ConfigureAwait(true);
                    
                    if (measurementExists)
                    {
                        measurementsToRemove.Add(measurement);
                    }
                }
            }

            foreach (var record in measurementsToRemove)
            {
                measurements.Remove(record);
            }
          

            if (measurements.Count == 0)
            {
                return false;
            }

            bool dataUploaded;

            foreach (var measurement in measurements)
            {
                measurement.MeasurementData.Clear(); //Not uploading measurement report data at the moment
            }

            using (DescDBContext dbContext = new DescDBContext())
            {
                dataUploaded = await DatabaseWorkflowHandler.AddMeasurementsToDataBaseAsync(dbContext, measurements);
            }

            return dataUploaded;
        }

        public static async Task<bool> UploadDescriptionsFromFileToDatabaseAsync(string filename, [Optional] IntervalHierarchyNames columnIdentifiers)
        {
            var descriptions = await DescriptionHandler.GetDescriptionsFromFileAsync(filename, columnIdentifiers);


            using (DescDBContext dBContext = new DescDBContext())
            {
                foreach (var description in descriptions)
                {
                    if (await DatabaseWorkflowHandler.DoesLithologicDescriptionExistInDatabase(dBContext, description))
                    {
                        descriptions.Remove(description);
                    }
                }
            }

            if (descriptions.Count == 0)
            {
                return false;
            }

            bool dataUploaded;

            using (DescDBContext dbContext = new DescDBContext())
            {
                dataUploaded = await DatabaseWorkflowHandler.AddDescriptionsToDataBaseAsync(dbContext, descriptions);
            }

            return dataUploaded;
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
                string fileDirectory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\AGU2019\" + expedition + @"\data";
                fileCollection.AddFiles(fileDirectory, "*.csv");

                foreach (var file in fileCollection.Filenames)
                {
                    Log.Information(string.Format("{0}: Trying to add measurements to database", file));
                    try
                    {
                      var uploadSuccessful =  await Workflow.UploadMeasurementsFromFileToDatabaseAsync(file).ConfigureAwait(true);

                        if (uploadSuccessful)
                        {
                            Log.Information(string.Format("{0}: Successfully added measurements database", file));
                        }
                        else
                        {
                            Log.Information(string.Format("{0}: The file was not successfully uploaded. Either the all the measurments have been previously uploaded", file));
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(string.Format("{0}: Could not add measurement file to database", file));
                        Log.Warning(ex.Message);
                        Log.Warning(ex.StackTrace);
                        ErrorFiles.Add(file);
                    }

                    Thread.Sleep(1000);
                }

            }

            return ErrorFiles;
        }

       public static void AddAllDescriptionsToDatabase()
        {
            var list = new CinnamonList("ExpeditionList").Parameters;
            list.Reverse();


            foreach (var expedition in list)
            {
                FileCollection fileCollection = new FileCollection();
                string fileDirectory = @"C:\Users\vperc\Desktop\All Hard Drive Files\DESC_DATAMINE\CleanedLithologies\" + expedition;
                fileCollection.AddFiles(fileDirectory, "*.csv");

                foreach (var file in fileCollection.Filenames)
                {
                    Log.Information(string.Format("{0}: Trying to add descriptions the database", file));
                    try
                    {
                        //  AddDescriptionsToDataBaseAsync(file);
                        Log.Information(string.Format("{0}: Successfully added descriptions the database", file));
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(string.Format("{0}: Could not add descriptions to database", file));
                        Log.Warning(ex.Message);
                        Log.Warning(ex.StackTrace);
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        #endregion


        #region CheckingData
        public static void CheckMeasurementsAndSubIntervalsAreCorrelatedForExpedition(string expedition) { }

        public static async Task<bool> EnsureAllLithologicDescriptionsHaveSubintervals(string expedition)
        {
            bool keepRunning = true;
            int i = 1;

            while (keepRunning)
            {
                keepRunning = await SubintervalCreator.CreateSubIntervalsForDescriptions(100, expedition);

                Console.WriteLine(string.Format("Task has completed {0} records", (i * 100)).ToString());
                i++;
            }
            return true;
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
                    measurement.ID = await DatabaseWorkflowHandler.GetIDForMeasurement(dBContext, measurement).ConfigureAwait(true);

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
            DataTable dataTable = new DataTable();

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
           

            return true;

        }
        
        #endregion



    }
}
