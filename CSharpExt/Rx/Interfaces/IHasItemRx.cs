using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public interface IHasBeenSetItemRx<T> : IHasItemRxGetter<T>, IHasBeenSetItem<T>
    {
    }

    public interface IHasItemRxGetter<T> : IHasBeenSetRx, IHasItemGetter<T>
    {
        IObservable<T> ItemObservable { get; }
    }

    public interface IHasBeenSetRx : IHasBeenSetGetter
    {
        IObservable<bool> HasBeenSetObservable { get; }
    }
}
