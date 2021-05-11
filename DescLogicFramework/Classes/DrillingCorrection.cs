using DescLogicFramework.DataAccess;
using Serilog;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace DescLogicFramework
{

    /// <summary>
    /// This class is a amalgamation of spaghetti to correct drilling disturbance files. It is a one-off thing.
    /// </summary>
    public static class DrillingCorrection
    {

        public static void ChangeColumn(DataTable data, string columnName, string newColumnName)
        {
            /*
            if (data == null || string.IsNullOrEmpty(columnName))
            {
                return false;
            }
            */

            foreach (DataColumn column in data.Columns)
            {
                if (columnName.Equals(column.ColumnName))
                {
                    
                    column.ColumnName = newColumnName;
                }
                data.AcceptChanges();
                 
            }
        }


        public static List<DrillingDisturbanceRecord> FormatDrillingDisturbanceFile(string filename, string exportFilename)
        {
            Log.Information("--------Parsing a new file--------");
            DataTable sections = SectionInfoCollection.ImportAllSections(ConfigurationManager.AppSettings["AllSectionsFile"]);
            ICollection<DrillingDisturbanceRecord> descriptions = new HashSet<DrillingDisturbanceRecord>();
            List<DrillingDisturbanceRecord> FinalDescriptionsToAdd = new List<DrillingDisturbanceRecord>();

            #region ImportDrillingDisturbances
            var dataTableReader = new CSVReader();
            dataTableReader.ReadPath = filename;
            DataTable drillingDisturbances = dataTableReader.Read();


            //Correct Column Names:

            ChangeColumn(drillingDisturbances, "Drilling disturbance intensity [rank]", "Drilling disturbance intensity rank");
            ChangeColumn(drillingDisturbances, "Drilling disturbance intensity rank(read only)", "Drilling disturbance intensity rank");
            ChangeColumn(drillingDisturbances, "Drilling disturbance intensity rank (read only)", "Drilling disturbance intensity rank");

            ChangeColumn(drillingDisturbances, "Label ID", "Sample");

            ChangeColumn(drillingDisturbances, "Top depth [m]", "Top Depth [m]");
            ChangeColumn(drillingDisturbances, "Bottom depth [m]", "Bottom Depth [m]");

            ChangeColumn(drillingDisturbances, "Disturbance [name]", "Disturbance");

            ChangeColumn(drillingDisturbances, "File data", "File Data");

            //Add additional columns
            if (!drillingDisturbances.Columns.Contains("Drilling disturbance comment"))
            {
                drillingDisturbances.Columns.Add("Drilling disturbance comment");
            }
            if (!drillingDisturbances.Columns.Contains("Drilling disturbance type"))
            {
                drillingDisturbances.Columns.Add("Drilling disturbance type");
            }
            if (!drillingDisturbances.Columns.Contains("Drilling disturbance intensity"))
            {
                drillingDisturbances.Columns.Add("Drilling disturbance intensity");
            }
            if (!drillingDisturbances.Columns.Contains("Drilling disturbance intensity rank"))
            {
                drillingDisturbances.Columns.Add("Drilling disturbance intensity rank");
            }

            try
            {
                //Collection of all drilling disturbances
                foreach (DataRow row in drillingDisturbances.Rows)
                {
                    DrillingDisturbanceRecord record = new DrillingDisturbanceRecord()
                    {
                        Column1 = row["Column1"].ToString(),
                        SampleID = row["Sample"].ToString(),
                        Top_cm = row["Top [cm]"].ToString(),
                        Bottom_cm = row["Bottom [cm]"].ToString(),
                        TopDepth_m = row["Top Depth [m]"].ToString(),
                        BottomDepth_m = row["Bottom Depth [m]"].ToString(),
                        DrillingDisturbanceType = row["Drilling disturbance type"].ToString(),
                        DrillingDisturbanceIntensity = row["Drilling disturbance intensity"].ToString(),
                        DrillingDisturbanceIntensityRank = row["Drilling disturbance intensity rank"].ToString(),
                        DrillingDisturbanceComment = row["Drilling disturbance comment"].ToString(),
                        // ShipFileLinks = row["Ship File Links"].ToString(),
                        // ShoreFileLinks = row["Shore File Links"].ToString(),
                        FileData = row["File Data"].ToString()
                    };
                    descriptions.Add(record);
                }

            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                Log.Warning($"Could not created disturbance records from {filename}");
                return FinalDescriptionsToAdd;
            }

            #endregion


            #region GetTheSectionsInCoreDescription
            foreach (var description in descriptions)
            {
                //Find rows where sample Id doesn't end in A OR W
                if (description.SampleID.EndsWith("A") || description.SampleID.EndsWith("W"))
                {
                    FinalDescriptionsToAdd.Add(description);
                    continue;
                }

                Log.Information($"{description.SampleID} is a description on the Core");
                Log.Information($"{description.SampleID} TOP OFFSET: {description.Top_cm} BOTTOM OFFSET: {description.Bottom_cm} TOPDEPTH: {description.TopDepth_m} BOTTOM DEPTH: {description.BottomDepth_m}");

                //At this point the description should be of the entire Core;
                //Parse Core information from the Sample:
                SectionInfo coreInfo = new SectionInfo(description.SampleID);

                //Find all the sections within the AllSectionsTable which overlap with the Top/Bottom offsets of the Core description
                var constituentSections = sections.AsEnumerable()
                    .Where(x => x.Field<string>("Exp") == coreInfo.Expedition)
                    .Where(x => x.Field<string>("Site") == coreInfo.Site)
                    .Where(x => x.Field<string>("Hole") == coreInfo.Hole)
                    .Where(x => x.Field<string>("Core") == coreInfo.Core)
                    .Where(x => x.Field<string>("Type") == coreInfo.Type)
                    .Where(x =>

                    (x.Field<string>("Top depth CSF-A (m)").ToDouble() >= description.TopDepth_m.ToDouble() && x.Field<string>("Top depth CSF-A (m)").ToDouble() < description.BottomDepth_m.ToDouble())
                    || (x.Field<string>("Bottom depth CSF-A (m)").ToDouble() > description.TopDepth_m.ToDouble() && x.Field<string>("Bottom depth CSF-A (m)").ToDouble() <= description.BottomDepth_m.ToDouble())
                    )
                        .Select(x => x)
                        .ToHashSet(); //collection of datarows...

                //Create new drilling disturbance records by mashing up data between original Disturbances and sections:
                //Need to relook at this: Use only the top and bottom offsets from the Sample
                //Find the sections from the allsections table which overlap those intervals
                //Create new drilling disturbances with section information
                HashSet<DrillingDisturbanceRecord> newDrillingRecords = new HashSet<DrillingDisturbanceRecord>();
                foreach (var section in constituentSections)
                {
                    //Create new sampleID, All of them will be on the Archive half
                    string newSampleID = string.Format("{0}-{1}{2}-{3}{4}-{5}-A",
                        section["Exp"],
                        section["Site"],
                        section["Hole"],
                        section["Core"],
                        section["Type"],
                        section["Sect"]);

                    //Create a new drilling disturbance record
                    var record = new DrillingDisturbanceRecord
                    {
                        Column1 = description.Column1,
                        SampleID = newSampleID,
                        Top_cm = "0", //section["Top Offset (cm)"].ToString(),
                        Bottom_cm = Math.Round((section["Curated length (m)"].ToString().ToDouble() * 100), 2).ToString(),
                        TopDepth_m = section["Top depth CSF-A (m)"].ToString(),
                        BottomDepth_m = section["Bottom depth CSF-A (m)"].ToString(),
                        DrillingDisturbanceType = description.DrillingDisturbanceType,
                        DrillingDisturbanceIntensity = description.DrillingDisturbanceIntensityRank,
                        DrillingDisturbanceComment = description.DrillingDisturbanceComment,
                        DrillingDisturbanceIntensityRank = description.DrillingDisturbanceIntensityRank,
                        ShipFileLinks = description.ShipFileLinks,
                        ShoreFileLinks = description.ShoreFileLinks,
                        FileData = description.FileData

                    };


                    newDrillingRecords.Add(record);
                    Log.Information($"{newSampleID}: Section Added: TOP OFFSET: {record.Top_cm} BOTTOM OFFSET: {record.Bottom_cm} TOP DEPTH: {record.TopDepth_m} BOTTOM DEPTH: {record.BottomDepth_m}");

                }
                #endregion region


                #region SetTheOffsetsForBorderingSections
                //Set the TOP/BOTTOM offsets for core description's first and last sections to be equal to the core descriptions TOP/BOTTOM Offsets
                var topSection = newDrillingRecords.OrderBy(x => x.TopDepth_m).First();
                var topCorrection = topSection.Top_cm.ToDouble() + (description.TopDepth_m.ToDouble() - topSection.TopDepth_m.ToDouble()) * 100;
                topCorrection = Math.Round(topCorrection, 2);
                topSection.Top_cm = topCorrection.ToString(); //Need to calculate offsets based on depths
                topSection.TopDepth_m = description.TopDepth_m;

                Log.Information($"{topSection.SampleID}: Changed TopDepth to {topSection.TopDepth_m} and TopOffset to {topSection.Top_cm}");

                var bottomSection = newDrillingRecords.OrderBy(x => x.BottomDepth_m).Last();
                var bottomCorrection = bottomSection.Bottom_cm.ToDouble() - (bottomSection.BottomDepth_m.ToDouble() - description.BottomDepth_m.ToDouble()) * 100;
                bottomCorrection = Math.Round(bottomCorrection, 2);
                bottomSection.Bottom_cm = bottomCorrection.ToString();
                bottomSection.BottomDepth_m = description.BottomDepth_m;

                #endregion
                Log.Information($"{bottomSection.SampleID}: Changed BottomDepth to {bottomSection.BottomDepth_m} and BottomOffset to {bottomSection.Bottom_cm}");

                foreach (var newSectionDescription in newDrillingRecords)
                {
                    //Find all descriptions the core describer's made for this SECTION. The sample ID's should be equal
                    var describedIntervalsOnSection = descriptions.Where(x => x.SampleID == newSectionDescription.SampleID).ToHashSet();



                    //If they described any intervals, pass those intervals into the algo to correctly process gap intervals
                    if (describedIntervalsOnSection.Any())
                    {

                        ICollection<DrillingDisturbanceRecord> finalRecords = CoreToSectionAlgo(newSectionDescription, describedIntervalsOnSection);
                        FinalDescriptionsToAdd.AddRange(finalRecords);

                        foreach (var record in finalRecords)
                        {
                            Log.Information($"{newSectionDescription.SampleID}: Adding to final descriptions");
                        }

                    }
                    else if (!describedIntervalsOnSection.Any())
                    {
                        FinalDescriptionsToAdd.Add(newSectionDescription);
                        Log.Information($"{newSectionDescription.SampleID}: Adding to final descriptions");
                    }

                }
            }

            //These are new descriptions to add to the file
            foreach (var record in FinalDescriptionsToAdd)
            {
                var offsetDifference = Math.Round(record.Bottom_cm.ToDouble() - record.Top_cm.ToDouble(), 2);
                var depthDifference = Math.Round((record.BottomDepth_m.ToDouble() - record.TopDepth_m.ToDouble()) * 100, 2);

                if (offsetDifference != depthDifference)
                {
                    Log.Warning($"Error in Offsets: {record.SampleID}: TOP OFFSET: {record.Top_cm} BOTTOMOFFSET: {record.Bottom_cm} TOPDEPTH: {record.TopDepth_m} BOTTOMDEPTH: {record.BottomDepth_m}");
                }
                else
                {
                    Log.Information($"{record.SampleID}: TOP OFFSET: {record.Top_cm} BOTTOMOFFSET: {record.Bottom_cm} TOPDEPTH: {record.TopDepth_m} BOTTOMDEPTH: {record.BottomDepth_m}");
                }

            }


            DataTable dt = new DataTable();
           // DataColumn[] columns = new DataColumn[drillingDisturbances.Columns.Count];
           // drillingDisturbances.Columns.CopyTo(columns, 0);
           // dt.Columns.AddRange(columns);

            for (int i = 0; i < drillingDisturbances.Columns.Count; i++)
            {
                dt.Columns.Add(drillingDisturbances.Columns[i].ColumnName, drillingDisturbances.Columns[i].DataType) ;
            }

            //Add in new corrected drilling disturbances
            foreach (var item in FinalDescriptionsToAdd)
            {
                DataRow row = dt.NewRow();
                //row.BeginEdit();
                row["Column1"] = item.Column1;
                row["Sample"] = item.SampleID;
                row["Top [cm]"] = item.Top_cm;
                row["Bottom [cm]"] = item.Bottom_cm;
                row["Top Depth [m]"] = item.TopDepth_m;
                row["Bottom Depth [m]"] = item.BottomDepth_m;
                row["Drilling disturbance type"] = item.DrillingDisturbanceType;
                row["Drilling disturbance intensity"] = item.DrillingDisturbanceIntensity;
                row["Drilling disturbance intensity rank"] = item.DrillingDisturbanceIntensityRank;
                row["Drilling disturbance comment"] = item.DrillingDisturbanceComment;
                //row["Ship File Links"] = item.ShipFileLinks;
                //row["Shore File Links"] = item.ShoreFileLinks;
                row["File Data"] = item.FileData;

                dt.Rows.Add(row);
                //row.EndEdit();
            }

            Importer.ExportDataTableAsNewFile(exportFilename, dt);
            return FinalDescriptionsToAdd;
        }

        public class DrillingDisturbanceRecord
        {
            public string Column1 { get; set; }
            public string SampleID { get; set; }
            public string Top_cm { get; set; }
            public string Bottom_cm { get; set; }
            public string TopDepth_m { get; set; }
            public string BottomDepth_m { get; set; }
            public string DrillingDisturbanceType { get; set; }
            public string DrillingDisturbanceIntensity { get; set; }
            public string DrillingDisturbanceIntensityRank { get; set; }
            public string DrillingDisturbanceComment { get; set; }
            public string ShipFileLinks { get; set; }
            public string ShoreFileLinks { get; set; }
            public string FileData { get; set; }
        }



        //This should be one section passed in, and described intervals on the same section. So they would have the same SampleID
        private static ICollection<DrillingDisturbanceRecord> CoreToSectionAlgo(DrillingDisturbanceRecord newSectionRecord, ICollection<DrillingDisturbanceRecord> describedSectionIntervals)
        {

            double seedTop = newSectionRecord.Top_cm.ToDouble();
            double seedBottom = newSectionRecord.Bottom_cm.ToDouble();

            //Check if there are any describedSectionIntervals that overlap the entire newSectionRecord:
            var match = describedSectionIntervals.Where(d => d.Top_cm == newSectionRecord.Top_cm && d.Bottom_cm == newSectionRecord.Bottom_cm).FirstOrDefault();
            var gapIntervalsDescriptions = new List<DrillingDisturbanceRecord>();


            //This entire section has been described, so do not add in my artifical section description
            if (match != null)
            {
                return describedSectionIntervals;
            }

            //There are described intervals on the section which only take up part of the section. I need to create artifical section descriptions to fill in the gaps.
            //Find described intervals on the section which overlap the new section description I've created, return the one with the greatest depth
            // var intervalDescription = describedSectionIntervals.Where(d => d.Top_cm.ToDouble() <= seedTop && d.Bottom_cm.ToDouble() > seedTop).OrderBy(d => d.Bottom_cm.ToDouble()).LastOrDefault();
            foreach (var intervalDescription in describedSectionIntervals.OrderBy(d => d.TopDepth_m).Select(x => x))
            {
                //Interval has been described, change the TOP reference and go again.
                if (intervalDescription.Top_cm.ToDouble() <= seedTop && intervalDescription.Bottom_cm.ToDouble() > seedTop)
                {
                    seedTop = intervalDescription.Bottom_cm.ToDouble();
                    continue;
                }
                else if (intervalDescription.Top_cm.ToDouble() > seedTop)
                {
                    //Create a new drilling description to account for this interval;
                    //Remember these fields are from the CORE description, aside from the TOP/BOTTOM offsets and depths
                    DrillingDisturbanceRecord gapInterval = new DrillingDisturbanceRecord
                    {
                        Column1 = newSectionRecord.Column1,
                        SampleID = newSectionRecord.SampleID,
                        Top_cm = seedTop.ToString(), //Gap interval goes from seedTop to the top of the next shallowest core description interval
                        Bottom_cm = intervalDescription.Top_cm, //This is correct

                        TopDepth_m = (intervalDescription.TopDepth_m.ToDouble() - (intervalDescription.Top_cm.ToDouble() - seedTop) / 100).ToString(), //Both the top depth and bottom depth need to be adjusted....
                        BottomDepth_m = intervalDescription.TopDepth_m,

                        DrillingDisturbanceType = newSectionRecord.DrillingDisturbanceType,
                        DrillingDisturbanceIntensity = newSectionRecord.DrillingDisturbanceIntensity,
                        DrillingDisturbanceComment = newSectionRecord.DrillingDisturbanceComment,
                        DrillingDisturbanceIntensityRank = newSectionRecord.DrillingDisturbanceIntensityRank,
                        ShipFileLinks = newSectionRecord.ShipFileLinks,
                        ShoreFileLinks = newSectionRecord.ShoreFileLinks,
                        FileData = newSectionRecord.FileData
                    };

                    gapIntervalsDescriptions.Add(gapInterval);

                    seedTop = intervalDescription.Bottom_cm.ToDouble(); //Set the new seed top to be at the bottom of the last described interval
                }

            }

            return gapIntervalsDescriptions;

        }
    }
}
