using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace DescLogicFramework
{

    /// <summary>
    /// Represents a single lithologic description over a depth interval.
    /// </summary>
    public class LithologicDescription: Interval
    {
 
        //Default resolution of 1cm
        private double _resolution = 1;
        private List<LithologicSubinterval> _lithologicSubintervals;

        public DataRow DataRow { get; set; }
        public string LithologicID { get; set; } = "-1";


        //MetaData from the imported Descklogic ExcelFiles
        public string DescriptionReport { get; set; } = "";
        public string DescriptionTab { get; set; } = "";
        public string DescriptionGroup { get; set; } = "";
        public string DescriptionType { get; set; } = "";
     


        /// <summary>
        /// Instantiates a new Lithologic Description
        /// </summary>
        public LithologicDescription()
        {
            SectionInfo = new SectionInfo();
            StartOffset = new OffsetInfo();
            StartOffset.SectionInfo = new SectionInfo();

            EndOffset = new OffsetInfo();
            EndOffset.SectionInfo = new SectionInfo();
            
            _lithologicSubintervals = new List<LithologicSubinterval>();
            
            
        }
        /// <summary>
        /// Instantiates a new Lithologic Description 
        /// </summary>
        /// <param name="SampleID">A IODP sample ID to be parsed to fill the Lithologic Description's SectionInfo fields</param>
        public LithologicDescription(string SampleID) : this()
        {
            SectionInfo.ParseSampleID(SampleID); //At this point I need to reject the file if the sample iD says something like "No Data"
            _lithologicSubintervals = new List<LithologicSubinterval>();
        }

        /// <summary>
        /// Returns the Lithologic Subinterval for an offset within the bounds of this Lithologic Description.
        /// </summary>
        /// <param name="offsetInfo">The OffsetInfo to reference</param>
        /// <returns>LithologicSubinterval</returns>
        public LithologicSubinterval GetSubinterval(Interval interval)
        {

            LithologicSubinterval query = _lithologicSubintervals.FirstOrDefault(z => z.Contains(interval));


            /*//Create subintervals then rerun query::::This was my attempt to make it so a bunch of subintervals were not generated which would never be used, but it
             * didn't work correctly.
            if (query == null)
            {
             int firstSubIntervalID = (int)Math.Floor((interval.StartOffset.Offset - this.StartOffset.Offset) / _resolution);
             int lastSubIntervalID = (int)Math.Floor((interval.EndOffset.Offset - this.StartOffset.Offset) / _resolution);

                if (firstSubIntervalID != lastSubIntervalID)
                {
                    for (int i = firstSubIntervalID; i <= lastSubIntervalID; i++)
                    {
                        CreateSubInterval(i);
                    }
                }
            }

            query = _lithologicSubintervals.FirstOrDefault(z => z.Contains(interval)); */
            return query;
        }

        /// <summary>
        /// Sets the unique identification string for a Lithologic Description
        /// </summary>
        public void SetLithologicID()
        {
            LithologicIDGenerator generator = new LithologicIDGenerator();
            LithologicID = generator.GenerateID(this);
        }

        [Obsolete]
        public void CreateSubInterval(int ID)
        {
            LithologicSubinterval subinterval = new LithologicSubinterval(ID);
            subinterval.SectionInfo = this.SectionInfo;
            subinterval.StartOffset.SectionInfo = this.SectionInfo;
            subinterval.EndOffset.SectionInfo = this.SectionInfo;

            subinterval.StartOffset.Offset = this.StartOffset.Offset + _resolution * (ID - 1);
            subinterval.EndOffset.Offset = this.StartOffset.Offset + _resolution * ID;
            _lithologicSubintervals.Add(subinterval);
        }

        /// <summary>
        /// Determines the number to subintervals to create within a Lithologic Description.
        /// </summary>
        /// <returns></returns>
        private int CountSubintervals()
        {
            return (int)Math.Ceiling((this.EndOffset.Offset - this.StartOffset.Offset) / _resolution);
   
        }

        /// <summary>
        /// Creates a collection of Lithologic Subintervals of a specific resolution.
        /// </summary>
        public void GenerateSubintervals()
        {
            int count = this.CountSubintervals();
            for (int i = 1; i <= count; i++)
            {
                LithologicSubinterval subinterval = new LithologicSubinterval(i);
                subinterval.SectionInfo = this.SectionInfo;
                subinterval.StartOffset.SectionInfo = this.SectionInfo;
                subinterval.EndOffset.SectionInfo = this.SectionInfo;

                //Problem here is that you possibly create an extra longer interval because of the rounding error with the resolution and interval start-end distance.
                //I'll leave it in because the measurement is based off the Description Interval--which is correct. The subinterval offsets are not output to file.
                subinterval.StartOffset.Offset = this.StartOffset.Offset + _resolution*(i-1);
                subinterval.EndOffset.Offset = this.StartOffset.Offset + _resolution * i;
                //need to determine how subintervals will be uniquely named
                _lithologicSubintervals.Add(subinterval);
            }
        }

    }
}
