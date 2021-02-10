using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace DescLogicFramework
{
    /// <summary>
    /// Class used to lookup Lithologic Description ID's for Measurements
    /// </summary>
    public class LithologicAssociator
    {
        int nonMatchIntervals = 0;
        int nonMatchSubintervals = 0;

        Dictionary<int, Measurement> IntervalTemporaryCache = new Dictionary<int, Measurement>();

        public event EventHandler<MultipleDescriptionsEventArgs> MultipleDescriptionsDetected;
        protected virtual void OnMultipleDescriptionsDetected(object sender, MultipleDescriptionsEventArgs e)
        {
            MultipleDescriptionsDetected?.Invoke(this, e);
        }
        public class MultipleDescriptionsEventArgs : EventArgs
        {
            public int DescriptionCount { get; set; }
            public Measurement Measurement { get; set; }
            public List<LithologicDescription> Descriptions { get; }
            public MultipleDescriptionsEventArgs(Measurement measurement, int descriptionCount)
            {
                DescriptionCount = descriptionCount;
                Measurement = measurement;
                Descriptions = new List<LithologicDescription>();
            }
        }
        public LithologicAssociator()
        {
            //   this.MultipleDescriptionsDetected += NotifyConsoleOfMultipleDescriptionsDetected;
        }

        public void NotifyConsoleOfMultipleDescriptionsDetected(object sender, MultipleDescriptionsEventArgs e)
        {
            Console.WriteLine(@"Measurement has {0} descriptions!", e.DescriptionCount.ToString());
            Console.WriteLine(@"Measurement {0}, StartOffset:{1}, EndOffset{2}", e.Measurement.SectionInfo.ToString(), e.Measurement.StartOffset.ToString(), e.Measurement.EndOffset.ToString());
            int i = 1;
            foreach (LithologicDescription description in e.Descriptions)
            {
                Console.WriteLine(@"Description {0}: {1} StartOffset:{2} EndOffset{3}", i.ToString(), description.SectionInfo.ToString(), description.StartOffset.ToString(), description.EndOffset.ToString());
                i++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Looks up the the LithologicID's and LithologicSubID's for a collection of Measurements
        /// </summary>
        public void Associate(Dictionary<int, Measurement> Measurements, Dictionary<SectionInfo, Dictionary<string, LithologicDescription>> Descriptions)
        {
            if (Measurements.Count > 0)
            {
                IntervalTemporaryCache.Clear();

                SetMeasurementLithologicDescription( Measurements, Descriptions);

                //Add in all the measurements which overlap more than one interval back into the main cache
                int lastMeasurementKey = Measurements.Keys.Max();
                foreach (KeyValuePair<int, Measurement> record in IntervalTemporaryCache)
                {
                    lastMeasurementKey++;
                    Measurements.Add(lastMeasurementKey, record.Value);
                }

                SetSubIntervals(Measurements);

                Console.WriteLine(string.Format("Could not find lithologic descriptions for {0} of imported measurements.", nonMatchIntervals.ToString()));
                Console.WriteLine(string.Format("Could not find lithologic subintervals for {0} of measurements with associated lithologic descriptions.", nonMatchSubintervals.ToString()));

            }
        }

        /// <summary>
        /// Identifies the Lithologic Descriptions for a collection of Measurements. 
        /// </summary>
        private Dictionary<int, Measurement> SetMeasurementLithologicDescription( Dictionary<int, Measurement> Measurements, Dictionary<SectionInfo, Dictionary<string, LithologicDescription>> Descriptions)
        {
            foreach (Measurement measurement in Measurements.Values)
            {
                
                var matchingDescriptions = Descriptions.Where(z => z.Key.Equals(measurement.SectionInfo))
                                                           .SelectMany(x => x.Value.Values.Where(x => x.Contains(measurement)));
                                                           
                if (IsOffsetTypeMeasurement(measurement))
                {
                    measurement.LithologicDescription = matchingDescriptions.FirstOrDefault();
                }
                else if (IsIntervalTypeMeasurement(measurement))
                {
                     var matchingDescriptionsList = matchingDescriptions.ToList();
                                                                           
                    measurement.LithologicDescription = matchingDescriptionsList.FirstOrDefault();

                    DuplicateMeasurementForEachMatchingDescription(measurement, matchingDescriptionsList);
                }
            }
            return Measurements;
        }

        private void DuplicateMeasurementForEachMatchingDescription(Measurement measurement, List<LithologicDescription> matchingDescriptionsList)
        {
            int recordCount = matchingDescriptionsList.Count;
            int cacheCount = IntervalTemporaryCache.Count;
            for (int i = 2; i < recordCount; i++)
            {
                Measurement measurementDifferentLithology = new Measurement(measurement);
                measurementDifferentLithology.LithologicDescription = matchingDescriptionsList.ElementAt(i);
                IntervalTemporaryCache.Add(cacheCount + i - 1, measurementDifferentLithology);
            }
        }

        /// <summary>
        /// Identifies the Lithologic Subinterval for a given Measurement
        /// </summary>
        private void SetSubIntervals(Dictionary<int, Measurement> Measurements)
        {
            foreach (Measurement measurement in Measurements.Values)
            {
                if (measurement.LithologicDescription != null)
                {
                    try
                    {
                        measurement.LithologicSubinterval = measurement.LithologicDescription.GetSubinterval(measurement);
                    }
                    catch (Exception ex)
                    {
                        
                        nonMatchSubintervals++;
                    }
                }
                else
                {
                    nonMatchIntervals++;
                }

            }
        }

        private static bool IsOffsetTypeMeasurement(Measurement measurement)
        {
            return measurement.StartOffset != -1 && measurement.StartOffset == measurement.EndOffset ? true : false;
        }

        private bool IsIntervalTypeMeasurement(Measurement measurement)
        {
            return measurement.StartOffset != -1 && measurement.EndOffset != -1 ? true : false;
        }

    }
}
