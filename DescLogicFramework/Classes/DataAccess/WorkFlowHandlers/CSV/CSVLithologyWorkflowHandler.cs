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
    public class CSVLithologyWorkflowHandler : DataFileWorkFlowHandler<LithologicDescription>, IWorkFlowHandler<string, LithologicDescription>
    {

        public FileCollection FileCollection { get; set; }

        public string ExportFileName {get; set;}
        public string ExportDirectory { get; set; }
        public Cache<string, LithologicDescription> Cache { get; set; }


        public Cache<string, LithologicDescription> ImportCache()
        {
            var LithologyCache = new Cache<string, LithologicDescription>();
            var LithologyConvertor = new LithologyConvertor();
            var dtReader = new CSVReader();

            foreach (string path in FileCollection.Filenames)
            {
                Console.WriteLine("Processing description file: " + path);

                string filename = path.Split(@"\").Last();
                string[] metaData = filename.Split("_");

                
                dtReader.ReadPath = path;
                var lithologyDataTable = ImportIODPDataTable(dtReader);

                var ConvertedLithologyCache = LithologyConvertor.Convert(lithologyDataTable);

                foreach (var record in ConvertedLithologyCache.GetCollection())
                {
                    ImportMetaData(metaData, record.Value);
                    LithologyCache.Add(record.Key, record.Value);
                }

                ExportFileName = filename;
                ExportCache(ConvertedLithologyCache);

            }

            return LithologyCache;
        }

        public void ExportCache(Cache<string, LithologicDescription> cache)
        {
            if (cache.GetCollection().Count > 1)
            {
               
                System.IO.Directory.CreateDirectory(ExportDirectory);
                var exporter = new CSVReader();
                exporter.WritePath = ExportDirectory + ExportFileName;
                exporter.Write(cache.First().DataRow.Table);


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
            if (metaData.Count() == 5)
            {
               
                description.DescriptionReport = metaData[1];
                description.DescriptionGroup = metaData[2];
                description.DescriptionTab = metaData[3];
            }
            else if (metaData.Count() > 5)
            {
                description.DescriptionReport = metaData[1] + " " + metaData[2];
                description.DescriptionGroup = metaData[3];
                description.DescriptionTab = metaData[4];
            }
        }
    }

}
