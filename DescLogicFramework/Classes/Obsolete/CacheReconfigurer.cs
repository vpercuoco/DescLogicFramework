using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;


namespace DescLogicFramework
{
    public static class CacheReconfigurer
    {
        public static Dictionary<SectionInfo,Dictionary<string,LithologicDescription>> CreateDescriptionSearchHierarchy(Dictionary<string, LithologicDescription> Descriptions)
        {

        return Descriptions
                .GroupBy(x => x.Value.SectionInfo)
                .ToDictionary(x => x.Key,
                              x => x.ToDictionary(y => y.Value.LithologicID,
                                                y => y.Value));

        }

    }
}
