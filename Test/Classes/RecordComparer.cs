using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data;


namespace DescLogicFramework
{

    //Create a measurment table interface in which you specify the hierarcy info. The individual track systems can inherit from it.
    //Maybe its acutally a delegate that I want to use
    public delegate string GetColumnHeader(string name);


    public struct HierarchyInfo : IEquatable<HierarchyInfo>
    {
        //public string Sample, Expedition, Site, Hole, Core, Type, Section;
        public string Sample { get; set; }
        public string Expedition { get; set; }
        public string Site { get; set; }
        public string Hole { get; set; }
        public string Core { get; set; }
        public string Type { get; set; }
        public string Section { get; set; }


        public bool Equals(HierarchyInfo hi)
        {
            if ((Sample == hi.Sample) && (Expedition == hi.Expedition) && (Site == hi.Site) && (Hole == hi.Hole) && (Core == hi.Core) && (Type == hi.Type) && (Section == hi.Section))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public HierarchyInfo(string SampleInfo)
        {
            string[] f = SampleInfo.Split("-");
            switch (f.Length)
            {
                case 0:
                    Sample = SampleInfo;
                    Expedition = null;
                    Site = null;
                    Hole = null;
                    Core = null;
                    Type = null;
                    Section = null;
                    break;
                case 1:
                    Sample = SampleInfo;
                    Expedition = f[0]; //I may want to change in the iteration because I keep getting "No Sample" in the Exedition Column
                    Site = null;
                    Hole = null;
                    Core = null;
                    Type = null;
                    Section = null;
                    break;
                case 2:
                    Sample = SampleInfo;
                    Expedition = f[0];
                    Site = f[1].Substring(0, f[1].Length - 1);
                    Hole = f[1].Substring(f[1].Length - 1, 1);
                    Core = null;
                    Type = null;
                    Section = null;
                    break;
                case 3:
                    Sample = SampleInfo;
                    Expedition = f[0];
                    Site = f[1].Substring(0, f[1].Length - 1);
                    Hole = f[1].Substring(f[1].Length - 1, 1);
                    Core = f[2].Substring(0, f[2].Length - 1);
                    Type = f[2].Substring(f[2].Length - 1, 1);
                    Section = null;
                    break;
                default:
                    Sample = SampleInfo;
                    Expedition = f[0];
                    Site = f[1].Substring(0, f[1].Length - 1);
                    Hole = f[1].Substring(f[1].Length - 1, 1);
                    Core = f[2].Substring(0, f[2].Length - 1);
                    Type = f[2].Substring(f[2].Length - 1, 1);
                    Section = f[3];
                    break;

            }
        }
    }

    public class IntervalInfo
    {
        public OffsetInfo OffsetStart { get; set; }
        public OffsetInfo OffsetEnd { get; set; }
    }

    public class Sample
    {
        public HierarchyInfo hierarchyInfo { get; set; }
        public OffsetInfo offsetInfo { get; set; }
        public IntervalInfo intervalInfo { get; set; }

        //Three unique ids for database keys according to the Expedition Hierarchy down to section
        //The lithologic description interval the measurement falls within
        //and the sub lithologic description interval the measurement falls within
        public string HierarchyID { get; set; }
        public string LithologicID { get; set; }
        public string SubLithologicID { get; set; }

        public Sample(HierarchyInfo hi, OffsetInfo oi, IntervalInfo ii)
        {
            hierarchyInfo = hi;
            offsetInfo = oi;
            intervalInfo = ii;
        }
    }

}
