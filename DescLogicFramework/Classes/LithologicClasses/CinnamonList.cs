using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DescLogicFramework
{
    /// <summary>
    /// An object which stores a list of strings which are synonyms for the lookup value.
    /// </summary>
    public class CinnamonList
    {
        /// <summary>
        /// The name of the reference list in the CinnamonList.json file.
        /// </summary>
        public string ListName { get; }

        /// <summary>
        /// The reference list obtained from the CinnamonList.json file.
        /// </summary>
        public List<string> Parameters { get; } = new List<string>();

        /// <summary>
        /// Constructor for the CinnamonList. The listname parameter identifies the list to reference within the CinnamonLists.json file.
        /// </summary>
        /// <param name="listName"></param>
        public CinnamonList(string listName)
        {
            ListName = listName;
            //TODO: Add the location of the global list to the appsettings file
            string jsonFile = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Classes\GlobalLists\CinnamonLists.json");
            dynamic json = JObject.Parse(jsonFile);
            var listParameters = json[listName];
         
            Parameters = listParameters.ToObject<List<string>>();
        }

        /// <summary>
        /// Adds a string value to the CinnamonList
        /// </summary>
        public void Add(string name)
        {
            Parameters.Add(name);
        }
        /// <summary>
        /// Removes a string value from the CinnamonList
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            Parameters.Remove(name);
        }

        /// <summary>
        /// Determines whether a string appears in the CinnamonList.
        /// </summary>
        public bool FindInList(string query)
        {
            foreach (var name in Parameters)
            {
                if (query == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if any string within a list of strings appears in the CinnamonList. Returns the first match.
        /// </summary>
        /// <param name="ColumnNames">A list of strings to search for in the CinnamonList</param>
        /// <returns>The first matched string</returns>
        public string FindInList(List<string> ColumnNames)
        {
            foreach (var name in Parameters)
            {
                foreach (string ColumnName in ColumnNames)
                {
                    if (name==ColumnName)
                    {
                        return name;
                    }
                }
            }
            return string.Empty;
        }

    }
}
