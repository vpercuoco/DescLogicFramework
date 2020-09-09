using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DescLogicFramework;
using System.Configuration;


namespace DescLogicFramework.DataAccess
{
    /// <summary>
    /// A class to handle importing, manipulating and exporting lithologic description files.
    /// </summary>
    public class CSVLithologyWorkflowHandler : DataFileWorkFlowHandler<LithologicDescription>
    {
        public FileCollection FileCollection { get; set; }
        public string ExportFileName {get; set;}
        public string ExportDirectory { get; set; }

        public Dictionary<string, LithologicDescription> ImportCache(SectionInfoCollection SectionCollection)
        {

            var LithologyCache = new Dictionary<string, LithologicDescription>();

            var dataTableReader = new CSVReader();

            foreach (string path in FileCollection.Filenames)
            {
                Console.WriteLine("Processing description file: " + path);

                string filename = path.Split(@"\").Last();
                string[] metaData = filename.Split("_");

                dataTableReader.ReadPath = path;

                var lithologyDataTable = ImportIODPDataTable(dataTableReader);
                  
                var ConvertedLithologyCache = LithologyConvertor.Convert(lithologyDataTable, SectionCollection);

                foreach (var record in ConvertedLithologyCache)
                {
                    ImportMetaData(metaData, record.Value);
                    LithologyCache.Add(record.Key, record.Value);
                }

                ExportFileName = filename;

                if (ProgramSettings.ExportCachesToFiles)
                {
                    ExportToFile(ConvertedLithologyCache);
                }
            }

            return LithologyCache;
        }

        public void ExportToFile(Dictionary<string, LithologicDescription> lithologyCache)
        {
            _ = lithologyCache ?? throw new ArgumentNullException(nameof(lithologyCache));

            bool lithologyCacheHasRecords = lithologyCache.Count > 1 ? true : false;

            if (lithologyCacheHasRecords)
            {
                System.IO.Directory.CreateDirectory(ExportDirectory);
                var exporter = new CSVReader();
                exporter.WritePath = ExportDirectory + ExportFileName;
                exporter.Write(lithologyCache.Values.First().DataRow.Table);


                Console.WriteLine("Finished exporting Lithology file " + ExportFileName);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Couldn't process " + ExportFileName + "because file has no records.");
                Console.WriteLine();
            }
        }

         public override void ImportMetaData(string[] metaData, LithologicDescription description)
        {
            _ = description ?? throw new ArgumentNullException(nameof(description));

            _ = metaData ?? throw new ArgumentNullException(nameof(metaData));

            if (metaData.Length == 5)
            {
                description.DescriptionReport = metaData[1];
                description.DescriptionGroup = metaData[2];
                description.DescriptionTab = metaData[3];
            }
            else if (metaData.Length > 5)
            {
                description.DescriptionReport = metaData[1] + " " + metaData[2];
                description.DescriptionGroup = metaData[3];
                description.DescriptionTab = metaData[4];
            }
        }
    }

}
