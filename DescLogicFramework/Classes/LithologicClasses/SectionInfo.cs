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

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SectionTextID { get; set; }

        public SectionInfo() { }

        /// <summary>
        /// Creates a SectionInfo object using the the hierarchal information contained in a sampleID.
        /// </summary>
        public SectionInfo(string sampleID)
        {
            ParseSampleID(sampleID);
        }

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

            string[] f = sampleID.Split("-");
            switch (f.Length)
            {
                case 0:
                    SampleID = sampleID;
                    Expedition = null;
                    Site = null;
                    Hole = null;
                    Core = null;
                    Type = null;
                    Section = null;
                    break;
                case 1:
                    SampleID = sampleID;
                    Expedition = f[0]; //I may want to change in the iteration because I keep getting "No Sample" in the Exedition Column
                    Site = null;
                    Hole = null;
                    Core = null;
                    Type = null;
                    Section = null;
                    break;
                case 2:
                    SampleID = sampleID;
                    Expedition = f[0];
                    Site = f[1].Substring(0, f[1].Length - 1);
                    Hole = f[1].Substring(f[1].Length - 1, 1);
                    Core = null;
                    Type = null;
                    Section = null;
                    break;
                case 3:
                    SampleID = sampleID;
                    Expedition = f[0];
                    Site = f[1].Substring(0, f[1].Length - 1);
                    Hole = f[1].Substring(f[1].Length - 1, 1);
                    Core = f[2].Substring(0, f[2].Length - 1);
                    Type = f[2].Substring(f[2].Length - 1, 1);
                    Section = null;
                    break;
                default:
                    SampleID = sampleID;
                    Expedition = f[0];
                    Site = f[1].Substring(0, f[1].Length - 1);
                    Hole = f[1].Substring(f[1].Length - 1, 1);
                    Core = f[2].Substring(0, f[2].Length - 1);
                    Type = f[2].Substring(f[2].Length - 1, 1);
                    Section = f[3];
                    break;
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
    }
}
