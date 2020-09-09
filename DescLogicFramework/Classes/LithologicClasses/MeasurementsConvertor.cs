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

        private Dictionary<int, Measurement> _measurements = new Dictionary<int, Measurement>();

        public Dictionary<int, Measurement> Convert(IODPDataTable dataTable, ref SectionInfoCollection SectionCollection)
        {
            int measurementCount = _measurements.Count + 1;

            //TODO: Ignore record if error is thrown, ex: offsets with TCON
            foreach (DataRow record in dataTable.DataTable.Rows)
            {
                SectionInfo measurementSectionInfo = new SectionInfo();
                try
                {
                    measurementSectionInfo.Expedition = record[dataTable.Expedition].ToString();
                    measurementSectionInfo.Site = record[dataTable.Site].ToString();
                    measurementSectionInfo.Hole = record[dataTable.Hole].ToString();
                    measurementSectionInfo.Core = record[dataTable.Core].ToString();
                    measurementSectionInfo.Type = record[dataTable.Type].ToString();
                    measurementSectionInfo.Section = record[dataTable.Section].ToString();
                }
                catch (Exception)
                {
                    throw new IndexOutOfRangeException(nameof(record));
                }


                Measurement measurement = new Measurement(measurementSectionInfo);

                //Determine if section is already in collection, if so get reference to the section:
                #region GlobalSectionList
                 measurement.SectionInfo = SectionCollection.GetExistingElseAddAndGetCurrentSection(measurement.SectionInfo);
                 measurement.StartOffset.SectionInfo = measurement.SectionInfo;
                 measurement.EndOffset.SectionInfo = measurement.SectionInfo;
                #endregion

                //CARB files throw error here because there isn't an offset field within the file. Ensure there is.
                try
                {
                    if (!string.IsNullOrEmpty(dataTable.Offset))
                    {
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
                catch (Exception)
                {
                    //throw;
                }
            }

            return _measurements;
        }
    }
}
