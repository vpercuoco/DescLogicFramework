using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DescLogicFramework
{
    /// <summary>
    /// An object used to identify a core section.
    /// </summary>
    public class SectionInfo : IEquatable<SectionInfo>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Expedition { get; set; }
        public string Site { get; set; }
        public string Hole { get; set; }
        public string Core { get; set; }
        public string Type { get; set; }
        public string Section { get; set; }
        public string SampleID { get; set; }
        public string Half { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ParentTextID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ArchiveTextID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string WorkingTextID { get; set; }

        public SectionInfo() { }

        /// <summary>
        /// Creates a SectionInfo object using the the hierarchal information contained in a sampleID.
        /// </summary>
        public SectionInfo(string sampleID)
        { 
            GetPropertiesFromSampleHierarchy(Importer.ParseSampleID(sampleID)); 
        }

        public SectionInfo(IntervalHierarchyValues sampleHierarchy)
        {
            GetPropertiesFromSampleHierarchy(sampleHierarchy);
        }


        public void GetPropertiesFromSampleHierarchy(IntervalHierarchyValues hierarchy)
        {
            SampleID = hierarchy.SampleID;
            Expedition = hierarchy.Expedition;
            Site = hierarchy.Site;
            Hole = hierarchy.Hole;
            Core = hierarchy.Core;
            Type = hierarchy.Type;
            Section = hierarchy.Section;
            Half = hierarchy.Half;
            ParentTextID = hierarchy.ParentTextID;
            WorkingTextID = hierarchy.WorkingTextID;
            ArchiveTextID = hierarchy.ArchiveTextID;
        }
       
        /// <summary>
        /// Determines if two SectionInfo objects are identical
        /// </summary>
        public bool Equals(SectionInfo sectionInfo)
        {
            if (Expedition == sectionInfo.Expedition && 
                Site == sectionInfo.Site && 
                Hole == sectionInfo.Hole 
                && Core == sectionInfo.Core 
                && Type == sectionInfo.Type 
                && Section == sectionInfo.Section)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Displays a string of SectionInfo properties.
        /// </summary>
        public override string ToString()
        {
            return string.Format(@"Exp: {0}, Site: {1}, Hole: {2}, Core: {3}, Type: {4}, Section: {5}", Expedition, Site, Hole, Core, Type, Section);
        }

    }
}
