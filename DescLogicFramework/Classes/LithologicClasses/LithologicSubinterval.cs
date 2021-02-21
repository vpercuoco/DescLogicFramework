using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;


namespace DescLogicFramework
{
    /// <summary>
    /// An object representing an interval within a Lithologic Description interval.
    /// </summary>
    public class LithologicSubinterval : Interval
    {
        [Key]
        public int ID { get; set; }

        public int? LithologicSubID { get; set; }

        public LithologicDescription LithologicDescription { get; set; }

        public ICollection<Measurement> Measurements { get; set; }// = new HashSet<Measurement>();      


        public LithologicSubinterval() { }

        /// <summary>
        /// Creates a new Lithologic Subinterval
        /// </summary>
        public LithologicSubinterval(int subID, LithologicDescription Description) : this()
        {
            _ = Description ?? throw new ArgumentNullException(nameof(Description));

            LithologicSubID = subID;
            SectionInfo = Description.SectionInfo;
        }

        /// <summary>
        /// Writes the Subinterval's offsets to the console/
        /// </summary>
        public override string ToString()
        {    
            return string.Format(CultureInfo.CurrentCulture, @"Subinterval Start:{0}, End:{1}",  StartOffset?.ToString(CultureInfo.CurrentCulture), EndOffset?.ToString(CultureInfo.CurrentCulture));
        }

    }
}
