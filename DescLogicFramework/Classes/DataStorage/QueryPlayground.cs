using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DescLogicFramework
{
    public static class QueryPlayground
    {

        public static async Task<ICollection<string>> GetUniquePrincipalLithologies()
        {

            using (DescDBContext dbContext = new DescDBContext())
            {
                return await dbContext.DescriptionColumnValuePairs
                     .Where(x => x.CorrectedColumnName == "Principal Lithology")
                     .Select(x => x.Value)
                     .Distinct()
                     .ToListAsync().ConfigureAwait(true);

            }

        }

        public static async Task<ICollection<string>> GetColumnNamesWhichContainsString(string lookupSet)
        {

            using (DescDBContext dbContext = new DescDBContext())
            {
                return await dbContext.DescriptionColumnValuePairs
                     .Where(x => x.ColumnName.Contains(lookupSet, StringComparison.CurrentCulture))
                     .Select(x => x.ColumnName)
                     .Distinct()
                     .ToListAsync().ConfigureAwait(true);

            }

        }

        public static async Task<ICollection<Measurement>> GetMeasurementIDsForLithology(string principalLithology)
        {

            using (DescDBContext dbContext = new DescDBContext())
            {
                return await dbContext.LithologicDescriptions
                    .Include(x => x.DescriptionColumnValues
                                       .Where(x => x.CorrectedColumnName == "Principal Lithology")
                                       .Where(x => x.Value == principalLithology))
                  .Include(x => x.LithologicSubintervals)
                  .ThenInclude(x => x.Measurements
                  .Where(x => x.InstrumentSystem == "GRA" || x.InstrumentSystem == "MAD"))
                  .SelectMany(x => x.LithologicSubintervals)
                  .SelectMany(x => x.Measurements)
                  .Take(1000)
                  .ToListAsync().ConfigureAwait(true);

            }

        }

        public static async Task<ICollection<Mashup>> GetMeasurementsWithDrillingDisturbances(string instrumentReport)
        {

            using (DescDBContext descDBContext = new DescDBContext())
            {

                return await descDBContext.MeasurementDescriptions
                    .Where(x => x.InstrumentSystem == "GRA")
                    .Include(x => x.LithologicSubintervals
                    .Where(x => x.LithologicDescription
                                    .DescriptionColumnValues
                                    .Any(x => x.ColumnName == "Drilling disturbance type" || x.ColumnName == "Drilling disturbance intensity")))
                    .ThenInclude(x => x.LithologicDescription
                                      .DescriptionColumnValues
                                      .Where(x => x.ColumnName == "Drilling disturbance type" || x.ColumnName == "Drilling disturbance intensity"))
                    .SelectMany(measurement=>measurement.LithologicSubintervals,(measurement,subinterval) => new {measurement.ID,subinterval})
                    .SelectMany(x=>x.subinterval.LithologicDescription.DescriptionColumnValues,
                    (x,columnvalues) => new Mashup {MeasurementID = x.ID, LithologicSubID = x.subinterval.ID, ColumnValuePairID =  columnvalues.ID, ColumnName = columnvalues.ColumnName, ColumnValue = columnvalues.Value})
                   .Where(x=>x.ColumnName == "Drilling disturbance type" || x.ColumnName == "Drilling disturbance intensity")
                   .Select(x=>x)
                   .ToListAsync().ConfigureAwait(true);

            }

        }

    }

    public class Mashup
    {
        public int? MeasurementID { get; set; } = -1;

        public int? LithologicSubID { get; set; } = -1;

        public int? ColumnValuePairID { get; set; } = -1;

        public string ColumnName { get; set; } = string.Empty;

        public string ColumnValue { get; set; } = string.Empty;

        public string WriteString()
        {
            return $"{MeasurementID},{LithologicSubID},{ColumnValuePairID},{ColumnName},{ColumnValue}";
        }
    }

}
