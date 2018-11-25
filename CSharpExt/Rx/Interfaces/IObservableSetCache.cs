using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public interface IObservableSetCache<V, K> : IObservableCache<V, K>, IHasItemRxGetter<IEnumerable<V>>
    {
    }
}
