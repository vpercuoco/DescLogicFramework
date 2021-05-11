using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Data;
using Serilog;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace DescLogicFramework
{
    public static class DatabaseWorkflowHandler
    {
        #region DatabaseQuerying
        /// <summary>
        /// Asynchronously returns all of the SectionInfos from the database for a given list of expedition numbers
        /// </summary>
        /// <param name="dBContext">A DESCDatabase context</param>
        /// <param name="expeditions">AN enumerable of expedition strings</param>
        /// <returns>A collection of SectionInfos</returns>
        public static async Task<ICollection<SectionInfo>> GetAllSectionsFromDatabaseForExpeditionAsync(DescDBContext dBContext, IEnumerable<string> expeditions)
        {
            return await Task.Run(() => dBContext.Sections.Where(record => expeditions.Contains(record.Expedition))
                                     .OrderBy(x => x.Expedition)
                                     .ThenBy(x=>x.Site)
                                     .ThenBy(x => x.Hole)
                                     .ThenBy(x => x.Core)
                                     .ThenBy(x => x.Type)
                                     .ThenBy(x => x.Section).ToHashSet()).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously gets a SectionInfo from the databse for a given measurement or lithologic description interval
        /// </summary>
        /// <param name="dbContext">A DESCDatabase context</param>
        /// <param name="interval">An interval</param>
        /// <returns>A single SectionInfo</returns>
        public static async Task<SectionInfo> GetSectionInfoFromDatabaseForIntervalAsync(DescDBContext dbContext, SectionInfo section)
        {
            return await dbContext.Sections.Where(record =>
                              record.Expedition == section.Expedition &&
                              record.Site == section.Site &&
                              record.Hole == section.Hole &&
                              record.Core == section.Core &&
                              record.Type == section.Type &&
                              record.Section == section.Section).FirstOrDefaultAsync().ConfigureAwait(true);
        }

        public static SectionInfo GetSectionInfoFromCollection(ICollection<SectionInfo> sections, SectionInfo section)
        {
            return sections.Where(record => section.Equals(record)).FirstOrDefault();
        }

        private static DescDBContext AddToContext<TEntity>(DescDBContext context, IEnumerable<TEntity> entities, int count, int commitCount, bool recreateContext) where TEntity : class
        {
            context.AddRange(entities);


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

        public static async Task<bool> FindMeasurementInDatabase(DescDBContext dbContext, Measurement measurement)
        {

            //TODO: Throw an error if the measurement doesn't have a sectioninfo ID

            bool recordExists = await dbContext.MeasurementDescriptions
                   .Where(x => x.SectionInfo.ID == measurement.SectionInfo.ID)
                   .Where(x => x.TextID == measurement.TextID)
                   .Where(x => x.TestNumber == measurement.TestNumber)
                   .Where(x => x.StartOffset == measurement.StartOffset && x.EndOffset == measurement.EndOffset)
                   .AnyAsync().ConfigureAwait(true);


            if (recordExists)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static async Task<bool> CheckForDescriptionAsync(DescDBContext dBContext, LithologicDescription description)
        {
            bool recordExists = await dBContext.LithologicDescriptions
                .Where(x => x.SectionInfo.ID == description.SectionInfo.ID)
                .Where(x=>x.DescriptionReport == description.DescriptionReport)  //Report names may change depending on how we name the files, 
                .Where(x => x.StartOffset == description.StartOffset && x.EndOffset == description.EndOffset)
                .AnyAsync().ConfigureAwait(true);

            return recordExists;
        }

        public static async Task<bool> CheckForSectionAsync(DescDBContext dBContext, SectionInfo section)
        {
            bool recordExists;

            if (section.ID != 0)
            {
                recordExists = await dBContext.Sections
                .Where(x => x.ID == section.ID)
                .AnyAsync().ConfigureAwait(true);
            }
            else if (section.ParentTextID != "")
            {
                 recordExists = await dBContext.Sections
                    .Where(x => x.ParentTextID  == section.ParentTextID)
                    .AnyAsync().ConfigureAwait(true);
            }
            else if (section.ID == 0)
            {
                recordExists = await dBContext.Sections
                 .Where(x => section.Equals(x))
                 .AnyAsync().ConfigureAwait(true);
            }
            else
            {
                return false;
            }

            return recordExists;
        }

        public static async Task<int> GetMeasurementIDAsync(DescDBContext dbContext, Measurement measurement)
        {
            var record = await dbContext.MeasurementDescriptions
          .Where(x => x.SectionInfo.ID == measurement.SectionInfo.ID)
          .Where(x => x.TextID == measurement.TextID)
          .Where(x => x.TestNumber == measurement.TestNumber)
          .Where(x=>x.InstrumentSystem == measurement.InstrumentSystem)
          .Where(x => x.StartOffset == measurement.StartOffset && x.EndOffset == measurement.EndOffset)
          .FirstOrDefaultAsync().ConfigureAwait(true);

            if (record != null)
            {
                return record.ID;
            }
            else
            {
                return -1;
            }
            

        }

        #endregion


        #region SQLRawQueries


        public static void AttemptAtSQLRaw()
        {
            List<LithologicDescription> IrregularOffsets = new List<LithologicDescription>();

            using (DescDBContext dbContext = new DescDBContext())
            {
               IrregularOffsets = dbContext.LithologicDescriptions
                    .FromSqlRaw(@"SELECT * FROM [DESCDatabase].[dbo].[LithologicDescriptions] t1 WHERE t1.StartOffset > t1.EndOffset")
                    .ToList();
            }

        }
        #endregion



        #region AddingDataToDatabase
        public static async Task<ICollection<SectionInfo>> GetSectionsFromFileAsync(string fileName, [Optional] IntervalHierarchyNames columnNames)
        {
            SectionInfoCollection sectionCollection = new SectionInfoCollection();

            columnNames = columnNames ?? new IntervalHierarchyNames 
            { 
                Expedition = "Exp", 
                Site = "Site", 
                Hole = "Hole", 
                Core = "Core", 
                Type = "Type", 
                Section = "Sect", 
                ParentTextID = "Text ID of section", 
                ArchiveTextID = "Text ID of archive half", 
                WorkingTextID = "Text ID of working half" 
            };

            try
            {
                sectionCollection.ParseSectionInfoFromDataTable(SectionInfoCollection.ImportAllSections(fileName), columnNames);
            }
            catch (Exception ex)
            {
                throw new Exception($"{fileName}: Could not parse section info from datatable");
            }

            return sectionCollection.Sections;
        }

        public static async Task<bool> AddSectionsToDatabaseAsync(DescDBContext dBContext, ICollection<SectionInfo> sections)
        {

            foreach (var section in sections)
            {
               var exists = await CheckForSectionAsync(dBContext, section).ConfigureAwait(true);
                if (exists)
                {
                    sections.Remove(section);
                }
            }
            

            try
            {
                using (DescDBContext dbContext = new DescDBContext())
                {

                    dbContext.AddRange(sections);
                    await dbContext.SaveChangesAsync().ConfigureAwait(true);
                    return true;
                }
            }
            catch (Exception)
            {

                throw new Exception(string.Format("Error adding sections to database"));
            }

        }

        public static async Task<bool> AddDescriptionsToDataBaseAsync(DescDBContext dbContext, ICollection<LithologicDescription> descriptions)
        {        
                try
                {
                     dbContext.AttachRange(descriptions);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {

                    throw new Exception("Error adding descriptions to database context.");
                }    
        }

        public static async Task<bool> AddMeasurementsToDataBaseAsync(DescDBContext dbContext, ICollection<Measurement> measurements)
        {     
                try
                {
                    dbContext.AttachRange(measurements);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex.Message);
                    throw new Exception("Error adding measurements to database context.");
                }
        }


        #endregion


        #region Obsolete
        [Obsolete]
        public static void SendLithologiesToDatabase(Dictionary<string, LithologicDescription> LithologyCache)
        {
            _ = LithologyCache ?? throw new ArgumentNullException(nameof(LithologyCache));

            DescDBContext context = null;
            try
            {
                context = new DescDBContext();
                //context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;

                var descriptionList = LithologyCache.Values.Where(x => !string.IsNullOrEmpty(x.LithologicID));

                // context.AddRange(descriptionList);

                for (int i = 0; i < descriptionList.Count(); i = i + 100)
                {
                    var entityToInsert = descriptionList.Skip(i).Take(100).ToList();
                    count = 100;
                    context = AddLithologiesToContext(context, entityToInsert, count, 100, true);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }

        }
        [Obsolete]
        public static void SendMeasurementsToDatabase(Dictionary<int, Measurement> MeasurementCache)
        {
            _ = MeasurementCache ?? throw new ArgumentNullException(nameof(MeasurementCache));

            DescDBContext context = null;
            try
            {
                context = new DescDBContext();
                //context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                var measurementList = MeasurementCache.Values.Where(x => x.LithologicSubID.HasValue);
                for (int i = 0; i < measurementList.Count(); i = i + 100)
                {
                    var entityToInsert = measurementList.Skip(i).Take(100);
                    count = 100;
                    context = AddMeasurementsToContext(context, entityToInsert, count, 100, true);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }
        [Obsolete]
        private static DescDBContext AddLithologiesToContext(DescDBContext context, IEnumerable<LithologicDescription> descriptions, int count, int commitCount, bool recreateContext)
        {
            context.AddRange(descriptions);

            //Detach all child records which already have been added otherwise I get the following error
            // Cannot insert explicit value for identity column in table 'Sections' when IDENTITY_INSERT is set to OFF.
            foreach (var description in descriptions)
            {
                if (description.SectionInfo.ID != 0)
                {
                    context.Entry(description.SectionInfo).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }

                foreach (LithologicSubinterval subInterval in description.LithologicSubintervals)
                {
                    //Detach subintervals already added (and have non-zero primary keys)
                    if (subInterval.ID != 0)
                    {
                        context.Entry(subInterval).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                    //Detach subinterval sectioninfo already added (and have non-zero primary keys)
                    if (subInterval.SectionInfo.ID != 0)
                    {
                        context.Entry(subInterval.SectionInfo).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                }
            }

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
        [Obsolete]
        private static DescDBContext AddMeasurementsToContext(DescDBContext context, IEnumerable<Measurement> measurements, int count, int commitCount, bool recreateContext)
        {
            context.AddRange(measurements);

            //Detach all child records which already have been added otherwise I get the following error
            // Cannot insert explicit value for identity column in table ... when IDENTITY_INSERT is set to OFF.
            //Measurements can only get descriptions and subintervals which have already been loaded
            foreach (var measurement in measurements)
            {

                //try detaching only the subinterval
                // context.Entry(measurement.LithologicSubinterval).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                //context.Entry(measurement.LithologicDescription).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                context.Entry(measurement.LithologicDescription).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

                foreach (LithologicSubinterval subInterval in measurement.LithologicDescription.LithologicSubintervals)
                {
                    if (measurement.LithologicSubinterval.ID != 0)
                    {
                        context.Entry(subInterval).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }

                }

                if (measurement.LithologicSubinterval.ID != 0)
                {
                    context.Entry(measurement.LithologicSubinterval).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }

                //DescriptionColumnValuePairs are tracked and accessed via the measurement's LithologicSubinterval property
                foreach (DescriptionColumnValuePair entry in measurement.LithologicDescription.DescriptionColumnValues)
                {
                    context.Entry(entry).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }

                if (measurement.SectionInfo.ID != 0)
                {
                    context.Entry(measurement.SectionInfo).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }
            }

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
        #endregion 
    }
}

