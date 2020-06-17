using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{
    /// <summary>
    /// An object representing an interval within a Lithologic Description interval.
    /// </summary>
    public class LithologicSubinterval : Interval
    {
        private int _lithologicSubID;
        public int LithologicSubID { get { return _lithologicSubID; } set { _lithologicSubID = value; } }

        /// <summary>
        /// Creates a new Lithologic Subinterval
        /// </summary>
        /// <param name="ID">An indentication number for the subinterval</param>
        public LithologicSubinterval(int ID)
        {
            this._lithologicSubID = ID;

            this.StartOffset = new OffsetInfo();
            this.EndOffset = new OffsetInfo();
        }

        /// <summary>
        /// Writes the Subinterval's offsets to the console/
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"Subinterval Start:{0}, End:{1}",StartOffset.Offset, EndOffset.Offset);
        }

    }
}
