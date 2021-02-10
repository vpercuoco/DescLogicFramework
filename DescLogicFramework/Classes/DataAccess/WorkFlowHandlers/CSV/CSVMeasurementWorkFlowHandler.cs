using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Configuration;


namespace DescLogicFramework.DataAccess
{
    /// <summary>
    /// A class to handle importing, manipulating and exporting measurement files
    /// </summary>
    public class CSVMeasurementWorkFlowHandler : DataFileWorkFlowHandler<Measurement>
    {
        public FileCollection FileCollection { get; set; }

        public string ExportFileName { get; set; }
        public string ExportDirectory { get; set; }

        public override void ImportMetaData(string[] metaData, Measurement measurement)
        {
            _ = metaData ?? throw new ArgumentNullException(nameof(metaData));
            _ = measurement ?? throw new ArgumentNullException(nameof(measurement));


            if (metaData.Length == 3)
            {
                measurement.InstrumentReport = metaData[0];
                measurement.InstrumentSystem = metaData[0];
            }
        }

        public Dictionary<int, Measurement> ImportCache(SectionInfoCollection SectionCollection)
        {
            
            string path = FileCollection.Filenames.FirstOrDefault();

            ExportFileName = path.Split(@"\").Last();
            string[] metaData = ExportFileName.Split("_");

            var dtReader = new CSVReader();
            dtReader.ReadPath = path;
            var Measurements = ImportIODPDataTable(dtReader);

            var MeasurementCache = MeasurementsConvertor.Convert(Measurements, SectionCollection);

            foreach (var record in MeasurementCache)
            {
                ImportMetaData(metaData, record.Value);
            }

            return MeasurementCache;
        }

        public void ExportToFile(Dictionary<int, Measurement> cache)
        {
            _ = cache ?? throw new ArgumentNullException(nameof(cache));

            System.IO.Directory.CreateDirectory(ExportDirectory);

            if (cache.Count > 0)
            {
                var Exporter = new CSVReader();
                Exporter.WritePath = ExportDirectory + ExportFileName;
                Exporter.Write(cache[1].DataRow.Table);
            }

            Console.WriteLine("Finished exporting file " + ExportFileName);
            Console.WriteLine();
        }


        /// <summary>
        /// A top method which uses a Lithologic Associator class to correlate to measurements with corresponding Lithologic Descriptions
        /// </summary>
        public void UpdateMeasurementCacheWithLithologicDescriptions( Dictionary<int, Measurement> measurementCache, Dictionary<SectionInfo, Dictionary<string,LithologicDescription>> lithologyCache)
        {
           (new LithologicAssociator()).Associate(measurementCache, lithologyCache);

            //Set LithologicID, and LithologicSubIDs as new columns in datatable
            if (measurementCache.Count > 1)
            {
                measurementCache[1].DataRow.Table.Columns.Add("LithologicID_VP", typeof(string)).SetOrdinal(0);
                measurementCache[1].DataRow.Table.Columns.Add("LithologicSubID_VP", typeof(string)).SetOrdinal(1);

                foreach (var record in measurementCache)
                {
                    record.Value.DataRow.BeginEdit();
                    if (record.Value.LithologicDescription != null)
                    {
                        record.Value.DataRow["LithologicID_VP"] = record.Value.LithologicDescription.LithologicID;
                        record.Value.DataRow.EndEdit();
                    }
                    else
                    {
                        record.Value.DataRow["LithologicID_VP"] = "-1";
                        record.Value.DataRow.EndEdit();
                    }

                    if (record.Value.LithologicSubinterval != null)
                    {
                        record.Value.DataRow["LithologicSubID_VP"] = record.Value.LithologicDescription.LithologicID + "-" + record.Value.LithologicSubinterval.LithologicSubID.ToString();
                        record.Value.DataRow.EndEdit();
                    }
                    else
                    {
                        record.Value.DataRow["LithologicSubID_VP"] = "-1";
                        record.Value.DataRow.EndEdit();
                    }
                }
            }
        }
    }
}
