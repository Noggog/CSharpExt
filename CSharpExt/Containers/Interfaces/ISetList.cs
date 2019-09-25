using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface ISetList<T> : IList<T>, IHasBeenSet, IReadOnlySetList<T>
    {
        new int Count { get; }
    }

    public interface IReadOnlySetList<out T> : IReadOnlyList<T>, IHasBeenSetGetter
    {
        new int Count { get; }
    }
}
