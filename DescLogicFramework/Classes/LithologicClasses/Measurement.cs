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
        public Guid ID { get; set; }
      
        public List<MeasurementColumnValuePair> Data { get; set; } = new List<MeasurementColumnValuePair>();

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

        //[MaxLength(50)]
        //[Column(TypeName = "int")]
        [NotMapped]
        public LithologicSubinterval LithologicSubinterval
        {
            get { return _lithologicSubInterval; }
            set
            {
                _lithologicSubInterval = value;

                if (value != null)
                {
                    this.LithologicSubID = value.LithologicSubID;
                    foreach (var item in this.Data)
                    {
                        item.LithologicSubID = value.LithologicSubID;
                    }
                }
            }
        }

        [NotMapped]
        private DataRow _dataRow;

        /// <summary>
        /// The datarow of the Measurement within an IODP LORE Report.
        /// </summary>
        /// 
        [NotMapped]
        public DataRow DataRow
        {
            get { return _dataRow; }
            set
            {
                _dataRow = value;

                foreach (DataColumn column in value.Table.Columns)
                {

                    Data.Add(new MeasurementColumnValuePair() { ColumnName = column.ColumnName, Value = value[column].ToString() });
                }
            }
        }


        /// <summary>
        /// A lithologic description in which this Measurement was taken.
        /// </summary>
        [NotMapped]
        public LithologicDescription LithologicDescription {
            get { return _lithologicDescription; } 
            set { _lithologicDescription = value ;
                if (value != null)
                {
                    this.LithologicID = value.LithologicID;
                    foreach (var item in this.Data)
                    {
                        item.LithologicID = value.LithologicID;
                    }
                } 
            } 
        }

        [NotMapped]
        private LithologicDescription _lithologicDescription { get; set; }

        [NotMapped]
        private LithologicSubinterval _lithologicSubInterval { get; set; }

        public Measurement()
        {
            StartOffset = new OffsetInfo();
            EndOffset = new OffsetInfo();
            SectionInfo = new SectionInfo();
        }

        /// <summary>
        /// Duplicates a Measurement. Creates a new Measurement by copying the properties of another Measurement.
        /// </summary>
        /// <param name="measurement">The Measurement object to copy.</param>
        public Measurement(Measurement measurement) : this()
        {
            SectionInfo = measurement.SectionInfo;
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
            if (Description.DataRow.Table.Columns.Contains(descriptionColumnName))
            {
                this.AddValueToColumn(Description.DataRow[descriptionColumnName].ToString(), columnName);
            }
        }
    }
}
