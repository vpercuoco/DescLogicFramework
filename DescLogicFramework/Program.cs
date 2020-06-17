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


namespace DescLogicFramework
{
    public class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Starting up at " + DateTime.Now.ToString());
            
         /*   var configfile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
           

            
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("connectionStrings");
            var s1 = ConfigurationManager.AppSettings;
            var s2 = ConfigurationManager.ConnectionStrings;
            */

            var conn = ConfigurationManager.ConnectionStrings["DBconnection"];



            //  var s3 = s1["MySetting"];
           // ServiceContainer sc = new ServiceContainer();
           // sc.a
            ProgramSettings.SendDataToDataBase = false;

            //var x = new TestServices(new ILogger());
            ProgramWorkFlowHandler p = new ProgramWorkFlowHandler();
            Console.WriteLine("Program finished at " + DateTime.Now.ToString());
        }


        //I am just going to test to see if I can add in services through dependency injection similar to ASP.NET
       /*
        *public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger, Logger>();

        }
        */
    }

    /*
    public interface ILogger
    {
        public void Log(string message);
    }
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("The Logger service is working properly");
        }
            
    }
    public class TestServices
    {
        private readonly ILogger _logger;
        public TestServices(ILogger logger)
        {
            _logger = logger;
        }
    }

    */

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
                FileSegregator descriptionFileCollection = new FileSegregator();

                descriptionFileCollection.AddFiles(@"C:\Users\percuoco\Desktop\AGU2019\AGU2019\" + expedition + @"\output\extracted_csv", "*.csv");
                descriptionFileCollection.ExportDirectory = ConfigurationManager.AppSettings["ExportDirectory"] + expedition + @"\Lithology\";
                descriptionFileCollection.Filenames.RemoveAll(x => !x.ToLower().Contains("sediment_macroscopic"));

                FileSegregator measurementFileCollection = new FileSegregator();

                measurementFileCollection.AddFiles(@"C:\Users\percuoco\Desktop\AGU2019\AGU2019\" + expedition + @"\output\track_data", "*.csv");
                measurementFileCollection.ExportDirectory = ConfigurationManager.AppSettings["ExportDirectory"] + expedition + @"\Measurements\";

                ProcessData(descriptionFileCollection, measurementFileCollection);

                this.OnBatchProcessCompleted(this, new BatchProcessCompleteEventArgs(expedition));
            }
            while (expedition != "E") ;
        }
        public ProgramWorkFlowHandler(FileSegregator DescriptionFileCollection, FileSegregator MeasurementFileCollection)
        {
            if (DescriptionFileCollection.ExportDirectory != null && MeasurementFileCollection.ExportDirectory != null )
            {
                ProcessData(DescriptionFileCollection, MeasurementFileCollection);
            }
        }
       
        private void ProcessData( FileSegregator DescriptionFileCollection, FileSegregator MeasurementFileCollection)
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
                (new DBLithologyWorkFlowHandler()).SendDataTableToDatabase(lithologyCache);
            }

            var measurementWorkFlowHandler = new CSVMeasurementWorkFlowHandler();
            var singleMeasurementFile = new FileSegregator();

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
                     (new DBMeasurementWorkFlowHandler()).SendDataTableToDatabase(associatedMeasurementCache);
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
