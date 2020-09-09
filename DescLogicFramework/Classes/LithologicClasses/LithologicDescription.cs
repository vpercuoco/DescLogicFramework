using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DescLogicFramework
{

    /// <summary>
    /// Represents a single lithologic description over a depth interval.
    /// </summary>
    public class LithologicDescription : Interval
    {

        #region EFCoreProperties
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LithologicID { get; set; } = "-1";

        //MetaData from the imported Descklogic ExcelFiles
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionReport { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionTab { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionGroup { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionType { get; set; } = string.Empty;

        [NotMapped]
        private List<DescriptionColumnValuePair> _backing = new List<DescriptionColumnValuePair>();

        public List<DescriptionColumnValuePair> Data
        {
            get
            {
                if (_backing.Count == 0)
                {
                    foreach (DataColumn column in DataRow.Table.Columns)
                    {
                        var pair = new DescriptionColumnValuePair() { ColumnName = column.ColumnName, Value = DataRow[column].ToString() };
                        _backing.Add(pair);
                    }

                }
                return _backing;
            }
        }

        #endregion

        [NotMapped]
        private double _resolution = 1;

        public List<LithologicSubinterval> LithologicSubintervals { get; } = new List<LithologicSubinterval>();

        [NotMapped]
        public DataRow DataRow { get; set; }

        //TODO: Might as well get rid of offsetinfo objects and just implement offset properties in Measurments and Descriptions
        /// <summary>
        /// Instantiates a new Lithologic Description
        /// </summary>
        public LithologicDescription() { SectionInfo = new SectionInfo();}

        /// <summary>
        /// Instantiates a new Lithologic Description 
        /// </summary>
        /// <param name="SampleID">A IODP sample ID to be parsed to fill the Lithologic Description's SectionInfo fields</param>
        public LithologicDescription(string SampleID) : this() { SectionInfo.ParseSampleID(SampleID); }

        /// <summary>
        /// Returns the Lithologic Subinterval for an offset within the bounds of this Lithologic Description.
        /// </summary>
        public LithologicSubinterval GetSubinterval(Interval interval)
        {
            return LithologicSubintervals.FirstOrDefault(z => z.Contains(interval)); 
        }

        /// <summary>
        /// Creates a collection of Lithologic Subintervals of a specific resolution.
        /// </summary>
        public void GenerateSubintervals()
        {
            int subintervalCount = (int)Math.Ceiling((EndOffset- StartOffset) / _resolution);

            for (int currentSubintervalID = 1; currentSubintervalID <= subintervalCount; currentSubintervalID++)
            {
                LithologicSubinterval subinterval = new LithologicSubinterval(currentSubintervalID, this);

                //Problem here is that you possibly create an extra longer interval because of the rounding error with the resolution and interval start-end distance.
                //I'll leave it in because the measurement is based off the Description Interval--which is correct. The subinterval offsets are not output to file.
                subinterval.StartOffset = StartOffset + _resolution*(currentSubintervalID-1);
                subinterval.EndOffset = StartOffset + _resolution * currentSubintervalID;
                //need to determine how subintervals will be uniquely named
                LithologicSubintervals.Add(subinterval);
            }
        }

    }
}
