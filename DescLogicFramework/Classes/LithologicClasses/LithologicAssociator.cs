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
        public Dictionary<int, Measurement> Associate(ref Dictionary<int, Measurement> Measurements, ref Dictionary<SectionInfo, Dictionary<string, LithologicDescription>> Descriptions)
        {
            if (Measurements.Count > 0)
            {

                IntervalTemporaryCache.Clear();

                SetMeasurementLithologicDescription(ref Measurements, ref Descriptions);

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
            return Measurements;
        }




        /// <summary>
        /// Identifies the Lithologic Descriptions for a collection of Measurements. 
        /// </summary>
        /// <param name="Measurements">The cache of measurements</param>
        /// <param name="Descriptions">The cache of Lithologic Descriptions</param>
        /// <returns></returns>
        private Dictionary<int, Measurement> SetMeasurementLithologicDescription(ref Dictionary<int, Measurement> Measurements, ref Dictionary<SectionInfo, Dictionary<string, LithologicDescription>> Descriptions)
        {

            foreach (Measurement measurement in Measurements.Values)
            {
                if (IsOffsetTypeMeasurement(measurement))
                {
                    //Method must use the original Cache<string,LithologicDescriptions> Descriptions parameter for the following two lines to work:
                    //var matchingDescriptions = Descriptions.GetCollection().Where(z => z.Value.Contains(measurement.Value)).ToList();
                    // measurement.Value.LithologicDescription = matchingDescriptions.FirstOrDefault().Value;

                    measurement.LithologicDescription = Descriptions.Where(z => z.Key.Equals(measurement.SectionInfo))
                                                                    .SelectMany(x => x.Value.Values.Where(x => x.Contains(measurement)))
                                                                    .FirstOrDefault();
                                                                     

                        //.FirstOrDefault(x => x.Value.Contains(measurement.Value)).Value;
                                                                           // .Select(x => x.Value.Where(y => y.Value.Contains(measurement.Value)).FirstOrDefault().Value).FirstOrDefault();

                   
                }
                else if (IsIntervalTypeMeasurement(measurement))
                {
                    //Method must use the original Cache<string,LithologicDescriptions> Descriptions parameter for the following two lines to work:
                    // var matchingDescriptions = Descriptions.GetCollection().Where(z => z.Value.Contains(measurement.Value)).ToList();
                    // int recordCount = matchingDescriptions.Count;
                    // measurement.Value.LithologicDescription = matchingDescriptions.FirstOrDefault().Value;


                    var matchingDescriptions = Descriptions.Where(z => z.Key.Equals(measurement.SectionInfo))
                                                           .SelectMany(x => x.Value.Values.Where(x => x.Contains(measurement)))
                                                           .ToList();
                                                                            
                    int recordCount = matchingDescriptions.Count;
                    measurement.LithologicDescription = matchingDescriptions.FirstOrDefault();

                    //Code which duplicates a measurement in the temp cache if its interval overlaps more than one lithologic description interval
                    int cacheCount = IntervalTemporaryCache.Count();
                    for (int i = 2; i < recordCount; i++)
                    {
                        Measurement measurementDifferentLithology = new Measurement(measurement);
                        //measurementDifferentLithology.LithologicDescription = matchingDescriptions.ElementAt(i).Value;
                        measurementDifferentLithology.LithologicDescription = matchingDescriptions.ElementAt(i);
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


        private bool IsOffsetTypeMeasurement(Measurement measurement)
        {

            if (measurement.StartOffset.Offset != -1 && measurement.StartOffset.Offset == measurement.EndOffset.Offset)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool IsIntervalTypeMeasurement(Measurement measurement)
        {
            if (measurement.StartOffset.Offset != -1 && measurement.EndOffset.Offset != -1)
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
