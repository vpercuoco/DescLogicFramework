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
        public string DescriptionReport { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionTab { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionGroup { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionType { get; set; } = "";

        public List<DescriptionColumnValuePair> Data { get; set; } = new List<DescriptionColumnValuePair>();

        #endregion


        // [Key]
        //  public int ID { get; set; }

        [NotMapped]
        private DataRow _dataRow;

        //Default resolution of 1cm
        [NotMapped]
        private double _resolution = 1;

        public List<LithologicSubinterval> LithologicSubintervals
        {
            get { return _lithologicSubintervals; }

            set { _lithologicSubintervals = value; }

        }

        private List<LithologicSubinterval> _lithologicSubintervals;// = new List<LithologicSubinterval>();


        [NotMapped]
        public DataRow DataRow {
            get { return _dataRow; }
            set {
                _dataRow = value;

                foreach (DataColumn column in value.Table.Columns)
                {
                    
                    Data.Add(new DescriptionColumnValuePair() { ColumnName = column.ColumnName, Value = value[column].ToString() });
                }
            }
        }

        /// <summary>
        /// Instantiates a new Lithologic Description
        /// </summary>
        public LithologicDescription()
        {
            SectionInfo = new SectionInfo();
            StartOffset = new OffsetInfo(SectionInfo);
            EndOffset = new OffsetInfo(SectionInfo);

            _lithologicSubintervals = new List<LithologicSubinterval>();
        }
        /// <summary>
        /// Instantiates a new Lithologic Description 
        /// </summary>
        /// <param name="SampleID">A IODP sample ID to be parsed to fill the Lithologic Description's SectionInfo fields</param>
        public LithologicDescription(string SampleID) : this()
        {
            SectionInfo.ParseSampleID(SampleID);
          //  DescriptionSectionInfo = this.SectionInfo;
        }

        /// <summary>
        /// Returns the Lithologic Subinterval for an offset within the bounds of this Lithologic Description.
        /// </summary>
        /// <param name="offsetInfo">The OffsetInfo to reference</param>
        /// <returns>LithologicSubinterval</returns>
        public LithologicSubinterval GetSubinterval(Interval interval)
        {

            LithologicSubinterval query = _lithologicSubintervals.FirstOrDefault(z => z.Contains(interval));
            return query;
        }


        /// <summary>
        /// Creates a collection of Lithologic Subintervals of a specific resolution.
        /// </summary>
        public void GenerateSubintervals()
        {
            int subintervalCount = (int)Math.Ceiling((this.EndOffset.Offset - this.StartOffset.Offset) / _resolution);

            for (int currentSubintervalID = 1; currentSubintervalID <= subintervalCount; currentSubintervalID++)
            {
                LithologicSubinterval subinterval = new LithologicSubinterval(currentSubintervalID, this);

                //Problem here is that you possibly create an extra longer interval because of the rounding error with the resolution and interval start-end distance.
                //I'll leave it in because the measurement is based off the Description Interval--which is correct. The subinterval offsets are not output to file.
                subinterval.StartOffset.Offset = this.StartOffset.Offset + _resolution*(currentSubintervalID-1);
                subinterval.EndOffset.Offset = this.StartOffset.Offset + _resolution * currentSubintervalID;
                //need to determine how subintervals will be uniquely named
                _lithologicSubintervals.Add(subinterval);
            }
        }

    }
}
