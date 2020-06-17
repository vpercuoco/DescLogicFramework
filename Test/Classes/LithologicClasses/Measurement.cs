using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace DescLogicFramework
{
    /// <summary>
    /// A measurement performed by an IODP instrument on material that has been described.
    /// </summary>
    public class Measurement : Interval
    {
        /// <summary>
        /// The datarow of the Measurement within an IODP LORE Report.
        /// </summary>
        public DataRow DataRow { get; set; }

        
        /// <summary>
        /// A lithologic description in which this Measurement was taken.
        /// </summary>
        public LithologicDescription LithologicDescription { get; set; }

        /// <summary>
        /// The subinterval of the the description in which the Measurement most closely lies.
        /// </summary>
        public LithologicSubinterval LithologicSubinterval { get; set; }

        public string InstrumentReport { get; set; } = "";
        public string InstrumentSystem { get; set; } = "";


        /// <summary>
        /// Creates a new Measurement object
        /// </summary>
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
