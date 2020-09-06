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
        Cache<int, Measurement> IntervalTemporaryCache = new Cache<int, Measurement>();

        public event EventHandler<MultipleDescriptionsEventArgs> MultipleDescriptionsDetected;
        protected virtual void OnMultipleDescriptionsDetected(object sender, MultipleDescriptionsEventArgs e)
        {
            MultipleDescriptionsDetected?.Invoke(this, e);
        }
        public class MultipleDescriptionsEventArgs : EventArgs
        {
            public int DescriptionCount { get; set; }
            public Measurement Measurement { get; set; }
            public List<LithologicDescription> Descriptions { get; set; }
            public MultipleDescriptionsEventArgs(Measurement measurement, int descriptionCount)
            {
                DescriptionCount = descriptionCount;
                Measurement = measurement;
                Descriptions = new List<LithologicDescription>();
            }
        }




        /// <summary>
        /// Creates a new Lithologic Associator object
        /// </summary>
        public LithologicAssociator()
        {
            //   this.MultipleDescriptionsDetected += NotifyConsoleOfMultipleDescriptionsDetected;
        }

        public void NotifyConsoleOfMultipleDescriptionsDetected(object sender, MultipleDescriptionsEventArgs e)
        {
            Console.WriteLine(@"Measurement has {0} descriptions!", e.DescriptionCount.ToString());
            Console.WriteLine(@"Measurement {0}, StartOffset:{1}, EndOffset{2}", e.Measurement.SectionInfo.ToString(), e.Measurement.StartOffset.Offset.ToString(), e.Measurement.EndOffset.Offset.ToString());
            int i = 1;
            foreach (LithologicDescription description in e.Descriptions)
            {
                Console.WriteLine(@"Description {0}: {1} StartOffset:{2} EndOffset{3}", i.ToString(), description.SectionInfo.ToString(), description.StartOffset.Offset.ToString(), description.EndOffset.Offset.ToString());
                i++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Looks up the the LithologicID's and LithologicSubID's for a collection of Measurements
        /// </summary>
        /// <param name="Measurements">The cache of measurements</param>
        /// <param name="Descriptions">The cache of Lithologic Descriptions</param>
        /// <returns>A cache of Measurements</returns>
        public Cache<int, Measurement> Associate(ref Cache<int, Measurement> Measurements, ref Cache<string, LithologicDescription> Descriptions)
        {
            if (Measurements.GetCollection().Count > 0)
            {

                IntervalTemporaryCache.GetCollection().Clear();

                SetMeasurementLithologicDescription(ref Measurements, ref Descriptions);

                //Add in all the measurements which overlap more than one interval back into the main cache
                int lastMeasurementKey = Measurements.GetCollection().Keys.Max();
                foreach (KeyValuePair<int, Measurement> record in IntervalTemporaryCache.GetCollection())
                {
                    lastMeasurementKey++;
                    Measurements.Add(lastMeasurementKey, record.Value);
                }

                SetSubIntervals(Measurements);

                Console.WriteLine("The number of nonmatch intervals were: " + nonMatchIntervals.ToString());
                Console.WriteLine("The number of nonmatch subintervals were: " + nonMatchSubintervals.ToString());

            }
            return Measurements;
        }




        /// <summary>
        /// Identifies the Lithologic Descriptions for a collection of Measurements. 
        /// </summary>
        /// <param name="Measurements">The cache of measurements</param>
        /// <param name="Descriptions">The cache of Lithologic Descriptions</param>
        /// <returns></returns>
        private Cache<int, Measurement> SetMeasurementLithologicDescription(ref Cache<int, Measurement> Measurements, ref Cache<string, LithologicDescription> Descriptions)
        {

            foreach (KeyValuePair<int, Measurement> measurement in Measurements.GetCollection())
            {
                if (IsOffsetTypeMeasurement(measurement))
                {
                    var matchingDescriptions = Descriptions.GetCollection().Where(z => z.Value.Contains(measurement.Value)).ToList();
                    measurement.Value.LithologicDescription = matchingDescriptions.FirstOrDefault().Value;
                }
                else if (IsIntervalTypeMeasurement(measurement))
                {
                    var matchingDescriptions = Descriptions.GetCollection().Where(z => z.Value.Contains(measurement.Value)).ToList();
                    int recordCount = matchingDescriptions.Count;

                    measurement.Value.LithologicDescription = matchingDescriptions.FirstOrDefault().Value;

                    //Code which duplicates a measurement in the temp cache if its interval overlaps more than one lithologic description interval
                    int cacheCount = IntervalTemporaryCache.Count();
                    for (int i = 2; i < recordCount; i++)
                    {
                        Measurement measurementDifferentLithology = new Measurement(measurement.Value);
                        measurementDifferentLithology.LithologicDescription = matchingDescriptions.ElementAt(i).Value;
                        IntervalTemporaryCache.Add(cacheCount + i - 1, measurementDifferentLithology);
                    }
                }
            }
            return Measurements;
        }

        /// <summary>
        /// Identifies the Lithologic Subinterval for a given Measurement
        /// </summary>
        /// <param name="Measurements">A collection of Measurements</param>
        private void SetSubIntervals(Cache<int, Measurement> Measurements)
        {
            foreach (KeyValuePair<int, Measurement> measurement in Measurements.GetCollection())
            {
                if (measurement.Value.LithologicDescription != null)
                {
                    try
                    {
                        measurement.Value.LithologicSubinterval = measurement.Value.LithologicDescription.GetSubinterval(measurement.Value);
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


        private bool IsOffsetTypeMeasurement(KeyValuePair<int, Measurement> measurement)
        {

            if (measurement.Value.StartOffset.Offset != -1 && measurement.Value.StartOffset.Offset == measurement.Value.EndOffset.Offset)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool IsIntervalTypeMeasurement(KeyValuePair<int, Measurement> measurement)
        {
            if (measurement.Value.StartOffset.Offset != -1 && measurement.Value.EndOffset.Offset != -1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
