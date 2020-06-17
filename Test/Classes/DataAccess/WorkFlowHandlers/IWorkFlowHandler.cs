using System;
using System.Collections.Generic;
using System.Text;
using DescLogicFramework;

namespace DescLogicFramework.DataAccess
{
    interface IWorkFlowHandler<T,T1>
    {
        public Cache<T,T1> Cache { get; set; }
        public Cache<T, T1> ImportCache();
        public void ExportCache(Cache<T,T1> cache);

    }
}
