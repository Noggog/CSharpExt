using DynamicData;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public interface ISourceSetCache<V, K> : ISourceCache<V, K>, IHasBeenSetItemRx<IEnumerable<V>>, IObservableSetCache<V, K>
    {
        void Edit(Action<ISourceUpdater<V, K>> updateAction, bool hasBeenSet);
    }
}
