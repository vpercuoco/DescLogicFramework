using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DescLogicFramework
{

    /// <summary>
    /// Used to create unique identification numbers for lithologic descriptions
    /// </summary>
    public class LithologicIDGenerator
    {
        /// <summary>
        /// Creates a new LithologicIDGenerator
        /// </summary>
        public LithologicIDGenerator() { }

        /// <summary>
        /// Returns a string ID based off the hierarchal information of a lithologic description
        /// </summary>
        /// <param name="description">The lithologic description to identify</param>
        /// <returns>A string representing a LithologicID</returns>
        public string GenerateID(LithologicDescription description)
        {

            try
            {
                string LithologicID = GenerateExpedition(description.SectionInfo.Expedition).ToString() +
                                    GenerateExpeditionModifier(description.SectionInfo.Expedition).ToString() +
                                    GenerateSite(description.SectionInfo.Site).ToString() +
                                    GenerateHole(description.SectionInfo.Hole).ToString() +
                                    GenerateCore(description.SectionInfo.Core).ToString() +
                                    GenerateType(description.SectionInfo.Type).ToString() +
                                    GenerateSection(description.SectionInfo.Section).ToString() +
                                    GenerateSectionHalf(description.SectionInfo.Half) +
                                    GenerateOffset(description.StartOffset.ToString()).ToString() +
                                    GenerateOffset(description.EndOffset.ToString()).ToString();

                description.LithologicID = LithologicID;
                return LithologicID;
            }
            catch (Exception)
            {

                throw;
           
            }

            


        }
        /// <summary>
        /// Expedition number, it should be 3 digits
        /// </summary>
        /// <param name="Expedition"></param>
        /// <returns></returns>
        private string GenerateExpedition(string Expedition)
        {
            string output = Regex.Replace(Expedition.Trim(),"[^0-9.]","");
            return PadStringStart(output, 3);

        }
        /// <summary>
        /// Expedition letter modifier, should be 2 digits and equal to its position in the alphabet
        /// </summary>
        /// <param name="Expedition"></param>
        /// <returns></returns>
        private string GenerateExpeditionModifier(string Expedition)
        {
            string output = "";
            string modifier = Regex.Replace(Expedition,@"[^A-Z]+",String.Empty);
            if (modifier != "")
            {
              output = (char.ToUpper(modifier.ToCharArray()[0]) - 64).ToString();
            }
            else
            {
                output = "00";
            }

            return PadStringStart(output,2);
        }
        private string GenerateSite(string Site)
        {
            string output = "";
            output = Regex.Replace(Site.Trim(), "[^0-9.]", "");
            return PadStringStart(output, 4);
        }
        private string GenerateHole(string Hole)
        {
            string output = "";
            output = (char.ToUpper(Hole.ToCharArray()[0]) - 64).ToString();
            return PadStringStart(output, 2);
        }
        private string GenerateCore(string Core)
        {
            string output = "";
            output = Regex.Replace(Core.Trim(), "[^0-9.]", "");
            return PadStringStart(output, 3);
        }

        private string GenerateType(string Type)
        {
            string output = "";
            output = (char.ToUpper(Type.ToCharArray()[0]) - 64).ToString();
            return PadStringStart(output, 2);
        }
        /// <summary>
        /// The Section number, should be 2 digits. A Core Catcher ("CC") has a value of 99.
        /// </summary>
        /// <param name="Section"></param>
        /// <returns></returns>
        private string GenerateSection(string Section)
        {
            string output = "";

            if (Section.ToUpper() == "CC")
            {
                output = "99";
                return output;
            }
            else
            {
                output = Section;
                return PadStringStart(output, 2);
            }
        }
        /// <summary>
        /// Section half returns a 2 digit number based on letter position in the alphabet. Pading must happen before and affter....
        /// <param name="SectionHalf"></param>
        /// <returns></returns>
        private string GenerateSectionHalf(string SectionHalf)
        {
            string output = "";
            if (SectionHalf == null)
            {
                return "99";
            }

            if (SectionHalf.ToLower() == "w" || SectionHalf.ToLower() == "a")
            {

                output = (char.ToUpper(SectionHalf.ToCharArray()[0]) - 64).ToString();
                return PadStringStart(output, 2);
            }
            else if (SectionHalf.ToLower()=="PAL")
            {
                return "90";
            }
            else
            {
                return "91";
            }
            
        }
        private string GenerateOffset(string Offset)
        {
            string output = "";
            string o = Regex.Replace(Offset.Trim(), "[^0-9.]", "");

            if (o.Contains("."))
            {
                string firstPart = o.Split(".")[0];
                string secondPart = o.Split(".")[1];
                output = PadStringStart(firstPart, 4) + PadStringEnd(secondPart, 3);
            }
            else
            {
                output = PadStringStart(o, 4) + "000";
            }
            
            //Separate string into two substrings, then pad beginning of first string and end of second string.
            return output;

        }
        /// <summary>
        /// Add a certain number of 0's before a string. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string PadStringStart(string input, int desiredLength)
        {
            string pad = "";

            //Pad if it isnt 3 digits
            if (input.Length < desiredLength)
            {
                int i = desiredLength - input.Length;
                while (i > 0)
                {
                    pad = pad + "0";
                    i = i - 1;
                }
            }
            else if (input.Length > desiredLength)
            {
                return "Error";
            }

            return (pad + input);
        }
        /// <summary>
        /// Add a certain number of 0's after a string. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string PadStringEnd(string input, int desiredLength)
        {
            string pad = "";

            //Pad if it isnt the desired length
            if (input.Length < desiredLength)
            {
                int i = desiredLength - input.Length;
                while (i > 0)
                {
                    pad = pad + "0";
                    i = i - 1;
                }
            }
            else if (input.Length > desiredLength)
            {
                return "Error";
            }

            return (input + pad);
        }
    }
}
