using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public LithologicSubinterval()
        {

        }

        /// <summary>
        /// Creates a new Lithologic Subinterval
        /// </summary>
        /// <param name="ID">An identification number for the subinterval</param>
        public LithologicSubinterval(int subID, LithologicDescription Description)
        {
            _ = Description ?? throw new ArgumentNullException(nameof(Description));

            LithologicSubID = subID;
            SectionInfo = Description.SectionInfo;
            StartOffset = new OffsetInfo(Description.SectionInfo);
            EndOffset = new OffsetInfo(Description.SectionInfo);
        }

        /// <summary>
        /// Writes the Subinterval's offsets to the console/
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"Subinterval Start:{0}, End:{1}", StartOffset.Offset, EndOffset.Offset);
        }

    }
}
