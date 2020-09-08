﻿using DescLogicFramework.DataAccess;
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
            ProgramSettings.ExportCachesToFiles = true;

            ProgramWorkFlowHandler p = new ProgramWorkFlowHandler();
            Console.WriteLine("Program finished at " + DateTime.Now.ToString(CultureInfo.CurrentCulture));

        }
    }


    static class ProgramSettings
    {
        public static bool SendDataToDataBase { get; set; } = false;
        public static bool ExportCachesToFiles { get; set; } = false;
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

                //Measurement files with Laurel's related description data:
                // measurementFileCollection.AddFiles(@"C:\Users\percuoco\Desktop\AGU2019\AGU2019\" + expedition + @"\output\track_data", "*.csv");
                //Referencing the original measurement files:
                measurementFileCollection.AddFiles(@"C:\Users\percuoco\Desktop\AGU2019\AGU2019\" + expedition + @"\data\measurements", "*.csv");
                measurementFileCollection.ExportDirectory = ConfigurationManager.AppSettings["ExportDirectory"] + expedition + @"\Measurements\";

                ProcessData(descriptionFileCollection, measurementFileCollection);

                this.OnBatchProcessCompleted(this, new BatchProcessCompleteEventArgs(expedition));
            }
            while (expedition != "E");


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
                var dbWorkflowHandler = new DatabaseWorkflowHandler();
                dbWorkflowHandler.SendLithologiesToDatabase(lithologyCache);
            }

            //Reconfigure Lithologys
            var LithCache = CacheReconfigurer.CreateDescriptionSearchHierarachy(lithologyCache);

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
                Console.WriteLine(string.Format("Processing {0} measurements", measurementCache.Count.ToString()));
                // var associatedMeasurementCache = measurementWorkFlowHandler.UpdateMeasurementCacheWithLithologicDescriptions(ref measurementCache, ref lithologyCache);
                var associatedMeasurementCache = measurementWorkFlowHandler.UpdateMeasurementCacheWithLithologicDescriptions(ref measurementCache, ref LithCache);
                if (ProgramSettings.SendDataToDataBase)
                {
                    var dbWorkflowHandler = new DatabaseWorkflowHandler();
                    dbWorkflowHandler.SendMeasurementsToDatabase(associatedMeasurementCache);
                }

                if (ProgramSettings.ExportCachesToFiles)
                {
                    measurementWorkFlowHandler.ExportDirectory = MeasurementFileCollection.ExportDirectory;
                    measurementWorkFlowHandler.ExportCache(associatedMeasurementCache);
                }

                measurementCache = null;
                associatedMeasurementCache = null;
                GC.Collect();
                

                Console.WriteLine("The total section count is: " + SectionCollection.Sections.Count);
                
            }
            #endregion

            Console.WriteLine("Finished processing measurement files at: " + DateTime.Now.ToString());
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
