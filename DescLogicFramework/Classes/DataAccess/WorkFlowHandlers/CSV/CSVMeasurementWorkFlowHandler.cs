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

        public Dictionary<int, Measurement> ImportCache(ref SectionInfoCollection SectionCollection)
        {
            
            string path = FileCollection.Filenames.FirstOrDefault();

            ExportFileName = path.Split(@"\").Last();
            string[] metaData = ExportFileName.Split("_");

            var dtReader = new CSVReader();
            dtReader.ReadPath = path;
            var Measurements = ImportIODPDataTable(dtReader);

            var MeasurementCache = (new MeasurementsConvertor()).Convert(Measurements, ref SectionCollection);

            foreach (var record in MeasurementCache)
            {
                ImportMetaData(metaData, record.Value);
            }

            return MeasurementCache;
        }

        public void ExportCache(Dictionary<int, Measurement> cache)
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
        /// <param name="measurementCache"></param>
        /// <param name="lithologyCache"></param>
        /// <returns></returns>
        public Dictionary<int, Measurement> UpdateMeasurementCacheWithLithologicDescriptions(ref Dictionary<int, Measurement> measurementCache, ref Dictionary<SectionInfo, Dictionary<string,LithologicDescription>> lithologyCache)
        {
            var associatedMeasurements = (new LithologicAssociator()).Associate(ref measurementCache, ref lithologyCache);

            //Set LithologicID, and LithologicSubIDs as new columns in datatable
            if (associatedMeasurements.Count > 1)
            {
                associatedMeasurements[1].DataRow.Table.Columns.Add("LithologicID_VP", typeof(string)).SetOrdinal(0);
                associatedMeasurements[1].DataRow.Table.Columns.Add("LithologicSubID_VP", typeof(string)).SetOrdinal(1);

                foreach (var record in associatedMeasurements)
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
            return associatedMeasurements;
        }
    }
}
