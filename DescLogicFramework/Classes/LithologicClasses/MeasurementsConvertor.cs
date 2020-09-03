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

        public Cache<int, Measurement> Convert(IODPDataTable dataTable)
        {
            int measurementCount = _measurements.GetCollection().Count + 1;
            
            foreach (DataRow record in dataTable.DataTable.Rows)
            {
                Measurement measurement = new Measurement();

                //Assign the record values to appropriate Sections. The sectioninfo must be assigned to both the offset and offset intervals.
                measurement.SectionInfo.Expedition = record[dataTable.Expedition].ToString();
                measurement.SectionInfo.Site = record[dataTable.Site].ToString();
                measurement.SectionInfo.Hole = record[dataTable.Hole].ToString();
                measurement.SectionInfo.Core = record[dataTable.Core].ToString();
                measurement.SectionInfo.Type = record[dataTable.Type].ToString();
                measurement.SectionInfo.Section = record[dataTable.Section].ToString();

                //CARB files throw error here because there isn't an offset field within the file. Ensure there is.
                if (!string.IsNullOrEmpty(dataTable.Offset))
                {
                    //measurement.OffsetInfo.Offset = double.Parse(record[dt.Offset].ToString());
                    measurement.StartOffset.Offset = double.Parse(record[dataTable.Offset].ToString());
                    measurement.EndOffset.Offset = double.Parse(record[dataTable.Offset].ToString());
                } 
                if (!string.IsNullOrEmpty(dataTable.OffsetIntervals[0]))
                {
                    measurement.StartOffset.Offset = double.Parse(record[dataTable.OffsetIntervals[0]].ToString());
                }
                if (!string.IsNullOrEmpty(dataTable.OffsetIntervals[1]))
                {
                    measurement.EndOffset.Offset = double.Parse(record[dataTable.OffsetIntervals[1]].ToString());
                }

                //If there measurement intervals overlapping description intervals then you want to duplicate measurements to the cache, and then attribute the individual descriptions to each, respectively.
                measurement.DataRow = record;

                _measurements.Add(measurementCount, measurement);
                measurementCount++;
            }
            return _measurements;
        }
    }
}
