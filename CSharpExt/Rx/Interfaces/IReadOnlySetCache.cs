using DynamicData;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpExt.Rx
{
    public interface IReadOnlySetCache<TObject, TKey> : IReadOnlyCache<TObject, TKey>, IHasBeenSetItemRxGetter<IEnumerable<TObject>>
    {
    }
}
