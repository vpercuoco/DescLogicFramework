﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DescLogicFramework
{
    /// <summary>
    /// A measurement performed by an IODP instrument on material that has been described.
    /// </summary>
    public class Measurement : Interval
    {
        #region EFCoreProperties
        //TODO: Return a set of specified measurement key value pairs. Pretty much the measurment itself, textId, offsets, and testNo

        [Key]
        public int ID { get; set; }

        public List<MeasurementColumnValuePair> MeasurementData {get; set;} = new List<MeasurementColumnValuePair>();

        [NotMapped]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LithologicID { get { return LithologicDescription?.LithologicID ?? "-1"; } set {; } }

        [NotMapped]
        public int? LithologicSubID { get{  return LithologicSubinterval?.LithologicSubID ?? -1;}  set {; } }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string InstrumentReport { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string InstrumentSystem { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(15)")]
        public string TextID { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(15)")]
        public string TestNumber { get; set; } = "";

        [NotMapped]
        public LithologicSubinterval LithologicSubinterval { get; set; }

        /// <summary>
        /// The collection of measurements contained within this subinterval
        /// </summary>
        public ICollection<LithologicSubinterval> LithologicSubintervals { get; set; } = new HashSet<LithologicSubinterval>();

        [NotMapped]
        public ICollection<LithologicDescription> Descriptions { get; set; }

        #endregion

        [NotMapped]
        private DataRow _datarow; 

        /// <summary>
        /// The datarow of the Measurement within an IODP LORE Report.
        /// </summary>
        [NotMapped]
        public DataRow DataRow { 
            get 
            { return _datarow; } 
            set 
            {
                _datarow = value;
                if (MeasurementData.Count  >0 )
                {
                    MeasurementData.Clear();
                }
                foreach (DataColumn column in DataRow.Table.Columns)
                {
                    var pair = new MeasurementColumnValuePair() { ColumnName = column.ColumnName, Value = DataRow[column].ToString() };
                    pair.LithologicID = LithologicID;
                    pair.LithologicSubID = LithologicSubID;
                    MeasurementData.Add(pair);
                }
            }
        }

        /// <summary>
        /// A lithologic description in which this Measurement was taken.
        /// </summary>
        [NotMapped]
        public LithologicDescription LithologicDescription { get; set; } 

        public Measurement() 
        {
            //LithologicSubintervals = new HashSet<LithologicSubinterval>();
        }

        public Measurement(SectionInfo sectionInfo) : this() { SectionInfo = sectionInfo; }

        /// <summary>
        /// Duplicates a Measurement. Creates a new Measurement by copying the properties of another Measurement.
        /// </summary>
        public Measurement(Measurement measurement) : this(measurement?.SectionInfo ?? throw new ArgumentNullException(nameof(measurement)))
        {
            StartOffset = measurement.StartOffset;
            EndOffset = measurement.EndOffset;
            DataRow = measurement.DataRow;
        }

    }
}
