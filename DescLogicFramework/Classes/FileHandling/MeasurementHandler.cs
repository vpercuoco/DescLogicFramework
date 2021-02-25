using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Data;
using Serilog;

namespace DescLogicFramework
{
    public static class MeasurementHandler
    {
        public static async Task<ICollection<Measurement>> GetMeasurementsFromFileAsync(string filename, [Optional] IntervalHierarchyNames columnIdentifiers)
        {
            columnIdentifiers = columnIdentifiers ?? new IntervalHierarchyNames()
            {

                Expedition = "Exp",
                Site = "Site",
                Hole = "Hole",
                Core = "Core",
                Type = "Type",
                Section = "Sect",
                Half = "A/W",
                TopOffset = "Offset (cm)",
                BottomOffset = "Offset (cm)",
                ArchiveTextID = "ArchiveSectionTextID_VP",
                WorkingTextID = "WorkingSectionTextID_VP",
                ParentTextID = "ParentSectionTextID_VP",
                SampleID = "Sample",
                TextID = "Text ID",
                TestNumber = "Test No.",

            };

            //TODO: need to get this some other way
            string InstrumentSystem =  Importer.GetFileNameWithoutExtension(filename);

            if (InstrumentSystem == "CARB" || InstrumentSystem == "ICP")
            {
                columnIdentifiers.TopOffset = "Top offset on section (cm)";
                columnIdentifiers.BottomOffset = "Bot offset on section (cm)";
            }

            IODPDataTable iODPDataTable = Importer.ImportDataTableFromFile(filename, columnIdentifiers);

            ICollection<Measurement> measurements = new HashSet<Measurement>();

            try
            {
                foreach (DataRow row in iODPDataTable.DataTable.Rows)
                {
                    IntervalHierarchyValues parsedValues = Importer.GetHierarchyValuesFromDataRow(row, columnIdentifiers);

                    Measurement measurement = new Measurement();
                    measurement.SectionInfo = new SectionInfo(parsedValues); //Creating a SectionInfo here that will be used to find the one stored in the DB.
                    measurement.DataRow = row;
                    measurement.InstrumentReport = "";
                    measurement.InstrumentSystem = InstrumentSystem;
                    measurement.TextID = parsedValues.TextID;
                    measurement.TestNumber = parsedValues.TestNumber;
                    measurement.StartOffset = double.TryParse(row[columnIdentifiers.TopOffset].ToString(), out double startOffset) ? startOffset : -1;
                    measurement.EndOffset = double.TryParse(row[columnIdentifiers.BottomOffset].ToString(), out double endOffset) ? endOffset : -1;

                    measurements.Add(measurement);
                }
            }
            catch (Exception)
            {

                throw new Exception("Error creating measurement from data row");
            }

            using (DescDBContext dbContext = new DescDBContext())
            {
                string[] expeditions = measurements.Select(x => x.SectionInfo.Expedition).Distinct().ToArray();

                ICollection<SectionInfo> sections;
                try
                {
                    sections = await DatabaseWorkflowHandler.GetAllSectionsFromDatabaseForExpeditionAsync(dbContext, expeditions);

                }
                catch (Exception)
                {

                    throw new Exception("Could not get sections from the database");
                }


                foreach (var measurement in measurements)
                {
                    measurement.SectionInfo = DatabaseWorkflowHandler.GetSectionInfoFromCollection(sections, measurement.SectionInfo);
                }

                return measurements;

            }
        }

        public static async Task<ICollection<LithologicDescription>> GetDescriptionsWithinMeasurementIntervalAsync(DescDBContext dbContext, Measurement measurement, [Optional] ICollection<string> columnNames)
        {
            measurement.SectionInfo = measurement.SectionInfo ?? throw new Exception("Measurement does not have Section information");

 
            SectionInfo section = dbContext.Sections.Where(x => measurement.SectionInfo.Equals(x))
                                                    .FirstOrDefault();
            if (section == null)
            {
                return new HashSet<LithologicDescription>();
            }

            var x = dbContext.LithologicDescriptions
                                    .Where(description => description.SectionInfo.Equals(measurement.SectionInfo))
                                    .Where(description => description.StartOffset <= measurement.StartOffset && description.EndOffset >= measurement.EndOffset);

          
            if (columnNames.Any())
            {
              x = x.Include(description => description.DescriptionColumnValues.Where(columnValuPair=>columnNames.Contains(columnValuPair.ColumnName))        
                                       );
            }
    

            return x.Select(g => g)
                    .ToHashSet();
        }

        public static async Task<Measurement> GetDescriptionForMeasurementAsync(DescDBContext dbContext, Measurement measurement, [Optional] ICollection<string> columnNames)
        {
            measurement.SectionInfo = measurement.SectionInfo ?? throw new Exception("Measurement does not have Section information");


            SectionInfo section = await dbContext.Sections.Where(x => measurement.SectionInfo.Equals(x))
                                                    .FirstOrDefaultAsync().ConfigureAwait(true);
            if (section == null)
            {
                return measurement;
            }

            var x = dbContext.LithologicDescriptions
                                    .Where(description => description.SectionInfo.Equals(measurement.SectionInfo))
                                    .Where(description => description.StartOffset <= measurement.StartOffset && description.EndOffset >= measurement.EndOffset);


            if (columnNames.Any())
            {
                x = x.Include(description => description.DescriptionColumnValues.Where(columnValuPair => columnNames.Contains(columnValuPair.ColumnName))
                                         );
            }
            measurement.Descriptions = x.Select(g => g).ToHashSet();

            return measurement;
        }

        public static async Task<ICollection<LithologicDescription>[]> GetDescriptionsWithinMeasurementIntervalAsync(DescDBContext dBContext, IEnumerable<Measurement> measurements, [Optional] ICollection<string> columnNames)
        {

            var userTasks = new List<Task<ICollection<LithologicDescription>>>();

            foreach (var measurement in measurements)
            {
                userTasks.Add(GetDescriptionsWithinMeasurementIntervalAsync(dBContext, measurement, columnNames));
            }

            return await Task.WhenAll(userTasks);
        }

        public static async Task<ICollection<Measurement>> GetDescriptionsForMeasurementAsync(DescDBContext dBContext, IEnumerable<Measurement> measurements, [Optional] ICollection<string> columnNames)
        {

            var userTasks = new List<Task<Measurement>>();

            foreach (var measurement in measurements)
            {
                userTasks.Add(GetDescriptionForMeasurementAsync(dBContext, measurement, columnNames));
            }

            return await Task.WhenAll(userTasks);
        }


       

        /*
        public static async Task<ICollection<LithologicDescription>[]> GetDescriptionsColumnValuesAsync(DescDBContext dbContext, IEnumerable<Measurement> measurements, ICollection<string> columns)
        {
            foreach (var measurement in measurements)
            {
                dbContext.LithologicDescriptions
                        .Where(description => description.SectionInfo.Equals(measurement))
                        .Include(x => x.DescriptionColumnValues
                                      .Where(y => columns.Contains(y.ColumnName))
                                      .Select(x => x)
                                )
                        .Select(x => x);

                dbContext.find
                                      
    
            }

        }
        */
    }
}
