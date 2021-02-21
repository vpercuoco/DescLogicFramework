using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;


namespace DescLogicFramework.DataAccess
{
    /// <summary>
    /// Top level class for common methods for handling importing, manipulating and exporting Lithologic Descriptions and Measurements
    /// </summary>
    public abstract class DataFileWorkFlowHandler<T>
    {
        /// <summary>
        /// Import an IODP Data Table object
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected IODPDataTable ImportIODPDataTable(CSVReader dtReader)
        {

            using (DataTable dt = dtReader.Read())
            {
                IODPDataTable idt = new IODPDataTable(dt);
                return idt;
            }

        }

        public abstract void ImportMetaData(string[] metaData, T interval);

    }
}
