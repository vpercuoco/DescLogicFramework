using System;
using System.Collections.Generic;
using System.Text;



namespace DescLogicFramework
{
    /// <summary>
    /// Class to maintain a collection of file paths.
    /// </summary>
    public class FileCollection
    {
        public string ExportDirectory { get; set; }

        public FileCollection() { }

        public List<string> Filenames { get; } = new List<string>();

        /// <summary>
        /// Add a collection of filepaths from a folder directory.
        /// </summary>
        /// <param name="directory">The folder directory which stores the file collection</param>
        /// <param name="extension">The file extension. Only handles .csv</param>
        public void AddFiles(string directory, string extension)
        {
            RecursiveFileSearch search = new RecursiveFileSearch();
            foreach(var item in search.GetFiles(directory, extension))
            {
                Filenames.Add(item);
            }
        }

        /// <summary>
        /// Remove a filepath from the collection.
        /// </summary>
        public void RemoveFiles()
        {
            Filenames.Clear();
        }
    }
}
