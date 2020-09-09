using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DescLogicFramework
{
    public class SectionInfoCollection
    {
        public List<SectionInfo> Sections { get; } = new List<SectionInfo>();

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
    }
}
