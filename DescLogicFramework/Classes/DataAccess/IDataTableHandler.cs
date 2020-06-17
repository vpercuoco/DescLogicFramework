using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DescLogicFramework.DataAccess
{
    public interface IDataTableHandler
    {
        public DataTable Read();
        public void Write(DataTable dt);
        


    }
}
