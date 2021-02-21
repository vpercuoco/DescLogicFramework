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

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(ConfigurationManager.AppSettings["LogFileLocation"])
                .CreateLogger();


            //            Examples.GetMeasurementsFromFileThenGetCertainDescriptionColumns();


            var x = Task.Run(async () => await SubintervalCreator.GetLithologicSubIntervalsForMeasurements());
            x.Wait();

           // DatabaseWorkflowHandler.AttemptAtSQLRaw();


           // var list = new CinnamonList("ExpeditionList").Parameters;
           // List<string> expeditions = new List<string>();
           // foreach (var item in list)
           // {
           //    expeditions.Add( item.Remove(0, 1));
           // }
           // expeditions.Reverse();

           //// expeditions.Remove("330");
           // //expeditions.Remove("323");
           // //expeditions.Remove("321");
           // //  AddAllDescriptionsToDatabase();
           // foreach (var item in expeditions)
           // {
           //     Console.WriteLine(string.Format("Creating Subintervals for {0}",item.ToString()));

           //     var xx = Task.Run(async () => await Workflow.EnsureAllLithologicDescriptionsHaveSubintervals(item.ToString()));
           //     xx.Wait();
           // }


            Log.CloseAndFlush();


           // // var myTasks = new List<Task<bool>>();

           // // var x = Task.Run(async () => await SubintervalCreator.EnsureAllLithologicDescriptionsHaveSubintervals());
           // //  var y = Task.Run(async () => await SubintervalCreator.EnsureAllLithologicDescriptionsHaveSubintervals());
           // // var z = Task.Run(async () => await SubintervalCreator.EnsureAllLithologicDescriptionsHaveSubintervals());

           // // myTasks.Add(x);
           // // myTasks.Add(y);
           // // myTasks.Add(z);

           // // var t =  Task.Run(async () => await Task.WhenAll(myTasks));
           // // t.Wait();
           // // var cc = t.Result;

           // var x = Task.Run(async () => await SubintervalCreator.GetDescriptionsForMeasurment("MAD", new List<string>() {"Expedition_VP" }).ConfigureAwait(true));

           //// var x = Task.Run(async () => await SubintervalCreator.GetLithologicSubIntervalsForMeasurements().ConfigureAwait(true));
           // x.Wait();
            
            
   



            Console.WriteLine("Starting up at " + DateTime.Now.ToString());

            var conn = ConfigurationManager.ConnectionStrings["DBconnection"];

            ProgramSettings.SendDataToDESCDataBase = false;

            ProgramSettings.SendDataToLithologyDataBase = false;

            ProgramSettings.ExportCachesToFiles = false;

            ProgramSettings.ProcessMeaurements = false;

            _ = new ProgramWorkFlowHandler();
            Console.WriteLine("Program finished at " + DateTime.Now.ToString(CultureInfo.CurrentCulture));

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
