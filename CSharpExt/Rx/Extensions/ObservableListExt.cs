using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ObservableListExt
    {
        public static IObservable<IChangeSet<TObject>> DistinctValues<TObject>(this IObservable<IChangeSet<TObject>> source)
        {
            return DynamicData.ObservableListEx.DistinctValues(source, i => i);
        }
    }
}
