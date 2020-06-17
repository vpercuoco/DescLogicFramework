using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace DescLogicFramework
{

    /// <summary>
    /// Used to return the files within a nested folder structure.
    /// </summary>
    public class RecursiveFileSearch
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        /// <summary>
        /// Returns the files of a specific extension witin a directory
        /// </summary>
        /// <param name="directory">The directory in which to search.</param>
        /// <param name="extension">The file extension to search for.</param>
        /// <returns></returns>
        public string[] GetFiles(string directory, string extension)
        {
            //Start with drives if you have to search the entire computer
            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                //Here we skip the drive if it is not ready to by read. This
                //is not necessarily the appropriate action in all scenarios
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                //  System.IO.DirectoryInfo rootDir = di.RootDirectory;
               

                // WalkDirectoryTree(rootDir);

            }
            return Directory.GetFiles(directory, extension);

            /* //Write out all the files that could not be processed.
             Console.WriteLine("Files with restricted access");
             foreach (string s in log)
             {
                 Console.WriteLine(s);
             }
             //Keep the console window open in debug mode
             Console.WriteLine("Press any key");
             Console.ReadKey();*/
        }

        /// <summary>
        /// Steps through the folders within a directory tree.
        /// </summary>
        /// <param name="rootDir">The root drive</param>
        static void WalkDirectoryTree(System.IO.DirectoryInfo rootDir)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            //First, process all the files directly under this folder
            try
            {
                files = rootDir.GetFiles("*.*");

            }
            //This is thrown if even one of the files requires permissions greater
            //than the application provides.
            catch (UnauthorizedAccessException e)
            {
                //This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate you privelages and access the file again.
                log.Add(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    //In this example, we only access the existing FileInfo object. If we
                    //want to open, delete or modify the file, then
                    //a try-catch block is required here to handle the case
                    // where the file has been deleted sinces the call to TranverseTree()
                    Console.WriteLine(fi.FullName);
                }
                //Now find all the subdirectories under this directory.
                subDirs = rootDir.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    //Recursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }
        }
    }
}
