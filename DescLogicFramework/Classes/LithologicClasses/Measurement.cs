using System;
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

        [NotMapped]
        private List<MeasurementColumnValuePair> _backing = new List<MeasurementColumnValuePair>();


        public List<MeasurementColumnValuePair> Data
        {
            get
            {
                if (_backing.Count == 0)
                {
                    foreach (DataColumn column in DataRow.Table.Columns)
                    {
                        var pair = new MeasurementColumnValuePair() { ColumnName = column.ColumnName, Value = DataRow[column].ToString() };
                        pair.LithologicID = LithologicID;
                        pair.LithologicSubID = LithologicSubID;
                        _backing.Add(pair);
                    }

                }
                return _backing;
            }
        }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LithologicID { get; set; }

        public int? LithologicSubID { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string InstrumentReport { get; set; } = "";

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string InstrumentSystem { get; set; } = "";

        #endregion

        public LithologicSubinterval LithologicSubinterval
        {
            get { return _lithologicSubInterval; }
            set
            {
                _lithologicSubInterval = value;
                if (value != null)
                {
                   LithologicSubID = value.LithologicSubID;
                }
              
            }
        }
        /// <summary>
        /// The collection of measurements contained within this subinterval
        /// </summary>
        public virtual ICollection<LithologicSubinterval> LithologicSubintervals { get; set; }

        /// <summary>
        /// The datarow of the Measurement within an IODP LORE Report.
        /// </summary>
        [NotMapped]
        public DataRow DataRow { get; set; }

        /// <summary>
        /// A lithologic description in which this Measurement was taken.
        /// </summary>
        [NotMapped]
        public LithologicDescription LithologicDescription {
            get { return _lithologicDescription; } 
            set { _lithologicDescription = value ;
                if (value != null)
                {
                    LithologicID = value.LithologicID;
                } 
            } 
        }

        [NotMapped]
        private LithologicDescription _lithologicDescription { get; set; }

        [NotMapped]
        private LithologicSubinterval _lithologicSubInterval { get; set; }

        public Measurement() { }

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

        /// <summary>
        /// Updates a value to a column in the Measurement's Datarow  
        /// </summary>
        private void AddValueToColumn(string valueToAdd, string columnName)
        {
            if (!DataRow.Table.Columns.Contains(columnName))
            {
                DataRow.Table.Columns.Add(columnName);
            }
            DataRow[columnName] = valueToAdd;
        }

        /// <summary>
        /// Updates a value in the Measurement's DataRow from a column in its corresponding Lithologic Description.
        /// </summary>
        private void AddValueToColumn(LithologicDescription Description, string descriptionColumnName, string columnName)
        {
            _ = Description ?? throw new ArgumentNullException(nameof(Description));

            if (Description.DataRow.Table.Columns.Contains(descriptionColumnName))
            {
                AddValueToColumn(Description.DataRow[descriptionColumnName].ToString(), columnName);
            }
        }
    }
}
