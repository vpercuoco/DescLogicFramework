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
        public double Offset { get; set; } = -1;

        public SectionInfo SectionInfo { get; set; }

        public OffsetInfo()
        {
           SectionInfo = new SectionInfo();
        }

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
