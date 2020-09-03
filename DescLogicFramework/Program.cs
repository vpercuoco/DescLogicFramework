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

namespace DescLogicFramework
{
    public class Program
    {


        static void Main(string[] args)
        {

            Console.WriteLine("Starting up at " + DateTime.Now.ToString());

            var conn = ConfigurationManager.ConnectionStrings["DBconnection"];

            ProgramSettings.SendDataToDataBase = true;

         
            ProgramWorkFlowHandler p = new ProgramWorkFlowHandler();
            Console.WriteLine("Program finished at " + DateTime.Now.ToString());
        }
    }


    static class ProgramSettings
    {
        public static bool SendDataToDataBase { get; set; } = false;
    }


    /// <summary>
    /// 
    /// </summary>
    public class ProgramWorkFlowHandler
    {

        public event EventHandler<BatchProcessCompleteEventArgs> BatchProcessCompleted;
        protected virtual void OnBatchProcessCompleted(object sender, BatchProcessCompleteEventArgs e)
        {
            BatchProcessCompleted?.Invoke(this, e);
        }
        private void NotifyConsoleOfBatchProcessed(object sender, BatchProcessCompleteEventArgs e)
        {
            Console.WriteLine(@"All the files for Expedition {0} have been processed!", e.Expedition);
        }
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
            while (expedition != "E") ;

           
        }
        public ProgramWorkFlowHandler(FileCollection DescriptionFileCollection, FileCollection MeasurementFileCollection)
        {
            if (DescriptionFileCollection.ExportDirectory != null && MeasurementFileCollection.ExportDirectory != null )
            {
                ProcessData(DescriptionFileCollection, MeasurementFileCollection);
            }
        }


        private DescDBContext AddToContext<TEntity>(DescDBContext context, IEnumerable<TEntity> entity, int count, int commitCount, bool recreateContext) where TEntity : class
        {
            context.AddRange(entity);
            

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
        private void ProcessData( FileCollection DescriptionFileCollection, FileCollection MeasurementFileCollection)
        {

            foreach (string fileName in DescriptionFileCollection.Filenames)
            {
                Console.WriteLine(fileName.ToString());
            }

            var lithologyWorkflowHandler = new CSVLithologyWorkflowHandler();
            lithologyWorkflowHandler.FileCollection = DescriptionFileCollection;
            lithologyWorkflowHandler.ExportDirectory = DescriptionFileCollection.ExportDirectory;
            var lithologyCache = lithologyWorkflowHandler.ImportCache();


            if (ProgramSettings.SendDataToDataBase)
            {
                    DescDBContext context = null;
                    try
                    {
                        context = new DescDBContext();
                        //context.Configuration.AutoDetectChangesEnabled = false;
                        
                        int count = 0;

                    var descriptionList = lithologyCache.GetCollection().Values.Where(x => !string.IsNullOrEmpty(x.LithologicID));

                    for (int i = 0; i < descriptionList.Count(); i=i+100)
                    {
                        var entityToInsert = descriptionList.Skip(i).Take(100).ToList();
                        count = 100;
                        context = AddToContext(context, entityToInsert, count, 100, true);
                    }
                        context.SaveChanges();
                    }
                    finally
                    {
                        if (context != null)
                            context.Dispose();
                    }
            }

            var measurementWorkFlowHandler = new CSVMeasurementWorkFlowHandler();
            var singleMeasurementFile = new FileCollection();



            //Theh Load Measurements

            foreach (string path in MeasurementFileCollection.Filenames)
            {
                Console.WriteLine("Processing measurement file: " + path);

                singleMeasurementFile.RemoveFiles();
                singleMeasurementFile.Filenames.Add(path);
               
                measurementWorkFlowHandler.FileCollection = singleMeasurementFile;
                var measurementCache = measurementWorkFlowHandler.ImportCache();
                var associatedMeasurementCache = measurementWorkFlowHandler.UpdateMeasurementCacheWithLithologicDescriptions(ref measurementCache, ref lithologyCache);

                if (ProgramSettings.SendDataToDataBase)
                {
                        DescDBContext context = null;
                        try
                        {
                            context = new DescDBContext();
                            //context.Configuration.AutoDetectChangesEnabled = false;

                        int count = 0;
                        var descriptionList = associatedMeasurementCache.GetCollection().Values.Where(x => x.LithologicSubID.HasValue);
                        for (int i = 0; i < descriptionList.Count(); i = i + 100)
                        {
                            var entityToInsert = descriptionList.Skip(i).Take(100);
                            count = 100;
                            context = AddToContext(context, entityToInsert, count, 100, true);
                        }

                        context.SaveChanges();
                        }
                        finally
                        {
                            if (context != null)
                                context.Dispose();
                        }

                }

                measurementWorkFlowHandler.ExportDirectory = MeasurementFileCollection.ExportDirectory;
                measurementWorkFlowHandler.ExportCache(associatedMeasurementCache);


                measurementCache.Dispose();
                measurementCache = null;
                associatedMeasurementCache.Dispose();
                associatedMeasurementCache = null;
            }


            

            Console.WriteLine("Finished processing measurement file at: " + DateTime.Now.ToString());
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
