using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using DescLogicFramework.DataAccess;
using System.Configuration;

namespace DescLogicFramework
{
    public class SectionInfoCollection
    {
        public ICollection<SectionInfo> Sections { get; } = new HashSet<SectionInfo>();

        public SectionInfoCollection() { }

        public SectionInfo GetExistingElseAddAndGetCurrentSection(SectionInfo section)
        {
            var matchedSection = Sections.Where(x => section.Equals(x)).FirstOrDefault();

            if (matchedSection != null)
            {
                return matchedSection;
            }

            Sections.Add(section);
            return section;

        }

        public static DataTable ImportAllSections(string fileName)
        {
            var dataTableReader = new CSVReader();
            dataTableReader.ReadPath = fileName;
            return dataTableReader.Read();

        }

        public void ParseSectionInfoFromDataTable(DataTable sectionsDatatable, IntervalHierarchyNames hierarchyNames)
        {

            foreach (DataRow row in sectionsDatatable.Rows)
            {
                try
                {
                    IntervalHierarchyValues values = Importer.GetHierarchyValuesFromDataRow(row, hierarchyNames);
                    SectionInfo section = new SectionInfo(values);
                    Sections.Add(section);
                }
                catch (Exception)
                {

                    throw new Exception("Error parsing SectionInfo from data row");
                }
            }
        }
    }
}
