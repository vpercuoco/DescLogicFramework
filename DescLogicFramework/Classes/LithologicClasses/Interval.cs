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
        private OffsetInfo _startOffset; //= new OffsetInfo();

        [NotMapped]
        private OffsetInfo _endOffset; //= new OffsetInfo();

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

      
        public SectionInfo SectionInfo { get; set; } //= new SectionInfo();

        /// <summary>
        /// Determines if an offset falls within the interval offsets.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool Contains(OffsetInfo offset)
        {
            if (offset.Offset >= this.StartOffset.Offset && offset.Offset <= this.EndOffset.Offset && this.SectionInfo.Equals(offset.SectionInfo))
            {
                return true;
            }
            else
            {
                return false;
            }
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
                if (interval.StartOffset.Offset >= this.StartOffset.Offset && interval.EndOffset.Offset <= this.EndOffset.Offset)
                {
                    return true;
                }
                //if the intervals is partially within, topside. Start <= start, End <= end, End >= start
                else if (interval.StartOffset.Offset <= this.StartOffset.Offset && interval.EndOffset.Offset <= this.EndOffset.Offset && interval.EndOffset.Offset >= this.StartOffset.Offset )
                {
                    return true;
                }
                //if the interval is partially within, bottomside. Start >= start, End >= end, Start <= end
                else if (interval.StartOffset.Offset >= this.StartOffset.Offset && interval.EndOffset.Offset >= this.EndOffset.Offset && interval.StartOffset.Offset <= this.EndOffset.Offset)
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
            if (_startOffsetSet == true && _endOffsetSet == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
