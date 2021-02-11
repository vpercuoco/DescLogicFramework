using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DescLogicFramework
{
    /// <summary>
    /// An object used to represent an interval within a section.
    /// </summary>
    public class Interval
    {
        [NotMapped]
        private double _startOffset = -1; 

        [NotMapped]
        private double _endOffset = -1; 

        [NotMapped]
        private bool _startOffsetSet = false;

        [NotMapped]
        private bool _endOffsetSet = false;

        /// <summary>
        /// The top offset of the interval within the section.
        /// </summary>
      
        public double StartOffset { get { return _startOffset; } set{ _startOffset = value; _startOffsetSet = true; } } 
    

        /// <summary>
        /// The bottom offset of the interval within the section.
        /// </summary>
        
        public double EndOffset{ get {return _endOffset; } set{ _endOffset = value; _endOffsetSet = true;} }

        /// <summary>
        /// The core section identifying information.
        /// </summary>
        public SectionInfo SectionInfo { get; set; } 

        /// <summary>
        /// Determines if an interval falls completely or partially within this interval
        /// </summary>
        /// <param name="interval">The interval to compare against</param>
        /// <returns></returns>
        public bool Contains(Interval interval)
        {
            if (SectionInfo.Equals(interval.SectionInfo))
            {
                if (interval.StartOffset >= StartOffset && interval.EndOffset <= EndOffset)
                {
                    return true;
                }
                else if (interval.StartOffset <= StartOffset && interval.EndOffset <= EndOffset && interval.EndOffset >= StartOffset )
                {
                    return true;
                }
                else if (interval.StartOffset >= StartOffset && interval.EndOffset >= EndOffset && interval.StartOffset <= EndOffset)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Returns true if a Lithologic Description has had its Offsets properties assigned 
        /// </summary>
        public bool OffsetsSet()
        {
            return _startOffsetSet == true && _endOffsetSet == true ? true : false;
        }
    }
}
