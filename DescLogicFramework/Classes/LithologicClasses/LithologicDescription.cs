﻿using System;
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
        public int ID { get; set; }

        [NotMapped]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LithologicID { get; set; } = "-1";

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionReport { get; set; } = string.Empty;

        [NotMapped]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionTab { get; set; } = string.Empty;

        [NotMapped]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionGroup { get; set; } = string.Empty;

        [NotMapped]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string DescriptionType { get; set; } = string.Empty;


        public List<DescriptionColumnValuePair> DescriptionColumnValues { get; } = new List<DescriptionColumnValuePair>();

        #endregion

        [NotMapped]
        private double _resolution = 1;

        public List<LithologicSubinterval> LithologicSubintervals { get; } = new List<LithologicSubinterval>();

        [NotMapped]
        private DataRow _datarow;

        [NotMapped]
        public DataRow DataRow 
        {
            get { return _datarow; }
            set {
                _datarow = value;
                
                foreach(DataColumn column in value.Table.Columns)
                    {
                    var pair = new DescriptionColumnValuePair() { ColumnName = column.ColumnName, Value = DataRow[column].ToString() };
                    DescriptionColumnValues.Add(pair);
                }
            }
        }

        /// <summary>
        /// Instantiates a new Lithologic Description
        /// </summary>
        public LithologicDescription() { SectionInfo = new SectionInfo();} //Issue with new SectionInfo Overwriting data from database

        public LithologicDescription(SectionInfo sectionInfo)
        {
            SectionInfo = sectionInfo;
        }
        /// <summary>
        /// Instantiates a new Lithologic Description 
        /// </summary>
        /// <param name="SampleID">A IODP sample ID to be parsed to fill the Lithologic Description's SectionInfo fields</param>
        public LithologicDescription(string SampleID) : this() { SectionInfo = new SectionInfo(SampleID); }

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
            //Cases: where start and end are the same
                //One Subinterval
            //Case: where start is less than end
                //If end-start is less than resolution: one subinterval
                //if end-start is greater than resoltuion: end-start/resolution rounded up to nearest int is the number of subintervals


            //Case: Accidentlly reversing End and Start Offset
            int subintervalCount = (int)Math.Ceiling((decimal)((EndOffset - StartOffset) / _resolution));

            if (subintervalCount == 0)
            {
                LithologicSubinterval subinterval = new LithologicSubinterval(0, this);
                LithologicSubintervals.Add(subinterval);
            }
            else
            {
                for (int currentSubintervalID = 1; currentSubintervalID <= subintervalCount; currentSubintervalID++)
                {
                    LithologicSubinterval subinterval = new LithologicSubinterval(currentSubintervalID, this);

                    //Problem here is that you possibly create an extra longer interval because of the rounding error with the resolution and interval start-end distance.
                    //I'll leave it in because the measurement is based off the Description Interval--which is correct. The subinterval offsets are not output to file.
                    subinterval.StartOffset = StartOffset + _resolution * (currentSubintervalID - 1);
                    subinterval.EndOffset = StartOffset + _resolution * currentSubintervalID;
                    //need to determine how subintervals will be uniquely named
                    LithologicSubintervals.Add(subinterval);
                }
            }
            
        }

    }
}
