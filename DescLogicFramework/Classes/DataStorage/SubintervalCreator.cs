using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace DescLogicFramework
{
    public class SubintervalCreator
    {
        /// <summary>
        /// T
        /// </summary>
        /// <param name="descriptionsToTake">The number of records to create intervals for before the dbContext is disposed</param>
        /// <param name="expedition">The expedition used to filter descriptions</param>
        /// <returns></returns>
        public static async Task<bool> CreateSubIntervalsForDescriptions(int descriptionsToTake, string expedition)
        {
            //Do this expedition by expedition, after expedition measurements have been uploaded, this would only need to be called once

            try
            {
                using (DescDBContext dbContext = new DescDBContext())
                {

                    var descriptions = dbContext.LithologicDescriptions
                        .Include(x => x.SectionInfo)
                        .Include(x => x.LithologicSubintervals)
                        .ThenInclude(x => x.SectionInfo)
                        .Include(x => x.LithologicSubintervals)
                        .ThenInclude(x => x.LithologicDescription)  //The object graph, I want to add subintervals to a description, and add sectioninfo and description references to those subintervals
                        .Where(x => x.SectionInfo.Expedition == expedition)
                        .Where(x => x.LithologicSubintervals.Any() == false)  //The description should not have subintervals generated for it yet.
                        .Where (x=>x.SectionInfo.Expedition == expedition)
                        .Where (x=>x.EndOffset > x.StartOffset) //There are some descriptions that have incorrectly entered start and end offsets
                        .Take(descriptionsToTake)
                        .ToHashSet();

                    if (descriptions.Any() == false)
                    {
                        return false;
                    }

                    foreach (var description in descriptions)
                    {
                        description.GenerateSubintervals();
                    }

                     dbContext.AttachRange(descriptions);  //I can use Attach, but the objects are being tracked presently, SaveChanges should work just fine
                    dbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error generating subintervals for descriptions.");
            }
   
            return true;

        }



        public static async Task<bool> GetMeasurementsForSubIntervals(int skip, int take)
        {
            Console.Clear();

            using (DescDBContext dbContext = new DescDBContext())
            {

                var measurements = dbContext.MeasurementDescriptions
                       .Where(x=>x.ID > 14100000-1)
                       .Include(x=>x.SectionInfo)
                       .Include(x=>x.LithologicSubintervals)
                       .Skip(skip)
                       .Take(take)
                       .Select(x => x)
                       .ToHashSet();


                foreach (var measurement in measurements)
                {
                    Console.WriteLine(string.Format("Measurement ID: {0}",measurement.ID.ToString()));


                    List<LithologicSubinterval> matchingSubintervals;
                    try
                    {
                     matchingSubintervals = await dbContext.LithologicSubintervals
                                         .Include(x => x.SectionInfo)
                                         .Where(subinterval => subinterval.SectionInfo.ID == measurement.SectionInfo.ID)
                                         .Where(description =>
                                         (measurement.StartOffset >= description.StartOffset && measurement.StartOffset <= description.EndOffset)   //Finds an interval in which the measurement falls in part
                                         || (measurement.EndOffset >= description.StartOffset && measurement.EndOffset <= description.EndOffset))
                                         .ToListAsync().ConfigureAwait(true);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error finding subintervals for measurement");
                        continue;
                    }

                    if (matchingSubintervals.Count == 0)
                    {
                        continue;
                    }

                    try
                    {
                        foreach (var subinterval in matchingSubintervals)
                        {

                            //Add only if it doesn't contain the subinterval
                            if (!measurement.LithologicSubintervals.Contains(subinterval))
                            {
                                measurement.LithologicSubintervals.Add(subinterval);
                                Console.WriteLine(string.Format("Added Subinterval ID: {0}", subinterval.ID.ToString()));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Error adding Subintervals for Measurement: {0}", measurement.ID.ToString()));
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }


               
                };

                await dbContext.SaveChangesAsync().ConfigureAwait(true);

            }

            return true;
        }


        public static async Task<bool> GetLithologicSubIntervalsForMeasurements()
        {
            bool keepRunning = true;
            int skip = 0;
            int take = 10000;

            while (keepRunning)
            {
                keepRunning = await SubintervalCreator.GetMeasurementsForSubIntervals(skip, take);
                skip = skip + take;
                Console.WriteLine("Task Completed 1000 records");
            }
            return true;
        }


        public static async Task<bool> GetDescriptionsForMeasurment(string InstrumentSystem, ICollection<string> DescriptionColumns)
        {

           // ICollection<Measurement> measurements;

            using (DescDBContext dbContext = new DescDBContext())
            {
              var measurements =  await dbContext.MeasurementDescriptions
                        .Include(x => x.LithologicSubintervals)
                        .ThenInclude(x => x.LithologicDescription)
                        .ThenInclude(x => x.DescriptionColumnValues.Where(x=>DescriptionColumns.Contains(x.ColumnName)))
                        .Where(x => x.InstrumentSystem == InstrumentSystem)
                        .Take(100)
                        .Select(x=>x)
                        .ToListAsync().ConfigureAwait(true);
            }


            //foreach (var measurement in measurements)
            //{
            //    PrintMeasurementToConsole(measurement);
            //    foreach (var subinterval in measurement.LithologicSubintervals)
            //    {
            //        PrintSubintervalToConsole(subinterval);
            //        PrintDescriptionToConsole(subinterval.LithologicDescription);
            //    }
            //}

            return true;

        }


        public static void PrintMeasurementToConsole(Measurement measurement)
        {
            Console.WriteLine(string.Format("ID:{0}, System:{1}, Test:{2}, TextID:{3}", measurement.ID, measurement.InstrumentSystem, measurement.TestNumber, measurement.TextID));
        }
        public static void PrintSubintervalToConsole(LithologicSubinterval subinterval)
        {
            Console.WriteLine(string.Format("ID:{0}, LithologicSubID:{1}, StartOffset:{2}, EndOffset:{3}", subinterval.ID, subinterval.LithologicSubID, subinterval.StartOffset, subinterval.EndOffset));
        }
        public static void PrintDescriptionToConsole(LithologicDescription description)
        {
            Console.WriteLine(string.Format("ID:{0}, LithologicSubID:{1}, StartOffset:{2}, EndOffset:{3}", description.ID, description.LithologicID, description.StartOffset, description.EndOffset));
        }
    }
}
