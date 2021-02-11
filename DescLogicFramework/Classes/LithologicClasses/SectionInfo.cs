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
        public SectionInfo(string sampleID) { ParseSampleID(sampleID); }

        /// <summary>
        /// Displays a string of SectionInfo properties.
        /// </summary>
        public override string ToString()
        {
            return string.Format(@"Exp: {0}, Site: {1}, Hole: {2}, Core: {3}, Type: {4}, Section: {5}", Expedition, Site, Hole, Core, Type, Section);
        }

        /// <summary>
        /// Separates a sample ID into its components, then assigns to properties.
        /// </summary>
        /// <param name="sampleID">DescLogic Sample ID</param>
        public void ParseSampleID(string sampleID)
        {
            _ = sampleID ?? throw new ArgumentNullException(nameof(sampleID));

            string[] sampleIDComponents = sampleID.Split("-");
            try
            {
                switch (sampleIDComponents.Length)
                {
                    case 0:
                        SampleID = sampleID;
                        Expedition = null;
                        Site = null;
                        Hole = null;
                        Core = null;
                        Type = null;
                        Section = null;
                        Half = null;
                        break;
                    case 1:
                        SampleID = sampleID;
                        Expedition = sampleIDComponents[0]; //I may want to change in the iteration because I keep getting "No Sample" in the Exedition Column
                        Site = null;
                        Hole = null;
                        Core = null;
                        Type = null;
                        Section = null;
                        Half = null;
                        break;
                    case 2:
                        SampleID = sampleID;
                        Expedition = sampleIDComponents[0];
                        Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        Core = null;
                        Type = null;
                        Section = null;
                        Half = null;
                        break;
                    case 3:
                        SampleID = sampleID;
                        Expedition = sampleIDComponents[0];
                        Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        Section = null;
                        Half = null;
                        break;
                    case 4:
                        SampleID = sampleID;
                        Expedition = sampleIDComponents[0];
                        Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        Section = sampleIDComponents[3];
                        Half = null;
                        break;
                    case 5:
                        SampleID = sampleID;
                        Expedition = sampleIDComponents[0];
                        Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        Section = sampleIDComponents[3];
                        Half = CleanUpSectionHalf(sampleIDComponents[4].Split(" ")[0]);
                        break;
                    default:
                        SampleID = sampleID;
                        Expedition = sampleIDComponents[0];
                        Site = sampleIDComponents[1].Substring(0, sampleIDComponents[1].Length - 1);
                        Hole = sampleIDComponents[1].Substring(sampleIDComponents[1].Length - 1, 1);
                        Core = sampleIDComponents[2].Substring(0, sampleIDComponents[2].Length - 1);
                        Type = sampleIDComponents[2].Substring(sampleIDComponents[2].Length - 1, 1);
                        Section = sampleIDComponents[3];
                        Half = CleanUpSectionHalf(sampleIDComponents[4].Split(" ")[0]);
                        break;
                }
            }
            catch (Exception)
            {

                throw new Exception("Error Parsing SampleID");
            }
            
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

        private string CleanUpSectionHalf(string sectionHalf)
        {

            if (sectionHalf.Contains("PAL"))
            {
                return "PAL";
            }
            else
            {
                return sectionHalf;
            }
        }
    }
}
