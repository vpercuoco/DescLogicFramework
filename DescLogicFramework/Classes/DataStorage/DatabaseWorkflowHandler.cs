using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DescLogicFramework
{
    public class DatabaseWorkflowHandler
    {
        public DatabaseWorkflowHandler()
        {

        }

        public void SendLithologiesToDatabase(Dictionary<string, LithologicDescription> LithologyCache)
        {
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
        public void SendMeasurementsToDatabase(Dictionary<int, Measurement> MeasurementCache)
        {
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

        private DescDBContext AddToContext<TEntity>(DescDBContext context, IEnumerable<TEntity> entities, int count, int commitCount, bool recreateContext) where TEntity : class
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

        private DescDBContext AddLithologiesToContext(DescDBContext context, IEnumerable<LithologicDescription> descriptions, int count, int commitCount, bool recreateContext)
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
        private DescDBContext AddMeasurementsToContext(DescDBContext context, IEnumerable<Measurement> measurements, int count, int commitCount, bool recreateContext)
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
                foreach (DescriptionColumnValuePair entry in measurement.LithologicDescription.Data)
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

    }
}
