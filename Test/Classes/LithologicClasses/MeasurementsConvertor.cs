using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace DescLogicFramework
{
    /// <summary>
    /// Class to create a collection of Measurements from a DataTable.
    /// </summary>
    public class MeasurementsConvertor
    {
        private Cache<int, Measurement> _measurements = new Cache<int, Measurement>();

        public MeasurementsConvertor()
        {
        }
        public Cache<int, Measurement> Convert(IODPDataTable dt)
        {
            //count could be 0
            int i = _measurements.GetCollection().Count + 1;
            
            foreach (DataRow record in dt.DataTable.Rows)
            {
                Measurement measurement = new Measurement();

                //Assign the record values to appropriate Sections. The sectioninfo must be assigned to both the offset and offset intervals.
                measurement.SectionInfo.Expedition = record[dt.Expedition].ToString();
                measurement.SectionInfo.Site = record[dt.Site].ToString();
                measurement.SectionInfo.Hole = record[dt.Hole].ToString();
                measurement.SectionInfo.Core = record[dt.Core].ToString();
                measurement.SectionInfo.Type = record[dt.Type].ToString();
                measurement.SectionInfo.Section = record[dt.Section].ToString();

                //CARB files throw error here because there isn't an offset field within the file. Ensure there is.
                if (!string.IsNullOrEmpty(dt.Offset))
                {
                    //measurement.OffsetInfo.Offset = double.Parse(record[dt.Offset].ToString());
                    measurement.StartOffset.Offset = double.Parse(record[dt.Offset].ToString());
                    measurement.EndOffset.Offset = double.Parse(record[dt.Offset].ToString());
                } 
                if (!string.IsNullOrEmpty(dt.OffsetIntervals[0]))
                {
                    measurement.StartOffset.Offset = double.Parse(record[dt.OffsetIntervals[0]].ToString());
                }
                if (!string.IsNullOrEmpty(dt.OffsetIntervals[1]))
                {
                    measurement.EndOffset.Offset = double.Parse(record[dt.OffsetIntervals[1]].ToString());
                }

                //If there measurement intervals overlapping description intervals then you want to duplicate measurements to the cache, and then attribute the individual descriptions to each, respectively.
                measurement.DataRow = record;

                _measurements.Add(i, measurement);
                i++;
            }
            return _measurements;
        }
    }
}
