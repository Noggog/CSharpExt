using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public interface IHasBeenSetItemRx<T> : IHasBeenSetItemRxGetter<T>, IHasBeenSetItem<T>
    {
    }

    public interface IHasBeenSetItemRxGetter<out T> : IHasBeenSetRx, IHasBeenSetItemGetter<T>
    {
        IObservable<T> ItemObservable { get; }
    }

    public interface IHasBeenSetRx : IHasBeenSetGetter
    {
        IObservable<bool> HasBeenSetObservable { get; }
    }
}
