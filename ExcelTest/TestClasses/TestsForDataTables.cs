using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DescLogicFramework;

namespace DescLogicFrameworkTests
{
    [TestClass]
    public class TestsForDataTables
    {
        [TestMethod]
        [Ignore]
        public void PrintDataTable()
        {
                /*
            string path = @"C:\Users\percuoco\Desktop\IODP Backup\IODP Coding\PowerQuery Work\DESC Logic Data Query Prep For PowerBI\DESC_Queries_Formatted_Select_CSV_Correct\346_description_U1422D___General.csv";
            CSVReader myConversion = new CSVReader(path);
            DataTable dt = myConversion.ImportDataTableFromCSV();
            StaticClassHoard.PrintDataTableToConsole(dt);
            */
        }

        [TestMethod]
        [Ignore]
        public void PrintPivotDataTable()
        {
          /*  string path = @"C:\Users\percuoco\Desktop\IODP Backup\IODP Coding\PowerQuery Work\DESC Logic Data Query Prep For PowerBI\DESC_Queries_Formatted_Select_CSV_Correct\346_description_U1422D___General.csv";
            CSVReader myConversion = new CSVReader(path);
            DataTable dt = myConversion.PivotDataTable(myConversion.ImportDataTableFromCSV()); */
        }

        [TestMethod]
        [Ignore]
        public void SaveDataTableToCSV()
        {
           /* 
            string inputPath = @"C:\Users\percuoco\Desktop\IODP Backup\IODP Coding\PowerQuery Work\DESC Logic Data Query Prep For PowerBI\DESC_Queries_Formatted_Select_CSV_Correct\346_description_U1422D___General.csv";
            string outputPath = @"C:\Users\percuoco\source\repos\Test\ExcelTests\TestClasses\TestFiles\DataTableConvertedToCSV.csv";
            CSVReader myConversion = new CSVReader(inputPath);
            DataTable dt = myConversion.PivotDataTable(myConversion.ImportDataTableFromCSV());
            myConversion.SaveDataTableToCSV(dt, outputPath);
           */
        }

    }
 
  
}
