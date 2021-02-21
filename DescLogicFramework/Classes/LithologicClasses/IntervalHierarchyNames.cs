using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{
    public class IntervalHierarchyNames
    {
        public string Expedition { get; set; } = "Expedition";
        public string Site { get; set; } = "Site";
        public string Hole { get; set; } = "Hole";
        public string Core { get; set; } = "Core";
        public string Type { get; set; } = "Type";
        public string Section { get; set; } = "Section";
        public string Half { get; set; } = "Half";
        public string ParentTextID { get; set; } = "Parent";
        public string WorkingTextID { get; set; } = "TextID";
        public string ArchiveTextID { get; set; } = "TextID";
        public string SampleID { get; set; } = "SampleID";
        public string Offset { get; set; } = "Offset (cm)";
        public string TopOffset { get; set; } = "Top Offset (cm)";
        public string BottomOffset { get; set; } = "Bottom Offset (cm)";
        public string TextID { get; set; } = "TextID";
        public string TestNumber { get; set; } = "Test No.";

    }

    public class CarbHierachNames : IntervalHierarchyNames
    {
        new public string TopOffset { get; set; } = "A";
       

    }


    public class IntervalHierarchyValues
    {
       public string Expedition { get; set; } = "-1";
        public string Site { get; set; } = "-1";
        public string Hole { get; set; } = "-1";
        public string Core { get; set; } = "-1";
       public string Type { get; set; } = "-1";
       public string Section { get; set; } = "-1";
        public string Half { get; set; } = "-1";
       public string ParentTextID { get; set; } = "-1";
        public string WorkingTextID { get; set; } = "-1";
         public string ArchiveTextID { get; set; } = "-1";
         public string SampleID { get; set; } = "-1";
         public string Offset { get; set; } = "-1";
        public string TopOffset { get; set; } = "-1";
         public string BottomOffset { get; set; } = "-1";
         public string TextID { get; set; } = "-1";
        public string TestNumber { get; set; } = "-1";
    }
}
