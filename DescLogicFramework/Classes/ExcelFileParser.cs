using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DescLogicFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DescLogicFramework
{
    delegate void WriteRange(string s, Excel.Range rng);

    public class ExcelFileParser
    {
        public void ParseMyExcelFile(string excelFilePath /*, string csvOutputFile, int worksheetNumber*/)
        {
            List<object> ExcelObjectsToDispose = new List<object>();

            Excel.Application app = new Excel.Application();
            ExcelObjectsToDispose.Add(app);

            app.DisplayAlerts = false;
            app.Visible = false;


            //app.DisplayAlerts = false;
            Excel.Workbook wb = app.Workbooks.Open(excelFilePath,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing);
            ExcelObjectsToDispose.Add(wb);


            //Gets the root of the filename, less the extension
            string wbName = wb.Name.Split(".")[0];

            int i;
            string filepath = null;
            filepath = wb.Name;

            try
            {
                for (i = 1; i <= wb.Worksheets.Count; i++)
                {

                    Excel.Worksheet xlWorkSheet = wb.Worksheets[i];
                    xlWorkSheet.Activate();
                    ExcelObjectsToDispose.Add(xlWorkSheet);
                    string wkName = xlWorkSheet.Name;
                    Console.WriteLine("\r\n Processing Workbook: \r\n {0} \r\n Worksheet: \r\n {1}", wb.Name, xlWorkSheet.Name);

                    Excel.Range rngStart = xlWorkSheet.Cells[1, 1];
                    ExcelObjectsToDispose.Add(rngStart);

                    Excel.Range rngEnd = xlWorkSheet.Range["A1"].End[Excel.XlDirection.xlToRight];
                    ExcelObjectsToDispose.Add(rngEnd);

                    Excel.Range rngColumnHeaders = xlWorkSheet.Range[rngStart, rngEnd];
                    ExcelObjectsToDispose.Add(rngColumnHeaders);

                    //Procedure for preparing workbook with correct Depth Lookups
                    #region Find Depths


                    string json = File.ReadAllText(@"C:\Users\percuoco\source\repos\Test\Test\Classes\GlobalLists\DepthColumnNames.json");
                    //not sure why dynamic is used here
                    dynamic jsonDepths = JObject.Parse(json);
                    var derp = jsonDepths.Depths;
                   
                    List<string> listColumnNames = derp.ToObject<List<string>>();

                    
                    List<string> listMyMatches = FindColumns(rngColumnHeaders, listColumnNames);
                    if (listMyMatches.Count > 2)
                    {
                        throw new ColumnHeaderException("There are more than two matching Top/Bottom depth ranges on this worksheet");
                    }

                    //Rename depth column header. What if there are no depth columns?
                    Excel.Range rng = null;
                    switch (listMyMatches.Count)
                    {
                        //No Columns are present, add them in.
                        case 0:
                            xlWorkSheet.Range["A1"].EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                            xlWorkSheet.Range["A1"].Value2 = "Top Depth (m)";
                            xlWorkSheet.Range["A1"].EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                            xlWorkSheet.Range["A1"].Value2 = "Bottom Depth (m)";
                            break;
                        //One depth column present
                        case 1:
                            rng = rngColumnHeaders.Find(listMyMatches[0]);
                            rng.Value2 = listMyMatches[0].Contains("Top") ? "Top Depth (m)" : "Bottom Depth (m)";
                            xlWorkSheet.Range["A1"].EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                            xlWorkSheet.Range["A1"].Value2 = (rng.Value2 = "Top Depth (m)") ? "Bottom Depth (m)" : "Top Depth (m)";
                            break;
                        //Both are present, make sure they are correctly named
                        case 2:
                            foreach (string s in listMyMatches)
                            {
                                rng = rngColumnHeaders.Find(s);
                                rng.Value2 = s.Contains("Top") ? "Top Depth (m)" : "Bottom Depth (m)";
                            }
                            break;
                    }
                    #endregion

                    //Determines whether the sheet has certain hierarchy and depth columns, if not, creates them
                    #region Create Archive Columns
                    listColumnNames.Clear();
                    listMyMatches.Clear();


                    json = File.ReadAllText(@"C:\Users\percuoco\source\repos\Test\Test\Classes\GlobalLists\HierarchyColumnNames.json");
                    //not sure why dynamic is used here
                     jsonDepths = JObject.Parse(json);
                     derp = jsonDepths.Hierarchy;
                     listColumnNames = derp.ToObject<List<string>>();

                    listMyMatches = FindColumns(rngColumnHeaders, listColumnNames);

                    List<string> listColumnsToCreate = new List<string>(listColumnNames.Except(listMyMatches));

                    if (listColumnsToCreate.Count != 0)
                    {
                        //Inserts columns into the spreadsheet at renames the first cell in the column.
                        foreach (string s in listColumnNames)
                        {
                            xlWorkSheet.Range["A1"].EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                            xlWorkSheet.Range["A1"].Value2 = s;
                        }
                    }
                    else
                    {
                        Console.WriteLine("All the archive columns are already present on this worksheet");
                    }

                    #endregion

                    //Populate the hierarchal archive columns with parsed values from the Sample column.
                    #region Populate Archive Columns
                    try
                    {
                        //Redefine the ColumnHeaders range of Sample Values, as new columns were added
                        rngStart = xlWorkSheet.Cells[1, 1];
                        rngEnd = xlWorkSheet.Range["A1"].End[Excel.XlDirection.xlToRight];

                        rngColumnHeaders = xlWorkSheet.Range[rngStart, rngEnd];
                        int colSample = rngColumnHeaders.Find("Sample", Type.Missing, Type.Missing, Excel.XlLookAt.xlWhole).Column;


                        //Goes to Sample Column, returns range extending from A2 to the bottom of the worksheet, and then going upwards until a value is found.
                        rngStart = xlWorkSheet.Cells[2, colSample];

                        //If there is only 1 row then it is the header row, so set the lastrow to row 2, in which case there will be only 1 empty cell in my rngSampleInfo.
                        int lastrow = (xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row > 1) ? xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row : 2;
                        rngEnd = xlWorkSheet.Cells[lastrow, colSample];


                        Excel.Range rngSampleInfo = xlWorkSheet.Range[rngStart, rngEnd];
                        ExcelObjectsToDispose.Add(rngSampleInfo);

                        //Find hierarchy column indices
                        int colExpedition = (rngColumnHeaders.Find("Exp")).Column;
                        int colSite = rngColumnHeaders.Find("Site").Column;
                        int colHole = rngColumnHeaders.Find("Hole").Column;
                        int colCore = rngColumnHeaders.Find("Core").Column;
                        int colType = rngColumnHeaders.Find("Type").Column;
                        int colSection = rngColumnHeaders.Find("Section").Column;

                        int j = 1;
                        //Step through each value and parse the string into a struct, Assign values from the struct to hierarchy columns in the same row
                        if (rngSampleInfo.Count > 0)
                        {
                            for (j = 1; j <= rngSampleInfo.Rows.Count; j++)
                            {
                                if ((rngSampleInfo.Item[j] as Excel.Range).Value2 != null)
                                {
                                    HierarchyInfo h = new HierarchyInfo((rngSampleInfo.Item[j] as Excel.Range).Value2);

                                    WriteRange wr = delegate (string s, Excel.Range rng)
                                    {
                                        if (s == null)
                                        {
                                            rng.Value2 = "";
                                        }
                                        else
                                        {
                                            rng.Value2 = s;
                                        }
                                    };

                                    //Console.WriteLine("Expedition: {0}, Row: {1}; Column: {2}", h.Expedition, (rngSampleInfo.Item[j] as Excel.Range).Row, colExpedition);
                                    wr(h.Expedition, (xlWorkSheet.Cells[(rngSampleInfo.Item[j] as Excel.Range).Row, colExpedition]));
                                    wr(h.Site, (xlWorkSheet.Cells[(rngSampleInfo.Item[j] as Excel.Range).Row, colSite]));
                                    wr(h.Hole, (xlWorkSheet.Cells[(rngSampleInfo.Item[j] as Excel.Range).Row, colHole]));
                                    wr(h.Core, (xlWorkSheet.Cells[(rngSampleInfo.Item[j] as Excel.Range).Row, colCore]));
                                    wr(h.Type, (xlWorkSheet.Cells[(rngSampleInfo.Item[j] as Excel.Range).Row, colType]));
                                    wr(h.Section, (xlWorkSheet.Cells[(rngSampleInfo.Item[j] as Excel.Range).Row, colSection]));
                                }
                            }
                        }
                    }
                    catch (ColumnHeaderException e)
                    {
                        Console.WriteLine(e);
                    }

                    #endregion


                    //Save current workbook sheet as different .csv file, this part goes hand-in-hand with the sheet.Activate() method above.
                    wb.SaveAs(string.Format(@"C:\Users\percuoco\Desktop\IODP Backup\IODP Coding\PowerQuery Work\DESC Logic Data Query Prep For PowerBI\DESC_Queries_Formatted_Select_CSV_Correct\{0}___{1}.csv", wbName, wkName), Microsoft.Office.Interop.Excel.XlFileFormat.xlCSV, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange);
                    Console.WriteLine("\r\n Finished Processing Workbook: \r\n {0}, \r\n Worksheet: \r\n {1}", wb.Name, xlWorkSheet.Name);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine(e.StackTrace.ToString());
            }
            finally
            {
                wb.Close();
                app.Quit();

                foreach (var obj in ExcelObjectsToDispose)
                {
                    CleanUpComObjects(obj);
                }

                Console.WriteLine(string.Format("Cleaned up COM Resources!"));

            }

        }

        /// <summary>
        /// Parse the Sample Column to get Hierarchal Info
        /// </summary>
        /// <example>
        /// Examples: 351-U1438E-8R-4-W, 351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106, 352-U1442A-1R, 352-U1442A-1R-CC-A, 352-U1442A-4R-1-A
        /// </example>
        /// <returns>
        /// A hierarchy of Sample, Expedition, Site, Hole, Core, Type and Section.
        /// </returns>



        /// <summary>
        /// Searches an Excel Range for a series of provided strings
        /// </summary>
        /// <param name="rngSearchRange"></param>
        /// <param name="strSearchValues"></param>
        /// <returns> A list of strings</returns>
        public List<string> FindColumns(Excel.Range rngSearchRange, List<string> strSearchValues)
        {
            List<string> matchedValues = new List<string>();
            foreach (string s in rngSearchRange.Value2)
            {
                if (strSearchValues.Contains(s))
                {
                    matchedValues.Add(s);
                }
            }

            return matchedValues;
        }

        /// <summary>
        /// Decouples program from underlying com objects
        /// </summary>
        /// <param name="o">A Com object </param>
        public void CleanUpComObjects(object o)
        {
            Marshal.ReleaseComObject(o);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// My awesome custom exception based on certain properties of excel columns
        /// </summary>
        [Serializable]
        public class ColumnHeaderException : Exception
        {
            public ColumnHeaderException() { }
            public ColumnHeaderException(string message) : base(message) { }
            public ColumnHeaderException(string message, Exception inner) : base(message, inner) { }
            protected ColumnHeaderException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
