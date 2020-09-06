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
    /// 
   // [Owned]
    public class SectionInfo : IEquatable<SectionInfo>
    {
        #region Fields
        private string _expedition;
        private string _site;
        private string _hole;
        private string _core;
        private string _section;
        private string _type;
        private string _sampleID;
        private int _sectionTextID;
        #endregion

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Expedition{ get {return _expedition;} set {_expedition = value.ToString(); }}
        public string Site{ get{return _site;} set{ _site = value.ToString(); }}
        public string Hole{ get{ return _hole;} set{_hole = value.ToString(); }}
        public string Core{ get{ return _core;} set{_core = value.ToString(); }}
        public string Type{ get{return _type;} set{_type = value.ToString(); }}
        public string Section{ get{ return _section;} set{ _section = value.ToString(); }}


        public string SampleID{ get{ return _sampleID;} set{ _sampleID = value; }}

        //The DatabaseGeneratedOption.None attribute prevents EF from creating autoincremented values for this property
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SectionTextID { get { return _sectionTextID; } set { _sectionTextID = value; } }

        #endregion

        /// <summary>
        /// Creates a SectionInfo object.
        /// </summary>
        public SectionInfo()
        {
           
        }
        /// <summary>
        /// Creates a SectionInfo object using the the hierarchal information contained in a sampleID.
        /// </summary>
        /// <param name="sampleID">A DescLogic SampleID</param>
        public SectionInfo(string sampleID)
        {
            ParseSampleID(sampleID);
        }

        /// <summary>
        /// Displays a string of SectionInfo properties.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"Exp: {0}, Site: {1}, Hole: {2}, Core: {3}, Type: {4}, Section: {5}", this.Expedition, this.Site, this.Hole, this.Core, this.Type, this.Section);
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
                    this._sampleID = sampleID;
                    this._expedition = null;
                    this._site = null;
                    this._hole = null;
                    this._core = null;
                    this._type = null;
                    this._section = null;
                    break;
                case 1:
                    this._sampleID = sampleID;
                    this._expedition = f[0]; //I may want to change in the iteration because I keep getting "No Sample" in the Exedition Column
                    this._site = null;
                    this._hole = null;
                    this._core = null;
                    this._type = null;
                    this._section = null;
                    break;
                case 2:
                    this._sampleID = sampleID;
                    this._expedition = f[0];
                    this._site = f[1].Substring(0, f[1].Length - 1);
                    this._hole = f[1].Substring(f[1].Length - 1, 1);
                    this._core = null;
                    this._type = null;
                    this._section = null;
                    break;
                case 3:
                    this._sampleID = sampleID;
                    this._expedition = f[0];
                    this._site = f[1].Substring(0, f[1].Length - 1);
                    this._hole = f[1].Substring(f[1].Length - 1, 1);
                    this._core = f[2].Substring(0, f[2].Length - 1);
                    this._type = f[2].Substring(f[2].Length - 1, 1);
                    this._section = null;
                    break;
                default:
                    this._sampleID = sampleID;
                    this._expedition = f[0];
                    this._site = f[1].Substring(0, f[1].Length - 1);
                    this._hole = f[1].Substring(f[1].Length - 1, 1);
                    this._core = f[2].Substring(0, f[2].Length - 1);
                    this._type = f[2].Substring(f[2].Length - 1, 1);
                    this._section = f[3];
                    break;
            }
        }
        /// <summary>
        /// Determines if two SectionInfo objects are identical
        /// </summary>
        /// <param name="sectionInfo">The SectionInfo to compare against</param>
        /// <returns></returns>
        public bool Equals(SectionInfo sectionInfo)
        {
            if (this.Expedition == sectionInfo.Expedition && this.Site == sectionInfo.Site && this.Hole == sectionInfo.Hole && this.Core == sectionInfo.Core && this.Type == sectionInfo.Type && this.Section == sectionInfo.Section)
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
