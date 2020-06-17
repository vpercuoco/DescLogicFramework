using Microsoft.VisualStudio.TestTools.UnitTesting;
using DescLogicFramework;
using System.Data;
using System.Text.RegularExpressions;

namespace DescLogicFrameworkTests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        public void Test_HierarchyInfo()
        {
            //I can start adding code now
            //, , , 
            var x1 = new DescLogicFramework.HierarchyInfo("351-U1438E-8R-4-W");
            Assert.IsTrue(x1.Sample == "351-U1438E-8R-4-W");
            Assert.IsTrue(x1.Expedition == "351");
            Assert.IsTrue(x1.Site == "U1438");
            Assert.IsTrue(x1.Hole == "E");
            Assert.IsTrue(x1.Core == "8");
            Assert.IsTrue(x1.Type == "R");
            Assert.IsTrue(x1.Section == "4");


            var x2 = new DescLogicFramework.HierarchyInfo("351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x2.Sample == "351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x2.Expedition == "351");
            Assert.IsTrue(x2.Site == "U1438");
            Assert.IsTrue(x2.Hole == "E");
            Assert.IsTrue(x2.Core == "72");
            Assert.IsTrue(x2.Type == "R");
            Assert.IsTrue(x2.Section == "1");


            var x3 = new DescLogicFramework.HierarchyInfo("352-U1442A-1R");
            Assert.IsTrue(x3.Sample == "352-U1442A-1R");
            Assert.IsTrue(x3.Expedition == "352");
            Assert.IsTrue(x3.Site == "U1442");
            Assert.IsTrue(x3.Hole == "A");
            Assert.IsTrue(x3.Core == "1");
            Assert.IsTrue(x3.Type == "R");
            Assert.IsTrue(x3.Section == null);

            var x4 = new DescLogicFramework.HierarchyInfo("352-U1442A-1R-CC-A");
            Assert.IsTrue(x4.Sample == "352-U1442A-1R-CC-A");
            Assert.IsTrue(x4.Expedition == "352");
            Assert.IsTrue(x4.Site == "U1442");
            Assert.IsTrue(x4.Hole == "A");
            Assert.IsTrue(x4.Core == "1");
            Assert.IsTrue(x4.Type == "R");
            Assert.IsTrue(x4.Section == "CC");

            var x5 = new DescLogicFramework.HierarchyInfo("352");
            Assert.IsTrue(x5.Sample == "352");
            Assert.IsTrue(x5.Expedition == "352");
            Assert.IsTrue(x5.Site == null);
            Assert.IsTrue(x5.Hole == null);
            Assert.IsTrue(x5.Core == null);
            Assert.IsTrue(x5.Type == null);
            Assert.IsTrue(x5.Section == null);

            var x6 = new DescLogicFramework.HierarchyInfo("351-U1438A-1H-1-KEND-RADS62");
            Assert.IsTrue(x6.Sample == "351-U1438A-1H-1-KEND-RADS62");
            Assert.IsTrue(x6.Expedition == "351");
            Assert.IsTrue(x6.Site == "U1438");
            Assert.IsTrue(x6.Hole == "A");
            Assert.IsTrue(x6.Core == "1");
            Assert.IsTrue(x6.Type == "H");
            Assert.IsTrue(x6.Section == "1");
           
            var x7 = new DescLogicFramework.HierarchyInfo("351-U1438E-56R-1-W 0/3-TSB(0-3)-TS_89");
            Assert.IsTrue(x7.Sample == "351-U1438E-56R-1-W 0/3-TSB(0-3)-TS_89");
            Assert.IsTrue(x7.Expedition == "351");
            Assert.IsTrue(x7.Site == "U1438");
            Assert.IsTrue(x7.Hole == "E");
            Assert.IsTrue(x7.Core == "56");
            Assert.IsTrue(x7.Type == "R");
            Assert.IsTrue(x7.Section == "1");


        }

        [TestMethod]
        public void Test_HierarchInfoEquals()
        {
            var x2 = new DescLogicFramework.HierarchyInfo("351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x2.Sample == "351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x2.Expedition == "351");
            Assert.IsTrue(x2.Site == "U1438");
            Assert.IsTrue(x2.Hole == "E");
            Assert.IsTrue(x2.Core == "72");
            Assert.IsTrue(x2.Type == "R");
            Assert.IsTrue(x2.Section == "1");

            var x3 = new DescLogicFramework.HierarchyInfo("351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x3.Sample == "351-U1438E-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x3.Expedition == "351");
            Assert.IsTrue(x3.Site == "U1438");
            Assert.IsTrue(x3.Hole == "E");
            Assert.IsTrue(x3.Core == "72");
            Assert.IsTrue(x3.Type == "R");
            Assert.IsTrue(x3.Section == "1");

            //Test Equality for equal Hierarchy info
            Assert.IsTrue(x2.Equals(x3));
            Assert.IsTrue(x3.Equals(x2));

            var x4 = new DescLogicFramework.HierarchyInfo("351-U1438A-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x4.Sample == "351-U1438A-72R-1-W 12/15-TSB(12-15)-TS_106");
            Assert.IsTrue(x4.Expedition == "351");
            Assert.IsTrue(x4.Site == "U1438");
            Assert.IsTrue(x4.Hole == "A");
            Assert.IsTrue(x4.Core == "72");
            Assert.IsTrue(x4.Type == "R");
            Assert.IsTrue(x4.Section == "1");

            //Test equality for non-equal Hierarchy info
            Assert.IsTrue(!x2.Equals(x4));
            Assert.IsTrue(!x4.Equals(x2));

        }

        [TestMethod]
        public void Test_LithologicIDGenerator()
        {
          //Trial 1:
            LithologicDescription ld = new LithologicDescription();
            ld.StartOffset = new OffsetInfo();
            ld.EndOffset = new OffsetInfo();

            ld.SectionInfo.Expedition = "379T";
            ld.SectionInfo.Site = "U1456A";
            ld.SectionInfo.Hole = "A";
            ld.SectionInfo.Core = "43";
            ld.SectionInfo.Type = "R";
            ld.SectionInfo.Section = "CC";
            ld.StartOffset.Offset = 20.01;
            ld.EndOffset.Offset = 5.25;

            LithologicIDGenerator generator = new LithologicIDGenerator();
            generator.GenerateID(ld);

            //I'm writing the output string with spaces to understand the format of the underlying numbers
            string result = ("379 20 1456 01 043 18 99 01 0020010 0005250").Replace(" ", string.Empty);
            //379 20 1456 01 043 18 99 01 0000100 0005250
            //379 20 1456 01 043 18 99 01 0010010 0005250
           // Assert.IsTrue(generator.GenerateOffset("5.25") == "0005" + "250");
            
            Assert.IsTrue(generator.GenerateID(ld) == result);

          //Trial 2:
            ld.SectionInfo.Expedition = "379";
            ld.SectionInfo.Site = "U1456C";
            ld.SectionInfo.Hole = "C";
            ld.SectionInfo.Core = "8";
            ld.SectionInfo.Type = "H";
            ld.SectionInfo.Section = "5";
            ld.StartOffset.Offset = 0;
            ld.EndOffset.Offset = 0;

            generator.GenerateID(ld);

            //I'm writing the output string with spaces to understand the format of the underlying numbers
            result = ("379 00 1456 03 008 08 05 01 0000000 0000000").Replace(" ", string.Empty);
            // Assert.IsTrue(generator.GenerateOffset("5.25") == "0005" + "250");

            Assert.IsTrue(generator.GenerateID(ld) == result); 
        }

    }


}
