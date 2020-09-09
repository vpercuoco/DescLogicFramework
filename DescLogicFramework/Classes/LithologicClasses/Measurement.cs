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

        [Key]
        public int ID { get; set; }
      
      //  public List<MeasurementColumnValuePair> Data { get; set; } = new List<MeasurementColumnValuePair>();

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
                        pair.LithologicID = this.LithologicID;
                        pair.LithologicSubID = this.LithologicSubID;
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
        /// The datarow of the Measurement within an IODP LORE Report.
        /// </summary>
        /// 
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

        public Measurement()
        {

        }

        public Measurement(SectionInfo sectionInfo) : this()
        {
            SectionInfo = sectionInfo;
           // StartOffset = new OffsetInfo(SectionInfo);
          //  EndOffset = new OffsetInfo(SectionInfo);
            
        }

        /// <summary>
        /// Duplicates a Measurement. Creates a new Measurement by copying the properties of another Measurement.
        /// </summary>
        /// <param name="measurement">The Measurement object to copy.</param>
        public Measurement(Measurement measurement) : this(measurement?.SectionInfo ?? throw new ArgumentNullException(nameof(measurement)))
        {

            StartOffset = measurement.StartOffset;
            EndOffset = measurement.EndOffset;
            DataRow = measurement.DataRow;
        }

        /// <summary>
        /// Updates a value to a column in the Measurement's Datarow  
        /// </summary>
        /// <param name="valueToAdd">A column in the Lithologic Description DataRow</param>
        /// <param name="columnName">A column in the Measurement's DataRow</param>
        public void AddValueToColumn(string valueToAdd, string columnName)
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
        /// <param name="Description">The Measurement's Lithologic Description</param>
        /// <param name="descriptionColumnName">A column in the Lithologic Description DataRow</param>
        /// <param name="columnName">A column in the Measurement's DataRow</param>
        public void AddValueToColumn(LithologicDescription Description, string descriptionColumnName, string columnName)
        {
            _ = Description ?? throw new ArgumentNullException(nameof(Description));

            if (Description.DataRow.Table.Columns.Contains(descriptionColumnName))
            {
                this.AddValueToColumn(Description.DataRow[descriptionColumnName].ToString(), columnName);
            }
        }
    }
}
