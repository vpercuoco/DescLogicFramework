using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DescLogicFramework
{
    public static class StaticClassHoard
    {

        /// <summary>
        /// Takes in a Datatable and prints all the headers, rows to the console
        /// </summary>
        /// <param name="dt"></param>
       public static void PrintDataTableToConsole(DataTable dt)
        {
            string output = " ";
            foreach (DataColumn dataColumn in dt.Columns)
            {
                output = output + " " + dataColumn.ColumnName;
            }

            foreach (DataRow dr in dt.Rows)
            {
                output = " ";
                foreach (var item in dr.ItemArray)
                {
                    output = output + " " + item.ToString();
                }
                Console.WriteLine(output);
            }
        }
    }
}
