using System;
using System.Collections.Generic;
using System.Text;
using DescLogicFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DescLogicFrameworkTests
{
    [TestClass]
    public class LithologyLookupTests
    {
        [TestMethod]
        public void VerifyLithologyContainsMeasurement()
        {
            LithologicDescription description = new LithologicDescription();
            Measurement measurement = new Measurement();

            description.StartOffset.Offset = 0;
            description.EndOffset.Offset = 10;

            SectionInfo section = new SectionInfo();
            section.Expedition = "356";
            section.Site = "U1500";
            section.Hole = "A";
            section.Core = "10";
            section.Type = "H";
            section.Section = "3";

            measurement.StartOffset.SectionInfo = section;
            measurement.EndOffset.SectionInfo = section;
            description.SectionInfo = section;

            measurement.StartOffset.Offset = 5;
            measurement.EndOffset.Offset = 5;

            Assert.IsTrue(description.Contains(measurement.StartOffset));
        }
    }  
}
