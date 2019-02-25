using DynamicData;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public interface IObservableSetList<T> : IObservableList<T>, IHasBeenSetItemRxGetter<IEnumerable<T>>, IReadOnlyList<T>

    {
        new int Count { get; }
    }
}
