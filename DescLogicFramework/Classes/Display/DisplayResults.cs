using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{
    public static class DisplayResults
    { 
        public static void PrintMeasurementToConsole(Measurement measurement)
        {
            Console.WriteLine(string.Format("ID:{0}, System:{1}, Test:{2}, TextID:{3}", measurement.ID, measurement.InstrumentSystem, measurement.TestNumber, measurement.TextID));
        }
        public static void PrintSubintervalToConsole(LithologicSubinterval subinterval)
        {
            Console.WriteLine(string.Format("ID:{0}, LithologicSubID:{1}, StartOffset:{2}, EndOffset:{3}", subinterval.ID, subinterval.LithologicSubID, subinterval.StartOffset, subinterval.EndOffset));
        }
        public static void PrintDescriptionToConsole(LithologicDescription description)
        {
            Console.WriteLine(string.Format("ID:{0}, LithologicSubID:{1}, StartOffset:{2}, EndOffset:{3}", description.ID, description.LithologicID, description.StartOffset, description.EndOffset));
        }
    }
}
