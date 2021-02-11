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
            else
            {
                Sections.Add(section);
                return section;
            }
        }

        public DataTable SectionsDatatable { get; set; } = ImportAllSections();
        public static DataTable ImportAllSections()
        {
            var dataTableReader = new CSVReader();
            dataTableReader.ReadPath = ConfigurationManager.AppSettings["AllSectionsFile"];
            return dataTableReader.Read();

        }

        public void ParseSectionInfoFromDataTable(DataTable sectionsDatatable, SampleHierarchy hierarchyNames)
        {


            foreach (DataRow row in sectionsDatatable.Rows)
            {
                try
                {
                    SectionInfo section = new SectionInfo();
                    section.Expedition = row[hierarchyNames.Expedition].ToString();
                    section.Site = row[hierarchyNames.Site].ToString();
                    section.Hole = row[hierarchyNames.Hole].ToString();
                    section.Core = row[hierarchyNames.Core].ToString();
                    section.Type = row[hierarchyNames.Type].ToString();
                    section.Section = row[hierarchyNames.Section].ToString();
                    //section.Half = row[hierarchyNames.Half].ToString();
                    section.ParentTextID = row[hierarchyNames.ParentTextID].ToString();
                    section.ArchiveTextID = row[hierarchyNames.ArchiveTextID].ToString();
                    section.WorkingTextID = row[hierarchyNames.WorkingTextID].ToString();


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
