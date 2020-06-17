using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace DescLogicFramework
{
    /// <summary>
    /// Class used to create LithologicDescriptions objects from a DataTable.
    /// </summary>
    public class LithologyConvertor
    {
     
        /// <summary>
        /// Converts an IODPDataTable object into a collection of Lithologic Descriptions
        /// </summary>
        /// <param name="dt">The datatable to convert</param>
        /// <returns></returns>
        public Cache<string, LithologicDescription> Convert(IODPDataTable dt)
        {

            Cache<string, LithologicDescription> LithologyCache = new Cache<string, LithologicDescription>();

            int i = 0;
            LithologicIDGenerator IDGenerator = new LithologicIDGenerator();

            //Add a column in the datatable to ensure consistency between files with and without descriptions:
            dt.DataTable.Columns.Add("LithologicID_VP", typeof(string)).SetOrdinal(0);


            foreach (DataRow record in dt.DataTable.Rows)
            {
                record["LithologicID_VP"] = "-1";

                //Reject files with no datarows
                CinnamonList noDataEntriesList = new CinnamonList("NoSampleEntries");

                if (record.Table.Columns.Contains(dt.SampleID))
                {
                    if (noDataEntriesList.FindInList(record[dt.SampleID].ToString().ToLower()))
                    {
                        Console.WriteLine("There are no sample entries in this description file");
                        return LithologyCache;
                    }
                }
                else
                {
                    Console.WriteLine("Could not identify the Sample ID column in this file");
                    return LithologyCache;
                }


                LithologicDescription description = new LithologicDescription(record[dt.SampleID].ToString());

                //need to determine if the file is a Hole Summary, which means that there is no section identifier in the Sample Data field.
                if (description.SectionInfo.Section == null)
                {
                    Console.WriteLine("Could not identify the section numbers in this file");
                    return LithologyCache;
                }

                description.DataRow = record;
                double number = 0;


                if (record.Table.Columns.Contains(dt.OffsetIntervals[0]))
                {
                    if (double.TryParse(record[dt.OffsetIntervals[0]].ToString(), out number))
                    {
                        description.StartOffset.Offset = number;
                    }
                    else
                    {

                        description.StartOffset.Offset = -1;
                        Console.WriteLine("There are erroneous top offsets values in this file");
                        return LithologyCache;
                    }
                }
                else
                {
                    Console.WriteLine("Could not identify the top offset column in this file");
                    return LithologyCache;
                }


                if (record.Table.Columns.Contains(dt.OffsetIntervals[1]))
                {
                    if (double.TryParse(record[dt.OffsetIntervals[1]].ToString(), out number))
                    {
                        description.EndOffset.Offset = number;
                    }
                    else
                    {
                        description.EndOffset.Offset = -1;
                        Console.WriteLine("There are erroneous bottom offsets in this file");
                        return LithologyCache;
                    }
                }
                else
                {
                    Console.WriteLine("Could not identify the bottom offset column in this file");
                    return LithologyCache;
                }

     
                //Determine the Lithologic ID at this point
                IDGenerator.GenerateID(description);

                if (description.OffsetsSet())
                {
                    description.GenerateSubintervals();
                }

                description.DataRow["LithologicID_VP"] = description.LithologicID;
                LithologyCache.Add(description.LithologicID, description);

                i++;
            }
            return LithologyCache;
        }

    }
}
