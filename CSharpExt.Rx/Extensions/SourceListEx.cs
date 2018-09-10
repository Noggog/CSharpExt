using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicData
{
    public static class SourceListEx
    {
        public static void SetTo<T>(this SourceList<T> list, IEnumerable<T> items)
        {
            list.Edit((l) =>
            {
                l.SetTo(items);
            });
        }
    }
}
