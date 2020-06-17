using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{

    /// <summary>
    /// Defines a specific offset on a core section
    /// </summary>
    public class OffsetInfo : IEquatable<OffsetInfo>
    {
        /// <summary>
        /// Default value of an offset before it has been assigned.
        /// </summary>
        private double _offset = -1;
       
        /// <summary>
        /// The offset (cm) on a section
        /// </summary>
        public double Offset { get {return _offset;} set {_offset = value;} }

        /// <summary>
        /// Creates a new OffsetInfo object
        /// </summary>
        public OffsetInfo()
        {
            this.SectionInfo = new SectionInfo();
        }
        /// <summary>
        /// The section information from which the offset is derived.
        /// </summary>
        public SectionInfo SectionInfo { get; set; }

        /// <summary>
        /// Determines whether two offsets where taken at the same depth horizon within the same section.
        /// </summary>
        /// <param name="offsetInfo">The offset to compare against.</param>
        /// <returns></returns>
        public bool Equals(OffsetInfo offsetInfo)
        {
            if ((this.Offset == offsetInfo.Offset) && (this.SectionInfo.Equals(offsetInfo.SectionInfo)))
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
