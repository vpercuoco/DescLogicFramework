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
        /// The offset (cm) on a section
        /// </summary>
        public double Offset { get; set; } = -1;

        /// <summary>
        /// Creates a new OffsetInfo object
        /// </summary>
        public OffsetInfo()
        {
           SectionInfo = new SectionInfo();
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
