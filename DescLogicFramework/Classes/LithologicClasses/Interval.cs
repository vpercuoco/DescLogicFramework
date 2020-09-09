using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DescLogicFramework
{
    /// <summary>
    /// An object used to represent an interval within a section.
    /// </summary>
    /// 
    public class Interval
    {
        [NotMapped]
        private OffsetInfo _startOffset; 

        [NotMapped]
        private OffsetInfo _endOffset; 

        [NotMapped]
        private bool _startOffsetSet = false;

        [NotMapped]
        private bool _endOffsetSet = false;

        /// <summary>
        /// The top offset of the interval within the section.
        /// </summary>
        [NotMapped]
        public OffsetInfo StartOffset { get { return _startOffset; } set{ _startOffset = value; _startOffsetSet = true; } } 
    

        /// <summary>
        /// The bottom offset of the interval within the section.
        /// </summary>
        [NotMapped]
        public OffsetInfo EndOffset{ get {return _endOffset; } set{ _endOffset = value; _endOffsetSet = true;} }

        /// <summary>
        /// The core section identifying information.
        /// </summary>

      
        public SectionInfo SectionInfo { get; set; } 

        /// <summary>
        /// Determines if an offset falls within the interval offsets.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool Contains(OffsetInfo offset)
        {
            _ = offset ?? throw new ArgumentNullException(nameof(offset));

           return offset.Offset >= StartOffset.Offset && offset.Offset <= EndOffset.Offset && SectionInfo.Equals(offset.SectionInfo) ? true : false;      
        }

        /// <summary>
        /// Determines if an interval falls completely or partially within this interval
        /// </summary>
        /// <param name="interval">The interval to compare against</param>
        /// <returns></returns>
        public bool Contains(Interval interval)
        {
            if (this.SectionInfo.Equals(interval.SectionInfo))
            {
                //If the interval is totally within the lithologic description
                if (interval.StartOffset.Offset >= StartOffset.Offset && interval.EndOffset.Offset <= EndOffset.Offset)
                {
                    return true;
                }
                //if the intervals is partially within, topside. Start <= start, End <= end, End >= start
                else if (interval.StartOffset.Offset <= StartOffset.Offset && interval.EndOffset.Offset <= EndOffset.Offset && interval.EndOffset.Offset >= StartOffset.Offset )
                {
                    return true;
                }
                //if the interval is partially within, bottomside. Start >= start, End >= end, Start <= end
                else if (interval.StartOffset.Offset >= StartOffset.Offset && interval.EndOffset.Offset >= EndOffset.Offset && interval.StartOffset.Offset <= EndOffset.Offset)
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
        /// <returns></returns>
        public bool OffsetsSet()
        {
            return _startOffsetSet == true && _endOffsetSet == true ? true : false;
        }
    }
}
