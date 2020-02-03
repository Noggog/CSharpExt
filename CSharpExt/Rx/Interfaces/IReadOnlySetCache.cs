using CSharpExt.Rx;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface IReadOnlySetCache<TObject, TKey> : IReadOnlyCache<TObject, TKey>, IHasBeenSetItemRxGetter<IEnumerable<TObject>>
    {
    }
}
