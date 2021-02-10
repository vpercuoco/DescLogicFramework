using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{

    /// <summary>
    /// An event args class to handle processing of all files from a single expedition
    /// </summary>
    public class BatchProcessCompleteEventArgs : EventArgs
    {
        public string Expedition { get; set; } = "None set!";
        public BatchProcessCompleteEventArgs(string expedition)
        {
            Expedition = expedition;
        }
    }
}
