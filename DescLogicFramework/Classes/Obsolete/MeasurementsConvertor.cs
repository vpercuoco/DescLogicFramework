using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;

namespace DescLogicFramework
{
    /// <summary>
    /// Class to create a collection of Measurements from a DataTable.
    /// </summary>
    public static class MeasurementsConvertor
    {
        public static Dictionary<int, Measurement> Convert(IODPDataTable dataTable, SectionInfoCollection SectionCollection)
        {

            _ = SectionCollection ?? throw new ArgumentNullException(nameof(SectionCollection));
            _ = dataTable ?? throw new ArgumentNullException(nameof(dataTable));

            Dictionary<int, Measurement> _measurements = new Dictionary<int, Measurement>();

            int measurementCount = _measurements.Count + 1;

            //TODO: Ignore record if error is thrown, ex: offsets with TCON
            foreach (DataRow record in dataTable.DataTable.Rows)
            {
                SectionInfo measurementSectionInfo = new SectionInfo();
                try
                {
                    measurementSectionInfo.Expedition = record[dataTable.ExpeditionColumn].ToString();
                    measurementSectionInfo.Site = record[dataTable.SiteColumn].ToString();
                    measurementSectionInfo.Hole = record[dataTable.HoleColumn].ToString();
                    measurementSectionInfo.Core = record[dataTable.CoreColumn].ToString();
                    measurementSectionInfo.Type = record[dataTable.TypeColumn].ToString();
                    measurementSectionInfo.Section = record[dataTable.SectionColumn].ToString();
                }
                catch (Exception)
                {
                    throw new IndexOutOfRangeException(nameof(record));
                }

                Measurement measurement = new Measurement(measurementSectionInfo);

                measurement.SectionInfo = SectionCollection.GetExistingElseAddAndGetCurrentSection(measurement.SectionInfo);

                //CARB files throw error here because there isn't an offset field within the file. Ensure there is.
                try
                {
                    if (!string.IsNullOrEmpty(dataTable.OffsetColumn))
                    {
                        measurement.StartOffset = double.Parse(record[dataTable.OffsetColumn].ToString(), CultureInfo.CurrentCulture);
                        measurement.EndOffset = double.Parse(record[dataTable.OffsetColumn].ToString(), CultureInfo.CurrentCulture);
                    }
                    if (!string.IsNullOrEmpty(dataTable.TopOffsetColumn))
                    {
                        measurement.StartOffset = double.Parse(record[dataTable.TopOffsetColumn].ToString(), CultureInfo.CurrentCulture);
                    }
                    if (!string.IsNullOrEmpty(dataTable.BottomOffsetColumn))
                    {
                        measurement.EndOffset = double.Parse(record[dataTable.BottomOffsetColumn].ToString(), CultureInfo.CurrentCulture);
                    }
                  
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
