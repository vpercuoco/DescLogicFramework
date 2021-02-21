using System;
using System.Collections.Generic;
using System.Linq;

namespace DescLogicFramework
{
    public static class LithologyDatabaseWorkflowHandler
    {
        public static void SendLithologiesToDataBase(Dictionary<string,LithologicDescription> LithologyCache, List<string> acceptableColumns)
        {
            _ = LithologyCache ?? throw new ArgumentNullException(nameof(LithologyCache));

            LithologyDBContext context = null;
            try
            {
                context = new LithologyDBContext();
                //context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;

                var descriptionList = LithologyCache.Values.Where(x => !string.IsNullOrEmpty(x.LithologicID));

                // context.AddRange(descriptionList);

                for (int i = 0; i < descriptionList.Count(); i = i + 100)
                {
                    var entityToInsert = descriptionList.Skip(i).Take(100).ToList();
                    count = 100;
                    context = AddLithologiesToContext(context, entityToInsert, count, 100, true, acceptableColumns);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }
        public static LithologyDBContext AddLithologiesToContext(LithologyDBContext context, IEnumerable<LithologicDescription> descriptions, int count, int commitCount, bool recreateContext, List<string> acceptableColumns)
        {
            context.AddRange(descriptions);

            //Detach all child records which already have been added otherwise I get the following error
            // Cannot insert explicit value for identity column in table 'Sections' when IDENTITY_INSERT is set to OFF.
            foreach (var description in descriptions)
            {
               var acceptedDescriptionColumnValues =  description.DescriptionColumnValues.Where(x => !acceptableColumns.Contains(x.ColumnName)).ToList();
                //I want to ensure only certain DescriptionColumnValuePairsAre loaded
                //foreach (var ColumnValuePair in acceptedDescriptionColumnValues)
               // {
                 //   context.Entry(ColumnValuePair).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
               // }
                
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
                    context = new LithologyDBContext();
                }
            }

            return context;

        }
    }
}

