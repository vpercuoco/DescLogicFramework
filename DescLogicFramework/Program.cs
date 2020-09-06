using DescLogicFramework.DataAccess;
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

namespace DescLogicFramework
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up at " + DateTime.Now.ToString());

            var conn = ConfigurationManager.ConnectionStrings["DBconnection"];

            ProgramSettings.SendDataToDataBase = false;

            ProgramWorkFlowHandler p = new ProgramWorkFlowHandler();
            Console.WriteLine("Program finished at " + DateTime.Now.ToString(CultureInfo.CurrentCulture));

        }
    }


    static class ProgramSettings
    {
        public static bool SendDataToDataBase { get; set; } = false;
    }
    public class ProgramWorkFlowHandler
    {
        public ProgramWorkFlowHandler()
        {
            this.BatchProcessCompleted += NotifyConsoleOfBatchProcessed;
            string expedition = "";

            do
            {
                Console.WriteLine("Enter an expedition number to process its files or press E to exit");
                expedition = Console.ReadLine();
                expedition = "X" + expedition;

                //Create a batch of description csv files
                FileCollection descriptionFileCollection = new FileCollection();

                descriptionFileCollection.AddFiles(@"C:\Users\percuoco\Desktop\AGU2019\AGU2019\" + expedition + @"\output\extracted_csv", "*.csv");
                descriptionFileCollection.ExportDirectory = ConfigurationManager.AppSettings["ExportDirectory"] + expedition + @"\Lithology\";
                descriptionFileCollection.Filenames.RemoveAll(x => !x.ToLower().Contains("sediment_macroscopic"));

                FileCollection measurementFileCollection = new FileCollection();

                measurementFileCollection.AddFiles(@"C:\Users\percuoco\Desktop\AGU2019\AGU2019\" + expedition + @"\output\track_data", "*.csv");
                measurementFileCollection.ExportDirectory = ConfigurationManager.AppSettings["ExportDirectory"] + expedition + @"\Measurements\";

                ProcessData(descriptionFileCollection, measurementFileCollection);

                this.OnBatchProcessCompleted(this, new BatchProcessCompleteEventArgs(expedition));
            }
            while (expedition != "E");


        }
        public ProgramWorkFlowHandler(FileCollection descriptionFileCollection, FileCollection measurementFileCollection)
        {
            _ = descriptionFileCollection ?? throw new ArgumentNullException(nameof(descriptionFileCollection));
            _ = measurementFileCollection ?? throw new ArgumentNullException(nameof(descriptionFileCollection));


            if (descriptionFileCollection.ExportDirectory != null && measurementFileCollection.ExportDirectory != null)
            {
                ProcessData(descriptionFileCollection, measurementFileCollection);
            }
        }

        
        private void ProcessData(FileCollection DescriptionFileCollection, FileCollection MeasurementFileCollection)
        {

            #region ImportDescriptionData
            foreach (string fileName in DescriptionFileCollection.Filenames)
            {
                Console.WriteLine(fileName.ToString());
            }

            var lithologyWorkflowHandler = new CSVLithologyWorkflowHandler();
            lithologyWorkflowHandler.FileCollection = DescriptionFileCollection;
            lithologyWorkflowHandler.ExportDirectory = DescriptionFileCollection.ExportDirectory;

            var SectionCollection = new SectionInfoCollection();
            var lithologyCache = lithologyWorkflowHandler.ImportCache(ref SectionCollection);

            if (ProgramSettings.SendDataToDataBase)
            {
                SendLithologiesToDatabase(lithologyCache);
            }
            #endregion

            #region ImportMeasurementData
            var measurementWorkFlowHandler = new CSVMeasurementWorkFlowHandler();
            measurementWorkFlowHandler.FileCollection = new FileCollection();

            foreach (string path in MeasurementFileCollection.Filenames)
            {
                Console.WriteLine("Processing measurement file: " + path);

                measurementWorkFlowHandler.FileCollection.RemoveFiles();
                measurementWorkFlowHandler.FileCollection.Filenames.Add(path);

                var measurementCache = measurementWorkFlowHandler.ImportCache(ref SectionCollection);
                var associatedMeasurementCache = measurementWorkFlowHandler.UpdateMeasurementCacheWithLithologicDescriptions(ref measurementCache, ref lithologyCache);

                if (ProgramSettings.SendDataToDataBase)
                {
                    SendMeasurementsToDatabase(associatedMeasurementCache);
                }

                measurementWorkFlowHandler.ExportDirectory = MeasurementFileCollection.ExportDirectory;
                measurementWorkFlowHandler.ExportCache(associatedMeasurementCache);

                measurementCache.Dispose();
                measurementCache = null;
                associatedMeasurementCache.Dispose();
                associatedMeasurementCache = null;
                GC.Collect();

                Console.WriteLine("The total section count is: " + SectionCollection.Sections.Count);
                #endregion
            }

            Console.WriteLine("Finished processing measurement files at: " + DateTime.Now.ToString());
        }
        private void SendLithologiesToDatabase(Cache<string, LithologicDescription> LithologyCache)
        {
            DescDBContext context = null;
            try
            {
                context = new DescDBContext();
                //context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;

                var descriptionList = LithologyCache.GetCollection().Values.Where(x => !string.IsNullOrEmpty(x.LithologicID));

                // context.AddRange(descriptionList);

                for (int i = 0; i < descriptionList.Count(); i = i + 100)
                {
                    var entityToInsert = descriptionList.Skip(i).Take(100).ToList();
                    count = 100;
                    context = AddLithologiesToContext(context, entityToInsert, count, 100, true);
                }


                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }

        }
        private void SendMeasurementsToDatabase(Cache<int, Measurement> MeasurementCache)
        {
            DescDBContext context = null;
            try
            {
                context = new DescDBContext();
                //context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                var descriptionList = MeasurementCache.GetCollection().Values.Where(x => x.LithologicSubID.HasValue);
                for (int i = 0; i < descriptionList.Count(); i = i + 100)
                {
                    var entityToInsert = descriptionList.Skip(i).Take(100);
                    count = 100;
                    context = AddMeasurementsToContext(context, entityToInsert, count, 100, true);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        private DescDBContext AddToContext<TEntity>(DescDBContext context, IEnumerable<TEntity> entities, int count, int commitCount, bool recreateContext) where TEntity : class
        {
            context.AddRange(entities);


            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new DescDBContext();
                }
            }

            return context;
        }

        private DescDBContext AddLithologiesToContext(DescDBContext context, IEnumerable<LithologicDescription> descriptions, int count, int commitCount, bool recreateContext)
        {
            context.AddRange(descriptions);

            //Detach all child records which already have been added otherwise I get the following error
            // Cannot insert explicit value for identity column in table 'Sections' when IDENTITY_INSERT is set to OFF.
            foreach (var description in descriptions)
            {
                if (description.SectionInfo.ID != 0)
                {
                    context.Entry(description.SectionInfo).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }

                foreach (LithologicSubinterval subInterval in description.LithologicSubintervals)
                {
                    //Detach subintervals already added (and have non-zero primary keys)
                    if (subInterval.ID != 0)
                    {
                        context.Entry(subInterval).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                    //Detach subinterval sectioninfo already added (and have non-zero primary keys)
                    if (subInterval.SectionInfo.ID != 0)
                    {
                        context.Entry(subInterval.SectionInfo).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                }
            }

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new DescDBContext();
                }
            }

            return context;

        }
        private DescDBContext AddMeasurementsToContext(DescDBContext context, IEnumerable<Measurement> measurements, int count, int commitCount, bool recreateContext)
        {
            context.AddRange(measurements);


            //Detach all child records which already have been added otherwise I get the following error
            // Cannot insert explicit value for identity column in table ... when IDENTITY_INSERT is set to OFF.
            //Measurements can only get descriptions and subintervals which have already been loaded
            foreach (var measurement in measurements)
            {

                //try detaching only the subinterval
                // context.Entry(measurement.LithologicSubinterval).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                //context.Entry(measurement.LithologicDescription).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                context.Entry(measurement.LithologicDescription).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

                foreach (LithologicSubinterval subInterval in measurement.LithologicDescription.LithologicSubintervals)
                {
                    if (measurement.LithologicSubinterval.ID != 0)
                    {
                        context.Entry(subInterval).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }

                }

                if (measurement.LithologicSubinterval.ID != 0)
                {
                    context.Entry(measurement.LithologicSubinterval).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }

                //DescriptionColumnValuePairs are tracked and accessed via the measurement's LithologicSubinterval property
                foreach (DescriptionColumnValuePair entry in measurement.LithologicDescription.Data)
                {
                    context.Entry(entry).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }

                if (measurement.SectionInfo.ID != 0)
                {
                    context.Entry(measurement.SectionInfo).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }
            }

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new DescDBContext();
                }
            }

            return context;

        }

        public event EventHandler<BatchProcessCompleteEventArgs> BatchProcessCompleted;
        protected virtual void OnBatchProcessCompleted(object sender, BatchProcessCompleteEventArgs e)
        {
            BatchProcessCompleted?.Invoke(this, e);
        }
        private void NotifyConsoleOfBatchProcessed(object sender, BatchProcessCompleteEventArgs e)
        {
            Console.WriteLine(@"All the files for Expedition {0} have been processed!", e.Expedition);
        }
    }

    /// <summary>
    /// An event args class to handle processing of all files from a single expedition
    /// </summary>
    public class BatchProcessCompleteEventArgs : EventArgs
    {
        public string Expedition { get; set; } = "None set!";
        public BatchProcessCompleteEventArgs(string expedition)
        {
            Expedition = expedition;
        }
    }
}
